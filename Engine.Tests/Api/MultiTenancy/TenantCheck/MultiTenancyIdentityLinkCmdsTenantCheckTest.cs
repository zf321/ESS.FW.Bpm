using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    

	public class MultiTenancyIdentityLinkCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyIdentityLinkCmdsTenantCheckTest()
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
	  public virtual void setAssigneeForTaskWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		taskService.SetAssignee(task.Id, "demo");

		// then
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "demo").Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void setAssigneeForTaskWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.SetAssignee(task.Id, "demo");

	  }


        [Test]
        public virtual void setAssigneeForTaskWithDisabledTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		taskService.SetAssignee(task.Id, "demo");
		// then
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "demo").Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void setOwnerForTaskWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		taskService.SetOwner(task.Id, "demo");

		// then
		Assert.That(taskService.CreateTaskQuery()
            //.TaskOwner("demo")
            .Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void setOwnerForTaskWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		taskService.SetOwner(task.Id, "demo");

	  }


        [Test]
        public virtual void setOwnerForTaskWithDisabledTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		taskService.SetOwner(task.Id, "demo");
		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskOwner("demo")
            .Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void getIdentityLinkWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		taskService.SetOwner(task.Id, "demo");

		Assert.That(taskService.GetIdentityLinksForTask(task.Id).First().Type, Is.EqualTo("owner"));
	  }


        [Test]
        public virtual void getIdentityLinkWitNoAuthenticatedTenant()
	  {

		taskService.SetOwner(task.Id, "demo");
		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.GetIdentityLinksForTask(task.Id);

	  }


        [Test]
        public virtual void getIdentityLinkWithDisabledTenantCheck()
	  {

		taskService.SetOwner(task.Id, "demo");
		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		Assert.That(taskService.GetIdentityLinksForTask(task.Id).First().Type, Is.EqualTo("owner"));

	  }


        [Test]
        public virtual void addCandidateUserWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		taskService.AddCandidateUser(task.Id, "demo");

		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateUser("demo")
            .Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void addCandidateUserWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.AddCandidateUser(task.Id, "demo");

	  }


        [Test]
        public virtual void addCandidateUserWithDisabledTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		taskService.AddCandidateUser(task.Id, "demo");

		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateUser("demo")
            .Count(), Is.EqualTo(1L));
	  }


        [Test]
        // add candidate group
        public virtual void addCandidateGroupWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		taskService.AddCandidateGroup(task.Id, "demo");

		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateGroup("demo")
            .Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void addCandidateGroupWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		taskService.AddCandidateGroup(task.Id, "demo");

	  }


        [Test]
        public virtual void addCandidateGroupWithDisabledTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		taskService.AddCandidateGroup(task.Id, "demo");

		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateGroup("demo")
            .Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void deleteCandidateUserWithAuthenticatedTenant()
	  {

		taskService.AddCandidateUser(task.Id, "demo");
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateUser("demo")
            .Count(), Is.EqualTo(1L));

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		taskService.DeleteCandidateUser(task.Id, "demo");
		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateUser("demo")
            .Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void deleteCandidateUserWithNoAuthenticatedTenant()
	  {

		taskService.AddCandidateUser(task.Id, "demo");
		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		taskService.DeleteCandidateUser(task.Id, "demo");

	  }


        [Test]
        public virtual void deleteCandidateUserWithDisabledTenantCheck()
	  {

		taskService.AddCandidateUser(task.Id, "demo");
		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		taskService.DeleteCandidateUser(task.Id, "demo");

		// then
		Assert.That(taskService.CreateTaskQuery().Count(), Is.EqualTo(0L));//.Count(), Is.EqualTo(0L));//.TaskCandidateUser("demo").Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void deleteCandidateGroupWithAuthenticatedTenant()
	  {

		taskService.AddCandidateGroup(task.Id, "demo");
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateGroup("demo")
            .Count(), Is.EqualTo(1L));

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		taskService.DeleteCandidateGroup(task.Id, "demo");
		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateGroup("demo")
            .Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void deleteCandidateGroupWithNoAuthenticatedTenant()
	  {

		taskService.AddCandidateGroup(task.Id, "demo");
		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.DeleteCandidateGroup(task.Id, "demo");

	  }


        [Test]
        public virtual void deleteCandidateGroupWithDisabledTenantCheck()
	  {

		taskService.AddCandidateGroup(task.Id, "demo");
		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		taskService.DeleteCandidateGroup(task.Id, "demo");

		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateGroup("demo")
            .Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void addUserIdentityLinkWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		taskService.AddUserIdentityLink(task.Id, "demo", IdentityLinkType.Candidate);

		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateUser("demo")
            .Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void addUserIdentityLinkWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		taskService.AddUserIdentityLink(task.Id, "demo", IdentityLinkType.Candidate);

	  }


        [Test]
        public virtual void addUserIdentityLinkWithDisabledTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		taskService.AddUserIdentityLink(task.Id, "demo", IdentityLinkType.Assignee);

		// then
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "demo").Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void addGroupIdentityLinkWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		taskService.AddGroupIdentityLink(task.Id, "demo", IdentityLinkType.Candidate);

		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateGroup("demo")
            .Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void addGroupIdentityLinkWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.AddGroupIdentityLink(task.Id, "demo", IdentityLinkType.Candidate);

	  }


        [Test]
        public virtual void addGroupIdentityLinkWithDisabledTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		taskService.AddGroupIdentityLink(task.Id, "demo", IdentityLinkType.Candidate);

		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateGroup("demo")
            .Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void deleteUserIdentityLinkWithAuthenticatedTenant()
	  {

		taskService.AddUserIdentityLink(task.Id, "demo", IdentityLinkType.Assignee);
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "demo").Count(), Is.EqualTo(1L));

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		taskService.DeleteUserIdentityLink(task.Id, "demo", IdentityLinkType.Assignee);
		// then
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "demo").Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void deleteUserIdentityLinkWithNoAuthenticatedTenant()
	  {

		taskService.AddUserIdentityLink(task.Id, "demo", IdentityLinkType.Assignee);
		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.DeleteUserIdentityLink(task.Id, "demo", IdentityLinkType.Assignee);

	  }


        [Test]
        public virtual void deleteUserIdentityLinkWithDisabledTenantCheck()
	  {

		taskService.AddUserIdentityLink(task.Id, "demo", IdentityLinkType.Assignee);
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "demo").Count(), Is.EqualTo(1L));

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		taskService.DeleteUserIdentityLink(task.Id, "demo", IdentityLinkType.Assignee);

		// then
		Assert.That(taskService.CreateTaskQuery(c=>c.Assignee == "demo").Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void deleteGroupIdentityLinkWithAuthenticatedTenant()
	  {

		taskService.AddGroupIdentityLink(task.Id, "demo", IdentityLinkType.Candidate);
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateGroup("demo")
            .Count(), Is.EqualTo(1L));

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		taskService.DeleteGroupIdentityLink(task.Id, "demo", IdentityLinkType.Candidate);
		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateGroup("demo")
            .Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void deleteGroupIdentityLinkWithNoAuthenticatedTenant()
	  {

		taskService.AddGroupIdentityLink(task.Id, "demo", IdentityLinkType.Candidate);
		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		taskService.DeleteGroupIdentityLink(task.Id, "demo", IdentityLinkType.Candidate);

	  }


        [Test]
        public virtual void deleteGroupIdentityLinkWithDisabledTenantCheck()
	  {

		taskService.AddGroupIdentityLink(task.Id, "demo", IdentityLinkType.Candidate);
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateGroup("demo")
            .Count(), Is.EqualTo(1L));

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		taskService.DeleteGroupIdentityLink(task.Id, "demo", IdentityLinkType.Candidate);

		// then
		Assert.That(taskService.CreateTaskQuery()//.TaskCandidateGroup("demo")
            .Count(), Is.EqualTo(0L));
	  }
	}

}