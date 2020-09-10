using System;
using System.Threading;
using System.Collections.Generic;
using Base.Net.Impl;

namespace Zyq.Game.Client
{
    public class UdpConnection : IConnection
    {
        public const int BUFFER_SIZE = 1024 * 1024;

        //IP
        private string mHost;

        //端口
        private int mPort;

        //处理相应事件的回调句柄
        private INetHandler mNetHandler;

        //对接收或者发送的信息再处理
        private IPacketHandler mPacketHandler;

        //当接收到一个包后在unity main线程中dispathcer出去
        private IPacketDispatcherHandler mPacketDispatcherHandler;

        //网络状态
        private volatile NetState mNetState;

        //当有消息需要发送的时候用来唤醒在此信号量上等待的线程
        private AutoResetEvent mSendPacketEvent;

        //同步不同线程之前对mSendPacketQueues的操作
        private System.Object mSendPacketLock;

        //待发送消息队列
        private Queue<Packet> mSendPacketQueue;

        //底层的Socket连接
        private KcpUdpClient mClient;

        //总共发送的字节数
        private int mTotalSendBytes;

        //总共接收的字节数
        private int mTotalRecvBytes;

        //发送器
        private Sender mSender;

        //接收器
        private Receiver mReceiver;

        public UdpConnection()
        {
            mClient = null;
            mNetState = NetState.None;
            mSendPacketEvent = new AutoResetEvent(false);
            mSendPacketLock = new System.Object();
            mSendPacketQueue = new Queue<Packet>();
            mSender = new Sender(this);
            mReceiver = new Receiver(this);
        }

        public INetHandler GetNetHandler()
        {
            return mNetHandler;
        }

        public IPacketHandler GetPacketHandler()
        {
            return mPacketHandler;
        }

        public IPacketDispatcherHandler GetPacketDispatcherHandler()
        {
            return mPacketDispatcherHandler;
        }

        public NetState GetNetState()
        {
            return mNetState;
        }

        public void SetCurrentState(NetState currentState)
        {
            mNetState = currentState;
        }

        public AutoResetEvent GetSendPacketEvent()
        {
            return mSendPacketEvent;
        }

        public System.Object GetSendPacketLock()
        {
            return mSendPacketLock;
        }

        public Queue<Packet> GetSendPacketQueue()
        {
            return mSendPacketQueue;
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

        public void DisConnect()
        {
            mNetState = NetState.None;

            if (mClient != null)
            {
                mClient.Dispose();
                mClient = null;
            }

            mSendPacketEvent.Set();

            lock (mSendPacketLock)
            {
                mSendPacketQueue.Clear();
            }
        }

        public void SetNetHandler(INetHandler netHandler)
        {
            mNetHandler = netHandler;
        }

        public void SetPacketHandler(IPacketHandler packetHandler)
        {
            mPacketHandler = packetHandler;
        }

        public void SetPacketDispatcherHandler(IPacketDispatcherHandler packetDispatcherHandler)
        {
            mPacketDispatcherHandler = packetDispatcherHandler;
        }

        public void Connect(string host, int port)
        {
            mHost = host;
            mPort = port;
            ConnectServer();
        }

        public void ReConnect()
        {
            ConnectServer();
        }

        public void Send(Packet packet)
        {
            if (IsConnected())
            {
                lock (mSendPacketLock)
                {
                    mSendPacketQueue.Enqueue(packet);
                }

                mSendPacketEvent.Set();
            }
        }

        public bool OnUpdate(float delta)
        {
            if (mPacketDispatcherHandler != null)
            {
                mPacketDispatcherHandler.Dispatcher();
            }

            if (mClient == null || mNetState == NetState.None)
            {
                return false;
            }

            switch (mNetState)
            {
                case NetState.Connecting:
                {
                    if (IsConnected())
                    {
                        mNetState = NetState.Connected;
                        StartWorkingThread();
                        mNetHandler.OnConnectedSuccess();
                    }
                }
                    break;
                case NetState.ConnectedError:
                {
                    DisConnect();
                    mNetHandler.OnConnectedFailed();
                }
                    break;
                case NetState.ConnectedTimeout:
                {
                    DisConnect();
                    mNetHandler.OnConnectedTimeout();
                }
                    break;
                case NetState.Connected:
                {
                    if (!IsConnected())
                    {
                        mNetState = NetState.Disconneted;
                    }
                }
                    break;
                case NetState.Disconneted:
                {
                    DisConnect();
                    mNetHandler.OnDisConnected();
                }
                    break;
            }

            return mNetState == NetState.Connected;
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

            DisConnect();
            mClient = new KcpUdpClient();
            mNetState = NetState.Connecting;
            mClient.Connect(mHost, mPort, (KcpUdpClient.ConnectStatus status) =>
            {
                if (status == KcpUdpClient.ConnectStatus.Error)
                {
                    mNetState = NetState.ConnectedError;
                }
                else if(status == KcpUdpClient.ConnectStatus.Timeout)
                {
                    mNetState = NetState.ConnectedTimeout;
                }
                else if (status == KcpUdpClient.ConnectStatus.Success)
                {
                    mNetState = NetState.Connected;
                }
            });
        }

        private void StartWorkingThread()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(mSender.SendLooper), mClient);
            ThreadPool.QueueUserWorkItem(new WaitCallback(mReceiver.RecvLooper), mClient);
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

