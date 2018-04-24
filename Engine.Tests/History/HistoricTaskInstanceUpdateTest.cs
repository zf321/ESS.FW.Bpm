using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.History
{



    /// <summary>
    /// 
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity) ]

    [TestFixture]
    public class HistoricTaskInstanceUpdateTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testHistoricTaskInstanceUpdate()
		{
		    var id = runtimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest").Id;

		    ITask task = taskService.CreateTaskQuery().First();

		// Update and save the task's fields before it is finished
		task.Priority = 12345;
		task.Description = "Updated description";
		task.Name = "Updated name";
		task.Assignee = "gonzo";
		taskService.SaveTask(task);

		taskService.Complete(task.Id);
		Assert.AreEqual(true,historyService.CreateHistoricTaskInstanceQuery().Count()>0);

		IHistoricTaskInstance historicTaskInstance = historyService.CreateHistoricTaskInstanceQuery().First();
		Assert.AreEqual("Updated name", historicTaskInstance.Name);
		Assert.AreEqual("Updated description", historicTaskInstance.Description);
		Assert.AreEqual("gonzo", historicTaskInstance.Assignee);
		Assert.AreEqual("task", historicTaskInstance.TaskDefinitionKey);
		}
	}

}