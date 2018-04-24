using System.Linq;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Async
{
    [TestFixture]
    public class AsyncCallActivityTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment(new string[] { "resources/bpmn/async/AsyncCallActivityTest.asyncStartEvent.bpmn20.xml", "resources/bpmn/async/AsyncCallActivityTest.TestCallSubProcess.bpmn20.xml" })]
        public virtual void testCallProcessWithAsyncOnStartEvent()
        {

            runtimeService.StartProcessInstanceByKey("callAsyncSubProcess");

            ESS.FW.Bpm.Engine.Runtime.IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            managementService.ExecuteJob(job.Id);

            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

        }
    }

}