//using System.Linq;
//using ESS.FW.Bpm.Engine.Authorization;
//using ESS.FW.Bpm.Engine.Filter;
//using ESS.FW.Bpm.Engine.Task;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Task
//{
//    /// <summary>
//    /// </summary>
//    [TestFixture]
//    public class TaskQueryDisabledAdhocExpressionsTest : PluggableProcessEngineTestCase
//    {
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: protected void setUp() throws Exception
//        [SetUp]
//        protected internal virtual void setUp()
//        {
//            MUTABLE_FIELD = 0;
//        }

//        protected internal const string EXPECTED_ADHOC_QUERY_FAILURE_MESSAGE =
//            "Expressions are forbidden in adhoc queries. " +
//            "This behavior can be toggled in the process engine configuration";

////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//        public static readonly string STATE_MANIPULATING_EXPRESSION = "${''.GetClass().ForName('" +
//                                                                      typeof(TaskQueryDisabledAdhocExpressionsTest)
//                                                                          .FullName +
//                                                                      "').GetField('MUTABLE_FIELD').SetLong(null, 42)}";

//        public static long MUTABLE_FIELD;

//        public virtual void testDefaultSetting()
//        {
//            Assert.True(processEngineConfiguration.EnableExpressionsInStoredQueries);
//            Assert.IsFalse(processEngineConfiguration.EnableExpressionsInAdhocQueries);
//        }

//        protected internal virtual bool fieldIsUnchanged()
//        {
//            return MUTABLE_FIELD == 0;
//        }

//        protected internal virtual void executeAndValidateFailingQuery(IQueryable<ITask> query)
//        {
//            try
//            {
//                query.ToList();
//            }
//            catch (BadUserRequestException e)
//            {
//                AssertTextPresent(EXPECTED_ADHOC_QUERY_FAILURE_MESSAGE, e.Message);
//            }

//            Assert.True(fieldIsUnchanged());

//            try
//            {
//                query.Count();
//            }
//            catch (BadUserRequestException e)
//            {
//                AssertTextPresent(EXPECTED_ADHOC_QUERY_FAILURE_MESSAGE, e.Message);
//            }

//            Assert.True(fieldIsUnchanged());
//        }

//        protected internal virtual void extendFilterAndValidateFailingQuery(IFilter filter, IQueryable<ITask> query)
//        {
//            try
//            {
//                filterService.List<IQueryable<ITask>>(filter.Id); //filter.Id, query
//            }
//            catch (BadUserRequestException e)
//            {
//                AssertTextPresent(EXPECTED_ADHOC_QUERY_FAILURE_MESSAGE, e.Message);
//            }

//            Assert.True(fieldIsUnchanged());

//            try
//            {
//                //  filterService.Count<IQueryable<ITask>>(filter.Id, query);
//            }
//            catch (BadUserRequestException e)
//            {
//                AssertTextPresent(EXPECTED_ADHOC_QUERY_FAILURE_MESSAGE, e.Message);
//            }

//            Assert.True(fieldIsUnchanged());
//        }

//        [Test]
//        public virtual void testAdhocExpressionsFail()
//        {
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .DueAfterExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .DueBeforeExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .DueDateExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .FollowUpAfterExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .FollowUpBeforeExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .FollowUpBeforeOrNotExistentExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .FollowUpDateExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .TaskAssigneeExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .TaskAssigneeLikeExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .TaskCandidateGroupExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .TaskCandidateGroupInExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .TaskCandidateUserExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .TaskCreatedAfterExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .TaskCreatedBeforeExpression(STATE_MANIPULATING_EXPRESSION));
//            executeAndValidateFailingQuery(taskService.CreateTaskQuery()
//                .TaskOwnerExpression(STATE_MANIPULATING_EXPRESSION));
//        }

//        [Test]
//        public virtual void testExtendStoredFilterByExpression()
//        {
//            // given a stored filter
//            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery()
//                .DueAfterExpression("${now()}");
//            var filter = filterService.NewTaskFilter("filter");
//            //filter.Query = taskQuery;
//            filterService.SaveFilter(filter);

//            // it is possible to execute the stored query with an expression
//            Assert.AreEqual(0, filterService.Count(filter.Id));
//            Assert.AreEqual(0, filterService.List<string>(filter.Id)
//                .Count);

//            // but it is not possible to executed the filter with an extended query that uses expressions
//            extendFilterAndValidateFailingQuery(filter, taskService.CreateTaskQuery()
//                .DueAfterExpression(STATE_MANIPULATING_EXPRESSION));

//            // cleanup
//            filterService.DeleteFilter(filter.Id);
//        }

//        [Test]
//        public virtual void testExtendStoredFilterByScalar()
//        {
//            // given a stored filter
//            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery()
//                .DueAfterExpression("${now()}");
//            var filter = filterService.NewTaskFilter("filter");
//            //filter.Query = taskQuery;
//            filterService.SaveFilter(filter);

//            // it is possible to execute the stored query with an expression
//            Assert.AreEqual(0, filterService.Count(filter.Id));
//            Assert.AreEqual(0, filterService.List<string>(filter.Id)
//                .Count);

//            // and it is possible to extend the filter query when not using an expression
//            //Assert.AreEqual(0, filterService.Count<object>(filter.Id, taskService.CreateTaskQuery().DueAfter(DateTime.Now)));
//            Assert.AreEqual(0, filterService.List<string>(filter.Id)
//                .Count);
//            //, taskService.CreateTaskQuery().DueAfter(DateTime.Now)).Count());

//            // cleanup
//            filterService.DeleteFilter(filter.Id);
//        }
//    }
//}