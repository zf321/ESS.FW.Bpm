using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query
{
    

	public class MultiTenancyJobQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal void setUp()
	  {
		IBpmnModelInstance asyncTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().UserTask().CamundaAsyncBefore().EndEvent().Done();

		Deployment(asyncTaskProcess);
		DeploymentForTenant(TENANT_ONE, asyncTaskProcess);
		DeploymentForTenant(TENANT_TWO, asyncTaskProcess);

		runtimeService.CreateProcessInstanceByKey("testProcess").ProcessDefinitionWithoutTenantId().Execute();
		runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(TENANT_TWO).Execute();
	  }

	   [Test]   public virtual void testQueryNoTenantIdSet()
	  {
		IQueryable<IJob> query = managementService.CreateJobQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByTenantId()
	  {
		IQueryable<IJob> query = managementService.CreateJobQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = managementService.CreateJobQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		IQueryable<IJob> query = managementService.CreateJobQuery(c=>
	      new []{
	          TENANT_ONE,
	          TENANT_TWO
	      }.Contains(c.TenantId)
	      );

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByJobsWithoutTenantId()
	  {
		IQueryable<IJob> query = managementService.CreateJobQuery(c=>c.TenantId == null);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIdsIncludeJobsWithoutTenantId()
	  {
		IQueryable<IJob> query = managementService.CreateJobQuery(c=>c.TenantId == TENANT_ONE);//;//.IncludeJobsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = managementService.CreateJobQuery(c=>c.TenantId == TENANT_TWO);//.IncludeJobsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = managementService.CreateJobQuery(c=> new[]{
              TENANT_ONE,
              TENANT_TWO
          }.Contains(c.TenantId));//.IncludeJobsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IJob> query = managementService.CreateJobQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  managementService.CreateJobQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	  {
		// exclude jobs without tenant id because of database-specific ordering
		IList<IJob> jobs = managementService.CreateJobQuery(c=> new[]{
              TENANT_ONE,
              TENANT_TWO
          }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Asc()*/.ToList();

		Assert.That(jobs.Count, Is.EqualTo(2));
		Assert.That(jobs[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(jobs[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		// exclude jobs without tenant id because of database-specific ordering
	      IList<IJob> jobs = managementService.CreateJobQuery(c=>new string[]{ TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))
	          /*.OrderByTenantId()*/
	          /*.Desc()*/
	          .ToList();

		Assert.That(jobs.Count, Is.EqualTo(2));
		Assert.That(jobs[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(jobs[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IJob> query = managementService.CreateJobQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IJob> query = managementService.CreateJobQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId))
            //.IncludeJobsWithoutTenantId()
            .Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IJob> query = managementService.CreateJobQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(o => string.IsNullOrEmpty(o.TenantId)).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IJob> query = managementService.CreateJobQuery();
		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	}

}