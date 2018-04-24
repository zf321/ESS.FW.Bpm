using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Calendar;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.History.UserOperationLog
{


    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class UserOperationLogTaskTest : AbstractUserOperationLogTest
	{

	  protected internal IProcessDefinition processDefinition;
	  protected internal IProcessInstance process;
	  protected internal ITask task;

        [Test][Deployment( "resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testCreateAndCompleteTask()
	  {
		startTestProcess();

		// expect: no entry for the task creation by process engine
		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();
		Assert.AreEqual(0, query.Count());

		completeTestProcess();

		// expect: one entry for the task completion
		query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeComplete);
		Assert.AreEqual(1, query.Count());
		IUserOperationLogEntry complete = query.First();
		Assert.AreEqual(Permissions.Delete, complete.Property);
		Assert.True(bool.Parse(complete.NewValue));
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testAssignTask()
	  {
		startTestProcess();

		// then: assign the task
		taskService.SetAssignee(task.Id, "icke");

		// expect: one entry for the task assignment
		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeAssign);
		Assert.AreEqual(1, query.Count());

		// Assert: details
		IUserOperationLogEntry assign = query.First();
		Assert.AreEqual(TaskEntity.ASSIGNEE, assign.Property);
		Assert.AreEqual("icke", assign.NewValue);

		completeTestProcess();
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testChangeTaskOwner()
	  {
		startTestProcess();

		// then: change the task owner
		taskService.SetOwner(task.Id, "icke");

		// expect: one entry for the owner change
		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeSetOwner);
		Assert.AreEqual(1, query.Count());

		// Assert: details
		IUserOperationLogEntry change = query.First();
		Assert.AreEqual(TaskEntity.OWNER, change.Property);
		Assert.AreEqual("icke", change.NewValue);

		completeTestProcess();
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testSetPriority()
	  {
		startTestProcess();

		// then: set the priority of the task to 10
		taskService.SetPriority(task.Id, 10);

		// expect: one entry for the priority update
		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeSetPriority);
		Assert.AreEqual(1, query.Count());

		// Assert: correct priority set
		IUserOperationLogEntry userOperationLogEntry = query.First();
		Assert.AreEqual(TaskEntity.PRIORITY, userOperationLogEntry.Property);
		// note: 50 is the default task priority
		Assert.AreEqual(50, int.Parse(userOperationLogEntry.OrgValue));
		Assert.AreEqual(10, int.Parse(userOperationLogEntry.NewValue));

		// move clock by 5 minutes
		DateTime date = DateTimeUtil.Now().AddMinutes(5);
		ClockUtil.CurrentTime = date;

		// then: set priority again
		taskService.SetPriority(task.Id, 75);

		// expect: one entry for the priority update
		query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeSetPriority);
		Assert.AreEqual(2, query.Count());

		// Assert: correct priority set
		//userOperationLogEntry = query/*.OrderByTimestamp()*//*.Asc()*/.ToList()[1];
		Assert.AreEqual(TaskEntity.PRIORITY, userOperationLogEntry.Property);
		Assert.AreEqual(10, int.Parse(userOperationLogEntry.OrgValue));
		Assert.AreEqual(75, int.Parse(userOperationLogEntry.NewValue));
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testClaimTask()
	  {
		startTestProcess();

		// then: claim a new the task
		taskService.Claim(task.Id, "icke");

		// expect: one entry for the claim
		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeClaim);
		Assert.AreEqual(1, query.Count());

		// Assert: details
		IUserOperationLogEntry claim = query.First();
		Assert.AreEqual(TaskEntity.ASSIGNEE, claim.Property);
		Assert.AreEqual("icke", claim.NewValue);

		completeTestProcess();
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testDelegateTask()
	  {
		startTestProcess();

		// then: delegate the assigned task
		taskService.Claim(task.Id, "icke");
		taskService.DelegateTask(task.Id, "er");

		// expect: three entries for the delegation
		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeDelegate);
		Assert.AreEqual(3, query.Count());

		// Assert: details
		Assert.AreEqual("icke", queryOperationDetails(UserOperationLogEntryFields.OperationTypeDelegate, TaskEntity.OWNER).First().NewValue);
		Assert.AreEqual("er", queryOperationDetails(UserOperationLogEntryFields.OperationTypeDelegate, TaskEntity.ASSIGNEE).First().NewValue);
		Assert.AreEqual(DelegationState.Pending.ToString(), queryOperationDetails(UserOperationLogEntryFields.OperationTypeDelegate, TaskEntity.Delegation).First().NewValue);

		completeTestProcess();
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testResolveTask()
	  {
		startTestProcess();

		// then: resolve the task
		taskService.ResolveTask(task.Id);

		// expect: one entry for the resolving
		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeResolve);
		Assert.AreEqual(1, query.Count());

		// Assert: details
		Assert.AreEqual(DelegationState.Resolved.ToString(), query.First().NewValue);

		completeTestProcess();
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testSubmitTaskForm_Complete()
	  {
		startTestProcess();

		formService.SubmitTaskForm(task.Id, new Dictionary<string, ITypedValue>());

		// expect: two entries for the resolving (delegation and assignee changed)
		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeComplete);
		Assert.AreEqual(1, query.Count());

		// Assert: Delete
		//Assert.IsFalse(bool.Parse(query.Property("Delete").First().OrgValue));
		//Assert.True(bool.Parse(query.Property("Delete").First().NewValue));

		AssertProcessEnded(process.Id);
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testSubmitTaskForm_Resolve()
	  {
		startTestProcess();

		taskService.DelegateTask(task.Id, "demo");

		formService.SubmitTaskForm(task.Id, new Dictionary<string, ITypedValue>());

		// expect: two entries for the resolving (delegation and assignee changed)
		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeResolve);
		Assert.AreEqual(2, query.Count());

		//// Assert: delegation
		//Assert.AreEqual(DelegationState.Pending.ToString(), query.Property("delegation").First().OrgValue);
		//Assert.AreEqual(DelegationState.Resolved.ToString(), query.Property("delegation").First().NewValue);

		//// Assert: assignee
		//Assert.AreEqual("demo", query.Property("assignee").First().OrgValue);
		//Assert.AreEqual(null, query.Property("assignee").First().NewValue);

		completeTestProcess();
	  }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCompleteCaseExecution()
	  {
		// given
		//string caseDefinitionId = repositoryService.CreateCaseDefinitionQuery().First().Id;

		//string caseInstanceId = caseService.WithCaseDefinition(caseDefinitionId).Create().Id;

		//string humanTaskId = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1").First().Id;

		// when
		//caseService.WithCaseExecution(humanTaskId).Complete();

		// then
		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeComplete);

		Assert.AreEqual(1, query.Count());

		IUserOperationLogEntry entry = query.First();
		Assert.NotNull(entry);

		//Assert.AreEqual(caseDefinitionId, entry.CaseDefinitionId);
		//Assert.AreEqual(caseInstanceId, entry.CaseInstanceId);
		//Assert.AreEqual(humanTaskId, entry.CaseExecutionId);
		Assert.AreEqual(DeploymentId, entry.DeploymentId);

		Assert.IsFalse(Convert.ToBoolean(entry.OrgValue));
		Assert.True(Convert.ToBoolean(entry.NewValue));
		Assert.AreEqual(Permissions.Delete, entry.Property);

	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testKeepOpLogEntriesOnUndeployment()
	  {
		// given
		startTestProcess();
		// an op log entry directly related to the process instance is created
		taskService.ResolveTask(task.Id);

		// and an op log entry with indirect reference to the process instance is created
		runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

		// when
		// the deployment is deleted with cascade
		repositoryService.DeleteDeployment(DeploymentId, true);

		// then
		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();
		Assert.AreEqual(3, query.Count());
		//Assert.AreEqual(1, query.Where(c=>c.OperationType==UserOperationLogEntryFields.OperationTypeSuspend).Count());
		//Assert.AreEqual(1, query.Where(c=>c.OperationType==UserOperationLogEntryFields.OperationTypeResolve).Count());
		//Assert.AreEqual(1, query.Where(c=>c.OperationType==UserOperationLogEntryFields.OperationTypeDelete).Count());
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteOpLogEntry()
	  {
		// given
		startTestProcess();

		// an op log instance is created
		taskService.ResolveTask(task.Id);
		IUserOperationLogEntry opLogEntry = historyService.CreateUserOperationLogQuery().First();

		// when the op log instance is deleted
		historyService.DeleteUserOperationLogEntry(opLogEntry.Id);

		// then it should be removed from the database
		Assert.AreEqual(0, historyService.CreateUserOperationLogQuery().Count());
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteOpLogEntryWithNullArgument()
	  {
		// given
		startTestProcess();

		// an op log instance is created
		taskService.ResolveTask(task.Id);

		// when null is used as deletion parameter
		try
		{
		  historyService.DeleteUserOperationLogEntry(null);
		  Assert.Fail("exeception expected");
		}
		catch (NotValidException)
		{
		  // then there should be an exception that signals an illegal input
		}
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteOpLogNonExstingEntry()
	  {
		// given
		startTestProcess();

		// an op log instance is created
		taskService.ResolveTask(task.Id);

		// when a non-existing id is used
		historyService.DeleteUserOperationLogEntry("a non existing id");

		// then no op log entry should have been deleted
		Assert.AreEqual(1, historyService.CreateUserOperationLogQuery().Count());
	  }

        [Test]
        public virtual void testOnlyTaskCompletionIsLogged()
	  {
		// given
		string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

		string taskId = taskService.CreateTaskQuery().First().Id;

		// when
		taskService.Complete(taskId);

		// then
		Assert.True((bool) runtimeService.GetVariable(ProcessInstanceId, "taskListenerCalled"));
		Assert.True((bool) runtimeService.GetVariable(ProcessInstanceId, "serviceTaskCalled"));

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

		Assert.AreEqual(1, query.Count());

		IUserOperationLogEntry log = query.First();
		Assert.AreEqual("process", log.ProcessDefinitionKey);
		Assert.AreEqual(ProcessInstanceId, log.ProcessInstanceId);
		Assert.AreEqual(DeploymentId, log.DeploymentId);
		Assert.AreEqual(taskId, log.TaskId);
		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeComplete, log.OperationType);
	  }

	  protected internal virtual void startTestProcess()
	  {
		processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="oneTaskProcess").First();

		process = runtimeService.StartProcessInstanceById(processDefinition.Id);
		task = taskService.CreateTaskQuery().First();
	  }

	  protected internal virtual IQueryable<IUserOperationLogEntry> queryOperationDetails(string type)
	  {
		return historyService.CreateUserOperationLogQuery(c=>c.OperationType ==type);
	  }

	  protected internal virtual IQueryable<IUserOperationLogEntry> queryOperationDetails(string type, string property)
	  {
		return historyService.CreateUserOperationLogQuery(c=>c.OperationType ==type)/*.Property(property)*/;
	  }

	  protected internal virtual void completeTestProcess()
	  {
		taskService.Complete(task.Id);
		AssertProcessEnded(process.Id);
	  }

	}

}