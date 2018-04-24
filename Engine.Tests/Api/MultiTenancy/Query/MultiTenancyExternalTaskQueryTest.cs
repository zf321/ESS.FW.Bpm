using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query
{
    

	public class MultiTenancyExternalTaskQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal void setUp()
	  {
		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess().StartEvent().ServiceTask().CamundaType("external").CamundaTopic("test").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, process);
		DeploymentForTenant(TENANT_TWO, process);

		startProcessInstance(TENANT_ONE);
		startProcessInstance(TENANT_TWO);
	  }

	   [Test]   public virtual void testQueryWithoutTenantId()
	  {
		IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByTenantId()
	  {
		IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = externalTaskService.CreateExternalTaskQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  externalTaskService.CreateExternalTaskQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	   {
	       IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks = externalTaskService.CreateExternalTaskQuery()
	           /*.OrderByTenantId()*/
	           /*.Asc()*/
	           .ToList();

		Assert.That(externalTasks.Count, Is.EqualTo(2));
		Assert.That(externalTasks[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(externalTasks[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks = externalTaskService.CreateExternalTaskQuery()/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(externalTasks.Count, Is.EqualTo(2));
		Assert.That(externalTasks[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(externalTasks[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery();

		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		//Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId)).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> query = externalTaskService.CreateExternalTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	  protected internal virtual void startProcessInstance(string tenant)
	  {
		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == tenant).First().Id;

		runtimeService.StartProcessInstanceById(processDefinitionId);
	  }

	}

}