using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query.History
{
    


	public class MultiTenancyHistoricDetailVariableUpdateQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string VARIABLE_NAME = "myVar";
	  protected internal const string TENANT_ONE_VAR = "tenant1Var";
	  protected internal const string TENANT_TWO_VAR = "tenant2Var";
        [SetUp]
	  protected internal void setUp()
	  {
		IBpmnModelInstance oneTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().UserTask().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, oneTaskProcess);
		DeploymentForTenant(TENANT_TWO, oneTaskProcess);

		IProcessInstance processInstanceOne = startProcessInstanceForTenant(TENANT_ONE, TENANT_ONE_VAR);
		IProcessInstance processInstanceTwo = startProcessInstanceForTenant(TENANT_TWO, TENANT_TWO_VAR);

		completeUserTask(processInstanceOne, TENANT_ONE_VAR + "_updated");
		completeUserTask(processInstanceTwo, TENANT_TWO_VAR + "_updated");

	  }
        
         [Test]   public virtual void testQueryWithoutTenantId()
	  {
		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()/*;//.VariableUpdates()*/;

		Assert.That(query.Count(), Is.EqualTo(4L));
	  }
        
         [Test]   public virtual void testQueryByTenantId()
	  {
		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();//.VariableUpdates(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = historyService.CreateHistoricDetailQuery();//.VariableUpdates(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();//.VariableUpdates(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(4L));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();//.VariableUpdates(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.CreateHistoricDetailQuery();//.VariableUpdates(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	  {
		IList<IHistoricDetail> historicDetails = historyService.CreateHistoricDetailQuery()/*;//.VariableUpdates()*//*.OrderByTenantId()*//*.Asc()*/.ToList();

		Assert.That(historicDetails.Count, Is.EqualTo(4));
		Assert.That(historicDetails[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(historicDetails[1].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(historicDetails[2].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(historicDetails[3].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	   {
	       IList<IHistoricDetail> historicDetails = historyService.CreateHistoricDetailQuery()
	           /*;//.VariableUpdates()*/
	           /*.OrderByTenantId()*/
	           /*.Desc()*/
	           .ToList();

		Assert.That(historicDetails.Count, Is.EqualTo(4));
		Assert.That(historicDetails[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(historicDetails[1].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(historicDetails[2].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(historicDetails[3].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId)).Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();

		Assert.That(query.Count(), Is.EqualTo(4L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();
		Assert.That(query.Count(), Is.EqualTo(4L));
	  }

	  protected internal virtual IProcessInstance startProcessInstanceForTenant(string tenant, string @var)
	  {
		return runtimeService.CreateProcessInstanceByKey("testProcess").SetVariable(VARIABLE_NAME, @var).SetProcessDefinitionTenantId(tenant).Execute();
	  }

	  protected internal virtual void completeUserTask(IProcessInstance processInstance, string varValue)
	  {
		ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
		Assert.That(task, null);

		IDictionary<string, object> updatedVariables = new Dictionary<string, object>();
		updatedVariables[VARIABLE_NAME] = varValue;
		taskService.Complete(task.Id, updatedVariables);
	  }

	}

}