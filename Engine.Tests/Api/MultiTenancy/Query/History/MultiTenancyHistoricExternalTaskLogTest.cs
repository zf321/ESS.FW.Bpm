using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Api.Runtime.Migration.Models.Builder;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.Query.History
{

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class MultiTenancyHistoricExternalTaskLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricExternalTaskLogTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			//ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal IHistoryService historyService;
	  protected internal IRuntimeService runtimeService;
	  protected internal IRepositoryService repositoryService;
	  protected internal IIdentityService identityService;
	  protected internal IExternalTaskService externalTaskService;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
	  //public RuleChain ruleChain;

	  protected internal readonly string TENANT_ONE = "tenant1";
	  protected internal readonly string TENANT_TWO = "tenant2";

	  protected internal readonly string WORKER_ID = "aWorkerId";
	  protected internal readonly string ERROR_DETAILS = "These are the error details!";
	  protected internal readonly long LOCK_DURATION = 5 * 60L * 1000L;


    [SetUp]
	  public virtual void setUp()
	  {
		repositoryService = engineRule.RepositoryService;
		historyService = engineRule.HistoryService;
		runtimeService = engineRule.RuntimeService;
		identityService = engineRule.IdentityService;
		externalTaskService = engineRule.ExternalTaskService;

		testRule.DeployForTenant(TENANT_ONE, ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);

		startProcessInstanceAndFailExternalTask(TENANT_ONE);
		startProcessInstanceFailAndCompleteExternalTask(TENANT_TWO);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithoutTenantId()
	   [Test]   public virtual void testQueryWithoutTenantId()
	  {

		//given two process with different tenants

		// when
		IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

		// then
		Assert.That(query.Count(), Is.EqualTo(5L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantId()
	   [Test]   public virtual void testQueryByTenantId()
	  {

		// given two process with different tenants

		// when
		IQueryable<IHistoricExternalTaskLog> queryTenant1 = historyService.CreateHistoricExternalTaskLogQuery(c=>c.TenantId == TENANT_ONE);
		IQueryable<IHistoricExternalTaskLog> queryTenant2 = historyService.CreateHistoricExternalTaskLogQuery(c=>c.TenantId == TENANT_TWO);

		// then
		Assert.That(queryTenant1.Count(), Is.EqualTo(2L));
		Assert.That(queryTenant2.Count(), Is.EqualTo(3L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantIds()
	   [Test]   public virtual void testQueryByTenantIds()
	  {

		//given two process with different tenants

		// when
		IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		// then
		Assert.That(query.Count(), Is.EqualTo(5L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingTenantId()
	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {

		//given two process with different tenants

		// when
		IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery(c=>c.TenantId == "nonExisting");

		// then
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailQueryByTenantIdNull()
	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.CreateHistoricExternalTaskLogQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		  // test passed
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingAsc()
	   [Test]   public virtual void testQuerySortingAsc()
	  {

		//given two process with different tenants

		// when
	      IList<IHistoricExternalTaskLog> HistoricExternalTaskLogs = historyService.CreateHistoricExternalTaskLogQuery()
	          /*.OrderByTenantId()*/
	          /*.Asc()*/
	          .ToList();

		// then
		Assert.That(HistoricExternalTaskLogs.Count, Is.EqualTo(5));
		Assert.That(HistoricExternalTaskLogs[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(HistoricExternalTaskLogs[1].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(HistoricExternalTaskLogs[2].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(HistoricExternalTaskLogs[3].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(HistoricExternalTaskLogs[4].TenantId, Is.EqualTo(TENANT_TWO));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingDesc()
	   [Test]   public virtual void testQuerySortingDesc()
	  {

		//given two process with different tenants

		// when
		IList<IHistoricExternalTaskLog> HistoricExternalTaskLogs = historyService.CreateHistoricExternalTaskLogQuery()/*.OrderByTenantId()*//*.Desc()*/.ToList();

		// then
		Assert.That(HistoricExternalTaskLogs.Count, Is.EqualTo(5));
		Assert.That(HistoricExternalTaskLogs[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(HistoricExternalTaskLogs[1].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(HistoricExternalTaskLogs[2].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(HistoricExternalTaskLogs[3].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(HistoricExternalTaskLogs[4].TenantId, Is.EqualTo(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryNoAuthenticatedTenants()
	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {

		// given
		identityService.SetAuthentication("user", null, null);

		// when
		IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

		// then
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryAuthenticatedTenant()
	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		// given
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_TWO});

		// when
		IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

		// then
		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId)).Count(), Is.EqualTo(2L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryAuthenticatedTenants()
	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		// given
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		// when
		IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

		// then
		Assert.That(query.Count(), Is.EqualTo(5L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(3L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryDisabledTenantCheck()
	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		// given
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		// when
		IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

		// then
		Assert.That(query.Count(), Is.EqualTo(5L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsNoAuthenticatedTenants()
	   [Test]   public virtual void testGetErrorDetailsNoAuthenticatedTenants()
	  {
		// given
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_TWO});

		string failedHistoricExternalTaskLogId = historyService.CreateHistoricExternalTaskLogQuery()
                //.FailureLog(c=>c.TenantId == TENANT_ONE)
                .First().Id;
		identityService.ClearAuthentication();
		identityService.SetAuthentication("user", null, null);


		try
		{
		  // when
		  historyService.GetHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);
		  Assert.Fail("Exception expected: It should not be possible to retrieve the error details");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  string errorMessage = e.Message;
		  Assert.That(errorMessage.Contains("Cannot get the historic external task log "), Is.EqualTo(true));
		  Assert.That(errorMessage.Contains(failedHistoricExternalTaskLogId), Is.EqualTo(true));
		  Assert.That(errorMessage.Contains("because it belongs to no authenticated tenant."), Is.EqualTo(true));
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsAuthenticatedTenant()
	   [Test]   public virtual void testGetErrorDetailsAuthenticatedTenant()
	  {
		// given
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_TWO});

		string failedHistoricExternalTaskLogId = historyService.CreateHistoricExternalTaskLogQuery()//.FailureLog(c=>c.TenantId == TENANT_ONE)
                .First().Id;

		// when
		string stacktrace = historyService.GetHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);

		// then
		Assert.That(stacktrace!=null);
		Assert.That(stacktrace, Is.EqualTo(ERROR_DETAILS));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsAuthenticatedTenants()
	   [Test]   public virtual void testGetErrorDetailsAuthenticatedTenants()
	  {
		// given
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		string logIdTenant1 = historyService.CreateHistoricExternalTaskLogQuery()//.FailureLog(c=>c.TenantId == TENANT_ONE)
                .First().Id;

		string logIdTenant2 = historyService.CreateHistoricExternalTaskLogQuery()//.FailureLog(c=>c.TenantId == TENANT_ONE)
                .First().Id;

		// when
		string stacktrace1 = historyService.GetHistoricExternalTaskLogErrorDetails(logIdTenant1);
		string stacktrace2 = historyService.GetHistoricExternalTaskLogErrorDetails(logIdTenant2);

		// then
		Assert.That(stacktrace1!=null);
		Assert.That(stacktrace1, Is.EqualTo(ERROR_DETAILS));
		Assert.That(stacktrace2!=null);
		Assert.That(stacktrace2, Is.EqualTo(ERROR_DETAILS));
	  }

	  // helper methods

	  protected internal virtual void completeExternalTask(string externalTaskId)
	  {
		//IList<ILockedExternalTask> list = externalTaskService.FetchAndLock(100, WORKER_ID, true).Topic(ExternalTaskModels.TOPIC, LOCK_DURATION).Execute();
		//externalTaskService.Complete(externalTaskId, WORKER_ID);
		//// unlock the remaining tasks
		//foreach (ILockedExternalTask lockedExternalTask in list)
		//{
		//  if (!lockedExternalTask.Id.Equals(externalTaskId))
		//  {
		//	externalTaskService.Unlock(lockedExternalTask.Id);
		//  }
		//}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") protected org.Camunda.bpm.Engine.Externaltask.IExternalTask startProcessInstanceAndFailExternalTask(String tenant)
	  protected internal virtual ESS.FW.Bpm.Engine.Externaltask.IExternalTask startProcessInstanceAndFailExternalTask(string tenant)
	  {
		IProcessInstance pi = runtimeService.CreateProcessInstanceByKey(DefaultExternalTaskModelBuilder.DefaultProcessKey).SetProcessDefinitionTenantId(tenant).Execute();
		ESS.FW.Bpm.Engine.Externaltask.IExternalTask externalTask = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId == pi.Id).First();
		reportExternalTaskFailure(externalTask.Id);
		return externalTask;
	  }

	  protected internal virtual void startProcessInstanceFailAndCompleteExternalTask(string tenant)
	  {
		ESS.FW.Bpm.Engine.Externaltask.IExternalTask task = startProcessInstanceAndFailExternalTask(tenant);
		completeExternalTask(task.Id);
	  }

	  protected internal virtual void reportExternalTaskFailure(string externalTaskId)
	  {
		reportExternalTaskFailure(externalTaskId, ExternalTaskModels.TOPIC, WORKER_ID, 1, false, "This is an error!");
	  }

	  protected internal virtual void reportExternalTaskFailure(string externalTaskId, string topic, string workerId, int retries, bool usePriority, string errorMessage)
	  {
		IList<ILockedExternalTask> list = externalTaskService.FetchAndLock(100, workerId, usePriority)
                 //.Topic(topic, LOCK_DURATION).Execute();
                 as IList<ILockedExternalTask>;

            externalTaskService.HandleFailure(externalTaskId, workerId, errorMessage, ERROR_DETAILS, retries, 0L);

		foreach (ILockedExternalTask lockedExternalTask in list)
		{
		  externalTaskService.Unlock(lockedExternalTask.Id);
		}
	  }
	}

}