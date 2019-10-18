using SocketDA.ViewModels;

namespace SocketDA.Models
{
    internal class SendModel : MainWindowBase
    {
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
            SendDataCount = 0;
            SendData = string.Empty;

            AutoSendNum = 1000;
        }
    }
}
