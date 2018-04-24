//using System;
//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Filter;
//using ESS.FW.Bpm.Engine.Identity;
//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Task;
//using NUnit.Framework;
//using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;


//namespace ESS.FW.Bpm.Engine.Tests.Api.Filter
//{

//    /// <summary>
//	/// 
//	/// </summary>
//	public class FilterTaskQueryTest : PluggableProcessEngineTestCase
//	{

//	  protected internal IFilter filter;

//	  protected internal string testString = "test";
//	  protected internal int? testInteger = 1;
//	  protected internal DelegationState testDelegationState = DelegationState.Pending;
//	  protected internal DateTime testDate = new DateTime();
//	  protected internal string[] testActivityInstances = new string[] {"a", "b", "c"};
//	  protected internal string[] testKeys = new string[] {"d", "e"};
//	  protected internal IList<string> testCandidateGroups = new List<string>();

//	  protected internal string[] variableNames = new string[] {"a", "b", "c", "d", "e", "f"};
//	  protected internal object[] variableValues = new object[] {1, 2, "3", "4", 5, 6};
//	  protected internal QueryOperator[] variableOperators = new QueryOperator[] {QueryOperator.EQUALS, QueryOperator.GreaterThanOrEqual, QueryOperator.LessThan, QueryOperator.Like, QueryOperator.NotEquals, QueryOperator.LessThanOrEqual};
//	  protected internal bool[] isTaskVariable = new bool[] {true, true, false, false, false, false};
//	  protected internal bool[] isProcessVariable = new bool[] {false, false, true, true, false, false};
//	  protected internal IUser testUser;
//	  protected internal IGroup testGroup;

//	  protected internal JsonTaskQueryConverter queryConverter;

//	  public void setUp()
//	  {
//		filter = filterService.NewTaskFilter("name").SetOwner("owner").SetQuery(taskService.CreateTaskQuery()).SetProperties(new Dictionary<string, object>());
//		testUser = identityService.NewUser("user");
//		testGroup = identityService.NewGroup("group");
//		identityService.SaveUser(testUser);
//		identityService.SaveGroup(testGroup);
//		identityService.CreateMembership(testUser.Id, testGroup.Id);

//		IGroup anotherGroup = identityService.NewGroup("anotherGroup");
//		identityService.SaveGroup(anotherGroup);
//		testCandidateGroups.Add(testGroup.Id);
//		testCandidateGroups.Add(anotherGroup.Id);

//		createTasks();

//		queryConverter = new JsonTaskQueryConverter();
//	  }

//	  public void tearDown()
//	  {
//		foreach (IFilter filter in filterService.CreateTaskFilterQuery().ToList())
//		{
//		  filterService.DeleteFilter(filter.Id);
//		}
//		foreach (IGroup group in identityService.CreateGroupQuery().ToList())
//		{
//		  identityService.DeleteGroup(group.Id);
//		}
//		foreach (IUser user in identityService.CreateUserQuery().ToList())
//		{
//		  identityService.DeleteUser(user.Id);
//		}
//		foreach (ITask task in taskService.CreateTaskQuery().ToList())
//		{
//		  taskService.DeleteTask(task.Id, true);
//		}
//	  }

//	  public virtual void testEmptyQuery()
//	  {
//		IQueryable<ITask> emptyQuery = taskService.CreateTaskQuery();
//		string emptyQueryJson = "{}";

//		////filter.Query = emptyQuery;

//		Assert.AreEqual(emptyQueryJson, ((FilterEntity) filter).QueryInternal);
//		//Assert.NotNull(filter.Query);
//	  }

//	  public virtual void testTaskQuery()
//	  {
//		// create query
//		TaskQueryImpl query = new TaskQueryImpl();
//		query.Where(c=>c.TaskId==testString);
//		query.TaskName(testString);
//		query.TaskNameNotEqual(testString);
//		query.TaskNameLike(testString);
//		query.TaskNameNotLike(testString);
//		query.TaskDescription(testString);
//		query.TaskDescriptionLike(testString);
//		query.TaskPriority(testInteger);
//		query.TaskMinPriority(testInteger);
//		query.TaskMaxPriority(testInteger);
//		query.TaskAssignee(testString);
//		query.TaskAssigneeExpression(testString);
//		query.TaskAssigneeLike(testString);
//		query.TaskAssigneeLikeExpression(testString);
//		query.TaskInvolvedUser(testString);
//		query.TaskInvolvedUserExpression(testString);
//		query.TaskOwner(testString);
//		query.TaskOwnerExpression(testString);
//		query.TaskUnassigned();
//		query.TaskAssigned();
//		query.TaskDelegationState(testDelegationState);
//		query.TaskCandidateGroupIn(testCandidateGroups);
//		query.TaskCandidateGroupInExpression(testString);
//		query.WithCandidateGroups();
//		query.WithoutCandidateGroups();
//		query.WithCandidateUsers();
//		query.WithoutCandidateUsers();
//		query.Where(c=>c.ProcessInstanceId==testString);
//		query.ExecutionId(testString);
//		query.ActivityInstanceIdIn(testActivityInstances);
//		query.TaskCreatedOn(testDate);
//		query.TaskCreatedOnExpression(testString);
//		query.TaskCreatedBefore(testDate);
//		query.TaskCreatedBeforeExpression(testString);
//		query.TaskCreatedAfter(testDate);
//		query.TaskCreatedAfterExpression(testString);
//		query.Where(c=>c.TaskDefinitionKey==testString);
//		query.TaskDefinitionKeyIn(testKeys);
//		query.TaskDefinitionKeyLike(testString);
//		query.Where(c=>c.ProcessDefinitionKey== testString);
//		query.ProcessDefinitionKeyIn(testKeys);
//		query.Where(c=>c.ProcessDefinitionId==testString);
//		query.ProcessDefinitionName(testString);
//		query.ProcessDefinitionNameLike(testString);
//		query.ProcessInstanceBusinessKey(testString);
//		query.ProcessInstanceBusinessKeyIn(testKeys);
//		query.ProcessInstanceBusinessKeyLike(testString);

//		// variables
//		query////.TaskVariableValueEquals(variableNames[0], variableValues[0]);
//		query.TaskVariableValueGreaterThanOrEquals(variableNames[1], variableValues[1]);
//		query.ProcessVariableValueLessThan(variableNames[2], variableValues[2]);
//		query.ProcessVariableValueLike(variableNames[3], (string) variableValues[3]);
//		query.CaseInstanceVariableValueNotEquals(variableNames[4], variableValues[4]);
//		query.CaseInstanceVariableValueLessThanOrEquals(variableNames[5], variableValues[5]);

