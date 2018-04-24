//using System;
//using System.Collections.Generic;


//namespace ESS.FW.Bpm.Engine.Tests.History
//{


//	/// <summary>
//	/// 
//	/// </summary>
//[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT) ]
//	public class HistoricCaseActivityInstanceTest : CmmnProcessEngineTestCase
//	{
////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[]{"resources/api/cmmn/emptyStageWithManualActivationCase.cmmn"}) public void testHistoricCaseActivityInstanceProperties()
//		public virtual void testHistoricCaseActivityInstanceProperties()
//		{
//		string activityId = "PI_Stage_1";

//		createCaseInstance();
//		ICaseExecution stage = queryCaseExecutionByActivityId(activityId);
//		IHistoricCaseActivityInstance historicStage = queryHistoricActivityCaseInstance(activityId);

//		Assert.AreEqual(stage.Id, historicStage.Id);
//		Assert.AreEqual(stage.ParentId, historicStage.ParentCaseActivityInstanceId);
//		Assert.AreEqual(stage.CaseDefinitionId, historicStage.CaseDefinitionId);
//		Assert.AreEqual(stage.CaseInstanceId, historicStage.CaseInstanceId);
//		Assert.AreEqual(stage.ActivityId, historicStage.CaseActivityId);
//		Assert.AreEqual(stage.ActivityName, historicStage.CaseActivityName);
//		Assert.AreEqual(stage.ActivityType, historicStage.CaseActivityType);

//		manualStart(stage.Id);

//		historicStage = queryHistoricActivityCaseInstance(activityId);
//		Assert.NotNull(historicStage.EndTime);
//		}

//[Test]
//[Deployment]
//	  public virtual void testHistoricCaseActivityTaskStates()
//	  {
//		string humanTaskId1 = "PI_HumanTask_1";
//		string humanTaskId2 = "PI_HumanTask_2";
//		string humanTaskId3 = "PI_HumanTask_3";

//		// given
//		string caseInstanceId = createCaseInstance().Id;
//		string taskInstanceId1 = queryCaseExecutionByActivityId(humanTaskId1).Id;
//		string taskInstanceId2 = queryCaseExecutionByActivityId(humanTaskId2).Id;
//		string taskInstanceId3 = queryCaseExecutionByActivityId(humanTaskId3).Id;

//		// human task 1 should enabled and human task 2 and 3 will be available cause the sentry is not fulfilled
//		AssertHistoricState(humanTaskId1, ENABLED);
//		AssertHistoricState(humanTaskId2, AVAILABLE);
//		AssertHistoricState(humanTaskId3, AVAILABLE);
//		AssertStateQuery(ENABLED, AVAILABLE, AVAILABLE);

//		// when human task 1 is started
//		manualStart(taskInstanceId1);

//		// then human task 1 is active and human task 2 and 3 are still available
//		AssertHistoricState(humanTaskId1, ACTIVE);
//		AssertHistoricState(humanTaskId2, AVAILABLE);
//		AssertHistoricState(humanTaskId3, AVAILABLE);
//		AssertStateQuery(ACTIVE, AVAILABLE, AVAILABLE);

//		// when human task 1 is completed
//		complete(taskInstanceId1);

//		// then human task 1 is completed and human task 2 is enabled and human task 3 is active
//		AssertHistoricState(humanTaskId1, COMPLETED);
//		AssertHistoricState(humanTaskId2, ENABLED);
//		AssertHistoricState(humanTaskId3, ACTIVE);
//		AssertStateQuery(COMPLETED, ENABLED, ACTIVE);

//		// disable human task 2
//		disable(taskInstanceId2);
//		AssertHistoricState(humanTaskId1, COMPLETED);
//		AssertHistoricState(humanTaskId2, DISABLED);
//		AssertHistoricState(humanTaskId3, ACTIVE);
//		AssertStateQuery(COMPLETED, DISABLED, ACTIVE);

//		// re-enable human task 2
//		reenable(taskInstanceId2);
//		AssertHistoricState(humanTaskId1, COMPLETED);
//		AssertHistoricState(humanTaskId2, ENABLED);
//		AssertHistoricState(humanTaskId3, ACTIVE);
//		AssertStateQuery(COMPLETED, ENABLED, ACTIVE);

//		// suspend human task 3
//		suspend(taskInstanceId3);
//		AssertHistoricState(humanTaskId1, COMPLETED);
//		AssertHistoricState(humanTaskId2, ENABLED);
//		AssertHistoricState(humanTaskId3, SUSPENDED);
//		AssertStateQuery(COMPLETED, ENABLED, SUSPENDED);

//		// resume human task 3
//		resume(taskInstanceId3);
//		AssertHistoricState(humanTaskId1, COMPLETED);
//		AssertHistoricState(humanTaskId2, ENABLED);
//		AssertHistoricState(humanTaskId3, ACTIVE);
//		AssertStateQuery(COMPLETED, ENABLED, ACTIVE);

//		// when the case instance is suspended
//		suspend(caseInstanceId);

//		// then human task 2 and 3 are suspended
//		AssertHistoricState(humanTaskId1, COMPLETED);
//		AssertHistoricState(humanTaskId2, SUSPENDED);
//		AssertHistoricState(humanTaskId3, SUSPENDED);
//		AssertStateQuery(COMPLETED, SUSPENDED, SUSPENDED);

//		// when case instance is re-activated
//		reactivate(caseInstanceId);

//		// then human task 2 is enabled and human task is active
//		AssertHistoricState(humanTaskId1, COMPLETED);
//		AssertHistoricState(humanTaskId2, ENABLED);
//		AssertHistoricState(humanTaskId3, ACTIVE);
//		AssertStateQuery(COMPLETED, ENABLED, ACTIVE);

//		manualStart(taskInstanceId2);
//		// when human task 3 is terminated
//		terminate(taskInstanceId3);

//		// then human task 2 and 3 are terminated caused by the exitCriteria of human task 2
//		AssertHistoricState(humanTaskId1, COMPLETED);
//		AssertHistoricState(humanTaskId2, TERMINATED);
//		AssertHistoricState(humanTaskId3, TERMINATED);
//		AssertStateQuery(COMPLETED, TERMINATED, TERMINATED);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @IDeployment public void testHistoricCaseActivityMilestoneStates()
//	  public virtual void testHistoricCaseActivityMilestoneStates()
//	  {
//		string milestoneId1 = "PI_Milestone_1";
//		string milestoneId2 = "PI_Milestone_2";
//		string humanTaskId1 = "PI_HumanTask_1";
//		string humanTaskId2 = "PI_HumanTask_2";

