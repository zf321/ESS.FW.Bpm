//using System;
//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.Impl.JobExecutor;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Api.Runtime;
//using ESS.FW.Bpm.Engine.Variable;
//using NUnit.Framework;
//using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;


//namespace ESS.FW.Bpm.Engine.Tests.History
//{




//    /// <summary>
//	/// 
//	/// 
//	/// </summary>
//[RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//	public class HistoricJobLogQueryTest : PluggableProcessEngineTestCase
//	{
//[Test]
//[Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
//		public virtual void testQuery()
//		{
//		runtimeService.StartProcessInstanceByKey("process");
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

//		//verifyQueryResults(query, 1);
//		}

//[Test]
//[Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
//	  public virtual void testQueryByLogId()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");
//		string logId = historyService.CreateHistoricJobLogQuery().First().Id;

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().LogId(logId);

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidLogId()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().LogId("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.LogId(null);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByJobId()
//	  public virtual void testQueryByJobId()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");
//		string jobId = managementService.CreateJobQuery().First().Id;

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.Id ==jobId);

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidJobId()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.Id =="invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.JobId(null);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByJobExceptionMessage()
//	  public virtual void testQueryByJobExceptionMessage()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");
//		string jobId = managementService.CreateJobQuery().First().Id;
//		try
//		{
//		  managementService.ExecuteJob(jobId);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		  // expected
//		}

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().JobExceptionMessage(FailingDelegate.EXCEPTION_MESSAGE);

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidJobExceptionMessage()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().JobExceptionMessage("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.JobExceptionMessage(null);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByJobDefinitionId()
//	  public virtual void testQueryByJobDefinitionId()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");
//		string jobDefinitionId = managementService.CreateJobQuery().First().JobDefinitionId;

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.Id==jobDefinitionId);

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidJobDefinitionId()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.Id=="invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.JobDefinitionId(null);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByJobDefinitionType()
//	  public virtual void testQueryByJobDefinitionType()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().JobDefinitionType(AsyncContinuationJobHandler.TYPE);

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidJobDefinitionType()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().JobDefinitionType("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.JobDefinitionType(null);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByJobDefinitionConfiguration()
//	  public virtual void testQueryByJobDefinitionConfiguration()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().JobDefinitionConfiguration(MessageJobDeclaration.AsyncBefore);

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidJobDefinitionConfiguration()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().JobDefinitionConfiguration("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.JobDefinitionConfiguration(null);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByActivityId()
//	  public virtual void testQueryByActivityId()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.ActivityId =="serviceTask");

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidActivityId()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.ActivityId =="invalid");

//		//verifyQueryResults(query, 0);

//		string[] nullValue = null;

//		try
//		{
//		  query.ActivityIdIn(nullValue);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}

//		string[] activityIdsContainsNull = new string[] {"a", null, "b"};

//		try
//		{
//		  query.ActivityIdIn(activityIdsContainsNull);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}

//		string[] activityIdsContainsEmptyString = new string[] {"a", "", "b"};

//		try
//		{
//		  query.ActivityIdIn(activityIdsContainsEmptyString);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByExecutionId()
//	  public virtual void testQueryByExecutionId()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");
//		string executionId = managementService.CreateJobQuery().First().ExecutionId;

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().ExecutionIdIn(executionId);

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidExecutionId()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().ExecutionIdIn("invalid");

//		//verifyQueryResults(query, 0);

//		string[] nullValue = null;

//		try
//		{
//		  query.ExecutionIdIn(nullValue);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}

//		string[] executionIdsContainsNull = new string[] {"a", null, "b"};

//		try
//		{
//		  query.ExecutionIdIn(executionIdsContainsNull);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}

//		string[] executionIdsContainsEmptyString = new string[] {"a", "", "b"};

//		try
//		{
//		  query.ExecutionIdIn(executionIdsContainsEmptyString);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByProcessInstanceId()
//	  public virtual void testQueryByProcessInstanceId()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");
//		string ProcessInstanceId = managementService.CreateJobQuery().First().ProcessInstanceId;

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.ProcessInstanceId == ProcessInstanceId);

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidProcessInstanceId()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.ProcessInstanceId == "invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.Where(c=>c.ProcessInstanceId==null);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByProcessDefinitionId()
//	  public virtual void testQueryByProcessDefinitionId()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");
//		string processDefinitionId = managementService.CreateJobQuery().First().ProcessDefinitionId;

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.ProcessDefinitionId ==processDefinitionId);

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidProcessDefinitionId()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.ProcessDefinitionId =="invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.Where(c=>c.ProcessDefinitionId==null);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByProcessDefinitionKey()
//	  public virtual void testQueryByProcessDefinitionKey()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");
//		string processDefinitionKey = managementService.CreateJobQuery().First().ProcessDefinitionKey;

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.ProcessDefinitionKey==processDefinitionKey);

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidProcessDefinitionKey()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.ProcessDefinitionKey=="invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.Where(c=>c.ProcessDefinitionKey== null);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByDeploymentId()
//	  public virtual void testQueryByDeploymentId()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");
//		string deploymentId = managementService.CreateJobQuery().First().DeploymentId;

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=> c.DeploymentId == deploymentId);

