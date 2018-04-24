using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query.History
{


    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class MultiTenancyHistoricDecisionInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string DMN = "resources/api/multitenancy/simpleDecisionTable.Dmn";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal void setUp()
	  {
		DeploymentForTenant(TENANT_ONE, DMN);
		DeploymentForTenant(TENANT_TWO, DMN);

		evaluateDecisionInstanceForTenant(TENANT_ONE);
		evaluateDecisionInstanceForTenant(TENANT_TWO);
	  }
        [Test]
        public virtual void testQueryWithoutTenantId()
	  {
		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }
        [Test]
        public virtual void testQueryByTenantId()
	  {
		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = historyService.CreateHistoricDecisionInstanceQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }
        [Test]
        public virtual void testQueryByTenantIds()
	  {
		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }
        [Test]
        public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }
        [Test]
        public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.CreateHistoricDecisionInstanceQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }
        [Test]
        public virtual void testQuerySortingAsc()
        {
            IList<IHistoricDecisionInstance> historicDecisionInstances =
                historyService.CreateHistoricDecisionInstanceQuery()
                    /*.OrderByTenantId()*/
                    /*.Asc()*/
                    .ToList();

		Assert.That(historicDecisionInstances.Count, Is.EqualTo(2));
		Assert.That(historicDecisionInstances[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(historicDecisionInstances[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }
        [Test]
        public virtual void testQuerySortingDesc()
	  {
		IList<IHistoricDecisionInstance> historicDecisionInstances = historyService.CreateHistoricDecisionInstanceQuery()/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(historicDecisionInstances.Count, Is.EqualTo(2));
		Assert.That(historicDecisionInstances[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(historicDecisionInstances[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }
        [Test]
        public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }
        [Test]
        public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId)).Count(), Is.EqualTo(1L));
	  }
        [Test]
        public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }
        [Test]
        public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	  protected internal virtual void evaluateDecisionInstanceForTenant(string tenant)
	  {
		string decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.TenantId == tenant).First().Id;

		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("status", "bronze");
		decisionService.EvaluateDecisionTableById(decisionDefinitionId, variables);
	  }

	}

}