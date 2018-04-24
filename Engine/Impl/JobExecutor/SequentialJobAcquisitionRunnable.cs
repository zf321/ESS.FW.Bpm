using System;
using System.Diagnostics;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public class SequentialJobAcquisitionRunnable : AcquireJobsRunnable
    {
        protected internal readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;
        

        public JobAcquisitionContext AcquisitionContext { get; protected internal set; }

        public SequentialJobAcquisitionRunnable(JobExecutor jobExecutor) : base(jobExecutor)
        {
            AcquisitionContext = InitializeAcquisitionContext();
        }


        public override void Run()
        {
            lock (this)
            {
                Log.StartingToAcquireJobs(JobExecutor.Name);

                var acquisitionStrategy = InitializeAcquisitionStrategy();

                while (!IsInterrupted)
                {
                    AcquisitionContext.Reset();
                    AcquisitionContext.AcquisitionTime = (DateTime.Now.Ticks/ 10000000);//DateTime.Now.Millisecond;


                    var engineIterator = JobExecutor.EngineIterator();

                    try
                    {
                        while (engineIterator.MoveNext())
                        {
                            var currentProcessEngine = engineIterator.Current;
                            if (!JobExecutor.HasRegisteredEngine(currentProcessEngine))
                                continue;

                            var acquiredJobs = AcquireJobs(AcquisitionContext, acquisitionStrategy, currentProcessEngine);
                            ExecuteJobs(AcquisitionContext, currentProcessEngine, acquiredJobs);
                           
                        }
                    }
                    catch (System.Exception e)
                    {
                        Log.ExceptionDuringJobAcquisition(e);

                        AcquisitionContext.AcquisitionException = e;
                    }

                    AcquisitionContext.JobAdded = IsJobAdded;
                    ConfigureNextAcquisitionCycle(AcquisitionContext, acquisitionStrategy);
                    //The clear had to be done after the configuration, since a hint can be
                    //appear in the suspend and the flag shouldn't be cleaned in this case.
                    //The loop will restart after suspend with the isJobAdded flag and
                    //reconfigure with this flag
                    ClearJobAddedNotification();

                    var waitTime = acquisitionStrategy.WaitTime;
                    // wait the requested wait time minus the time that acquisition itself took
                    // this makes the intervals of job acquisition more constant and therefore predictable
                    waitTime = Math.Max(0, (AcquisitionContext.AcquisitionTime + waitTime) - (DateTime.Now.Ticks/ 10000000));
                    SuspendAcquisition(waitTime);
                }

                Log.StoppedJobAcquisition(JobExecutor.Name);
            }
        }

        protected internal virtual JobAcquisitionContext InitializeAcquisitionContext()
        {
            return new JobAcquisitionContext();
        }

        /// <summary>
        ///     Reconfigure the acquisition strategy based on the current cycle's acquisition context.
        ///     A strategy implementation may update internal data structure to calculate a different wait time
        ///     before the next cycle of acquisition is performed.
        /// </summary>
        protected internal virtual void ConfigureNextAcquisitionCycle(JobAcquisitionContext acquisitionContext, IJobAcquisitionStrategy acquisitionStrategy)
        {
            acquisitionStrategy.Reconfigure(acquisitionContext);
        }

        protected internal virtual IJobAcquisitionStrategy InitializeAcquisitionStrategy()
        {
            return new BackoffJobAcquisitionStrategy(JobExecutor);
        }

        protected internal virtual void ExecuteJobs(JobAcquisitionContext context, ProcessEngineImpl currentProcessEngine, AcquiredJobs acquiredJobs)
        {
            // submit those jobs that were acquired in previous cycles but could not be scheduled for execution
            var additionalJobs = context.AdditionalJobBatchesByEngine.GetValueOrNull(currentProcessEngine.Name);
            if (additionalJobs != null)
                foreach (var jobBatch in additionalJobs)
                {
                    Log.ExecuteJobs(currentProcessEngine.Name, jobBatch);

                    JobExecutor.ExecuteJobs(jobBatch, currentProcessEngine);
                }

            // submit those jobs that were acquired in the current cycle
            foreach (var jobIds in acquiredJobs.JobIdBatches)
            {
                Log.ExecuteJobs(currentProcessEngine.Name, jobIds);

                JobExecutor.ExecuteJobs(jobIds, currentProcessEngine);
            }
        }

        protected virtual AcquiredJobs AcquireJobs(JobAcquisitionContext context, IJobAcquisitionStrategy acquisitionStrategy, ProcessEngineImpl currentProcessEngine)
        {
            ICommandExecutor commandExecutor = ((ProcessEngineConfigurationImpl)currentProcessEngine.ProcessEngineConfiguration).CommandExecutorTxRequired;

            var numJobsToAcquire = acquisitionStrategy.GetNumJobsToAcquire(currentProcessEngine.Name);

            AcquiredJobs acquiredJobs = null;

            if (numJobsToAcquire > 0)
            {
                JobExecutor.LogAcquisitionAttempt(currentProcessEngine);
                acquiredJobs = commandExecutor.Execute(JobExecutor.GetAcquireJobsCmd(numJobsToAcquire));
            }
            else
            {
                acquiredJobs = new AcquiredJobs(numJobsToAcquire);
            }

            context.SubmitAcquiredJobs(currentProcessEngine.Name, acquiredJobs);

            JobExecutor.LogAcquiredJobs(currentProcessEngine, acquiredJobs.Size());
            JobExecutor.LogAcquisitionFailureJobs(currentProcessEngine, acquiredJobs.NumberOfJobsFailedToLock);

            Log.AcquiredJobs(currentProcessEngine.Name, acquiredJobs);

            return acquiredJobs;
        }
    }
}