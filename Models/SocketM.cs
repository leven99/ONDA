using SocketDA.ModelsSocket;
using SocketDA.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace SocketDA.Models
{
    internal class SocketTCPServer
    {
        protected SocketBase _SocketBase = null;
        protected SocketSetting _SocketSetting = null;
        protected SocketBufferManager _SocketBufferManager = null;
        protected SocketAsyncEventArgsPool _SocketAsyncEventArgsPool = null;

        /// <summary>
        /// 客户端连接请求互斥锁
        /// </summary>
        protected Mutex MutexConnections = null;

        /// <summary>
        /// 客户端连接请求信号量
        /// </summary>
        protected Semaphore SemaphoreConnections = null;

        protected Socket SocketConnections = null;
        protected IPEndPoint IPEndPointConnections = null;

        public SocketTCPServer()
        {
            _SocketBase = new SocketBase();
            _SocketSetting = new SocketSetting();
            _SocketBufferManager = new SocketBufferManager(
                _SocketSetting.DefaultMaxConnctions * _SocketSetting.OpsToPreAlloc * _SocketSetting.BufferSize,
                _SocketSetting.OpsToPreAlloc * _SocketSetting.BufferSize);
            _SocketAsyncEventArgsPool = new SocketAsyncEventArgsPool(_SocketSetting.DefaultMaxConnctions);

            MutexConnections = new Mutex();
            SemaphoreConnections = new Semaphore(_SocketSetting.DefaultMaxConnctions, _SocketSetting.DefaultMaxConnctions);
        }

        /// <summary>
        /// 初始化服务器
        /// </summary>
        public void Init()
        {
            _SocketBufferManager.InitBuffer();

            SocketAsyncEventArgs _SocketAsyncEventArgs;
            
            /* 预分配SocketAsyncEventArgs对象池 */
            for (int count = 0; count < _SocketSetting.DefaultMaxConnctions; count++)
            {
                _SocketAsyncEventArgs = new SocketAsyncEventArgs();
                _SocketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                _SocketBufferManager.SetBuffer(_SocketAsyncEventArgs);

                /* 添加SocketAsyncEventArgs对象到SocketAsyncEventArgs对象池 */
                _SocketAsyncEventArgsPool.Push(_SocketAsyncEventArgs);
            }
        }

        /// <summary>
        /// 当SocketAsyncEventArgs对象完成接收或发送时，调用此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="asyncEventArgs"></param>
        private void OnIOCompleted(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            switch (asyncEventArgs.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(asyncEventArgs);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(asyncEventArgs);
                    break;
                default:
                    throw new ArgumentException(asyncEventArgs.ConnectByNameError.Message);
            }
        }

        /// <summary>
        /// 启动服务器，开始侦听客户端连接
        /// </summary>
        /// <returns></returns>
        public bool Start(IPAddress ipAddress, int port)
        {
            SocketConnections = _SocketBase.CreateSocket(ipAddress, port, ProtocolType.Tcp);
            IPEndPointConnections = _SocketBase._IPEndPoint;

            try
            {
                SocketConnections.Bind(IPEndPointConnections);
                SocketConnections.Listen(_SocketSetting.DefaultMaxConnctions);

                StartAcceptAsync(null);
                MutexConnections.WaitOne();

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
        public void Stop()
        {
            SocketConnections.Close();
            MutexConnections.ReleaseMutex();
        }

        /// <summary>
        /// 开始等待来自客户端的连接请求
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void StartAcceptAsync(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs == null)
            {
                acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else
            {
                acceptEventArgs.AcceptSocket = null;
            }

            /* 信号量用于控制对资源或资源池的访问，可以防止超出预先设置的客户端最大连接数 */
            SemaphoreConnections.WaitOne();

            bool _AcceptPending = SocketConnections.AcceptAsync(acceptEventArgs);

            if (!_AcceptPending)
            {
                ProcessAccept(acceptEventArgs);
            }
        }

        /// <summary>
        /// 异步的客户端连接请求完成时调用此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="acceptEventArgs"></param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs acceptEventArgs)
        {
            ProcessAccept(acceptEventArgs);
        }

        /// <summary>
        /// 处理（接收/发送）已经完成连接请求的客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            Socket _AcceptSocket = acceptEventArgs.AcceptSocket;

            if (!_AcceptSocket.Connected)
            {
                /* 如果_acceptSocket没有连接到客户端，则直接返回 */
                return;
            }

            try
            {
                /* 从SocketAsyncEventArgs池中获取一个SocketAsyncEventArgs */
                SocketAsyncEventArgs _AcceptSocketAsyncEventArgs = _SocketAsyncEventArgsPool.Pop();

                if (_AcceptSocketAsyncEventArgs != null)
                {
                    _AcceptSocketAsyncEventArgs.UserToken = new SocketUserToKen();

                    bool _ReceivePending = _AcceptSocket.ReceiveAsync(_AcceptSocketAsyncEventArgs);

                    if (!_ReceivePending)
                    {
                        ProcessReceive(_AcceptSocketAsyncEventArgs);
                    }
                }
                else
                {
                    /* 拒绝客户端连接，因为已达到服务器允许的最大客户端连接数 */
                    _AcceptSocket.Close();
                }
            }
            catch
            {
                return;
            }

            StartAcceptAsync(acceptEventArgs);   /* 开始等待来自下一个客户端的连接请求 */
        }

        /// <summary>
        /// 处理接收数据，数据来自客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="receiveEventArgs"></param>
        private void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            if(receiveEventArgs.SocketError != SocketError.Success)
            {
                CloseClientSocket(receiveEventArgs);
            }

            /* 需要处理的字节数 */
            int remainingBytesToProcess = receiveEventArgs.BytesTransferred;

            if (remainingBytesToProcess > 0)
            {
                bool _ReceivePending = receiveEventArgs.AcceptSocket.ReceiveAsync(receiveEventArgs);

                if (!_ReceivePending)
                {
                    ProcessReceive(receiveEventArgs);   /* 递归调用，直到receiveSocket上没有数据为止（全部接收完毕） */
                }
            }
            else
            {
                /* 客户端已经关闭连接 */
                CloseClientSocket(receiveEventArgs);
            }
        }

        /// <summary>
        /// 处理发送数据，数据发送给客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="sendEventArgs"></param>
        private void ProcessSend(SocketAsyncEventArgs sendEventArgs)
        {
            if (sendEventArgs.SocketError != SocketError.Success)
            {
                CloseClientSocket(sendEventArgs);
            }

            bool _SendPending = sendEventArgs.AcceptSocket.SendAsync(sendEventArgs);

            if (!_SendPending)
            {
                ProcessSend(sendEventArgs);   /* 递归调用，直到sendSocket上没有数据为止（全部发送完毕） */
            }
        }

        /// <summary>
        /// 关闭客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void CloseClientSocket(SocketAsyncEventArgs acceptEventArgs)
        {
            acceptEventArgs.AcceptSocket.Close();
            _SocketAsyncEventArgsPool.Push(acceptEventArgs);
            SemaphoreConnections.Release();
        }
    }

    internal class SocketTCPClient
    {
        protected SocketBase _SocketBase = null;
        protected SocketSetting _SocketSetting = null;

        /// <summary>
        /// 连接服务器请求信号量
        /// </summary>
        protected Semaphore SemaphoreConnections = null;

        protected Socket SocketConnections = null;
        protected IPEndPoint IPEndPointConnections = null;
        protected SocketAsyncEventArgs SocketAsyncEventArgsConnections = null;

        public SocketTCPClient()
        {
            _SocketBase = new SocketBase();
            _SocketSetting = new SocketSetting();
        }

        /// <summary>
        /// 初始化客户端
        /// </summary>
        public void Init()
        {
            SocketAsyncEventArgsConnections = new SocketAsyncEventArgs();
            SocketAsyncEventArgsConnections.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
            SocketAsyncEventArgsConnections.SetBuffer(new byte[_SocketSetting.BufferSize * _SocketSetting.OpsToPreAlloc],
                0, _SocketSetting.OpsToPreAlloc * _SocketSetting.BufferSize);
            SemaphoreConnections = new Semaphore(1, 1);   /* 一个客户端程序只连接一个服务器 */
        }

        /// <summary>
        /// 当SocketAsyncEventArgs对象完成接收或发送时，调用此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="asyncEventArgs"></param>
        private void OnIOCompleted(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            switch (asyncEventArgs.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(asyncEventArgs);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(asyncEventArgs);
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
        public bool Connect(IPAddress ipAddress, int port)
        {
            SocketConnections = _SocketBase.CreateSocket(ipAddress, port, ProtocolType.Tcp);
            IPEndPointConnections = _SocketBase._IPEndPoint;

            try
            {
                SocketAsyncEventArgsConnections.RemoteEndPoint = IPEndPointConnections;
                SocketAsyncEventArgsConnections.AcceptSocket = SocketConnections;

                StartConnectAsync(null);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void StartConnectAsync(SocketAsyncEventArgs connectEventArgs)
        {
            connectEventArgs = new SocketAsyncEventArgs();
            connectEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);

            SemaphoreConnections.WaitOne();

            bool _ConnectPending = connectEventArgs.AcceptSocket.ConnectAsync(connectEventArgs);

            if(!_ConnectPending)
            {
                ProcessConnect(connectEventArgs);
            }
        }

        private void OnConnectCompleted(object sender, SocketAsyncEventArgs connectEventArgs)
        {
            ProcessConnect(connectEventArgs);
        }

        /// <summary>
        /// 处理连接服务器
        /// </summary>
        /// <param name="connectEventArgs"></param>
        private void ProcessConnect(SocketAsyncEventArgs connectEventArgs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 处理接收数据，数据来自服务器SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="receiveEventArgs"></param>
        private void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 处理发送数据，数据发送给客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="sendEventArg"></param>
        private void ProcessSend(SocketAsyncEventArgs sendEventArg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        public void DisConnect()
        {
            SocketConnections.Close();
            SemaphoreConnections.Release();
        }
    }

    internal class SocketUDPServer
    {
        protected SocketBase SocketBase = new SocketBase();
    }

    internal class SocketUDPClient
    {
        protected SocketBase SocketBase = new SocketBase();
    }

    internal class SocketModel : MainWindowBase
    {
        /// <summary>
        /// 本机IP地址
        /// </summary>
        public Collection<IPAddress> SocketSrcIPAddrItemsSource { get; set; }

        /// <summary>
        /// 本机IP地址，包含网络配置器的名称
        /// </summary>
        public Collection<string> SocketSourceIPAddressItemsSource { get; set; }

        /// <summary>
        /// 判断IP地址和端口号的合法性
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool TryParseIPAddressPort(string ipAddress, int port)
        {
            if(IPAddress.TryParse(ipAddress, out _))
            {
                if ( (port > 0) && (port < 65535) )
                {
                    return true;
                }
            }

            return false;
        }

        public void SocketDataContext()
        {
            SocketSrcIPAddrItemsSource = new Collection<IPAddress>();
            SocketSourceIPAddressItemsSource = new Collection<string>();

            /* 本地计算机的所有网络设配器 */
            NetworkInterface[] NetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var _NetworkInterface in NetworkInterfaces)
            {
                if (_NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    /* 获取网络设配器的名称 */
                    string _NetworkInterfaceName = _NetworkInterface.Name;

                    var _NetworkInterfaceIPProperties = _NetworkInterface.GetIPProperties();

                    /* 网络设配器接口的单播地址个数 */
                    var _IPAddressCount = _NetworkInterfaceIPProperties.UnicastAddresses.Count;

                    for (int _Count = 0; _Count < _IPAddressCount; _Count++)
                    {
                        /* 获取网络设配器接口单播地址 */
                        IPAddress _IPAddress = _NetworkInterfaceIPProperties.UnicastAddresses[_Count].Address;

                        SocketSrcIPAddrItemsSource.Add(_IPAddress);
                        SocketSourceIPAddressItemsSource.Add(_IPAddress.ToString() + " / " + _NetworkInterfaceName);
                    }
                }
            }
        }
    }
}
