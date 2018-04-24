//using System;
//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Externaltask;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Api.Authorization.Util;
//using ESS.FW.Bpm.Engine.Tests.Api.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Api.Runtime.Migration.Models.Builder;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using ESS.FW.Bpm.Model.Bpmn;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.History
//{


//[RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//	public class HistoricExternalTaskLogQuerySortingTest
//	{
//		private bool _instanceFieldsInitialized = false;

//		public HistoricExternalTaskLogQuerySortingTest()
//		{
//			if (!_instanceFieldsInitialized)
//			{
//				InitializeInstanceFields();
//				_instanceFieldsInitialized = true;
//			}
//		}

//		private void InitializeInstanceFields()
//		{
//			AuthRule = new AuthorizationTestRule(EngineRule);
//			TestHelper = new ProcessEngineTestRule(EngineRule);
//			//ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
//		}


//	  protected internal readonly string WorkerId = "aWorkerId";
//	  protected internal readonly long LockDuration = 5 * 60L * 1000L;

//	  protected internal ProcessEngineRule EngineRule = new ProvidedProcessEngineRule();
//	  protected internal AuthorizationTestRule AuthRule;
//	  protected internal ProcessEngineTestRule TestHelper;

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
//	  ////public RuleChain ruleChain;

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Rule public final org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
//	  //public readonly ExpectedException Thrown = ExpectedException.None();

//	  protected internal IProcessInstance ProcessInstance;
//	  protected internal IRuntimeService RuntimeService;
//	  protected internal IHistoryService HistoryService;
//	  protected internal IExternalTaskService ExternalTaskService;

//[SetUp]
//	  public virtual void SetUp()
//	  {
//		RuntimeService = EngineRule.RuntimeService;
//		HistoryService = EngineRule.HistoryService;
//		ExternalTaskService = EngineRule.ExternalTaskService;
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByTimestampAsc()
//	  public virtual void TestQuerySortingByTimestampAsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query/*.OrderByTimestamp()*//*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskByTimestamp());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByTimestampDsc()
//	  public virtual void TestQuerySortingByTimestampDsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query/*.OrderByTimestamp()*//*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.inverted(TestOrderingUtil.historicExternalTaskByTimestamp()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByTaskIdAsc()
//	  public virtual void TestQuerySortingByTaskIdAsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.OrderByExternalTaskId()/*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskLogByExternalTaskId());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByTaskIdDsc()
//	  public virtual void TestQuerySortingByTaskIdDsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.OrderByExternalTaskId()/*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.inverted(TestOrderingUtil.historicExternalTaskLogByExternalTaskId()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByRetriesAsc()
//	  public virtual void TestQuerySortingByRetriesAsc()
//	  {

//		// given
//		int taskCount = 10;
//		IList<IExternalTask> list = StartProcesses(taskCount);
//		ReportExternalTaskFailure(list);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.FailureLog().OrderByRetries()/*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskLogByRetries());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByRetriesDsc()
//	  public virtual void TestQuerySortingByRetriesDsc()
//	  {

//		// given
//		int taskCount = 10;
//		IList<IExternalTask> list = StartProcesses(taskCount);
//		ReportExternalTaskFailure(list);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.FailureLog().OrderByRetries()/*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.inverted(TestOrderingUtil.historicExternalTaskLogByRetries()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByPriorityAsc()
//	  public virtual void TestQuerySortingByPriorityAsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.OrderByPriority()/*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskLogByPriority());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByPriorityDsc()
//	  public virtual void TestQuerySortingByPriorityDsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.OrderByPriority()/*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.inverted(TestOrderingUtil.historicExternalTaskLogByPriority()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByTopicNameAsc()
//	  public virtual void TestQuerySortingByTopicNameAsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcessesByTopicName(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.OrderByTopicName()/*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskLogByTopicName());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByTopicNameDsc()
//	  public virtual void TestQuerySortingByTopicNameDsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcessesByTopicName(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.OrderByTopicName()/*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.inverted(TestOrderingUtil.historicExternalTaskLogByTopicName()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByWorkerIdAsc()
//	  public virtual void TestQuerySortingByWorkerIdAsc()
//	  {

//		// given
//		int taskCount = 10;
//		IList<IExternalTask> list = StartProcesses(taskCount);
//		CompleteExternalTasksWithWorkers(list);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.SuccessLog().OrderByWorkerId()/*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskLogByWorkerId());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByWorkerIdDsc()
//	  public virtual void TestQuerySortingByWorkerIdDsc()
//	  {

