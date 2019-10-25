using SocketDA.ViewModels;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Media;

namespace SocketDA.Models
{
    internal class TCPClientModel : MainWindowBase
    {
        public ObservableCollection<IPAddress> SocketDestIPAddrItemsSource { get; set; }

        #region 网络配置 - Socket属性
        private int _SocketDestPort;
        public int SocketDestPort
        {
            get
            {
                return _SocketDestPort;
            }
            set
            {
                if (_SocketDestPort != value)
                {
                    _SocketDestPort = value;
                    RaisePropertyChanged(nameof(SocketDestPort));
                }
            }
        }
        #endregion

        private string _SocketDestPAddrText;
        public string SocketDestIPAddrText
        {
            get
            {
                return _SocketDestPAddrText;
            }
            set
            {
                if (_SocketDestPAddrText != value)
                {
                    _SocketDestPAddrText = value;
                    RaisePropertyChanged(nameof(SocketDestIPAddrText));
                }
            }
        }

        private int _SocketDestIPAddrSelectedIndex;
        public int SocketDestIPAddrSelectedIndex
        {
            get
            {
                return _SocketDestIPAddrSelectedIndex;
            }
            set
            {
                if (_SocketDestIPAddrSelectedIndex != value)
                {
                    _SocketDestIPAddrSelectedIndex = value;
                    RaisePropertyChanged(nameof(SocketDestIPAddrSelectedIndex));
                }
            }
        }

        #region 网络配置 - Socket属性控件启用/不启用
        private bool _SocketDestIPAddrEnable;
        public bool SocketDestIPAddrEnable
        {
            get
            {
                return _SocketDestIPAddrEnable;
            }
            set
            {
                if (_SocketDestIPAddrEnable != value)
                {
                    _SocketDestIPAddrEnable = value;
                    RaisePropertyChanged(nameof(SocketDestIPAddrEnable));
                }
            }
        }

        private bool _SocketDestPortEnable;
        public bool SocketDestPortEnable
        {
            get
            {
                return _SocketDestPortEnable;
            }
            set
            {
                if (_SocketDestPortEnable != value)
                {
                    _SocketDestPortEnable = value;
                    RaisePropertyChanged(nameof(SocketDestPortEnable));
                }
            }
        }
        #endregion

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

        public void TCPClientDataContext()
        {
            SocketDestIPAddrItemsSource = new ObservableCollection<IPAddress>();

            SocketDestPort = 8088;

            SocketDestIPAddrText = string.Empty;
            SocketDestIPAddrSelectedIndex = 0;

            SocketDestIPAddrEnable = true;
            SocketDestPortEnable = true;

            SocketBrush = Brushes.Red;
            OpenCloseSocket = string.Format(cultureInfo, "TCP 连接");
        }
    }
}
