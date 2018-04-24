using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    

	/// <summary>
	/// 
	/// </summary>
	public class MultiTenancySignalReceiveCmdTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancySignalReceiveCmdTenantCheckTest()
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

	  protected internal static readonly IBpmnModelInstance SIGNAL_START_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("signalStart").StartEvent().Signal("signal").UserTask().EndEvent().Done();

	  protected internal static readonly IBpmnModelInstance SIGNAL_CATCH_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("signalCatch").StartEvent().IntermediateCatchEvent().Signal("signal").UserTask().EndEvent().Done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;


        [Test]
        public virtual void sendSignalToStartEventNoAuthenticatedTenants()
	  {
		testRule.Deploy(SIGNAL_START_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		engineRule.IdentityService.ClearAuthentication();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(o=>string.IsNullOrEmpty(o.TenantId)).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void sendSignalToStartEventWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_START_PROCESS);

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		engineRule.IdentityService.ClearAuthentication();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void sendSignalToStartEventDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_START_PROCESS);

		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void sendSignalToIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.Deploy(SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").ProcessDefinitionWithoutTenantId().Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(o=>string.IsNullOrEmpty(o.ParentTaskId)).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void sendSignalToIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void sendSignalToIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void sendSignalToStartAndIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.Deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").ProcessDefinitionWithoutTenantId().Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(o=>string.IsNullOrEmpty(o.TenantId)).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void sendSignalToStartAndIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void sendSignalToStartAndIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(4L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(2L));
	  }


        [Test]
        public virtual void sendSignalToIntermediateCatchEventWithExecutionIdAndAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		IExecution execution = engineRule.RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == "signalCatch").First();//.SignalEventSubscriptionName("signal").First();

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.RuntimeService.CreateSignalEvent("signal").SetExecutionId(execution.Id).Send();

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void failToSendSignalToIntermediateCatchEventWithExecutionIdAndNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		IExecution execution = engineRule.RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == "signalCatch").First();//.SignalEventSubscriptionName("signal").First();

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance");

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.CreateSignalEvent("signal").SetExecutionId(execution.Id).Send();
	  }


        [Test]
        public virtual void signalIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.Deploy(SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").Execute();

		IExecution execution = engineRule.RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == "signalCatch").First();//.SignalEventSubscriptionName("signal").First();

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.Signal(execution.Id, "signal", null, null);

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(o=>string.IsNullOrEmpty(o.TenantId)).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void signalIntermediateCatchEventWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		IExecution execution = engineRule.RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == "signalCatch").First();//.SignalEventSubscriptionName("signal").First();

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.RuntimeService.Signal(execution.Id, "signal", null, null);

		engineRule.IdentityService.ClearAuthentication();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void signalIntermediateCatchEventDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		IExecution execution = engineRule.RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == "signalCatch")
                //.SignalEventSubscriptionName("signal").Where(c=>c.TenantId == TENANT_ONE)
                .First();

		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.Signal(execution.Id, "signal", null, null);

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void failToSignalIntermediateCatchEventNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").Execute();

		IExecution execution = engineRule.RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == "signalCatch").First();//.SignalEventSubscriptionName("signal").First();

		// declared expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance");

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RuntimeService.Signal(execution.Id, "signal", null, null);
	  }
	}

}