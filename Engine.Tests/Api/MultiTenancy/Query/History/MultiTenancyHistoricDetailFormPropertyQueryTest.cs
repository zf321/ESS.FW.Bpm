using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query.History
{
    


	public class MultiTenancyHistoricDetailFormPropertyQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

    [SetUp]
	  protected internal void setUp()
	  {
		IBpmnModelInstance oneTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().UserTask("userTask").camundaFormField().CamundaId("myFormField").CamundaType("string").CamundaFormFieldDone().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, oneTaskProcess);
		DeploymentForTenant(TENANT_TWO, oneTaskProcess);

		IProcessInstance processInstanceOne = startProcessInstanceForTenant(TENANT_ONE);
		IProcessInstance processInstanceTwo = startProcessInstanceForTenant(TENANT_TWO);

		completeUserTask(processInstanceOne);
		completeUserTask(processInstanceTwo);
	  }
        [Test]
        public virtual void testQueryWithoutTenantId()
	  {
		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();//.FormFields();

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }
        [Test]
        public virtual void testQueryByTenantId()
	  {
		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()
                ;//.FormFields(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = historyService.CreateHistoricDetailQuery()
                ;//.FormFields(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }
        [Test]
        public virtual void testQueryByTenantIds()
        {
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()
                ;//.FormFields(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }
        [Test]
        public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery()
                ;//.FormFields(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }
        [Test]
        public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.CreateHistoricDetailQuery()
                    ;//.FormFields(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }
        [Test]
        public virtual void testQuerySortingAsc()
	  {
		IList<IHistoricDetail> historicDetails = historyService.CreateHistoricDetailQuery()
                //.FormFields()/*.OrderByTenantId()*/.Asc()
            .ToList();

		Assert.That(historicDetails.Count, Is.EqualTo(2));
		Assert.That(historicDetails[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(historicDetails[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }
        [Test]
        public virtual void testQuerySortingDesc()
        {
            IList<IHistoricDetail> historicDetails = historyService.CreateHistoricDetailQuery()
               // .FormFields()
                ///*.OrderByTenantId()*/
                //.Desc()
                .ToList();

		Assert.That(historicDetails.Count, Is.EqualTo(2));
		Assert.That(historicDetails[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(historicDetails[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }
        [Test]
        public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }
        [Test]
        public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query
            //.TenantIdIn(TENANT_ONE)
            .Count(), Is.EqualTo(2L));
		Assert.That(query
            //.TenantIdIn(TENANT_TWO)
            .Count(), Is.EqualTo(0L));
		Assert.That(query
           // .TenantIdIn(TENANT_ONE, TENANT_TWO)
            .Count(), Is.EqualTo(2L));
	  }
        [Test]
        public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();

		Assert.That(query.Count(), Is.EqualTo(4L));
		Assert.That(query
           // .TenantIdIn(TENANT_ONE)
            .Count(), Is.EqualTo(2L));
		Assert.That(query
           // .TenantIdIn(TENANT_TWO)
            .Count(), Is.EqualTo(2L));
	  }
        [Test]
        public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery();
		Assert.That(query.Count(), Is.EqualTo(4L));
	  }
        
	  protected internal virtual IProcessInstance startProcessInstanceForTenant(string tenant)
	  {
		return runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(tenant).Execute();
	  }

	  protected internal virtual void completeUserTask(IProcessInstance processInstance)
	  {
		ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
		Assert.That(task, null);

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["myFormField"] = "myFormFieldValue";
		formService.SubmitTaskForm(task.Id, properties as IDictionary<string, ITypedValue>);
	  }

	}

}