//		// given
//		string caseInstanceId = createCaseInstance().Id;
//		string milestoneInstance1 = queryCaseExecutionByActivityId(milestoneId1).Id;
//		string milestoneInstance2 = queryCaseExecutionByActivityId(milestoneId2).Id;
//		string humanTaskInstance1 = queryCaseExecutionByActivityId(humanTaskId1).Id;

//		// then milestone 1 and 2 are available and
//		// humanTask 1 and 2 are enabled
//		AssertHistoricState(milestoneId1, AVAILABLE);
//		AssertHistoricState(milestoneId2, AVAILABLE);
//		AssertHistoricState(humanTaskId1, ENABLED);
//		AssertHistoricState(humanTaskId2, ENABLED);
//		AssertStateQuery(AVAILABLE, AVAILABLE, ENABLED, ENABLED);

//		// suspend event milestone 1 and 2
//		suspend(milestoneInstance1);
//		suspend(milestoneInstance2);
//		AssertHistoricState(milestoneId1, SUSPENDED);
//		AssertHistoricState(milestoneId2, SUSPENDED);
//		AssertHistoricState(humanTaskId1, ENABLED);
//		AssertHistoricState(humanTaskId2, ENABLED);
//		AssertStateQuery(SUSPENDED, SUSPENDED, ENABLED, ENABLED);

//		// resume IUser milestone 1
//		resume(milestoneInstance1);
//		AssertHistoricState(milestoneId1, AVAILABLE);
//		AssertHistoricState(milestoneId2, SUSPENDED);
//		AssertHistoricState(humanTaskId1, ENABLED);
//		AssertHistoricState(humanTaskId2, ENABLED);
//		AssertStateQuery(AVAILABLE, SUSPENDED, ENABLED, ENABLED);

//		// when humanTask 1 is terminated
//		manualStart(humanTaskInstance1);
//		terminate(humanTaskInstance1);

//		// then humanTask 1 is terminated and milestone 1 is completed caused by its entryCriteria
//		AssertHistoricState(milestoneId1, COMPLETED);
//		AssertHistoricState(milestoneId2, SUSPENDED);
//		AssertHistoricState(humanTaskId1, TERMINATED);
//		AssertHistoricState(humanTaskId2, ENABLED);
//		AssertStateQuery(COMPLETED, SUSPENDED, TERMINATED, ENABLED);

//		// when the case instance is terminated
//		terminate(caseInstanceId);

//		// then milestone 2 is terminated
//		AssertHistoricState(milestoneId1, COMPLETED);
//		AssertHistoricState(milestoneId2, TERMINATED);
//		AssertHistoricState(humanTaskId1, TERMINATED);
//		AssertHistoricState(humanTaskId2, TERMINATED);
//		AssertStateQuery(COMPLETED, TERMINATED, TERMINATED, TERMINATED);
//	  }


//[Test]
//[Deployment]
//	  public virtual void testHistoricCaseActivityInstanceDates()
//	  {
//		string taskId1 = "PI_HumanTask_1";
//		string taskId2 = "PI_HumanTask_2";
//		string taskId3 = "PI_HumanTask_3";
//		string milestoneId1 = "PI_Milestone_1";
//		string milestoneId2 = "PI_Milestone_2";
//		string milestoneId3 = "PI_Milestone_3";

//		// create test dates
//		long duration = 72 * 3600 * 1000;
//		DateTime created = ClockUtil.CurrentTime;
//		DateTime ended = new DateTime(created.Ticks + duration);

//		ClockUtil.CurrentTime = created;
//		string caseInstanceId = createCaseInstance().Id;
//		string taskInstance1 = queryCaseExecutionByActivityId(taskId1).Id;
//		string taskInstance2 = queryCaseExecutionByActivityId(taskId2).Id;
//		string taskInstance3 = queryCaseExecutionByActivityId(taskId3).Id;
//		string milestoneInstance1 = queryCaseExecutionByActivityId(milestoneId1).Id;
//		string milestoneInstance2 = queryCaseExecutionByActivityId(milestoneId2).Id;
//		string milestoneInstance3 = queryCaseExecutionByActivityId(milestoneId3).Id;

//		// Assert create time of all historic instances
//		AssertHistoricCreateTime(taskId1, created);
//		AssertHistoricCreateTime(taskId2, created);
//		AssertHistoricCreateTime(milestoneId1, created);
//		AssertHistoricCreateTime(milestoneId2, created);

//		// complete human task 1
//		ClockUtil.CurrentTime = ended;
//		complete(taskInstance1);

//		// Assert end time of human task 1
//		AssertHistoricEndTime(taskId1, ended);
//		AssertHistoricDuration(taskId1, duration);

//		// complete milestone 1
//		ClockUtil.CurrentTime = ended;
//		occur(milestoneInstance1);

//		// Assert end time of milestone 1
//		AssertHistoricEndTime(milestoneId1, ended);
//		AssertHistoricDuration(milestoneId1, duration);

//		// terminate human task 2
//		ClockUtil.CurrentTime = ended;
//		terminate(taskInstance2);

//		// Assert end time of human task 2
//		AssertHistoricEndTime(taskId2, ended);
//		AssertHistoricDuration(taskId2, duration);

//		// terminate milestone 2
//		ClockUtil.CurrentTime = ended;
//		terminate(milestoneInstance2);

//		// Assert end time of IUser event 2
//		AssertHistoricEndTime(milestoneId2, ended);
//		AssertHistoricDuration(milestoneId2, duration);

//		// disable human task 3 and suspend milestone 3
//		disable(taskInstance3);
//		suspend(milestoneInstance3);

//		// when terminate case instance
//		ClockUtil.CurrentTime = ended;
//		terminate(caseInstanceId);

//		// then human task 3 and milestone 3 should be terminated and a end time is set
//		AssertHistoricEndTime(taskId3, ended);
//		AssertHistoricEndTime(milestoneId3, ended);
//		AssertHistoricDuration(taskId3, duration);
//		AssertHistoricDuration(milestoneId3, duration);

//		// test queries
//		DateTime beforeCreate = new DateTime(created.Ticks - 3600 * 1000);
//		DateTime afterEnd = new DateTime(ended.Ticks + 3600 * 1000);

//		AssertCount(6, historicQuery().CreatedAfter(beforeCreate));
//		AssertCount(0, historicQuery().CreatedAfter(ended));

