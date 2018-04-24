using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public class BackoffJobAcquisitionStrategy : IJobAcquisitionStrategy
    {
        public static long DefaultExecutionSaturationWaitTime = 100;

        /*
	   * all wait times are in milliseconds
	   */

        /*
	   * managing the idle level
	   */
        protected internal long BaseIdleWaitTime { get; set; }

        protected internal float IdleIncreaseFactor { get; set; }

        protected internal int IdleLevel { get; set; }

        protected internal int MaxIdleLevel { get; set; }

        protected internal long MaxIdleWaitTime { get; set; }

        /*
	   * managing the backoff level
	   */
        protected internal long BaseBackoffWaitTime { get; set; }

        protected internal float BackoffIncreaseFactor { get; set; }

        protected internal int BackoffLevel { get; set; }

        protected internal int MaxBackoffLevel { get; set; }

        protected internal long MaxBackoffWaitTime { get; set; }

        protected internal bool ApplyJitter { get; set; } = false;

        /*
	   * Keeping a history of recent acquisitions without locking failure
	   * for backoff level decrease
	   */
        protected internal int NumAcquisitionsWithoutLockingFailure { get; set; } = 0;

        protected internal int BackoffDecreaseThreshold { get; set; }

        protected internal int BaseNumJobsToAcquire { get; set; }

        protected internal IDictionary<string, int> JobsToAcquire { get; set; } = new Dictionary<string, int>();


        /*
	   * Backing off when the execution resources (queue) are saturated
	   * in order to not busy wait for free resources
	   */
        protected internal bool ExecutionSaturated { get; set; } = false;

        protected internal long ExecutionSaturationWaitTime { get; set; } = DefaultExecutionSaturationWaitTime;




        public BackoffJobAcquisitionStrategy(
            long baseIdleWaitTime,
            float idleIncreaseFactor,
            long maxIdleTime,
            long baseBackoffWaitTime,
            float backoffIncreaseFactor,
            long maxBackoffTime,
            int backoffDecreaseThreshold,
            int baseNumJobsToAcquire)
        {
            BaseIdleWaitTime = baseIdleWaitTime;
            IdleIncreaseFactor = idleIncreaseFactor;
            IdleLevel = 0;
            MaxIdleWaitTime = maxIdleTime;

            BaseBackoffWaitTime = baseBackoffWaitTime;
            BackoffIncreaseFactor = backoffIncreaseFactor;
            BackoffLevel = 0;
            MaxBackoffWaitTime = maxBackoffTime;
            BackoffDecreaseThreshold = backoffDecreaseThreshold;

            BaseNumJobsToAcquire = baseNumJobsToAcquire;

            InitializeMaxLevels();
        }

        public BackoffJobAcquisitionStrategy(JobExecutor jobExecutor)
            : this(jobExecutor.WaitTimeInMillis,
                jobExecutor.WaitIncreaseFactor,
                jobExecutor.MaxWait,
                jobExecutor.BackoffTimeInMillis,
                jobExecutor.WaitIncreaseFactor,
                jobExecutor.MaxBackoff,
                jobExecutor.BackoffDecreaseThreshold,
                jobExecutor.MaxJobsPerAcquisition)
        {
        }

        protected internal void InitializeMaxLevels()
        {
            if (BaseIdleWaitTime > 0 && MaxIdleWaitTime > 0 && IdleIncreaseFactor > 0 && MaxIdleWaitTime >= BaseIdleWaitTime)
            {
                // the maximum level that produces an idle time <= maxIdleTime:
                // see class docs for an explanation
                MaxIdleLevel = (int)Log(IdleIncreaseFactor, MaxIdleWaitTime / BaseIdleWaitTime) + 1;

                // + 1 to get the minimum level that produces an idle time > maxIdleTime
                MaxIdleLevel += 1;
            }
            else
            {
                MaxIdleLevel = 0;
            }

            if (BaseBackoffWaitTime > 0 && MaxBackoffWaitTime > 0 && BackoffIncreaseFactor > 0 && MaxBackoffWaitTime >= BaseBackoffWaitTime)
            {
                // the maximum level that produces a backoff time < maxBackoffTime:
                // see class docs for an explanation
                MaxBackoffLevel = (int)Log(BackoffIncreaseFactor, MaxBackoffWaitTime / BaseBackoffWaitTime) + 1;

                // + 1 to get the minimum level that produces a backoff time > maxBackoffTime
                MaxBackoffLevel += 1;
            }
            else
            {
                MaxBackoffLevel = 0;
            }
        }

        protected internal virtual double Log(double @base, double value)
        {
            return Math.Log10(value) / Math.Log10(@base);
        }


        public virtual void Reconfigure(JobAcquisitionContext context)
        {
            ReconfigureIdleLevel(context);
            ReconfigureBackoffLevel(context);
            ReconfigureNumberOfJobsToAcquire(context);
            ExecutionSaturated = AllSubmittedJobsRejected(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns> true, if all acquired jobs (spanning all engines) were rejected for execution</returns>
        protected internal virtual bool AllSubmittedJobsRejected(JobAcquisitionContext context)
        {
            foreach (var acquiredJobsForEngine in context.AcquiredJobsByEngine)
            {
                var engineName = acquiredJobsForEngine.Key;

                var acquiredJobBatches = acquiredJobsForEngine.Value.JobIdBatches;
                var resubmittedJobBatches = context.AdditionalJobBatchesByEngine.GetValueOrNull(engineName);
                var rejectedJobBatches = context.RejectedJobBatchesByEngine.GetValueOrNull(engineName);

                var numJobsSubmittedForExecution = acquiredJobBatches.Count;
                if (resubmittedJobBatches != null)
                    numJobsSubmittedForExecution += resubmittedJobBatches.Count;

                var numJobsRejected = 0;
                if (rejectedJobBatches != null)
                    numJobsRejected += rejectedJobBatches.Count;

                // if not all jobs scheduled for execution have been rejected
                if ((numJobsRejected == 0) || numJobsSubmittedForExecution > numJobsRejected)
                    return false;
            }

            return true;
        }

        protected internal virtual void ReconfigureIdleLevel(JobAcquisitionContext context)
        {
            if (context.JobAdded)
            {
                IdleLevel = 0;
            }
            else
            {
                if (context.AreAllEnginesIdle() || context.AcquisitionException != null)
                {
                    if (IdleLevel < MaxIdleLevel)
                        IdleLevel++;
                }
                else
                {
                    IdleLevel = 0;
                }
            }
        }

        protected internal virtual void ReconfigureBackoffLevel(JobAcquisitionContext context)
        {
            // if for any engine, jobs could not be locked due to optimistic locking, back off

            if (context.HasJobAcquisitionLockFailureOccurred())
            {
                NumAcquisitionsWithoutLockingFailure = 0;
                ApplyJitter = true;
                if (BackoffLevel < MaxBackoffLevel)
                    BackoffLevel++;
            }
            else
            {
                ApplyJitter = false;
                NumAcquisitionsWithoutLockingFailure++;
                if (NumAcquisitionsWithoutLockingFailure >= BackoffDecreaseThreshold && BackoffLevel > 0)
                {
                    BackoffLevel--;
                    NumAcquisitionsWithoutLockingFailure = 0;
                }
            }
        }

        protected internal virtual void ReconfigureNumberOfJobsToAcquire(JobAcquisitionContext context)
        {
            // calculate the number of jobs to acquire next time
            JobsToAcquire.Clear();
            foreach (var acquiredJobsEntry in context.AcquiredJobsByEngine)
            {
                var engineName = acquiredJobsEntry.Key;

                var numJobsToAcquire = (int)(BaseNumJobsToAcquire * Math.Pow(BackoffIncreaseFactor, BackoffLevel));
                var rejectedJobBatchesForEngine = context.RejectedJobBatchesByEngine.GetValueOrNull(engineName);
                if (rejectedJobBatchesForEngine != null)
                    numJobsToAcquire -= rejectedJobBatchesForEngine.Count;
                numJobsToAcquire = Math.Max(0, numJobsToAcquire);

                JobsToAcquire[engineName] = numJobsToAcquire;
            }
        }

        public virtual long WaitTime
        {
            get
            {
                if (IdleLevel > 0)
                    return CalculateIdleTime();
                if (BackoffLevel > 0)
                    return CalculateBackoffTime();
                if (ExecutionSaturated)
                    return ExecutionSaturationWaitTime;
                return 0;
            }
        }

        protected internal virtual long CalculateIdleTime()
        {
            if (IdleLevel <= 0)
                return 0;
            if (IdleLevel >= MaxIdleLevel)
                return MaxIdleWaitTime;
            return (long)(BaseIdleWaitTime * Math.Pow(IdleIncreaseFactor, IdleLevel - 1));
        }

        protected internal virtual long CalculateBackoffTime()
        {
            long backoffTime = 0;

            if (BackoffLevel <= 0)
                backoffTime = 0;
            else if (BackoffLevel >= MaxBackoffLevel)
                backoffTime = MaxBackoffWaitTime;
            else
                backoffTime = (long)(BaseBackoffWaitTime * Math.Pow(BackoffIncreaseFactor, BackoffLevel - 1));

            if (ApplyJitter)
                backoffTime += (long)(new Random().NextDouble() * (backoffTime / 2));

            return backoffTime;
        }
        
        public virtual int GetNumJobsToAcquire(string processEngine)
        {
            if (JobsToAcquire.ContainsKey(processEngine))
                return JobsToAcquire[processEngine];
            return BaseNumJobsToAcquire;
        }
    }
}