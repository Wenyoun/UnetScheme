﻿using System;
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
        
        private ConcurrentQueue<Packet> sendPacketQueue;
        private ConcurrentQueue<Packet> recvPacketQueue;

        public KcpUdpClient()
        {
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
            get { return !isDispose && con != null ? con.ConId : 0; }
        }

        public bool IsConnected
        {
            get { return !isDispose && con != null && status == Success; }
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
            SocketDispose();
            
            sendPacketQueue.Clear();
            sendPacketQueue = null;
            
            recvPacketQueue.Clear();
            recvPacketQueue = null;
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
                    KcpHelper.Encode32u(buffer, 0, KcpConstants.ConnectFlag);
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
                        if (flag == KcpConstants.ConnectFlag)
                        {
                            con = new KcpConn(conv, socket);
                            con.Input(buffer, 0, count);
                            
                            count = con.Recv(buffer, 0, buffer.Length);
                            if (count == 8)
                            {
                                KcpHelper.Encode32u(buffer, 0, KcpConstants.ConnectFlag);
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
                Dispose();
                Debug.LogError(e.ToString());
            }
        }

        private void UpdateKcpLooper(object obj)
        {
            try
            {
                ClientDataProcessingCenter process = new ClientDataProcessingCenter();
                while (!isDispose && con != null && status == Success)
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
            finally
            {
                Dispose();
            }
        }

        private void RecvUdpDataLooper(object obj)
        {
            try
            {
                ClientDataProcessingCenter process = new ClientDataProcessingCenter();
                byte[] buffer = new byte[KcpConstants.Length];
                while (!isDispose && con != null && status == Success)
                {
                    if (!socket.Poll(100000, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    CheckDispose();

                    int count = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
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
            finally
            {
                Dispose();
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