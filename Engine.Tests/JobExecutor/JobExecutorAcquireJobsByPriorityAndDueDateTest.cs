using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    [TestFixture]
    public class JobExecutorAcquireJobsByPriorityAndDueDateTest : AbstractJobExecutorAcquireJobsTest
    {
        [SetUp]
        public virtual void PrepareProcessEngineConfiguration()
        {
            Configuration.JobExecutorAcquireByPriority = true;
            Configuration.SetJobExecutorAcquireByDueDate(true);
        }

        [Test]
        public virtual void TestProcessEngineConfiguration()
        {
            Assert.IsFalse(Configuration.JobExecutorPreferTimerJobs);
            Assert.True(Configuration.JobExecutorAcquireByDueDate);
            Assert.True(Configuration.JobExecutorAcquireByPriority);
        }

        [Test][Deployment( new []{ "resources/jobexecutor/jobPrioProcess.bpmn20.xml", "resources/jobexecutor/timerJobPrioProcess.bpmn20.xml" })]
        public virtual void TestAcquisitionByPriorityAndDueDate()
        {
            // job with priority 10
            var instance1 = StartProcess("jobPrioProcess", "task1");

            // job with priority 5
            ClockTestUtil.IncrementClock(1);
            var instance2 = StartProcess("jobPrioProcess", "task2");

            // job with priority 10
            ClockTestUtil.IncrementClock(1);
            var instance3 = StartProcess("jobPrioProcess", "task1");

            // job with priority 5
            ClockTestUtil.IncrementClock(1);
            var instance4 = StartProcess("jobPrioProcess", "task2");

            var acquirableJobs = FindAcquirableJobs();
            Assert.AreEqual(4, acquirableJobs.Count);
            Assert.AreEqual(instance1, acquirableJobs[0].ProcessInstanceId);
            Assert.AreEqual(instance3, acquirableJobs[1].ProcessInstanceId);
            Assert.AreEqual(instance2, acquirableJobs[2].ProcessInstanceId);
            Assert.AreEqual(instance4, acquirableJobs[3].ProcessInstanceId);
        }
    }
}