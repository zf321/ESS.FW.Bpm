using System;
using System.Threading;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class AcquireJobsRunnable
    {
        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;

        protected internal readonly JobExecutor JobExecutor;
        protected internal readonly object Monitor = new object();

        protected internal volatile bool IsInterrupted = false;

        protected internal volatile bool IsJobAdded = false;

        //protected internal readonly AtomicBoolean isWaiting = new AtomicBoolean(false);
        protected internal bool isWaiting = false;

        public AcquireJobsRunnable(JobExecutor jobExecutor)
        {
            this.JobExecutor = jobExecutor;
            
        }

        public virtual bool JobAdded => IsJobAdded;

        protected virtual void SuspendAcquisition(long millis)
        {
            if (millis <= 0)
                return;

            try
            {
                Log.DebugJobAcquisitionThreadSleeping(millis);
                lock (Monitor)
                {
                    if (!IsInterrupted)
                    {
                        isWaiting = true;
                        System.Threading.Monitor.Wait(Monitor, TimeSpan.FromMilliseconds(millis));
                    }
                }
                Log.JobExecutorThreadWokeUp();
            }
            catch (ThreadInterruptedException ex)
            {
                Log.JobExecutionWaitInterrupted();
            }
            finally
            {
                isWaiting = false;
            }
        }

        public virtual void Stop()
        {
            lock (Monitor)
            {
                IsInterrupted = true;
                if (isWaiting)
                {
                    isWaiting = false;
                    System.Threading.Monitor.PulseAll(Monitor);                    
                }
                // if (isWaiting.compareAndSet(true, false))
                // {
                //Monitor.PulseAll(MONITOR);
                // }
            }
        }

        public virtual void JobWasAdded()
        {
            IsJobAdded = true;
            if (isWaiting)
            {
                isWaiting = false;
                lock (Monitor)
                {
                    System.Threading.Monitor.PulseAll(Monitor);
                }
            }
            //if (isWaiting.compareAndSet(true, false))
            //{
            //  // ensures we only notify once
            //  // I am OK with the race condition
            //  lock (MONITOR)
            //  {
            //	Monitor.PulseAll(MONITOR);
            //  }
            //}
        }

        protected internal virtual void ClearJobAddedNotification()
        {
            IsJobAdded = false;
        }

        public abstract void Run();
    }
}