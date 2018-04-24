using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.UserTask
{


    /// <summary>
    /// Simple process test to validate the current implementation protoype.
    /// 
    ///  
    /// </summary>
    public class TaskAssigneeTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void testTaskAssignee()
        {

            // Start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("taskAssigneeExampleProcess");

            // Get task list
            IList<ITask> tasks = taskService.CreateTaskQuery().ToList().Where(c => c.Assignee == "kermit").ToList();
            Assert.IsTrue(tasks.Count > 0);
            ITask myTask = tasks[0];
            Assert.AreEqual("Schedule meeting", myTask.Name);
            Assert.AreEqual("Schedule an engineering meeting for next week with the new hire.", myTask.Description);

            // Complete task. Process is now finished
            taskService.Complete(myTask.Id);
            // Assert if the process instance completed
            AssertProcessEnded(processInstance.Id);
        }

    }

}