            if (mPacketDispatcherHandler == null)
            {
                throw new NullReferenceException("mPacketDispatcherHandler is null!");
            }
        }
    }

    public class Sender
    {
        private byte[] mSendBuffer;
        private UdpConnection mTcpConnection;

        public Sender(UdpConnection connection)
        {
            mTcpConnection = connection;
            mSendBuffer = new byte[UdpConnection.BUFFER_SIZE];
        }

        public void SendLooper(object obj)
        {
            KcpUdpClient client = obj as KcpUdpClient;

            try
            {
                while (mTcpConnection.IsConnected())
                {
                    Packet packet = null;

                    lock (mTcpConnection.GetSendPacketLock())
                    {
                        Queue<Packet> queue = mTcpConnection.GetSendPacketQueue();
                        if (queue.Count > 0)
                        {
                            packet = queue.Dequeue();
                        }
                    }

                    if (packet != null)
                    {
                        int length = mTcpConnection.GetPacketHandler().HandlerSend(mSendBuffer, packet);

                        int sendLength = client.Send(mSendBuffer, 0, length);

                        mTcpConnection.AddSendBytes(sendLength);
                    }
                    else
                    {
                        mTcpConnection.GetSendPacketEvent().WaitOne();
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("<-[Sender]->" + e.ToString());
            }
            finally
            {
                client.Dispose();
            }
        }
    }

    public class Receiver
    {
        private byte[] mRecvBuffer;
        private int mOffset;
        private int mIndex;
        private int mCount;
        private List<Packet> mPackets;
        private UdpConnection mTcpConnection;

        public Receiver(UdpConnection connection)
        {
            mTcpConnection = connection;
            mRecvBuffer = new byte[UdpConnection.BUFFER_SIZE];
            mOffset = 0;
            mIndex = 0;
            mCount = 0;
            mPackets = new List<Packet>();
        }

        public void RecvLooper(object obj)
        {
            KcpUdpClient client = obj as KcpUdpClient;

            try
            {
                mOffset = 0;
                mIndex = 0;
                mCount = 0;
                mPackets.Clear();

                int headerLength = mTcpConnection.GetPacketHandler().GetHeaderLength();

                IPacketHandler handler = mTcpConnection.GetPacketHandler();

                IPacketDispatcherHandler dispatcher = mTcpConnection.GetPacketDispatcherHandler();

                while (mTcpConnection.IsConnected())
                {
                    int count = client.Recv(mRecvBuffer, mIndex, mRecvBuffer.Length - mIndex);

                    if (count == 0)
                    {
                        continue;
                    }

                    mCount += count;
                    mIndex += count;

                    mTcpConnection.AddRecvBytes(count);

                    while (mCount > headerLength && mOffset < mIndex)
                    {
                        int length = handler.ParseRecvHeaderLength(mRecvBuffer, mOffset);

                        if (mCount - headerLength >= length)
                        {
                            mCount -= headerLength;
                            mOffset += headerLength;

                            Packet packet = handler.HandlerRecv(mRecvBuffer, mOffset, length);
                            if (packet != null)
                            {
                                mPackets.Add(packet);
                            }

                            mCount -= length;
                            mOffset += length;

                            if (mCount == 0)
                            {
                                mIndex = 0;
                                mOffset = 0;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (mPackets.Count > 0 && dispatcher != null)
                    {
                        dispatcher.Add(mPackets);
                    }

                    mPackets.Clear();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("<-[Receiver]->" + e.ToString());
            }
            finally
            {
                client.Dispose();
                mTcpConnection.GetSendPacketEvent().Set();
            }
        }
    }
}