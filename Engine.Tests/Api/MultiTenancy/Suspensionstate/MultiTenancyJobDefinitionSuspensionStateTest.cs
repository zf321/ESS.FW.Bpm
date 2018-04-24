using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.Suspensionstate
{
    

	public class MultiTenancyJobDefinitionSuspensionStateTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyJobDefinitionSuspensionStateTest()
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


	  public virtual void suspendAndActivateJobDefinitionsForAllTenants()
	  {
		// given activated job definitions
		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		// first suspend
		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

		// then activate
		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
	  }


	  public virtual void suspendJobDefinitionForTenant()
	  {
		// given activated job definitions
		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(TENANT_ONE).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


	  public virtual void suspendJobDefinitionForNonTenant()
	  {
		// given activated job definitions
		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).GetProcessDefinitionWithoutTenantId().Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }


	  public virtual void activateJobDefinitionForTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(TENANT_ONE).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


	  public virtual void jobProcessDefinitionForNonTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).GetProcessDefinitionWithoutTenantId().Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }


	  public virtual void suspendAndActivateJobDefinitionsIncludingJobsForAllTenants()
	  {
		// given activated job definitions
		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		// first suspend
		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).SetIncludeJobs(true).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

		// then activate
		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).SetIncludeJobs(true).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
	  }


	  public virtual void suspendJobDefinitionIncludingJobsForTenant()
	  {
		// given activated job definitions
		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(TENANT_ONE).SetIncludeJobs(true).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


	  public virtual void suspendJobDefinitionIncludingJobsForNonTenant()
	  {
		// given activated job definitions
		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).GetProcessDefinitionWithoutTenantId().SetIncludeJobs(true).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }


	  public virtual void activateJobDefinitionIncludingJobsForTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).SetIncludeJobs(true).Suspend();

		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(TENANT_ONE).SetIncludeJobs(true).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


	  public virtual void activateJobDefinitionIncludingJobsForNonTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).SetIncludeJobs(true).Suspend();

		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).GetProcessDefinitionWithoutTenantId().SetIncludeJobs(true).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }


	  public virtual void delayedSuspendJobDefinitionsForAllTenants()
	  {
		// given activated job definitions

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ExecutionDate(tomorrow()).Suspend();

		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		// when execute the job to suspend the job definitions
		IJob job = engineRule.ManagementService.CreateJobQuery()/*/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
	  }


	  public virtual void delayedSuspendJobDefinitionsForTenant()
	  {
		// given activated job definitions

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(TENANT_ONE).ExecutionDate(tomorrow()).Suspend();

		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		// when execute the job to suspend the job definitions
		IJob job = engineRule.ManagementService.CreateJobQuery()/*/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


	  public virtual void delayedSuspendJobDefinitionsForNonTenant()
	  {
		// given activated job definitions

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).GetProcessDefinitionWithoutTenantId().ExecutionDate(tomorrow()).Suspend();

		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		// when execute the job to suspend the job definitions
		IJob job = engineRule.ManagementService.CreateJobQuery()/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }


	  public virtual void delayedActivateJobDefinitionsForAllTenants()
	  {
		// given suspend job definitions
		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ExecutionDate(tomorrow()).Activate();

		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

		// when execute the job to activate the job definitions
		IJob job = engineRule.ManagementService.CreateJobQuery()/*/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
	  }


	  public virtual void delayedActivateJobDefinitionsForTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(TENANT_ONE).ExecutionDate(tomorrow()).Activate();

		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

		// when execute the job to activate the job definitions
		IJob job = engineRule.ManagementService.CreateJobQuery()/*/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


	  public virtual void delayedActivateJobDefinitionsForNonTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).GetProcessDefinitionWithoutTenantId().ExecutionDate(tomorrow()).Activate();

		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

		// when execute the job to activate the job definitions
		IJob job = engineRule.ManagementService.CreateJobQuery()/*/*.Timers()*/.First();
		Assert.That(job!=null);

		engineRule.ManagementService.ExecuteJob(job.Id);

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }


	  public virtual void suspendJobDefinitionNoAuthenticatedTenants()
	  {
		// given activated job definitions
		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.IdentityService.ClearAuthentication();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }


	  public virtual void suspendJobDefinitionWithAuthenticatedTenant()
	  {
		// given activated job definitions
		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.IdentityService.ClearAuthentication();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


	  public virtual void suspendJobDefinitionDisabledTenantCheck()
	  {
		// given activated job definitions
		IQueryable<IJobDefinition> query = engineRule.ManagementService.CreateJobDefinitionQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.ManagementService.UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.IncludeJobDefinitionsWithoutTenantId()*/.Count(), Is.EqualTo(3L));
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
		  private readonly MultiTenancyJobDefinitionSuspensionStateTest outerInstance;

		  public CommandAnonymousInnerClass(MultiTenancyJobDefinitionSuspensionStateTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public virtual object Execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByHandlerType(TimerActivateJobDefinitionHandler.TYPE);
			commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByHandlerType(TimerSuspendJobDefinitionHandler.TYPE);
			return null;
		  }
	  }

	}

}