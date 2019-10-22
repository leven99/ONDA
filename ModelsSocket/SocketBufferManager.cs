using System.Collections.Generic;
using System.Net.Sockets;

namespace SocketDA.ModelsSocket
{
    /// <summary>
    /// 创建一个缓冲区，该缓冲区可以被分割分配给SocketAsyncEventArgs对象，以便与每个Socket I/O操作使用
    /// 
    /// 目的：缓冲区易于重复使用，并防止碎片化堆内存。
    /// 
    /// 提示：该类上的公开操作不是线程安全的
    /// </summary>
    internal sealed class SocketBufferManager
    {
        /// <summary>
        /// 缓冲区管理器控制的字节总数
        /// </summary>
        private readonly int _ManagerNumBytes;

        /// <summary>
        /// 缓冲区管理器当前的索引
        /// </summary>
        private int _ManagerCurrentIndex;

        /// <summary>
        /// 缓冲区管理器维护的分配给每一个SocketAsyncEventArgs对象的字节数组
        /// </summary>
        private byte[] _ManagerBuffer;

        /// <summary>
        /// 缓冲管理器维护的分配给每一个SocketAsyncEventArgs对象的字节数组的大小
        /// </summary>
        private readonly int _ManagerBufferSize;

        private readonly Stack<int> _ManagerFreeIndexPool;

        public SocketBufferManager(int totalBytes, int bufferSize)
        {
            _ManagerNumBytes = totalBytes;
            _ManagerCurrentIndex = 0;
            _ManagerBufferSize = bufferSize;
            _ManagerFreeIndexPool = new Stack<int>();
        }

        /// <summary>
        /// 初始化缓冲区管理器所使用的总的缓冲区空间
        /// </summary>
        internal void InitBuffer()
        {
            _ManagerBuffer = new byte[_ManagerNumBytes];
        }

        /// <summary>
        /// 将缓冲区管理器控制的所有缓冲区分割到每个 SocketAsyncEventArgs 对象
        /// </summary>
        /// <param name="args"></param>
        /// <returns>设置成功返回True，否则返回False</returns>
        internal bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (_ManagerFreeIndexPool.Count > 0)
            {
                args.SetBuffer(_ManagerBuffer, _ManagerFreeIndexPool.Pop(), _ManagerBufferSize);
            }
            else
            {
                if( (_ManagerNumBytes - _ManagerBufferSize) < _ManagerCurrentIndex )
                {
                    return false;
                }

                args.SetBuffer(_ManagerBuffer, _ManagerCurrentIndex, _ManagerBufferSize);
                _ManagerCurrentIndex += _ManagerBufferSize;
            }

            return true;
        }

        /// <summary>
        /// 从SocketAsyncEventArg对象中删除缓冲区，缓冲区释放回缓冲池
        /// </summary>
        /// <param name="args"></param>
        internal void FreeBuffer(SocketAsyncEventArgs args)
        {
            _ManagerFreeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}