//		AssertCount(0, historicQuery().CreatedBefore(beforeCreate));
//		AssertCount(6, historicQuery().CreatedBefore(ended));

//		AssertCount(0, historicQuery().CreatedBefore(beforeCreate).CreatedAfter(ended));

//		AssertCount(6, historicQuery().EndedAfter(created));
//		AssertCount(0, historicQuery().EndedAfter(afterEnd));

//		AssertCount(0, historicQuery().EndedBefore(created));
//		AssertCount(6, historicQuery().EndedBefore(afterEnd));

//		AssertCount(0, historicQuery().EndedBefore(created).EndedAfter(afterEnd));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/api/cmmn/oneTaskCaseWithManualActivation.cmmn"}) public void testHistoricCaseActivityTaskId()
//	  public virtual void testHistoricCaseActivityTaskId()
//	  {
//		string taskId = "PI_HumanTask_1";

//		createCaseInstance();

//		// as long as the human task was not started there should be no task id set
//		AssertCount(0, taskService.CreateTaskQuery());
//		IHistoricCaseActivityInstance historicInstance = queryHistoricActivityCaseInstance(taskId);
//		Assert.IsNull(historicInstance.TaskId);

//		// start human task manually to create task instance
//		ICaseExecution humanTask = queryCaseExecutionByActivityId(taskId);
//		manualStart(humanTask.Id);

//		// there should exist a single task
//		ITask task = taskService.CreateTaskQuery().First();
//		Assert.NotNull(task);

//		// check that the task id was correctly set
//		historicInstance = queryHistoricActivityCaseInstance(taskId);
//		Assert.AreEqual(task.Id, historicInstance.TaskId);

//		// complete task
//		taskService.Complete(task.Id);

//		// check that the task id is still set
//		AssertCount(0, taskService.CreateTaskQuery());
//		historicInstance = queryHistoricActivityCaseInstance(taskId);
//		Assert.AreEqual(task.Id, historicInstance.TaskId);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[]{ "resources/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn", "resources/api/oneTaskProcess.bpmn20.xml" }) public void testHistoricCaseActivityCalledProcessInstanceId()
//	  public virtual void testHistoricCaseActivityCalledProcessInstanceId()
//	  {
//		string taskId = "PI_ProcessTask_1";

//		createCaseInstanceByKey("oneProcessTaskCase").Id;

//		// as long as the process task is not activated there should be no process instance
//		AssertCount(0, runtimeService.CreateProcessInstanceQuery());

//		IHistoricCaseActivityInstance historicInstance = queryHistoricActivityCaseInstance(taskId);
//		Assert.IsNull(historicInstance.CalledProcessInstanceId);

//		// start process task manually to create case instance
//		ICaseExecution processTask = queryCaseExecutionByActivityId(taskId);
//		manualStart(processTask.Id);

//		// there should exist a new process instance
//		IProcessInstance calledProcessInstance = runtimeService.CreateProcessInstanceQuery().First();
//		Assert.NotNull(calledProcessInstance);

//		// check that the called process instance id was correctly set
//		historicInstance = queryHistoricActivityCaseInstance(taskId);
//		Assert.AreEqual(calledProcessInstance.Id, historicInstance.CalledProcessInstanceId);

//		// complete task and thereby the process instance
//		ITask task = taskService.CreateTaskQuery().First();
//		taskService.Complete(task.Id);

//		// check that the task id is still set
//		AssertCount(0, runtimeService.CreateProcessInstanceQuery());
//		historicInstance = queryHistoricActivityCaseInstance(taskId);
//		Assert.AreEqual(calledProcessInstance.Id, historicInstance.CalledProcessInstanceId);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn", "resources/api/cmmn/oneTaskCaseWithManualActivation.cmmn" }) public void testHistoricCaseActivityCalledCaseInstanceId()
//	  public virtual void testHistoricCaseActivityCalledCaseInstanceId()
//	  {
//		string taskId = "PI_CaseTask_1";

//		string calledCaseId = "oneTaskCase";
//		string calledTaskId = "PI_HumanTask_1";

//		createCaseInstanceByKey("oneCaseTaskCase").Id;

//		// as long as the case task is not activated there should be no other case instance
//		AssertCount(0, caseService.CreateCaseInstanceQuery().CaseDefinitionKey(calledCaseId));

//		IHistoricCaseActivityInstance historicInstance = queryHistoricActivityCaseInstance(taskId);
//		Assert.IsNull(historicInstance.CalledCaseInstanceId);

//		// start case task manually to create case instance
//		ICaseExecution caseTask = queryCaseExecutionByActivityId(taskId);
//		manualStart(caseTask.Id);

//		// there should exist a new case instance
//		ICaseInstance calledCaseInstance = caseService.CreateCaseInstanceQuery().CaseDefinitionKey(calledCaseId).First();
//		Assert.NotNull(calledCaseInstance);

//		// check that the called case instance id was correctly set
//		historicInstance = queryHistoricActivityCaseInstance(taskId);
//		Assert.AreEqual(calledCaseInstance.Id, historicInstance.CalledCaseInstanceId);

//		// disable task to complete called case instance and close it
//		ICaseExecution calledTask = queryCaseExecutionByActivityId(calledTaskId);
//		disable(calledTask.Id);
//		close(calledCaseInstance.Id);

//		// check that the called case instance id is still set
//		AssertCount(0, caseService.CreateCaseInstanceQuery().CaseDefinitionKey(calledCaseId));
//		historicInstance = queryHistoricActivityCaseInstance(taskId);
//		Assert.AreEqual(calledCaseInstance.Id, historicInstance.CalledCaseInstanceId);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/api/cmmn/oneTaskAndOneStageWithManualActivationCase.cmmn"}) public void testHistoricCaseActivityQuery()
//	  public virtual void testHistoricCaseActivityQuery()
//	  {
//		string stageId = "PI_Stage_1";
//		string stageName = "A HumanTask";
//		string taskId = "PI_HumanTask_1";
//		string taskName = "A HumanTask";

//		string caseInstanceId = createCaseInstance().Id;

//		ICaseExecution stageExecution = queryCaseExecutionByActivityId(stageId);
//		ICaseExecution taskExecution = queryCaseExecutionByActivityId(taskId);

//		AssertCount(1, historicQuery().CaseActivityInstanceId(stageExecution.Id));
//		AssertCount(1, historicQuery().CaseActivityInstanceId(taskExecution.Id));