//		//verifyQueryResults(query, 1);
//	  }

//	  public virtual void testQueryByInvalidDeploymentId()
//	  {
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=> c.DeploymentId == "invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.DeploymentId(null);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		}
//	  }

//[Test]
//[Deployment]
//	  public virtual void testQueryByJobPriority()
//	  {
//		// given 5 process instances with 5 jobs
//		IList<IProcessInstance> processInstances = new List<IProcessInstance>();

//		for (int i = 0; i < 5; i++)
//		{
//		  processInstances.Add(runtimeService.StartProcessInstanceByKey("process", Variable.Variables.CreateVariables().PutValue("priority", i)));
//		}

//		// then the creation logs can be filtered by priority of the jobs
//		// (1) lower than or equal a priority
//		IList<IHistoricJobLog> jobLogs = historyService.CreateHistoricJobLogQuery().JobPriorityLowerThanOrEquals(2L).OrderByJobPriority()/*.Asc()*/.ToList();

//		Assert.AreEqual(3, jobLogs.Count);
//		foreach (IHistoricJobLog log in jobLogs)
//		{
//		  Assert.True(log.JobPriority <= 2);
//		}

//		// (2) higher than or equal a given priorty
//		jobLogs = historyService.CreateHistoricJobLogQuery().JobPriorityHigherThanOrEquals(3L).OrderByJobPriority()/*.Asc()*/.ToList();

//		Assert.AreEqual(2, jobLogs.Count);
//		foreach (IHistoricJobLog log in jobLogs)
//		{
//		  Assert.True(log.JobPriority >= 3);
//		}

//		// (3) lower and higher than or equal
//		jobLogs = historyService.CreateHistoricJobLogQuery().JobPriorityHigherThanOrEquals(1L).JobPriorityLowerThanOrEquals(3L).OrderByJobPriority()/*.Asc()*/.ToList();

//		Assert.AreEqual(3, jobLogs.Count);
//		foreach (IHistoricJobLog log in jobLogs)
//		{
//		  Assert.True(log.JobPriority >= 1 && log.JobPriority <= 3);
//		}

//		// (4) lower and higher than or equal are disjunctive
//		jobLogs = historyService.CreateHistoricJobLogQuery().JobPriorityHigherThanOrEquals(3).JobPriorityLowerThanOrEquals(1).OrderByJobPriority()/*.Asc()*/.ToList();
//		Assert.AreEqual(0, jobLogs.Count);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByCreationLog()
//	  public virtual void testQueryByCreationLog()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().CreationLog();

//		//verifyQueryResults(query, 1);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByFailureLog()
//	  public virtual void testQueryByFailureLog()
//	  {
//		runtimeService.StartProcessInstanceByKey("process");
//		string jobId = managementService.CreateJobQuery().First().Id;
//		try
//		{
//		  managementService.ExecuteJob(jobId);
//		  Assert.Fail();
//		}
//		catch (Exception)
//		{
//		  // expected
//		}

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.State== ExternalTaskStateFields.Failed.StateCode.ToString());

//		//verifyQueryResults(query, 1);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryBySuccessLog()
//	  public virtual void testQueryBySuccessLog()
//	  {
//		runtimeService.StartProcessInstanceByKey("process", Variable.Variables.CreateVariables().PutValue("Assert.Fail", false));
//		string jobId = managementService.CreateJobQuery().First().Id;
//		managementService.ExecuteJob(jobId);

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().SuccessLog();

//		//verifyQueryResults(query, 1);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQueryByDeletionLog()
//	  public virtual void testQueryByDeletionLog()
//	  {
//		string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;
//		runtimeService.DeleteProcessInstance(ProcessInstanceId, null);

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery().DeletionLog();

//		//verifyQueryResults(query, 1);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQuerySorting()
//	  public virtual void testQuerySorting()
//	  {
//		for (int i = 0; i < 10; i++)
//		{
//		  runtimeService.StartProcessInstanceByKey("process");
//		}

//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

//		// asc
//		query/*.OrderByTimestamp()*//*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, historicJobLogByTimestamp());

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByJobId()/*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, historicJobLogByJobId());

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByJobDefinitionId()/*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, historicJobLogByJobDefinitionId());

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByJobDueDate()/*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, historicJobLogByJobDueDate());

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByJobRetries()/*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, historicJobLogByJobRetries());

//		query = historyService.CreateHistoricJobLogQuery();

//		query/*.OrderByActivityId()*//*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, historicJobLogByActivityId());

//		query = historyService.CreateHistoricJobLogQuery();

//		query//.OrderByExecutionId()/*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, historicJobLogByExecutionId());

//		query = historyService.CreateHistoricJobLogQuery();

//		query//.OrderByProcessInstanceId()/*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, historicJobLogByProcessInstanceId());

//		query = historyService.CreateHistoricJobLogQuery();

//		query/*.OrderByProcessDefinitionId()*//*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, historicJobLogByProcessDefinitionId());

//		query = historyService.CreateHistoricJobLogQuery();

//		query//.OrderByProcessDefinitionKey()/*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, historicJobLogByProcessDefinitionKey(processEngine));

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByDeploymentId()/*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, historicJobLogByDeploymentId());

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByJobPriority()/*.Asc()*/;

