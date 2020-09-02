using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Base.Net.Impl
{
    public class KcpUdpClient : IDisposable
    {
        #region 连接状态

        public const byte None = 0;

        //连接中
        public const byte Connecting = 1;

        //连接超时
        public const byte ConnectingTimeout = 2;

        //连接成功
        public const byte ConnectingSuccess = 3;
        
        //Socket错误
        public const byte SocketError = 4;

        #endregion

        private Socket udp;
        private KcpConn con;
        private byte status;
        private EndPoint point;
        private bool isDispose;

        public KcpUdpClient()
        {
            isDispose = false;
        }

        public void Connect(string host, int port)
        {
            if (status == Connecting)
            {
                return;
            }
            
            status = Connecting;
            point = new IPEndPoint(IPAddress.Parse(host), port);
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ThreadPool.QueueUserWorkItem(ConnectLooper);
        }

        public void Dispose()
        {
            isDispose = true;
            
            if (con != null)
            {
                con.Dispose();
                con = null;
            }
            
            if (udp != null)
            {
                udp.Dispose();
                udp = null;
            }
            
            status = 0;
            point = null;
        }
        
        public int Send(byte[] buffer, int offset, int length)
        {
            if (con != null)
            {
                return con.Send(buffer, offset, length);
            }
            return -20;
        }

        public int Recv(byte[] buffer, int offset, int length)
        {
            if (con != null)
            {
                return con.Recv(buffer, offset, length);
            }

            return -20;
        }

        private void ConnectLooper(object obj)
        {
            try
            {
                int time = 0;
                int timeout = 5000;
                udp.Connect(point);
                byte[] buffer = new byte[1024];
                while (!isDispose && udp != null && status == Connecting)
                {
                    int size = KcpHelper.Encode32u(buffer, 0, KcpHelper.Flag);
                    udp.Send(buffer, 0, size, SocketFlags.None);
                    if (!udp.Poll(100000, SelectMode.SelectRead))
                    {
                        time += 100;
                        if (time > timeout)
                        {
                            status = ConnectingTimeout;
                            break;
                        }

                        continue;
                    }

                    if (isDispose)
                    {
                        throw new ObjectDisposedException("KcpUdpClient already dispose!");
                    }

                    int count = udp.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    if (count == 28)
                    {
                        uint conv = KcpHelper.Decode32u(buffer, 24);
                        con = new KcpConn(conv, udp);
                        status = ConnectingSuccess;
                        ThreadPool.QueueUserWorkItem(UpdateLooper);
                        ThreadPool.QueueUserWorkItem(RecvUpdDataLooper);
                        ThreadPool.QueueUserWorkItem(RecvKcpDataLooper);
                        con.Input(buffer, 0, count);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                status = SocketError;
                Debug.Log(e.ToString());
            }
        }

        private void UpdateLooper(object obj)
        {
            try
            {
                while (!isDispose && udp != null && status == ConnectingSuccess)
                {
                    con.Update(DateTime.Now);
                    Thread.Sleep(10);
                }
            }
            catch (Exception e)
            {
                status = SocketError;
                Debug.Log(e.ToString());
            }
        }

        private void RecvUpdDataLooper(object obj)
        {
            try
            {
                byte[] buffer = new byte[2048];
                int size = KcpHelper.Encode32u(buffer, 0, KcpHelper.Flag);
                con.Send(buffer, 0, size);
                while (!isDispose && udp != null && status == ConnectingSuccess)
                {
                    if (!udp.Poll(100000, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    if (isDispose)
                    {
                        throw new ObjectDisposedException("KcpUdpClient already dispose!");
                    }
                    
                    int count = udp.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    con.Input(buffer, 0, count);
                }
            }
            catch (Exception e)
            {
                status = SocketError;
                Debug.Log(e.ToString());
            }
        }

        private void RecvKcpDataLooper(object obj)
        {
            byte[] buffer = new byte[2048]; 
            byte[] data = System.Text.Encoding.UTF8.GetBytes("yinhuayong");
            con.Send(data, 0, data.Length);
            try
            {
                while (!isDispose && udp != null && status == ConnectingSuccess)
                {
                    int count;
                    do
                    {
                        count = Recv(buffer, 0, buffer.Length);
                        if (count < 0)
                        {
                            break;
                        }

                        con.Send(buffer, 0, count);
                    } while (count > 0);

                    Thread.Sleep(10);
                }
            }
            catch (Exception e)
            {
                status = SocketError;
                Debug.Log(e.ToString());
            }
        }

        #region Properties

        public byte Status
        {
            get { return status; }
        }

        public bool IsConnected
        {
            get { return status == ConnectingSuccess; }
        }

        #endregion
    }
}