//		AssertCount(2, historicQuery(c=>c.CaseInstanceId ==caseInstanceId));
//		AssertCount(2, historicQuery(c=>c.CaseDefinitionId== stageExecution.CaseDefinitionId));

//		AssertCount(1, historicQuery().CaseExecutionId(stageExecution.Id));
//		AssertCount(1, historicQuery().CaseExecutionId(taskExecution.Id));

//		AssertCount(1, historicQuery(c=>c.CaseActivityId== stageId));
//		AssertCount(1, historicQuery(c=>c.CaseActivityId== taskId));

//		AssertCount(1, historicQuery().CaseActivityName(stageName));
//		AssertCount(1, historicQuery().CaseActivityName(taskName));

//		AssertCount(1, historicQuery().CaseActivityType("stage"));
//		AssertCount(1, historicQuery().CaseActivityType("humanTask"));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/api/cmmn/oneTaskCase.cmmn"}) public void testQueryPaging()
//	  public virtual void testQueryPaging()
//	  {
//		createCaseInstance();
//		createCaseInstance();
//		createCaseInstance();
//		createCaseInstance();

//		Assert.AreEqual(3, historicQuery().ListPage(0, 3).Count);
//		Assert.AreEqual(2, historicQuery().ListPage(2, 2).Count);
//		Assert.AreEqual(1, historicQuery().ListPage(3, 2).Count);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneTaskCase.cmmn", "resources/api/cmmn/twoTaskCase.cmmn" }) public void testQuerySorting()
//	  public virtual void testQuerySorting()
//	  {
//		string taskId1 = "PI_HumanTask_1";
//		string taskId2 = "PI_HumanTask_2";

//		string oneTaskCaseId = createCaseInstanceByKey("oneTaskCase").Id;
//		string twoTaskCaseId = createCaseInstanceByKey("twoTaskCase").Id;

//		ICaseExecution task1 = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==oneTaskCaseId&& c.ActivityId ==taskId1).First();
//		ICaseExecution task2 = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==twoTaskCaseId&& c.ActivityId ==taskId1).First();
//		ICaseExecution task3 = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==twoTaskCaseId&& c.ActivityId ==taskId2).First();

//		// sort by historic case activity instance ids
//		AssertQuerySorting("id", historicQuery().OrderByHistoricCaseActivityInstanceId(), task1.Id, task2.Id, task3.Id);

//		// sort by case instance ids
//		AssertQuerySorting("caseInstanceId", historicQuery()//.OrderByCaseInstanceId(), oneTaskCaseId, twoTaskCaseId, twoTaskCaseId);

//		// sort by case execution ids
//		AssertQuerySorting("caseExecutionId", historicQuery()//.OrderByCaseExecutionId(), task1.Id, task2.Id, task3.Id);

//		// sort by case activity ids
//		AssertQuerySorting("caseActivityId", historicQuery().OrderByCaseActivityId(), taskId1, taskId1, taskId2);

//		// sort by case activity names
//		AssertQuerySorting("caseActivityName", historicQuery().OrderByCaseActivityName(), "A HumanTask", "A HumanTask", "Another HumanTask");

//		// sort by case definition ids
//		AssertQuerySorting("caseDefinitionId", historicQuery()//.OrderByCaseDefinitionId(), task1.CaseDefinitionId, task2.CaseDefinitionId, task3.CaseDefinitionId);

//		// manually start tasks to be able to complete them
//		manualStart(task2.Id);
//		manualStart(task3.Id);

//		// complete tasks to set end time and duration
//		foreach (ITask task in taskService.CreateTaskQuery())
//		{
//		  taskService.Complete(task.Id);
//		}

//		HistoricCaseActivityInstanceQuery query = historyService.CreateHistoricCaseActivityInstanceQuery();
//		IHistoricCaseActivityInstance historicTask1 = query.CaseInstanceId(oneTaskCaseId).CaseActivityId(taskId1).First();
//		IHistoricCaseActivityInstance historicTask2 = query.CaseInstanceId(twoTaskCaseId).CaseActivityId(taskId1).First();
//		IHistoricCaseActivityInstance historicTask3 = query.CaseInstanceId(twoTaskCaseId).CaseActivityId(taskId2).First();

//		// sort by create times
//		AssertQuerySorting("createTime", historicQuery().OrderByHistoricCaseActivityInstanceCreateTime(), historicTask1.CreateTime, historicTask2.CreateTime, historicTask3.CreateTime);

//		// sort by end times
//		AssertQuerySorting("endTime", historicQuery().OrderByHistoricCaseActivityInstanceEndTime(), historicTask1.EndTime, historicTask2.EndTime, historicTask3.EndTime);

//		// sort by durations times
//		AssertQuerySorting("durationInMillis", historicQuery().OrderByHistoricCaseActivityInstanceDuration(), historicTask1.DurationInMillis, historicTask2.DurationInMillis, historicTask3.DurationInMillis);
//	  }

//////
//[Test]
//[Deployment]
//	  public virtual void testQuerySortingCaseActivityType()
//	  {
//		createCaseInstance().Id;

//		// sort by case activity type
//		AssertQuerySorting("caseActivityType", historicQuery().OrderByCaseActivityType(), "milestone", "processTask", "humanTask");
//	  }

//	  public virtual void testInvalidSorting()
//	  {
//		try
//		{
//		  historicQuery()/*.Asc()*/;
//		  Assert.Fail("Exception expected");
//		}
//		catch (ProcessEngineException)
//		{
//		  // expected
//		}

//		try
//		{
//		  historicQuery()/*.Desc()*/;
//		  Assert.Fail("Exception expected");
//		}
//		catch (ProcessEngineException)
//		{
//		  // expected
//		}

//		try
//		{
//		  historicQuery().OrderByHistoricCaseActivityInstanceId().Count();
//		  Assert.Fail("Exception expected");
//		}
//		catch (ProcessEngineException)
//		{
//		  // expected
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/api/cmmn/oneTaskCase.cmmn"}) public void testNativeQuery()
//	  public virtual void testNativeQuery()
//	  {
//		createCaseInstance();
//		createCaseInstance();
//		createCaseInstance();
//		createCaseInstance();

//		string instanceId = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1")[0].Id;

//		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
//		string tableName = managementService.GetTableName(typeof(HistoricCaseActivityInstance));

//		Assert.AreEqual(tablePrefix + "ACT_HI_CASEACTINST", tableName);
//		Assert.AreEqual(tableName, managementService.GetTableName(typeof(HistoricCaseActivityInstanceEntity)));

