using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public enum NetState
    {
        //未开始连接
        None = 0,

        //连接中
        Connecting,

        //连接失败
        ConnectedError,

        //连接成功
        Connected,
    }

    public interface IPacketDispatcherHandler
    {
        void OnUpdate();
        
        void Add(List<Packet> packets);
    }

    public interface INetHandler
    {
        //连接成功
        void OnConnectedSuccess();

        //连接失败
        void OnConnectedFailed();
    }

    public interface IClientConnection
    {
        void SetNetHandler(INetHandler netHandler);

        void SetPacketHandler(PacketHandler packetHandler);

        bool IsConnected();

        void Disconnect();

        void Connect(string host, int port);

        void Reconnect();

        void Send(Packet packet);

        void OnUpdate(float delta);

        int GetTotalSendBytes();

        int GetTotalRecvBytes();
    }
}