using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    

	public class MultiTenancyExternalTaskCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyExternalTaskCmdsTenantCheckTest()
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


	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string PROCESS_DEFINITION_KEY = "twoExternalTaskProcess";
	  private const string ERROR_DETAILS = "anErrorDetail";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal const string WORKER_ID = "aWorkerId";

	  protected internal const long LOCK_TIME = 10000L;

	  protected internal const string TOPIC_NAME = "externalTaskTopic";

	  protected internal const string ERROR_MESSAGE = "errorMessage";

	  protected internal IExternalTaskService externalTaskService;

	  protected internal ITaskService taskService;

	  protected internal string ProcessInstanceId;

	  protected internal IIdentityService identityService;

[SetUp]
	  public virtual void init()
	  {

		externalTaskService = engineRule.ExternalTaskService;

		taskService = engineRule.TaskService;

		identityService = engineRule.IdentityService;

		testRule.DeployForTenant(TENANT_ONE, "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml");

		ProcessInstanceId = engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

	  }

	  // fetch and lock test cases
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithAuthenticatedTenant()
	   [Test]   public virtual void testFetchAndLockWithAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		//IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
		//Assert.AreEqual(1, externalTasks.Count);

	  }


	   [Test]   public virtual void testFetchAndLockWithNoAuthenticatedTenant()
	  {

		identityService.SetAuthentication("aUserId", null);

		// then external task cannot be fetched due to the absence of tenant Id authentication
		//IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
		//Assert.AreEqual(0, externalTasks.Count);

	  }


	   [Test]   public virtual void testFetchAndLockWithDifferentTenant()
	  {

		identityService.SetAuthentication("aUserId", null, new List<string>(){ "tenantTwo"});

		// then external task cannot be fetched due to the absence of 'tenant1' authentication
		//IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
	//	Assert.AreEqual(0, externalTasks.Count);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithDisabledTenantCheck()
	   [Test]   public virtual void testFetchAndLockWithDisabledTenantCheck()
	  {

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		// then
		//IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute();
		//Assert.AreEqual(1, externalTasks.Count);

	  }

	  // complete external task test cases
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithAuthenticatedTenant()
	   [Test]   public virtual void testCompleteWithAuthenticatedTenant()
	  {

		string externalTaskId = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		Assert.AreEqual(1, externalTaskService.CreateExternalTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		externalTaskService.Complete(externalTaskId, WORKER_ID);

		Assert.That(externalTaskService.CreateExternalTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithNoAuthenticatedTenant()
	   [Test]   public virtual void testCompleteWithNoAuthenticatedTenant()
	  {

		string externalTaskId = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		Assert.AreEqual(1, externalTaskService.CreateExternalTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		externalTaskService.Complete(externalTaskId, WORKER_ID);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithDisableTenantCheck()
	   [Test]   public virtual void testCompleteWithDisableTenantCheck()
	  {

		string externalTaskId = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		Assert.AreEqual(1, externalTaskService.CreateExternalTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		externalTaskService.Complete(externalTaskId, WORKER_ID);
		// then
		Assert.That(externalTaskService.CreateExternalTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
	  }

	  // handle failure test cases
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleFailureWithAuthenticatedTenant()
	   [Test]   public virtual void testHandleFailureWithAuthenticatedTenant()
	  {

		//ILockedExternalTask task = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute().First();

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		//externalTaskService.HandleFailure(task.Id, WORKER_ID, ERROR_MESSAGE, 1, 0);

		// then
		//Assert.AreEqual(ERROR_MESSAGE, externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute().First().ErrorMessage);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleFailureWithNoAuthenticatedTenant()
	   [Test]   public virtual void testHandleFailureWithNoAuthenticatedTenant()
	  {

		//ILockedExternalTask task = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute().First();

		identityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		//externalTaskService.HandleFailure(task.Id, WORKER_ID, ERROR_MESSAGE, 1, 0);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleFailureWithDisabledTenantCheck()
	   [Test]   public virtual void testHandleFailureWithDisabledTenantCheck()
	  {

		string taskId = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		externalTaskService.HandleFailure(taskId, WORKER_ID, ERROR_MESSAGE, 1, 0);
		// then
		//Assert.AreEqual(ERROR_MESSAGE, externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.Execute().First().ErrorMessage);
	  }

	  // handle BPMN error
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleBPMNErrorWithAuthenticatedTenant()
	   [Test]   public virtual void testHandleBPMNErrorWithAuthenticatedTenant()
	  {

		string taskId = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// when
		externalTaskService.HandleBpmnError(taskId, WORKER_ID, "ERROR-OCCURED");

		// then
		Assert.AreEqual(taskService.CreateTaskQuery().First().TaskDefinitionKey, "afterBpmnError");
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleBPMNErrorWithNoAuthenticatedTenant()
	   [Test]   public virtual void testHandleBPMNErrorWithNoAuthenticatedTenant()
	  {

		string taskId = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		identityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		externalTaskService.HandleBpmnError(taskId, WORKER_ID, "ERROR-OCCURED");

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleBPMNErrorWithDisabledTenantCheck()
	   [Test]   public virtual void testHandleBPMNErrorWithDisabledTenantCheck()
	  {

		string taskId = externalTaskService.FetchAndLock(1, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		externalTaskService.HandleBpmnError(taskId, WORKER_ID, "ERROR-OCCURED");

		// then
		Assert.AreEqual(taskService.CreateTaskQuery().First().TaskDefinitionKey, "afterBpmnError");

	  }

	  // setRetries test
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithAuthenticatedTenant()
	   [Test]   public virtual void testSetRetriesWithAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// when
		externalTaskService.SetRetries(externalTaskId, 5);

		// then
		Assert.AreEqual(5, (int) externalTaskService.CreateExternalTaskQuery().First().Retries);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithNoAuthenticatedTenant()
	   [Test]   public virtual void testSetRetriesWithNoAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		identityService.SetAuthentication("aUserId", null);
		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		externalTaskService.SetRetries(externalTaskId, 5);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithDisabledTenantCheck()
	   [Test]   public virtual void testSetRetriesWithDisabledTenantCheck()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		externalTaskService.SetRetries(externalTaskId, 5);

		// then
		Assert.AreEqual(5, (int) externalTaskService.CreateExternalTaskQuery().First().Retries);

	  }

	  // set priority test cases
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriorityWithAuthenticatedTenant()
	   [Test]   public virtual void testSetPriorityWithAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// when
		externalTaskService.SetPriority(externalTaskId, 1);

		// then
		Assert.AreEqual(1, (int) externalTaskService.CreateExternalTaskQuery().First().Priority);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriorityWithNoAuthenticatedTenant()
	   [Test]   public virtual void testSetPriorityWithNoAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		identityService.SetAuthentication("aUserId", null);
		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		externalTaskService.SetPriority(externalTaskId, 1);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriorityWithDisabledTenantCheck()
	   [Test]   public virtual void testSetPriorityWithDisabledTenantCheck()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		externalTaskService.SetPriority(externalTaskId, 1);

		// then
		Assert.AreEqual(1, (int) externalTaskService.CreateExternalTaskQuery().First().Priority);
	  }

	  // unlock test cases
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlockWithAuthenticatedTenant()
	   [Test]   public virtual void testUnlockWithAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*///.Execute()
                .First().Id;

		Assert.That(externalTaskService.CreateExternalTaskQuery()//.Locked()
            .Count(), Is.EqualTo(1L));

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// when
		externalTaskService.Unlock(externalTaskId);

		// then
		Assert.That(externalTaskService.CreateExternalTaskQuery()//.Locked()
            .Count(), Is.EqualTo(0L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlockWithNoAuthenticatedTenant()
	   [Test]   public virtual void testUnlockWithNoAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*///.Execute()
                .First().Id;

		identityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		externalTaskService.Unlock(externalTaskId);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlockWithDisabledTenantCheck()
	   [Test]   public virtual void testUnlockWithDisabledTenantCheck()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		externalTaskService.Unlock(externalTaskId);
		// then
		Assert.That(externalTaskService.CreateExternalTaskQuery()//.Locked()
            .Count(), Is.EqualTo(0L));
	  }

	  // get error details tests
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsWithAuthenticatedTenant()
	   [Test]   public virtual void testGetErrorDetailsWithAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		externalTaskService.HandleFailure(externalTaskId,WORKER_ID,ERROR_MESSAGE,ERROR_DETAILS,1,1000L);

		identityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// when then
		Assert.That(externalTaskService.GetExternalTaskErrorDetails(externalTaskId), Is.EqualTo(ERROR_DETAILS));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsWithNoAuthenticatedTenant()
	   [Test]   public virtual void testGetErrorDetailsWithNoAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		externalTaskService.HandleFailure(externalTaskId,WORKER_ID,ERROR_MESSAGE,ERROR_DETAILS,1,1000L);

		identityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		externalTaskService.GetExternalTaskErrorDetails(externalTaskId);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsWithDisabledTenantCheck()
	   [Test]   public virtual void testGetErrorDetailsWithDisabledTenantCheck()
	  {
		// given
		string externalTaskId = externalTaskService.FetchAndLock(5, WORKER_ID)/*.Topic(TOPIC_NAME, LOCK_TIME)*/.First().Id;//.Execute().First().Id;

		externalTaskService.HandleFailure(externalTaskId,WORKER_ID,ERROR_MESSAGE,ERROR_DETAILS,1,1000L);

		identityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		Assert.That(externalTaskService.GetExternalTaskErrorDetails(externalTaskId), Is.EqualTo(ERROR_DETAILS));
	  }
	}

}