//		Assert.AreEqual(4, historyService.CreateNativeHistoricCaseActivityInstanceQuery().Sql("SELECT * FROM " + tableName).Count());
//		Assert.AreEqual(4, historyService.CreateNativeHistoricCaseActivityInstanceQuery().Sql("SELECT Count(*) FROM " + tableName).Count());

//		Assert.AreEqual(16, historyService.CreateNativeHistoricCaseActivityInstanceQuery().Sql("SELECT Count(*) FROM " + tableName + " H1, " + tableName + " H2").Count());

//		// select with distinct
//		Assert.AreEqual(4, historyService.CreateNativeHistoricCaseActivityInstanceQuery().Sql("SELECT DISTINCT * FROM " + tableName).Count());

//		Assert.AreEqual(1, historyService.CreateNativeHistoricCaseActivityInstanceQuery().Sql("SELECT Count(*) FROM " + tableName + " H WHERE H.ID_ = '" + instanceId + "'").Count());
//		Assert.AreEqual(1, historyService.CreateNativeHistoricCaseActivityInstanceQuery().Sql("SELECT * FROM " + tableName + " H WHERE H.ID_ = '" + instanceId + "'").Count());

//		// use parameters
//		Assert.AreEqual(1, historyService.CreateNativeHistoricCaseActivityInstanceQuery().Sql("SELECT Count(*) FROM " + tableName + " H WHERE H.ID_ = #{caseActivityInstanceId}").Parameter("caseActivityInstanceId", instanceId).Count());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/api/cmmn/oneTaskCase.cmmn"}) public void testNativeQueryPaging()
//	  public virtual void testNativeQueryPaging()
//	  {
//		createCaseInstance();
//		createCaseInstance();
//		createCaseInstance();
//		createCaseInstance();

//		string tableName = managementService.GetTableName(typeof(HistoricCaseActivityInstance));
//		Assert.AreEqual(3, historyService.CreateNativeHistoricCaseActivityInstanceQuery().Sql("SELECT * FROM " + tableName).ListPage(0, 3).Count);
//		Assert.AreEqual(2, historyService.CreateNativeHistoricCaseActivityInstanceQuery().Sql("SELECT * FROM " + tableName).ListPage(2, 2).Count);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/api/cmmn/oneTaskCaseWithManualActivation.cmmn"}) public void testDeleteHistoricCaseActivityInstance()
//	  public virtual void testDeleteHistoricCaseActivityInstance()
//	  {
//		ICaseInstance caseInstance = createCaseInstance();

//		IHistoricCaseActivityInstance historicInstance = historicQuery().First();
//		Assert.NotNull(historicInstance);

//		// disable human task to complete case
//		disable(historicInstance.Id);
//		// close case to be able to Delete historic case instance
//		close(caseInstance.Id);
//		// Delete historic case instance
//		historyService.DeleteHistoricCaseInstance(caseInstance.Id);

//		AssertCount(0, historicQuery());
//	  }

//[Test]
//[Deployment]
//	  public virtual void testNonBlockingHumanTask()
//	  {
//		ICaseInstance caseInstance = createCaseInstance();
//		Assert.NotNull(caseInstance);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] "resources/cmmn/required/RequiredRuleTest.TestVariableBasedRule.cmmn") public void testRequiredRuleEvaluatesToTrue()
//	  public virtual void testRequiredRuleEvaluatesToTrue()
//	  {
//		caseService.CreateCaseInstanceByKey("case", new Dictionary<string, ITypedValue>(){{"required", true));

//		IHistoricCaseActivityInstance task = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_HumanTask_1").First();

//		Assert.NotNull(task);
//		Assert.True(task.Required);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] "resources/cmmn/required/RequiredRuleTest.TestVariableBasedRule.cmmn") public void testRequiredRuleEvaluatesToFalse()
//	  public virtual void testRequiredRuleEvaluatesToFalse()
//	  {
//		caseService.CreateCaseInstanceByKey("case", new Dictionary<string, ITypedValue>(){{"required", false));

//		IHistoricCaseActivityInstance task = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_HumanTask_1").First();

//		Assert.NotNull(task);
//		Assert.IsFalse(task.Required);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] "resources/cmmn/required/RequiredRuleTest.TestVariableBasedRule.cmmn") public void testQueryByRequired()
//	  public virtual void testQueryByRequired()
//	  {
//		caseService.CreateCaseInstanceByKey("case", new Dictionary<string, ITypedValue>(){{"required", true));

//		HistoricCaseActivityInstanceQuery query = historyService.CreateHistoricCaseActivityInstanceQuery().Required();

//		Assert.AreEqual(1, query.Count());
//		Assert.AreEqual(1, query.Count());

//		IHistoricCaseActivityInstance activityInstance = query.First();
//		Assert.NotNull(activityInstance);
//		Assert.True(activityInstance.Required);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/cmmn/stage/AutoCompleteTest.TestCasePlanModel.cmmn"}) public void testAutoCompleteEnabled()
//	  public virtual void testAutoCompleteEnabled()
//	  {
//		string caseInstanceId = createCaseInstanceByKey("case").Id;

//		IHistoricCaseInstance caseInstance = historyService.CreateHistoricCaseInstanceQuery(c=>c.CaseInstanceId ==caseInstanceId).First();
//		Assert.NotNull(caseInstance);
//		Assert.True(caseInstance.Completed);

//		HistoricCaseActivityInstanceQuery query = historyService.CreateHistoricCaseActivityInstanceQuery();

//		IHistoricCaseActivityInstance humanTask1 = query.CaseActivityId("PI_HumanTask_1").First();
//		Assert.NotNull(humanTask1);
//		Assert.True(humanTask1.Terminated);
//		Assert.NotNull(humanTask1.EndTime);
//		Assert.NotNull(humanTask1.DurationInMillis);


//		IHistoricCaseActivityInstance humanTask2 = query.CaseActivityId("PI_HumanTask_2").First();
//		Assert.NotNull(humanTask2);
//		Assert.True(humanTask2.Terminated);
//		Assert.NotNull(humanTask2.EndTime);
//		Assert.NotNull(humanTask2.DurationInMillis);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/cmmn/repetition/RepetitionRuleTest.TestRepeatTask.cmmn"}) public void testRepeatTask()
//	  public virtual void testRepeatTask()
//	  {
//		// given
//		createCaseInstance();
//		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

//		// when
//		complete(firstHumanTaskId);

