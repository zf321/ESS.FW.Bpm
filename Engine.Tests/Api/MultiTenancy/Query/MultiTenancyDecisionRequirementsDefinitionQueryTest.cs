using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.Query
{
    
	public class MultiTenancyDecisionRequirementsDefinitionQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyDecisionRequirementsDefinitionQueryTest()
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


	  protected internal const string DECISION_REQUIREMENTS_DEFINITION_KEY = "score";
	  protected internal const string DMN = "resources/dmn/deployment/drdScore.Dmn11.xml";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;



	  protected internal IRepositoryService repositoryService;
	  protected internal IIdentityService identityService;

    [SetUp]
	  public virtual void setUp()
	  {
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;

		testRule.Deploy(DMN);
		testRule.DeployForTenant(TENANT_ONE, DMN);
		testRule.DeployForTenant(TENANT_TWO, DMN);
	  }


	  public virtual void queryNoTenantIdSet()
	  {
		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }


	  public virtual void queryByTenantId()
	  {
		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }


	  public virtual void queryByTenantIds()
	  {
		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }


	  public virtual void queryByDefinitionsWithoutTenantId()
	  {
		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.TenantId == null);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }


	  public virtual void queryByTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.TenantId == TENANT_ONE);//.IncludeDecisionRequirementsDefinitionsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.TenantId == TENANT_TWO);//.IncludeDecisionRequirementsDefinitionsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));//.IncludeDecisionRequirementsDefinitionsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }


	  public virtual void queryByKey()
	  {
		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key==DECISION_REQUIREMENTS_DEFINITION_KEY);
		// one definition for each tenant
		Assert.That(query.Count(), Is.EqualTo(3L));

		query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key==DECISION_REQUIREMENTS_DEFINITION_KEY && c.TenantId == null);
		// one definition without tenant id
		Assert.That(query.Count(), Is.EqualTo(1L));

		query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key==DECISION_REQUIREMENTS_DEFINITION_KEY).Where(c=>c.TenantId == TENANT_ONE);
		// one definition for tenant one
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }


	  public virtual void queryByLatestNoTenantIdSet()
	  {
		// deploy a second version for tenant one
		testRule.DeployForTenant(TENANT_ONE, DMN);

		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key==DECISION_REQUIREMENTS_DEFINITION_KEY)/*.LatestVersion()*/;
		// one definition for each tenant
		Assert.That(query.Count(), Is.EqualTo(3L));

		IDictionary<string, IDecisionRequirementsDefinition> definitionsForTenant = getDecisionRequirementsDefinitionsForTenant(query.ToList());
		Assert.That(definitionsForTenant[TENANT_ONE].Version, Is.EqualTo(2));
		Assert.That(definitionsForTenant[TENANT_TWO].Version, Is.EqualTo(1));
		Assert.That(definitionsForTenant[null].Version, Is.EqualTo(1));
	  }


	  public virtual void queryByLatestWithTenantId()
	  {
		// deploy a second version for tenant one
		testRule.DeployForTenant(TENANT_ONE, DMN);

		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key==DECISION_REQUIREMENTS_DEFINITION_KEY);//.LatestVersion(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		IDecisionRequirementsDefinition IDecisionRequirementsDefinition = query.First();
		Assert.That(IDecisionRequirementsDefinition.TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(IDecisionRequirementsDefinition.Version, Is.EqualTo(2));

		query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key==DECISION_REQUIREMENTS_DEFINITION_KEY);//.LatestVersion(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));

		IDecisionRequirementsDefinition = query.First();
		Assert.That(IDecisionRequirementsDefinition.TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(IDecisionRequirementsDefinition.Version, Is.EqualTo(1));
	  }


	  public virtual void queryByLatestWithTenantIds()
	  {
		// deploy a second version for tenant one
		testRule.DeployForTenant(TENANT_ONE, DMN);

	      IQueryable<IDecisionRequirementsDefinition> query =
	              repositoryService.CreateDecisionRequirementsDefinitionQuery(
	                  c => c.Key == DECISION_REQUIREMENTS_DEFINITION_KEY)
	          ;//.LatestVersion(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Asc()*/;
		// one definition for each tenant
		Assert.That(query.Count(), Is.EqualTo(2L));

		IDictionary<string, IDecisionRequirementsDefinition> definitionsForTenant = getDecisionRequirementsDefinitionsForTenant(query.ToList());
		Assert.That(definitionsForTenant[TENANT_ONE].Version, Is.EqualTo(2));
		Assert.That(definitionsForTenant[TENANT_TWO].Version, Is.EqualTo(1));
	  }


	  public virtual void queryByLatestWithoutTenantId()
	  {
		// deploy a second version without tenant id
		testRule.Deploy(DMN);

		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key==DECISION_REQUIREMENTS_DEFINITION_KEY)/*.LatestVersion(c=>c.TenantId == null)*/;

		Assert.That(query.Count(), Is.EqualTo(1L));

		IDecisionRequirementsDefinition IDecisionRequirementsDefinition = query.First();
		Assert.That(IDecisionRequirementsDefinition.TenantId, Is.EqualTo(null));
		Assert.That(IDecisionRequirementsDefinition.Version, Is.EqualTo(2));
	  }


	  public virtual void queryByLatestWithTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		// deploy a second version without tenant id
		testRule.Deploy(DMN);
		// deploy a third version for tenant one
		testRule.DeployForTenant(TENANT_ONE, DMN);
		testRule.DeployForTenant(TENANT_ONE, DMN);

		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key==DECISION_REQUIREMENTS_DEFINITION_KEY);//.LatestVersion(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId)).IncludeDecisionRequirementsDefinitionsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(3L));

		IDictionary<string, IDecisionRequirementsDefinition> definitionsForTenant = getDecisionRequirementsDefinitionsForTenant(query.ToList());
		Assert.That(definitionsForTenant[TENANT_ONE].Version, Is.EqualTo(3));
		Assert.That(definitionsForTenant[TENANT_TWO].Version, Is.EqualTo(1));
		Assert.That(definitionsForTenant[null].Version, Is.EqualTo(2));
	  }


	  public virtual void queryByNonExistingTenantId()
	  {
		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }


	  public virtual void failQueryByTenantIdNull()
	  {

		//thrown.Expect(typeof(NullValueException));

		repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.TenantId == (string) null);
	  }


	  public virtual void querySortingAsc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<IDecisionRequirementsDefinition> DecisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Asc()*/.ToList();

		Assert.That(DecisionRequirementsDefinitions.Count, Is.EqualTo(2));
		Assert.That(DecisionRequirementsDefinitions[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(DecisionRequirementsDefinitions[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }


	  public virtual void querySortingDesc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<IDecisionRequirementsDefinition> DecisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(DecisionRequirementsDefinitions.Count, Is.EqualTo(2));
		Assert.That(DecisionRequirementsDefinitions[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(DecisionRequirementsDefinitions[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }


	  public virtual void queryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }


	  public virtual void queryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId))//.IncludeDecisionRequirementsDefinitionsWithoutTenantId()
            .Count(), Is.EqualTo(2L));
	  }


	  public virtual void queryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(o => string.IsNullOrEmpty(o.TenantId)).Count(), Is.EqualTo(1L));
	  }

	  public virtual void queryDisabledTenantCheck()
	  {
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();
		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	  protected internal virtual IDictionary<string, IDecisionRequirementsDefinition> getDecisionRequirementsDefinitionsForTenant(IList<IDecisionRequirementsDefinition> definitions)
	  {
		IDictionary<string, IDecisionRequirementsDefinition> definitionsForTenant = new Dictionary<string, IDecisionRequirementsDefinition>();

		foreach (IDecisionRequirementsDefinition definition in definitions)
		{
		  definitionsForTenant[definition.TenantId] = definition;
		}
		return definitionsForTenant;
	  }

	}

}