//		query.DueDate(testDate);
//		query.DueDateExpression(testString);
//		query.DueBefore(testDate);
//		query.DueBeforeExpression(testString);
//		query.DueAfter(testDate);
//		query.DueAfterExpression(testString);
//		query.FollowUpDate(testDate);
//		query.FollowUpDateExpression(testString);
//		query.FollowUpBefore(testDate);
//		query.FollowUpBeforeExpression(testString);
//		query.FollowUpAfter(testDate);
//		query.FollowUpAfterExpression(testString);
//		query/*.ExcludeSubtasks()*/;
//		query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode);
//		query.CaseDefinitionKey(testString);
//		query.CaseDefinitionId(testString);
//		query.CaseDefinitionName(testString);
//		query.CaseDefinitionNameLike(testString);
//		query.CaseInstanceId(testString);
//		query.CaseInstanceBusinessKey(testString);
//		query.CaseInstanceBusinessKeyLike(testString);
//		query.CaseExecutionId(testString);

//		// ordering
//		query//.OrderByExecutionId()/*.Desc()*/;
//		query.OrderByDueDate()/*.Asc()*/;
//		query.OrderByProcessVariable("var", ValueType.STRING)/*.Desc()*/;

//		IList<QueryOrderingProperty> expectedOrderingProperties = query.OrderingProperties;

//		// save filter
//		filter.Query = query;
//		filterService.SaveFilter(filter);

//		// fetch from db
//		filter = filterService.CreateTaskFilterQuery().First();

//		// test query
//		query = filter.Query;
//		Assert.AreEqual(testString, query.TaskId);
//		Assert.AreEqual(testString, query.Name);
//		Assert.AreEqual(testString, query.NameNotEqual);
//		Assert.AreEqual(testString, query.NameNotLike);
//		Assert.AreEqual(testString, query.NameLike);
//		Assert.AreEqual(testString, query.Description);
//		Assert.AreEqual(testString, query.DescriptionLike);
//		Assert.AreEqual(testInteger, query.Priority);
//		Assert.AreEqual(testInteger, query.MinPriority);
//		Assert.AreEqual(testInteger, query.MaxPriority);
//		Assert.AreEqual(testString, query.Assignee);
//		Assert.AreEqual(testString, query.Expressions.Get("taskAssignee"));
//		Assert.AreEqual(testString, query.AssigneeLike);
//		Assert.AreEqual(testString, query.Expressions.Get("taskAssigneeLike"));
//		Assert.AreEqual(testString, query.InvolvedUser);
//		Assert.AreEqual(testString, query.Expressions.Get("taskInvolvedUser"));
//		Assert.AreEqual(testString, query.Owner);
//		Assert.AreEqual(testString, query.Expressions.Get("taskOwner"));
//		Assert.True(query.Unassigned);
//		Assert.True(query.Assigned);
//		Assert.AreEqual(testDelegationState, query.DelegationState);
//		Assert.AreEqual(testCandidateGroups, query.CandidateGroups);
//		Assert.True(query.WithCandidateGroups);
//		Assert.True(query.WithoutCandidateGroups);
//		Assert.True(query.WithCandidateUsers);
//		Assert.True(query.WithoutCandidateUsers);
//		Assert.AreEqual(testString, query.Expressions.Get("taskCandidateGroupIn"));
//		Assert.AreEqual(testString, query.ProcessInstanceId);
//		Assert.AreEqual(testString, query.ExecutionId);
//		Assert.AreEqual(testActivityInstances.Length, query.ActivityInstanceIdIn.Length);
//		for (int i = 0; i < query.ActivityInstanceIdIn.Length; i++)
//		{
//		  Assert.AreEqual(testActivityInstances[i], query.ActivityInstanceIdIn[i]);
//		}
//		Assert.AreEqual(testDate, query.CreateTime);
//		Assert.AreEqual(testString, query.Expressions.Get("taskCreatedOn"));
//		Assert.AreEqual(testDate, query.CreateTimeBefore);
//		Assert.AreEqual(testString, query.Expressions.Get("taskCreatedBefore"));
//		Assert.AreEqual(testDate, query.CreateTimeAfter);
//		Assert.AreEqual(testString, query.Expressions.Get("taskCreatedAfter"));
//		Assert.AreEqual(testString, query.Key);
//		Assert.AreEqual(testKeys.Length, query.Keys.Length);
//		for (int i = 0; i < query.Keys.Length; i++)
//		{
//		  Assert.AreEqual(testKeys[i], query.Keys[i]);
//		}
//		Assert.AreEqual(testString, query.KeyLike);
//		Assert.AreEqual(testString, query.ProcessDefinitionKey);
//		for (int i = 0; i < query.ProcessDefinitionKeys.Length; i++)
//		{
//		  Assert.AreEqual(testKeys[i], query.ProcessDefinitionKeys[i]);
//		}
//		Assert.AreEqual(testString, query.ProcessDefinitionId);
//		Assert.AreEqual(testString, query.ProcessDefinitionName);
//		Assert.AreEqual(testString, query.ProcessDefinitionNameLike);
//		Assert.AreEqual(testString, query.ProcessInstanceBusinessKey);
//		for (int i = 0; i < query.ProcessInstanceBusinessKeys.Length; i++)
//		{
//		  Assert.AreEqual(testKeys[i], query.ProcessInstanceBusinessKeys[i]);
//		}
//		Assert.AreEqual(testString, query.ProcessInstanceBusinessKeyLike);

//		// variables
//		IList<TaskQueryVariableValue> variables = query.Variables;
//		for (int i = 0; i < variables.Count; i++)
//		{
//		  TaskQueryVariableValue variable = variables[i];
//		  Assert.AreEqual(variableNames[i], variable.Name);
//		  Assert.AreEqual(variableValues[i], variable.Value);
//		  Assert.AreEqual(variableOperators[i], variable.Operator);
//		  Assert.AreEqual(isTaskVariable[i], variable.Local);
//		  Assert.AreEqual(isProcessVariable[i], variable.ProcessInstanceVariable);
//		}

