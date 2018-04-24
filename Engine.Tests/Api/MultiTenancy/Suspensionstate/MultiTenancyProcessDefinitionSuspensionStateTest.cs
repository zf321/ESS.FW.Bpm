using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.Suspensionstate
{

	public class MultiTenancyProcessDefinitionSuspensionStateTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyProcessDefinitionSuspensionStateTest()
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

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

	  protected internal static readonly IBpmnModelInstance PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().CamundaAsyncBefore().EndEvent().Done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

        [SetUp]
	  public virtual void setUp()
	  {

		testRule.DeployForTenant(TENANT_ONE, PROCESS);
		testRule.DeployForTenant(TENANT_TWO, PROCESS);
		testRule.Deploy(PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(TENANT_TWO).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Execute();
	  }

        [Test]
        public virtual void suspendAndActivateProcessDefinitionsForAllTenants()
	  {
		// given activated process definitions
		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		// first suspend
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

		// then activate
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void suspendProcessDefinitionForTenant()
	  {
		// given activated process definitions
		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void suspendProcessDefinitionForNonTenant()
	  {
		// given activated process definitions
		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void activateProcessDefinitionForTenant()
	  {
		// given suspend process definitions
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void activateProcessDefinitionForNonTenant()
	  {
		// given suspend process definitions
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void suspendAndActivateProcessDefinitionsIncludeInstancesForAllTenants()
	  {
		// given activated process instances
		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		// first suspend
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).IncludeProcessInstances(true).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

		// then activate
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).IncludeProcessInstances(true).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void suspendProcessDefinitionIncludeInstancesForTenant()
	  {
		// given activated process instances
		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).IncludeProcessInstances(true).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void suspendProcessDefinitionIncludeInstancesForNonTenant()
	  {
		// given activated process instances
		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().IncludeProcessInstances(true).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void activateProcessDefinitionIncludeInstancesForTenant()
	  {
		// given suspended process instances
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).IncludeProcessInstances(true).Suspend();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).IncludeProcessInstances(true).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void activateProcessDefinitionIncludeInstancesForNonTenant()
	  {
		// given suspended process instances
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).IncludeProcessInstances(true).Suspend();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().IncludeProcessInstances(true).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void delayedSuspendProcessDefinitionsForAllTenants()
	  {
		// given activated process definitions

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ExecutionDate(tomorrow()).Suspend();

		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		// when execute the job to suspend the process definitions
		IJob job = engineRule.ManagementService.CreateJobQuery()/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
	  }

        [Test]
        public virtual void delayedSuspendProcessDefinitionsForTenant()
	  {
		// given activated process definitions

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).ExecutionDate(tomorrow()).Suspend();

		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		// when execute the job to suspend the process definition
		IJob job = engineRule.ManagementService.CreateJobQuery()/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void delayedSuspendProcessDefinitionsForNonTenant()
	  {
		// given activated process definitions

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().ExecutionDate(tomorrow()).Suspend();

		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		// when execute the job to suspend the process definition
		IJob job = engineRule.ManagementService.CreateJobQuery()/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void delayedActivateProcessDefinitionsForAllTenants()
	  {
		// given suspended process definitions
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ExecutionDate(tomorrow()).Activate();

		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		// when execute the job to activate the process definitions
		IJob job = engineRule.ManagementService.CreateJobQuery()/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
	  }

        [Test]
        public virtual void delayedActivateProcessDefinitionsForTenant()
	  {
		// given suspended process definitions
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).ExecutionDate(tomorrow()).Activate();

		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		// when execute the job to activate the process definition
		IJob job = engineRule.ManagementService.CreateJobQuery()/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void delayedActivateProcessDefinitionsForNonTenant()
	  {
		// given suspended process definitions
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().ExecutionDate(tomorrow()).Activate();

		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		// when execute the job to activate the process definition
		IJob job = engineRule.ManagementService.CreateJobQuery()/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void suspendProcessDefinitionIncludingJobDefinitionsForAllTenants()
	  {
		// given activated jobs
		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
	  }

        [Test]
        public virtual void suspendProcessDefinitionIncludingJobDefinitionsForTenant()
	  {
		// given activated jobs
		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void suspendProcessDefinitionIncludingJobDefinitionsForNonTenant()
	  {
		// given activated jobs
		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void activateProcessDefinitionIncludingJobDefinitionsForAllTenants()
	  {
		// given suspended jobs
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
	  }
        [Test]
        public virtual void activateProcessDefinitionIncludingJobDefinitionsForTenant()
	  {
		// given suspended jobs
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }
        [Test]
        public virtual void activateProcessDefinitionIncludingJobDefinitionsForNonTenant()
	  {
		// given suspended jobs
		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void suspendProcessDefinitionNoAuthenticatedTenants()
	  {
		// given activated process definitions
		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.IdentityService.ClearAuthentication();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void failToSuspendProcessDefinitionByIdNoAuthenticatedTenants()
	  {
		IProcessDefinition processDefinition = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY).Where(c=>c.TenantId == TENANT_ONE).First();

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process definition '" + processDefinition.Id + "' because it belongs to no authenticated tenant");

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinition.Id).Suspend();
	  }

        [Test]
        public virtual void suspendProcessDefinitionWithAuthenticatedTenant()
	  {
		// given activated process definitions
		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.IdentityService.ClearAuthentication();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void suspendProcessDefinitionDisabledTenantCheck()
	  {
		// given activated process definitions
		IQueryable<IProcessDefinition> query = engineRule.RepositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		ProcessEngineConfigurationImpl processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		processEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))
            //.IncludeProcessDefinitionsWithoutTenantId()
            .Count(), Is.EqualTo(3L));
	  }

	  protected internal virtual DateTime tomorrow()
	  {
		DateTime calendar = new DateTime();
		calendar.AddDays(1);
		return calendar;
	  }

        [TearDown]
	  public virtual void tearDown()
	  {
		ICommandExecutor commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.Execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : ICommand<object>
	  {
		  private readonly MultiTenancyProcessDefinitionSuspensionStateTest outerInstance;

		  public CommandAnonymousInnerClass(MultiTenancyProcessDefinitionSuspensionStateTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public virtual object Execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByHandlerType(TimerActivateProcessDefinitionHandler.TYPE);
			commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
			return null;
		  }
	  }

	}

}