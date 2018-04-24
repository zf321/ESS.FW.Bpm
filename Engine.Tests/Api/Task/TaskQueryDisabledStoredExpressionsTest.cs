//using System;
//using ESS.FW.Bpm.Engine.Authorization;
//using ESS.FW.Bpm.Engine.Filter;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using ESS.FW.Bpm.Engine.Task;
//using NUnit.Framework;
//using ResourceProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.ResourceProcessEngineTestCase;


//namespace ESS.FW.Bpm.Engine.Tests.Api.Task
//{
    
//	/// <summary>
//	/// 
//	/// 
//	/// </summary>
//	[TestFixture]
//	public class TaskQueryDisabledStoredExpressionsTest : ResourceProcessEngineTestCase
//	{


//	  protected internal const string EXPECTED_STORED_QUERY_FAILURE_MESSAGE = "Expressions are forbidden in stored queries. This behavior can be toggled in the process engine configuration";
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//	  public static readonly string STATE_MANIPULATING_EXPRESSION = "${''.GetClass().ForName('" + typeof(TaskQueryDisabledStoredExpressionsTest).FullName + "').GetField('MUTABLE_FIELD').SetLong(null, 42)}";

//	  public static long MUTABLE_FIELD = 0;

//	  public TaskQueryDisabledStoredExpressionsTest() : base("resources/api/task/task-query-disabled-stored-expressions-test.Camunda.cfg.xml")
//	  {
//	  }

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: protected void setUp() throws Exception
//        [SetUp]
//	  protected internal virtual void setUp()
//	  {
//		MUTABLE_FIELD = 0;
//	  }

//        [Test]
//        public virtual void testStoreFilterWithoutExpression()
//	  {
//		IQueryable<ITask> taskQuery = taskService.CreateTaskQuery().DueAfter(DateTime.Now);
//		IFilter filter = filterService.NewTaskFilter("filter");
//		//filter.Query = taskQuery;

//		// saving the filter suceeds
//		filterService.SaveFilter(filter);
//		Assert.AreEqual(1, filterService.CreateFilterQuery().Count());

//		// cleanup
//		filterService.DeleteFilter(filter.Id);
//	  }

//        [Test]
//        public virtual void testStoreFilterWithExpression()
//	  {
//		IQueryable<ITask> taskQuery = taskService.CreateTaskQuery().DueAfterExpression(STATE_MANIPULATING_EXPRESSION);
//		IFilter filter = filterService.NewTaskFilter("filter");
//		//filter.Query = taskQuery;

//		try
//		{
//		  filterService.SaveFilter(filter);
//		}
//		catch (ProcessEngineException e)
//		{
//		  AssertTextPresent(EXPECTED_STORED_QUERY_FAILURE_MESSAGE, e.Message);
//		}
//		Assert.True(fieldIsUnchanged());
//	  }

//        [Test]
//        public virtual void testUpdateFilterWithExpression()
//	  {
//		// given a stored filter
//		IQueryable<ITask> taskQuery = taskService.CreateTaskQuery().DueAfter(DateTime.Now);
//		IFilter filter = filterService.NewTaskFilter("filter");
//		//filter.Query = taskQuery;
//		filterService.SaveFilter(filter);

//		// updating the filter with an expression does not suceed
//		//filter.Query = taskQuery.DueBeforeExpression(STATE_MANIPULATING_EXPRESSION);
//		Assert.AreEqual(1, filterService.CreateFilterQuery().Count());

//		try
//		{
//		  filterService.SaveFilter(filter);
//		}
//		catch (ProcessEngineException e)
//		{
//		  AssertTextPresent(EXPECTED_STORED_QUERY_FAILURE_MESSAGE, e.Message);
//		}
//		Assert.True(fieldIsUnchanged());

//		// cleanup
//		filterService.DeleteFilter(filter.Id);
//	  }

//        [Test]
//        public virtual void testCannotExecuteStoredFilter()
//	  {
//        IQueryable<ITask> filterQuery = taskService.CreateTaskQuery().DueAfterExpression(STATE_MANIPULATING_EXPRESSION);

//		// store a filter bypassing validation
//		// the API way of doing this would be by reconfiguring the engine
//		string filterId = processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, filterQuery));

//		extendFilterAndValidateFailingQuery(filterId, null);

//		// cleanup
//		filterService.DeleteFilter(filterId);
//	  }

//	  private class CommandAnonymousInnerClass : ICommand<string>
//	  {
//		  private readonly TaskQueryDisabledStoredExpressionsTest outerInstance;

//		  private IQueryable<ITask> FilterQuery;

//		  public CommandAnonymousInnerClass(TaskQueryDisabledStoredExpressionsTest outerInstance, IQueryable<ITask> filterQuery)
//		  {
//			  this.outerInstance = outerInstance;
//			  this.FilterQuery = filterQuery;
//		  }


//		  public virtual string Execute(CommandContext commandContext)
//		  {
//			FilterEntity filter = new FilterEntity(EntityTypes.Task);
//			//filter.Query = FilterQuery;
//			filter.Name = "filter";
//			commandContext.DbEntityManager.Insert(filter);
//			return filter.Id;
//		  }
//	  }

//	  protected internal virtual bool fieldIsUnchanged()
//	  {
//		return MUTABLE_FIELD == 0;
//	  }

//	  protected internal virtual void extendFilterAndValidateFailingQuery(string filterId, IQueryable<ITask> query)
//	  {
//		try
//		{
//		  filterService.List<String>(filterId);//, query
//            }
//		catch (BadUserRequestException e)
//		{
//		  AssertTextPresent(EXPECTED_STORED_QUERY_FAILURE_MESSAGE, e.Message);
//		}

//		Assert.True(fieldIsUnchanged());

//		try
//		{
//		  filterService.List<string>(filterId);//.Count(filterId, query);//filterId, query
//            }
//		catch (BadUserRequestException e)
//		{
//		  AssertTextPresent(EXPECTED_STORED_QUERY_FAILURE_MESSAGE, e.Message);
//		}

//		Assert.True(fieldIsUnchanged());
//	  }
//	}

//}