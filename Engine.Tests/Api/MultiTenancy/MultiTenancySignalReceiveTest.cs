using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{
    

	public class MultiTenancySignalReceiveTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancySignalReceiveTest()
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

	  protected internal static readonly IBpmnModelInstance SIGNAL_INTERMEDIATE_THROW_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("signalThrow").StartEvent().IntermediateThrowEvent().Signal("signal").EndEvent().Done();

	  protected internal static readonly IBpmnModelInstance SIGNAL_END_THROW_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("signalThrow").StartEvent().EndEvent().Signal("signal").Done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;


        [Test]
        public virtual void sendSignalToStartEventForNonTenant()
	  {
		testRule.Deploy(SIGNAL_START_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);

		engineRule.RuntimeService.CreateSignalEvent("signal" ).SetTenantId( null).Send();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.First().TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void sendSignalToStartEventForTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_START_PROCESS);

		engineRule.RuntimeService.CreateSignalEvent("signal").SetTenantId(TENANT_ONE).Send();
		engineRule.RuntimeService.CreateSignalEvent("signal").SetTenantId(TENANT_TWO).Send();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void sendSignalToStartEventWithoutTenantIdForNonTenant()
	  {
		testRule.Deploy(SIGNAL_START_PROCESS);

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void sendSignalToStartEventWithoutTenantIdForTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void sendSignalToIntermediateCatchEventForNonTenant()
	  {
		testRule.Deploy(SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").ProcessDefinitionWithoutTenantId().Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		engineRule.RuntimeService.CreateSignalEvent("signal").SetTenantId(null).Send();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.First().TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void sendSignalToIntermediateCatchEventForTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.RuntimeService.CreateSignalEvent("signal").SetTenantId(TENANT_ONE).Send();
		engineRule.RuntimeService.CreateSignalEvent("signal").SetTenantId(TENANT_TWO).Send();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void sendSignalToIntermediateCatchEventWithoutTenantIdForNonTenant()
	  {
		testRule.Deploy(SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").Execute();

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void sendSignalToIntermediateCatchEventWithoutTenantIdForTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").Execute();

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void sendSignalToStartAndIntermediateCatchEventForNonTenant()
	  {
		testRule.Deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").ProcessDefinitionWithoutTenantId().Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		engineRule.RuntimeService.CreateSignalEvent("signal").SetTenantId(null).Send();

	      IList<ITask> tasks = engineRule.TaskService.CreateTaskQuery()
	          .ToList();
		Assert.That(tasks.Count, Is.EqualTo(2));
		Assert.That(tasks[0].TenantId, Is.EqualTo(null));
		Assert.That(tasks[1].TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void sendSignalToStartAndIntermediateCatchEventForTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.RuntimeService.CreateSignalEvent("signal").SetTenantId(TENANT_ONE).Send();
		engineRule.RuntimeService.CreateSignalEvent("signal").SetTenantId(TENANT_TWO).Send();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(2L));
	  }


        [Test]
        public virtual void sendSignalToStartEventsForMultipleTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_START_PROCESS);

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void sendSignalToIntermediateCatchEventsForMultipleTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void sendSignalToStartAndIntermediateCatchEventForMultipleTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_START_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").Execute();

		engineRule.RuntimeService.CreateSignalEvent("signal").Send();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void failToSendSignalWithExecutionIdForTenant()
	  {

		//thrown.Expect(typeof(BadUserRequestException));
		//thrown.ExpectMessage("Cannot specify a tenant-id when deliver a signal to a single execution.");

		engineRule.RuntimeService.CreateSignalEvent("signal").SetExecutionId("id").SetTenantId(TENANT_ONE).Send();
	  }


        [Test]
        public virtual void throwIntermediateSignalForTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS, SIGNAL_INTERMEDIATE_THROW_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.Deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").ProcessDefinitionWithoutTenantId().Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.RuntimeService.StartProcessInstanceByKey("signalThrow");

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query
           // .WithoutTenantId()
            .Where(o=>string.IsNullOrEmpty(o.TenantId))
            .Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void throwIntermediateSignalForNonTenant()
	  {
		testRule.Deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS, SIGNAL_INTERMEDIATE_THROW_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").ProcessDefinitionWithoutTenantId().Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		engineRule.RuntimeService.StartProcessInstanceByKey("signalThrow");

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query
            .Where(o => string.IsNullOrEmpty(o.TenantId))
            .Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void throwEndSignalForTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS, SIGNAL_END_THROW_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.Deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").ProcessDefinitionWithoutTenantId().Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.RuntimeService.StartProcessInstanceByKey("signalThrow");

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query
            .Where(o => string.IsNullOrEmpty(o.TenantId))
            .Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void throwEndSignalForNonTenant()
	  {
		testRule.Deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS, SIGNAL_END_THROW_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").ProcessDefinitionWithoutTenantId().Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("signalCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		engineRule.RuntimeService.StartProcessInstanceByKey("signalThrow");

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query
            .Where(o => string.IsNullOrEmpty(o.TenantId))
            .Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
	  }
	}

}