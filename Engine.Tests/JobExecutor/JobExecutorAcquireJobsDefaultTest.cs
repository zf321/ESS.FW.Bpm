using System.Linq;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    [TestFixture]
    public class JobExecutorAcquireJobsDefaultTest : AbstractJobExecutorAcquireJobsTest
    {
        [Test]
        public virtual void TestProcessEngineConfiguration()
        {
            Assert.IsFalse(Configuration.JobExecutorPreferTimerJobs);
            Assert.IsFalse(Configuration.JobExecutorAcquireByDueDate);
        }

        [Test][Deployment("resources/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
        public virtual void TestMessageJobHasNoDueDateSet()
        {
            RuntimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            var job = ManagementService.CreateJobQuery().First();
            Assert.IsNull(job.Duedate);
        }
    }
}