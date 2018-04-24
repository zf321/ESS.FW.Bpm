using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     <para>
    ///         Interface to the component responsible for performing
    ///         background work (<seealso cref="IJob" />).
    ///     </para>
    ///     <para>
    ///         The <seealso cref="JobExecutor" /> is capable of dispatching to multiple process engines,
    ///         ie. multiple process engines can share a single Thread Pool for performing Background
    ///         Work.
    ///     </para>
    ///     <para>
    ///         In clustered situations, you can have multiple Job Executors running against the
    ///         same queue + pending job list.
    ///     </para>
    ///     
    /// </summary>
    public abstract class JobExecutor
    {
        private static readonly JobExecutorLogger _log = ProcessEngineLogger.JobExecutorLogger;


        public virtual void Start()
        {
            if (IsActive)
                return;
            _log.StartingUpJobExecutor(GetType().FullName);
            EnsureInitialization();
            StartExecutingJobs();
            IsActive = true;
        }

        public virtual void Shutdown()
        {
            lock (this)
            {
                if (!IsActive)
                    return;
                _log.ShuttingDownTheJobExecutor(GetType().FullName);
                AcquireJobsRunnable.Stop();
                StopExecutingJobs();
                EnsureCleanup();
                IsActive = false;
            }
        }

        protected internal virtual void EnsureInitialization()
        {
            AcquireJobsCmdFactory = new DefaultAcquireJobsCommandFactory(this);
            AcquireJobsRunnable = new SequentialJobAcquisitionRunnable(this);
        }

        protected internal virtual void EnsureCleanup()
        {
            AcquireJobsCmdFactory = null;
            AcquireJobsRunnable = null;
        }

        public virtual void JobWasAdded()
        {
            if (IsActive)
                AcquireJobsRunnable.JobWasAdded();
        }

        public virtual void RegisterProcessEngine(ProcessEngineImpl processEngine)
        {
            lock (this)
            {
                ProcessEngines.Add(processEngine);

                // when we register the first process engine, start the jobexecutor
                if ((ProcessEngines.Count == 1) && AutoActivate)
                    Start();
            }
        }

        public virtual void UnregisterProcessEngine(ProcessEngineImpl processEngine)
        {
            lock (this)
            {
                ProcessEngines.Remove(processEngine);

                // if we unregister the last process engine, auto-shutdown the jobexecutor
                if (ProcessEngines.Count == 0 && IsActive)
                    Shutdown();
            }
        }

        protected internal abstract void StartExecutingJobs();

        protected internal abstract void StopExecutingJobs();

        public abstract void ExecuteJobs(IList<string> jobIds, ProcessEngineImpl processEngine);


        [Obsolete("use ExecuteJobs(IList<string> jobIds, ProcessEngineImpl processEngine) instead")]
        public virtual void ExecuteJobs(IList<string> jobIds)
        {
            if (ProcessEngines.Count > 0)
                ExecuteJobs(jobIds, ProcessEngines.ElementAt(0));
        }

        public virtual void LogAcquisitionAttempt(ProcessEngineImpl engine)
        {
            var conf = engine?.ProcessEngineConfiguration as ProcessEngineConfigurationImpl;

            if (conf != null && conf.MetricsEnabled)
                conf.MetricsRegistry.MarkOccurrence(Management.Metrics.JobAcquisitionAttempt);
        }

        public virtual void LogAcquiredJobs(ProcessEngineImpl engine, int numJobs)
        {
            var conf = engine?.ProcessEngineConfiguration as ProcessEngineConfigurationImpl;
            if (conf != null && conf.MetricsEnabled)
                conf.MetricsRegistry.MarkOccurrence(Management.Metrics.JobAcquiredSuccess, numJobs);
        }

        public virtual void LogAcquisitionFailureJobs(ProcessEngineImpl engine, int numJobs)
        {
            var conf = engine?.ProcessEngineConfiguration as ProcessEngineConfigurationImpl;
            if (conf != null && conf.MetricsEnabled)
                conf.MetricsRegistry.MarkOccurrence(Management.Metrics.JobAcquiredFailure, numJobs);
        }

        public virtual void LogRejectedExecution(ProcessEngineImpl engine, int numJobs)
        {
            var conf = engine?.ProcessEngineConfiguration as ProcessEngineConfigurationImpl;
            if (conf != null && conf.MetricsEnabled)
                conf.MetricsRegistry.MarkOccurrence(Management.Metrics.JobExecutionRejected, numJobs);
        }

        /// <summary>
        ///     Must return an iterator of registered process engines
        ///     that is independent of concurrent modifications
        ///     to the underlying data structure of engines.
        /// </summary>
        public virtual IEnumerator<ProcessEngineImpl> EngineIterator()
        {
            // Todo: Concurrent并发安全
            // a CopyOnWriteArrayList's iterator is safe in the presence
            // of modifications
            return ProcessEngines.GetEnumerator();
        }

        public virtual bool HasRegisteredEngine(ProcessEngineImpl engine)
        {
            return ProcessEngines.Contains(engine);
        }

        // Todo: java.util.concuurrent.CopyOnWriteArrayList<E>
        protected internal virtual IList<ProcessEngineImpl> ProcessEngines { get; set; } = new List<ProcessEngineImpl>();

        public virtual int WaitTimeInMillis { get; set; } = 5 * 1000;

        /// <summary>
        ///  backoff when job acquisition fails to lock all jobs
        /// </summary>
        public virtual int BackoffTimeInMillis { get; set; } = 5 * 1000;

        public virtual int LockTimeInMillis { get; set; } = 5 * 60 * 1000;

        public virtual string LockOwner { get; set; } = Guid.NewGuid().ToString();

        public virtual bool AutoActivate { get; set; } = false;

        public virtual int MaxJobsPerAcquisition { get; set; } = 3;

        public virtual float WaitIncreaseFactor { get; set; } = 2;

        public virtual long MaxWait { get; set; } = 60 * 1000;

        public virtual long MaxBackoff { get; set; } = 0;

        /// <summary>
        /// The number of job acquisition cycles without locking failures
        /// until the backoff level is reduced.
        /// </summary>
        public virtual int BackoffDecreaseThreshold { get; set; } = 100;

        public virtual string Name => $"JobExecutor[{GetType().FullName}]";

        public virtual ICommand<AcquiredJobs> GetAcquireJobsCmd(int numJobs)
        {
            return AcquireJobsCmdFactory.GetCommand(numJobs);
        }

        public virtual IAcquireJobsCommandFactory AcquireJobsCmdFactory { get; set; }

        public virtual bool IsActive { get; private set; }

        public virtual IRejectedJobsHandler RejectedJobsHandler { get; set; }

        protected internal Thread JobAcquisitionThread { get; set; }

        protected internal virtual void StartJobAcquisitionThread()
        {
            if (JobAcquisitionThread == null)
            {
                JobAcquisitionThread = new Thread(AcquireJobsRunnable.Run);
            }
            JobAcquisitionThread.Start();
        }

        protected internal virtual void StopJobAcquisitionThread()
        {
            try
            {
                JobAcquisitionThread.Join();
            }
            catch (ThreadInterruptedException e)
            {
                _log.InterruptedWhileShuttingDownjobExecutor(e);
            }
            JobAcquisitionThread = null;
        }

        public virtual AcquireJobsRunnable AcquireJobsRunnable { get; protected internal set; }

        public virtual ExecuteJobsRunnable GetExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
        {
            return new ExecuteJobsRunnable(jobIds, processEngine);
        }

    }
}