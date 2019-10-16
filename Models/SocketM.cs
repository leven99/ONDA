using SocketDA.ViewModels;
using System.Windows.Media;

namespace SocketDA.Models
{
    class SocketModel : MainWindowBase
    {
        public string[] SocketProtocolItemsSource { get; set; }

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

        private string _SocketSourcePort;
        public string SocketSourcePort
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

        private string _SocketDestinationPort;
        public string SocketDestinationPort
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

        public void SocketDataContext()
        {
            SocketProtocolItemsSource = new string[] { "TCP Server", "TCP Client", "UDP Server", "UDP Client" };
            SocketProtocolSelectedIndex = 0;
            SocketProtocol = "TCP Server";

            SocketSourcePort = "8088";
            SocketDestinationPort = "8088";

            SocketBrush = Brushes.Red;
            OpenCloseSocket = string.Format(cultureInfo, "TCP ÕìÌý");

            SocketProtocolIsEnabled = true;
            DestinationVisibility = "Collapsed";
        }
    }
}
