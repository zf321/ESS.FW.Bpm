using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query
{
    

	public class MultiTenancyDeploymentQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

    [SetUp]
	  public  void setUp()
	  {
		IBpmnModelInstance emptyProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess().Done();

		Deployment(emptyProcess);
		DeploymentForTenant(TENANT_ONE, emptyProcess);
		DeploymentForTenant(TENANT_TWO, emptyProcess);
	  }

	   [Test]   public virtual void testQueryNoTenantIdSet()
	  {
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();

	   Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByTenantId()
	  {
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = repositoryService.CreateDeploymentQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryWithoutTenantId()
	  {
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery(c=>c.TenantId == null);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIdsIncludeDeploymentsWithoutTenantId()
	  {
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery(c=>c.TenantId == TENANT_ONE)/*.IncludeDeploymentsWithoutTenantId()*/;

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = repositoryService.CreateDeploymentQuery(c=>c.TenantId == TENANT_TWO)/*.IncludeDeploymentsWithoutTenantId()*/;

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = repositoryService.CreateDeploymentQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.IncludeDeploymentsWithoutTenantId()*/;

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  repositoryService.CreateDeploymentQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	  {
		// exclude deployments without tenant id because of database-specific ordering
	      IList<IDeployment> deployments = repositoryService.CreateDeploymentQuery(c=>new string[]{ TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))
	          ///*.OrderByTenantId()*/
	          /*.Asc()*/
	          .ToList();

		Assert.That(deployments.Count, Is.EqualTo(2));
		Assert.That(deployments[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(deployments[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		// exclude deployments without tenant id because of database-specific ordering
		IList<IDeployment> deployments = repositoryService.CreateDeploymentQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(deployments.Count, Is.EqualTo(2));
		Assert.That(deployments[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(deployments[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		//Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId))/*.IncludeDeploymentsWithoutTenantId()*/.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, new List<string>() { TENANT_ONE, TENANT_TWO });

		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
		//Assert.That(query.WithoutTenantId().Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		Assert.That(query.Count(), Is.EqualTo(3L));
	  }


	}

}