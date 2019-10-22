namespace SocketDA.ModelsSocket
{
    internal sealed class SocketUserToKen
    {
        /// <summary>
        /// 以发送字节数
        /// </summary>
        internal int _SendBytesAlreadyCount = 0;

        /// <summary>
        /// 剩余发送字节数
        /// </summary>
        internal int _SendBytesRemainingCount = 0;

        public SocketUserToKen()
        {
            
        }
    }
}
