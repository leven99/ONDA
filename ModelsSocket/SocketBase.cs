using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using SocketDA.Models;

namespace SocketDA.ModelsSocket
{
    /// <summary>
    /// 这是一个Socket基类，只有创建IP节点和创建Socket两个操作
    /// </summary>
    internal sealed class SocketBase
    {
        internal IPEndPoint IPEndPoint { get; set; }

        public bool TryParseIPAddressPort(string ipAddress, int port)
        {
            if (IPAddress.TryParse(ipAddress, out _))
            {
                if ((port > 0) && (port < 65535))
                {
                    return true;
                }
            }

            return false;
        }

        internal IPEndPoint CreateIPEndPoint(IPAddress ipAddress, int port)
        {
            try
            {
                IPEndPoint = new IPEndPoint(ipAddress, port);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }

            return IPEndPoint;
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
                    return new Socket(IPEndPoint.AddressFamily, SocketType.Stream, protocolType);
                }
                else if (protocolType == ProtocolType.Udp)
                {
                    /* 创建 UDP Socket（支持IPv4，IPv6） */
                    return new Socket(IPEndPoint.AddressFamily, SocketType.Dgram, protocolType);
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

        internal void GitNetworkInterface(ObservableCollection<IPAddress> IPAddrItemsSource, 
            ObservableCollection<string> IPAddrInfoItemsSource)
        {
            /* 本地计算机的所有网络设配器 */
            NetworkInterface[] NetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var _NetworkInterface in NetworkInterfaces)
            {
                if (_NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    /* 获取网络设配器的名称 */
                    string _NetworkInterfaceName = _NetworkInterface.Name;

                    var _NetworkInterfaceIPProperties = _NetworkInterface.GetIPProperties();

                    /* 网络设配器接口的单播地址个数 */
                    var _IPAddressCount = _NetworkInterfaceIPProperties.UnicastAddresses.Count;

                    for (int _Count = 0; _Count < _IPAddressCount; _Count++)
                    {
                        /* 获取网络设配器接口单播地址 */
                        IPAddress _IPAddress = _NetworkInterfaceIPProperties.UnicastAddresses[_Count].Address;

                        IPAddrItemsSource.Add(_IPAddress);
                        IPAddrInfoItemsSource.Add(_IPAddress.ToString() + " / " + _NetworkInterfaceName);
                    }
                }
            }
        }
    }
}
