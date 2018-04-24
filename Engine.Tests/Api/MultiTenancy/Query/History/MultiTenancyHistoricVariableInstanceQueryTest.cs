using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query.History
{
    
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT) public class MultiTenancyHistoricVariableInstanceQueryTest extends org.Camunda.bpm.Engine.impl.Test.PluggableProcessEngineTestCase
	public class MultiTenancyHistoricVariableInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string TENANT_ONE_VAR = "tenant1Var";
	  protected internal const string TENANT_TWO_VAR = "tenant2Var";
        [SetUp]
	  protected internal void setUp()
	  {
		IBpmnModelInstance oneTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, oneTaskProcess);
		DeploymentForTenant(TENANT_TWO, oneTaskProcess);

		startProcessInstanceForTenant(TENANT_ONE, TENANT_ONE_VAR);
		startProcessInstanceForTenant(TENANT_TWO, TENANT_TWO_VAR);
	  }

	   [Test]   public virtual void testQueryWithoutTenantId()
	  {
		IQueryable<IHistoricVariableInstance> query = historyService.CreateHistoricVariableInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByTenantId()
	  {
		IQueryable<IHistoricVariableInstance> query = historyService.CreateHistoricVariableInstanceQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.AreEqual(query.First().Value, TENANT_ONE_VAR);

		query = historyService.CreateHistoricVariableInstanceQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.AreEqual(query.First().Value, TENANT_TWO_VAR);
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		IQueryable<IHistoricVariableInstance> query = historyService.CreateHistoricVariableInstanceQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IHistoricVariableInstance> query = historyService.CreateHistoricVariableInstanceQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.CreateHistoricVariableInstanceQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	   {
	       IList<IHistoricVariableInstance> historicVariableInstances =
	           historyService.CreateHistoricVariableInstanceQuery()
	               /*.OrderByTenantId()*/
	               /*.Asc()*/
	               .ToList();

		Assert.That(historicVariableInstances.Count, Is.EqualTo(2));
		Assert.That(historicVariableInstances[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.AreEqual(historicVariableInstances[0].Value, TENANT_ONE_VAR);
		Assert.That(historicVariableInstances[1].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.AreEqual(historicVariableInstances[1].Value, TENANT_TWO_VAR);
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		IList<IHistoricVariableInstance> historicVariableInstances = historyService.CreateHistoricVariableInstanceQuery()/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(historicVariableInstances.Count, Is.EqualTo(2));
		Assert.That(historicVariableInstances[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.AreEqual(historicVariableInstances[0].Value, TENANT_TWO_VAR);
		Assert.That(historicVariableInstances[1].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.AreEqual(historicVariableInstances[1].Value, TENANT_ONE_VAR);
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricVariableInstance> query = historyService.CreateHistoricVariableInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IHistoricVariableInstance> query = historyService.CreateHistoricVariableInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId)).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IHistoricVariableInstance> query = historyService.CreateHistoricVariableInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricVariableInstance> query = historyService.CreateHistoricVariableInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	  protected internal virtual IProcessInstance startProcessInstanceForTenant(string tenant, string @var)
	  {
		return runtimeService.CreateProcessInstanceByKey("testProcess").SetVariable("myVar", @var).SetProcessDefinitionTenantId(tenant).Execute();
	  }

	}

}