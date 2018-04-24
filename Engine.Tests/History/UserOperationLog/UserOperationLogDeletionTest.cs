using System.Linq;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.History.UserOperationLog
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    [TestFixture]
    public class UserOperationLogDeletionTest : AbstractUserOperationLogTest
	{

	  protected internal const string PROCESS_PATH = "resources/api/oneTaskProcess.bpmn20.xml";
	  protected internal const string PROCESS_KEY = "oneTaskProcess";

        [Test][Deployment( PROCESS_PATH) ]
	  public virtual void testDeleteProcessTaskKeepTaskOperationLog()
	  {
		// given
		runtimeService.StartProcessInstanceByKey(PROCESS_KEY);

		string taskId = taskService.CreateTaskQuery().First().Id;
		taskService.SetAssignee(taskId, "demo");
		taskService.Complete(taskId);

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.Id == taskId);
		Assert.AreEqual(2, query.Count());

		// when
		historyService.DeleteHistoricTaskInstance(taskId);

		// then
		Assert.AreEqual(2, query.Count());
	  }

        [Test]
        public virtual void testDeleteStandaloneTaskKeepUserOperationLog()
	  {
		// given
		string taskId = "my-task";
		ITask task = taskService.NewTask(taskId);
		taskService.SaveTask(task);

		taskService.SetAssignee(taskId, "demo");
		taskService.Complete(taskId);

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.Id == taskId);
		Assert.AreEqual(3, query.Count());

		// when
		historyService.DeleteHistoricTaskInstance(taskId);

		// then
		Assert.AreEqual(3, query.Count());
	  }

        [Test][Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testDeleteCaseTaskKeepUserOperationLog()
	  {
		// given
		caseService.WithCaseDefinitionByKey("oneTaskCase").Create();

		string caseExecutionId = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1").First().Id;

		string taskId = taskService.CreateTaskQuery().First().Id;
		taskService.SetAssignee(taskId, "demo");
		taskService.Complete(taskId);

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.Id == taskId);
		Assert.AreEqual(2, query.Count());

		// when
		historyService.DeleteHistoricTaskInstance(taskId);

		// then
		Assert.AreEqual(2, query.Count());
	  }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testDeleteProcessInstanceKeepUserOperationLog()
	  {
		// given
		string ProcessInstanceId = runtimeService.StartProcessInstanceByKey(PROCESS_KEY).Id;

		runtimeService.SuspendProcessInstanceById(ProcessInstanceId);
		runtimeService.ActivateProcessInstanceById(ProcessInstanceId);

		string taskId = taskService.CreateTaskQuery().First().Id;
		taskService.Complete(taskId);

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.ProcessInstanceId == ProcessInstanceId);
		Assert.AreEqual(3, query.Count());

		// when
		historyService.DeleteHistoricProcessInstance(ProcessInstanceId);

		// then
		Assert.AreEqual(3, query.Count());
	  }

        [Test][Deployment("resources/api/cmmn/oneTaskCase.cmmn") ]
        public virtual void testDeleteCaseInstanceKeepUserOperationLog()
	  {
		// given
		string caseInstanceId = caseService.WithCaseDefinitionByKey("oneTaskCase").Create().Id;

		string caseExecutionId = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1").First().Id;

		string taskId = taskService.CreateTaskQuery().First().Id;
		taskService.Complete(taskId);

		caseService.CloseCaseInstance(caseInstanceId);

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.CaseInstanceId ==caseInstanceId);
		Assert.AreEqual(1, query.Count());

		// when
		historyService.DeleteHistoricCaseInstance(caseInstanceId);

		// then
		Assert.AreEqual(1, query.Count());
	  }


        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testDeleteProcessDefinitionKeepUserOperationLog()
	  {
		// given
		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		string ProcessInstanceId = runtimeService.StartProcessInstanceByKey(PROCESS_KEY).Id;

		runtimeService.SuspendProcessInstanceById(ProcessInstanceId);

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.ProcessInstanceId == ProcessInstanceId);
		Assert.AreEqual(1, query.Count());

		// when
		repositoryService.DeleteProcessDefinition(processDefinitionId, true);

		// then new log is created and old stays
		Assert.AreEqual(1, query.Count());
	  }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testDeleteDeploymentKeepUserOperationLog()
	  {
		// given
		string deploymentId = repositoryService.CreateDeploymentQuery().First().Id;

		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;

		repositoryService.SuspendProcessDefinitionById(processDefinitionId);

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.ProcessDefinitionId ==processDefinitionId);
		Assert.AreEqual(1, query.Count());

		// when
		repositoryService.DeleteDeployment(deploymentId, true);

		// then
		Assert.AreEqual(1, query.Count());
	  }

	}

}