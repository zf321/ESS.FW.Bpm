using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.UserTask
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class TaskPriorityExtensionsTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testPriorityExtension()
        {
            testPriorityExtension(25);
            testPriorityExtension(75);
        }


        private void testPriorityExtension(int priority)
        {

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["taskPriority"] = priority;


            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("taskPriorityExtension",
                variables);


            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

            Assert.AreEqual(priority, task.Priority);
        }


        [Test]
        [Deployment]
        public virtual void testPriorityExtensionString()
        {

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("taskPriorityExtensionString");

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
            Assert.AreEqual(42, task.Priority);
        }
    }

}