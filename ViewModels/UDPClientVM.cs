using Microsoft.Win32;
using SocketDA.ModelsSocket;
using System;
using System.Windows.Threading;

namespace SocketDA.ViewModels
{
    internal partial class MainWindowViewModel : MainWindowBase
    {
        #region 字段
        protected SocketBase UDPClientSocketBase = new SocketBase();
        #endregion

        #region 打开/关闭网络
        public void UDPClientOpenCloseSocket()
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

                    UDPClientStartAutoSendTimer(SendModel.TCPServerAutoSendNum);
                }
                else
                {
                    UDPClientStopAutoSendTimer();
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

        #region 自动发送定时器实现
        private readonly DispatcherTimer UDPClientAutoSendDispatcherTimer = new DispatcherTimer();

        private void UDPClientInitAutoSendTimer()
        {
            UDPClientAutoSendDispatcherTimer.IsEnabled = false;
            UDPClientAutoSendDispatcherTimer.Tick += UDPClientAutoSendDispatcherTimer_Tick;
        }

        private void UDPClientAutoSendDispatcherTimer_Tick(object sender, EventArgs e)
        {

        }

        private void UDPClientStartAutoSendTimer(int interval)
        {
            UDPClientAutoSendDispatcherTimer.IsEnabled = true;
            UDPClientAutoSendDispatcherTimer.Interval = TimeSpan.FromMilliseconds(interval);
            UDPClientAutoSendDispatcherTimer.Start();
        }

        private void UDPClientStopAutoSendTimer()
        {
            UDPClientAutoSendDispatcherTimer.IsEnabled = false;
            UDPClientAutoSendDispatcherTimer.Stop();
        }
        #endregion

        #region 路径选择
        public void UDPClientSaveRecvPath()
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

        #region 发送
        #endregion

        #region 发送文件
        #endregion

        #region 路径选择
        #endregion

        #region 清空接收
        #endregion

        #region 清空发送
        #endregion

        #region 清空计数
        #endregion
    }
}
