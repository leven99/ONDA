using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace SocketDA.ModelsSocket
{
    internal sealed class SocketAsyncEventArgsPool
    {
        private readonly ConcurrentQueue<SocketAsyncEventArgs> _SocketConcurrentQueue;

        /// <summary>
        /// 初始化 SocketAsyncEventArgs Pool 的大小
        /// </summary>
        internal SocketAsyncEventArgsPool()
        {
            _SocketConcurrentQueue = new ConcurrentQueue<SocketAsyncEventArgs>();
        }

        /// <summary>
        /// 将一个SocketAsyncEventArgs实例入队到SocketAsyncEventArgsPool中
        /// </summary>
        /// <param name="item"></param>
        internal void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(item.ConnectByNameError.Message);
            }

            _SocketConcurrentQueue.Enqueue(item);
        }

        /// <summary>
        /// 从一个SocketAsyncEventArgsPool中出队SocketAsyncEventArgs实例
        /// </summary>
        /// <returns></returns>
        internal SocketAsyncEventArgs Pop()
        {
            if (_SocketConcurrentQueue.Count > 0)
            {
                if (_SocketConcurrentQueue.TryDequeue(out SocketAsyncEventArgs args) )
                {
                    return args;
                }
            }

            return null;
        }
    }
}
