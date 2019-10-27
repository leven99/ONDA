using SocketDA.Models;

namespace SocketDA.ViewModels
{
    internal partial class MainWindowViewModel : MainWindowBase
    {
        public TCPServerModel TCPServerModel { get; set; }
        public TCPClientModel TCPClientModel { get; set; }
        public UDPServerModel UDPServerModel { get; set; }
        public UDPClientModel UDPClientModel { get; set; }
        public SendModel SendModel { get; set; }
        public RecvModel RecvModel { get; set; }
        public TimerModel TimerModel { get; set; }
        public HelpModel HelpModel { get; set; }

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

        public MainWindowViewModel()
        {
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

            TCPServerHexSend = false;
            TCPServerSaveRecv = false;
            TCPServerAutoSend = false;
            TCPServerInitAutoSendTimer();

            TCPClientHexSend = false;
            TCPClientSaveRecv = false;
            TCPClientAutoSend = false;
            TCPClientInitAutoSendTimer();

            UDPServerHexSend = false;
            UDPServerSaveRecv = false;
            UDPServerAutoSend = false;
            UDPServerInitAutoSendTimer();

            UDPClientHexSend = false;
            UDPClientSaveRecv = false;
            UDPClientAutoSend = false;
            UDPClientInitAutoSendTimer();

            DepictInfo = string.Format(cultureInfo, "网络端口调试助手");
        }
    }
}
