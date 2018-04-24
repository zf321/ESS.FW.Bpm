using System.Collections.Generic;
using System.Threading;
using Engine.Tests.ConCurrency;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    ///     IJob executor that uses a <seealso cref="ControllableThread" /> for job acquisition. That means,
    ///     the job acquisition thread returns control with each iteration of acquiring jobs (specifically
    ///     between selecting jobs and returning them to the acquisition runnable).
    /// </summary>
    public class ControllableJobExecutor : ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor
    {
        //private  ConcurrencyTestCase.ThreadControl _acquisitionThreadControl;
        //private  ConcurrencyTestCase.ThreadControl _executionThreadControl;
        private bool _syncOnShutdown = true;
        //private bool _syncAsSuspendEnabled;


        public ControllableJobExecutor()
        {
            AcquireJobsRunnable = new RecordingAcquireJobsRunnable(this);
            JobAcquisitionThread = new Thread(AcquireJobsRunnable.Run);
            AcquisitionThreadControl = new ConcurrencyTestCase.ThreadControl(JobAcquisitionThread);
            ExecutionThreadControl = new ConcurrencyTestCase.ThreadControl(JobAcquisitionThread);
            // execution thread is same as acquisition thread
            AcquireJobsCmdFactory = new ControllableJobAcquisitionCommandFactory(this);
        }

        /// <summary>
        ///     <para>
        ///         Creates the job executor and registers the given process engine
        ///         with it.
        ///     </para>
        ///     <para>
        ///         Use this constructor if the process engine is not registered
        ///         with the job executor when the process engine is bootstrapped.
        ///     </para>
        ///     <para>
        ///         Note: this is a hack since it enables to use multiple job executors with
        ///         the same engine which is not a supported feature (and for example clashes with
        ///         processEngineConfiguration#getJobExecutor)
        ///     </para>
        /// </summary>
        public ControllableJobExecutor(ProcessEngineImpl processEngine) : this()
        {
            ProcessEngines.Add(processEngine);
        }

        public ControllableJobExecutor(bool syncAsSuspendEnabled) : this()
        {
            SyncAsSuspendEnabled = syncAsSuspendEnabled;
        }

        public virtual bool SyncAsSuspendEnabled { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="syncOnShutdown">
        /// true: behave like embedded job executor where shutdown waits for all jobs to end
        /// false: behave like runtime container job executor where shutdown does not influence job execution
        /// </param>
        /// <returns></returns>
        public virtual ControllableJobExecutor ProceedAndWaitOnShutdown(bool syncOnShutdown)
        {
            _syncOnShutdown = syncOnShutdown;
            return this;
        }

        protected override void EnsureInitialization()
        {
            // already initialized in constructor
        }

        public virtual ConcurrencyTestCase.ThreadControl AcquisitionThreadControl { get; }

        public virtual ConcurrencyTestCase.ThreadControl ExecutionThreadControl { get; }

        protected override void StartExecutingJobs()
        {
            JobAcquisitionThread.Start();
        }

        protected override void StopExecutingJobs()
        {
            if (_syncOnShutdown)
                AcquisitionThreadControl.WaitUntilDone(true);
        }

        public new RecordingAcquireJobsRunnable AcquireJobsRunnable
        {
            get => (RecordingAcquireJobsRunnable)base.AcquireJobsRunnable;
            protected internal set => base.AcquireJobsRunnable = value;
        }

        public override void ExecuteJobs(IList<string> jobIds, ProcessEngineImpl processEngine)
        {
            GetExecuteJobsRunnable(jobIds, processEngine).Run(null);
        }

        public class ControllableJobAcquisitionCommandFactory : IAcquireJobsCommandFactory
        {
            private readonly ControllableJobExecutor _outerInstance;

            public ControllableJobAcquisitionCommandFactory(ControllableJobExecutor outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual ICommand<AcquiredJobs> GetCommand(int numJobsToAcquire)
            {
                return new ControllableAcquisitionCommand(_outerInstance, _outerInstance.AcquisitionThreadControl, numJobsToAcquire);
            }
        }

        public class ControllableAcquisitionCommand : ConcurrencyTestCase.ControllableCommand<AcquiredJobs>
        {
            private readonly ControllableJobExecutor _outerInstance;
            private int _numJobsToAcquire;

            public ControllableAcquisitionCommand(ControllableJobExecutor outerInstance, ConcurrencyTestCase.ThreadControl threadControl, int numJobsToAcquire) : base(threadControl)
            {
                _outerInstance = outerInstance;
                _numJobsToAcquire = numJobsToAcquire;
            }

            public override AcquiredJobs Execute(CommandContext commandContext)
            {
                Monitor.Sync(); // Wait till makeContinue() is called from test thread

                var acquiredJobs = new AcquireJobsCmd(_outerInstance, _numJobsToAcquire).Execute(commandContext);

                Monitor.Sync(); // Wait till makeContinue() is called from test thread

                return acquiredJobs;
            }
        }
    }
}