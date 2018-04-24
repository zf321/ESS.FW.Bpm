using System.Collections.Generic;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class MultiTenancyMigrationPlanCreateTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMigrationPlanCreateTenantCheckTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			////ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";



	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;


        [Test]
        public virtual void canCreateMigrationPlanForDefinitionsOfAuthenticatedTenant()
	  {
		// given
		IProcessDefinition tenant1Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition tenant2Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);


		// when
		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});
		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(tenant1Definition.Id, tenant2Definition.Id).MapEqualActivities().Build();

		// then
		Assert.NotNull(migrationPlan);
	  }


        [Test]
        public virtual void cannotCreateMigrationPlanForDefinitionsOfNonAuthenticatedTenantsCase1()
	  {
		// given
		IProcessDefinition tenant1Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition tenant2Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		engineRule.IdentityService.SetAuthentication("user", null, new List<string>(){ TENANT_TWO });

		// then
		//exception.Expect(typeof(ProcessEngineException));
		//exception.ExpectMessage("Cannot get process definition '" + tenant1Definition.Id + "' because it belongs to no authenticated tenant");

		// when
		engineRule.RuntimeService.CreateMigrationPlan(tenant1Definition.Id, tenant2Definition.Id).MapEqualActivities().Build();
	  }


        [Test]
        public virtual void cannotCreateMigrationPlanForDefinitionsOfNonAuthenticatedTenantsCase2()
	  {
		// given
		IProcessDefinition tenant1Definition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition tenant2Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		engineRule.IdentityService.SetAuthentication("user", null, new List<string>() { TENANT_TWO });

		// then
		//exception.Expect(typeof(ProcessEngineException));
		//exception.ExpectMessage("Cannot get process definition '" + tenant2Definition.Id + "' because it belongs to no authenticated tenant");

		// when
		engineRule.RuntimeService.CreateMigrationPlan(tenant1Definition.Id, tenant2Definition.Id).MapEqualActivities().Build();
	  }


        [Test]
        public virtual void cannotCreateMigrationPlanForDefinitionsOfNonAuthenticatedTenantsCase3()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		// then
		//exception.Expect(typeof(ProcessEngineException));
		//exception.ExpectMessage("Cannot get process definition '" + sourceDefinition.Id + "' because it belongs to no authenticated tenant");

		// when
		engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();
	  }



        [Test]
        public virtual void canCreateMigrationPlanForSharedDefinitionsWithNoAuthenticatedTenants()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

		// when
		engineRule.IdentityService.SetAuthentication("user", null, null);
		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		// then
		Assert.NotNull(migrationPlan);
	  }



        [Test]
        public virtual void canCreateMigrationPlanWithDisabledTenantCheck()
	  {

		// given
		IProcessDefinition tenant1Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition tenant2Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);

		// when
		engineRule.IdentityService.SetAuthentication("user", null, null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(tenant1Definition.Id, tenant2Definition.Id).MapEqualActivities().Build();

		// then
		Assert.NotNull(migrationPlan);

	  }
	}

}