using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.Suspensionstate
{
    
	public class MultiTenancyJobSuspensionStateTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyJobSuspensionStateTest()
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

	  protected internal static readonly IBpmnModelInstance PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().CamundaAsyncBefore().EndEvent().Done();

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

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendAndActivateJobsForAllTenants()
	  public virtual void suspendAndActivateJobsForAllTenants()
	  {
		// given activated jobs
		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		// first suspend
		engineRule.ManagementService.UpdateJobSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

		// then activate
		engineRule.ManagementService.UpdateJobSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobForTenant()
	  public virtual void suspendJobForTenant()
	  {
		// given activated jobs
		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		//Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobsForNonTenant()
	  public virtual void suspendJobsForNonTenant()
	  {
		// given activated jobs
		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		//Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateJobsForTenant()
	  public virtual void activateJobsForTenant()
	  {
		// given suspend jobs
		engineRule.ManagementService.UpdateJobSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateJobsForNonTenant()
	  public virtual void activateJobsForNonTenant()
	  {
		// given suspend jobs
		engineRule.ManagementService.UpdateJobSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ManagementService.UpdateJobSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Activate();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobNoAuthenticatedTenants()
	  public virtual void suspendJobNoAuthenticatedTenants()
	  {
		// given activated jobs
		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.ManagementService.UpdateJobSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.IdentityService.ClearAuthentication();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode && c.TenantId == null).Count(), Is.EqualTo(1L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobWithAuthenticatedTenant()
	  public virtual void suspendJobWithAuthenticatedTenant()
	  {
		// given activated jobs
		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		engineRule.ManagementService.UpdateJobSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		engineRule.IdentityService.ClearAuthentication();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobDisabledTenantCheck()
	  public virtual void suspendJobDisabledTenantCheck()
	  {
		// given activated jobs
		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		engineRule.ManagementService.UpdateJobSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
		//Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId)).IncludeJobsWithoutTenantId().Count(), Is.EqualTo(3L));
	  }

	}

}