using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query
{
    

	public class MultiTenancyProcessInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal void setUp()
	  {
		IBpmnModelInstance oneTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().UserTask().EndEvent().Done();

		Deployment(oneTaskProcess);
		DeploymentForTenant(TENANT_ONE, oneTaskProcess);
		DeploymentForTenant(TENANT_TWO, oneTaskProcess);

		runtimeService.CreateProcessInstanceByKey("testProcess").ProcessDefinitionWithoutTenantId().Execute();
		runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(TENANT_TWO).Execute();
	  }

	   [Test]   public virtual void testQueryNoTenantIdSet()
	  {
		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByTenantId()
	  {
		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = runtimeService.CreateProcessInstanceQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByInstancesWithoutTenantId()
	  {
		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery(c=>c.TenantId == null);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  runtimeService.CreateProcessInstanceQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	  {
		// exclude instances without tenant id because of database-specific ordering
		IList<IProcessInstance> processInstances = runtimeService.CreateProcessInstanceQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Asc()*/.ToList();

		Assert.That(processInstances.Count, Is.EqualTo(2));
		Assert.That(processInstances[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(processInstances[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		// exclude instances without tenant id because of database-specific ordering
		IList<IProcessInstance> processInstances = runtimeService.CreateProcessInstanceQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(processInstances.Count, Is.EqualTo(2));
		Assert.That(processInstances[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(processInstances[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId)).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(o=>string.IsNullOrEmpty(o.TenantId)).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	}

}