//		// given
//		int taskCount = 10;
//		IList<IExternalTask> list = StartProcesses(taskCount);
//		CompleteExternalTasksWithWorkers(list);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.SuccessLog().OrderByWorkerId()/*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.Inverted(TestOrderingUtil.historicExternalTaskLogByWorkerId()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByActivityIdAsc()
//	  public virtual void TestQuerySortingByActivityIdAsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcessesByActivityId(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query/*.OrderByActivityId()*//*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskLogByActivityId());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByActivityIdDsc()
//	  public virtual void TestQuerySortingByActivityIdDsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcessesByActivityId(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query/*.OrderByActivityId()*//*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.inverted(TestOrderingUtil.historicExternalTaskLogByActivityId()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByActivityInstanceIdAsc()
//	  public virtual void TestQuerySortingByActivityInstanceIdAsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.OrderByActivityInstanceId()/*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskLogByActivityInstanceId());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByActivityInstanceIdDsc()
//	  public virtual void TestQuerySortingByActivityInstanceIdDsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query.OrderByActivityInstanceId()/*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.inverted(TestOrderingUtil.historicExternalTaskLogByActivityInstanceId()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByExecutionIdAsc()
//	  public virtual void TestQuerySortingByExecutionIdAsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query//.OrderByExecutionId()/*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskLogByExecutionId());
//	  }


////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByExecutionIdDsc()
//	  public virtual void TestQuerySortingByExecutionIdDsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query//.OrderByExecutionId()/*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.inverted(TestOrderingUtil.historicExternalTaskLogByExecutionId()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByProcessInstanceIdAsc()
//	  public virtual void TestQuerySortingByProcessInstanceIdAsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query//.OrderByProcessInstanceId()/*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskLogByProcessInstanceId());
//	  }


////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByProcessInstanceIdDsc()
//	  public virtual void TestQuerySortingByProcessInstanceIdDsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query//.OrderByProcessInstanceId()/*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.inverted(TestOrderingUtil.historicExternalTaskLogByProcessInstanceId()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByProcessDefinitionIdAsc()
//	  public virtual void TestQuerySortingByProcessDefinitionIdAsc()
//	  {

//		// given
//		int taskCount = 8;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query/*.OrderByProcessDefinitionId()*//*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskLogByProcessDefinitionId());
//	  }


////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByProcessDefinitionIdDsc()
//	  public virtual void TestQuerySortingByProcessDefinitionIdDsc()
//	  {

//		// given
//		int taskCount = 8;
//		StartProcesses(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query/*.OrderByProcessDefinitionId()*//*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.inverted(TestOrderingUtil.historicExternalTaskLogByProcessDefinitionId()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByProcessDefinitionKeyAsc()
//	  public virtual void TestQuerySortingByProcessDefinitionKeyAsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcessesByProcessDefinitionKey(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query//.OrderByProcessDefinitionKey()/*.Asc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.historicExternalTaskLogByProcessDefinitionKey(EngineRule.ProcessEngine));
//	  }


////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQuerySortingByProcessDefinitionKeyDsc()
//	  public virtual void TestQuerySortingByProcessDefinitionKeyDsc()
//	  {

//		// given
//		int taskCount = 10;
//		StartProcessesByProcessDefinitionKey(taskCount);

//		// when
//		IQueryable<IHistoricExternalTaskLog> query = HistoryService.CreateHistoricExternalTaskLogQuery();
//		query//.OrderByProcessDefinitionKey()/*.Desc()*/;

//		// then
//		VerifyQueryWithOrdering(query, taskCount, TestOrderingUtil.inverted(TestOrderingUtil.historicExternalTaskLogByProcessDefinitionKey(EngineRule.ProcessEngine)));
//	  }

//	  // helper ------------------------------------

//	  protected internal virtual void CompleteExternalTasksWithWorkers(IList<IExternalTask> taskLIst)
//	  {
//		for (int i = 0; i < taskLIst.Count; i++)
//		{
//		  CompleteExternalTaskWithWorker(taskLIst[i].Id, i.ToString());
//		}
//	  }

//	  protected internal virtual void CompleteExternalTaskWithWorker(string externalTaskId, string workerId)
//	  {
//		CompleteExternalTask(externalTaskId, DefaultExternalTaskModelBuilder.DefaultTopic, workerId, false);

//	  }

//	  protected internal virtual void CompleteExternalTask(string externalTaskId, string topic, string workerId, bool usePriority)
//	  {
//		IList<ILockedExternalTask> list = ExternalTaskService.FetchAndLock(100, workerId, usePriority).Topic(topic, LockDuration).Execute();
//		ExternalTaskService.Complete(externalTaskId, workerId);
//		// unlock the remaining tasks
//		foreach (ILockedExternalTask lockedExternalTask in list)
//		{
//		  if (!lockedExternalTask.Id.Equals(externalTaskId))
//		  {
//			ExternalTaskService.Unlock(lockedExternalTask.Id);
//		  }
//		}
//	  }

//	  protected internal virtual void ReportExternalTaskFailure(IList<IExternalTask> taskLIst)
//	  {
//		for (int i = 0; i < taskLIst.Count; i++)
//		{
//		  ReportExternalTaskFailure(taskLIst[i].Id, DefaultExternalTaskModelBuilder.DefaultTopic, WorkerId, i + 1, false, "foo");
//		}
//	  }

