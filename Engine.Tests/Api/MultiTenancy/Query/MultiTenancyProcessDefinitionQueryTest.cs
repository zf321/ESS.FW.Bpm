using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query
{
    

	public class MultiTenancyProcessDefinitionQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PROCESS_DEFINITION_KEY = "process";
	  protected internal static readonly IBpmnModelInstance emptyProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).Done();

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal void setUp()
	  {
		Deployment(emptyProcess);
		DeploymentForTenant(TENANT_ONE, emptyProcess);
		DeploymentForTenant(TENANT_TWO, emptyProcess);
	  }

	   [Test]   public virtual void testQueryNoTenantIdSet()
	  {
		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByTenantId()
	  {
		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByDefinitionsWithoutTenantId()
	  {
		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == null);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_ONE)
                ;//.IncludeProcessDefinitionsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_TWO)
                ;//.IncludeProcessDefinitionsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = repositoryService.CreateProcessDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))
                ;//.IncludeProcessDefinitionsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByKey()
	  {
		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY);
		// one definition for each tenant
		Assert.That(query.Count(), Is.EqualTo(3L));

		query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY && c.TenantId == null);
		// one definition without tenant id
		Assert.That(query.Count(), Is.EqualTo(1L));

		query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY).Where(c=>c.TenantId == TENANT_ONE);
		// one definition for tenant one
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByLatestNoTenantIdSet()
	  {
		// deploy a second version for tenant one
		DeploymentForTenant(TENANT_ONE, emptyProcess);

		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY)/*.LatestVersion()*/;
		// one definition for each tenant
		Assert.That(query.Count(), Is.EqualTo(3L));

		IDictionary<string, IProcessDefinition> processDefinitionsForTenant = getProcessDefinitionsForTenant(query.ToList());
		Assert.That(processDefinitionsForTenant[TENANT_ONE].Version, Is.EqualTo(2));
		Assert.That(processDefinitionsForTenant[TENANT_TWO].Version, Is.EqualTo(1));
		Assert.That(processDefinitionsForTenant[null].Version, Is.EqualTo(1));
	  }

	   [Test]   public virtual void testQueryByLatestWithTenantId()
	  {
		// deploy a second version for tenant one
		DeploymentForTenant(TENANT_ONE, emptyProcess);

		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY)
                ;//.LatestVersion(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		IProcessDefinition processDefinition = query.First();
		Assert.That(processDefinition.TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(processDefinition.Version, Is.EqualTo(2));

		query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY)
                ;//.LatestVersion(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));

		processDefinition = query.First();
		Assert.That(processDefinition.TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(processDefinition.Version, Is.EqualTo(1));
	  }

	   [Test]   public virtual void testQueryByLatestWithTenantIds()
	  {
		// deploy a second version for tenant one
		DeploymentForTenant(TENANT_ONE, emptyProcess);

		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY)
                ;//.LatestVersion(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));
		// one definition for each tenant
		Assert.That(query.Count(), Is.EqualTo(2L));

		IDictionary<string, IProcessDefinition> processDefinitionsForTenant = getProcessDefinitionsForTenant(query.ToList());
		Assert.That(processDefinitionsForTenant[TENANT_ONE].Version, Is.EqualTo(2));
		Assert.That(processDefinitionsForTenant[TENANT_TWO].Version, Is.EqualTo(1));
	  }

	   [Test]   public virtual void testQueryByLatestWithoutTenantId()
	  {
		// deploy a second version without tenant id
		Deployment(emptyProcess);

	      IQueryable<IProcessDefinition> query =
	              repositoryService.CreateProcessDefinitionQuery(c => c.Key == PROCESS_DEFINITION_KEY)
	          ;//.LatestVersion(c=>c.TenantId == null);

		Assert.That(query.Count(), Is.EqualTo(1L));

		IProcessDefinition processDefinition = query.First();
		Assert.That(processDefinition.TenantId, Is.EqualTo(null));
		Assert.That(processDefinition.Version, Is.EqualTo(2));
	  }

	   [Test]   public virtual void testQueryByLatestWithTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		// deploy a second version without tenant id
		Deployment(emptyProcess);
		// deploy a third version for tenant one
		DeploymentForTenant(TENANT_ONE, emptyProcess);
		DeploymentForTenant(TENANT_ONE, emptyProcess);

	      IQueryable<IProcessDefinition> query =
	          repositoryService.CreateProcessDefinitionQuery(c => c.Key == PROCESS_DEFINITION_KEY);
                //.LatestVersion(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId)).IncludeProcessDefinitionsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(3L));

		IDictionary<string, IProcessDefinition> processDefinitionsForTenant = getProcessDefinitionsForTenant(query.ToList());
		Assert.That(processDefinitionsForTenant[TENANT_ONE].Version, Is.EqualTo(3));
		Assert.That(processDefinitionsForTenant[TENANT_TWO].Version, Is.EqualTo(1));
		Assert.That(processDefinitionsForTenant[null].Version, Is.EqualTo(2));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
	      IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery(c=>new string[]{ TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))
	          /*.OrderByTenantId()*/
	          /*.Asc()*/
	          .ToList();

		Assert.That(processDefinitions.Count, Is.EqualTo(2));
		Assert.That(processDefinitions[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(processDefinitions[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(processDefinitions.Count, Is.EqualTo(2));
		Assert.That(processDefinitions[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(processDefinitions[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	  protected internal virtual IDictionary<string, IProcessDefinition> getProcessDefinitionsForTenant(IList<IProcessDefinition> processDefinitions)
	  {
		IDictionary<string, IProcessDefinition> definitionsForTenant = new Dictionary<string, IProcessDefinition>();

		foreach (IProcessDefinition definition in processDefinitions)
		{
		  definitionsForTenant[definition.TenantId] = definition;
		}
		return definitionsForTenant;
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId))
            //.IncludeProcessDefinitionsWithoutTenantId()
            .Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(o => string.IsNullOrEmpty(o.TenantId)).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();
		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	}

}