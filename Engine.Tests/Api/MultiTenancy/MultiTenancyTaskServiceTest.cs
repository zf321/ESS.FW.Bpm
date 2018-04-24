using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class MultiTenancyTaskServiceTest : PluggableProcessEngineTestCase
	{

	  private const string tenant1 = "the-tenant-1";
	  private const string tenant2 = "the-tenant-2";

	   [Test]   public virtual void testStandaloneTaskCreateWithTenantId()
	  {

		// given a transient task with tenant id
		ITask task = taskService.NewTask();
		task.TenantId = tenant1;

		// if
		// it is saved
		taskService.SaveTask(task);

		// then
		// when I load it, the tenant id is preserved
		task = taskService.CreateTaskQuery(c=>c.Id == task.Id).First();
		Assert.AreEqual(tenant1, task.TenantId);

		// Finally, Delete task
		deleteTasks(task);
	  }

	   [Test]   public virtual void testStandaloneTaskCannotChangeTenantIdIfNull()
	  {

		// given a persistent task without tenant id
		ITask task = taskService.NewTask();
		taskService.SaveTask(task);
		task = taskService.CreateTaskQuery().First();

		// if
		// change the tenant id
		task.TenantId = tenant1;

		// then
		// an exception is thrown on 'save'
		try
		{
		  taskService.SaveTask(task);
		  Assert.Fail("Expected an exception");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("ENGINE-03072 Cannot change tenantId of Task", e.Message);
		}

		// Finally, Delete task
		deleteTasks(task);
	  }

	   [Test]   public virtual void testStandaloneTaskCannotChangeTenantId()
	  {

		// given a persistent task with tenant id
		ITask task = taskService.NewTask();
		task.TenantId = tenant1;
		taskService.SaveTask(task);
		task = taskService.CreateTaskQuery().First();

		// if
		// change the tenant id
		task.TenantId = tenant2;

		// then
		// an exception is thrown on 'save'
		try
		{
		  taskService.SaveTask(task);
		  Assert.Fail("Expected an exception");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("ENGINE-03072 Cannot change tenantId of Task", e.Message);
		}

		// Finally, Delete task
		deleteTasks(task);
	  }

	   [Test]   public virtual void testStandaloneTaskCannotSetDifferentTenantIdOnSubTask()
	  {

		// given a persistent task with a tenant id
		ITask task = taskService.NewTask();
		task.TenantId = tenant1;
		taskService.SaveTask(task);

		// if
		// I create a subtask with a different tenant id
		ITask subTask = taskService.NewTask();
		subTask.ParentTaskId = task.Id;
		subTask.TenantId = tenant2;

		// then an exception is thrown on save
		try
		{
		  taskService.SaveTask(subTask);
		  Assert.Fail("Exception expected.");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("ENGINE-03073 Cannot set different tenantId on subtask than on parent Task", e.Message);
		}
		// Finally, Delete task
		deleteTasks(task);
	  }

	   [Test]   public virtual void testStandaloneTaskCannotSetDifferentTenantIdOnSubTaskWithNull()
	  {

		// given a persistent task without tenant id
		ITask task = taskService.NewTask();
		taskService.SaveTask(task);

		// if
		// I create a subtask with a different tenant id
		ITask subTask = taskService.NewTask();
		subTask.ParentTaskId = task.Id;
		subTask.TenantId = tenant1;

		// then an exception is thrown on save
		try
		{
		  taskService.SaveTask(subTask);
		  Assert.Fail("Exception expected.");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("ENGINE-03073 Cannot set different tenantId on subtask than on parent Task", e.Message);
		}
		// Finally, Delete task
		deleteTasks(task);
	  }

	   [Test]   public virtual void testStandaloneTaskPropagateTenantIdToSubTask()
	  {

		// given a persistent task with a tenant id
		ITask task = taskService.NewTask();
		task.TenantId = tenant1;
		taskService.SaveTask(task);

		// if
		// I create a subtask without tenant id
		ITask subTask = taskService.NewTask();
		subTask.ParentTaskId = task.Id;
		taskService.SaveTask(subTask);

		// then
		// the parent task's tenant id is propagated to the sub task
		subTask = taskService.CreateTaskQuery(c=>c.Id == subTask.Id).First();
		Assert.AreEqual(tenant1, subTask.TenantId);

		// Finally, Delete task
		deleteTasks(subTask, task);
	  }

	   [Test]   public virtual void testStandaloneTaskPropagatesTenantIdToVariableInstance()
	  {
		// given a task with tenant id
		ITask task = taskService.NewTask();
		task.TenantId = tenant1;
		taskService.SaveTask(task);

		// if we set a variable for the task
		taskService.SetVariable(task.Id, "var", "test");

		// then a variable instance with the same tenant id is created
		IVariableInstance variableInstance = runtimeService.CreateVariableInstanceQuery().First();
		Assert.That(variableInstance!=null);
		Assert.That(variableInstance.TenantId, Is.EqualTo(tenant1));

		deleteTasks(task);
	  }

	   [Test]   public virtual void testGetIdentityLinkWithTenantIdForCandidateUsers()
	  {

		// given
		IBpmnModelInstance oneTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().UserTask("task").CamundaCandidateUsers("aUserId").EndEvent().Done();

		DeploymentForTenant("tenant", oneTaskProcess);

		IProcessInstance tenantProcessInstance = runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId("tenant").Execute();

		ITask tenantTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==tenantProcessInstance.Id).First();

		IList<IIdentityLink> identityLinks = taskService.GetIdentityLinksForTask(tenantTask.Id);
		Assert.AreEqual(identityLinks.Count,1);
		Assert.AreEqual(identityLinks[0].TenantId, "tenant");
	  }

	   [Test]   public virtual void testGetIdentityLinkWithTenantIdForCandidateGroup()
	  {

		// given
		IBpmnModelInstance oneTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().UserTask("task").CamundaCandidateGroups("aGroupId").EndEvent().Done();

		DeploymentForTenant("tenant", oneTaskProcess);

		IProcessInstance tenantProcessInstance = runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId("tenant").Execute();

		ITask tenantTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==tenantProcessInstance.Id).First();

		IList<IIdentityLink> identityLinks = taskService.GetIdentityLinksForTask(tenantTask.Id);
		Assert.AreEqual(identityLinks.Count,1);
		Assert.AreEqual(identityLinks[0].TenantId, "tenant");
	  }

	  protected internal virtual void deleteTasks(params ITask[] tasks)
	  {
		foreach (ITask task in tasks)
		{
		  taskService.DeleteTask(task.Id, true);
		}
	  }

	}

}