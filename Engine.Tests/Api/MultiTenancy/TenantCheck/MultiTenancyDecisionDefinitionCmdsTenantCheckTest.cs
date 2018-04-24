using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Dmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    

	/// <summary>
	/// 
	/// </summary>
	public class MultiTenancyDecisionDefinitionCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyDecisionDefinitionCmdsTenantCheckTest()
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

	  protected internal const string DMN_MODEL = "resources/api/multitenancy/simpleDecisionTable.Dmn";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;



	  protected internal IRepositoryService repositoryService;
	  protected internal IIdentityService identityService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal string decisionDefinitionId;

    [SetUp]
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;

		testRule.DeployForTenant(TENANT_ONE, DMN_MODEL);

		decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery().First().Id;

	  }

        [Test]
        public virtual void failToGetDecisionModelNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the decision definition");

		repositoryService.GetDecisionModel(decisionDefinitionId);
	  }

        [Test]
        public virtual void getDecisionModelWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		System.IO.Stream inputStream = repositoryService.GetDecisionModel(decisionDefinitionId);

		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void getDecisionModelDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		System.IO.Stream inputStream = repositoryService.GetDecisionModel(decisionDefinitionId);

		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void failToGetDecisionDiagramNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the decision definition");

		repositoryService.GetDecisionDiagram(decisionDefinitionId);
	  }

        [Test]
        public virtual void getDecisionDiagramWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		System.IO.Stream inputStream = repositoryService.GetDecisionDiagram(decisionDefinitionId);

		// inputStream is always null because there is no decision diagram at the moment
		// what should be deployed as a diagram resource for DMN? 
		Assert.That(inputStream, null);
	  }

        [Test]
        public virtual void getDecisionDiagramDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		System.IO.Stream inputStream = repositoryService.GetDecisionDiagram(decisionDefinitionId);

		// inputStream is always null because there is no decision diagram at the moment
		// what should be deployed as a diagram resource for DMN? 
		Assert.That(inputStream, null);
	  }

        [Test]
        public virtual void failToGetDecisionDefinitionNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the decision definition");

		repositoryService.GetDecisionDefinition(decisionDefinitionId);
	  }

        [Test]
        public virtual void getDecisionDefinitionWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IDecisionDefinition definition = repositoryService.GetDecisionDefinition(decisionDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
	  }

        [Test]
        public virtual void getDecisionDefinitionDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IDecisionDefinition definition = repositoryService.GetDecisionDefinition(decisionDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
	  }

        [Test]
        public virtual void failToGetDmnModelInstanceNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the decision definition");

		repositoryService.GetDmnModelInstance(decisionDefinitionId);
	  }

        [Test]
        public virtual void updateHistoryTimeToLiveWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		repositoryService.UpdateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, 6);

		IDecisionDefinition definition = repositoryService.GetDecisionDefinition(decisionDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
		//Assert.That(definition.HistoryTimeToLive, Is.EqualTo(6));
	  }

        [Test]
        public virtual void updateHistoryTimeToLiveDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		repositoryService.UpdateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, 6);

		IDecisionDefinition definition = repositoryService.GetDecisionDefinition(decisionDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
		//Assert.That(definition.HistoryTimeToLive, Is.EqualTo(6));
	  }

        [Test]
        public virtual void updateHistoryTimeToLiveNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the decision definition");

		repositoryService.UpdateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, 6);
	  }

        [Test]
        public virtual void getDmnModelInstanceWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IDmnModelInstance modelInstance = repositoryService.GetDmnModelInstance(decisionDefinitionId);

		Assert.That(modelInstance!=null);
	  }

        [Test]
        public virtual void getDmnModelInstanceDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IDmnModelInstance modelInstance = repositoryService.GetDmnModelInstance(decisionDefinitionId);

		Assert.That(modelInstance!=null);
	  }

	}

}