//		Assert.AreEqual(testDate, query.DueDate);
//		Assert.AreEqual(testString, query.Expressions.Get("dueDate"));
//		Assert.AreEqual(testDate, query.DueBefore);
//		Assert.AreEqual(testString, query.Expressions.Get("dueBefore"));
//		Assert.AreEqual(testDate, query.DueAfter);
//		Assert.AreEqual(testString, query.Expressions.Get("dueAfter"));
//		Assert.AreEqual(testDate, query.FollowUpDate);
//		Assert.AreEqual(testString, query.Expressions.Get("followUpDate"));
//		Assert.AreEqual(testDate, query.FollowUpBefore);
//		Assert.AreEqual(testString, query.Expressions.Get("followUpBefore"));
//		Assert.AreEqual(testDate, query.FollowUpAfter);
//		Assert.AreEqual(testString, query.Expressions.Get("followUpAfter"));
//		Assert.True(query.ExcludeSubtasks);
//		Assert.AreEqual(SuspensionState.SUSPENDED, query.SuspensionState);
//		Assert.AreEqual(testString, query.CaseDefinitionKey);
//		Assert.AreEqual(testString, query.CaseDefinitionId);
//		Assert.AreEqual(testString, query.CaseDefinitionName);
//		Assert.AreEqual(testString, query.CaseDefinitionNameLike);
//		Assert.AreEqual(testString, query.CaseInstanceId);
//		Assert.AreEqual(testString, query.CaseInstanceBusinessKey);
//		Assert.AreEqual(testString, query.CaseInstanceBusinessKeyLike);
//		Assert.AreEqual(testString, query.CaseExecutionId);

//		// ordering
//		verifyOrderingProperties(expectedOrderingProperties, query.OrderingProperties);
//	  }

//	  protected internal virtual void verifyOrderingProperties(IList<QueryOrderingProperty> expectedProperties, IList<QueryOrderingProperty> actualProperties)
//	  {
//		Assert.AreEqual(expectedProperties.Count, actualProperties.Count);

//		for (int i = 0; i < expectedProperties.Count; i++)
//		{
//		  QueryOrderingProperty expectedProperty = expectedProperties[i];
//		  QueryOrderingProperty actualProperty = actualProperties[i];

//		  Assert.AreEqual(expectedProperty.Relation, actualProperty.Relation);
//		  Assert.AreEqual(expectedProperty.Direction, actualProperty.Direction);
//		  Assert.AreEqual(expectedProperty.ContainedProperty, actualProperty.ContainedProperty);
//		  Assert.AreEqual(expectedProperty.QueryProperty, actualProperty.QueryProperty);

//		  IList<QueryEntityRelationCondition> expectedRelationConditions = expectedProperty.RelationConditions;
//		  IList<QueryEntityRelationCondition> actualRelationConditions = expectedProperty.RelationConditions;

//		  if (expectedRelationConditions != null && actualRelationConditions != null)
//		  {
//			Assert.AreEqual(expectedRelationConditions.Count, actualRelationConditions.Count);

//			for (int j = 0; j < expectedRelationConditions.Count; j++)
//			{
//			  QueryEntityRelationCondition expectedFilteringProperty = expectedRelationConditions[j];
//			  QueryEntityRelationCondition actualFilteringProperty = expectedRelationConditions[j];

//			  Assert.AreEqual(expectedFilteringProperty.Property, actualFilteringProperty.Property);
//			  Assert.AreEqual(expectedFilteringProperty.ComparisonProperty, actualFilteringProperty.ComparisonProperty);
//			  Assert.AreEqual(expectedFilteringProperty.ScalarValue, actualFilteringProperty.ScalarValue);
//			}
//		  }
//		  else if ((expectedRelationConditions == null && actualRelationConditions != null) || (expectedRelationConditions != null && actualRelationConditions == null))
//		  {
//			Assert.Fail("Expected filtering properties: " + expectedRelationConditions + ". " + "Actual filtering properties: " + actualRelationConditions);
//		  }
//		}
//	  }

//	  public virtual void testTaskQueryByFollowUpBeforeOrNotExistent()
//	  {
//		// create query
//		TaskQueryImpl query = new TaskQueryImpl();

//		query.FollowUpBeforeOrNotExistent(testDate);

//		// save filter
//		filter.Query = query;
//		filterService.SaveFilter(filter);

//		// fetch from db
//		filter = filterService.CreateTaskFilterQuery().First();

//		// test query
//		query = filter.Query;
//		Assert.True(query.FollowUpNullAccepted);
//		Assert.AreEqual(testDate, query.FollowUpBefore);
//	  }

//	  public virtual void testTaskQueryByFollowUpBeforeOrNotExistentExtendingQuery()
//	  {
//		// create query
//		TaskQueryImpl query = new TaskQueryImpl();

//		query.FollowUpBeforeOrNotExistent(testDate);

//		// save filter without query
//		filterService.SaveFilter(filter);

//		// fetch from db
//		filter = filterService.CreateTaskFilterQuery().First();

//		// use query as extending query
//		IList<ITask> tasks = filterService.List(filter.Id, query);
//		Assert.AreEqual(3, tasks.Count);

//		// set as filter query and save filter
//		filter.Query = query;
//		filterService.SaveFilter(filter);

//		// fetch from db
//		filter = filterService.CreateTaskFilterQuery().First();

//		tasks = filterService.List(filter.Id);
//		Assert.AreEqual(3, tasks.Count);

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery();

//		extendingQuery/*.OrderByTaskCreateTime()*//*.Asc()*/;

//		tasks = filterService.List(filter.Id, extendingQuery);
//		Assert.AreEqual(3, tasks.Count);
//	  }

//	  public virtual void testTaskQueryByFollowUpBeforeOrNotExistentExpression()
//	  {
//		// create query
//		TaskQueryImpl query = new TaskQueryImpl();

//		query.FollowUpBeforeOrNotExistentExpression(testString);

//		// save filter
//		filter.Query = query;
//		filterService.SaveFilter(filter);

//		// fetch from db
//		filter = filterService.CreateTaskFilterQuery().First();

//		// test query
//		query = filter.Query;
//		Assert.True(query.FollowUpNullAccepted);
//		Assert.AreEqual(testString, query.Expressions.Get("followUpBeforeOrNotExistent"));
//	  }

//	  public virtual void testTaskQueryByFollowUpBeforeOrNotExistentExpressionExtendingQuery()
//	  {
//		// create query
//		TaskQueryImpl query = new TaskQueryImpl();

//		query.FollowUpBeforeOrNotExistentExpression("${dateTime().WithMillis(0)}");

//		// save filter without query
//		filterService.SaveFilter(filter);

//		// fetch from db
//		filter = filterService.CreateTaskFilterQuery().First();

//		// use query as extending query
//		IList<ITask> tasks = filterService.List(filter.Id, query);
//		Assert.AreEqual(3, tasks.Count);

//		// set as filter query and save filter
//		filter.Query = query;
//		filterService.SaveFilter(filter);

