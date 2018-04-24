using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class MultiTenancyMigrationExecuteTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMigrationExecuteTenantCheckTest()
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
        public virtual void canMigrateWithAuthenticatedTenant()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);

		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

		// when
		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});
		engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).Execute();

		// then
		AssertMigratedTo(processInstance, targetDefinition);

	  }


        [Test]
        public virtual void cannotMigrateOfNonAuthenticatedTenant()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);

		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){ TENANT_TWO });

		// then
		//exception.Expect(typeof(ProcessEngineException));
		//exception.ExpectMessage("Cannot migrate process instance '" + processInstance.Id + "' because it belongs to no authenticated tenant");

		// when
		engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).Execute();
	  }


        [Test]
        public virtual void cannotMigrateWithNoAuthenticatedTenant()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);

		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
		engineRule.IdentityService.SetAuthentication("user", null, null);

		// then
		//exception.Expect(typeof(ProcessEngineException));
		//exception.ExpectMessage("Cannot migrate process instance '" + processInstance.Id + "' because it belongs to no authenticated tenant");

		// when
		engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).Execute();
	  }


        [Test]
        public virtual void canMigrateSharedInstanceWithNoTenant()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

		// when
		engineRule.IdentityService.SetAuthentication("user", null, null);
		engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).Execute();

		// then
		AssertMigratedTo(processInstance, targetDefinition);

	  }


        [Test]
        public virtual void canMigrateInstanceWithTenantCheckDisabled()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);

		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

		// when
		engineRule.IdentityService.SetAuthentication("user", null, null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).Execute();

		// then
		AssertMigratedTo(processInstance, targetDefinition);

	  }

	  protected internal virtual void AssertMigratedTo(IProcessInstance processInstance, IProcessDefinition targetDefinition)
	  {
		Assert.AreEqual(1, engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id&& c.ProcessDefinitionId==targetDefinition.Id).Count());
	  }
	}

}