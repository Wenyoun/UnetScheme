using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Base.Net.Impl
{
    public class KcpUdpClient : IDisposable
    {
        public enum ConnectStatus
        {
            Error,
            Timeout,
            Success
        }

        #region 连接状态

        private const byte None = 0;
        private const byte Error = 1;
        private const byte Timeout = 2;
        private const byte Success = 3;
        private const byte Connecting = 4;

        #endregion

        private Socket udp;
        private KcpConn con;
        private EndPoint point;
        private volatile byte status;
        private volatile bool isDispose;
        private AutoResetEvent notifyEvent;
        private Action<ConnectStatus> connectCallback;

        public KcpUdpClient()
        {
            status = None;
            isDispose = false;
            notifyEvent = new AutoResetEvent(false);
        }

        ~KcpUdpClient()
        {
            Dispose();
        }

        public void Connect(string host, int port, Action<ConnectStatus> connectCallback)
        {
            CheckDispose();

            if (status == Connecting)
            {
                return;
            }

            status = Connecting;
            this.connectCallback = connectCallback;
            point = new IPEndPoint(IPAddress.Parse(host), port);
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ThreadPool.QueueUserWorkItem(ConnectLooper);
        }

        public void Dispose()
        {
            if (!isDispose)
            {
                isDispose = true;

                if (con != null)
                {
                    con.Dispose();
                }

                if (udp != null)
                {
                    udp.Dispose();
                }

                if (notifyEvent != null)
                {
                    notifyEvent.Set();
                }

                status = None;
            }
        }

        public int Send(byte[] buffer, int offset, int length)
        {
            CheckDispose();
            return con.Send(buffer, offset, length);
        }

        public int Recv(byte[] buffer, int offset, int length)
        {
            CheckDispose();

            if (con.PeekSize() <= 0)
            {
                notifyEvent.WaitOne();
            }

            return con.Recv(buffer, offset, length);
        }

        private void ConnectLooper(object obj)
        {
            try
            {
                int time = 0;
                int timeout = 5000;
                udp.Connect(point);
                byte[] buffer = new byte[1024];
                while (!isDispose && status == Connecting)
                {
                    int count = KcpHelper.Encode32u(buffer, 0, KcpHelper.Flag);
                    udp.Send(buffer, 0, count, SocketFlags.None);
                    if (!udp.Poll(100000, SelectMode.SelectRead))
                    {
                        time += 100;
                        if (time > timeout)
                        {
                            status = Timeout;
                            connectCallback?.Invoke(ConnectStatus.Timeout);
                            break;
                        }

                        continue;
                    }

                    CheckDispose();

                    count = udp.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    if (count == 32)
                    {
                        uint flag = KcpHelper.Decode32u(buffer, 24);
                        uint conv = KcpHelper.Decode32u(buffer, 28);
                        if (flag == KcpHelper.Flag)
                        {
                            con = new KcpConn(conv, udp);

                            con.Input(buffer, 0, count);
                            count = con.Recv(buffer, 0, buffer.Length);
                            if (count == 8)
                            {
                                KcpHelper.Encode32u(buffer, 0, KcpHelper.Flag);
                                KcpHelper.Encode32u(buffer, 4, conv);
                                con.Send(buffer, 0, 8);
                                con.Update(DateTime.Now);

                                status = Success;
                                ThreadPool.QueueUserWorkItem(UpdateLooper);
                                ThreadPool.QueueUserWorkItem(RecvUpdDataLooper);

                                connectCallback?.Invoke(ConnectStatus.Success);

                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                status = Error;
                connectCallback?.Invoke(ConnectStatus.Error);
                Debug.Log(e.ToString());
            }
        }

        private void UpdateLooper(object obj)
        {
            try
            {
                while (!isDispose && status == Success)
                {
                    con.Update(DateTime.Now);
                    Thread.Sleep(10);
                }
            }
            catch (Exception e)
            {
                status = Error;
                notifyEvent.Set();
                Debug.LogError(e.ToString());
            }
        }

        private void RecvUpdDataLooper(object obj)
        {
            try
            {
                byte[] buffer = new byte[1500];
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
                        notifyEvent.Set();
                    }
                }
            }
            catch (Exception e)
            {
                status = Error;
                notifyEvent.Set();
                Debug.LogError(e.ToString());
            }
        }

        private void CheckDispose()
        {
            if (status == Error)
            {
                throw new KcpClientException("KcpUdpClient client have a error");
            }

            if (isDispose)
            {
                throw new KcpClientException("KcpUdpClient client already dispose");
            }
        }

        #region Properties

        public bool IsConnected
        {
            get { return status == Success; }
        }

        #endregion
    }

    public class KcpClientException : Exception
    {
        public KcpClientException(string message) : base(message)
        {
        }
    }
}