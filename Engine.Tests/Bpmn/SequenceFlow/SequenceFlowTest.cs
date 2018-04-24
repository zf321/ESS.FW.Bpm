using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.SequenceFlow
{
    [TestFixture]
    public class SequenceFlowTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void TestTakeAllOutgoingFlowsFromNonScopeTask()
        {
            // given
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("testProcess");

            // when
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            // then
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task2").Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task3").Count());
            var tasks = taskService.CreateTaskQuery().ToList();
            foreach (ITask followUpTask in tasks)
            {
                taskService.Complete(followUpTask.Id);
            }

            AssertProcessEnded(instance.Id);

        }

        [Test]
        [Deployment]
        public virtual void TestTakeAllOutgoingFlowsFromScopeTask()
        {
            // given
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("testProcess");

            // when
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            // then
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task2").Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task3").Count());

            foreach (ITask followUpTask in taskService.CreateTaskQuery().ToList())
            {
                taskService.Complete(followUpTask.Id);
            }

            AssertProcessEnded(instance.Id);
        }
    }

}