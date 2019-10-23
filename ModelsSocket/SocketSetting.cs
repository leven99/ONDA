namespace SocketDA.ModelsSocket
{
    /// <summary>
    /// 这个类设置服务器/客户端相关的核心参数
    /// </summary>
    internal sealed class SocketSetting
    {
        /// <summary>
        /// 服务器最大可以连接的客户端数量
        /// </summary>
        internal int DefaultMaxConnctions { get; } = 10;

        /// <summary>
        /// 一个SocketAsyncEventArgs对象的缓冲区大小
        /// </summary>
        internal int BufferSize { get; } = 256;

        /// <summary>
        /// 预分配操作读与写
        /// </summary>
        internal int OpsToPreAlloc { get; } = 2;
    }
}
