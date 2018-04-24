using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Common
{
    /// <summary>
    /// 多线程安全的自增
    /// </summary>
    public class AtomicLong
    {
        private long atomic = 0;
        private static object atomic_LOCK = new object();
        public AtomicLong()
        {

        }
        public AtomicLong(long val)
        {
            atomic = val;
        }
        /// <summary>
        /// 慎用，可能破坏线程安全
        /// </summary>
        /// <returns></returns>
        public long GetValue()
        {
            return atomic;
        }

        public long GetAndAdd(long value)
        {
            long r = atomic;
            lock (atomic_LOCK)
            {
                atomic = atomic + value;
            }
            return r;
        }

        /// <summary>
        /// 返回自增之后的值
        /// </summary>
        /// <returns></returns>
        public long IncrementAndGet()
        {
            lock (atomic_LOCK)
            {
                atomic++;
            }
            return atomic;
        }
        /// <summary>
        /// 返回自增之前的旧值
        /// </summary>
        /// <returns></returns>
        public long GetAndIncrement()
        {
            long r = atomic;
            lock (atomic_LOCK)
            {
                atomic++;
            }
            return r;
        }
        public long GetAndSet(long value)
        {
            long r = atomic;
            lock (atomic_LOCK)
            {
                atomic = value;
            }
            return r;
        }
    }
}
