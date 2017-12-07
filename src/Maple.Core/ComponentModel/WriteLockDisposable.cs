using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Maple.Core.ComponentModel
{
    /// <summary>
    /// 为实现对资源的锁定访问提供了一种方便的方法
    /// </summary>
    public class WriteLockDisposable : IDisposable
    {
        private readonly ReaderWriterLockSlim _rwLock;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rwLock"></param>
        public WriteLockDisposable(ReaderWriterLockSlim rwLock)
        {
            _rwLock = rwLock;
            _rwLock.EnterWriteLock();
        }

        void IDisposable.Dispose()
        {
            _rwLock.ExitWriteLock();
        }
    }
}
