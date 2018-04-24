using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    

	public class MultiTenancyMessageCorrelationCmdTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMessageCorrelationCmdTenantCheckTest()
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
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal static readonly IBpmnModelInstance MESSAGE_START_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("messageStart").StartEvent().Message("message").UserTask().EndEvent().Done();

	  protected internal static readonly IBpmnModelInstance MESSAGE_CATCH_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("messageCatch").StartEvent().IntermediateCatchEvent().Message("message").UserTask().EndEvent().Done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;


        [Test]
        public virtual void correlateMessageToStartEventNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);
		testRule.Deploy(MESSAGE_START_PROCESS);

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateMessageCorrelation("message").CorrelateStartMessage();

		engineRule.IdentityService.ClearAuthentication();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		//Assert.That(query.WithoutTenantId().Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void correlateMessageToStartEventWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.RuntimeService.CreateMessageCorrelation("message").CorrelateStartMessage();

		engineRule.IdentityService.ClearAuthentication();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateMessageToStartEventDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateMessageCorrelation("message").SetTenantId(TENANT_ONE).CorrelateStartMessage();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateMessageToIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);
		testRule.Deploy(MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").ProcessDefinitionWithoutTenantId().Execute();

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateMessageCorrelation("message").Correlate();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		//Assert.That(query.WithoutTenantId().Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateMessageToIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.RuntimeService.CreateMessageCorrelation("message").Correlate();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateMessageToIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateMessageCorrelation("message").SetTenantId(TENANT_ONE).Correlate();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateMessageToStartAndIntermediateCatchEventWithNoAuthenticatedTenants()
	  {
		testRule.Deploy(MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").ProcessDefinitionWithoutTenantId().Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateMessageCorrelation("message").CorrelateAll();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
		//Assert.That(query.WithoutTenantId().Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateMessageToStartAndIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.RuntimeService.CreateMessageCorrelation("message").CorrelateAll();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateMessageToStartAndIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateMessageCorrelation("message").CorrelateAll();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(4L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(2L));
	  }


        [Test]
        public virtual void failToCorrelateMessageByProcessInstanceIdNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		//IProcessInstance processInstance = engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		// declared expected exception
		//thrown.Expect(typeof(MismatchingMessageCorrelationException));
		//thrown.ExpectMessage("Cannot correlate message");

		engineRule.IdentityService.SetAuthentication("user", null, null);

		//engineRule.RuntimeService.CreateMessageCorrelation("message").Where(c=>c.ProcessInstanceId==processInstance.Id).Correlate();
	  }


        [Test]
        public virtual void correlateMessageByProcessInstanceIdWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		//IProcessInstance processInstance = engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").Execute();

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		//engineRule.RuntimeService.CreateMessageCorrelation("message").Where(c=>c.ProcessInstanceId==processInstance.Id).Correlate();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void failToCorrelateMessageByProcessDefinitionIdNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);

		IProcessDefinition processDefinition = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key == "messageStart").Where(c=>c.TenantId == TENANT_ONE).First();

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot create an instance of the process definition");

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateMessageCorrelation("message").ProcessDefinitionId(processDefinition.Id).CorrelateStartMessage();
	  }


        [Test]
        public virtual void correlateMessageByProcessDefinitionIdWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);

		IProcessDefinition processDefinition = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key == "messageStart").First();

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.RuntimeService.CreateMessageCorrelation("message").ProcessDefinitionId(processDefinition.Id).CorrelateStartMessage();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	}

}