using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    [TestFixture]
    public class JobExecutorAcquireJobsByDueDateTest : AbstractJobExecutorAcquireJobsTest
    {
        [SetUp]
        public virtual void PrepareProcessEngineConfiguration()
        {
            Configuration.SetJobExecutorAcquireByDueDate(true);
        }

        [Test]
        public virtual void TestProcessEngineConfiguration()
        {
            Assert.IsFalse(Configuration.JobExecutorPreferTimerJobs);
            Assert.True(Configuration.JobExecutorAcquireByDueDate);
            Assert.IsFalse(Configuration.JobExecutorAcquireByPriority);
        }

        [Test][Deployment(  "resources/jobexecutor/simpleAsyncProcess.bpmn20.xml") ]
        public virtual void TestMessageJobHasDueDateSet()
        {
            RuntimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            var job = ManagementService.CreateJobQuery().SingleOrDefault();
            Assert.NotNull(job.Duedate);

            // Todo: C# DateTime -> Oracle timestamp(6)小数秒丢失
            Assert.AreEqual(ClockUtil.CurrentTime.ToString("yyyy-MM-dd HH:mm:ss"), job.Duedate?.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        [Test][Deployment( new string[] { "resources/jobexecutor/simpleAsyncProcess.bpmn20.xml", "resources/jobexecutor/processWithTimerCatch.bpmn20.xml" })]
        public virtual void TestOldJobsArePreferred()
        {
            // first start process with timer job
            var timerProcess1 = RuntimeService.StartProcessInstanceByKey("testProcess");
            // then start process with async task
            ClockTestUtil.IncrementClock(1);
            var asyncProcess1 = RuntimeService.StartProcessInstanceByKey("simpleAsyncProcess");
            // then start process with timer job
            ClockTestUtil.IncrementClock(1);
            var timerProcess2 = RuntimeService.StartProcessInstanceByKey("testProcess");
            // and another process with async task after the timers are acquirable
            ClockTestUtil.IncrementClock(61);
            var asyncProcess2 = RuntimeService.StartProcessInstanceByKey("simpleAsyncProcess");
            string id1 = timerProcess1.Id;
            var timerJob1 = ManagementService.CreateJobQuery(c=>c.ProcessInstanceId == id1).FirstOrDefault();
            var timerJob2 = ManagementService.CreateJobQuery(c=>c.ProcessInstanceId == timerProcess2.Id).First();
            var messageJob1 = ManagementService.CreateJobQuery(c=>c.ProcessInstanceId == asyncProcess1.Id).First();
            var messageJob2 = ManagementService.CreateJobQuery(c=>c.ProcessInstanceId == asyncProcess2.Id).First();

            Assert.NotNull(timerJob1.Duedate);
            Assert.NotNull(timerJob2.Duedate);
            Assert.NotNull(messageJob1.Duedate);
            Assert.NotNull(messageJob2.Duedate);

            Assert.True(messageJob1.Duedate < timerJob1.Duedate);
            Assert.True(timerJob1.Duedate < timerJob2.Duedate);
            Assert.True(timerJob2.Duedate < messageJob2.Duedate);

            var acquirableJobs = FindAcquirableJobs();
            Assert.AreEqual(4, acquirableJobs.Count);
            Assert.AreEqual(messageJob1.Id, acquirableJobs[0].Id);
            Assert.AreEqual(timerJob1.Id, acquirableJobs[1].Id);
            Assert.AreEqual(timerJob2.Id, acquirableJobs[2].Id);
            Assert.AreEqual(messageJob2.Id, acquirableJobs[3].Id);
        }
    }
}