//		// then
//		HistoricCaseActivityInstanceQuery query = historicQuery(c=>c.CaseActivityId== "PI_HumanTask_2");
//		Assert.AreEqual(2, query.Count());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/cmmn/repetition/RepetitionRuleTest.TestRepeatStage.cmmn"}) public void testRepeatStage()
//	  public virtual void testRepeatStage()
//	  {
//		// given
//		createCaseInstance();

//		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

//		// when
//		complete(firstHumanTaskId);

//		// then
//		HistoricCaseActivityInstanceQuery query = historicQuery(c=>c.CaseActivityId== "PI_Stage_1");
//		Assert.AreEqual(2, query.Count());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/cmmn/repetition/RepetitionRuleTest.TestRepeatMilestone.cmmn"}) public void testRepeatMilestone()
//	  public virtual void testRepeatMilestone()
//	  {
//		// given
//		createCaseInstance();
//		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

//		// when
//		complete(firstHumanTaskId);

//		// then
//		HistoricCaseActivityInstanceQuery query = historicQuery(c=>c.CaseActivityId== "PI_Milestone_1");
//		Assert.AreEqual(2, query.Count());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/cmmn/repetition/RepetitionRuleTest.TestAutoCompleteStage.cmmn"}) public void testAutoCompleteStage()
//	  public virtual void testAutoCompleteStage()
//	  {
//		// given
//		createCaseInstance();
//		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

//		// when
//		complete(humanTask1);

//		// then
//		HistoricCaseActivityInstanceQuery query = historicQuery(c=>c.CaseActivityId== "PI_Stage_1");
//		Assert.AreEqual(1, query.Count());

//		query = historicQuery(c=>c.CaseActivityId== "PI_HumanTask_1");
//		Assert.AreEqual(1, query.Count());

//		query = historicQuery(c=>c.CaseActivityId== "PI_HumanTask_2");
//		Assert.AreEqual(2, query.Count());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/cmmn/repetition/RepetitionRuleTest.TestAutoCompleteStageWithoutEntryCriteria.cmmn"}) public void testAutoCompleteStageWithRepeatableTaskWithoutEntryCriteria()
//	  public virtual void testAutoCompleteStageWithRepeatableTaskWithoutEntryCriteria()
//	  {
//		// given
//		createCaseInstanceByKey("case", Variable.Variables.CreateVariables().PutValue("manualActivation", false));
//		queryCaseExecutionByActivityId("PI_Stage_1");

//		// when
//		string humanTask = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;
//		complete(humanTask);

//		// then
//		HistoricCaseActivityInstanceQuery query = historicQuery(c=>c.CaseActivityId== "PI_HumanTask_1");
//		Assert.AreEqual(2, query.Count());

//		query = historicQuery(c=>c.CaseActivityId== "PI_Stage_1");
//		Assert.AreEqual(1, query.Count());

//	  }


//[Test]
//[Deployment]
//	  public virtual void testDecisionTask()
//	  {
//		createCaseInstance();

//		IHistoricCaseActivityInstance decisionTask = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_DecisionTask_1").First();

//		Assert.NotNull(decisionTask);
//		Assert.AreEqual("decisionTask", decisionTask.CaseActivityType);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/api/cmmn/oneTaskCase.cmmn"}) public void testQueryByCaseInstanceId()
//	  public virtual void testQueryByCaseInstanceId()
//	  {
//		// given
//		createCaseInstance();

//		string taskInstanceId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

//		// when
//		HistoricCaseActivityInstanceQuery query = historicQuery().CaseActivityInstanceIdIn(taskInstanceId);

//		// then
//		AssertCount(1, query);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/api/cmmn/oneTaskCase.cmmn"}) public void testQueryByCaseInstanceIds()
//	  public virtual void testQueryByCaseInstanceIds()
//	  {
//		// given
//		ICaseInstance instance1 = createCaseInstance();
//		ICaseInstance instance2 = createCaseInstance();

//		string taskInstanceId1 = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==instance1.Id&& c.ActivityId =="PI_HumanTask_1").First().Id;

//		string taskInstanceId2 = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==instance2.Id&& c.ActivityId =="PI_HumanTask_1").First().Id;

//		// when
//		HistoricCaseActivityInstanceQuery query = historicQuery().CaseActivityInstanceIdIn(taskInstanceId1, taskInstanceId2);

//		// then
//		AssertCount(2, query);
//	  }

//	  public virtual void testQueryByInvalidCaseInstanceId()
//	  {

//		// when
//		HistoricCaseActivityInstanceQuery query = historicQuery().CaseActivityInstanceIdIn("invalid");

//		// then
//		AssertCount(0, query);

//		try
//		{
//		  historicQuery().CaseActivityInstanceIdIn((string[])null);
//		  Assert.Fail("A NotValidException was expected.");
//		}
//		catch (NotValidException)
//		{
//		}

//		try
//		{
//		  historicQuery().CaseActivityInstanceIdIn((string)null);
//		  Assert.Fail("A NotValidException was expected.");
//		}
//		catch (NotValidException)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneTaskCase.cmmn", "resources/api/cmmn/twoTaskCase.cmmn" }) public void testQueryByCaseActivityIds()
//	  public virtual void testQueryByCaseActivityIds()
//	  {
//		// given
//		createCaseInstanceByKey("oneTaskCase");
//		createCaseInstanceByKey("twoTaskCase");

//		// when
//		HistoricCaseActivityInstanceQuery query = historicQuery().CaseActivityIdIn("PI_HumanTask_1", "PI_HumanTask_2");

//		// then
//		AssertCount(3, query);
//	  }

//	  public virtual void testQueryByInvalidCaseActivityId()
//	  {

//		// when
//		HistoricCaseActivityInstanceQuery query = historicQuery().CaseActivityIdIn("invalid");

//		// then
//		AssertCount(0, query);

//		try
//		{
//		  historicQuery().CaseActivityIdIn((string[])null);
//		  Assert.Fail("A NotValidException was expected.");
//		}
//		catch (NotValidException)
//		{
//		}

//		try
//		{
//		  historicQuery().CaseActivityIdIn((string)null);
//		  Assert.Fail("A NotValidException was expected.");
//		}
//		catch (NotValidException)
//		{
//		}
//	  }

//	  protected internal virtual HistoricCaseActivityInstanceQuery historicQuery()
//	  {
//		return historyService.CreateHistoricCaseActivityInstanceQuery();
//	  }

