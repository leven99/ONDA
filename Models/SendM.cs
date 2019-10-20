using SocketDA.ViewModels;

namespace SocketDA.Models
{
    internal class SendModel : MainWindowBase
    {
        #region TCP Server
        private int _TCPServerSendDataCount;
        public int TCPServerSendDataCount
        {
            get
            {
                return _TCPServerSendDataCount;
            }
            set
            {
                if (_TCPServerSendDataCount != value)
                {
                    _TCPServerSendDataCount = value;
                    RaisePropertyChanged(nameof(TCPServerSendDataCount));
                }
            }
        }

        private string _TCPServerSendData;
        public string TCPServerSendData
        {
            get
            {
                return _TCPServerSendData;
            }
            set
            {
                if (_TCPServerSendData != value)
                {
                    _TCPServerSendData = value;
                    RaisePropertyChanged(nameof(TCPServerSendData));
                }
            }
        }

        private int _TCPServerAutoSendNum;
        public int TCPServerAutoSendNum
        {
            get
            {
                return _TCPServerAutoSendNum;
            }
            set
            {
                if (_TCPServerAutoSendNum != value)
                {
                    _TCPServerAutoSendNum = value;
                    RaisePropertyChanged(nameof(TCPServerAutoSendNum));
                }
            }
        }
        #endregion

        #region TCP Client
        private int _TCPClientSendDataCount;
        public int TCPClientSendDataCount
        {
            get
            {
                return _TCPClientSendDataCount;
            }
            set
            {
                if (_TCPClientSendDataCount != value)
                {
                    _TCPClientSendDataCount = value;
                    RaisePropertyChanged(nameof(TCPClientSendDataCount));
                }
            }
        }

        private string _TCPClientSendData;
        public string TCPClientSendData
        {
            get
            {
                return _TCPClientSendData;
            }
            set
            {
                if (_TCPClientSendData != value)
                {
                    _TCPClientSendData = value;
                    RaisePropertyChanged(nameof(TCPClientSendData));
                }
            }
        }

        private int _TCPClientAutoSendNum;
        public int TCPClientAutoSendNum
        {
            get
            {
                return _TCPClientAutoSendNum;
            }
            set
            {
                if (_TCPClientAutoSendNum != value)
                {
                    _TCPClientAutoSendNum = value;
                    RaisePropertyChanged(nameof(TCPClientAutoSendNum));
                }
            }
        }
        #endregion

        #region UDP Server
        private int _UDPServerSendDataCount;
        public int UDPServerSendDataCount
        {
            get
            {
                return _UDPServerSendDataCount;
            }
            set
            {
                if (_UDPServerSendDataCount != value)
                {
                    _UDPServerSendDataCount = value;
                    RaisePropertyChanged(nameof(UDPServerSendDataCount));
                }
            }
        }

        private string _UDPServerSendData;
        public string UDPServerSendData
        {
            get
            {
                return _UDPServerSendData;
            }
            set
            {
                if (_UDPServerSendData != value)
                {
                    _UDPServerSendData = value;
                    RaisePropertyChanged(nameof(UDPServerSendData));
                }
            }
        }

        private int _UDPServerAutoSendNum;
        public int UDPServerAutoSendNum
        {
            get
            {
                return _UDPServerAutoSendNum;
            }
            set
            {
                if (_UDPServerAutoSendNum != value)
                {
                    _UDPServerAutoSendNum = value;
                    RaisePropertyChanged(nameof(UDPServerAutoSendNum));
                }
            }
        }
        #endregion

        #region UDP Client
        private int _UDPClientSendDataCount;
        public int UDPClientSendDataCount
        {
            get
            {
                return _UDPClientSendDataCount;
            }
            set
            {
                if (_UDPClientSendDataCount != value)
                {
                    _UDPClientSendDataCount = value;
                    RaisePropertyChanged(nameof(UDPClientSendDataCount));
                }
            }
        }

        private string _UDPClientSendData;
        public string UDPClientSendData
        {
            get
            {
                return _UDPClientSendData;
            }
            set
            {
                if (_UDPClientSendData != value)
                {
                    _UDPClientSendData = value;
                    RaisePropertyChanged(nameof(UDPClientSendData));
                }
            }
        }

        private int _UDPClientAutoSendNum;
        public int UDPClientAutoSendNum
        {
            get
            {
                return _UDPClientAutoSendNum;
            }
            set
            {
                if (_UDPClientAutoSendNum != value)
                {
                    _UDPClientAutoSendNum = value;
                    RaisePropertyChanged(nameof(UDPClientAutoSendNum));
                }
            }
        }
        #endregion

        /// <summary>
        /// 发送区Header中的发送计数
        /// </summary>
        private int _SendDataCount;
        public int SendDataCount
        {
            get
            {
                return _SendDataCount;
            }
            set
            {
                if (_SendDataCount != value)
                {
                    _SendDataCount = value;
                    RaisePropertyChanged(nameof(SendDataCount));
                }
            }
        }

        private string _SendData;
        public string SendData
        {
            get
            {
                return _SendData;
            }
            set
            {
                if (_SendData != value)
                {
                    _SendData = value;
                    RaisePropertyChanged(nameof(SendData));
                }
            }
        }

        /// <summary>
        /// 辅助区 - 自送发送的时间间隔
        /// </summary>
        private int _AutoSendNum;
        public int AutoSendNum
        {
            get
            {
                return _AutoSendNum;
            }
            set
            {
                if (_AutoSendNum != value)
                {
                    _AutoSendNum = value;
                    RaisePropertyChanged(nameof(AutoSendNum));
                }
            }
        }

        public void SendDataContext()
        {
            TCPServerSendDataCount = 0;
            TCPServerSendData = string.Empty;
            TCPServerAutoSendNum = 1000;

            TCPClientSendDataCount = 0;
            TCPClientSendData = string.Empty;
            TCPClientAutoSendNum = 1000;

            UDPServerSendDataCount = 0;
            UDPServerSendData = string.Empty;
            UDPServerAutoSendNum = 1000;

            UDPClientSendDataCount = 0;
            UDPClientSendData = string.Empty;
            UDPClientAutoSendNum = 1000;

            SendDataCount = 0;
            SendData = string.Empty;

            AutoSendNum = 1000;
        }
    }
}
