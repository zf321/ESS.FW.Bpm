using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    
	public class MultiTenancyMessageEventReceivedCmdTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMessageEventReceivedCmdTenantCheckTest()
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
			////ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal static readonly IBpmnModelInstance MESSAGE_CATCH_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("messageCatch").StartEvent().IntermediateCatchEvent().Message("message").UserTask().EndEvent().Done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;


        [Test]
        public virtual void correlateReceivedMessageToIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.Deploy(MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").Execute();

		IExecution execution = engineRule.RuntimeService.CreateExecutionQuery()/*.ProcessDefinitionKey("messageCatch")*//*.MessageEventSubscriptionName("messageName")*/.First();

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.MessageEventReceived("message", execution.Id);

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == null)/*.WithoutTenantId()*/.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateReceivedMessageToIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").Execute();

		IExecution execution = engineRule.RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == "messageCatch")/*.MessageEventSubscriptionName("messageName")*/.First();

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.RuntimeService.MessageEventReceived("message", execution.Id);

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void correlateReceivedMessageToIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		IExecution execution = engineRule.RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == "messageCatch" && c.TenantId == TENANT_ONE)/*.MessageEventSubscriptionName("messageName")*/.First();

		engineRule.RuntimeService.MessageEventReceived("message", execution.Id);

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void failToCorrelateReceivedMessageToIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		IExecution execution = engineRule.RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == "messageCatch" && c.TenantId == TENANT_ONE)/*.MessageEventSubscriptionName("message")*/.First();

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance");

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.MessageEventReceived("message", execution.Id);
	  }

	}

}