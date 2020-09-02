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
        private ConcurrentDictionary<long, KcpConn> cons;

        public KcpUdpServer()
        {
            isDispose = false;
            cons = new ConcurrentDictionary<long, KcpConn>();
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Bind(int port)
        {
            udp.Bind(new IPEndPoint(IPAddress.Any, port));
            ThreadPool.QueueUserWorkItem(UpdateLooper);
            ThreadPool.QueueUserWorkItem(RecvUpdDataLooper);
            ThreadPool.QueueUserWorkItem(RecvKcpDataLooper);
        }

        public void Dispose()
        {
            isDispose = true;

            if (cons != null)
            {
                foreach (KcpConn con in cons.Values)
                {
                    con.Dispose();
                }

                cons.Clear();
                cons = null;
            }

            if (udp != null)
            {
                udp.Dispose();
                udp = null;
            }
        }

        private void RecvUpdDataLooper(object obj)
        {
            try
            {
                uint startConvId = 10000;
                byte[] buffer = new byte[2048];
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                while (!isDispose && udp != null)
                {
                    if (!udp.Poll(100000, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    if (isDispose)
                    {
                        throw new ObjectDisposedException("KcpUdpServer already dispose!");
                    }
                    
                    KcpConn con = null;
                    int count = udp.ReceiveFrom(buffer, SocketFlags.None, ref remote);
                    long conId = GetConId(remote);
                    if (!cons.TryGetValue(conId, out con))
                    {
                        if (count == 4)
                        {
                            uint flag = KcpHelper.Decode32u(buffer, 0);
                            if (flag == KcpHelper.Flag)
                            {
                                uint conv = startConvId++;
                                con = new KcpConn(conId, conv, udp, remote.Create(remote.Serialize()));
                                cons.TryAdd(conId, con);
                                
                                int size = KcpHelper.Encode32u(buffer, 0, conv);
                                con.Send(buffer, 0, size);
                            }
                        }
                    }
                    else
                    {
                        con.Input(buffer, 0, count);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private void UpdateLooper(object obj)
        {
            try
            {
                while (!isDispose && udp != null && cons != null)
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
                Debug.Log(e.ToString());
            }
        }

        private void RecvKcpDataLooper(object obj)
        {
            try
            {
                byte[] buffer = new byte[2048];
                while (!isDispose && udp != null && cons != null)
                {
                    foreach (KcpConn con in cons.Values)
                    {
                        int count = -1;
                        do
                        {
                            count = con.Recv(buffer, 0, buffer.Length);
                            if (count < 0)
                            {
                                break;
                            }

                            if (count == 4)
                            {
                                uint flag = KcpHelper.Decode32u(buffer, 0);
                                if (flag == KcpHelper.Flag)
                                {
                                    Debug.Log("Server: 连接确认建立..." + cons.Count);
                                }
                            }
                            else
                            {
                                con.Send(buffer, 0, count);
                            }
                        } while (count > 0);
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private long GetConId(EndPoint endPoint)
        {
            return endPoint.GetHashCode() * 100000 + ((IPEndPoint) endPoint).Port;
        }
    }
}