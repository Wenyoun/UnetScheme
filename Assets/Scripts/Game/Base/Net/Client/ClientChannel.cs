using System;
using UnityEngine;

namespace Zyq.Game.Base
{
    public class ClientChannel : AbstractChannel
    {
        private byte status;
        private IClient clientCallback;
        private KcpUdpClient kcpUdpClient;

        public ClientChannel(IClient client)
        {
            status = KcpUdpClient.None;
            kcpUdpClient = null;
            clientCallback = client;
        }

        public override long ChannelId
        {
            get { return kcpUdpClient != null ? kcpUdpClient.ConId : -1; }
        }

        public override bool IsConnected
        {
            get { return kcpUdpClient != null && kcpUdpClient.IsConnected; }
        }

        public override void Send(Packet packet)
        {
            if (kcpUdpClient != null)
            {
                kcpUdpClient.Send(packet);
            }
        }

        public override void Dispose()
        {
            Disconnect();
            clientCallback = null;
            base.Dispose();
        }

        public override void Disconnect()
        {
            ClearHandlers();
            status = KcpUdpClient.None;
            if (kcpUdpClient != null)
            {
                kcpUdpClient.Dispose();
                kcpUdpClient = null;
            }
        }

        public override void Dispatcher()
        {
            if (kcpUdpClient == null)
            {
                return;
            }

            HandlerPacket();
            CheckConnectStatus();
        }

        public void Connect(string host, int port)
        {
            if (status == KcpUdpClient.Connecting)
            {
                return;
            }

            status = KcpUdpClient.Connecting;
            kcpUdpClient = new KcpUdpClient();
            kcpUdpClient.Connect(host, port);
        }

        private void HandlerPacket()
        {
            try
            {
                if (handlers != null && kcpUdpClient != null)
                {
                    if (kcpUdpClient.Recv(out Packet packet))
                    {
                        ushort cmd = packet.Cmd;
                        ByteBuffer byteBuffer = packet.Buffer;
                        if (handlers.TryGetValue(packet.Cmd, out ChannelMessageDelegate handler))
                        {
                            handler(new ChannelMessage(cmd, byteBuffer, this));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void CheckConnectStatus()
        {
            if (status == KcpUdpClient.None)
            {
                return;
            }

            if (status == KcpUdpClient.Connecting)
            {
                if (kcpUdpClient.Status == KcpUdpClient.Success)
                {
                    status = KcpUdpClient.Success;
                    clientCallback.OnServerConnect(this);
                }
                else if (kcpUdpClient.Status == KcpUdpClient.Error)
                {
                    status = KcpUdpClient.None;
                    clientCallback.OnServerDisconnect(this);
                    Disconnect();
                }
            }
            else if (status == KcpUdpClient.Error)
            {
                status = KcpUdpClient.None;
                clientCallback.OnServerDisconnect(this);
                Disconnect();
            }
            else if (status == KcpUdpClient.Success)
            {
                if (!kcpUdpClient.IsConnected)
                {
                    status = KcpUdpClient.None;
                    clientCallback.OnServerDisconnect(this);
                    Disconnect();
                }
            }
        }
    }
}