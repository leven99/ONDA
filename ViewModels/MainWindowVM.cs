using Microsoft.Win32;
using SocketDA.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace SocketDA.ViewModels
{
    internal class MainWindowViewModel : MainWindowBase
    {
        #region 字段
        public SocketTCPServer SocketTCPServer = null;
        public SocketTCPClient SocketTCPClient = null;
        public SocketUDPServer SocketUDPServer = null;
        public SocketUDPClient SocketUDPClient = null;
        #endregion

        public SocketModel SocketModel { get; set; }
        public TCPServerModel TCPServerModel { get; set; }
        public TCPClientModel TCPClientModel { get; set; }
        public UDPServerModel UDPServerModel { get; set; }
        public UDPClientModel UDPClientModel { get; set; }
        public SendModel SendModel { get; set; }
        public RecvModel RecvModel { get; set; }
        public TimerModel TimerModel { get; set; }
        public HelpModel HelpModel { get; set; }
        public GitRelease LatestRelease { get; set; }

        #region 状态栏- 信息描述
        private string _DepictInfo;
        public string DepictInfo
        {
            get { return _DepictInfo; }
            set
            {
                if (_DepictInfo != value)
                {
                    _DepictInfo = value;
                    RaisePropertyChanged(nameof(DepictInfo));
                }
            }
        }
        #endregion

        #region 菜单栏

        #region 文件
        public void ExitWindow()
        {

        }
        #endregion

        #region 选项
        #endregion

        #region 视图
        public void Reduced_Enable()
        {
            HelpModel.ReducedEnable = !HelpModel.ReducedEnable;

            if (HelpModel.ReducedEnable)
            {
                HelpModel.ViewVisibility = "Collapsed";
            }
            else
            {
                HelpModel.ViewVisibility = "Visible";
            }
        }
        #endregion

        #region 帮助
        public void Update()
        {

        }
        #endregion

        #endregion

        #region TCP Server

        #region 打开/关闭网络
        public bool TCPServerOpenCloseSocket()
        {
            if(SocketTCPServer != null)
            {
                return CloseTCPServerSocket();
            }

            IPAddress _IPAddress;

            /* 判断Socket参数（IP地址与端口号）是否合法 */
            if (TCPServerModel.SocketSrcIPAddrSelectedIndex >= 0)
            {
                /* 索引选择的IP地址 */
                _IPAddress = SocketModel.SocketSrcIPAddrItemsSource[TCPServerModel.SocketSrcIPAddrSelectedIndex];
            }
            else
            {
                /* 手动输入的IP地址 */
                if(SocketModel.TryParseIPAddressPort(TCPServerModel.SocketSrcIPAddrText, TCPServerModel.SocketSrcPort))
                {
                    _IPAddress = IPAddress.Parse(TCPServerModel.SocketSrcIPAddrText);

                    if(!SocketModel.SocketSrcIPAddrItemsSource.Contains(_IPAddress))
                    {
                        SocketModel.SocketSrcIPAddrItemsSource.Add(_IPAddress);
                        SocketModel.SocketSourceIPAddressItemsSource.Add(_IPAddress.ToString());
                    }
                }
                else
                {
                    DepictInfo = string.Format(cultureInfo, "请输入合法的IP地址和端口号");

                    return false;   /* Socket参数不合法，直接返回 */
                }
            }

            SocketTCPServer = new SocketTCPServer();

            /* 启动TCP服务器 */
            var _Started = SocketTCPServer.Start(_IPAddress, TCPServerModel.SocketSrcPort);

            if(_Started)
            {
                TCPServerModel.SocketSrcIPAddrEnable = false;
                TCPServerModel.SocketSrcPortEnable = false;

                TCPServerModel.SocketBrush = Brushes.GreenYellow;
                TCPServerModel.OpenCloseSocket = string.Format(cultureInfo, "TCP 断开");
            }
            else
            {
                SocketTCPServer = null;
                return false;
            }

            return true;
        }

        public bool CloseTCPServerSocket()
        {
            if(SocketTCPServer != null)
            {
                SocketTCPServer.Stop();
                SocketTCPServer = null;

                TCPServerModel.SocketSrcIPAddrEnable = true;
                TCPServerModel.SocketSrcPortEnable = true;

                TCPServerModel.SocketBrush = Brushes.Red;
                TCPServerModel.OpenCloseSocket = string.Format(cultureInfo, "TCP 侦听");

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 辅助区
        private bool _TCPServerHexSend;
        public bool TCPServerHexSend
        {
            get
            {
                return _TCPServerHexSend;
            }
            set
            {
                if (_TCPServerHexSend != value)
                {
                    _TCPServerHexSend = value;
                    RaisePropertyChanged(nameof(TCPServerHexSend));

                    if (TCPServerHexSend == true)
                    {
                        DepictInfo = string.Format(cultureInfo, "请输入十六进制数据用空格隔开，比如A0 B1 C2 D3");
                    }
                    else
                    {
                        DepictInfo = string.Format(cultureInfo, "网络端口调试助手");
                    }
                }
            }
        }

        private bool _TCPServerAutoSend;
        public bool TCPServerAutoSend
        {
            get
            {
                return _TCPServerAutoSend;
            }
            set
            {
                if (_TCPServerAutoSend != value)
                {
                    _TCPServerAutoSend = value;
                    RaisePropertyChanged(nameof(TCPServerAutoSend));
                }

                if (TCPServerAutoSend == true)
                {
                    if (SendModel.TCPServerAutoSendNum <= 0)
                    {
                        DepictInfo = string.Format(cultureInfo, "请输入正确的发送时间间隔");
                        return;
                    }

                    StartAutoSendTimer(SendModel.TCPServerAutoSendNum);
                }
                else
                {
                    StopAutoSendTimer();
                }
            }
        }

        private bool _TCPServerSaveRecv;
        public bool TCPServerSaveRecv
        {
            get
            {
                return _TCPServerSaveRecv;
            }
            set
            {
                if (_TCPServerSaveRecv != value)
                {
                    _TCPServerSaveRecv = value;
                    RaisePropertyChanged(nameof(TCPServerSaveRecv));
                }

                if (TCPServerSaveRecv)
                {
                    DepictInfo = "接收数据默认保存在程序基目录，可以点击路径选择操作更换";
                }
                else
                {
                    DepictInfo = "网络端口调试助手";
                }
            }
        }
        #endregion

        #endregion

        #region TCP Client

        #region 打开/关闭网络
        public bool TCPClientOpenCloseSocket()
        {
            if(SocketTCPClient != null)
            {
                return CloseTCPClientSocket();
            }

            IPAddress _IPAddress;

            /* 判断Socket参数（IP地址与端口号）是否合法 */
            if (SocketModel.TryParseIPAddressPort(TCPClientModel.SocketDestIPAddrText, TCPClientModel.SocketDestPort))
            {
                _IPAddress = IPAddress.Parse(TCPClientModel.SocketDestIPAddrText);

                if(!TCPClientModel.SocketDestIPAddrItemsSource.Contains(_IPAddress))
                {
                    TCPClientModel.SocketDestIPAddrItemsSource.Add(_IPAddress);
                }
            }
            else
            {
                DepictInfo = string.Format(cultureInfo, "请输入合法的IP地址和端口号");

                return false;   /* Socket参数不合法，直接返回 */
            }

            SocketTCPClient = new SocketTCPClient();

            /* 连接TCP服务器 */
            var _Connected = SocketTCPClient.Connect(_IPAddress, TCPClientModel.SocketDestPort);

            if(_Connected)
            {
                TCPClientModel.SocketDestIPAddrEnable = false;
                TCPClientModel.SocketDestPortEnable = false;

                TCPClientModel.SocketBrush = Brushes.GreenYellow;
                TCPClientModel.OpenCloseSocket = string.Format(cultureInfo, "TCP 断开");
            }
            else
            {
                SocketTCPClient = null;

                return false;
            }

            return true;
        }

        public bool CloseTCPClientSocket()
        {
            if(SocketTCPClient != null)
            {
                SocketTCPClient.DisConnect();
                SocketTCPClient = null;

                TCPClientModel.SocketDestIPAddrEnable = true;
                TCPClientModel.SocketDestPortEnable = true;

                TCPClientModel.SocketBrush = Brushes.Red;
                TCPClientModel.OpenCloseSocket = string.Format(cultureInfo, "TCP 连接");

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 辅助区
        private bool _TCPClientHexSend;
        public bool TCPClientHexSend
        {
            get
            {
                return _TCPClientHexSend;
            }
            set
            {
                if (_TCPClientHexSend != value)
                {
                    _TCPClientHexSend = value;
                    RaisePropertyChanged(nameof(TCPClientHexSend));

                    if (TCPClientHexSend == true)
                    {
                        DepictInfo = string.Format(cultureInfo, "请输入十六进制数据用空格隔开，比如A0 B1 C2 D3");
                    }
                    else
                    {
                        DepictInfo = string.Format(cultureInfo, "网络端口调试助手");
                    }
                }
            }
        }

        private bool _TCPClientAutoSend;
        public bool TCPClientAutoSend
        {
            get
            {
                return _TCPClientAutoSend;
            }
            set
            {
                if (_TCPClientAutoSend != value)
                {
                    _TCPClientAutoSend = value;
                    RaisePropertyChanged(nameof(TCPClientAutoSend));
                }

                if (TCPClientAutoSend == true)
                {
                    if (SendModel.TCPClientAutoSendNum <= 0)
                    {
                        DepictInfo = string.Format(cultureInfo, "请输入正确的发送时间间隔");
                        return;
                    }

                    StartAutoSendTimer(SendModel.TCPServerAutoSendNum);
                }
                else
                {
                    StopAutoSendTimer();
                }
            }
        }

        private bool _TCPClientSaveRecv;
        public bool TCPClientSaveRecv
        {
            get
            {
                return _TCPClientSaveRecv;
            }
            set
            {
                if (_TCPClientSaveRecv != value)
                {
                    _TCPClientSaveRecv = value;
                    RaisePropertyChanged(nameof(TCPClientSaveRecv));
                }

                if (TCPClientSaveRecv)
                {
                    DepictInfo = "接收数据默认保存在程序基目录，可以点击路径选择操作更换";
                }
                else
                {
                    DepictInfo = "网络端口调试助手";
                }
            }
        }
        #endregion

        #endregion

        #region UDP Server

        #region 打开/关闭网络
        public void UDPServerOpenCloseSocket()
        {
            
        }
        #endregion

        #region 辅助区
        private bool _UDPServerHexSend;
        public bool UDPServerHexSend
        {
            get
            {
                return _UDPServerHexSend;
            }
            set
            {
                if (_UDPServerHexSend != value)
                {
                    _UDPServerHexSend = value;
                    RaisePropertyChanged(nameof(UDPServerHexSend));

                    if (UDPServerHexSend == true)
                    {
                        DepictInfo = string.Format(cultureInfo, "请输入十六进制数据用空格隔开，比如A0 B1 C2 D3");
                    }
                    else
                    {
                        DepictInfo = string.Format(cultureInfo, "网络端口调试助手");
                    }
                }
            }
        }

        private bool _UDPServerAutoSend;
        public bool UDPServerAutoSend
        {
            get
            {
                return _UDPServerAutoSend;
            }
            set
            {
                if (_UDPServerAutoSend != value)
                {
                    _UDPServerAutoSend = value;
                    RaisePropertyChanged(nameof(UDPServerAutoSend));
                }

                if (UDPServerAutoSend == true)
                {
                    if (SendModel.UDPServerAutoSendNum <= 0)
                    {
                        DepictInfo = string.Format(cultureInfo, "请输入正确的发送时间间隔");
                        return;
                    }

                    StartAutoSendTimer(SendModel.TCPServerAutoSendNum);
                }
                else
                {
                    StopAutoSendTimer();
                }
            }
        }

        private bool _UDPServerSaveRecv;
        public bool UDPServerSaveRecv
        {
            get
            {
                return _UDPServerSaveRecv;
            }
            set
            {
                if (_UDPServerSaveRecv != value)
                {
                    _UDPServerSaveRecv = value;
                    RaisePropertyChanged(nameof(UDPServerSaveRecv));
                }

                if (UDPServerSaveRecv)
                {
                    DepictInfo = "接收数据默认保存在程序基目录，可以点击路径选择操作更换";
                }
                else
                {
                    DepictInfo = "网络端口调试助手";
                }
            }
        }
        #endregion

        #endregion

        #region UDP Client

        #region 打开/关闭网络
        public void UDOClientOpenCloseSocket()
        {

        }
        #endregion

        #region 辅助区
        private bool _UDPClientHexSend;
        public bool UDPClientHexSend
        {
            get
            {
                return _UDPClientHexSend;
            }
            set
            {
                if (_UDPClientHexSend != value)
                {
                    _UDPClientHexSend = value;
                    RaisePropertyChanged(nameof(UDPClientHexSend));

                    if (UDPClientHexSend == true)
                    {
                        DepictInfo = string.Format(cultureInfo, "请输入十六进制数据用空格隔开，比如A0 B1 C2 D3");
                    }
                    else
                    {
                        DepictInfo = string.Format(cultureInfo, "网络端口调试助手");
                    }
                }
            }
        }

        private bool _UDPClientAutoSend;
        public bool UDPClientAutoSend
        {
            get
            {
                return _UDPClientAutoSend;
            }
            set
            {
                if (_UDPClientAutoSend != value)
                {
                    _UDPClientAutoSend = value;
                    RaisePropertyChanged(nameof(UDPClientAutoSend));
                }

                if (UDPClientAutoSend == true)
                {
                    if (SendModel.UDPClientAutoSendNum <= 0)
                    {
                        DepictInfo = string.Format(cultureInfo, "请输入正确的发送时间间隔");
                        return;
                    }

                    StartAutoSendTimer(SendModel.TCPServerAutoSendNum);
                }
                else
                {
                    StopAutoSendTimer();
                }
            }
        }

        private bool _UDPClientSaveRecv;
        public bool UDPClientSaveRecv
        {
            get
            {
                return _UDPClientSaveRecv;
            }
            set
            {
                if (_UDPClientSaveRecv != value)
                {
                    _UDPClientSaveRecv = value;
                    RaisePropertyChanged(nameof(UDPClientSaveRecv));
                }

                if (UDPClientSaveRecv)
                {
                    DepictInfo = "接收数据默认保存在程序基目录，可以点击路径选择操作更换";
                }
                else
                {
                    DepictInfo = "网络端口调试助手";
                }
            }
        }
        #endregion

        #endregion

        #region 自动发送定时器实现
        private readonly DispatcherTimer AutoSendDispatcherTimer = new DispatcherTimer();

        private void InitAutoSendTimer()
        {
            AutoSendDispatcherTimer.IsEnabled = false;
            AutoSendDispatcherTimer.Tick += AutoSendDispatcherTimer_Tick;
        }

        private void AutoSendDispatcherTimer_Tick(object sender, EventArgs e)
        {
            
        }

        private void StartAutoSendTimer(int interval)
        {
            AutoSendDispatcherTimer.IsEnabled = true;
            AutoSendDispatcherTimer.Interval = TimeSpan.FromMilliseconds(interval);
            AutoSendDispatcherTimer.Start();
        }

        private void StopAutoSendTimer()
        {
            AutoSendDispatcherTimer.IsEnabled = false;
            AutoSendDispatcherTimer.Stop();
        }
        #endregion

        #region 路径选择
        public void SaveRecvPath()
        {
            SaveFileDialog ReceDataSaveFileDialog = new SaveFileDialog
            {
                Title = string.Format(cultureInfo, "接收数据保存"),
                FileName = string.Format(cultureInfo, "{0}", DateTime.Now.ToString("yyyyMMdd", cultureInfo)),
                DefaultExt = ".txt",
                Filter = string.Format(cultureInfo, "文本文件|*.txt")
            };

            if (ReceDataSaveFileDialog.ShowDialog() == true)
            {
                _ = ReceDataSaveFileDialog.FileName;
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            SocketModel = new SocketModel();
            SocketModel.SocketDataContext();

            TCPServerModel = new TCPServerModel();
            TCPServerModel.TCPServerDataContext();

            TCPClientModel = new TCPClientModel();
            TCPClientModel.TCPClientDataContext();

            UDPServerModel = new UDPServerModel();
            UDPServerModel.UDPServerDataContext();

            UDPClientModel = new UDPClientModel();
            UDPClientModel.UDPClientDataContext();

            SendModel = new SendModel();
            SendModel.SendDataContext();

            RecvModel = new RecvModel();
            RecvModel.RecvDataContext();

            TimerModel = new TimerModel();
            TimerModel.TimerDataContext();

            HelpModel = new HelpModel();
            HelpModel.HelpDataContext();

            LatestRelease = new GitRelease();

            TCPServerHexSend = false;
            TCPServerAutoSend = false;
            TCPServerSaveRecv = false;

            TCPClientHexSend = false;
            TCPClientAutoSend = false;
            TCPClientSaveRecv = false;

            UDPServerHexSend = false;
            UDPServerAutoSend = false;
            UDPServerSaveRecv = false;

            UDPClientHexSend = false;
            UDPClientAutoSend = false;
            UDPClientSaveRecv = false;

            InitAutoSendTimer();

            DepictInfo = string.Format(cultureInfo, "网络端口调试助手");
        }
    }
}
