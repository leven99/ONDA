using Microsoft.Win32;
using SocketDA.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Threading;

namespace SocketDA.ViewModels
{
    internal class MainWindowViewModel : MainWindowBase
    {
        #region 字段
        private Socket SocketBase = null;

        private readonly int SocketListenBacklog = 10;

        private readonly ManualResetEvent AllDone = new ManualResetEvent(false);
        private readonly ManualResetEvent ConnectDone = new ManualResetEvent(false);
        private readonly ManualResetEvent SendDone = new ManualResetEvent(false);
        private readonly ManualResetEvent ReceiveDone = new ManualResetEvent(false);

        private string DataRecvPath = string.Empty;   /* 数据接收路径 */
        #endregion

        public SocketModel SocketModel { get; set; }
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
            if (SocketBase != null)
            {
                CloseSocket(SocketBase);
            }
        }
        #endregion

        #region 选项
        #endregion

        #region 视图
        public void AssistReduced_Enable()
        {
            HelpModel.AssistReducedEnable = !HelpModel.AssistReducedEnable;

            if(HelpModel.AssistReducedEnable)
            {
                HelpModel.AssistViewVisibility = "Collapsed";
            }
            else
            {
                HelpModel.AssistViewVisibility = "Visible";
            }
        }

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

            AssistReduced_Enable();
        }
        #endregion

        #region 帮助
        public void Update()
        {

        }
        #endregion

        #endregion

        #region 打开/关闭套接字
        public void OpenSocket()
        {
            if(SocketBase != null)
            {
                CloseSocket(SocketBase);
            }

            try
            {
                /* 对目的IP地址判断有效性 */
                if (IPAddress.TryParse(SocketModel.SocketDestinationIPAddressText, out _))
                {
                    IPAddress _IPAddress = IPAddress.Parse(SocketModel.SocketDestinationIPAddressText);

                    if (!SocketModel.SocketDestinationIPAddressItemsSource.Contains(_IPAddress))
                    {
                        SocketModel.SocketDestinationIPAddressItemsSource.Add(_IPAddress);
                    }

                    SocketModel.SocketDestinationIPAddress = _IPAddress;
                }

                /* 对目的端口号、源端口号判断有效性 */

                /* TCP Server */
                if (SocketModel.SocketProtocolSelectedIndex == 0)
                {
                    IPEndPoint localIPEndPoint = new IPEndPoint(SocketModel.SocketSourceIPAddress, SocketModel.SocketSourcePort);

                    /* 创建 TCP/IP Socket（区分 IPv4、IPv6） */
                    if (SocketModel.SocketSourceIPAddress.IsIPv6LinkLocal)
                    {
                        SocketBase = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.IPv6);
                    }
                    else
                    {
                        SocketBase = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IPv4);
                    }

                    /* 绑定本地Point */
                    SocketBase.Bind(localIPEndPoint);

                    /* 侦听客户端连接 */
                    SocketBase.Listen(SocketListenBacklog);

                    SocketModel.OpenCloseSocket = string.Format(cultureInfo, "TCP 断开");
                }
                /* TCP Client */
                else if (SocketModel.SocketProtocolSelectedIndex == 1)
                {
                    IPEndPoint RemoteIPEndPoint = new IPEndPoint(SocketModel.SocketDestinationIPAddress, SocketModel.SocketDestinationPort);

                    /* 创建 TCP/IP Socket（区分 IPv4、IPv6） */
                    if (SocketModel.SocketDestinationIPAddress.IsIPv6LinkLocal)
                    {
                        SocketBase = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.IPv6);
                    }
                    else
                    {
                        SocketBase = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IPv4);
                    }

                    /* 连接远程主机（服务器） */
                    SocketBase.BeginConnect(RemoteIPEndPoint, new AsyncCallback(ConnectCallback), SocketBase);
                    ConnectDone.WaitOne();

                    SocketModel.OpenCloseSocket = string.Format(cultureInfo, "TCP 断开");
                }
                /* UDP Server */
                else if (SocketModel.SocketProtocolSelectedIndex == 2)
                {
                    SocketModel.OpenCloseSocket = string.Format(cultureInfo, "UDP 断开");
                }
                /* UDP Client */
                else if (SocketModel.SocketProtocolSelectedIndex == 3)
                {
                    SocketModel.OpenCloseSocket = string.Format(cultureInfo, "UDP 断开");
                }
            }
            catch
            {
                DepictInfo = string.Format(cultureInfo, "请检查参数是否正确！");
            }
        }

        public void CloseSocket(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch
            {

            }
        }
        #endregion

        #region Socket回调函数
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                /* 主线程继续 */
                AllDone.Set();

                /* 获取并处理客户端请求的套接字 */
                Socket _Socket = (Socket)ar.AsyncState;
                Socket _SocketAccept = _Socket.EndAccept(ar);

                /* 创建状态对象（state object）*/
                StateObject _StateObject = new StateObject();
                _StateObject.workSocket = _SocketAccept;

                /* 从客户端接收数据 */
            }
            catch
            {

            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                /* 从状态对象（state object）检索 Socket */
                Socket _Socket = (Socket)ar.AsyncState;

                /* 连接服务器 */
                _Socket.EndConnect(ar);

                /* 已连接服务器 */
                ConnectDone.Set();
            }
            catch
            {

            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                /* 从状态对象（state object）检索 Socket */
                StateObject _StateObject = (StateObject)ar.AsyncState;
                Socket _Socket = _StateObject.workSocket;

                /* 从远程设备接收数据 */
                int _BytesSent = _Socket.EndReceive(ar);

                if(_BytesSent > 0)
                {

                }
            }
            catch
            {

            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                /* 从状态对象（state object）检索 Socket */
                Socket _Socket = (Socket)ar.AsyncState;

                /* 向远程设备发送数据 */
                int _BytesSent = _Socket.EndSend(ar);

                /* 已发送数据 */
                SendDone.Set();
            }
            catch
            {

            }
        }
        #endregion

        #region 辅助区
        private bool _HexSend;
        public bool HexSend
        {
            get
            {
                return _HexSend;
            }
            set
            {
                if (_HexSend != value)
                {
                    _HexSend = value;
                    RaisePropertyChanged(nameof(HexSend));

                    if (HexSend == true)
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

        private bool _AutoSend;
        public bool AutoSend
        {
            get
            {
                return _AutoSend;
            }
            set
            {
                if (_AutoSend != value)
                {
                    _AutoSend = value;
                    RaisePropertyChanged(nameof(AutoSend));
                }

                if (AutoSend == true)
                {
                    if (SendModel.AutoSendNum <= 0)
                    {
                        DepictInfo = string.Format(cultureInfo, "请输入正确的发送时间间隔");
                        return;
                    }

                    StartAutoSendTimer(SendModel.AutoSendNum);
                }
                else
                {
                    StopAutoSendTimer();
                }
            }
        }

        private bool _SaveRecv;
        public bool SaveRecv
        {
            get
            {
                return _SaveRecv;
            }
            set
            {
                if (_SaveRecv != value)
                {
                    _SaveRecv = value;
                    RaisePropertyChanged(nameof(SaveRecv));
                }

                if (SaveRecv)
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

        #region 自动发送定时器实现
        private readonly DispatcherTimer AutoSendDispatcherTimer = new DispatcherTimer();

        private void InitAutoSendTimer()
        {
            AutoSendDispatcherTimer.IsEnabled = false;
            AutoSendDispatcherTimer.Tick += AutoSendDispatcherTimer_Tick;
        }

        private void AutoSendDispatcherTimer_Tick(object sender, EventArgs e)
        {
            Send();
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

        #region 发送
        public void Send()
        {

        }
        #endregion

        #region 发送文件
        public void SendFile()
        {

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
                DataRecvPath = ReceDataSaveFileDialog.FileName;
            }
        }
        #endregion

        #region 清接收区
        public void ClarReceData()
        {

        }
        #endregion

        #region 清发送区
        public void ClearSendData()
        {
            SendModel.SendData = string.Empty;
        }
        #endregion

        #region 清空计数
        public void ClearCount()
        {

        }
        #endregion

        #region 保存接收数据
        public void SaveRecvData(string ReceData)
        {
            try
            {
                if (DataRecvPath == null)
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\ReceData\\");
                }
                else
                {

                }
            }
            catch
            {

            }
        }
        #endregion

        #region Combobox Support
        public void ProtocolComboBox_SelectionChanged()
        {
            if(SocketModel.SocketProtocolSelectedIndex == 0)
            {
                SocketModel.DestinationVisibility = "Collapsed";
                SocketModel.OpenCloseSocket = string.Format(cultureInfo, "TCP 侦听");
            }
            else if (SocketModel.SocketProtocolSelectedIndex == 1)
            {
                SocketModel.DestinationVisibility = "Visible";
                SocketModel.OpenCloseSocket = string.Format(cultureInfo, "TCP 连接");
            }
            else if (SocketModel.SocketProtocolSelectedIndex == 2)
            {
                SocketModel.DestinationVisibility = "Collapsed";
                SocketModel.OpenCloseSocket = string.Format(cultureInfo, "UDP 侦听");
            }
            else if (SocketModel.SocketProtocolSelectedIndex == 3)
            {
                SocketModel.DestinationVisibility = "Visible";
                SocketModel.OpenCloseSocket = string.Format(cultureInfo, "UDP 连接");
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            SocketModel = new SocketModel();
            SocketModel.SocketDataContext();

            SendModel = new SendModel();
            SendModel.SendDataContext();

            RecvModel = new RecvModel();
            RecvModel.RecvDataContext();

            TimerModel = new TimerModel();
            TimerModel.TimerDataContext();

            HelpModel = new HelpModel();
            HelpModel.HelpDataContext();

            SaveRecv = false;
            HexSend = false;
            AutoSend = false;
            InitAutoSendTimer();

            DepictInfo = string.Format(cultureInfo, "网络端口调试助手");
        }
    }
}
