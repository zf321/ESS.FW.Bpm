using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class BackoffJobAcquisitionStrategyTest
    {
        [SetUp]
        public virtual void SetUp()
        {
            Strategy = new BackoffJobAcquisitionStrategy(BaseIdleWaitTime, IdleIncreaseFactor, MaxIdleTime,
                BaseBackoffWaitTime, BackoffIncreaseFactor, MaxBackoffTime, DecreaseThreshold, NumJobsToAcquire);
        }

        // strategy configuration
        protected internal const long BaseIdleWaitTime = 50;
        protected internal const float IdleIncreaseFactor = 1.5f;
        protected internal const long MaxIdleTime = 500;

        protected internal const long BaseBackoffWaitTime = 80;
        protected internal const float BackoffIncreaseFactor = 2.0f;
        protected internal const long MaxBackoffTime = 1000;

        protected internal const int DecreaseThreshold = 3;
        protected internal const int NumJobsToAcquire = 10;

        // misc
        protected internal const string EngineName = "engine";

        protected internal IJobAcquisitionStrategy Strategy;

        [Test]
        public virtual void TestIdleWaitTime()
        {
            // given a job acquisition strategy and a job acquisition context
            // with no acquired jobs
            var context = new JobAcquisitionContext();

            context.SubmitAcquiredJobs(EngineName, BuildAcquiredJobs(NumJobsToAcquire, 0, 0));

            // when reconfiguring the strategy
            Strategy.Reconfigure(context);

            // then the job acquisition strategy returns the level 1 idle time
            Assert.AreEqual(BaseIdleWaitTime, Strategy.WaitTime);

            // when resubmitting the same acquisition result
            for (var idleLevel = 1; idleLevel < 6; idleLevel++)
            {
                context.Reset();
                context.SubmitAcquiredJobs(EngineName, BuildAcquiredJobs(NumJobsToAcquire, 0, 0));

                Strategy.Reconfigure(context);
                Assert.AreEqual((long)(BaseIdleWaitTime * Math.Pow(IdleIncreaseFactor, idleLevel)), Strategy.WaitTime);
            }

            // and the maximum idle level is finally reached
            context.Reset();
            context.SubmitAcquiredJobs(EngineName, BuildAcquiredJobs(NumJobsToAcquire, 0, 0));

            Strategy.Reconfigure(context);
            Assert.AreEqual(MaxIdleTime, Strategy.WaitTime);
        }

        [Test]
        public virtual void TestAcquisitionAfterIdleWait()
        {
            // given a job acquisition strategy and a job acquisition context
            // with no acquired jobs
            var context = new JobAcquisitionContext();

            context.SubmitAcquiredJobs(EngineName, BuildAcquiredJobs(NumJobsToAcquire, 0, 0));
            Strategy.Reconfigure(context);
            Assert.AreEqual(BaseIdleWaitTime, Strategy.WaitTime);

            // when receiving a successful acquisition result
            context.Reset();
            context.SubmitAcquiredJobs(EngineName, BuildAcquiredJobs(NumJobsToAcquire, NumJobsToAcquire, 0));

            Strategy.Reconfigure(context);

            // then the idle Wait time has been reset
            Assert.AreEqual(0L, Strategy.WaitTime);
        }

        [Test]
        public virtual void TestAcquireLessJobsOnRejection()
        {
            // given a job acquisition strategy and a job acquisition context
            // with acquired jobs, some of which have been rejected for execution
            var context = new JobAcquisitionContext();

            var acquiredJobs = BuildAcquiredJobs(NumJobsToAcquire, NumJobsToAcquire, 0);
            context.SubmitAcquiredJobs(EngineName, acquiredJobs);

            // when half of the jobs are rejected
            var numJobsRejected = 5;
            for (var i = 0; i < numJobsRejected; i++)
                context.SubmitRejectedBatch(EngineName, acquiredJobs.JobIdBatches[i]);

            // then the strategy only attempts to acquire the number of jobs that were successfully submitted
            Strategy.Reconfigure(context);

            Assert.AreEqual(NumJobsToAcquire - numJobsRejected, Strategy.GetNumJobsToAcquire(EngineName));

            // without a timeout
            Assert.AreEqual(0, Strategy.WaitTime);
        }

        [Test]
        public virtual void TestWaitTimeOnFullRejection()
        {
            // given a job acquisition strategy and a job acquisition context
            // with acquired jobs all of which have been rejected for execution
            var context = new JobAcquisitionContext();

            var acquiredJobs = BuildAcquiredJobs(NumJobsToAcquire, NumJobsToAcquire, 0);
            context.SubmitAcquiredJobs(EngineName, acquiredJobs);

            for (var i = 0; i < NumJobsToAcquire; i++)
                context.SubmitRejectedBatch(EngineName, acquiredJobs.JobIdBatches[i]);

            // when reconfiguring the strategy
            Strategy.Reconfigure(context);

            // then there is a slight Wait time to avoid constant spinning while
            // no execution resources are available
            Assert.AreEqual(BackoffJobAcquisitionStrategy.DefaultExecutionSaturationWaitTime, Strategy.WaitTime);
        }

        /// <summary>
        ///     numJobsToAcquire >= numJobsAcquired >= numJobsFailedToLock must hold
        /// </summary>
        protected internal virtual AcquiredJobs BuildAcquiredJobs(int numJobsToAcquire, int numJobsAcquired,
            int numJobsFailedToLock)
        {
            var acquiredJobs = new AcquiredJobs(numJobsToAcquire);
            for (var i = 0; i < numJobsAcquired; i++)
                //acquiredJobs.AddJobIdBatch(Convert.ToString(i));
                acquiredJobs.AddJobIdBatch(new List<string> { i.ToString() });

            for (var i = 0; i < numJobsFailedToLock; i++)
                acquiredJobs.RemoveJobId(Convert.ToString(i));

            return acquiredJobs;
        }
    }
}