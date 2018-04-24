using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query
{
    
    /// <summary>
	/// 
	/// 
	/// </summary>
	public class MultiTenancyTaskQueryTest : PluggableProcessEngineTestCase
	{

	  private const string TENANT_ONE = "tenant1";
	  private const string TENANT_TWO = "tenant2";
	  private const string TENANT_NON_EXISTING = "nonExistingTenant";

	  private readonly IList<string> taskIds = new List<string>();

        [SetUp]
	  protected internal void setUp()
	  {

		createTaskWithoutTenant();
		createTaskForTenant(TENANT_ONE);
		createTaskForTenant(TENANT_TWO);
	  }


	   [Test]   public virtual void testQueryNoTenantIdSet()
	  {
		IQueryable<ITask> query = taskService.CreateTaskQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }


	   [Test]   public virtual void testQueryByTenantId()
	  {
		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = taskService.CreateTaskQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }


	   [Test]   public virtual void testQueryByTenantIds()
	  {
		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = taskService.CreateTaskQuery(c=> new[] { TENANT_ONE, TENANT_NON_EXISTING }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }


	   [Test]   public virtual void testQueryByTasksWithoutTenantId()
	  {
		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.TenantId == null);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }


	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.TenantId == TENANT_NON_EXISTING);

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }


	   [Test]   public virtual void testQueryByTenantIdNullFails()
	  {
		try
		{
		  Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.TenantId == (string)null));

		  Assert.Fail("Exception expected");
		}
		catch (NullValueException)
		{
		  // expected
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	  {
		// exclude tasks without tenant id because of database-specific ordering
		IList<ITask> tasks = taskService.CreateTaskQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Asc()*/.ToList();

		Assert.That(tasks.Count, Is.EqualTo(2));
		Assert.That(tasks[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(tasks[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		// exclude tasks without tenant id because of database-specific ordering
	      IList<ITask> tasks = taskService.CreateTaskQuery(c=>new string[]{ TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))
	          /*.OrderByTenantId()*/
	          /*.Desc()*/
	          .ToList();

		Assert.That(tasks.Count, Is.EqualTo(2));
		Assert.That(tasks[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(tasks[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<ITask> query = taskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<ITask> query = taskService.CreateTaskQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		//Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId)).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<ITask> query = taskService.CreateTaskQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
		//Assert.That(query.WithoutTenantId().Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<ITask> query = taskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	  protected internal virtual string createTaskWithoutTenant()
	  {
		return createTaskForTenant(null);
	  }

	  protected internal virtual string createTaskForTenant(string tenantId)
	  {
		ITask task = taskService.NewTask();
		if (!string.ReferenceEquals(tenantId, null))
		{
		  task.TenantId = tenantId;
		}
		taskService.SaveTask(task);

		string taskId = task.Id;
		taskIds.Add(taskId);

		return taskId;
	  }

    [TearDown]
	  protected internal void tearDown()
	  {
		identityService.ClearAuthentication();
		foreach (string taskId in taskIds)
		{
		  taskService.DeleteTask(taskId, true);
		}
		taskIds.Clear();
	  }

	}

}