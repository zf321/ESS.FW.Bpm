using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    [TestFixture]
    public class JobExecutorAcquireJobsByDueDateNotPriorityTest : AbstractJobExecutorAcquireJobsTest
    {
        [SetUp]
        public virtual void PrepareProcessEngineConfiguration()
        {
            Configuration.SetJobExecutorAcquireByDueDate(true);
        }

        [Test][Deployment("resources/jobexecutor/jobPrioProcess.bpmn20.xml") ]
        public virtual void TestJobPriorityIsNotConsidered()
        {
            // prio 5
            var instance1 = StartProcess("jobPrioProcess", "task2");

            // prio 10
            ClockTestUtil.IncrementClock(1);
            var instance2 = StartProcess("jobPrioProcess", "task1");

            // prio 5
            ClockTestUtil.IncrementClock(1);
            var instance3 = StartProcess("jobPrioProcess", "task2");

            // prio 10
            ClockTestUtil.IncrementClock(1);
            var instance4 = StartProcess("jobPrioProcess", "task1");

            var acquirableJobs = FindAcquirableJobs();
            Assert.AreEqual(4, acquirableJobs.Count);

            Assert.AreEqual(5, (int) acquirableJobs[0].Priority);
            Assert.AreEqual(instance1, acquirableJobs[0].ProcessInstanceId);
            Assert.AreEqual(10, (int) acquirableJobs[1].Priority);
            Assert.AreEqual(instance2, acquirableJobs[1].ProcessInstanceId);
            Assert.AreEqual(5, (int) acquirableJobs[2].Priority);
            Assert.AreEqual(instance3, acquirableJobs[2].ProcessInstanceId);
            Assert.AreEqual(10, (int) acquirableJobs[3].Priority);
            Assert.AreEqual(instance4, acquirableJobs[3].ProcessInstanceId);
        }
    }
}