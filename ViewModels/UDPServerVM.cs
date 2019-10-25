using Microsoft.Win32;
using SocketDA.ModelsSocket;
using System;
using System.Windows.Threading;

namespace SocketDA.ViewModels
{
    internal partial class MainWindowViewModel : MainWindowBase
    {
        #region 字段
        protected SocketBase UDPServerSocketBase = new SocketBase();
        #endregion

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

                    UDPServerStartAutoSendTimer(SendModel.TCPServerAutoSendNum);
                }
                else
                {
                    UDPServerStopAutoSendTimer();
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

        #region 自动发送定时器实现
        private readonly DispatcherTimer UDPServerAutoSendDispatcherTimer = new DispatcherTimer();

        private void UDPServerInitAutoSendTimer()
        {
            UDPServerAutoSendDispatcherTimer.IsEnabled = false;
            UDPServerAutoSendDispatcherTimer.Tick += UDPServerAutoSendDispatcherTimer_Tick;
        }

        private void UDPServerAutoSendDispatcherTimer_Tick(object sender, EventArgs e)
        {

        }

        private void UDPServerStartAutoSendTimer(int interval)
        {
            UDPServerAutoSendDispatcherTimer.IsEnabled = true;
            UDPServerAutoSendDispatcherTimer.Interval = TimeSpan.FromMilliseconds(interval);
            UDPServerAutoSendDispatcherTimer.Start();
        }

        private void UDPServerStopAutoSendTimer()
        {
            UDPServerAutoSendDispatcherTimer.IsEnabled = false;
            UDPServerAutoSendDispatcherTimer.Stop();
        }
        #endregion

        #region 路径选择
        public void UDPServerSaveRecvPath()
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
