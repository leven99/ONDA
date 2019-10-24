namespace SocketDA.ModelsSocket
{
    internal sealed class SocketSetting
    {
        /// <summary>
        /// 服务器最大可以连接的客户端数量
        /// </summary>
        internal int DefaultMaxConnctions { get; } = 10;

        /// <summary>
        /// 一个SocketAsyncEventArgs对象的缓冲区大小
        /// </summary>
        internal int BufferSize { get; } = 1048576;   /* 1MB */

        /// <summary>
        /// 预分配操作，读与写
        /// </summary>
        internal int OpsToPreAlloc { get; } = 2;

        /// <summary>
        /// 接收缓冲区大小（单位：Bytes，系统默认8192）
        /// </summary>
        internal int ReceiveBufferSize { get; } = 2097152 + 2;   /* 2MB */

        /// <summary>
        /// 发送缓冲区大小（单位：Bytes，系统默认8192）
        /// </summary>
        internal int SendBufferSize { get; } = 2097152 + 1;   /* 2MB */

        /// <summary>
        /// 接收超时（适用：同步，单位：milliseconds，系统默认：0）
        /// </summary>
        internal int ReceiveTimeout { get; } = 1000;   /* 1000MS */

        /// <summary>
        /// 发送超时（适用：同步，单位：milliseconds，系统默认：0）
        /// </summary>
        internal int SendTimeout { get; } = 1000;   /* 1000MS */
    }
}
