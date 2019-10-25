using SocketDA.ViewModels;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;

namespace SocketDA.Models
{
    internal class SocketModel : MainWindowBase
    {
        /// <summary>
        /// 本机IP地址
        /// </summary>
        public Collection<IPAddress> SocketSrcIPAddrItemsSource { get; set; }

        /// <summary>
        /// 本机IP地址，包含网络配置器的名称
        /// </summary>
        public Collection<string> SocketSourceIPAddressItemsSource { get; set; }

        /// <summary>
        /// 判断IP地址和端口号的合法性
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool TryParseIPAddressPort(string ipAddress, int port)
        {
            if(IPAddress.TryParse(ipAddress, out _))
            {
                if ( (port > 0) && (port < 65535) )
                {
                    return true;
                }
            }

            return false;
        }

        public void SocketDataContext()
        {
            SocketSrcIPAddrItemsSource = new Collection<IPAddress>();
            SocketSourceIPAddressItemsSource = new Collection<string>();

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

                        SocketSrcIPAddrItemsSource.Add(_IPAddress);
                        SocketSourceIPAddressItemsSource.Add(_IPAddress.ToString() + " / " + _NetworkInterfaceName);
                    }
                }
            }
        }
    }
}
