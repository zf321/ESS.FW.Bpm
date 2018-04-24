using System.Linq;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Exclusive
{
    [TestFixture]
    public class ExclusiveEndEventTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testNonExclusiveEndEvent()
        {
            // start process
            runtimeService.StartProcessInstanceByKey("exclusive");
            // now there should be 1 non-exclusive job in the database:
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.IsFalse(((JobEntity)job).Exclusive);

            WaitForJobExecutorToProcessAllJobs(6000L);

            // all the jobs are done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testExclusiveEndEvent()
        {
            // start process 
            runtimeService.StartProcessInstanceByKey("exclusive");
            // now there should be 1 exclusive job in the database:
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.True(((JobEntity)job).Exclusive);

            WaitForJobExecutorToProcessAllJobs(6000L);

            // all the jobs are done
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());
        }
    }

}