//	  protected internal virtual IHistoricCaseActivityInstance queryHistoricActivityCaseInstance(string activityId)
//	  {
//		IHistoricCaseActivityInstance historicActivityInstance = historicQuery(c=>c.CaseActivityId== activityId).First();
//		Assert.NotNull("No historic activity instance found for activity id: " + activityId, historicActivityInstance);
//		return historicActivityInstance;
//	  }

//	  protected internal virtual void AssertHistoricState(string activityId, CaseExecutionState expectedState)
//	  {
//		HistoricCaseActivityInstanceEventEntity historicActivityInstance = (HistoricCaseActivityInstanceEventEntity) queryHistoricActivityCaseInstance(activityId);
//		int actualStateCode = historicActivityInstance.CaseActivityInstanceState;
//		CaseExecutionState actualState = CaseExecutionState.CaseExecutionStateImpl.GetStateForCode(actualStateCode);
//		Assert.AreEqual("The state of historic case activity '" + activityId + "' wasn't as expected", expectedState, actualState);
//	  }

//	  protected internal virtual void AssertHistoricCreateTime(string activityId, DateTime expectedCreateTime)
//	  {
//		IHistoricCaseActivityInstance historicActivityInstance = queryHistoricActivityCaseInstance(activityId);
//		DateTime actualCreateTime = historicActivityInstance.CreateTime;
//		AssertSimilarDate(expectedCreateTime, actualCreateTime);
//	  }

//	  protected internal virtual void AssertHistoricEndTime(string activityId, DateTime expectedEndTime)
//	  {
//		IHistoricCaseActivityInstance historicActivityInstance = queryHistoricActivityCaseInstance(activityId);
//		DateTime actualEndTime = historicActivityInstance.EndTime;
//		AssertSimilarDate(expectedEndTime, actualEndTime);
//	  }

//	  protected internal virtual void AssertSimilarDate(DateTime expectedDate, DateTime actualDate)
//	  {
//		long difference = Math.Abs(expectedDate.Ticks - actualDate.Ticks);
//		// Assert that the dates don't differ more than a second
//		Assert.True(difference < 1000);
//	  }

//	  protected internal virtual void AssertHistoricDuration(string activityId, long expectedDuration)
//	  {
//		long? actualDuration = queryHistoricActivityCaseInstance(activityId).DurationInMillis;
//		Assert.NotNull(actualDuration);
//		// test that duration is as expected with a maximal difference of one second
//		Assert.True(actualDuration >= expectedDuration);
//		Assert.True(actualDuration < expectedDuration + 1000);
//	  }

//	  protected internal virtual void AssertCount<T1>(long Count, Query<T1> historicQuery)
//	  {
//		Assert.AreEqual(Count, historicQuery.Count());
//	  }

//	  protected internal virtual void AssertStateQuery(params CaseExecutionState[] states)
//	  {
//		CaseExecutionStateCountMap stateCounts = new CaseExecutionStateCountMap(this);

//		if (states != null)
//		{
//		  foreach (CaseExecutionState state in states)
//		  {
//			stateCounts[state] = stateCounts[state] + 1;
//		  }
//		}

//		AssertCount(stateCounts.Count(), historicQuery());
//		AssertCount(stateCounts/*.Unfinished()*/.Value, historicQuery().NotEnded());
//		AssertCount(stateCounts.Finished().Value, historicQuery().Ended());

//		AssertCount(stateCounts[ACTIVE], historicQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode));
//		AssertCount(stateCounts[AVAILABLE], historicQuery().available());
//		AssertCount(stateCounts[COMPLETED], historicQuery().Completed());
//		AssertCount(stateCounts[DISABLED], historicQuery().Disabled());
//		AssertCount(stateCounts[ENABLED], historicQuery().Enabled());
//		AssertCount(stateCounts[TERMINATED], historicQuery().Terminated());
//	  }

//	  protected internal class CaseExecutionStateCountMap : Dictionary<CaseExecutionState, long?>
//	  {
//		  private readonly HistoricCaseActivityInstanceTest outerInstance;


//		internal const long serialVersionUID = 1L;

//		public readonly ICollection<CaseExecutionState> ALL_STATES = CaseExecutionState.CASE_EXECUTION_STATES.values();
//		public readonly ICollection<CaseExecutionState> ENDED_STATES = COMPLETED, TERMINATED;
//		public readonly ICollection<CaseExecutionState> NOT_ENDED_STATES;

//		public CaseExecutionStateCountMap(HistoricCaseActivityInstanceTest outerInstance)
//		{
//			this.outerInstance = outerInstance;
//		  NOT_ENDED_STATES = new List<CaseExecutionState>(ALL_STATES);
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
//		  NOT_ENDED_STATES.RemoveAll(ENDED_STATES);
//		}

//		public virtual long? get(CaseExecutionState state)
//		{
//		  return state != null && this.ContainsKey(state) ? base[state] : 0;
//		}

//		public virtual long? Count()
//		{
//		  return Count(ALL_STATES);
//		}

//		public virtual long? finished()
//		{
//		  return Count(ENDED_STATES);
//		}

//		public virtual long? unfinished()
//		{
//		  return Count(NOT_ENDED_STATES);
//		}

//		public virtual long? Count(ICollection<CaseExecutionState> states)
//		{
//		  long Count = 0;
//		  foreach (CaseExecutionState state in states)
//		  {
//			Count += this[state];
//		  }
//		  return Count;
//		}

//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) protected void AssertQuerySorting(String property, org.Camunda.bpm.Engine.query.Query<?, ?> query, Comparable.. items)
//	  protected internal virtual void AssertQuerySorting<T1>(string property, Query<T1> query, params IComparable[] items)
//	  {
////JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
////ORIGINAL LINE: org.Camunda.bpm.Engine.impl.AbstractQuery<?, ?> queryImpl = (org.Camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query;
//		AbstractQuery<object, object> queryImpl = (AbstractQuery<object, object>) query;

//		// save order properties to later reverse ordering
//		IList<QueryOrderingProperty> orderProperties = queryImpl.OrderingProperties;

////JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
////ORIGINAL LINE: List<? extends Comparable> sortedList = (items);
//		IList<IComparable> sortedList = items;
//		sortedList.Sort();

//		IList<Matcher<object>> matchers = new List<Matcher<object>>();
//		foreach (IComparable comparable in sortedList)
//		{
//		  matchers.Add(hasProperty(property, equalTo(comparable)));
//		}

////JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
////ORIGINAL LINE: List<?> instances = query/*.Asc()*/.ToList();
//		IList<object> instances = query/*.Asc()*/.ToList();
//		Assert.AreEqual(sortedList.Count, instances.Count);
//		Assert.That(instances, contains(matchers.ToArray()));

