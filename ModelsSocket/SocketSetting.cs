namespace SocketDA.ModelsSocket
{
    internal sealed class SocketSetting
    {
        /// <summary>
        /// 服务器最大可以连接的客户端数量
        /// </summary>
        internal int DefaultMaxConnctions { get; } = 4;

        /// <summary>
        /// 一个SocketAsyncEventArgs对象的缓冲区大小
        /// </summary>
        internal int BufferSize { get; } = 512;

        /// <summary>
        /// 预分配操作，读与写
        /// </summary>
        internal int OpsToPreAlloc { get; } = 2;

        /// <summary>
        /// 接收缓冲区大小（单位：Bytes，系统默认8192）
        /// </summary>
        internal int ReceiveBufferSize { get; } = 2097152;   /* 2MB */

        /// <summary>
        /// 发送缓冲区大小（单位：Bytes，系统默认8192）
        /// </summary>
        internal int SendBufferSize { get; } = 2097152;   /* 2MB */

        /// <summary>
        /// 客户端连接服务器超时
        /// </summary>
        internal int ConnectTimeout { get; } = 3000;   /* 3000MS */

        /// <summary>
        /// 接收超时（适用：同步，单位：milliseconds，系统默认：0[无限等待]）
        /// </summary>
        internal int ReceiveTimeout { get; } = 1000;   /* 1000MS */

        /// <summary>
        /// 发送超时（适用：同步，单位：milliseconds，系统默认：0[无限等待]）
        /// </summary>
        internal int SendTimeout { get; } = 1000;   /* 1000MS */

        /// <summary>
        /// IP数据包的生存时间（Time To Live）值（取值范围：0 ~ 255）
        /// 
        /// 注释：生存时间是指容许这个数据包在到达其目的地之前通过多少个路由器。数据包每通过一个路由器，
        /// 其生存时间都会由路由器减一。当生存时间降为零时， 路由器就会丢弃这个数据包。
        /// </summary>
        internal short Ttl { get; } = 32;
    }
}
