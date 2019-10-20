using SocketDA.ViewModels;

namespace SocketDA.Models
{
    internal class RecvModel : MainWindowBase
    {
        #region TCP Server
        private int _TCPServerRecvDataCount;
        public int TCPServerRecvDataCount
        {
            get
            {
                return _TCPServerRecvDataCount;
            }
            set
            {
                if (_TCPServerRecvDataCount != value)
                {
                    _TCPServerRecvDataCount = value;
                    RaisePropertyChanged(nameof(TCPServerRecvDataCount));
                }
            }
        }

        private string _TCPServerRecvAutoSave;
        public string TCPServerRecvAutoSave
        {
            get
            {
                return _TCPServerRecvAutoSave;
            }
            set
            {
                if (_TCPServerRecvAutoSave != value)
                {
                    _TCPServerRecvAutoSave = value;
                    RaisePropertyChanged(nameof(TCPServerRecvAutoSave));
                }
            }
        }

        private string _TCPServerRecvEnable;
        public string TCPServerRecvEnable
        {
            get
            {
                return _TCPServerRecvEnable;
            }
            set
            {
                if (_TCPServerRecvEnable != value)
                {
                    _TCPServerRecvEnable = value;
                    RaisePropertyChanged(nameof(TCPServerRecvEnable));
                }
            }
        }

        private bool _TCPServerHexRecv;
        public bool TCPServerHexRecv
        {
            get
            {
                return _TCPServerHexRecv;
            }
            set
            {
                if (_TCPServerHexRecv != value)
                {
                    _TCPServerHexRecv = value;
                    RaisePropertyChanged(nameof(TCPServerHexRecv));
                }
            }
        }
        #endregion

        #region TCP Client
        private int _TCPClientRecvDataCount;
        public int TCPClientRecvDataCount
        {
            get
            {
                return _TCPClientRecvDataCount;
            }
            set
            {
                if (_TCPClientRecvDataCount != value)
                {
                    _TCPClientRecvDataCount = value;
                    RaisePropertyChanged(nameof(TCPClientRecvDataCount));
                }
            }
        }

        private string _TCPClientRecvAutoSave;
        public string TCPClientRecvAutoSave
        {
            get
            {
                return _TCPClientRecvAutoSave;
            }
            set
            {
                if (_TCPClientRecvAutoSave != value)
                {
                    _TCPClientRecvAutoSave = value;
                    RaisePropertyChanged(nameof(TCPClientRecvAutoSave));
                }
            }
        }

        private string _TCPClientRecvEnable;
        public string TCPClientRecvEnable
        {
            get
            {
                return _TCPClientRecvEnable;
            }
            set
            {
                if (_TCPClientRecvEnable != value)
                {
                    _TCPClientRecvEnable = value;
                    RaisePropertyChanged(nameof(TCPClientRecvEnable));
                }
            }
        }

        private bool _TCPClientHexRecv;
        public bool TCPClientHexRecv
        {
            get
            {
                return _TCPClientHexRecv;
            }
            set
            {
                if (_TCPClientHexRecv != value)
                {
                    _TCPClientHexRecv = value;
                    RaisePropertyChanged(nameof(TCPClientHexRecv));
                }
            }
        }
        #endregion

        #region UDP Server
        private int _UDPServerRecvDataCount;
        public int UDPServerRecvDataCount
        {
            get
            {
                return _UDPServerRecvDataCount;
            }
            set
            {
                if (_UDPServerRecvDataCount != value)
                {
                    _UDPServerRecvDataCount = value;
                    RaisePropertyChanged(nameof(UDPServerRecvDataCount));
                }
            }
        }

        private string _UDPServerRecvAutoSave;
        public string UDPServerRecvAutoSave
        {
            get
            {
                return _UDPServerRecvAutoSave;
            }
            set
            {
                if (_UDPServerRecvAutoSave != value)
                {
                    _UDPServerRecvAutoSave = value;
                    RaisePropertyChanged(nameof(UDPServerRecvAutoSave));
                }
            }
        }

        private string _UDPServerRecvEnable;
        public string UDPServerRecvEnable
        {
            get
            {
                return _UDPServerRecvEnable;
            }
            set
            {
                if (_UDPServerRecvEnable != value)
                {
                    _UDPServerRecvEnable = value;
                    RaisePropertyChanged(nameof(UDPServerRecvEnable));
                }
            }
        }

        private bool _UDPServerHexRecv;
        public bool UDPServerHexRecv
        {
            get
            {
                return _UDPServerHexRecv;
            }
            set
            {
                if (_UDPServerHexRecv != value)
                {
                    _UDPServerHexRecv = value;
                    RaisePropertyChanged(nameof(UDPServerHexRecv));
                }
            }
        }
        #endregion

