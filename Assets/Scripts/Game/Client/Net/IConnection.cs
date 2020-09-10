using System;
using System.Collections.Generic;

namespace Zyq.Game.Client
{
    public enum NetState
    {
        //未开始连接
        None = 0,
        //连接中
        Connecting,
        //连接失败
        ConnectedError,
        //连接超时
        ConnectedTimeout,
        //连接成功
        Connected,
        //连接断开
        Disconneted
    }

    public class Packet : IDisposable
    {
        public Packet( int cmd , byte[] buffer )
        {
            Cmd = cmd;
            Buffer = buffer;
        }

        public int Cmd
        {
            private set;
            get;
        }

        public byte[] Buffer
        {
            private set;
            get;
        }

        public void Dispose()
        {
            Cmd = 0;
            Buffer = null;
        }
    }

    public interface IPacketDispatcherHandler
    {
        void Add( Packet packet );

        void Add( List<Packet> packets );

        void Dispatcher();
    }

    public interface IPacketHandler
    {
        int GetHeaderLength();

        int ParseRecvHeaderLength( byte[] data , int offset );

        int HandlerSend( byte[] buf , Packet packet );

        Packet HandlerRecv( byte[] data , int offset , int size );
    }

    public interface INetHandler
    {
        //连接成功
        void OnConnectedSuccess();

        //接收包
        void OnRecvPacket( int cmd , byte[] buffer );

        //如果超过GetConnectingTimeout后未连接成功则连接超时
        void OnConnectedTimeout();

        //连接失败
        void OnConnectedFailed();

        //连接断开
        void OnDisConnected();
    }

    public interface IConnection
    {
        void SetNetHandler( INetHandler netHandler );

        void SetPacketHandler( IPacketHandler packetHandler );

        void SetPacketDispatcherHandler( IPacketDispatcherHandler packetDispatcher );

        bool IsConnected();

        void DisConnect();

        void Connect( string host , int port );

        void ReConnect();

        void Send( Packet packet );

        bool OnUpdate( float delta );

        int GetTotalSendBytes();

        int GetTotalRecvBytes();
    }
}