//		// fetch from db
//		filter = filterService.CreateTaskFilterQuery().First();

//		tasks = filterService.List(filter.Id);
//		Assert.AreEqual(3, tasks.Count);

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery();

//		extendingQuery/*.OrderByTaskCreateTime()*//*.Asc()*/;

//		tasks = filterService.List(filter.Id, extendingQuery);
//		Assert.AreEqual(3, tasks.Count);
//	  }

//	  public virtual void testTaskQueryCandidateUser()
//	  {
//		TaskQueryImpl query = new TaskQueryImpl();
//		query.TaskCandidateUser(testUser.Id);
//		query.TaskCandidateUserExpression(testUser.Id);

//		filter.Query = query;
//		query = filter.Query;

//		Assert.AreEqual(testUser.Id, query.CandidateUser);
//		Assert.AreEqual(testUser.Id, query.Expressions.Get("taskCandidateUser"));
//	  }

//	  public virtual void testTaskQueryCandidateGroup()
//	  {
//		TaskQueryImpl query = new TaskQueryImpl();
//		query.TaskCandidateGroup(testGroup.Id);
//		query.TaskCandidateGroupExpression(testGroup.Id);

//		filter.Query = query;
//		query = filter.Query;

//		Assert.AreEqual(testGroup.Id, query.CandidateGroup);
//		Assert.AreEqual(testGroup.Id, query.Expressions.Get("taskCandidateGroup"));
//	  }

//	  public virtual void testTaskQueryCandidateUserIncludeAssignedTasks()
//	  {
//		TaskQueryImpl query = new TaskQueryImpl();
//		query.TaskCandidateUser(testUser.Id);
//		query.IncludeAssignedTasks();

//		saveQuery(query);
//		query = filterService.GetFilter(filter.Id).Query;

//		Assert.AreEqual(testUser.Id, query.CandidateUser);
//		Assert.True(query.IncludeAssignedTasks);
//	  }

//	  public virtual void testTaskQueryCandidateUserExpressionIncludeAssignedTasks()
//	  {
//		TaskQueryImpl query = new TaskQueryImpl();
//		query.TaskCandidateUserExpression(testString);
//		query.IncludeAssignedTasks();

//		saveQuery(query);
//		query = filterService.GetFilter(filter.Id).Query;

//		Assert.AreEqual(testString, query.Expressions.Get("taskCandidateUser"));
//		Assert.True(query.IncludeAssignedTasks);
//	  }

//	  public virtual void testTaskQueryCandidateGroupIncludeAssignedTasks()
//	  {
//		TaskQueryImpl query = new TaskQueryImpl();
//		query.TaskCandidateGroup(testGroup.Id);
//		query.IncludeAssignedTasks();

//		saveQuery(query);
//		query = filterService.GetFilter(filter.Id).Query;

//		Assert.AreEqual(testGroup.Id, query.CandidateGroup);
//		Assert.True(query.IncludeAssignedTasks);
//	  }

//	  public virtual void testTaskQueryCandidateGroupExpressionIncludeAssignedTasks()
//	  {
//		TaskQueryImpl query = new TaskQueryImpl();
//		query.TaskCandidateGroupExpression(testString);
//		query.IncludeAssignedTasks();

//		saveQuery(query);
//		query = filterService.GetFilter(filter.Id).Query;

//		Assert.AreEqual(testString, query.Expressions.Get("taskCandidateGroup"));
//		Assert.True(query.IncludeAssignedTasks);
//	  }

//	  public virtual void testTaskQueryCandidateGroupsIncludeAssignedTasks()
//	  {
//		TaskQueryImpl query = new TaskQueryImpl();
//		query.TaskCandidateGroupIn(testCandidateGroups);
//		query.IncludeAssignedTasks();

//		saveQuery(query);
//		query = filterService.GetFilter(filter.Id).Query;

//		Assert.AreEqual(testCandidateGroups, query.CandidateGroupsInternal);
//		Assert.True(query.IncludeAssignedTasks);
//	  }

//	  public virtual void testTaskQueryCandidateGroupsExpressionIncludeAssignedTasks()
//	  {
//		TaskQueryImpl query = new TaskQueryImpl();
//		query.TaskCandidateGroupInExpression(testString);
//		query.IncludeAssignedTasks();

//		saveQuery(query);
//		query = filterService.GetFilter(filter.Id).Query;

//		Assert.AreEqual(testString, query.Expressions.Get("taskCandidateGroupIn"));
//		Assert.True(query.IncludeAssignedTasks);
//	  }

//	  public virtual void testExecuteTaskQueryList()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		query.TaskNameLike("Task%");

//		saveQuery(query);

//		IList<ITask> tasks = filterService.List(filter.Id);
//		Assert.AreEqual(3, tasks.Count);
//		foreach (ITask task in tasks)
//		{
//		  Assert.AreEqual(testUser.Id, task.Owner);
//		}
//	  }

//	  public virtual void testExtendingTaskQueryList()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();

//		saveQuery(query);

//		IList<ITask> tasks = filterService.List(filter.Id);
//		Assert.AreEqual(3, tasks.Count);

//		tasks = filterService.List(filter.Id, query);
//		Assert.AreEqual(3, tasks.Count);

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery();

//		extendingQuery.TaskDelegationState(DelegationState.RESOLVED);

//		tasks = filterService.List(filter.Id, extendingQuery);
//		Assert.AreEqual(2, tasks.Count);

//		foreach (ITask task in tasks)
//		{
//		  Assert.AreEqual(DelegationState.RESOLVED, task.DelegationState);
//		}
//	  }

//	  public virtual void testExtendingTaskQueryListWithCandidateGroups()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();

//		IList<string> candidateGroups = new List<string>();
//		candidateGroups.Add("accounting");
//		query.TaskCandidateGroupIn(candidateGroups);

//		saveQuery(query);

//		IList<ITask> tasks = filterService.List(filter.Id);
//		Assert.AreEqual(1, tasks.Count);

//		tasks = filterService.List(filter.Id, query);
//		Assert.AreEqual(1, tasks.Count);

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery();

//		extendingQuery/*.OrderByTaskCreateTime()*//*.Asc()*/;

//		tasks = filterService.List(filter.Id, extendingQuery);
//		Assert.AreEqual(1, tasks.Count);
//	  }

//	  public virtual void testExtendingTaskQueryListWithIncludeAssignedTasks()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();

//		query.TaskCandidateGroup("accounting");

//		saveQuery(query);

