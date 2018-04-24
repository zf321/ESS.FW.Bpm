using System.Threading;

namespace ESS.FW.Bpm.Engine.Impl.Metrics
{
    /// <summary>
    ///     A Meter implementation based on AtomicLong
    /// </summary>
    public class Meter
    {
        protected internal int counter;

        protected internal string name;

        public Meter(string name)
        {
            this.name = name;
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public virtual long AndClear
        {
            get { return Interlocked.Exchange(ref counter, 0); }
        }

        public virtual void mark()
        {
            Interlocked.Increment(ref counter);
        }

        public virtual void markTimes(long times)
        {
            //counter.addAndGet(times);
            Interlocked.Increment(ref counter);
        }

        public virtual long get()
        {
            return counter;
        }
    }
}