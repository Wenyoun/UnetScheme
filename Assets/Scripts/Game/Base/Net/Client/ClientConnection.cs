using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zyq.Game.Base
{
    internal class ClientConnection : IClientConnection
    {
        public const int BUFFER_SIZE = 1024 * 1024;

        //端口
        private int mPort;
        
        //Host
        private string mHost;

        //处理相应事件的回调句柄
        private INetHandler mNetHandler;

        //对接收或者发送的信息再处理
        private PacketHandler mPacketHandler;

        //网络状态
        private volatile NetState mNetState;

        //当有消息需要发送的时候用来唤醒在此信号量上等待的线程
        private AutoResetEvent mSendEvent;

        //待发送消息队列
        private ConcurrentQueue<Packet> mSendQueue;

        //底层的Socket连接
        private KcpUdpClient mClient;

        //总共发送的字节数
        private int mTotalSendBytes;

        //总共接收的字节数
        private int mTotalRecvBytes;
        
        //连接超时时间
        private float mTimeout;

        //发送器
        private Sender mSender;

        //接收器
        private Receiver mReceiver;

        public ClientConnection()
        {
            mNetState = NetState.None;
            mSendEvent = new AutoResetEvent(false);
            mSendQueue = new ConcurrentQueue<Packet>();
            mSender = new Sender(this);
            mReceiver = new Receiver(this);
        }

        public PacketHandler GetPacketHandler()
        {
            return mPacketHandler;
        }

        public AutoResetEvent GetSendPacketEvent()
        {
            return mSendEvent;
        }

        public ConcurrentQueue<Packet> GetSendPacketQueue()
        {
            return mSendQueue;
        }

        public void AddSendBytes(int count)
        {
            mTotalSendBytes += count;
        }

        public void AddRecvBytes(int count)
        {
            mTotalRecvBytes += count;
        }

        public int GetTotalSendBytes()
        {
            return mTotalSendBytes;
        }

        public int GetTotalRecvBytes()
        {
            return mTotalRecvBytes;
        }

        public bool IsConnected()
        {
            return mClient != null && mClient.IsConnected;
        }

        public void Disconnect()
        {
            mTimeout = 0;
            mNetState = NetState.None;

            if (mClient != null)
            {
                mClient.Dispose();
                mClient = null;
            }

            mSendEvent.Set();

            while (mSendQueue.TryDequeue(out Packet packet)) ;
        }

        public void SetNetHandler(INetHandler netHandler)
        {
            mNetHandler = netHandler;
        }

        public void SetPacketHandler(PacketHandler packetHandler)
        {
            mPacketHandler = packetHandler;
        }

        public void Connect(string host, int port)
        {
            mHost = host;
            mPort = port;
            ConnectServer();
        }

        public void Reconnect()
        {
            ConnectServer();
        }

        public void Send(Packet packet)
        {
            if (IsConnected())
            {
                mSendQueue.Enqueue(packet);
                mSendEvent.Set();
            }
        }

        public void OnUpdate(float delta)
        {
            if (mClient == null || mNetState == NetState.None)
            {
                return;
            }

            switch (mNetState)
            {
                case NetState.Connecting:
                {
                    if (IsConnected())
                    {
                        mTimeout = 0;
                        mNetState = NetState.Connected;
                        StartWorkingThread();
                        mNetHandler?.OnConnectedSuccess();
                    }
                    else
                    {
                        mTimeout += delta;
                        if (mTimeout >= 5)
                        {
                            mTimeout = 0;
                            mNetState = NetState.ConnectedError;
                        }
                    }
                }
                    break;
                case NetState.Connected:
                {
                    if (!IsConnected())
                    {
                        mNetState = NetState.ConnectedError;
                    }
                }
                    break;
                case NetState.ConnectedError:
                {
                    Disconnect();
                    mNetHandler?.OnConnectedFailed();
                }
                    break;
            }
        }

        private void ConnectServer()
        {
            Validate();
            StartConnecting();
        }

        private void StartConnecting()
        {
            if (mNetState == NetState.Connecting || mNetState == NetState.Connected)
            {
                return;
            }

            Disconnect();
            mNetState = 0;
            mClient = new KcpUdpClient();
            mNetState = NetState.Connecting;
            mClient.Connect(mHost, mPort);
        }

        private void StartWorkingThread()
        {
            ThreadPool.QueueUserWorkItem(mSender.SendLooper, mClient);
            ThreadPool.QueueUserWorkItem(mReceiver.RecvLooper, mClient);
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(mHost))
            {
                throw new NullReferenceException("Host is empty!");
            }

            if (mPort <= 0)
            {
                throw new NullReferenceException("mPort <= 0!");
            }

            if (mNetHandler == null)
            {
                throw new NullReferenceException("NetHandler is null!");
            }

            if (mPacketHandler == null)
            {
                throw new NullReferenceException("PacketHandler is null!");
            }
        }
    }

    internal class Sender
    {
        private ClientConnection connection;

        public Sender(ClientConnection connection)
        {
            this.connection = connection;
        }

        public void SendLooper(object obj)
        {
            KcpUdpClient client = obj as KcpUdpClient;

            try
            {
                byte[] buffer = new byte[ClientConnection.BUFFER_SIZE];
                AutoResetEvent notifyEvent = connection.GetSendPacketEvent();
                PacketHandler packetHandler = connection.GetPacketHandler();
                ConcurrentQueue<Packet> queue = connection.GetSendPacketQueue();
                
                while (connection.IsConnected())
                {
                    if(queue.TryDequeue(out Packet packet) && packet.Cmd > 0)
                    {
                        int length = packetHandler.HandleSend(buffer, packet);
                        int sendLength = 0;//client.Send(buffer, 0, length);
                        connection.AddSendBytes(sendLength);
                    }
                    else
                    {
                        notifyEvent.WaitOne();
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[Sender]: " + e.ToString());
            }
            finally
            {
                client.Dispose();
            }
        }
    }

    internal class Receiver
    {
        private ClientConnection connection;

        public Receiver(ClientConnection connection)
        {
            this.connection = connection;
        }

        public void RecvLooper(object obj)
        {
            KcpUdpClient client = obj as KcpUdpClient;

            try
            {
                int size = 0;
                int index = 0;
                int offset = 0;
                List<Packet> packets = new List<Packet>();
                byte[] buffer = new byte[ClientConnection.BUFFER_SIZE];

                PacketHandler handler = connection.GetPacketHandler();

                int headLength = PacketHandler.HeadLength;
                while (connection.IsConnected())
                {
                    int count = 1;//client.Recv(buffer, index, ClientConnection.BUFFER_SIZE - index);

                    if (count <= 0)
                    {
                        continue;
                    }

                    size += count;
                    index += count;

                    connection.AddRecvBytes(count);

                    while (size > headLength && offset < index)
                    {
                        int length = handler.ParseHeadLength(buffer, offset);

                        if (size - headLength >= length)
                        {
                            size -= headLength;
                            offset += headLength;

                            Packet packet = handler.HandleRecv(buffer, offset, length);
                            if (packet.Cmd > 0)
                            {
                                packets.Add(packet);
                            }

                            size -= length;
                            offset += length;

                            if (size == 0)
                            {
                                index = 0;
                                offset = 0;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (packets.Count > 0)
                    {
                    }

                    packets.Clear();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[Receiver]: " + e.ToString());
            }
            finally
            {
                client.Dispose();
                connection.GetSendPacketEvent().Set();
            }
        }
    }
}