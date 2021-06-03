using System;
using System.Buffers;

namespace Net.KcpImpl
{
    /// <summary>
    /// Kcp回调
    /// </summary>
    public interface IKcpCallback
    {
        /// <summary>
        /// kcp 发送方向输出
        /// </summary>
        /// <param name="buffer">kcp 交出发送缓冲区控制权，缓冲区来自<see cref="RentBuffer(int)"/></param>
        /// <param name="avalidLength">数据的有效长度</param>
        /// <returns>不需要返回值</returns>
        /// <remarks>通过增加 avalidLength 能够在协议栈中有效的减少数据拷贝</remarks>
        void Output(Memory<byte> memory, int length);
    }


    /// <summary>
    /// 外部提供缓冲区,可以在外部链接一个内存池
    /// </summary>
    public interface IRentable
    {
        /// <summary>
        /// 外部提供缓冲区,可以在外部链接一个内存池
        /// </summary>
        IMemoryOwner<byte> RentBuffer(int length);
    }

    public interface IKcpSetting
    {
        int SetMtu(int mtu);
        int Interval(int interval);
        int WndSize(int sndwnd, int rcvwnd);
        int NoDelay(int nodelay, int interval, int resend, int nc);
    }

    public interface IKcpUpdate
    {
        void Update(long time);
    }
}