//		IList<ITask> tasks = filterService.List(filter.Id);
//		Assert.AreEqual(1, tasks.Count);

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery();

//		extendingQuery.TaskCandidateGroup("accounting").IncludeAssignedTasks();

//		tasks = filterService.List(filter.Id, extendingQuery);
//		Assert.AreEqual(2, tasks.Count);
//	  }

//	  public virtual void testExtendTaskQueryWithCandidateUserExpressionAndIncludeAssignedTasks()
//	  {
//		// create an empty query and save it as a filter
//		IQueryable<ITask> emptyQuery = taskService.CreateTaskQuery();
//		IFilter emptyFilter = filterService.NewTaskFilter("empty");
//		emptyFilter.Query = emptyQuery;

//		// create a query with candidate user expression and include assigned tasks
//		// and save it as filter
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		query.TaskCandidateUserExpression("${'test'}").IncludeAssignedTasks();
//		IFilter filter = filterService.NewTaskFilter("filter");
//		filter.Query = query;

//		// extend empty query by query with candidate user expression and include assigned tasks
//		IFilter extendedFilter = emptyFilter.Extend(query);
//		TaskQueryImpl extendedQuery = extendedFilter.Query;
//		Assert.AreEqual("${'test'}", extendedQuery.Expressions.Get("taskCandidateUser"));
//		Assert.True(extendedQuery.IncludeAssignedTasks);

//		// extend query with candidate user expression and include assigned tasks with empty query
//		extendedFilter = filter.Extend(emptyQuery);
//		extendedQuery = extendedFilter.Query;
//		Assert.AreEqual("${'test'}", extendedQuery.Expressions.Get("taskCandidateUser"));
//		Assert.True(extendedQuery.IncludeAssignedTasks);
//	  }

//	  public virtual void testExtendTaskQueryWithCandidateGroupExpressionAndIncludeAssignedTasks()
//	  {
//		// create an empty query and save it as a filter
//		IQueryable<ITask> emptyQuery = taskService.CreateTaskQuery();
//		IFilter emptyFilter = filterService.NewTaskFilter("empty");
//		emptyFilter.Query = emptyQuery;

//		// create a query with candidate group expression and include assigned tasks
//		// and save it as filter
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		query.TaskCandidateGroupExpression("${'test'}").IncludeAssignedTasks();
//		IFilter filter = filterService.NewTaskFilter("filter");
//		filter.Query = query;

//		// extend empty query by query with candidate group expression and include assigned tasks
//		IFilter extendedFilter = emptyFilter.Extend(query);
//		TaskQueryImpl extendedQuery = extendedFilter.Query;
//		Assert.AreEqual("${'test'}", extendedQuery.Expressions.Get("taskCandidateGroup"));
//		Assert.True(extendedQuery.IncludeAssignedTasks);

//		// extend query with candidate group expression and include assigned tasks with empty query
//		extendedFilter = filter.Extend(emptyQuery);
//		extendedQuery = extendedFilter.Query;
//		Assert.AreEqual("${'test'}", extendedQuery.Expressions.Get("taskCandidateGroup"));
//		Assert.True(extendedQuery.IncludeAssignedTasks);
//	  }

//	  public virtual void testExtendTaskQueryWithCandidateGroupInExpressionAndIncludeAssignedTasks()
//	  {
//		// create an empty query and save it as a filter
//		IQueryable<ITask> emptyQuery = taskService.CreateTaskQuery();
//		IFilter emptyFilter = filterService.NewTaskFilter("empty");
//		emptyFilter.Query = emptyQuery;

//		// create a query with candidate group in expression and include assigned tasks
//		// and save it as filter
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		query.TaskCandidateGroupInExpression("${'test'}").IncludeAssignedTasks();
//		IFilter filter = filterService.NewTaskFilter("filter");
//		filter.Query = query;

//		// extend empty query by query with candidate group in expression and include assigned tasks
//		IFilter extendedFilter = emptyFilter.Extend(query);
//		TaskQueryImpl extendedQuery = extendedFilter.Query;
//		Assert.AreEqual("${'test'}", extendedQuery.Expressions.Get("taskCandidateGroupIn"));
//		Assert.True(extendedQuery.IncludeAssignedTasks);

//		// extend query with candidate group in expression and include assigned tasks with empty query
//		extendedFilter = filter.Extend(emptyQuery);
//		extendedQuery = extendedFilter.Query;
//		Assert.AreEqual("${'test'}", extendedQuery.Expressions.Get("taskCandidateGroupIn"));
//		Assert.True(extendedQuery.IncludeAssignedTasks);
//	  }

//	  public virtual void testExecuteTaskQueryListPage()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		query.TaskNameLike("Task%");

//		saveQuery(query);

//		IList<ITask> tasks = filterService.ListPage(filter.Id, 1, 2);
//		Assert.AreEqual(2, tasks.Count);
//		foreach (ITask task in tasks)
//		{
//		  Assert.AreEqual(testUser.Id, task.Owner);
//		}
//	  }

//	  public virtual void testExtendingTaskQueryListPage()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();

//		saveQuery(query);

//		IList<ITask> tasks = filterService.ListPage(filter.Id, 1, 2);
//		Assert.AreEqual(2, tasks.Count);

//		tasks = filterService.ListPage(filter.Id, query, 1, 2);
//		Assert.AreEqual(2, tasks.Count);

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery();

//		extendingQuery.TaskDelegationState(DelegationState.RESOLVED);

//		tasks = filterService.ListPage(filter.Id, extendingQuery, 1, 2);
//		Assert.AreEqual(1, tasks.Count);

//		Assert.AreEqual(DelegationState.RESOLVED, tasks[0].DelegationState);
//	  }

//	  public virtual void testExecuteTaskQuerySingleResult()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		query.TaskDelegationState(DelegationState.PENDING);

//		saveQuery(query);

//		ITask task = filterService.singleResult(filter.Id);
//		Assert.NotNull(task);
//		Assert.AreEqual("ITask 1", task.Name);
//	  }

//	  public virtual void testFailTaskQuerySingleResult()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();

//		saveQuery(query);

//		try
//		{
//		  filterService.singleResult(filter.Id);
//		  Assert.Fail("Exception expected");
//		}
//		catch (ProcessEngineException)
//		{
//		  // expected
//		}
//	  }

//	  public virtual void testExtendingTaskQuerySingleResult()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		query.TaskDelegationState(DelegationState.PENDING);

//		saveQuery(query);

//		ITask task = filterService.singleResult(filter.Id);
//		Assert.NotNull(task);
//		Assert.AreEqual("ITask 1", task.Name);
//		Assert.AreEqual("task1", task.Id);

