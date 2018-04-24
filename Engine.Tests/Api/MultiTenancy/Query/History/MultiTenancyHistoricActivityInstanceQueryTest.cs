using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query.History
{


	public class MultiTenancyHistoricActivityInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal void setUp()
	  {
		IBpmnModelInstance oneTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, oneTaskProcess);
		DeploymentForTenant(TENANT_TWO, oneTaskProcess);

		startProcessInstanceForTenant(TENANT_ONE);
		startProcessInstanceForTenant(TENANT_TWO);
	  }
        [Test]
	  public virtual void testQueryWithoutTenantId()
	  {
		IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(4L));
	  }
        [Test]
        public virtual void testQueryByTenantId()
	  {
		IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = historyService.CreateHistoricActivityInstanceQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }
        [Test]
        public virtual void testQueryByTenantIds()
	  {
		IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(4L));
	  }
        [Test]
        public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }
        [Test]
        public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.CreateHistoricActivityInstanceQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }
        [Test]
        public virtual void testQuerySortingAsc()
        {
            IList<IHistoricActivityInstance> historicActivityInstances =
                historyService.CreateHistoricActivityInstanceQuery()
                    /*.OrderByTenantId()*/
                    /*.Asc()*/
                    .ToList();

		Assert.That(historicActivityInstances.Count, Is.EqualTo(4));
		Assert.That(historicActivityInstances[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(historicActivityInstances[1].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(historicActivityInstances[2].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(historicActivityInstances[3].TenantId, Is.EqualTo(TENANT_TWO));
	  }
        [Test]
        public virtual void testQuerySortingDesc()
	  {
		IList<IHistoricActivityInstance> historicActivityInstances = historyService.CreateHistoricActivityInstanceQuery()/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(historicActivityInstances.Count, Is.EqualTo(4));
		Assert.That(historicActivityInstances[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(historicActivityInstances[1].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(historicActivityInstances[2].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(historicActivityInstances[3].TenantId, Is.EqualTo(TENANT_ONE));
	  }
        [Test]
        public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }
        [Test]
        public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId)).Count(), Is.EqualTo(2L));
	  }
        [Test]
        public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(4L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(2L));
	  }
        [Test]
        public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(4L));
	  }

	  protected internal virtual IProcessInstance startProcessInstanceForTenant(string tenant)
	  {
		return runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(tenant).Execute();
	  }

	}

}