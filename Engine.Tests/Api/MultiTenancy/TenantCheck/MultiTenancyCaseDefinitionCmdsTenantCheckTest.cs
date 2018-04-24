using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Cmmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    
	/// <summary>
	/// 
	/// </summary>
	public class MultiTenancyCaseDefinitionCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyCaseDefinitionCmdsTenantCheckTest()
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

	  protected internal const string CMMN_MODEL = "resources/api/cmmn/emptyStageCase.cmmn";
	  protected internal const string CMMN_DIAGRAM = "resources/api/cmmn/emptyStageCase.png";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;



	  protected internal IRepositoryService repositoryService;
	  protected internal IIdentityService identityService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal string caseDefinitionId;

        [SetUp]
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;

		testRule.DeployForTenant(TENANT_ONE, CMMN_MODEL, CMMN_DIAGRAM);

		caseDefinitionId = repositoryService.CreateCaseDefinitionQuery().First().Id;
	  }

        [Test]
        public virtual void failToGetCaseModelNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the case definition");

		repositoryService.GetCaseModel(caseDefinitionId);
	  }

        [Test]
        public virtual void getCaseModelWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		System.IO.Stream inputStream = repositoryService.GetCaseModel(caseDefinitionId);

		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void getCaseModelDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		System.IO.Stream inputStream = repositoryService.GetCaseModel(caseDefinitionId);

		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void failToGetCaseDiagramNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the case definition");

		repositoryService.GetCaseDiagram(caseDefinitionId);
	  }

        [Test]
        public virtual void getCaseDiagramWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		System.IO.Stream inputStream = repositoryService.GetCaseDiagram(caseDefinitionId);

		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void getCaseDiagramDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		System.IO.Stream inputStream = repositoryService.GetCaseDiagram(caseDefinitionId);

		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void failToGetCaseDefinitionNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the case definition");

		repositoryService.GetCaseDefinition(caseDefinitionId);
	  }

        [Test]
        public virtual void getCaseDefinitionWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		ICaseDefinition definition = repositoryService.GetCaseDefinition(caseDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
	  }

        [Test]
        public virtual void getCaseDefinitionDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		ICaseDefinition definition = repositoryService.GetCaseDefinition(caseDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
	  }

        [Test]
        public virtual void failToGetCmmnModelInstanceNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the case definition");

		repositoryService.GetCmmnModelInstance(caseDefinitionId);
	  }

        [Test]
        public virtual void getCmmnModelInstanceWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		ICmmnModelInstance modelInstance = repositoryService.GetCmmnModelInstance(caseDefinitionId);

		Assert.That(modelInstance!=null);
	  }

        [Test]
        public virtual void getCmmnModelInstanceDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

            ICmmnModelInstance modelInstance = repositoryService.GetCmmnModelInstance(caseDefinitionId);

		Assert.That(modelInstance!=null);
	  }

        [Test]
        public virtual void updateHistoryTimeToLiveWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		repositoryService.UpdateCaseDefinitionHistoryTimeToLive(caseDefinitionId, 6);

		ICaseDefinition definition = repositoryService.GetCaseDefinition(caseDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
		//Assert.That(definition.HistoryTimeToLive, Is.EqualTo(6));
	  }

        [Test]
        public virtual void updateHistoryTimeToLiveDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		repositoryService.UpdateCaseDefinitionHistoryTimeToLive(caseDefinitionId, 6);

		ICaseDefinition definition = repositoryService.GetCaseDefinition(caseDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
		//Assert.That(definition.HistoryTimeToLive, Is.EqualTo(6));
	  }

        [Test]
        public virtual void updateHistoryTimeToLiveNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the case definition");

		repositoryService.UpdateCaseDefinitionHistoryTimeToLive(caseDefinitionId, 6);
	  }

	}

}