//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Filter;
//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Task;
//using ESS.FW.Bpm.Model.Xml.instance;
//using NUnit.Framework;
//using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;



//namespace ESS.FW.Bpm.Engine.Tests.Api.MultiTenancy
//{
    

//	public class MultiTenancyFilterServiceTest : PluggableProcessEngineTestCase
//	{

//	  protected internal const string TENANT_ONE = "tenant1";
//	  protected internal const string TENANT_TWO = "tenant2";
//	  protected internal static readonly string[] TENANT_IDS = new string[] {TENANT_ONE, TENANT_TWO};

//	  protected internal string filterId = null;
//	  protected internal readonly IList<string> taskIds = new List<string>();

//        [SetUp]
//	  protected internal void setUp()
//	  {
//		createTaskWithoutTenantId();
//		createTaskForTenant(TENANT_ONE);
//		createTaskForTenant(TENANT_TWO);
//	  }

//	   [Test]   public virtual void testCreateFilterWithTenantIdCriteria()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.TenantId == TENANT_IDS);
//		filterId = createFilter(query);

//		IFilter savedFilter = filterService.GetFilter(filterId);
//		//TaskQueryImpl savedQuery = savedFilter.Query;

//		//Assert.That(savedQuery.TenantIds, Is.EqualTo(TENANT_IDS));
//	  }

//	   [Test]   public virtual void testCreateFilterWithNoTenantIdCriteria()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.TenantId == null);
//		filterId = createFilter(query);

//		IFilter savedFilter = filterService.GetFilter(filterId);
//		//TaskQueryImpl savedQuery = savedFilter.Query;

//		//Assert.That(savedQuery.TenantIdSet, Is.EqualTo(true));

//		//Assert.That(savedQuery.TenantIds, Is.EqualTo(null));
//	  }

//	   [Test]   public virtual void testFilterTasksNoTenantIdSet()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		filterId = createFilter(query);

//		Assert.That(filterService.Count(filterId), Is.EqualTo(3L));
//	  }

//	   [Test]   public virtual void testFilterTasksByTenantIds()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.TenantId == TENANT_IDS);
//		filterId = createFilter(query);

//		Assert.That(filterService.Count(filterId), Is.EqualTo(2L));

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery(c=>c.Name =="testTask");
//	//	Assert.That(filterService.Count<IModelElementInstance>(filterId, extendingQuery), Is.EqualTo(2L));
//	  }

//	   [Test]   public virtual void testFilterTasksWithoutTenantId()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.TenantId == null);
//		filterId = createFilter(query);

//		Assert.That(filterService.Count(filterId), Is.EqualTo(1L));

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery(c=>c.Name =="testTask");
//		//Assert.That(filterService.Count(filterId, extendingQuery), Is.EqualTo(1L));
//	  }

//	   [Test]   public virtual void testFilterTasksByExtendingQueryWithTenantId()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.Name =="testTask");
//		filterId = createFilter(query);

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery(c=>c.TenantId == TENANT_ONE);
//		//Assert.That(filterService.Count(filterId, extendingQuery), Is.EqualTo(1L));
//	  }

//	   [Test]   public virtual void testFilterTasksByExtendingQueryWithoutTenantId()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.Name =="testTask");
//		filterId = createFilter(query);

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery(c=>c.TenantId == null);
//		//Assert.That(filterService.Count(filterId, extendingQuery), Is.EqualTo(1L));
//	  }

//	   [Test]   public virtual void testFilterTasksWithNoAuthenticatedTenants()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		filterId = createFilter(query);

//		identityService.SetAuthentication("user", null, null);

//		Assert.That(filterService.Count(filterId), Is.EqualTo(1L));
//	  }

//	   [Test]   public virtual void testFilterTasksWithAuthenticatedTenant()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		filterId = createFilter(query);

//		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

//		Assert.That(filterService.Count(filterId), Is.EqualTo(2L));
//	  }

//	   [Test]   public virtual void testFilterTasksWithAuthenticatedTenants()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		filterId = createFilter(query);

//		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

//		Assert.That(filterService.Count(filterId), Is.EqualTo(3L));
//	  }

//	   [Test]   public virtual void testFilterTasksByTenantIdNoAuthenticatedTenants()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.TenantId == TENANT_ONE);
//		filterId = createFilter(query);

//		identityService.SetAuthentication("user", null, null);

//		Assert.That(filterService.Count(filterId), Is.EqualTo(0L));
//	  }

//	   [Test]   public virtual void testFilterTasksByTenantIdWithAuthenticatedTenant()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.TenantId == TENANT_ONE);
//		filterId = createFilter(query);

//		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

//		Assert.That(filterService.Count(filterId), Is.EqualTo(1L));
//	  }

//	   [Test]   public virtual void testFilterTasksByExtendingQueryWithTenantIdNoAuthenticatedTenants()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.Name =="testTask");
//		filterId = createFilter(query);

//		identityService.SetAuthentication("user", null, null);

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery(c=>c.TenantId == TENANT_ONE);
//		//Assert.That(filterService.Count(filterId, extendingQuery), Is.EqualTo(0L));
//	  }

//	   [Test]   public virtual void testFilterTasksByExtendingQueryWithTenantIdAuthenticatedTenant()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.Name =="testTask");
//		filterId = createFilter(query);

//		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery(c=>c.TenantId == TENANT_ONE);
//		//Assert.That(filterService.Count(filterId, extendingQuery), Is.EqualTo(1L));
//	  }

//	   [Test]   public virtual void testFilterTasksWithDisabledTenantCheck()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		filterId = createFilter(query);

//		processEngineConfiguration.SetTenantCheckEnabled(false);
//		identityService.SetAuthentication("user", null, null);

//		Assert.That(filterService.Count(filterId), Is.EqualTo(3L));
//	  }

//	  protected internal virtual void createTaskWithoutTenantId()
//	  {
//		createTaskForTenant(null);
//	  }

//	  protected internal virtual void createTaskForTenant(string tenantId)
//	  {
//		ITask NewTask = taskService.NewTask();
//		NewTask.Name = "testTask";

//		if (!string.ReferenceEquals(tenantId, null))
//		{
//		  NewTask.TenantId = tenantId;
//		}

//		taskService.SaveTask(NewTask);

//		taskIds.Add(NewTask.Id);
//	  }

//	  protected internal virtual string createFilter(IQueryable<ITask> query)
//	  {
//		IFilter newFilter = filterService.NewTaskFilter("myFilter");
//		//newFilter.Query = query;

//		return filterService.SaveFilter(newFilter).Id;
//	  }

//        [TearDown]
//	  protected internal void tearDown()
//	  {
//		filterService.DeleteFilter(filterId);
//		identityService.ClearAuthentication();
//		foreach (string taskId in taskIds)
//		{
//		  taskService.DeleteTask(taskId, true);
//		}
//	  }
//	}

//}