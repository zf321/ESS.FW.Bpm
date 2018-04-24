using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    [TestFixture]
    public class JobExecutorAcquireJobsByTypeTest : AbstractJobExecutorAcquireJobsTest
    {
        [SetUp]
        public virtual void PrepareProcessEngineConfiguration()
        {
            Configuration.SetJobExecutorPreferTimerJobs(true);
        }

        [Test]
        public virtual void TestProcessEngineConfiguration()
        {
            Assert.True(Configuration.JobExecutorPreferTimerJobs);
            Assert.IsFalse(Configuration.JobExecutorAcquireByDueDate);
            Assert.IsFalse(Configuration.JobExecutorAcquireByPriority);
        }

        [Test][Deployment( "resources/jobexecutor/simpleAsyncProcess.bpmn20.xml") ]
        public virtual void TestMessageJobHasNoDueDateSet()
        {
            RuntimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            var job = ManagementService.CreateJobQuery().First();
            Assert.IsNull(job.Duedate);
        }

        [Test][Deployment( new []{ "resources/jobexecutor/simpleAsyncProcess.bpmn20.xml", "resources/jobexecutor/processWithTimerCatch.bpmn20.xml" })]
        public virtual void TestTimerJobsArePreferred()
        {
            // first start process with timer job
            RuntimeService.StartProcessInstanceByKey("testProcess");
            // then start process with async task
            RuntimeService.StartProcessInstanceByKey("simpleAsyncProcess");
            // then start process with timer job
            RuntimeService.StartProcessInstanceByKey("testProcess");
            // and another process with async task
            RuntimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            // increment clock so that timer events are acquirable
            ClockTestUtil.IncrementClock(70);

            var acquirableJobs = FindAcquirableJobs();
            Assert.AreEqual(4, acquirableJobs.Count);
            Assert.True(acquirableJobs[0] is TimerEntity);
            Assert.True(acquirableJobs[1] is TimerEntity);
            Assert.True(acquirableJobs[2] is MessageEntity);
            Assert.True(acquirableJobs[3] is MessageEntity);
        }
    }
}