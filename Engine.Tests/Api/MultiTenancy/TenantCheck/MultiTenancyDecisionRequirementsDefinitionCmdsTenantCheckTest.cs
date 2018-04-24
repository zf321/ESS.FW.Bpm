using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{



	/// 
	/// <summary>
	/// 
	/// 
	/// </summary>

	public class MultiTenancyDecisionRequirementsDefinitionCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyDecisionRequirementsDefinitionCmdsTenantCheckTest()
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

	  protected internal const string DRG_DMN = "resources/api/multitenancy/DecisionRequirementsGraph.Dmn";

	  protected internal const string DRD_DMN = "resources/api/multitenancy/DecisionRequirementsGraph.png";

	  protected internal ProcessEngineRule engineRule = new ProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal string decisionRequirementsDefinitionId;



	  protected internal IRepositoryService repositoryService;
	  protected internal IIdentityService identityService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

    [SetUp]
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;

		testRule.DeployForTenant(TENANT_ONE, DRG_DMN, DRD_DMN);
		decisionRequirementsDefinitionId = repositoryService.CreateDecisionRequirementsDefinitionQuery().First().Id;
	  }

        [Test]
        public virtual void failToGetDecisionRequirementsDefinitionNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the decision requirements definition");

		repositoryService.GetDecisionRequirementsDefinition(decisionRequirementsDefinitionId);
	  }

        [Test]
        public virtual void getDecisionRequirementsDefinitionWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IDecisionRequirementsDefinition definition = repositoryService.GetDecisionRequirementsDefinition(decisionRequirementsDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
	  }

        [Test]
        public virtual void getDecisionRequirementsDefinitionDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IDecisionRequirementsDefinition definition = repositoryService.GetDecisionRequirementsDefinition(decisionRequirementsDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
	  }

        [Test]
        public virtual void failToGetDecisionRequirementsModelNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the decision requirements definition");

		repositoryService.GetDecisionRequirementsModel(decisionRequirementsDefinitionId);
	  }

        [Test]
        public virtual void getDecisionRequirementsModelWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		System.IO.Stream inputStream = repositoryService.GetDecisionRequirementsModel(decisionRequirementsDefinitionId);

		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void getDecisionRequirementsModelDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		System.IO.Stream inputStream = repositoryService.GetDecisionRequirementsModel(decisionRequirementsDefinitionId);

		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void failToGetDecisionRequirementsDiagramNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the decision requirements definition");

		repositoryService.GetDecisionRequirementsDiagram(decisionRequirementsDefinitionId);
	  }

        [Test]
        public virtual void getDecisionDiagramWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		System.IO.Stream inputStream = repositoryService.GetDecisionRequirementsDiagram(decisionRequirementsDefinitionId);

		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void getDecisionDiagramDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		System.IO.Stream inputStream = repositoryService.GetDecisionRequirementsDiagram(decisionRequirementsDefinitionId);

		Assert.That(inputStream!=null);
	  }

	}

}