using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query.History
{


    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
    public class MultiTenancyHistoricTaskInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal void setUp()
	  {
		IBpmnModelInstance oneTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().UserTask().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, oneTaskProcess);
		DeploymentForTenant(TENANT_TWO, oneTaskProcess);

		IProcessInstance processInstanceOne = startProcessInstanceForTenant(TENANT_ONE);
		IProcessInstance processInstanceTwo = startProcessInstanceForTenant(TENANT_TWO);

		completeUserTask(processInstanceOne);
		completeUserTask(processInstanceTwo);
	  }

	   [Test]   public virtual void testQueryWithoutTenantId()
	  {
		IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByTenantId()
	  {
		IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = historyService.CreateHistoricTaskInstanceQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.CreateHistoricTaskInstanceQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	   {
	       IList<IHistoricTaskInstance> historicTaskInstances = historyService.CreateHistoricTaskInstanceQuery()
	           /*.OrderByTenantId()*/
	           /*.Asc()*/
	           .ToList();

		Assert.That(historicTaskInstances.Count, Is.EqualTo(2));
		Assert.That(historicTaskInstances[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(historicTaskInstances[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		IList<IHistoricTaskInstance> historicTaskInstances = historyService.CreateHistoricTaskInstanceQuery()/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(historicTaskInstances.Count, Is.EqualTo(2));
		Assert.That(historicTaskInstances[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(historicTaskInstances[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId)).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	  protected internal virtual IProcessInstance startProcessInstanceForTenant(string tenant)
	  {
		return runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(tenant).Execute();
	  }

	  protected internal virtual void completeUserTask(IProcessInstance processInstance)
	  {
	   ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
	   Assert.That(task!=null);
	   taskService.Complete(task.Id);
	  }

	}

}