using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.SequenceFlow
{
    [TestFixture]
    public class UncontrolledFlowTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void TestSubProcessTwoOutgoingFlowsCorrelateMessage()
        {
            // given a process instance
            runtimeService.StartProcessInstanceByKey("process");

            // that leaves the sub process via two outgoing sequence flows
            ITask innerTask = taskService.CreateTaskQuery().First();
            taskService.Complete(innerTask.Id);

            // then there are two tasks after the sub process
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "outerTask1").Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "outerTask2").Count());

            // and then the message for the event subprocess cannot be delivered
            try
            {
                //内部自定义异常，没有throw
                runtimeService.CorrelateMessage("Message1");
                Assert.Fail("should not succeed");
            }
            catch (ProcessEngineException ex)
            {
                var test = ex.Message;
            }
            catch (System.Exception e)
            {
                AssertTextPresent("Cannot correlate message 'Message1'", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void TestSubProcessTwoOutgoingFlowsEndProcess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");
            ITask innerTask = taskService.CreateTaskQuery().First();

            taskService.Complete(innerTask.Id);

            AssertProcessEnded(processInstance.Id);
        }
    }

}