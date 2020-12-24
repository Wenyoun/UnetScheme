using System;
using System.Collections.Generic;
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

        private KcpConn con;
        private Socket socket;

        private byte status;
        private bool isDispose;

        private ClientHeartbeatProcessing heartbeat;
        private ConcurrentQueue<Packet> sendPacketQueue;
        private ConcurrentQueue<Packet> recvPacketQueue;

        public KcpUdpClient()
        {
            status = None;
            isDispose = false;

            heartbeat = new ClientHeartbeatProcessing();
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

            sendPacketQueue.Clear();
            recvPacketQueue.Clear();

            status = Connecting;

            SocketDispose();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            KcpHelper.CreateThread(ConnectLooper, new IPEndPoint(IPAddress.Parse(host), port));
        }

        public void Send(Packet packet)
        {
            if (isDispose)
            {
                return;
            }

            sendPacketQueue.Enqueue(packet);
        }

        public bool Recv(out Packet packet)
        {
            if (isDispose)
            {
                packet = new Packet();
                return false;
            }

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
            lock (this)
            {
                if (isDispose)
                {
                    return;
                }

                isDispose = true;
            }

            status = None;
            con?.Dispose();
            SocketDispose();
            sendPacketQueue.Clear();
            recvPacketQueue.Clear();
        }

        private void ConnectLooper(object obj)
        {
            try
            {
                int time = 0;
                int timeout = 5000;
                socket.Connect((EndPoint) obj);
                byte[] buffer = new byte[KcpConstants.Length];
                while (!isDispose && status == Connecting)
                {
                    KcpHelper.Encode32u(buffer, 0, KcpConstants.Flag_Connect);
                    socket.Send(buffer, 0, 4, SocketFlags.None);
                    if (!socket.Poll(100000, SelectMode.SelectRead))
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

                    int count = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    if (count == 32)
                    {
                        uint flag = KcpHelper.Decode32u(buffer, 24);
                        uint conv = KcpHelper.Decode32u(buffer, 28);
                        if (flag == KcpConstants.Flag_Connect)
                        {
                            con = new KcpConn(conv, socket);
                            con.Input(buffer, 0, count);

                            count = con.Recv(buffer, 0, buffer.Length);
                            if (count == 8)
                            {
                                KcpHelper.Encode32u(buffer, 0, KcpConstants.Flag_Connect);
                                KcpHelper.Encode32u(buffer, 4, conv);
                                con.Send(buffer, 0, 8);
                                con.Flush();

                                status = Success;

                                KcpHelper.CreateThread(UpdateKcpLooper);
                                KcpHelper.CreateThread(RecvUdpDataLooper);
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

                    process.TryParseSendKcpData(con, sendPacketQueue);

                    con.Update(DateTime.Now);

                    Thread.Sleep(5);

                    heartbeat.Tick(this, con);
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
                List<Packet> packets = new List<Packet>();
                byte[] rawBuffer = new byte[KcpConstants.Length];
                ClientDataProcessingCenter process = new ClientDataProcessingCenter();

                while (!isDispose && status == Success)
                {
                    if (!socket.Poll(100000, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    CheckDispose();

                    int count = socket.Receive(rawBuffer, 0, rawBuffer.Length, SocketFlags.None);
                    if (count > 0)
                    {
                        con.Input(rawBuffer, 0, count);

                        if (process.TryParseRecvKcpData(con, packets, heartbeat))
                        {
                            int length = packets.Count;
                            if (length > 0)
                            {
                                for (int i = 0; i < length; ++i)
                                {
                                    recvPacketQueue.Enqueue(packets[i]);
                                }

                                packets.Clear();
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

        private void SocketDispose()
        {
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
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