using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;

namespace Base.Net.Impl
{
    public class KcpUdpServer : IDisposable
    {
        private Socket udp;
        private bool isDispose;
        private Action<KcpConn> connectCallback;
        private ConcurrentDictionary<long, KcpConn> cons;

        public KcpUdpServer()
        {
            isDispose = false;
            cons = new ConcurrentDictionary<long, KcpConn>();
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        ~KcpUdpServer()
        {
            Dispose();
        }

        public void Bind(int port)
        {
            udp.Bind(new IPEndPoint(IPAddress.Any, port));
            ThreadPool.QueueUserWorkItem(UpdateKcpLooper);
            ThreadPool.QueueUserWorkItem(RecvUpdDataLooper);
        }

        public void Dispose()
        {
            if (!isDispose)
            {
                isDispose = true;

                if (cons != null)
                {
                    foreach (KcpConn con in cons.Values)
                    {
                        con.Dispose();
                    }

                    cons.Clear();
                }

                if (udp != null)
                {
                    udp.Dispose();
                }
            }
        }

        public void SetConnectCallback(Action<KcpConn> callback)
        {
            connectCallback = callback;
        }

        private void RecvUpdDataLooper(object obj)
        {
            try
            {
                uint startConvId = 10000;
                byte[] buffer = new byte[KcpHelper.Length];
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                while (!isDispose)
                {
                    if (!udp.Poll(100000, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    CheckDispose();

                    int count = udp.ReceiveFrom(buffer, SocketFlags.None, ref remote);
                    long conId = CptConId(remote);
                    KcpConn con = null;
                    if (!cons.TryGetValue(conId, out con))
                    {
                        if (count == 4)
                        {
                            uint flag = KcpHelper.Decode32u(buffer, 0);
                            if (flag == KcpHelper.Flag)
                            {
                                uint conv = startConvId++;
                                con = new KcpConn(conId, conv, udp, remote.Create(remote.Serialize()));
                                if (cons.TryAdd(conId, con))
                                {
                                    KcpHelper.Encode32u(buffer, 0, KcpHelper.Flag);
                                    KcpHelper.Encode32u(buffer, 4, conv);
                                    con.Send(buffer, 0, 8);
                                }
                            }
                        }
                    }
                    else
                    {
                        con.Input(buffer, 0, count);

                        if (!con.IsConnected)
                        {
                            int size = con.Recv(buffer, 0, buffer.Length);

                            if (size == 8)
                            {
                                uint flag = KcpHelper.Decode32u(buffer, 0);
                                uint conv = KcpHelper.Decode32u(buffer, 4);
                                if (flag == KcpHelper.Flag && conv == con.Conv)
                                {
                                    con.IsConnected = true;
                                    connectCallback?.Invoke(con);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void UpdateKcpLooper(object obj)
        {
            try
            {
                while (!isDispose)
                {
                    DateTime now = DateTime.Now;
                    foreach (KcpConn con in cons.Values)
                    {
                        con.Update(now);
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private long CptConId(EndPoint endPoint)
        {
            return endPoint.GetHashCode() * 100000 + ((IPEndPoint) endPoint).Port;
        }

        private void CheckDispose()
        {
            if (isDispose)
            {
                throw new KcpServerException("KcpUdpServer server already dispose");
            }
        }
    }

    public class KcpServerException : Exception
    {
        public KcpServerException(string message) : base(message)
        {
        }
    }
}