//		task = filterService.singleResult(filter.Id, query);
//		Assert.NotNull(task);
//		Assert.AreEqual("ITask 1", task.Name);
//		Assert.AreEqual("task1", task.Id);

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery();

//		extendingQuery.TaskId("task1");

//		task = filterService.singleResult(filter.Id, extendingQuery);
//		Assert.NotNull(task);
//		Assert.AreEqual("ITask 1", task.Name);
//		Assert.AreEqual("task1", task.Id);
//	  }

//	  /// <summary>
//	  /// CAM-6363
//	  /// 
//	  /// Verify that search by name returns case insensitive results
//	  /// </summary>
//	  public virtual void testTaskQueryLookupByNameCaseInsensitive()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		query.TaskName("task 1");
//		saveQuery(query);

//		IList<ITask> tasks = filterService.List(filter.Id);
//		Assert.NotNull(tasks);
//		Assert.That(tasks.Count,Is.EqualTo(1));

//		query = taskService.CreateTaskQuery();
//		query.TaskName("tASk 2");
//		saveQuery(query);

//		tasks = filterService.List(filter.Id);
//		Assert.NotNull(tasks);
//		Assert.That(tasks.Count,Is.EqualTo(1));
//	  }

//	  /// <summary>
//	  /// CAM-6165
//	  /// 
//	  /// Verify that search by name like returns case insensitive results
//	  /// </summary>
//	  public virtual void testTaskQueryLookupByNameLikeCaseInsensitive()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();
//		query.TaskNameLike("%task%");
//		saveQuery(query);

//		IList<ITask> tasks = filterService.List(filter.Id);
//		Assert.NotNull(tasks);
//		Assert.That(tasks.Count,Is.EqualTo(3));

//		query = taskService.CreateTaskQuery();
//		query.TaskNameLike("%Task%");
//		saveQuery(query);

//		tasks = filterService.List(filter.Id);
//		Assert.NotNull(tasks);
//		Assert.That(tasks.Count,Is.EqualTo(3));
//	  }

//	  public virtual void testExecuteTaskQueryCount()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();

//		saveQuery(query);

//		long Count = filterService.Count(filter.Id);
//		Assert.AreEqual(3, Count);

//		query.TaskDelegationState(DelegationState.RESOLVED);

//		saveQuery(query);

//		Count = filterService.Count(filter.Id);
//		Assert.AreEqual(2, Count);
//	  }

//	  public virtual void testExtendingTaskQueryCount()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();

//		saveQuery(query);

//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery();

//		extendingQuery.TaskId("task3");

//		long Count = filterService.Count(filter.Id);

//		Assert.AreEqual(3, Count);

//		Count = filterService.Count(filter.Id, query);

//		Assert.AreEqual(3, Count);

//		Count = filterService.Count(filter.Id, extendingQuery);

//		Assert.AreEqual(1, Count);
//	  }

//	  public virtual void testSpecialExtendingQuery()
//	  {
//		IQueryable<ITask> query = taskService.CreateTaskQuery();

//		saveQuery(query);

//		long Count = filterService.Count(filter.Id, (Query) null);
//		Assert.AreEqual(3, Count);
//	  }

//	  public virtual void testExtendingSorting()
//	  {
//		// create empty query
//		TaskQueryImpl query = (TaskQueryImpl) taskService.CreateTaskQuery();
//		saveQuery(query);

//		// Assert default sorting
//		query = filter.Query;
//		Assert.True(query.OrderingProperties.Empty);

//		// extend query by new task query with sorting
//		TaskQueryImpl sortQuery = (TaskQueryImpl) taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/;
//		IFilter extendedFilter = filter.Extend(sortQuery);
//		query = extendedFilter.Query;

//		IList<QueryOrderingProperty> expectedOrderingProperties = new List<QueryOrderingProperty>(sortQuery.OrderingProperties);

//		verifyOrderingProperties(expectedOrderingProperties, query.OrderingProperties);

//		// extend query by new task query with additional sorting
//		TaskQueryImpl extendingQuery = (TaskQueryImpl) taskService.CreateTaskQuery()/*.OrderByTaskAssignee()*//*.Desc()*/;
//		extendedFilter = extendedFilter.Extend(extendingQuery);
//		query = extendedFilter.Query;

//		((List<QueryOrderingProperty>)expectedOrderingProperties).AddRange(extendingQuery.OrderingProperties);

//		verifyOrderingProperties(expectedOrderingProperties, query.OrderingProperties);

//		// extend query by incomplete sorting query (should add sorting anyway)
//		sortQuery = (TaskQueryImpl) taskService.CreateTaskQuery()//.OrderByCaseExecutionId();
//		extendedFilter = extendedFilter.Extend(sortQuery);
//		query = extendedFilter.Query;

//		((List<QueryOrderingProperty>)expectedOrderingProperties).AddRange(sortQuery.OrderingProperties);

//		verifyOrderingProperties(expectedOrderingProperties, query.OrderingProperties);
//	  }


//	  /// <summary>
//	  /// Tests compatibility with serialization format that was used in 7.2
//	  /// </summary>
////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("deprecation") public void testDeprecatedOrderingFormatDeserializationSingleOrdering()
//	  public virtual void testDeprecatedOrderingFormatDeserializationSingleOrdering()
//	  {
//		string sortByNameAsc = "RES." + TaskQueryProperty.NAME.Name + " " + Direction.ASCENDING.Name;

//		JsonTaskQueryConverter converter = (JsonTaskQueryConverter) FilterEntity.queryConverter.Get(EntityTypes.Resources.Task);
//		JSONObject queryJson = converter.ToJsonObject(filter.GetQuery<IQueryable<ITask>>());

//		// when I apply a specific ordering by one dimension
//		queryJson.put(JsonTaskQueryConverter.ORDER_BY, sortByNameAsc);
//		TaskQueryImpl deserializedTaskQuery = (TaskQueryImpl) converter.ToObject(queryJson);

//		// then the ordering is applied accordingly
//		Assert.AreEqual(1, deserializedTaskQuery.OrderingProperties.Count());

//		QueryOrderingProperty orderingProperty = deserializedTaskQuery.OrderingProperties.Get(0);
//		Assert.IsNull(orderingProperty.Relation);
//		Assert.AreEqual("asc", orderingProperty.Direction.Name);
//		Assert.IsNull(orderingProperty.RelationConditions);
//		Assert.True(orderingProperty.ContainedProperty);
//		Assert.AreEqual(TaskQueryProperty.NAME.Name, orderingProperty.QueryProperty.Name);
//		Assert.IsNull(orderingProperty.QueryProperty.Function);

