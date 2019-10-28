using Microsoft.Win32;
using SocketDA.Models;
using SocketDA.ModelsSocket;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;

namespace SocketDA.ViewModels
{
    internal partial class MainWindowViewModel : MainWindowBase
    {
        #region 字段
        protected SocketBase _TCPClientSocketBase = new SocketBase();
        protected SocketSetting _TCPClientSocketSetting = new SocketSetting();

        protected Socket TCPClientSocketConnections = null;
        protected SocketAsyncEventArgs TCPClientSocketAsyncEventArgsConnections = null;

        protected bool TCPClientConnectFlag = false;   /* 冗余判断客户端是否已经连接到服务器 */
        #endregion

        /// <summary>
        /// 初始化客户端
        /// </summary>
        public void TCPClientInit()
        {
            TCPClientSocketAsyncEventArgsConnections = new SocketAsyncEventArgs();
            TCPClientSocketAsyncEventArgsConnections.Completed += new EventHandler<SocketAsyncEventArgs>(TCPClientOnIOCompleted);
            TCPClientSocketAsyncEventArgsConnections.SetBuffer(
                new byte[_TCPClientSocketSetting.BufferSize * _TCPClientSocketSetting.OpsToPreAlloc],
                0, _TCPClientSocketSetting.OpsToPreAlloc * _TCPClientSocketSetting.BufferSize);
        }

        /// <summary>
        /// 当SocketAsyncEventArgs对象完成接收、发送和连接时，调用此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="asyncEventArgs"></param>
        private void TCPClientOnIOCompleted(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            switch (asyncEventArgs.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    TCPClientProcessReceive(asyncEventArgs);
                    break;
                case SocketAsyncOperation.Send:
                    TCPClientProcessSend(asyncEventArgs);
                    break;
                case SocketAsyncOperation.Connect:
                    TCPClientProcessConnect(asyncEventArgs);
                    break;
                default:
                    throw new ArgumentException(asyncEventArgs.ConnectByNameError.Message);
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public void TCPClientConnect(IPAddress ipAddress, int port)
        {
            TCPClientSocketConnections = _TCPClientSocketBase.CreateSocket(ipAddress, port, ProtocolType.Tcp);
            TCPClientSocketConnections.SetSocketOption(
                SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, _TCPClientSocketSetting.ReceiveBufferSize);
            TCPClientSocketConnections.SetSocketOption(
                SocketOptionLevel.Socket, SocketOptionName.SendBuffer, _TCPClientSocketSetting.SendBufferSize);

            TCPClientSocketAsyncEventArgsConnections.RemoteEndPoint = _TCPClientSocketBase.IPEndPoint;

            bool _ConnectPending = TCPClientSocketConnections.ConnectAsync(TCPClientSocketAsyncEventArgsConnections);

            DepictInfo = string.Format(cultureInfo, "正在连接服务器，请稍后......");

            if (!_ConnectPending)
            {
                TCPClientProcessConnect(TCPClientSocketAsyncEventArgsConnections);
            }
        }

        /// <summary>
        /// 处理已完成连接的服务器SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="connectEventArgs"></param>
        private void TCPClientProcessConnect(SocketAsyncEventArgs connectEventArgs)
        {
            if(connectEventArgs.SocketError == SocketError.Success)
            {
                TCPClientModel.IPAddrEnable = false;
                TCPClientModel.PortEnable = false;

                TCPClientModel.Brush = Brushes.GreenYellow;
                TCPClientModel.OpenClose = string.Format(cultureInfo, "TCP 断开");

                TCPClientConnectFlag = true;

                DepictInfo = string.Format(cultureInfo, "成功连接到服务器");
            }
            else if (connectEventArgs.SocketError == SocketError.TimedOut)
            {
                DepictInfo = string.Format(cultureInfo, "连接服务器超时或服务器未能响应......");
                return;
            }
            else
            {
                DepictInfo = string.Format(cultureInfo, "连接服务器失败！");
                return;
            }

            connectEventArgs.AcceptSocket = TCPClientSocketConnections;
            connectEventArgs.UserToken = new SocketUserToKen(connectEventArgs.AcceptSocket);

            /* 将服务器信息加入到连接区 */
            ThreadPool.QueueUserWorkItem(delegate
            {
                SynchronizationContext.SetSynchronizationContext(new
                    DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                SynchronizationContext.Current.Send(pl =>
                {
                    TCPClientModel.ConnectionsInfo.Add(new TCPClientConnectionsInfo()
                    {
                        RemoteEndPoint = connectEventArgs.AcceptSocket.RemoteEndPoint.ToString()
                    });
                }, null);
            });
        }

        /// <summary>
        /// 处理接收数据，数据来自服务器SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="receiveEventArgs"></param>
        private void TCPClientProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 处理发送数据，数据发送给客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="sendEventArg"></param>
        private void TCPClientProcessSend(SocketAsyncEventArgs sendEventArg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        public bool TCPClientDisConnect()
        {
            try
            {
                TCPClientSocketConnections.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        #region 打开/关闭网络
        public void TCPClientOpenCloseSocket()
        {
            if (TCPClientConnectFlag)
            {
                CloseTCPClientSocket();

                return;
            }

            IPAddress _IPAddress;

            SocketBase _SocketBase = new SocketBase();

            if (_SocketBase.TryParseIPAddressPort(TCPClientModel.IPAddrText, TCPClientModel.Port))
            {
                _IPAddress = IPAddress.Parse(TCPClientModel.IPAddrText);

                if (!TCPClientModel.IPAddrItemsSource.Contains(_IPAddress))
                {
                    TCPClientModel.IPAddrItemsSource.Add(_IPAddress);
                }
            }
            else
            {
                DepictInfo = string.Format(cultureInfo, "请输入合法的IP地址和端口号");

                return;
            }

            TCPClientInit();

            TCPClientConnect(_IPAddress, TCPClientModel.Port);
        }

        public void CloseTCPClientSocket()
        {
            if(TCPClientDisConnect())
            {
                TCPClientModel.IPAddrEnable = true;
                TCPClientModel.PortEnable = true;

                TCPClientModel.Brush = Brushes.Red;
                TCPClientModel.OpenClose = string.Format(cultureInfo, "TCP 连接");

                TCPClientConnectFlag = false;
                DepictInfo = string.Format(cultureInfo, "成功断开服务器");
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

                    TCPClientStartAutoSendTimer(SendModel.TCPServerAutoSendNum);
                }
                else
                {
                    TCPClientStopAutoSendTimer();
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

        #region 自动发送定时器实现
        private readonly DispatcherTimer TCPClientAutoSendDispatcherTimer = new DispatcherTimer();

        private void TCPClientInitAutoSendTimer()
        {
            TCPClientAutoSendDispatcherTimer.IsEnabled = false;
            TCPClientAutoSendDispatcherTimer.Tick += TCPClientAutoSendDispatcherTimer_Tick;
        }

        private void TCPClientAutoSendDispatcherTimer_Tick(object sender, EventArgs e)
        {

        }

        private void TCPClientStartAutoSendTimer(int interval)
        {
            TCPClientAutoSendDispatcherTimer.IsEnabled = true;
            TCPClientAutoSendDispatcherTimer.Interval = TimeSpan.FromMilliseconds(interval);
            TCPClientAutoSendDispatcherTimer.Start();
        }

        private void TCPClientStopAutoSendTimer()
        {
            TCPClientAutoSendDispatcherTimer.IsEnabled = false;
            TCPClientAutoSendDispatcherTimer.Stop();
        }
        #endregion

        #region 路径选择
        public void TCPClientSaveRecvPath()
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
