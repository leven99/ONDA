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

        /// <summary>
        /// 连接服务器请求信号量
        /// </summary>
        protected Semaphore TCPClientSemaphoreConnections = null;

        protected Socket TCPClientSocketConnections = null;
        protected IPEndPoint TCPClientIPEndPointConnections = null;
        protected SocketAsyncEventArgs TCPClientSocketAsyncEventArgsConnections = null;
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

            TCPClientSemaphoreConnections = new Semaphore(1, 1);   /* 一个客户端程序只连接一个服务器 */
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
                case SocketAsyncOperation.Disconnect:
                    TCPClientProcessDisconnect(asyncEventArgs);
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
        public bool TCPClientConnect(IPAddress ipAddress, int port)
        {
            TCPClientSocketConnections = _TCPClientSocketBase.CreateSocket(ipAddress, port, ProtocolType.Tcp);
            TCPClientIPEndPointConnections = _TCPClientSocketBase.IPEndPoint;
            TCPClientSocketConnections.SetSocketOption(
                SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, _TCPClientSocketSetting.ReceiveBufferSize);
            TCPClientSocketConnections.SetSocketOption(
                SocketOptionLevel.Socket, SocketOptionName.SendBuffer, _TCPClientSocketSetting.SendBufferSize);

            try
            {
                TCPClientSocketAsyncEventArgsConnections.RemoteEndPoint = TCPClientIPEndPointConnections;
                TCPClientSocketAsyncEventArgsConnections.AcceptSocket = TCPClientSocketConnections;

                TCPClientStartConnectAsync(TCPClientSocketAsyncEventArgsConnections);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 开始连接服务器
        /// </summary>
        /// <param name="connectEventArgs"></param>
        private void TCPClientStartConnectAsync(SocketAsyncEventArgs connectEventArgs)
        {
            TCPClientSemaphoreConnections.WaitOne();

            bool _ConnectPending = TCPClientSocketConnections.ConnectAsync(connectEventArgs);

            if (!_ConnectPending)
            {
                TCPClientProcessConnect(connectEventArgs);
            }
        }

        /// <summary>
        /// 处理已完成连接的服务器SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="connectEventArgs"></param>
        private void TCPClientProcessConnect(SocketAsyncEventArgs connectEventArgs)
        {
            try
            {
                if (connectEventArgs != null)
                {
                    connectEventArgs.UserToken = new SocketUserToKen(connectEventArgs.AcceptSocket);
                }
                else
                {
                    /* 服务器已关闭 */
                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// 处理已断开连接的服务器SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="disconnectEventArgs"></param>
        private void TCPClientProcessDisconnect(SocketAsyncEventArgs disconnectEventArgs)
        {
            throw new NotImplementedException();
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
        public void TCPClientDisConnect()
        {
            TCPClientSocketConnections.Close();
            TCPClientSemaphoreConnections.Release();
        }

        #region 打开/关闭网络
        public void TCPClientOpenCloseSocket()
        {
            IPAddress _IPAddress;

            if (SocketModel.TryParseIPAddressPort(TCPClientModel.SocketDestIPAddrText, TCPClientModel.SocketDestPort))
            {
                _IPAddress = IPAddress.Parse(TCPClientModel.SocketDestIPAddrText);

                if (!TCPClientModel.SocketDestIPAddrItemsSource.Contains(_IPAddress))
                {
                    TCPClientModel.SocketDestIPAddrItemsSource.Add(_IPAddress);
                }
            }
            else
            {
                DepictInfo = string.Format(cultureInfo, "请输入合法的IP地址和端口号");

                return;
            }

            TCPClientInit();

            var _Connected = TCPClientConnect(_IPAddress, TCPClientModel.SocketDestPort);

            if (_Connected)
            {
                TCPClientModel.SocketDestIPAddrEnable = false;
                TCPClientModel.SocketDestPortEnable = false;

                TCPClientModel.SocketBrush = Brushes.GreenYellow;
                TCPClientModel.OpenCloseSocket = string.Format(cultureInfo, "TCP 断开");
            }
            else
            {
                DepictInfo = string.Format(cultureInfo, "连接服务器失败");

                return;
            }
        }

        public void CloseTCPClientSocket()
        {
            TCPClientModel.SocketDestIPAddrEnable = true;
            TCPClientModel.SocketDestPortEnable = true;

            TCPClientModel.SocketBrush = Brushes.Red;
            TCPClientModel.OpenCloseSocket = string.Format(cultureInfo, "TCP 连接");
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
