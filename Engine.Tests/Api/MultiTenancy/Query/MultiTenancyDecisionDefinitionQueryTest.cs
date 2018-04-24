using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query
{
    

	public class MultiTenancyDecisionDefinitionQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string DECISION_DEFINITION_KEY = "decision";
	  protected internal const string DMN = "resources/api/multitenancy/simpleDecisionTable.Dmn";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal  void setUp()
	  {
		Deployment(DMN);
		DeploymentForTenant(TENANT_ONE, DMN);
		DeploymentForTenant(TENANT_TWO, DMN);
	  }

	   [Test]   public virtual void testQueryNoTenantIdSet()
	  {
		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByTenantId()
	  {
		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = repositoryService.CreateDecisionDefinitionQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByDefinitionsWithoutTenantId()
	  {
		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery(c=>c.TenantId == null);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery(c=>c.TenantId == TENANT_ONE)/*.IncludeDecisionDefinitionsWithoutTenantId()*/;

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = repositoryService.CreateDecisionDefinitionQuery(c=>c.TenantId == TENANT_TWO)/*.IncludeDecisionDefinitionsWithoutTenantId()*/;

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = repositoryService.CreateDecisionDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.IncludeDecisionDefinitionsWithoutTenantId()*/;

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByKey()
	  {
		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DECISION_DEFINITION_KEY);
		// one definition for each tenant
		Assert.That(query.Count(), Is.EqualTo(3L));

		query = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key == DECISION_DEFINITION_KEY && c.TenantId == null);
		// one definition without tenant id
		Assert.That(query.Count(), Is.EqualTo(1L));

		query = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key == DECISION_DEFINITION_KEY).Where(c=>c.TenantId == TENANT_ONE);
		// one definition for tenant one
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByLatestNoTenantIdSet()
	  {
		// deploy a second version for tenant one
		DeploymentForTenant(TENANT_ONE, DMN);

		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key == DECISION_DEFINITION_KEY)/*/*.LatestVersion()*/;
		// one definition for each tenant
		Assert.That(query.Count(), Is.EqualTo(3L));

		IDictionary<string, IDecisionDefinition> decisionDefinitionsForTenant = getDecisionDefinitionsForTenant(query.ToList());
		Assert.That(decisionDefinitionsForTenant[TENANT_ONE].Version, Is.EqualTo(2));
		Assert.That(decisionDefinitionsForTenant[TENANT_TWO].Version, Is.EqualTo(1));
		Assert.That(decisionDefinitionsForTenant[null].Version, Is.EqualTo(1));
	  }

	   [Test]   public virtual void testQueryByLatestWithTenantId()
	  {
		// deploy a second version for tenant one
		DeploymentForTenant(TENANT_ONE, DMN);

		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key == DECISION_DEFINITION_KEY)/*.LatestVersion(c=>c.TenantId == TENANT_ONE)*/;

		Assert.That(query.Count(), Is.EqualTo(1L));

		IDecisionDefinition decisionDefinition = query.First();
		Assert.That(decisionDefinition.TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(decisionDefinition.Version, Is.EqualTo(2));

		query = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key == DECISION_DEFINITION_KEY)/*.LatestVersion(c=>c.TenantId == TENANT_TWO)*/;

		Assert.That(query.Count(), Is.EqualTo(1L));

		decisionDefinition = query.First();
		Assert.That(decisionDefinition.TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(decisionDefinition.Version, Is.EqualTo(1));
	  }

	   [Test]   public virtual void testQueryByLatestWithTenantIds()
	  {
		// deploy a second version for tenant one
		DeploymentForTenant(TENANT_ONE, DMN);

		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key == DECISION_DEFINITION_KEY)/*.LatestVersion(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))*//*.OrderByTenantId()*//*.Asc()*/;
		// one definition for each tenant
		Assert.That(query.Count(), Is.EqualTo(2L));

		IDictionary<string, IDecisionDefinition> decisionDefinitionsForTenant = getDecisionDefinitionsForTenant(query.ToList());
		Assert.That(decisionDefinitionsForTenant[TENANT_ONE].Version, Is.EqualTo(2));
		Assert.That(decisionDefinitionsForTenant[TENANT_TWO].Version, Is.EqualTo(1));
	  }

	   [Test]   public virtual void testQueryByLatestWithoutTenantId()
	  {
		// deploy a second version without tenant id
		Deployment(DMN);

		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key == DECISION_DEFINITION_KEY)/*.LatestVersion(c=>c.TenantId == null)*/;

		Assert.That(query.Count(), Is.EqualTo(1L));

		IDecisionDefinition decisionDefinition = query.First();
		Assert.That(decisionDefinition.TenantId, Is.EqualTo(null));
		Assert.That(decisionDefinition.Version, Is.EqualTo(2));
	  }

	   [Test]   public virtual void testQueryByLatestWithTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		// deploy a second version without tenant id
		Deployment(DMN);
		// deploy a third version for tenant one
		DeploymentForTenant(TENANT_ONE, DMN);
		DeploymentForTenant(TENANT_ONE, DMN);

		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DECISION_DEFINITION_KEY)/*.LatestVersion(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))*//*.IncludeDecisionDefinitionsWithoutTenantId()*/;

		Assert.That(query.Count(), Is.EqualTo(3L));

		IDictionary<string, IDecisionDefinition> decisionDefinitionsForTenant = getDecisionDefinitionsForTenant(query.ToList());
		Assert.That(decisionDefinitionsForTenant[TENANT_ONE].Version, Is.EqualTo(3));
		Assert.That(decisionDefinitionsForTenant[TENANT_TWO].Version, Is.EqualTo(1));
		Assert.That(decisionDefinitionsForTenant[null].Version, Is.EqualTo(2));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  repositoryService.CreateDecisionDefinitionQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<IDecisionDefinition> decisionDefinitions = repositoryService.CreateDecisionDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Asc()*/.ToList();

		Assert.That(decisionDefinitions.Count, Is.EqualTo(2));
		Assert.That(decisionDefinitions[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(decisionDefinitions[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<IDecisionDefinition> decisionDefinitions = repositoryService.CreateDecisionDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(decisionDefinitions.Count, Is.EqualTo(2));
		Assert.That(decisionDefinitions[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(decisionDefinitions[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		//Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId))/*.IncludeDecisionDefinitionsWithoutTenantId()*/.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
		//Assert.That(query.WithoutTenantId().Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();
		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	  protected internal virtual IDictionary<string, IDecisionDefinition> getDecisionDefinitionsForTenant(IList<IDecisionDefinition> decisionDefinitions)
	  {
		IDictionary<string, IDecisionDefinition> definitionsForTenant = new Dictionary<string, IDecisionDefinition>();

		foreach (IDecisionDefinition definition in decisionDefinitions)
		{
		  definitionsForTenant[definition.TenantId] = definition;
		}
		return definitionsForTenant;
	  }

	}

}