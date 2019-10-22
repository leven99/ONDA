using System;
using System.Net;
using System.Net.Sockets;

namespace SocketDA.ModelsSocket
{
    /// <summary>
    /// 这是一个Socket基类，只有创建IP节点和创建Socket两个操作
    /// </summary>
    internal sealed class SocketBase
    {
        internal IPEndPoint _IPEndPoint = null;

        internal IPEndPoint CreateIPEndPoint(IPAddress ipAddress, int port)
        {
            try
            {
                _IPEndPoint = new IPEndPoint(ipAddress, port);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }

            return _IPEndPoint;
        }

        internal Socket CreateSocket(IPAddress ipAddress, int port, ProtocolType protocolType)
        {
            if (CreateIPEndPoint(ipAddress, port) == null)
            {
                return null;
            }

            try
            {
                if (protocolType == ProtocolType.Tcp)
                {
                    /* 创建 TCP Socket（支持IPv4，IPv6） */
                    return new Socket(_IPEndPoint.AddressFamily, SocketType.Stream, protocolType);
                }
                else if (protocolType == ProtocolType.Udp)
                {
                    /* 创建 UDP Socket（支持IPv4，IPv6） */
                    return new Socket(_IPEndPoint.AddressFamily, SocketType.Dgram, protocolType);
                }
                else
                {
                    return null;
                }
            }
            catch (SocketException)
            {
                return null;
            }
        }
    }
}