//	  }

//	  /// <summary>
//	  /// Tests compatibility with serialization format that was used in 7.2
//	  /// </summary>
////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("deprecation") public void testDeprecatedOrderingFormatDeserializationSecondaryOrdering()
//	  public virtual void testDeprecatedOrderingFormatDeserializationSecondaryOrdering()
//	  {
//		string sortByNameAsc = "RES." + TaskQueryProperty.NAME.Name + " " + Direction.ASCENDING.Name;
//		string secondaryOrdering = sortByNameAsc + ", RES." + TaskQueryProperty.ASSIGNEE.Name + " " + Direction.DESCENDING.Name;

//		JsonTaskQueryConverter converter = (JsonTaskQueryConverter) FilterEntity.queryConverter.Get(EntityTypes.Task);
//		JSONObject queryJson = converter.ToJsonObject(filter.GetQuery<IQueryable<ITask>>());

//		// when I apply a secondary ordering
//		queryJson.put(JsonTaskQueryConverter.ORDER_BY, secondaryOrdering);
//		TaskQueryImpl deserializedTaskQuery = (TaskQueryImpl) converter.ToObject(queryJson);

//		// then the ordering is applied accordingly
//		Assert.AreEqual(2, deserializedTaskQuery.OrderingProperties.Count());

//		QueryOrderingProperty orderingProperty1 = deserializedTaskQuery.OrderingProperties.Get(0);
//		Assert.IsNull(orderingProperty1.Relation);
//		Assert.AreEqual("asc", orderingProperty1.Direction.Name);
//		Assert.IsNull(orderingProperty1.RelationConditions);
//		Assert.True(orderingProperty1.ContainedProperty);
//		Assert.AreEqual(TaskQueryProperty.NAME.Name, orderingProperty1.QueryProperty.Name);
//		Assert.IsNull(orderingProperty1.QueryProperty.Function);

//		QueryOrderingProperty orderingProperty2 = deserializedTaskQuery.OrderingProperties.Get(1);
//		Assert.IsNull(orderingProperty2.Relation);
//		Assert.AreEqual("Desc", orderingProperty2.Direction.Name);
//		Assert.IsNull(orderingProperty2.RelationConditions);
//		Assert.True(orderingProperty2.ContainedProperty);
//		Assert.AreEqual(TaskQueryProperty.ASSIGNEE.Name, orderingProperty2.QueryProperty.Name);
//		Assert.IsNull(orderingProperty2.QueryProperty.Function);
//	  }

//	  /// <summary>
//	  /// Tests compatibility with serialization format that was used in 7.2
//	  /// </summary>
////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("deprecation") public void testDeprecatedOrderingFormatDeserializationFunctionOrdering()
//	  public virtual void testDeprecatedOrderingFormatDeserializationFunctionOrdering()
//	  {
//		string orderingWithFunction = "LOWER(RES." + TaskQueryProperty.NAME.Name + ") asc";

//		JsonTaskQueryConverter converter = (JsonTaskQueryConverter) FilterEntity.queryConverter.Get(EntityTypes.Resources.Task);
//		JSONObject queryJson = converter.ToJsonObject(filter.GetQuery<IQueryable<ITask>>());

//		// when I apply an ordering with a function
//		queryJson.put(JsonTaskQueryConverter.ORDER_BY, orderingWithFunction);
//		TaskQueryImpl deserializedTaskQuery = (TaskQueryImpl) converter.ToObject(queryJson);

//		Assert.AreEqual(1, deserializedTaskQuery.OrderingProperties.Count());

//		// then the ordering is applied accordingly
//		QueryOrderingProperty orderingProperty = deserializedTaskQuery.OrderingProperties.Get(0);
//		Assert.IsNull(orderingProperty.Relation);
//		Assert.AreEqual("asc", orderingProperty.Direction.Name);
//		Assert.IsNull(orderingProperty.RelationConditions);
//		Assert.IsFalse(orderingProperty.ContainedProperty);
//		Assert.AreEqual(TaskQueryProperty.NAME_CASE_INSENSITIVE.Name, orderingProperty.QueryProperty.Name);
//		Assert.AreEqual(TaskQueryProperty.NAME_CASE_INSENSITIVE.Function, orderingProperty.QueryProperty.Function);
//	  }


////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(resources={"resources/api/task/oneTaskWithFormKeyProcess.bpmn20.xml"}) public void testInitializeFormKeysEnabled()
//	  public virtual void testInitializeFormKeysEnabled()
//	  {
//		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id);

//		saveQuery(query);

//		ITask task = (Task) filterService.List(filter.Id).Get(0);

//		Assert.AreEqual("exampleFormKey", task.FormKey);

//		task = filterService.singleResult(filter.Id);

//		Assert.AreEqual("exampleFormKey", task.FormKey);

//		runtimeService.DeleteProcessInstance(processInstance.Id, "test");
//	  }

//	  public virtual void testExtendingVariableQuery()
//	  {
//		IQueryable<ITask> taskQuery = taskService.CreateTaskQuery()//.ProcessVariableValueEquals("hello", "world");
//		saveQuery(taskQuery);

//		// variables won't overridden variables with same name in different scopes
//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery()//.TaskVariableValueEquals("hello", "world").CaseInstanceVariableValueEquals("hello", "world");

//		IFilter extendedFilter = filter.Extend(extendingQuery);
//		TaskQueryImpl extendedQuery = extendedFilter.Query;
//		IList<TaskQueryVariableValue> variables = extendedQuery.Variables;

//		Assert.AreEqual(3, variables.Count);

//		// Assert variables (ordering: extending variables are inserted first)
//		Assert.AreEqual("hello", variables[0].Name);
//		Assert.AreEqual("world", variables[0].Value);
//		Assert.AreEqual(QueryOperator.EQUALS, variables[0].Operator);
//		Assert.IsFalse(variables[0].ProcessInstanceVariable);
//		Assert.True(variables[0].Local);
//		Assert.AreEqual("hello", variables[1].Name);
//		Assert.AreEqual("world", variables[1].Value);
//		Assert.AreEqual(QueryOperator.EQUALS, variables[1].Operator);
//		Assert.IsFalse(variables[1].ProcessInstanceVariable);
//		Assert.IsFalse(variables[1].Local);
//		Assert.AreEqual("hello", variables[2].Name);
//		Assert.AreEqual("world", variables[2].Value);
//		Assert.AreEqual(QueryOperator.EQUALS, variables[2].Operator);
//		Assert.True(variables[2].ProcessInstanceVariable);
//		Assert.IsFalse(variables[2].Local);

