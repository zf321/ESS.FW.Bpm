
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public class DefaultJobExecutor : ThreadPoolJobExecutor
    {
        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;

        // getters and setters //////////////////////////////////////////////////////
        /// <summary>
        /// 阻塞任务队列
        /// </summary>
        public virtual int QueueSize { get; set; } = 3;

        /// <summary>
        /// 核心线程池大小（大于此值且小于MaxPoolSize会丢到QueueSize）
        /// </summary>
        public virtual int CorePoolSize { get; set; } = 3;

        /// <summary>
        /// 最大线程数
        /// </summary>
        public virtual int MaxPoolSize { get; set; } = 10;

        protected internal override void StartExecutingJobs()
        {
            base.StartExecutingJobs();
        }

        protected internal override void StopExecutingJobs()
        {
            base.StopExecutingJobs();
            
            // Ask the thread pool to finish and exit
            //threadPoolExecutor.shutdown();
            //// Waits for 1 minute to finish all currently executing jobs
            //try
            //{
            //    if (!threadPoolExecutor.awaitTermination(60L, TimeUnit.SECONDS))
            //    {
            //        LOG.timeoutDuringShutdown();
            //    }
            //}
            //catch (InterruptedException e)
            //{
            //    LOG.interruptedWhileShuttingDownjobExecutor(e);
            //}
        }
    }
}