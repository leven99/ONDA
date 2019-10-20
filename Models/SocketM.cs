using SocketDA.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace SocketDA.Models
{
    /// <summary>
    /// 这是一个非常标准的堆栈实现，设置该堆栈就是设置可重复使用的异步套接字连接。
    /// 该类只有两个操作：压入堆栈和弹出堆栈
    /// </summary>
    sealed class OSAsyncEventStack
    {
        private readonly Stack<SocketAsyncEventArgs> SocketStack;

        /// <summary>
        /// Stack中最大可存储的items
        /// </summary>
        /// <param name="maxCapacity"></param>
        public OSAsyncEventStack(int maxCapacity)
        {
            SocketStack = new Stack<SocketAsyncEventArgs>(maxCapacity);
        }

        /// <summary>
        /// 从Stack顶部弹出一个item
        /// </summary>
        /// <returns></returns>
        public SocketAsyncEventArgs Pop()
        {
            lock (SocketStack)
            {
                if (SocketStack.Count > 0)
                {
                    return SocketStack.Pop();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 将一个item推入Stack顶部
        /// </summary>
        /// <param name="item"></param>
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(item.ConnectByNameError.Message);
            }

            lock (SocketStack)
            {
                SocketStack.Push(item);
            }
        }
    }

    sealed class OSUserToken : IDisposable
    {
        private Socket ownersocket = null;

        public OSUserToken(Socket readSocket, Int32 bufferSize)
        {

        }

        public Socket OwnerSocket
        {
            get
            {
                return ownersocket;
            }
        }

        public void ProcessData(SocketAsyncEventArgs args)
        {

        }

        public bool ReadSocketData(SocketAsyncEventArgs readSocket)
        {
            return true;
        }

        public void Dispose()
        {

        }
    }

    internal class OSCore
    {
        protected const int DEFAULT_BUFFER_SIZE = 2048;   /* 缓冲字节 */

        protected IPEndPoint ConnectionEndpoint = null;   /* 端点信息 */

        protected Socket TCPConnectionSocket = null;    /* TCP连接Socket */
        protected Socket UDPConnectionSocket = null;    /* UDP连接Socket */

        public IPEndPoint CreateIPEndPoint(IPAddress ipAddress, int port)
        {
            try
            {
                return new IPEndPoint(ipAddress, port);
            }
            catch
            {
                return null;
            }
        }

        protected bool CreateSocket(IPAddress ipAddress, int port, ProtocolType protocolType)
        {
            ConnectionEndpoint = CreateIPEndPoint(ipAddress, port);

            if(ConnectionEndpoint == null)
            {
                return false;
            }

            try
            {
                if(protocolType == ProtocolType.Tcp)
                {
                    /* 创建TCP Socket（支持IPv4，IPv6） */
                    TCPConnectionSocket = new Socket(ConnectionEndpoint.AddressFamily, SocketType.Stream, protocolType);
                }
                else if(protocolType == ProtocolType.Udp)
                {
                    /* 创建UDP Socket（支持IPv4，IPv6） */
                    UDPConnectionSocket = new Socket(ConnectionEndpoint.AddressFamily, SocketType.Dgram, protocolType);
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

            return true;
        }
    }

    internal class OSTCPServer : OSCore
    {
        protected const int DEFAULT_MAX_CONNECTIONS = 10;   /* TCP服务器最大连接客户端数量 */

        /* 使用互斥锁来阻止TCP服务器侦听器线程，以便在服务器上激活有限的客户端连接。如果停止服务器，则互斥体将被释放 */
        private static Mutex MutexConnections = null;

        /* 跟踪TCP服务器中客户端连接数的信号量 */
        protected int NumConnections = 0;

        /* TCP服务器Socket堆栈 */
        protected OSAsyncEventStack SocketPool = null;

        public OSTCPServer()
        {
            /* 设置互斥量和信号量 */
            MutexConnections = new Mutex();
            NumConnections = 0;

            /* 创建Socket堆栈 */
            SocketPool = new OSAsyncEventStack(DEFAULT_MAX_CONNECTIONS);

            /* 创建read sockets，用于服务器允许的最大客户端连接量，同时将
             * IO Completed的事件处理程序分配给每个socket，然后
             * 将其压入堆栈以等待客户端连接 */
            for (int count = 0; count < DEFAULT_MAX_CONNECTIONS; count++)
            {
                SocketAsyncEventArgs item = new SocketAsyncEventArgs();
                item.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                item.SetBuffer(new byte[DEFAULT_BUFFER_SIZE], 0, DEFAULT_BUFFER_SIZE);
                SocketPool.Push(item);
            }
        }

        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            throw new NotImplementedException();
        }

        public bool Start()
        {
            try
            {
                TCPConnectionSocket.Bind(ConnectionEndpoint);
                TCPConnectionSocket.Listen(DEFAULT_MAX_CONNECTIONS);
                StartAcceptAsync(null);
                MutexConnections.WaitOne();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Stop()
        {
            TCPConnectionSocket.Close();
            MutexConnections.ReleaseMutex();
        }

        /// <summary>
        /// 此方法实现了客户端连接事件的异步循环
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
                acceptEventArg.AcceptSocket = null;
            }

            bool _AcceptPending = TCPConnectionSocket.AcceptAsync(acceptEventArg);

            if (!_AcceptPending)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// 当accept套接字完成异步操作时触发此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="asyncEventArgs"></param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            ProcessAccept(asyncEventArgs);
        }

        /// <summary>
        /// 此方法用于处理接受套接字连接
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
                        _readSocket.UserToken = new OSUserToken(_acceptSocket, DEFAULT_BUFFER_SIZE);

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

                StartAcceptAsync(asyncEventArgs);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs readSocket)
        {
            if (readSocket.BytesTransferred > 0)
            {
                if (readSocket.SocketError == SocketError.Success)
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
                    CloseReadSocket(readSocket);
                }
            }
            else
            {
                CloseReadSocket(readSocket);
            }
        }

        private void CloseReadSocket(SocketAsyncEventArgs readSocket)
        {
            OSUserToken token = readSocket.UserToken as OSUserToken;
            CloseReadSocket(token, readSocket);
        }

        private void CloseReadSocket(OSUserToken token, SocketAsyncEventArgs readSocket)
        {
            token.Dispose();

            // Decrement the counter keeping track of the total number of clients connected to the server.
            Interlocked.Decrement(ref NumConnections);

            // Put the read socket back in the stack to be used again
            SocketPool.Push(readSocket);
        }
    }

    internal class OSTCPClient : OSCore
    {

    }

    internal class OSUDPServer : OSCore
    {

    }

    internal class OSUDPClient : OSCore
    {

    }

    internal class SocketModel : MainWindowBase
    {
        public Collection<IPAddress> SocketSrcPAddrItemsSource { get; set; }
        public Collection<string> SocketSourceIPAddressItemsSource { get; set; }

        public void SocketDataContext()
        {
            SocketSrcPAddrItemsSource = new Collection<IPAddress>();
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

                        SocketSrcPAddrItemsSource.Add(_IPAddress);
                        SocketSourceIPAddressItemsSource.Add(_IPAddress.ToString() + " / " + _NetworkInterfaceName);
                    }
                }
            }
        }
    }
}
