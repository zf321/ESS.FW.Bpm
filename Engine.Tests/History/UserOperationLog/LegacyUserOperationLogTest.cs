using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.History.UserOperationLog
{



    /// <summary>
    /// 
    /// 
    /// </summary>
    [TestFixture]
    public class LegacyUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public LegacyUserOperationLogTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(processEngineRule);
			//chain = RuleChain.outerRule(processEngineRule).around(testHelper);
		}


	  public const string USER_ID = "demo";

//JAVA TO C# CONVERTER TODO EntityTypes.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static util.ProcessEngineBootstrapRule bootstrapRule = new util.ProcessEngineBootstrapRule("resources/history/useroperationlog/enable.legacy.User.operation.log.Camunda.cfg.xml");
	  public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRule("resources/history/useroperationlog/enable.legacy.User.operation.log.Camunda.cfg.xml");
	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO EntityTypes.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(processEngineRule).around(testHelper);
	  //public RuleChain chain;

	  protected internal IIdentityService identityService;
	  protected internal IRuntimeService runtimeService;
	  protected internal ITaskService taskService;
	  protected internal IHistoryService historyService;
	  protected internal IManagementService managementService;

	  protected internal IBatch batch;

        [SetUp]
        public virtual void initServices()
	  {
		identityService = processEngineRule.IdentityService;
		runtimeService = processEngineRule.RuntimeService;
		taskService = processEngineRule.TaskService;
		historyService = processEngineRule.HistoryService;
		managementService = processEngineRule.ManagementService;
	  }
        [TearDown]
	  public virtual void removeBatch()
	  {
		IBatch batch = managementService.CreateBatchQuery().First();
		if (batch != null)
		{
		  managementService.DeleteBatch(batch.Id, true);
		}

		IHistoricBatch historicBatch = historyService.CreateHistoricBatchQuery().First();
		if (historicBatch != null)
		{
		  historyService.DeleteHistoricBatch(historicBatch.Id);
		}
	  }
        [Test] [Deployment( "resources/history/useroperationlog/UserOperationLogTaskTest.TestOnlyTaskCompletionIsLogged.bpmn20.xml") ]
	  public virtual void testLogAllOperationWithAuthentication()
	  {
		try
		{
		  // given
		  identityService.AuthenticatedUserId = USER_ID;
		  string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

		  string taskId = taskService.CreateTaskQuery().First().Id;

		  // when
		  taskService.Complete(taskId);

		  // then
		  Assert.True((bool) runtimeService.GetVariable(ProcessInstanceId, "taskListenerCalled"));
		  Assert.True((bool) runtimeService.GetVariable(ProcessInstanceId, "serviceTaskCalled"));

		  //IQueryable<IUserOperationLogEntry> query = userOperationLogQuery(c=>c.Id == USER_ID);
		  //Assert.AreEqual(3, query.Count());
		  //Assert.AreEqual(1, query.Where(c=>c.OperationType==UserOperationLogEntryFields.OperationTypeComplete).Count());
		  //Assert.AreEqual(2, query.Where(c=>c.OperationType==UserOperationLogEntryFields.OperationTypeSetVariable).Count());

		}
		finally
		{
		  identityService.ClearAuthentication();
		}
	  }

        [Test]
        [Deployment("resources/history/useroperationlog/UserOperationLogTaskTest.TestOnlyTaskCompletionIsLogged.bpmn20.xml")]
        public virtual void testLogOperationWithoutAuthentication()
	  {
		// given
		string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

		string taskId = taskService.CreateTaskQuery().First().Id;

		// when
		taskService.Complete(taskId);

		// then
		Assert.True((bool) runtimeService.GetVariable(ProcessInstanceId, "taskListenerCalled"));
		Assert.True((bool) runtimeService.GetVariable(ProcessInstanceId, "serviceTaskCalled"));

		Assert.AreEqual(4, userOperationLogQuery().Count());
		//Assert.AreEqual(1, userOperationLogQuery().OperationType(UserOperationLogEntryFields.OperationTypeComplete).Count());
		//Assert.AreEqual(2, userOperationLogQuery().OperationType(UserOperationLogEntryFields.OperationTypeSetVariable).Count());
		//Assert.AreEqual(1, userOperationLogQuery().OperationType(UserOperationLogEntryFields.OperationTypeCreate).Count());
	  }

        [Test]
        [Deployment("resources/history/useroperationlog/UserOperationLogTaskTest.TestOnlyTaskCompletionIsLogged.bpmn20.xml")]
        public virtual void testLogSetVariableWithoutAuthentication()
	  {
		// given
		string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

		// when
		runtimeService.SetVariable(ProcessInstanceId, "aVariable", "aValue");

		// then
		Assert.AreEqual(2, userOperationLogQuery().Count());
		//Assert.AreEqual(1, userOperationLogQuery().OperationType(UserOperationLogEntryFields.OperationTypeSetVariable).Count());
		//Assert.AreEqual(1, userOperationLogQuery().OperationType(UserOperationLogEntryFields.OperationTypeCreate).Count());
	  }

        [Test]
        public virtual void testDontWriteDuplicateLogOnBatchDeletionJobExecution()
	  {
		IProcessDefinition definition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessInstance processInstance = runtimeService.StartProcessInstanceById(definition.Id);
		batch = runtimeService.DeleteProcessInstancesAsync(new List<string>(){processInstance.Id}, null, "test reason");

		IJob seedJob = managementService.CreateJobQuery().First();
		managementService.ExecuteJob(seedJob.Id);

		foreach (IJob pending in managementService.CreateJobQuery().ToList())
		{
		  managementService.ExecuteJob(pending.Id);
		}

		Assert.AreEqual(4, userOperationLogQuery().Count());
	  }

        [Test]
        public virtual void testDontWriteDuplicateLogOnBatchMigrationJobExecution()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

		IProcessInstance processInstance = runtimeService.StartProcessInstanceById(sourceDefinition.Id);

		IMigrationPlan migrationPlan = runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		batch = runtimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).ExecuteAsync();
		IJob seedJob = managementService.CreateJobQuery().First();
		managementService.ExecuteJob(seedJob.Id);

		IJob migrationJob = managementService.CreateJobQuery(c=>c.JobDefinitionId==batch.BatchJobDefinitionId).First();

		// when
		managementService.ExecuteJob(migrationJob.Id);

		// then
		Assert.AreEqual(5, userOperationLogQuery().Count());
		//Assert.AreEqual(2, userOperationLogQuery().OperationType(UserOperationLogEntryFields.OperationTypeCreate).EntityType(EntityTypes.Deployment).Count());
		//Assert.AreEqual(3, userOperationLogQuery().OperationType(UserOperationLogEntryFields.OperationTypeMigrate).EntityType(EntityTypes.ProcessInstance).Count());
	  }

	  protected internal virtual IQueryable<IUserOperationLogEntry> userOperationLogQuery()
	  {
		return historyService.CreateUserOperationLogQuery();
	  }

	}

}