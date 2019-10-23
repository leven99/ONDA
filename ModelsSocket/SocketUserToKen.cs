using System.Net;
using System.Net.Sockets;

namespace SocketDA.ModelsSocket
{
    internal sealed class SocketUserToKen
    {
        /// <summary>
        /// 已发送字节数
        /// </summary>
        internal int SendBytesAlreadyCount { get; set; }

        /// <summary>
        /// 剩余发送字节数
        /// </summary>
        internal int SendBytesRemainingCount { get; set; }

        /// <summary>
        /// 已接收字节数
        /// </summary>
        internal int RecvBytesAlreadyCount { get; set; }

        /// <summary>
        /// 剩余接收字节数
        /// </summary>
        internal int RecvBytesRemainingCount { get; set; }

        /// <summary>
        /// IP地址
        /// 
        /// 对于服务器：这属于客户端的IP地址
        /// 对于客户端：这属于服务器的IP地址
        /// </summary>
        internal IPAddress IPAddressConnections { get; set; }

        /// <summary>
        /// 端口号
        /// 
        /// 对于服务器：这属于客户端的端口号
        /// 对于客户端：这属于服务器的端口号
        /// </summary>
        internal int PortConnections { get; set; }

        /// <summary>
        /// IP结点
        /// 
        /// 对于服务器：这属于客户端的IP结点
        /// 对于客户端：这属于服务器的IP结点
        /// </summary>
        internal IPEndPoint IPEndPointConnections { get; set; }

        /// <summary>
        /// 套接字
        /// 
        /// 对于服务器：这属于客户端的套接字
        /// 对于客户端：这属于服务器的套接字
        /// </summary>
        internal Socket SocketConnections { get; set; }

        public SocketUserToKen()
        {
            
        }
    }
}
