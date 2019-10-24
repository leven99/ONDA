using System.Net;
using System.Net.Sockets;

namespace SocketDA.ModelsSocket
{
    internal sealed class SocketUserToKen
    {
        /// <summary>
        /// 套接字
        /// </summary>
        internal Socket Socket { get; set; }

        /// <summary>
        /// 本地终结点
        /// </summary>
        internal EndPoint LocalEndPoint { get; set; }

        /// <summary>
        /// 远程终结点
        /// </summary>
        internal EndPoint RemoteEndPoint { get; set; }

        public SocketUserToKen(Socket socket)
        {
            Socket = socket;
            LocalEndPoint = socket.LocalEndPoint;
            RemoteEndPoint = socket.RemoteEndPoint;
        }
    }
}
