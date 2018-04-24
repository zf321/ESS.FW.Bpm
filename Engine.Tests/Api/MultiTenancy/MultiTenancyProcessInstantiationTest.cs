using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{
    


    public class MultiTenancyProcessInstantiationTest : PluggableProcessEngineTestCase
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyProcessInstantiationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			batchHelper = new BatchRestartHelper(this);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal static readonly IBpmnModelInstance PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().UserTask("userTask").EndEvent().Done();

	  public BatchRestartHelper batchHelper;

    [TearDown]
	  public virtual void tearDown()
	  {
		base.TearDown();
		authorizationService.CreateAuthorizationQuery();
		batchHelper.RemoveAllRunningAndHistoricBatches();
	  }

	   [Test]   public virtual void testStartProcessInstanceByKeyAndTenantId()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);
		DeploymentForTenant(TENANT_TWO, PROCESS);

		runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		Assert.That(runtimeService.CreateProcessInstanceQuery(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartProcessInstanceByKeyForAnyTenant()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);

		runtimeService.CreateProcessInstanceByKey("testProcess").Execute();

		Assert.That(runtimeService.CreateProcessInstanceQuery(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartProcessInstanceByKeyWithoutTenantId()
	  {
		Deployment(PROCESS);
		DeploymentForTenant(TENANT_ONE, PROCESS);

		runtimeService.CreateProcessInstanceByKey("testProcess").ProcessDefinitionWithoutTenantId().Execute();

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.First().TenantId, Is.EqualTo(null));
	  }

	   [Test]   public virtual void testFailToStartProcessInstanceByKeyForOtherTenant()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no processes deployed"));
		}
	  }

	   [Test]   public virtual void testFailToStartProcessInstanceByKeyForMultipleTenants()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);
		DeploymentForTenant(TENANT_TWO, PROCESS);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("testProcess").Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("multiple tenants"));
		}
	  }

	   [Test]   public virtual void testFailToStartProcessInstanceByIdAndTenantId()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		try
		{
		  runtimeService.CreateProcessInstanceById(processDefinition.Id).SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		  Assert.Fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  Assert.That(e.Message, Does.Contain("Cannot specify a tenant-id"));
		}
	  }

	   [Test]   public virtual void testFailToStartProcessInstanceByIdWithoutTenantId()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		try
		{
		  runtimeService.CreateProcessInstanceById(processDefinition.Id).ProcessDefinitionWithoutTenantId().Execute();

		  Assert.Fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  Assert.That(e.Message, Does.Contain("Cannot specify a tenant-id"));
		}
	  }

	   [Test]   public virtual void testStartProcessInstanceAtActivityByKeyAndTenantId()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);
		DeploymentForTenant(TENANT_TWO, PROCESS);

		runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(TENANT_ONE).StartBeforeActivity("userTask").Execute();

		Assert.That(runtimeService.CreateProcessInstanceQuery(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartProcessInstanceAtActivityByKeyForAnyTenant()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);

		runtimeService.CreateProcessInstanceByKey("testProcess").StartBeforeActivity("userTask").Execute();

		Assert.That(runtimeService.CreateProcessInstanceQuery(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartProcessInstanceAtActivityByKeyWithoutTenantId()
	  {
		Deployment(PROCESS);
		DeploymentForTenant(TENANT_ONE, PROCESS);

		runtimeService.CreateProcessInstanceByKey("testProcess").ProcessDefinitionWithoutTenantId().StartBeforeActivity("userTask").Execute();

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.First().TenantId, Is.EqualTo(null));
	  }

	   [Test]   public virtual void testFailToStartProcessInstanceAtActivityByKeyForOtherTenant()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(TENANT_TWO).StartBeforeActivity("userTask").Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no processes deployed"));
		}
	  }

	   [Test]   public virtual void testFailToStartProcessInstanceAtActivityByKeyForMultipleTenants()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);
		DeploymentForTenant(TENANT_TWO, PROCESS);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("testProcess").StartBeforeActivity("userTask").Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("multiple tenants"));
		}
	  }

	   [Test]   public virtual void testFailToStartProcessInstanceAtActivityByIdAndTenantId()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		try
		{
		  runtimeService.CreateProcessInstanceById(processDefinition.Id).SetProcessDefinitionTenantId(TENANT_ONE).StartBeforeActivity("userTask").Execute();

		  Assert.Fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  Assert.That(e.Message, Does.Contain("Cannot specify a tenant-id"));
		}
	  }

	   [Test]   public virtual void testFailToStartProcessInstanceAtActivityByIdWithoutTenantId()
	  {
		Deployment(PROCESS);

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		try
		{
		  runtimeService.CreateProcessInstanceById(processDefinition.Id).ProcessDefinitionWithoutTenantId().StartBeforeActivity("userTask").Execute();

		  Assert.Fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  Assert.That(e.Message, Does.Contain("Cannot specify a tenant-id"));
		}
	  }

	   [Test]   public virtual void testStartProcessInstanceByKeyWithoutTenantIdNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		Deployment(PROCESS);

		runtimeService.CreateProcessInstanceByKey("testProcess").ProcessDefinitionWithoutTenantId().Execute();

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testFailToStartProcessInstanceByKeyNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		DeploymentForTenant(TENANT_ONE, PROCESS);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("testProcess").Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no processes deployed with key 'testProcess'"));
		}
	  }

	   [Test]   public virtual void testFailToStartProcessInstanceByKeyWithTenantIdNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		DeploymentForTenant(TENANT_ONE, PROCESS);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("Cannot create an instance of the process definition"));
		}
	  }

	   [Test]   public virtual void testFailToStartProcessInstanceByIdNoAuthenticatedTenants()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		identityService.SetAuthentication("user", null, null);

		try
		{
		  runtimeService.CreateProcessInstanceById(processDefinition.Id).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("Cannot create an instance of the process definition"));
		}
	  }

	   [Test]   public virtual void testStartProcessInstanceByKeyWithTenantIdAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		DeploymentForTenant(TENANT_ONE, PROCESS);
		DeploymentForTenant(TENANT_TWO, PROCESS);

		runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartProcessInstanceByIdAuthenticatedTenant()
	  {
		DeploymentForTenant(TENANT_ONE, PROCESS);

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		runtimeService.CreateProcessInstanceById(processDefinition.Id).Execute();

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartProcessInstanceByKeyWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		DeploymentForTenant(TENANT_ONE, PROCESS);
		DeploymentForTenant(TENANT_TWO, PROCESS);

		runtimeService.CreateProcessInstanceByKey("testProcess").Execute();

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartProcessInstanceByKeyWithTenantIdDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		DeploymentForTenant(TENANT_ONE, PROCESS);

		runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

 
	   [Test]   public virtual void testRestartProcessInstanceSyncWithTenantId()
	  {
		// given
		IProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_TWO});

		// when
		runtimeService.RestartProcessInstances(processInstance.ProcessDefinitionId).StartBeforeActivity("userTask").SetProcessInstanceIds(processInstance.Id).Execute();

		// then
		IProcessInstance restartedInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode && c.ProcessDefinitionId == processInstance.ProcessDefinitionId).First();

		Assert.NotNull(restartedInstance);
		Assert.AreEqual(restartedInstance.TenantId, TENANT_ONE);
	  }

 
	   [Test]   public virtual void testRestartProcessInstanceAsyncWithTenantId()
	  {
		// given
		IProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_TWO});

		// when
		IBatch batch = runtimeService.RestartProcessInstances(processInstance.ProcessDefinitionId).StartBeforeActivity("userTask").SetProcessInstanceIds(processInstance.Id).ExecuteAsync();

		batchHelper.CompleteBatch(batch);

		// then
		IProcessInstance restartedInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode&& c.ProcessDefinitionId == processInstance.ProcessDefinitionId).First();

		Assert.NotNull(restartedInstance);
		Assert.AreEqual(restartedInstance.TenantId, TENANT_ONE);
	  }

 
	   [Test]   public virtual void testFailToRestartProcessInstanceSyncWithOtherTenantId()
	  {
		// given
		IProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);

		identityService.SetAuthentication("user", null, new List<string>(){TENANT_TWO});

		try
		{
		  // when
		  runtimeService.RestartProcessInstances(processInstance.ProcessDefinitionId).StartBeforeActivity("userTask").SetProcessInstanceIds(processInstance.Id).Execute();

		  Assert.Fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  Assert.That(e.Message, Does.Contain("Historic process instance cannot be found: historicProcessInstanceId is null"));
		}
	  }

 
	   [Test]   public virtual void testFailToRestartProcessInstanceAsyncWithOtherTenantId()
	  {
		// given
		IProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);

		identityService.SetAuthentication("user", null, new List<string>(){TENANT_TWO});

		try
		{
		  // when
		  runtimeService.RestartProcessInstances(processInstance.ProcessDefinitionId).StartBeforeActivity("userTask").SetProcessInstanceIds(processInstance.Id).ExecuteAsync();
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("Cannot restart process instances of process definition '" + processInstance.ProcessDefinitionId + "' because it belongs to no authenticated tenant."));
		}

	  }

 
	   [Test]   public virtual void testRestartProcessInstanceSyncWithTenantIdByHistoricProcessInstanceQuery()
	  {
		// given
		IProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);
		IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionId ==processInstance.ProcessDefinitionId);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_TWO});

		// when
		runtimeService.RestartProcessInstances(processInstance.ProcessDefinitionId).StartBeforeActivity("userTask").SetHistoricProcessInstanceQuery(query).Execute();

		// then
		IProcessInstance restartedInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode&& c.ProcessDefinitionId == processInstance.ProcessDefinitionId).First();

		Assert.NotNull(restartedInstance);
		Assert.AreEqual(restartedInstance.TenantId, TENANT_ONE);
	  }

 
	   [Test]   public virtual void testRestartProcessInstanceAsyncWithTenantIdByHistoricProcessInstanceQuery()
	  {
		// given
		IProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);
		IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionId ==processInstance.ProcessDefinitionId);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_TWO});

		// when
		IBatch batch = runtimeService.RestartProcessInstances(processInstance.ProcessDefinitionId).StartBeforeActivity("userTask").SetHistoricProcessInstanceQuery(query).ExecuteAsync();

		batchHelper.CompleteBatch(batch);

		// then
		IProcessInstance restartedInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode&& c.ProcessDefinitionId == processInstance.ProcessDefinitionId).First();

		Assert.NotNull(restartedInstance);
		Assert.AreEqual(restartedInstance.TenantId, TENANT_ONE);
	  }
         
 
	   [Test]   public virtual void testFailToRestartProcessInstanceSyncWithOtherTenantIdByHistoricProcessInstanceQuery()
	  {
		// given
		IProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);
		IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionId ==processInstance.ProcessDefinitionId);

		identityService.SetAuthentication("user", null, new List<string>(){TENANT_TWO});

		try
		{
		  // when
		  runtimeService.RestartProcessInstances(processInstance.ProcessDefinitionId).StartBeforeActivity("userTask").SetHistoricProcessInstanceQuery(query).Execute();

		  Assert.Fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  Assert.That(e.Message, Does.Contain("processInstanceIds is empty"));
		}
	  }

 
	   [Test]   public virtual void testFailToRestartProcessInstanceAsyncWithOtherTenantIdByHistoricProcessInstanceQuery()
	  {
		// given
		IProcessInstance processInstance = startAndDeleteProcessInstance(TENANT_ONE, PROCESS);
		IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionId ==processInstance.ProcessDefinitionId);

		identityService.SetAuthentication("user", null, new List<string>(){TENANT_TWO});

		try
		{
		  // when
		  runtimeService.RestartProcessInstances(processInstance.ProcessDefinitionId).StartBeforeActivity("userTask").SetHistoricProcessInstanceQuery(query).ExecuteAsync();
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("processInstanceIds is empty"));
		}

	  }


	  public virtual IProcessInstance startAndDeleteProcessInstance(string tenantId, IBpmnModelInstance modelInstance)
	  {
		string deploymentId = DeploymentForTenant(TENANT_ONE, PROCESS);
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == deploymentId).First();
		IProcessInstance processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
		runtimeService.DeleteProcessInstance(processInstance.Id, "test");

		return processInstance;
	  }

	}

}