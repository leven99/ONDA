namespace SocketDA.ModelsSocket
{
    /// <summary>
    /// 这个类设置服务器/客户端相关的核心参数
    /// </summary>
    internal sealed class SocketSetting
    {
        private const int DEFAULT_MAX_CONNECTIONS = 10;

        private const int BUFFER_SIZE = 512;

        /// <summary>
        /// 服务器最大可以连接的客户端数量
        /// </summary>
        internal int DefaultMaxConnctions
        {
            get
            {
                return DEFAULT_MAX_CONNECTIONS;
            }
        }

        /// <summary>
        /// 一个SocketAsyncEventArgs对象的缓冲区大小
        /// </summary>
        internal int BufferSize
        {
            get
            {
                return BUFFER_SIZE;
            }
        }
    }
}