//		// variables will override variables with same name in same scope
//		extendingQuery = taskService.CreateTaskQuery().ProcessVariableValueLessThan("hello", 42).TaskVariableValueLessThan("hello", 42).CaseInstanceVariableValueLessThan("hello", 42);

//		extendedFilter = filter.Extend(extendingQuery);
//		extendedQuery = extendedFilter.Query;
//		variables = extendedQuery.Variables;

//		Assert.AreEqual(3, variables.Count);

//		// Assert variables (ordering: extending variables are inserted first)
//		Assert.AreEqual("hello", variables[0].Name);
//		Assert.AreEqual(42, variables[0].Value);
//		Assert.AreEqual(QueryOperator.LESS_THAN, variables[0].Operator);
//		Assert.True(variables[0].ProcessInstanceVariable);
//		Assert.IsFalse(variables[0].Local);
//		Assert.AreEqual("hello", variables[1].Name);
//		Assert.AreEqual(42, variables[1].Value);
//		Assert.AreEqual(QueryOperator.LESS_THAN, variables[1].Operator);
//		Assert.IsFalse(variables[1].ProcessInstanceVariable);
//		Assert.True(variables[1].Local);
//		Assert.AreEqual("hello", variables[2].Name);
//		Assert.AreEqual(42, variables[2].Value);
//		Assert.AreEqual(QueryOperator.LESS_THAN, variables[2].Operator);
//		Assert.IsFalse(variables[2].ProcessInstanceVariable);
//		Assert.IsFalse(variables[2].Local);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(resources = {"resources/api/oneTaskProcess.bpmn20.xml"}) public void testExtendTaskQueryByOrderByProcessVariable()
//	  public virtual void testExtendTaskQueryByOrderByProcessVariable()
//	  {
//		IProcessInstance instance500 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variables.CreateVariables().PutValue("var", 500));
//		IProcessInstance instance1000 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variables.CreateVariables().PutValue("var", 1000));
//		IProcessInstance instance250 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variables.CreateVariables().PutValue("var", 250));

//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess");
//		saveQuery(query);

//		// asc
//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery().OrderByProcessVariable("var", ValueType.INTEGER)/*.Asc()*/;

//		IList<ITask> tasks = filterService.List(filter.Id, extendingQuery);

//		Assert.AreEqual(3, tasks.Count);
//		Assert.AreEqual(instance250.Id, tasks[0].ProcessInstanceId);
//		Assert.AreEqual(instance500.Id, tasks[1].ProcessInstanceId);
//		Assert.AreEqual(instance1000.Id, tasks[2].ProcessInstanceId);

//		// Desc
//		extendingQuery = taskService.CreateTaskQuery().OrderByProcessVariable("var", ValueType.INTEGER)/*.Desc()*/;

//		tasks = filterService.List(filter.Id, extendingQuery);

//		Assert.AreEqual(3, tasks.Count);
//		Assert.AreEqual(instance1000.Id, tasks[0].ProcessInstanceId);
//		Assert.AreEqual(instance500.Id, tasks[1].ProcessInstanceId);
//		Assert.AreEqual(instance250.Id, tasks[2].ProcessInstanceId);

//		runtimeService.DeleteProcessInstance(instance250.Id, null);
//		runtimeService.DeleteProcessInstance(instance500.Id, null);
//		runtimeService.DeleteProcessInstance(instance1000.Id, null);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(resources = {"resources/api/oneTaskProcess.bpmn20.xml"}) public void testExtendTaskQueryByOrderByTaskVariable()
//	  public virtual void testExtendTaskQueryByOrderByTaskVariable()
//	  {
//		IProcessInstance instance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//		IProcessInstance instance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//		IProcessInstance instance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//		ITask task500 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance1.Id).First();
//		taskService.SetVariableLocal(task500.Id, "var", 500);

//		ITask task250 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance2.Id).First();
//		taskService.SetVariableLocal(task250.Id, "var", 250);

//		ITask task1000 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance3.Id).First();
//		taskService.SetVariableLocal(task1000.Id, "var", 1000);

//		IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess");
//		saveQuery(query);

//		// asc
//		IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery().OrderByProcessVariable("var", ValueType.INTEGER)/*.Asc()*/;

//		IList<ITask> tasks = filterService.List(filter.Id, extendingQuery);

//		Assert.AreEqual(3, tasks.Count);
//		Assert.AreEqual(task250.Id, tasks[0].Id);
//		Assert.AreEqual(task500.Id, tasks[1].Id);
//		Assert.AreEqual(task1000.Id, tasks[2].Id);

//		// Desc
//		extendingQuery = taskService.CreateTaskQuery().OrderByProcessVariable("var", ValueType.INTEGER)/*.Desc()*/;

//		tasks = filterService.List(filter.Id, extendingQuery);

//		Assert.AreEqual(3, tasks.Count);
//		Assert.AreEqual(task1000.Id, tasks[0].Id);
//		Assert.AreEqual(task500.Id, tasks[1].Id);
//		Assert.AreEqual(task250.Id, tasks[2].Id);

//		runtimeService.DeleteProcessInstance(instance1.Id, null);
//		runtimeService.DeleteProcessInstance(instance2.Id, null);
//		runtimeService.DeleteProcessInstance(instance3.Id, null);
//	  }

//	  protected internal virtual void saveQuery(Query query)
//	  {
//		filter.Query = query;
//		filterService.SaveFilter(filter);
//		filter = filterService.GetFilter(filter.Id);
//	  }

//	  protected internal virtual void createTasks()
//	  {
//		ITask task = taskService.NewTask("task1");
//		task.Name = "ITask 1";
//		task.Owner = testUser.Id;
//		task.DelegationState = DelegationState.PENDING;
//		taskService.SaveTask(task);
//		taskService.AddCandidateGroup(task.Id, "accounting");

//		task = taskService.NewTask("task2");
//		task.Name = "ITask 2";
//		task.Owner = testUser.Id;
//		task.DelegationState = DelegationState.RESOLVED;
//		taskService.SaveTask(task);
//		taskService.SetAssignee(task.Id, "kermit");
//		taskService.AddCandidateGroup(task.Id, "accounting");

//		task = taskService.NewTask("task3");
//		task.Name = "ITask 3";
//		task.Owner = testUser.Id;
//		task.DelegationState = DelegationState.RESOLVED;
//		taskService.SaveTask(task);
//	  }

//	}

//}

