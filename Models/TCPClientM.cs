using SocketDA.ViewModels;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Media;

namespace SocketDA.Models
{
    internal class TCPClientConnectionsInfo
    {
        public string RemoteEndPoint { get; set; }
    }

    internal class TCPClientModel : MainWindowBase
    {
        /// <summary>
        /// 连接区 - 服务器信息
        /// </summary>
        public ObservableCollection<TCPClientConnectionsInfo> ConnectionsInfo { get; set; }

        #region 网络配置 - Socket属性
        public ObservableCollection<IPAddress> IPAddrItemsSource { get; set; }

        private string _IPAddrText;
        public string IPAddrText
        {
            get
            {
                return _IPAddrText;
            }
            set
            {
                if (_IPAddrText != value)
                {
                    _IPAddrText = value;
                    RaisePropertyChanged(nameof(IPAddrText));
                }
            }
        }

        private int _IPAddrSelectedIndex;
        public int IPAddrSelectedIndex
        {
            get
            {
                return _IPAddrSelectedIndex;
            }
            set
            {
                if (_IPAddrSelectedIndex != value)
                {
                    _IPAddrSelectedIndex = value;
                    RaisePropertyChanged(nameof(IPAddrSelectedIndex));
                }
            }
        }

        private int _Port;
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                if (_Port != value)
                {
                    _Port = value;
                    RaisePropertyChanged(nameof(Port));
                }
            }
        }
        #endregion

        #region 网络配置 - Socket属性控件启用/不启用
        private bool _IPAddrEnable;
        public bool IPAddrEnable
        {
            get
            {
                return _IPAddrEnable;
            }
            set
            {
                if (_IPAddrEnable != value)
                {
                    _IPAddrEnable = value;
                    RaisePropertyChanged(nameof(IPAddrEnable));
                }
            }
        }

        private bool _PortEnable;
        public bool PortEnable
        {
            get
            {
                return _PortEnable;
            }
            set
            {
                if (_PortEnable != value)
                {
                    _PortEnable = value;
                    RaisePropertyChanged(nameof(PortEnable));
                }
            }
        }
        #endregion

        #region 网络配置 - Socket打开/关闭按钮
        private Brush _Brush;
        public Brush Brush
        {
            get
            {
                return _Brush;
            }
            set
            {
                if (_Brush != value)
                {
                    _Brush = value;
                    RaisePropertyChanged(nameof(Brush));
                }
            }
        }

        private string _OpenClose;
        public string OpenClose
        {
            get
            {
                return _OpenClose;
            }
            set
            {
                if (_OpenClose != value)
                {
                    _OpenClose = value;
                    RaisePropertyChanged(nameof(OpenClose));
                }
            }
        }
        #endregion

        public void TCPClientDataContext()
        {
            ConnectionsInfo = new ObservableCollection<TCPClientConnectionsInfo>();

            IPAddrItemsSource = new ObservableCollection<IPAddress>();
            IPAddrText = string.Empty;
            IPAddrSelectedIndex = 0;

            Port = 8088;

            IPAddrEnable = true;
            PortEnable = true;

            Brush = Brushes.Red;
            OpenClose = string.Format(cultureInfo, "TCP 连接");
        }
    }
}
