using SocketDA.ViewModels;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Media;

namespace SocketDA.Models
{
    class SocketModel : MainWindowBase
    {
        public Collection<string> SocketProtocolItemsSource { get; set; }
        public IPAddress[] SocketSourceIPAddressItemsSource { get; set; }
        public Collection<IPAddress> SocketDestinationIPAddressItemsSource { get; set; }

        #region 网络配置 - Socket属性
        private string _SocketProtocol;
        public string SocketProtocol
        {
            get
            {
                return _SocketProtocol;
            }
            set
            {
                if (_SocketProtocol != value)
                {
                    _SocketProtocol = value;
                    RaisePropertyChanged(nameof(SocketProtocol));
                }
            }
        }

        private IPAddress _SocketSourceIPAddress;
        public IPAddress SocketSourceIPAddress
        {
            get
            {
                return _SocketSourceIPAddress;
            }
            set
            {
                if (_SocketSourceIPAddress != value)
                {
                    _SocketSourceIPAddress = value;
                    RaisePropertyChanged(nameof(SocketSourceIPAddress));
                }
            }
        }

        private string _SocketDestinationIPAddress;
        public string SocketDestinationIPAddress
        {
            get
            {
                return _SocketDestinationIPAddress;
            }
            set
            {
                if (_SocketDestinationIPAddress != value)
                {
                    _SocketDestinationIPAddress = value;
                    RaisePropertyChanged(nameof(SocketDestinationIPAddress));
                }
            }
        }

        private int _SocketSourcePort;
        public int SocketSourcePort
        {
            get
            {
                return _SocketSourcePort;
            }
            set
            {
                if (_SocketSourcePort != value)
                {
                    _SocketSourcePort = value;
                    RaisePropertyChanged(nameof(SocketSourcePort));
                }
            }
        }

        private int _SocketDestinationPort;
        public int SocketDestinationPort
        {
            get
            {
                return _SocketDestinationPort;
            }
            set
            {
                if (_SocketDestinationPort != value)
                {
                    _SocketDestinationPort = value;
                    RaisePropertyChanged(nameof(SocketDestinationPort));
                }
            }
        }
        #endregion

        private int _SocketProtocolSelectedIndex;
        public int SocketProtocolSelectedIndex
        {
            get
            {
                return _SocketProtocolSelectedIndex;
            }
            set
            {
                if (_SocketProtocolSelectedIndex != value)
                {
                    _SocketProtocolSelectedIndex = value;
                    RaisePropertyChanged(nameof(SocketProtocolSelectedIndex));
                }
            }
        }

        private int _SocketSourceIPAddressSelectedIndex;
        public int SocketSourceIPAddressSelectedIndex
        {
            get
            {
                return _SocketSourceIPAddressSelectedIndex;
            }
            set
            {
                if (_SocketSourceIPAddressSelectedIndex != value)
                {
                    _SocketSourceIPAddressSelectedIndex = value;
                    RaisePropertyChanged(nameof(SocketSourceIPAddressSelectedIndex));
                }
            }
        }

        private int _SocketDestinationIPAddressSelectedIndex;
        public int SocketDestinationIPAddressSelectedIndex
        {
            get
            {
                return _SocketDestinationIPAddressSelectedIndex;
            }
            set
            {
                if (_SocketDestinationIPAddressSelectedIndex != value)
                {
                    _SocketDestinationIPAddressSelectedIndex = value;
                    RaisePropertyChanged(nameof(SocketDestinationIPAddressSelectedIndex));
                }
            }
        }

        #region 网络配置 - Socket打开/关闭按钮
        private Brush _SocketBrush;
        public Brush SocketBrush
        {
            get
            {
                return _SocketBrush;
            }
            set
            {
                if (_SocketBrush != value)
                {
                    _SocketBrush = value;
                    RaisePropertyChanged(nameof(SocketBrush));
                }
            }
        }

        private string _OpenCloseSocket;
        public string OpenCloseSocket
        {
            get
            {
                return _OpenCloseSocket;
            }
            set
            {
                if (_OpenCloseSocket != value)
                {
                    _OpenCloseSocket = value;
                    RaisePropertyChanged(nameof(OpenCloseSocket));
                }
            }
        }
        #endregion

        #region 网络配置 - Socket属性控件启用/不启用
        private bool _SocketProtocolIsEnabled;
        public bool SocketProtocolIsEnabled
        {
            get
            {
                return _SocketProtocolIsEnabled;
            }
            set
            {
                if (_SocketProtocolIsEnabled != value)
                {
                    _SocketProtocolIsEnabled = value;
                    RaisePropertyChanged(nameof(SocketProtocolIsEnabled));
                }
            }
        }
        #endregion

        #region 网络配置 - Socket属性中目的IP地址/目的端口号可见性
        private string _DestinationVisibility;
        public string DestinationVisibility
        {
            get
            {
                return _DestinationVisibility;
            }
            set
            {
                if (_DestinationVisibility != value)
                {
                    _DestinationVisibility = value;
                    RaisePropertyChanged(nameof(DestinationVisibility));
                }
            }
        }
        #endregion

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
            SocketProtocolItemsSource = new Collection<string>
            {
                "TCP Server", "TCP Client", "UDP Server", "UDP Client"
            };
            SocketSourceIPAddressItemsSource = Dns.GetHostAddresses("");
            SocketDestinationIPAddressItemsSource = new Collection<IPAddress>();

            SocketProtocolSelectedIndex = 0;
            SocketSourceIPAddressSelectedIndex = 0;

            SocketSourcePort = 8088;
            SocketDestinationPort = 8088;

            SocketBrush = Brushes.Red;
            OpenCloseSocket = string.Format(cultureInfo, "TCP 侦听");

            SocketProtocolIsEnabled = true;
            DestinationVisibility = "Collapsed";

            IPString = "IP";
            PortString = "Port";
        }
    }
}
