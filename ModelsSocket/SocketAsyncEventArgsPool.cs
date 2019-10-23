using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SocketDA.ModelsSocket
{
    /// <summary>
    /// 这是一个非常标准的堆栈实现，设置该堆栈就是设置可重复使用的异步套接字连接
    /// </summary>
    internal sealed class SocketAsyncEventArgsPool
    {
        private readonly Stack<SocketAsyncEventArgs> _SocketStack;

        /// <summary>
        /// 初始化 SocketAsyncEventArgs Pool 的大小
        /// </summary>
        /// <param name="maxCapacity"></param>
        internal SocketAsyncEventArgsPool(int maxCapacity)
        {
            _SocketStack = new Stack<SocketAsyncEventArgs>(maxCapacity);
        }

        /// <summary>
        /// SocketAsyncEventArgs Pool 中的 SocketAsyncEventArgs 实例数
        /// </summary>
        internal int Count
        {
            get
            {
                return _SocketStack.Count;
            }
        }

        /// <summary>
        /// 从 SocketAsyncEventArgs Pool 中推出 SocketAsyncEventArgs 实例
        /// </summary>
        /// <returns></returns>
        internal SocketAsyncEventArgs Pop()
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
        /// 将 SocketAsyncEventArgs 实例推入到 SocketAsyncEventArgs Pool 中
        /// </summary>
        /// <param name="item"></param>
        internal void Push(SocketAsyncEventArgs item)
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
}
