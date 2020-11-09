using System;
using UnityEngine;

namespace Zyq.Game.Base
{
    public class ClientChannel : AbsChannel
    {
        private byte status;
        private KcpUdpClient kcpUdpClient;
        private IClientCallback clientCallback;

        public ClientChannel(IClientCallback callback)
        {
            clientCallback = callback;
            status = KcpUdpClient.None;
        }

        public override long ChannelId
        {
            get { return kcpUdpClient.ConId; }
        }

        public override bool IsConnected
        {
            get { return kcpUdpClient != null && kcpUdpClient.IsConnected; }
        }

        public override void Send(ushort cmd, ByteBuffer buffer)
        {
            if (kcpUdpClient != null)
            {
                kcpUdpClient.Send(new Packet(cmd, buffer));
            }
        }

        public override void Dispose()
        {
            Disconnect();
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

            HandlePacket();
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

        private void HandlePacket()
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