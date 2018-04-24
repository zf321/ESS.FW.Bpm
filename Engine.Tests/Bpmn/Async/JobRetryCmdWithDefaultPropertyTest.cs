using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Async
{

    [TestFixture]
    public class JobRetryCmdWithDefaultPropertyTest : ResourceProcessEngineTestCase
    {

        public JobRetryCmdWithDefaultPropertyTest() : base("resource/bpmn/async/default.job.Retry.property.Camunda.cfg.xml")
        {
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedTask.bpmn20.xml" })]
        public virtual void testDefaultNumberOfRetryProperty()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedTask");
            Assert.NotNull(pi);

            IJob job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi.ProcessInstanceId).First();
            Assert.NotNull(job);
            Assert.AreEqual(pi.ProcessInstanceId, job.ProcessInstanceId);
            Assert.AreEqual(2, job.Retries);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedServiceTask.bpmn20.xml" })]
        public virtual void testOverwritingPropertyWithBpmnExtension()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedServiceTask");
            Assert.NotNull(pi);

            IJob job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi.ProcessInstanceId).First();
            Assert.NotNull(job);
            Assert.AreEqual(pi.ProcessInstanceId, job.ProcessInstanceId);

            try
            {
                managementService.ExecuteJob(job.Id);
                Assert.Fail("Exception expected!");
            }
            catch (System.Exception)
            {
                // expected
            }

            job = managementService.CreateJobQuery(c=>c.Id ==job.Id).First();
            Assert.AreEqual(4, job.Retries);

        }
    }

}