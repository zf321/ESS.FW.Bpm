using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    
	/// 
	/// <summary>
	/// 
	/// 
	/// </summary>

	public class MultiTenancyTaskServiceCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyTaskServiceCmdsTenantCheckTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			//ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string PROCESS_DEFINITION_KEY = "oneTaskProcess";

	  protected internal static readonly IBpmnModelInstance ONE_TASK_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().EndEvent().Done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal ITaskService taskService;
	  protected internal IIdentityService identityService;

	  protected internal ITask task;

        [SetUp]
	  public virtual void init()
	  {

		testRule.DeployForTenant(TENANT_ONE, ONE_TASK_PROCESS);

		//engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		task = engineRule.TaskService.CreateTaskQuery().First();

		taskService = engineRule.TaskService;
		identityService = engineRule.IdentityService;
	  }


        [Test]
        public virtual void saveTaskWithAuthenticatedTenant()
	  {

		task = taskService.NewTask("NewTask");
		task.TenantId = TENANT_ONE;

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		taskService.SaveTask(task);
		// then
		Assert.That(taskService.CreateTaskQuery(c=>c.Id == task.Id).Count(), Is.EqualTo(1L));

		taskService.DeleteTask(task.Id, true);
	  }


        [Test]
        public virtual void saveTaskWithNoAuthenticatedTenant()
	  {

		task = taskService.NewTask("NewTask");
		task.TenantId = TENANT_ONE;

		identityService.SetAuthentication("aUserId", null);

		// then
		////thrown.Expect(typeof(ProcessEngineException));
		////thrown.ExpectMessage("Cannot create the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.SaveTask(task);

	  }


        [Test]
        public virtual void saveTaskWithDisabledTenantCheck()
	  {

		task = taskService.NewTask("NewTask");
		task.TenantId = TENANT_ONE;

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		taskService.SaveTask(task);
		// then
		Assert.That(taskService.CreateTaskQuery(c=>c.Id == task.Id).Count(), Is.EqualTo(1L));
		taskService.DeleteTask(task.Id, true);
	  }


        [Test]
        public virtual void updateTaskWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		task.Assignee = "aUser";
		taskService.SaveTask(task);

		// then
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "aUser").Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void updateTaskWithNoAuthenticatedTenant()
	  {

		task.Assignee = "aUser";
		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.SaveTask(task);

	  }


        [Test]
        public virtual void updateTaskWithDisabledTenantCheck()
	  {

		task.Assignee = "aUser";
		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		taskService.SaveTask(task);
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "aUser").Count(), Is.EqualTo(1L));

	  }


        [Test]
        public virtual void claimTaskWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		taskService.Claim(task.Id, "bUser");
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "bUser").Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void claimTaskWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot work on task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.Claim(task.Id, "bUser");

	  }


        [Test]
        public virtual void claimTaskWithDisableTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		taskService.Claim(task.Id, "bUser");
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "bUser").Count(), Is.EqualTo(1L));

	  }


        [Test]
        public virtual void completeTaskWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		taskService.Complete(task.Id);
		Assert.That(taskService.CreateTaskQuery(c=>c.Id == task.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void completeTaskWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot work on task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.Complete(task.Id);
	  }


        [Test]
        public virtual void completeWithDisabledTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		taskService.Complete(task.Id);
		Assert.That(taskService.CreateTaskQuery(c=>c.Id == task.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void delegateTaskWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		taskService.DelegateTask(task.Id, "demo");

		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "demo").Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void delegateTaskWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.DelegateTask(task.Id, "demo");

	  }


        [Test]
        public virtual void delegateTaskWithDisabledTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		taskService.DelegateTask(task.Id, "demo");
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "demo").Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void resolveTaskWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		taskService.ResolveTask(task.Id);

		Assert.That(taskService.CreateTaskQuery()//.TaskDelegationState(DelegationState.Resolved).TaskId(task.Id)
            .Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void resolveTaskWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot work on task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.ResolveTask(task.Id);

	  }


        [Test]
        public virtual void resolveTaskWithDisableTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		taskService.ResolveTask(task.Id);
		Assert.That(taskService.CreateTaskQuery()//.TaskDelegationState(DelegationState.Resolved).TaskId(task.Id)
            .Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void setPriorityForTaskWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		taskService.SetPriority(task.Id, 1);

		Assert.That(taskService.CreateTaskQuery(c=>c.Priority==1 && c.Id==task.Id).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void setPriorityForTaskWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.SetPriority(task.Id, 1);
	  }


        [Test]
        public virtual void setPriorityForTaskWithDisabledTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		taskService.SetPriority(task.Id, 1);
		Assert.That(taskService.CreateTaskQuery(c=>c.Priority==1 && c.Id==task.Id).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void deleteTaskWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		task = createTaskforTenant();
		Assert.That(taskService.CreateTaskQuery(c=>c.Id == task.Id).Count(), Is.EqualTo(1L));

		// then
		taskService.DeleteTask(task.Id, true);
		Assert.That(taskService.CreateTaskQuery(c=>c.Id == task.Id).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void deleteTaskWithNoAuthenticatedTenant()
	  {

		try
		{
		  task = createTaskforTenant();
		  identityService.SetAuthentication("aUserId", null);
		  // then
		  //thrown.Expect(typeof(ProcessEngineException));
		  //thrown.ExpectMessage("Cannot Delete the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		  taskService.DeleteTask(task.Id, true);
		}
		finally
		{
		  identityService.ClearAuthentication();
		  taskService.DeleteTask(task.Id, true);
		}
	  }


        [Test]
        public virtual void deleteTaskWithDisabledTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		task = createTaskforTenant();
		Assert.That(taskService.CreateTaskQuery(c=>c.Id == task.Id).Count(), Is.EqualTo(1L));

		// then
		taskService.DeleteTask(task.Id, true);
		Assert.That(taskService.CreateTaskQuery(c=>c.Id == task.Id).Count(), Is.EqualTo(0L));
	  }

	  protected internal virtual ITask createTaskforTenant()
	  {
		ITask NewTask = taskService.NewTask("NewTask");
		NewTask.TenantId = TENANT_ONE;
		taskService.SaveTask(NewTask);

		return NewTask;

	  }
	}

}