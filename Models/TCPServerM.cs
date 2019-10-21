using SocketDA.ViewModels;
using System.Windows.Media;

namespace SocketDA.Models
{
    internal class TCPServerModel : MainWindowBase
    {
        #region 网络配置 - Socket属性
        private int _SocketSrcPort;
        public int SocketSrcPort
        {
            get
            {
                return _SocketSrcPort;
            }
            set
            {
                if (_SocketSrcPort != value)
                {
                    _SocketSrcPort = value;
                    RaisePropertyChanged(nameof(SocketSrcPort));
                }
            }
        }
        #endregion

        private string _SocketSrcPAddrText;
        public string SocketSrcIPAddrText
        {
            get
            {
                return _SocketSrcPAddrText;
            }
            set
            {
                if (_SocketSrcPAddrText != value)
                {
                    _SocketSrcPAddrText = value;
                    RaisePropertyChanged(nameof(SocketSrcIPAddrText));
                }
            }
        }

        private int _SocketSrcIPAddrSelectedIndex;
        public int SocketSrcIPAddrSelectedIndex
        {
            get
            {
                return _SocketSrcIPAddrSelectedIndex;
            }
            set
            {
                if (_SocketSrcIPAddrSelectedIndex != value)
                {
                    _SocketSrcIPAddrSelectedIndex = value;
                    RaisePropertyChanged(nameof(SocketSrcIPAddrSelectedIndex));
                }
            }
        }

        #region 网络配置 - Socket属性控件启用/不启用
        private bool _SocketSrcIPAddrEnable;
        public bool SocketSrcIPAddrEnable
        {
            get
            {
                return _SocketSrcIPAddrEnable;
            }
            set
            {
                if (_SocketSrcIPAddrEnable != value)
                {
                    _SocketSrcIPAddrEnable = value;
                    RaisePropertyChanged(nameof(SocketSrcIPAddrEnable));
                }
            }
        }

        private bool _SocketSrcPortEnable;
        public bool SocketSrcPortEnable
        {
            get
            {
                return _SocketSrcPortEnable;
            }
            set
            {
                if (_SocketSrcPortEnable != value)
                {
                    _SocketSrcPortEnable = value;
                    RaisePropertyChanged(nameof(SocketSrcPortEnable));
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

        public void TCPServerDataContext()
        {
            SocketSrcPort = 8088;

            SocketSrcIPAddrText = string.Empty;
            SocketSrcIPAddrSelectedIndex = 0;

            SocketSrcIPAddrEnable = true;
            SocketSrcPortEnable = true;

            SocketBrush = Brushes.Red;
            OpenCloseSocket = string.Format(cultureInfo, "TCP 侦听");
        }
    }
}
