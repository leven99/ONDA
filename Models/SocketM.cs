using SocketDA.ModelsSocket;
using SocketDA.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketDA.Models
{
    internal sealed class OSUserToken : IDisposable
    {
        private Socket _OwnerSocket = null;

        private StringBuilder _ReadStringBuilder = null;   /* 从readScoket中累积的数据 */

        private Int32 _ReadTotalByteCount = 0;   /* 在_ReadStringBuilder中累积的总字节数 */

        public OSUserToken(Socket readSocket, Int32 bufferSize)
        {
            _OwnerSocket = readSocket;
            _ReadStringBuilder = new StringBuilder(bufferSize);
        }

        public Socket OwnerSocket
        {
            get
            {
                return _OwnerSocket;
            }
        }

        /// <summary>
        /// 对接收到的数据进行处理
        /// </summary>
        /// <param name="args"></param>
        public void ProcessData(SocketAsyncEventArgs args)
        {
            /* 获取从客户端收到的最后一条消息，该消息已存储在_ReadStringBuilder中 */
            String _Received = _ReadStringBuilder.ToString();

            /* 处理从客户端收到的消息 */

            /* 清除_ReadStringBuilder，以便它可以从客户端接收更多数据 */
            _ReadStringBuilder.Length = 0;
            _ReadTotalByteCount = 0;
        }

        public bool ReadSocketData(SocketAsyncEventArgs readSocket)
        {
            int _ByteCount = readSocket.BytesTransferred;

            if ((_ReadTotalByteCount + _ByteCount) > _ReadStringBuilder.Capacity)
            {
                return false;
            }
            else
            {
                _ReadStringBuilder.Append(Encoding.ASCII.GetString(readSocket.Buffer, readSocket.Offset, _ByteCount));
                _ReadTotalByteCount += _ByteCount;

                return true;
            }
        }

        public bool SendSocketData(SocketAsyncEventArgs sendSocket)
        {
            return true;
        }

        public void Dispose()
        {
            try
            {
                _OwnerSocket.Shutdown(SocketShutdown.Both);
            }
            catch
            {

            }
            finally
            {
                _OwnerSocket.Close();
            }
        }
    }

    internal class SocketTCPServer
    {
        protected SocketBase SocketBase = new SocketBase();

        /// <summary>
        /// 服务器最大连接客户端数量
        /// </summary>
        protected const int DEFAULT_MAX_CONNECTIONS = 10;

        /// <summary>
        /// 使用互斥锁来阻止TCP服务器侦听器线程，以便在服务器上激活有限的客户端连接。如果停止服务器，则互斥体将被释放
        /// </summary>
        private static Mutex MutexConnections = null;

        /// <summary>
        /// 跟踪TCP服务器中客户端连接数的信号量
        /// </summary>
        protected int NumConnections = 0;

        /// <summary>
        /// 服务器Socket堆栈
        /// </summary>
        protected SocketAsyncEventArgsPool SocketPool = null;

        protected IPEndPoint IPEndPoint = null;
        protected Socket Socket = null;

        public SocketTCPServer()
        {
            /* 设置互斥量和信号量 */
            MutexConnections = new Mutex();
            NumConnections = 0;

            /* 创建Socket堆栈 */
            SocketPool = new SocketAsyncEventArgsPool(DEFAULT_MAX_CONNECTIONS);

            /* 创建read sockets，用于服务器允许的最大客户端连接量，同时将
             * IO Completed的事件处理程序分配给每个socket，然后
             * 将其压入堆栈以等待客户端连接 */
            for (int count = 0; count < DEFAULT_MAX_CONNECTIONS; count++)
            {
                SocketAsyncEventArgs _SocketAsyncEventArgs = new SocketAsyncEventArgs();
                _SocketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                _SocketAsyncEventArgs.SetBuffer(new byte[2048], 0, 2048);
                SocketPool.Push(_SocketAsyncEventArgs);
            }
        }

        /// <summary>
        /// 当SocketAsyncEventArgs对象完成接收或发送时，调用此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException(e.ConnectByNameError.Message);
            }
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <returns></returns>
        public bool Start(IPAddress ipAddress, int port)
        {
            Socket = SocketBase.CreateSocket(ipAddress, port, ProtocolType.Tcp);

            if (Socket != null)
            {
                IPEndPoint = SocketBase.CreateIPEndPoint(ipAddress, port);

                try
                {
                    Socket.Bind(IPEndPoint);
                    Socket.Listen(DEFAULT_MAX_CONNECTIONS);
                    StartAcceptAsync(null);
                    MutexConnections.WaitOne();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public void Stop()
        {
            Socket.Close();
            MutexConnections.ReleaseMutex();
        }

        /// <summary>
        /// 开始等待来自客户端的连接请求
        /// </summary>
        /// <param name="acceptEventArg"></param>
        private void StartAcceptAsync(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else
            {
                /* 必须清除套接字，因为正在重用上下文对象 */
                acceptEventArg.AcceptSocket = null;
            }

            bool _AcceptPending = Socket.AcceptAsync(acceptEventArg);

            if (!_AcceptPending)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// 异步的客户端连接请求完成时调用此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="asyncEventArgs"></param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            ProcessAccept(asyncEventArgs);
        }

        /// <summary>
        /// 处理（接收/发送）已经完成连接请求的客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="asyncEventArgs"></param>
        private void ProcessAccept(SocketAsyncEventArgs asyncEventArgs)
        {
            Socket _acceptSocket = asyncEventArgs.AcceptSocket;

            if (_acceptSocket.Connected)
            {
                try
                {
                    SocketAsyncEventArgs _readSocket = SocketPool.Pop();

                    if (_readSocket != null)
                    {
                        _readSocket.UserToken = new OSUserToken(_acceptSocket, 2048);

                        Interlocked.Increment(ref NumConnections);

                        bool _ioPending = _acceptSocket.ReceiveAsync(_readSocket);

                        if (!_ioPending)
                        {
                            ProcessReceive(_readSocket);
                        }
                    }
                }
                catch
                {

                }

                StartAcceptAsync(asyncEventArgs);   /* 开始等待来自下一个客户端的连接请求 */
            }
        }

        /// <summary>
        /// 处理接收数据，数据来自客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="readSocket"></param>
        private void ProcessReceive(SocketAsyncEventArgs readSocket)
        {
            if ( (readSocket.BytesTransferred > 0) && (readSocket.SocketError == SocketError.Success) )
            {
                OSUserToken token = readSocket.UserToken as OSUserToken;

                if (token.ReadSocketData(readSocket))
                {
                    Socket _readSocket = token.OwnerSocket;

                    if (_readSocket.Available == 0)
                    {
                        token.ProcessData(readSocket);
                    }

                    bool _ioPending = _readSocket.ReceiveAsync(readSocket);

                    if (!_ioPending)
                    {
                        ProcessReceive(readSocket);
                    }
                }
            }
            else
            {
                CloseClientSocket(readSocket);
            }
        }

        /// <summary>
        /// 处理发送数据，数据发送给客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="sendSocket"></param>
        private void ProcessSend(SocketAsyncEventArgs sendSocket)
        {
            if (sendSocket.SocketError == SocketError.Success)
            {
                OSUserToken token = sendSocket.UserToken as OSUserToken;

                if (token.SendSocketData(sendSocket))
                {
                    Socket _sendSocket = token.OwnerSocket;

                    if (_sendSocket.Available == 0)
                    {
                        token.ProcessData(sendSocket);
                    }

                    bool _ioPending = _sendSocket.ReceiveAsync(sendSocket);

                    if (!_ioPending)
                    {
                        ProcessSend(sendSocket);
                    }
                }
            }
            else
            {

            }
        }

        /// <summary>
        /// 关闭客户端SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="readSocket"></param>
        private void CloseClientSocket(SocketAsyncEventArgs readSocket)
        {
            OSUserToken token = readSocket.UserToken as OSUserToken;

            token.Dispose();
            Interlocked.Decrement(ref NumConnections);
            SocketPool.Push(readSocket);
        }
    }

    internal class SocketTCPClient
    {
        protected SocketBase SocketBase = new SocketBase();

        protected IPEndPoint IPEndPoint = null;
        protected Socket Socket = null;

        /// <summary>
        /// 向服务器发送数据
        /// </summary>
        /// <param name="byData"></param>
        /// <returns></returns>
        public bool Send(byte[] byData)
        {
            try
            {
                if(Socket.Connected)
                {
                    Socket.Send(byData);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 接收来自服务器的数据
        /// </summary>
        /// <param name="byData"></param>
        public void Recv(byte[] byData)
        {
            try
            {
                if(Socket.Connected)
                {
                    Socket.Receive(byData);
                }
            }
            catch
            {

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
            Socket = SocketBase.CreateSocket(ipAddress, port, ProtocolType.Tcp);

            if (Socket != null)
            {
                IPEndPoint = SocketBase.CreateIPEndPoint(ipAddress, port);

                try
                {
                    Socket.Connect(IPEndPoint);

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        public void DisConnect()
        {
            try
            {
                Socket.Close();
            }
            catch
            {

            }
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