//		verifyQueryWithOrdering(query, 10, historicJobLogByJobPriority());

//		// Desc
//		query/*.OrderByTimestamp()*//*.Desc()*/;
//		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByTimestamp()));

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByJobId()/*.Desc()*/;
//		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByJobId()));

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByJobDefinitionId()/*.Asc()*/;
//		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByJobDefinitionId()));

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByJobDueDate()/*.Desc()*/;
//		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByJobDueDate()));

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByJobRetries()/*.Desc()*/;
//		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByJobRetries()));

//		query = historyService.CreateHistoricJobLogQuery();

//		query/*.OrderByActivityId()*//*.Desc()*/;
//		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByActivityId()));

//		query = historyService.CreateHistoricJobLogQuery();

//		query//.OrderByExecutionId()/*.Desc()*/;
//		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByExecutionId()));

//		query = historyService.CreateHistoricJobLogQuery();

//		query//.OrderByProcessInstanceId()/*.Desc()*/;
//		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByProcessInstanceId()));

//		query = historyService.CreateHistoricJobLogQuery();

//		query/*.OrderByProcessDefinitionId()*//*.Desc()*/;
//		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByProcessDefinitionId()));

//		query = historyService.CreateHistoricJobLogQuery();

//		query//.OrderByProcessDefinitionKey()/*.Desc()*/;
//		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByProcessDefinitionKey(processEngine)));

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByDeploymentId()/*.Desc()*/;
//		verifyQueryWithOrdering(query, 10, inverted(historicJobLogByDeploymentId()));

//		query = historyService.CreateHistoricJobLogQuery();

//		query.OrderByJobPriority()/*.Desc()*/;

//	  verifyQueryWithOrdering(query, 10, inverted(historicJobLogByJobPriority()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: Deployment(new string[] {"resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml"}) public void testQuerySortingPartiallyByOccurrence()
//	  public virtual void testQuerySortingPartiallyByOccurrence()
//	  {
//		string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;
//		string jobId = managementService.CreateJobQuery().First().Id;

//		ExecuteAvailableJobs();
//		runtimeService.SetVariable(ProcessInstanceId, "Assert.Fail", false);
//		managementService.ExecuteJob(jobId);

//		// asc
//		IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery(c=>c.Id ==jobId)//.OrderPartiallyByOccurrence()/*.Asc()*/;

//		verifyQueryWithOrdering(query, 5, historicJobLogPartiallyByOccurence());

//		// Desc
//		query = historyService.CreateHistoricJobLogQuery(c=>c.Id ==jobId)//.OrderPartiallyByOccurrence()/*.Desc()*/;

//		verifyQueryWithOrdering(query, 5, inverted(historicJobLogPartiallyByOccurence()));

//		runtimeService.DeleteProcessInstance(ProcessInstanceId, null);

//		// Delete job /////////////////////////////////////////////////////////

//		ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;
//		jobId = managementService.CreateJobQuery().First().Id;

//		ExecuteAvailableJobs();
//		managementService.DeleteJob(jobId);

//		// asc
//		query = historyService.CreateHistoricJobLogQuery(c=>c.Id ==jobId)//.OrderPartiallyByOccurrence()/*.Asc()*/;

//		verifyQueryWithOrdering(query, 5, historicJobLogPartiallyByOccurence());

//		// Desc
//		query = historyService.CreateHistoricJobLogQuery(c=>c.Id ==jobId)//.OrderPartiallyByOccurrence()/*.Desc()*/;

//		verifyQueryWithOrdering(query, 5, inverted(historicJobLogPartiallyByOccurence()));
//	  }

//	  protected internal virtual void verifyQueryResults(IQueryable<IHistoricJobLog> query, int countExpected)
//	  {
//		Assert.AreEqual(countExpected, query.Count());
//		Assert.AreEqual(countExpected, query.Count());

//		if (countExpected == 1)
//		{
//		  Assert.NotNull(query.First());
//		}
//		else if (countExpected > 1)
//		{
//		  verifySingleResultFails(query);
//		}
//		else if (countExpected == 0)
//		{
//		  Assert.IsNull(query.First());
//		}
//	  }

//	  protected internal virtual void verifyQueryWithOrdering(IQueryable<IHistoricJobLog> query, int countExpected, TestOrderingUtil.NullTolerantComparator<IHistoricJobLog> expectedOrdering)
//	  {
//		//verifyQueryResults(query, countExpected);
//		TestOrderingUtil.verifySorting(query.ToList();, expectedOrdering);
//	  }

//	  protected internal virtual void verifySingleResultFails(IQueryable<IHistoricJobLog> query)
//	  {
//		try
//		{
//		  query.First();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	}

//}