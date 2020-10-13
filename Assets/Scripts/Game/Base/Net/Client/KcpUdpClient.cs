using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Zyq.Game.Base
{
    public class KcpUdpClient : IDisposable
    {
        #region 连接状态

        public const byte None = 0;
        public const byte Error = 1;
        public const byte Timeout = 2;
        public const byte Success = 3;
        public const byte Connecting = 4;

        #endregion

        private Socket udp;
        private KcpConn con;
        private volatile byte status;
        private volatile bool isDispose;
        private ConcurrentQueue<Packet> sendPacketQueue;
        private ConcurrentQueue<Packet> recvPacketQueue;

        public KcpUdpClient()
        {
            udp = null;
            con = null;
            status = None;
            isDispose = false;
            sendPacketQueue = new ConcurrentQueue<Packet>();
            recvPacketQueue = new ConcurrentQueue<Packet>();
        }

        public void Connect(string host, int port)
        {
            CheckDispose();

            if (status == Connecting)
            {
                return;
            }

            ClearQueue(sendPacketQueue);
            ClearQueue(recvPacketQueue);
            status = Connecting;
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ThreadPool.QueueUserWorkItem(ConnectLooper, new IPEndPoint(IPAddress.Parse(host), port));
        }

        public void Send(Packet packet)
        {
            sendPacketQueue.Enqueue(packet);
        }

        public bool Recv(out Packet packet)
        {
            return recvPacketQueue.TryDequeue(out packet);
        }

        public long ConId
        {
            get { return con.ConId; }
        }

        public bool IsConnected
        {
            get { return status == Success; }
        }

        public byte Status
        {
            get { return status; }
        }

        public void Dispose()
        {
            if (!isDispose)
            {
                isDispose = true;
                status = None;

                if (udp != null)
                {
                    udp.Dispose();
                }

                if (con != null)
                {
                    con.Dispose();
                }

                ClearQueue(sendPacketQueue);

                ClearQueue(recvPacketQueue);
            }
        }

        private void ConnectLooper(object obj)
        {
            try
            {
                int time = 0;
                int timeout = 5000;
                udp.Connect((EndPoint) obj);
                byte[] buffer = new byte[KcpHelper.Length];
                while (!isDispose && status == Connecting)
                {
                    KcpHelper.Encode32u(buffer, 0, KcpHelper.ConnectFlag);
                    udp.Send(buffer, 0, 4, SocketFlags.None);
                    if (!udp.Poll(100000, SelectMode.SelectRead))
                    {
                        time += 100;
                        if (time > timeout)
                        {
                            status = Timeout;
                            break;
                        }

                        continue;
                    }

                    CheckDispose();

                    int count = udp.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    if (count == 32)
                    {
                        uint flag = KcpHelper.Decode32u(buffer, 24);
                        uint conv = KcpHelper.Decode32u(buffer, 28);
                        if (flag == KcpHelper.ConnectFlag)
                        {
                            con = new KcpConn(conv, udp);
                            con.Input(buffer, 0, count);
                            
                            count = con.Recv(buffer, 0, buffer.Length);
                            if (count == 8)
                            {
                                KcpHelper.Encode32u(buffer, 0, KcpHelper.ConnectFlag);
                                KcpHelper.Encode32u(buffer, 4, conv);
                                con.Send(buffer, 0, 8);
                                con.Flush();

                                status = Success;
                                ThreadPool.QueueUserWorkItem(UpdateKcpLooper);
                                ThreadPool.QueueUserWorkItem(RecvUdpDataLooper);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                status = Error;
                Debug.LogError(e.ToString());
            }
        }

        private void UpdateKcpLooper(object obj)
        {
            try
            {
                ClientDataProcessingCenter process = new ClientDataProcessingCenter();
                while (!isDispose && status == Success)
                {
                    CheckDispose();
                    process.TrySendKcpData(con, sendPacketQueue);
                    con.Update(DateTime.Now);
                    Thread.Sleep(5);
                }
            }
            catch (Exception e)
            {
                status = Error;
                Debug.LogError(e.ToString());
            }
        }

        private void RecvUdpDataLooper(object obj)
        {
            try
            {
                ClientDataProcessingCenter process = new ClientDataProcessingCenter();
                byte[] buffer = new byte[KcpHelper.Length];
                while (!isDispose && status == Success)
                {
                    if (!udp.Poll(100000, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    CheckDispose();

                    int count = udp.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    if (count > 0)
                    {
                        con.Input(buffer, 0, count);
                        if (process.TryRecvKcpData(con, out Packet packet))
                        {
                            recvPacketQueue.Enqueue(packet);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                status = Error;
                Debug.LogError(e.ToString());
            }
        }

        private void CheckDispose()
        {
            if (status == Error)
            {
                throw new KcpClientException("KcpUdpClient client error");
            }

            if (isDispose)
            {
                throw new KcpClientException("KcpUdpClient client already dispose");
            }
        }

        private void ClearQueue(ConcurrentQueue<Packet> queue)
        {
            while (queue.TryDequeue(out Packet packet))
            {
            }
        }

        private class KcpClientException : Exception
        {
            public KcpClientException(string message) : base(message)
            {
            }
        }
    }
}