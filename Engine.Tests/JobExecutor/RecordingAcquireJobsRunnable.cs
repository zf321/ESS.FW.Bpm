using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    public class RecordingAcquireJobsRunnable : SequentialJobAcquisitionRunnable
    {

        protected internal virtual IList<RecordedWaitEvent> WaitEvents { get; set; } = new List<RecordedWaitEvent>();

        protected internal virtual IList<RecordedAcquisitionEvent> AcquisitionEvents { get; set; } = new List<RecordedAcquisitionEvent>();

        public RecordingAcquireJobsRunnable(ControllableJobExecutor jobExecutor) : base(jobExecutor)
        {
        }

        protected override void SuspendAcquisition(long millis)
        {
            Log.DebugJobAcquisitionThreadSleeping(millis);
            var executor = JobExecutor as ControllableJobExecutor;
            if (executor == null) return;
            var controllableExecutor = executor;
            if (controllableExecutor.SyncAsSuspendEnabled)
                controllableExecutor.AcquisitionThreadControl.Sync();
        }

        protected override AcquiredJobs AcquireJobs(JobAcquisitionContext context, IJobAcquisitionStrategy configuration, ProcessEngineImpl currentProcessEngine)
        {
            AcquisitionEvents.Add(new RecordedAcquisitionEvent(DateTime.Now.Ticks, configuration.GetNumJobsToAcquire(currentProcessEngine.Name)));
            return base.AcquireJobs(context, configuration, currentProcessEngine);
        }

        protected override void ConfigureNextAcquisitionCycle(JobAcquisitionContext context, IJobAcquisitionStrategy acquisitionStrategy)
        {
            base.ConfigureNextAcquisitionCycle(context, acquisitionStrategy);

            var timeBetweenCurrentAndNextAcquisition = acquisitionStrategy.WaitTime;
            WaitEvents.Add(new RecordedWaitEvent(DateTime.Now.Ticks, timeBetweenCurrentAndNextAcquisition));
        }

        public class RecordedWaitEvent
        {
            public RecordedWaitEvent(long timestamp, long timeBetweenAcquisitions)
            {
                Timestamp = timestamp;
                TimeBetweenAcquisitions = timeBetweenAcquisitions;
            }

            public virtual long Timestamp { get; }

            public virtual long TimeBetweenAcquisitions { get; }
        }

        public class RecordedAcquisitionEvent
        {
            public RecordedAcquisitionEvent(long timestamp, int numJobsToAcquire)
            {
                Timestamp = timestamp;
                NumJobsToAcquire = numJobsToAcquire;
            }

            public virtual long Timestamp { get; }

            public virtual int NumJobsToAcquire { get; }
        }
    }
}