//	  protected internal virtual void ReportExternalTaskFailure(string externalTaskId, string topic, string workerId, int retries, bool usePriority, string errorMessage)
//	  {
//		IList<ILockedExternalTask> list = ExternalTaskService.FetchAndLock(100, workerId, usePriority).Topic(topic, LockDuration).Execute();
//		ExternalTaskService.HandleFailure(externalTaskId, workerId, errorMessage, retries, 0L);

//		foreach (ILockedExternalTask lockedExternalTask in list)
//		{
//		  ExternalTaskService.Unlock(lockedExternalTask.Id);
//		}
//	  }

//	  protected internal virtual IList<IExternalTask> StartProcesses(int count)
//	  {
//		IList<IExternalTask> list = new List<IExternalTask>();
//		for (int ithPrio = 0; ithPrio < count; ithPrio++)
//		{
//		  list.Add(StartExternalTaskProcessGivenPriority(ithPrio));
//		  EnsureEnoughTimePassedByForTimestampOrdering();
//		}
//		return list;
//	  }

//	  protected internal virtual IList<IExternalTask> StartProcessesByTopicName(int count)
//	  {
//		IList<IExternalTask> list = new List<IExternalTask>();
//		for (int? ithTopic = 0; ithTopic < count; ithTopic++)
//		{
//		  list.Add(StartExternalTaskProcessGivenTopicName(ithTopic.ToString()));
//		}
//		return list;
//	  }

//	  protected internal virtual IList<IExternalTask> StartProcessesByActivityId(int count)
//	  {
//		IList<IExternalTask> list = new List<IExternalTask>();
//		for (int? ithTopic = 0; ithTopic < count; ithTopic++)
//		{
//		  list.Add(StartExternalTaskProcessGivenActivityId("Activity" + ithTopic.ToString()));
//		}
//		return list;
//	  }

//	  protected internal virtual IList<IExternalTask> StartProcessesByProcessDefinitionKey(int count)
//	  {
//		IList<IExternalTask> list = new List<IExternalTask>();
//		for (int? ithTopic = 0; ithTopic < count; ithTopic++)
//		{
//		  list.Add(StartExternalTaskProcessGivenProcessDefinitionKey("ProcessKey" + ithTopic.ToString()));
//		}
//		return list;
//	  }

//	  protected internal virtual IExternalTask StartExternalTaskProcessGivenTopicName(string topicName)
//	  {
//		IBpmnModelInstance processModelWithCustomTopic = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel().Topic(topicName).Build();
//		IProcessDefinition sourceProcessDefinition = TestHelper.DeployAndGetDefinition(processModelWithCustomTopic);
//		IProcessInstance pi = RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
//		return ExternalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId == pi.Id).First();
//	  }

//	  protected internal virtual IExternalTask StartExternalTaskProcessGivenActivityId(string activityId)
//	  {
//		IBpmnModelInstance processModelWithCustomActivityId = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel().ExternalTaskName(activityId).Build();
//		IProcessDefinition sourceProcessDefinition = TestHelper.DeployAndGetDefinition(processModelWithCustomActivityId);
//		IProcessInstance pi = RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
//		return ExternalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId == pi.Id).First();
//	  }

//	  protected internal virtual IExternalTask StartExternalTaskProcessGivenProcessDefinitionKey(string processDefinitionKey)
//	  {
//		IBpmnModelInstance processModelWithCustomKey = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel().ProcessKey(processDefinitionKey).Build();
//		IProcessDefinition sourceProcessDefinition = TestHelper.DeployAndGetDefinition(processModelWithCustomKey);
//		IProcessInstance pi = RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
//		return ExternalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId == pi.Id).First();
//	  }

//	  protected internal virtual IExternalTask StartExternalTaskProcessGivenPriority(int priority)
//	  {
//		IBpmnModelInstance processModelWithCustomPriority = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel().Priority(priority).Build();
//		IProcessDefinition sourceProcessDefinition = TestHelper.DeployAndGetDefinition(processModelWithCustomPriority);
//		IProcessInstance pi = RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
//		return ExternalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId == pi.Id).First();
//	  }

//	  protected internal virtual void VerifyQueryWithOrdering(IQueryable<IHistoricExternalTaskLog> query, int countExpected, TestOrderingUtil.NullTolerantComparator<IHistoricExternalTaskLog> expectedOrdering)
//	  {
//		Assert.That(countExpected, Is.EqualTo(query.Count()));
//		Assert.That((long) countExpected, Is.EqualTo(query.Count()));
//		TestOrderingUtil.verifySorting(query.ToList();, expectedOrdering);
//	  }

//	  protected internal virtual void EnsureEnoughTimePassedByForTimestampOrdering()
//	  {
//		long timeToAddInSeconds = 5 * 1000L;
//		DateTime nowPlus5Seconds = new DateTime(ClockUtil.CurrentTime.TimeOfDay.Ticks + timeToAddInSeconds);
//		ClockUtil.CurrentTime = nowPlus5Seconds;
//	  }

//	}

//}