        #region UDP Client
        private int _UDPClientRecvDataCount;
        public int UDPClientRecvDataCount
        {
            get
            {
                return _UDPClientRecvDataCount;
            }
            set
            {
                if (_UDPClientRecvDataCount != value)
                {
                    _UDPClientRecvDataCount = value;
                    RaisePropertyChanged(nameof(UDPClientRecvDataCount));
                }
            }
        }

        private string _UDPClientRecvAutoSave;
        public string UDPClientRecvAutoSave
        {
            get
            {
                return _UDPClientRecvAutoSave;
            }
            set
            {
                if (_UDPClientRecvAutoSave != value)
                {
                    _UDPClientRecvAutoSave = value;
                    RaisePropertyChanged(nameof(UDPClientRecvAutoSave));
                }
            }
        }

        private string _UDPClientRecvEnable;
        public string UDPClientRecvEnable
        {
            get
            {
                return _UDPClientRecvEnable;
            }
            set
            {
                if (_UDPClientRecvEnable != value)
                {
                    _UDPClientRecvEnable = value;
                    RaisePropertyChanged(nameof(UDPClientRecvEnable));
                }
            }
        }

        private bool _UDPClientHexRecv;
        public bool UDPClientHexRecv
        {
            get
            {
                return _UDPClientHexRecv;
            }
            set
            {
                if (_UDPClientHexRecv != value)
                {
                    _UDPClientHexRecv = value;
                    RaisePropertyChanged(nameof(UDPClientHexRecv));
                }
            }
        }
        #endregion

        #region 接收区 - Header
        /// <summary>
        /// 接收区Header中的接收计数
        /// </summary>
        private int _RecvDataCount;
        public int RecvDataCount
        {
            get
            {
                return _RecvDataCount;
            }
            set
            {
                if (_RecvDataCount != value)
                {
                    _RecvDataCount = value;
                    RaisePropertyChanged(nameof(RecvDataCount));
                }
            }
        }

        /// <summary>
        /// 接收区Header中的 [保存中/已停止] 字符串
        /// </summary>
        private string _RecvAutoSave;
        public string RecvAutoSave
        {
            get
            {
                return _RecvAutoSave;
            }
            set
            {
                if (_RecvAutoSave != value)
                {
                    _RecvAutoSave = value;
                    RaisePropertyChanged(nameof(RecvAutoSave));
                }
            }
        }

        /// <summary>
        /// 接收区Header中的 [允许/暂停] 字符串
        /// </summary>
        private string _RecvEnable;
        public string RecvEnable
        {
            get
            {
                return _RecvEnable;
            }
            set
            {
                if (_RecvEnable != value)
                {
                    _RecvEnable = value;
                    RaisePropertyChanged(nameof(RecvEnable));
                }
            }
        }
        #endregion

        private string _RecvData;
        public string RecvData
        {
            get
            {
                return _RecvData;
            }
            set
            {
                if (_RecvData != value)
                {
                    _RecvData = value;
                    RaisePropertyChanged(nameof(RecvData));
                }
            }
        }

        /// <summary>
        /// 辅助区 - 十六进制接收
        /// </summary>
        private bool _HexRecv;
        public bool HexRecv
        {
            get
            {
                return _HexRecv;
            }
            set
            {
                if (_HexRecv != value)
                {
                    _HexRecv = value;
                    RaisePropertyChanged(nameof(HexRecv));
                }
            }
        }

        public void RecvDataContext()
        {
            TCPServerRecvDataCount = 0;
            TCPServerRecvAutoSave = string.Format(cultureInfo, "已停止");
            TCPServerRecvEnable = string.Format(cultureInfo, " 提示：双击文本框更改接收状态 ");
            TCPServerHexRecv = false;

            TCPClientRecvDataCount = 0;
            TCPClientRecvAutoSave = string.Format(cultureInfo, "已停止");
            TCPClientRecvEnable = string.Format(cultureInfo, " 提示：双击文本框更改接收状态 ");
            TCPClientHexRecv = false;

            UDPServerRecvDataCount = 0;
            UDPServerRecvAutoSave = string.Format(cultureInfo, "已停止");
            UDPServerRecvEnable = string.Format(cultureInfo, " 提示：双击文本框更改接收状态 ");
            UDPServerHexRecv = false;

            UDPClientRecvDataCount = 0;
            UDPClientRecvAutoSave = string.Format(cultureInfo, "已停止");
            UDPClientRecvEnable = string.Format(cultureInfo, " 提示：双击文本框更改接收状态 ");
            UDPClientHexRecv = false;

            RecvDataCount = 0;
            RecvAutoSave = string.Format(cultureInfo, "已停止");
            RecvEnable = string.Format(cultureInfo, " 提示：双击文本框更改接收状态 ");

            RecvData = string.Empty;
            HexRecv = false;
        }
    }
}
