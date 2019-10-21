using SocketDA.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketDA.Models
{
    /// <summary>
    /// 这是一个非常标准的堆栈实现，设置该堆栈就是设置可重复使用的异步套接字连接。
    /// 该类只有两个操作：压入堆栈和弹出堆栈
    /// </summary>
    sealed class OSAsyncEventStack
    {
        private readonly Stack<SocketAsyncEventArgs> _SocketStack;

        /// <summary>
        /// Stack中最大可存储的items
        /// </summary>
        /// <param name="maxCapacity"></param>
        public OSAsyncEventStack(int maxCapacity)
        {
            _SocketStack = new Stack<SocketAsyncEventArgs>(maxCapacity);
        }

        /// <summary>
        /// 从Stack顶部弹出一个item
        /// </summary>
        /// <returns></returns>
        public SocketAsyncEventArgs Pop()
        {
            lock (_SocketStack)
            {
                if (_SocketStack.Count > 0)
                {
                    return _SocketStack.Pop();
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

            lock (_SocketStack)
            {
                _SocketStack.Push(item);
            }
        }
    }

    sealed class OSUserToken : IDisposable
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

    /// <summary>
    /// 这是一个TCP/UDP, Server/Client都是用的基类
    /// </summary>
    internal class OSCore
    {
        public const int DEFAULT_BUFFER_SIZE = 2048;   /* 缓冲字节 */

        public IPEndPoint ConnectionEndpoint = null;   /* 端点信息 */

        public Socket TCPConnectionSocket = null;    /* TCP连接Socket */
        public Socket UDPConnectionSocket = null;    /* UDP连接Socket */

        public static IPEndPoint CreateIPEndPoint(IPAddress ipAddress, int port)
        {
            if(ipAddress == null)
            {
                return null;
            }

            try
            {
                return new IPEndPoint(ipAddress, port);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public bool CreateSocket(IPAddress ipAddress, int port, ProtocolType protocolType)
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
            catch (SocketException)
            {
                return false;
            }

            return true;
        }
    }

    internal class OSTCPServer
    {
        protected OSCore OSCore = new OSCore();

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
                item.SetBuffer(new byte[OSCore.DEFAULT_BUFFER_SIZE], 0, OSCore.DEFAULT_BUFFER_SIZE);
                SocketPool.Push(item);
            }
        }

        /// <summary>
        /// 每当套接字上的接收或发送完成时，就会调用此方法
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
        /// 如果服务器未启动，则调用此方法一次启动服务器
        /// </summary>
        /// <returns></returns>
        public bool Start(IPAddress ipAddress, int port)
        {
            if(OSCore.CreateSocket(ipAddress, port, ProtocolType.Tcp))
            {
                try
                {
                    OSCore.TCPConnectionSocket.Bind(OSCore.ConnectionEndpoint);
                    OSCore.TCPConnectionSocket.Listen(DEFAULT_MAX_CONNECTIONS);
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
        /// 如果服务器已启动，则调用词方法一次停止服务器
        /// </summary>
        public void Stop()
        {
            OSCore.TCPConnectionSocket.Close();
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

            bool _AcceptPending = OSCore.TCPConnectionSocket.AcceptAsync(acceptEventArg);

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
                        _readSocket.UserToken = new OSUserToken(_acceptSocket, OSCore.DEFAULT_BUFFER_SIZE);

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

        /// <summary>
        /// 一旦有事务，此方法将处理readSocket
        /// </summary>
        /// <param name="readSocket"></param>
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

        /// <summary>
        /// 一旦有事务，此方法将处理sendSocket
        /// </summary>
        /// <param name="sendSocket"></param>
        private void ProcessSend(SocketAsyncEventArgs sendSocket)
        {
            if (sendSocket.BytesTransferred > 0)
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
            else
            {

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

            Interlocked.Decrement(ref NumConnections);

            SocketPool.Push(readSocket);
        }
    }

    internal class OSTCPClient
    {
        protected OSCore OSCore = new OSCore();

        public bool Send(byte[] byData)
        {
            try
            {
                if(OSCore.TCPConnectionSocket.Connected)
                {
                    OSCore.TCPConnectionSocket.Send(byData);

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

        public bool Connect(IPAddress ipAddress, int port)
        {
            if(OSCore.CreateSocket(ipAddress, port, ProtocolType.Tcp))
            {
                try
                {
                    var _ConnectEndpoint = OSCore.CreateIPEndPoint(ipAddress, port);
                    OSCore.TCPConnectionSocket.Connect(_ConnectEndpoint);

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

        public void DisConnect()
        {
            try
            {
                OSCore.TCPConnectionSocket.Close();
            }
            catch
            {

            }
        }
    }

    internal class OSUDPServer
    {
        protected OSCore OSCore = new OSCore();
    }

    internal class OSUDPClient
    {
        protected OSCore OSCore = new OSCore();
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