//		// reverse ordering
//		foreach (QueryOrderingProperty orderingProperty in orderProperties)
//		{
//		  orderingProperty.Direction = Direction.DESCENDING;
//		}

//		// reverse matchers
//		matchers.Reverse();

//		instances = query.ToList();
//		Assert.AreEqual(sortedList.Count, instances.Count);
//		Assert.That(instances, contains(matchers.ToArray()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricCaseActivityInstanceTest.oneStageAndOneTaskCaseWithManualActivation.cmmn"}) public void testHistoricActivityInstanceWithinStageIsMarkedTerminatedOnComplete()
//	  public virtual void testHistoricActivityInstanceWithinStageIsMarkedTerminatedOnComplete()
//	  {

//		// given
//		createCaseInstance();

//		string stageExecutionId = queryCaseExecutionByActivityId("PI_Stage_1").Id;
//		manualStart(stageExecutionId);
//		string activeStageTaskExecutionId = queryCaseExecutionByActivityId("PI_HumanTask_Stage_2").Id;
//		complete(activeStageTaskExecutionId);
//		ICaseExecution enabledStageTaskExecutionId = queryCaseExecutionByActivityId("PI_HumanTask_Stage_1");
//		Assert.True(enabledStageTaskExecutionId.Enabled);

//		// when
//		complete(stageExecutionId);

//		// then the remaining stage task that was enabled is set to terminated in history
//		IHistoricCaseActivityInstance manualActivationTask = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_HumanTask_Stage_1").First();
//		IHistoricCaseActivityInstance completedTask = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_HumanTask_Stage_2").First();

//		Assert.True(manualActivationTask.Terminated);
//		Assert.True(completedTask.Completed);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricCaseActivityInstanceTest.oneStageAndOneTaskCaseWithManualActivation.cmmn"}) public void testHistoricActivityInstancesAreMarkedTerminatedOnComplete()
//	  public virtual void testHistoricActivityInstancesAreMarkedTerminatedOnComplete()
//	  {

//		// given
//		createCaseInstance();

//		ICaseExecution humanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
//		Assert.True(humanTask.Enabled);
//		ICaseExecution stage = queryCaseExecutionByActivityId("PI_Stage_1");
//		Assert.True(stage.Enabled);

//		// when
//		ICaseExecution casePlanExecution = queryCaseExecutionByActivityId("CasePlanModel_1");
//		complete(casePlanExecution.Id);

//		// then make sure all cases in the lower scope are marked as terminated in history
//		IHistoricCaseActivityInstance stageInstance = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_Stage_1").First();
//		IHistoricCaseActivityInstance taskInstance = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_HumanTask_3").First();

//		Assert.True(stageInstance.Terminated);
//		Assert.True(taskInstance.Terminated);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricCaseActivityInstanceTest.oneStageAndOneTaskCaseWithManualActivation.cmmn"}) public void testDisabledHistoricActivityInstancesStayDisabledOnComplete()
//	  public virtual void testDisabledHistoricActivityInstancesStayDisabledOnComplete()
//	  {

//		// given
//		createCaseInstance();

//		ICaseExecution humanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
//		Assert.True(humanTask.Enabled);
//		ICaseExecution stageExecution = queryCaseExecutionByActivityId("PI_Stage_1");
//		disable(stageExecution.Id);
//		stageExecution = queryCaseExecutionByActivityId("PI_Stage_1");
//		Assert.True(stageExecution.Disabled);

//		// when
//		ICaseExecution casePlanExecution = queryCaseExecutionByActivityId("CasePlanModel_1");
//		complete(casePlanExecution.Id);

//		// then make sure disabled executions stay disabled
//		IHistoricCaseActivityInstance stageInstance = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_Stage_1").First();
//		IHistoricCaseActivityInstance taskInstance = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_HumanTask_3").First();

//		Assert.True(stageInstance.Disabled);
//		Assert.True(taskInstance.Terminated);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @IDeployment public void testMilestoneHistoricActivityInstanceIsTerminatedOnComplete()
//	  public virtual void testMilestoneHistoricActivityInstanceIsTerminatedOnComplete()
//	  {

//		// given
//		createCaseInstance();
//		const string milestoneId = "PI_Milestone_1";
//		ICaseExecution caseMilestone = queryCaseExecutionByActivityId(milestoneId);
//		Assert.True(caseMilestone.Available);

//		// when
//		ICaseExecution casePlanExecution = queryCaseExecutionByActivityId("CasePlanModel_1");
//		complete(casePlanExecution.Id);

//		// then make sure that the milestone is terminated
//		IHistoricCaseActivityInstance milestoneInstance = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== milestoneId).First();

//		Assert.True(milestoneInstance.Terminated);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricCaseActivityInstanceTest.oneStageWithSentryAsEntryPointCase.cmmn"}) public void testHistoricTaskWithSentryIsMarkedTerminatedOnComplete()
//	  public virtual void testHistoricTaskWithSentryIsMarkedTerminatedOnComplete()
//	  {

//		// given
//		createCaseInstance();

//		// when
//		ICaseExecution casePlanExecution = queryCaseExecutionByActivityId("PI_Stage_1");
//		complete(casePlanExecution.Id);

//		// then both tasks are terminated
//		IHistoricCaseActivityInstance taskInstance = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_HumanTask_1").First();

//		IHistoricCaseActivityInstance taskInstance2 = historyService.CreateHistoricCaseActivityInstanceQuery(c=>c.CaseActivityId== "PI_HumanTask_2").First();

//		Assert.True(taskInstance.Terminated);
//		Assert.True(taskInstance2.Terminated);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricCaseActivityInstanceTest.oneStageWithSentryAsEntryPointCase.cmmn"}) public void testHistoricTaskWithSentryDoesNotReachStateActiveOnComplete()
//	  public virtual void testHistoricTaskWithSentryDoesNotReachStateActiveOnComplete()
//	  {

//		// given
//		createCaseInstance();

//		// when
//		ICaseExecution casePlanExecution = queryCaseExecutionByActivityId("PI_Stage_1");
//		complete(casePlanExecution.Id);

//		// then task 2 was never in state 'active'
//		IQueryable<IVariableInstance> query = runtimeService.CreateVariableInstanceQuery().CaseExecutionIdIn(casePlanExecution.Id);

//		Assert.AreEqual(0, query.Count());
//	  }

//	}

//}