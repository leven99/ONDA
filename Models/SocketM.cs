using SocketDA.ViewModels;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;

namespace SocketDA.Models
{
    internal class SocketModel : MainWindowBase
    {
        public Collection<IPAddress> SocketSrcPAddrItemsSource { get; set; }
        public Collection<string> SocketSourceIPAddressItemsSource { get; set; }

        #region 接收区/发送区Header[IP:Port]
        private string _IPString;
        public string IPString
        {
            get
            {
                return _IPString;
            }
            set
            {
                if (_IPString != value)
                {
                    _IPString = value;
                    RaisePropertyChanged(nameof(IPString));
                }
            }
        }

        private string _PortString;
        public string PortString
        {
            get
            {
                return _PortString;
            }
            set
            {
                if (_PortString != value)
                {
                    _PortString = value;
                    RaisePropertyChanged(nameof(PortString));
                }
            }
        }
        #endregion

        public void SocketDataContext()
        {
            SocketSrcPAddrItemsSource = new Collection<IPAddress>();
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

                        SocketSrcPAddrItemsSource.Add(_IPAddress);
                        SocketSourceIPAddressItemsSource.Add(_IPAddress.ToString() + " / " + _NetworkInterfaceName);
                    }
                }
            }

            IPString = "IP";
            PortString = "Port";
        }
    }
}
