using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query
{
    
	public class MultiTenancyJobDefinitionQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal void setUp()
	  {
		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().TimerWithDuration("PT1M").UserTask().EndEvent().Done();

		Deployment(process);
		DeploymentForTenant(TENANT_ONE, process);
		DeploymentForTenant(TENANT_TWO, process);

		// the deployed process definition contains a timer start event
		// - so a job definition is created on deployment.
	  }

	   [Test]   public virtual void testQueryNoTenantIdSet()
	  {
		IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByTenantId()
	  {
		IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = managementService.CreateJobDefinitionQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByDefinitionsWithoutTenantIds()
	  {
		IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery(c=>c.TenantId == null);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery(c=>c.TenantId == TENANT_ONE)/*.IncludeJobDefinitionsWithoutTenantId()*/;

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = managementService.CreateJobDefinitionQuery(c=>c.TenantId == TENANT_TWO)/*.IncludeJobDefinitionsWithoutTenantId()*/;

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = managementService.CreateJobDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.IncludeJobDefinitionsWithoutTenantId()*/;

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  managementService.CreateJobDefinitionQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	  {
		// exclude job definitions without tenant id because of database-specific ordering
		IList<IJobDefinition> jobDefinitions = managementService.CreateJobDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Asc()*/.ToList();

		Assert.That(jobDefinitions.Count, Is.EqualTo(2));
		Assert.That(jobDefinitions[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(jobDefinitions[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		// exclude job definitions without tenant id because of database-specific ordering
		IList<IJobDefinition> jobDefinitions = managementService.CreateJobDefinitionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(jobDefinitions.Count, Is.EqualTo(2));
		Assert.That(jobDefinitions[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(jobDefinitions[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId))/*.IncludeJobDefinitionsWithoutTenantId()*/.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(o => string.IsNullOrEmpty(o.TenantId)).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IJobDefinition> query = managementService.CreateJobDefinitionQuery();
		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	}

}