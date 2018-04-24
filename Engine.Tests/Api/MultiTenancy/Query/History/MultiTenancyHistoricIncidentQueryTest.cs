using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query.History
{
    


	public class MultiTenancyHistoricIncidentQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal static readonly IBpmnModelInstance BPMN = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("failingProcess").StartEvent().ServiceTask().Func(m=>m.CamundaAsyncBefore=true)/*.CamundaAsyncBefore()*/.Func(m=>m.CamundaExpression= "${failing}")/*.CamundaExpression("${failing}")*/.EndEvent().Done();

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal void setUp()
	  {
		DeploymentForTenant(TENANT_ONE, BPMN);
		DeploymentForTenant(TENANT_TWO, BPMN);

		startProcessInstanceAndExecuteFailingJobForTenant(TENANT_ONE);
		startProcessInstanceAndExecuteFailingJobForTenant(TENANT_TWO);
	  }

	   [Test]   public virtual void testQueryWithoutTenantId()
	  {
		IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByTenantId()
	  {
		IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = historyService.CreateHistoricIncidentQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.CreateHistoricIncidentQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	   {
	       IList<IHistoricIncident> historicIncidents = historyService.CreateHistoricIncidentQuery()
	           /*.OrderByTenantId()*/
	           /*.Asc()*/
	           .ToList();

		Assert.That(historicIncidents.Count, Is.EqualTo(2));
		Assert.That(historicIncidents[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(historicIncidents[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		IList<IHistoricIncident> historicIncidents = historyService.CreateHistoricIncidentQuery()/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(historicIncidents.Count, Is.EqualTo(2));
		Assert.That(historicIncidents[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(historicIncidents[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId)).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	  protected internal virtual void startProcessInstanceAndExecuteFailingJobForTenant(string tenant)
	  {
		runtimeService.CreateProcessInstanceByKey("failingProcess").SetProcessDefinitionTenantId(tenant).Execute();

		ExecuteAvailableJobs();
	  }

	}

}