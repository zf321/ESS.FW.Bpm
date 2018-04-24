using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    [TestFixture]
    public class JobExecutorAcquireJobsByPriorityTest : AbstractJobExecutorAcquireJobsTest
    {
        [SetUp]
        public virtual void PrepareProcessEngineConfiguration()
        {
            Configuration.JobExecutorAcquireByPriority = true;
        }

        [Test]
        public virtual void TestProcessEngineConfiguration()
        {
            Assert.IsFalse(Configuration.JobExecutorPreferTimerJobs);
            Assert.IsFalse(Configuration.JobExecutorAcquireByDueDate);
            Assert.True(Configuration.JobExecutorAcquireByPriority);
        }

        [Test][Deployment( new [] { "resources/jobexecutor/jobPrioProcess.bpmn20.xml", "resources/jobexecutor/timerJobPrioProcess.bpmn20.xml" })]
        public virtual void TestAcquisitionByPriority()
        {
            // jobs with priority 10
            StartProcess("jobPrioProcess", "task1", 5);

            // jobs with priority 5
            StartProcess("jobPrioProcess", "task2", 5);

            // jobs with priority 8
            StartProcess("timerJobPrioProcess", "timer1", 5);

            // jobs with priority 4
            StartProcess("timerJobPrioProcess", "timer2", 5);

            // make timers due
            ClockTestUtil.IncrementClock(61);

            var acquirableJobs = FindAcquirableJobs();
            Assert.AreEqual(20, acquirableJobs.Count);
            for (var i = 0; i < 5; i++)
                Assert.AreEqual(10, acquirableJobs[i].Priority);

            for (var i = 5; i < 10; i++)
                Assert.AreEqual(8, acquirableJobs[i].Priority);

            for (var i = 10; i < 15; i++)
                Assert.AreEqual(5, acquirableJobs[i].Priority);

            for (var i = 15; i < 20; i++)
                Assert.AreEqual(4, acquirableJobs[i].Priority);
        }

        [Test][Deployment( "resources/jobexecutor/jobPrioProcess.bpmn20.xml")]
        public virtual void TestMixedPriorityAcquisition()
        {
            // jobs with priority 10
            StartProcess("jobPrioProcess", "task1", 5);

            // jobs with priority 5
            StartProcess("jobPrioProcess", "task2", 5);

            // set some job priorities to NULL indicating that they were produced without priorities
        }
    }
}