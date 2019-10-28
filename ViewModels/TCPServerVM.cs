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
        protected static SocketBase _TCPServerSocketBase = new SocketBase();
        protected static SocketSetting _TCPServerSocketSetting = new SocketSetting();
        protected static SocketBufferManager _TCPServerSocketBufferManager = new SocketBufferManager(
            _TCPServerSocketSetting.DefaultMaxConnctions * _TCPServerSocketSetting.OpsToPreAlloc * _TCPServerSocketSetting.BufferSize,
            _TCPServerSocketSetting.OpsToPreAlloc * _TCPServerSocketSetting.BufferSize);
        protected static SocketAsyncEventArgsPool _TCPServerSocketAsyncEventArgsPool = new SocketAsyncEventArgsPool();

        protected Semaphore TCPServerSemaphoreConnections = new Semaphore(
            _TCPServerSocketSetting.DefaultMaxConnctions, _TCPServerSocketSetting.DefaultMaxConnctions);

        protected Socket TCPServerSocketConnections = null;

        protected bool TCPServerStartFlag = false;   /* 冗余判断服务器是否启动 */
        #endregion

        /// <summary>
        /// 初始化服务器
        /// </summary>
        public void TCPServerInit()
        {
            _TCPServerSocketBufferManager.InitBuffer();

            SocketAsyncEventArgs _SocketAsyncEventArgs;

            /* 预分配SocketAsyncEventArgs对象池 */
            for (int count = 0; count < _TCPServerSocketSetting.DefaultMaxConnctions; count++)
            {
                _SocketAsyncEventArgs = new SocketAsyncEventArgs();
                _SocketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(TCPServerOnIOCompleted);
                _TCPServerSocketBufferManager.SetBuffer(_SocketAsyncEventArgs);

                _TCPServerSocketAsyncEventArgsPool.Push(_SocketAsyncEventArgs);
            }
        }

        /// <summary>
        /// 当SocketAsyncEventArgs对象完成接收和发送时，调用此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="asyncEventArgs"></param>
        private void TCPServerOnIOCompleted(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            switch (asyncEventArgs.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    TCPServerProcessReceive(asyncEventArgs);
                    break;
                case SocketAsyncOperation.Send:
                    TCPServerProcessSend(asyncEventArgs);
                    break;
                default:
                    throw new ArgumentException(asyncEventArgs.ConnectByNameError.Message);
            }
        }

        /// <summary>
        /// 启动服务器，开始侦听客户端连接
        /// </summary>
        /// <param name="ipAddress">服务器IP地址</param>
        /// <param name="port">服务器端口号</param>
        /// <returns>启动成功返回True，否则返回False</returns>
        public bool TCPServerStart(IPAddress ipAddress, int port)
        {
            try
            {
                TCPServerSocketConnections = _TCPServerSocketBase.CreateSocket(ipAddress, port, ProtocolType.Tcp);
                TCPServerSocketConnections.SetSocketOption(
                    SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, _TCPServerSocketSetting.ReceiveBufferSize);
                TCPServerSocketConnections.SetSocketOption(
                    SocketOptionLevel.Socket, SocketOptionName.SendBuffer, _TCPServerSocketSetting.SendBufferSize);

                TCPServerSocketConnections.Bind(_TCPServerSocketBase.IPEndPoint);
                TCPServerSocketConnections.Listen(_TCPServerSocketSetting.DefaultMaxConnctions);

                TCPServerStartAcceptAsync(null);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 停止服务器，断开客户端所有连接
        /// </summary>
        public bool TCPServerStop()
        {
            try
            {
                TCPServerSocketConnections.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 开始等待来自客户端的连接请求
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void TCPServerStartAcceptAsync(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs == null)
            {
                acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(TCPServerOnAcceptCompleted);
            }
            else
            {
                acceptEventArgs.AcceptSocket = null;
            }

            /* 信号量用于控制对资源或资源池的访问，可以防止超出预先设置的客户端最大连接数 */
            TCPServerSemaphoreConnections.WaitOne();

            bool _AcceptPending = TCPServerSocketConnections.AcceptAsync(acceptEventArgs);

            if (!_AcceptPending)
            {
                TCPServerProcessAccept(acceptEventArgs);
            }
        }

        /// <summary>
        /// 异步的客户端连接请求完成时调用此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="acceptEventArgs"></param>
        private void TCPServerOnAcceptCompleted(object sender, SocketAsyncEventArgs acceptEventArgs)
        {
            TCPServerProcessAccept(acceptEventArgs);
        }

        /// <summary>
        /// 处理已经完成连接请求的客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void TCPServerProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs.SocketError != SocketError.Success)
            {
                return;
            }

            try
            {
                /* 从SocketAsyncEventArgs池中获取一个SocketAsyncEventArgs */
                SocketAsyncEventArgs _AcceptSocketAsyncEventArgs = _TCPServerSocketAsyncEventArgsPool.Pop();

                if (_AcceptSocketAsyncEventArgs != null)
                {
                    _AcceptSocketAsyncEventArgs.UserToken = new SocketUserToKen(acceptEventArgs.AcceptSocket);

                    /* 在连接区增加客户端的终结点信息 */

                    bool _ReceivePending = acceptEventArgs.AcceptSocket.ReceiveAsync(_AcceptSocketAsyncEventArgs);

                    if (!_ReceivePending)
                    {
                        TCPServerProcessReceive(_AcceptSocketAsyncEventArgs);
                    }
                }
                else
                {
                    /* 拒绝客户端连接，因为已达到服务器允许的最大客户端连接数 */
                    acceptEventArgs.AcceptSocket.Close();
                }
            }
            catch
            {
                return;
            }

            TCPServerStartAcceptAsync(acceptEventArgs);   /* 开始等待来自下一个客户端的连接请求 */
        }

        /// <summary>
        /// 处理接收数据，数据来自客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="receiveEventArgs"></param>
        private void TCPServerProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            if (receiveEventArgs.SocketError != SocketError.Success)
            {
                TCPServerCloseClientSocket(receiveEventArgs);
            }

            SocketUserToKen _SocketUserToKen = receiveEventArgs.UserToken as SocketUserToKen;

            /* 需要处理的字节数 */
            int remainingBytesToProcess = receiveEventArgs.BytesTransferred;

            if (remainingBytesToProcess > 0)
            {
                bool _ReceivePending = _SocketUserToKen.Socket.ReceiveAsync(receiveEventArgs);

                if (!_ReceivePending)
                {
                    TCPServerProcessReceive(receiveEventArgs);   /* 递归调用，直到receiveSocket上没有数据为止（全部接收完毕） */
                }
            }
            else
            {
                TCPServerCloseClientSocket(receiveEventArgs);   /* 客户端已经关闭连接 */
            }
        }

        /// <summary>
        /// 处理发送数据，数据发送给客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="sendEventArgs"></param>
        private void TCPServerProcessSend(SocketAsyncEventArgs sendEventArgs)
        {
            if (sendEventArgs.SocketError != SocketError.Success)
            {
                TCPServerCloseClientSocket(sendEventArgs);
            }

            SocketUserToKen _SocketUserToKen = sendEventArgs.UserToken as SocketUserToKen;

            bool _SendPending = _SocketUserToKen.Socket.SendAsync(sendEventArgs);

            if (!_SendPending)
            {
                TCPServerProcessSend(sendEventArgs);   /* 递归调用，直到sendSocket上没有数据为止（全部发送完毕） */
            }
        }

        /// <summary>
        /// 关闭客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void TCPServerCloseClientSocket(SocketAsyncEventArgs acceptEventArgs)
        {
            SocketUserToKen _SocketUserToKen = acceptEventArgs.UserToken as SocketUserToKen;

            _SocketUserToKen.Socket.Close();
            _TCPServerSocketAsyncEventArgsPool.Push(acceptEventArgs);
            TCPServerSemaphoreConnections.Release();
        }

        #region 打开/关闭网络
        public void TCPServerOpenSocket()
        {
            if(TCPServerStartFlag)
            {
                TCPServerCloseSocket();

                return;
            }

            IPAddress _IPAddress;

            if (TCPServerModel.IPAddrSelectedIndex >= 0)
            {
                /* 索引选择的IP地址 */
                _IPAddress = TCPServerModel.IPAddrItemsSource[TCPServerModel.IPAddrSelectedIndex];
            }
            else
            {
                SocketBase _SocketBase = new SocketBase();

                /* 手动输入的IP地址 */
                if (_SocketBase.TryParseIPAddressPort(TCPServerModel.IPAddrText, TCPServerModel.Port))
                {
                    _IPAddress = IPAddress.Parse(TCPServerModel.IPAddrText);

                    if (!TCPServerModel.IPAddrItemsSource.Contains(_IPAddress))
                    {
                        TCPServerModel.IPAddrItemsSource.Add(_IPAddress);
                        TCPServerModel.IPAddrInfoItemsSource.Add(_IPAddress.ToString());
                    }
                }
                else
                {
                    DepictInfo = string.Format(cultureInfo, "请输入合法的IP地址和端口号");

                    return;
                }
            }

            TCPServerInit();

            TCPServerStartFlag = TCPServerStart(_IPAddress, TCPServerModel.Port);

            if (TCPServerStartFlag)
            {
                TCPServerModel.IPAddrEnable = false;
                TCPServerModel.PortEnable = false;

                TCPServerModel.Brush = Brushes.GreenYellow;
                TCPServerModel.OpenClose = string.Format(cultureInfo, "TCP 断开");
            }
            else
            {
                DepictInfo = string.Format(cultureInfo, "服务器启动失败");

                return;
            }
        }

        public void TCPServerCloseSocket()
        {
            if(TCPServerStop())
            {
                TCPServerModel.IPAddrEnable = true;
                TCPServerModel.PortEnable = true;

                TCPServerModel.Brush = Brushes.Red;
                TCPServerModel.OpenClose = string.Format(cultureInfo, "TCP 侦听");

                TCPServerStartFlag = false;
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

                    TCPServerStartAutoSendTimer(SendModel.TCPServerAutoSendNum);
                }
                else
                {
                    TCPServerStopAutoSendTimer();
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

        #region 自动发送定时器实现
        private readonly DispatcherTimer TCPServerAutoSendDispatcherTimer = new DispatcherTimer();

        private void TCPServerInitAutoSendTimer()
        {
            TCPServerAutoSendDispatcherTimer.IsEnabled = false;
            TCPServerAutoSendDispatcherTimer.Tick += TCPServerAutoSendDispatcherTimer_Tick;
        }

        private void TCPServerAutoSendDispatcherTimer_Tick(object sender, EventArgs e)
        {

        }

        private void TCPServerStartAutoSendTimer(int interval)
        {
            TCPServerAutoSendDispatcherTimer.IsEnabled = true;
            TCPServerAutoSendDispatcherTimer.Interval = TimeSpan.FromMilliseconds(interval);
            TCPServerAutoSendDispatcherTimer.Start();
        }

        private void TCPServerStopAutoSendTimer()
        {
            TCPServerAutoSendDispatcherTimer.IsEnabled = false;
            TCPServerAutoSendDispatcherTimer.Stop();
        }
        #endregion

        #region 路径选择
        public void TCPServerSaveRecvPath()
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
