using SocketDA.ViewModels;
using System.Windows.Media;

namespace SocketDA.Models
{
    class SocketModel : MainWindowBase
    {
        public string[] SocketProtocolItemsSource { get; set; }

        public string _SocketProtocol;
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

        public string _SocketPort;
        public string SocketPort
        {
            get
            {
                return _SocketPort;
            }
            set
            {
                if (_SocketPort != value)
                {
                    _SocketPort = value;
                    RaisePropertyChanged(nameof(SocketPort));
                }
            }
        }

        public Brush _SocketBrush;
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

        public string _OpenCloseSocket;
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

        public void SocketDataContext()
        {
            SocketProtocolItemsSource = new string[] { "TCP Server", "TCP Client", "UDP Server", "UDP Client" };

            SocketProtocol = "TCP Server";
            SocketPort = "6088";

            SocketBrush = Brushes.Red;
            OpenCloseSocket = string.Format(cultureInfo, "TCP ÕìÌý");

            SocketProtocolIsEnabled = true;
        }
    }
}
