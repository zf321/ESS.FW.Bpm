//using System.Collections.Generic;
//using System.Linq.Dynamic;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using NUnit.Framework;
//using Enumerable = System.Linq.Enumerable;
//using Queryable = System.Linq.Queryable;
//using System.Linq;

//namespace ESS.FW.Bpm.Engine.Tests.History
//{
//    /// <summary>
//    /// </summary>
//    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
//    [TestFixture]
//    public class HistoricCaseActivityStatisticsQueryTest
//    {
//        [SetUp]
//        public virtual void setUp()
//        {
//            historyService = engineRule.HistoryService;
//            caseService = engineRule.CaseService;
//            repositoryService = engineRule.RepositoryService;
//        }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Rule public ProcessEngineRule engineRule = new util.ProvidedProcessEngineRule();
//        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//        protected internal IHistoryService historyService;
//        protected internal ICaseService caseService;
//        protected internal IRepositoryService repositoryService;

//        [Test]
//        public virtual void testCaseDefinitionNull()
//        {
//            // given

//            // when
//            try
//            {
//                historyService.CreateHistoricCaseActivityStatisticsQuery(null);
//                Assert.Fail("It should not be possible to query for statistics by null.");
//            }
//            catch (NullValueException)
//            {
//            }
//        }

//        [Test][Deployment( "resources/api/cmmn/oneTaskCase.cmmn") ]
//        public virtual void testNoCaseActivityInstances()
//        {
//            // given
//            var caseDefinitionId = ICaseDefinition.Id;

//            // when
//            var query = historyService.CreateHistoricCaseActivityStatisticsQuery(c => c.Id == ICaseDefinition.Id);

//            // then
//            Assert.AreEqual(0, query.Count());
//            Assert.That(query.Count()
//                , Is.EqualTo(0));
//        }

//        [Test]
//        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
//        public virtual void testSingleTask()
//        {
//            // given
//            var caseDefinitionId = CaseDefinition.Id;

//            createCaseByKey(5, "oneTaskCase");

//            // when
//            var query = historyService.CreateHistoricCaseActivityStatisticsQuery(c => c.Id == ICaseDefinition.Id);

//            // then
//            var statistics = query;

//            Assert.AreEqual(1, query.Count());
//            Assert.That(statistics.Count, Is.EqualTo(1));
//            AssertStatisitcs(Queryable.First(statistics), "PI_HumanTask_1", 5, 0, 0, 0, 0, 0);
//        }

//        [Test][Deployment( "resources/history/HistoricCaseActivityStatisticsQueryTest.TestMultipleTasks.cmmn" )]
//        public virtual void testStateCount()
//        {
//            // given
//            var caseDefinitionId = ICaseDefinition.Id;

//            createCaseByKey(3, "case");
//            completeByActivity("ACTIVE");
//            manuallyStartByActivity("AVAILABLE");
//            completeByActivity("AVAILABLE");

//            createCaseByKey(5, "case");
//            completeByActivity("ACTIVE");
//            disableByActivity("AVAILABLE");
//            reenableByActivity("AVAILABLE");
//            manuallyStartByActivity("AVAILABLE");
//            terminateByActivity("AVAILABLE");

//            createCaseByKey(5, "case");
//            terminateByActivity("ACTIVE");

//            manuallyStartByActivity("ENABLED");
//            completeByActivity("ENABLED");

//            manuallyStartByActivity("DISABLED");
//            terminateByActivity("DISABLED");

//            createCaseByKey(2, "case");
//            disableByActivity("DISABLED");

//            // when
//            var query = historyService.CreateHistoricCaseActivityStatisticsQuery(c => c.Id == ICaseDefinition.Id);

//            // then
//            var statistics = Enumerable.ToList(query);
//            Assert.That(statistics.Count, Is.EqualTo(6));
//            Assert.AreEqual(query.Count(), 6);

//            AssertStatisitcs(statistics[0], "ACTIVE", 2, 0, 8, 0, 0, 5);
//            AssertStatisitcs(statistics[1], "AVAILABLE", 0, 7, 3, 0, 0, 5);
//            AssertStatisitcs(statistics[2], "COMPLETED", 15, 0, 0, 0, 0, 0);
//            AssertStatisitcs(statistics[3], "DISABLED", 0, 0, 0, 2, 0, 13);
//            AssertStatisitcs(statistics[4], "ENABLED", 0, 0, 13, 0, 2, 0);
//            AssertStatisitcs(statistics[5], "TERMINATED", 15, 0, 0, 0, 0, 0);
//        }

//        [Test][Deployment( new []{ "resources/api/cmmn/oneTaskCase.cmmn", "resources/history/HistoricCaseActivityStatisticsQueryTest.TestMultipleTasks.cmmn" }) ]
//        public virtual void testMultipleCaseDefinitions()
//        {
//            // given
//            var caseDefinitionId1 = getCaseDefinition("oneTaskCase")
//                .Id;
//            var caseDefinitionId2 = getCaseDefinition("case")
//                .Id;

//            createCaseByKey(5, "oneTaskCase");
//            createCaseByKey(10, "case");

//            // when
//            var query1 = historyService.CreateHistoricCaseActivityStatisticsQuery(c => c.Id == caseDefinitionId1);
//            var query2 = historyService.CreateHistoricCaseActivityStatisticsQuery(c => c.Id == caseDefinitionId2);

//            // then
//            Assert.That(query1.Count, Is.EqualTo(1));
//            Assert.That(query2
//                .Count, Is.EqualTo(6));
//        }

//        [Test][Deployment( new []{ "resources/history/HistoricCaseActivityStatisticsQueryTest.TestMultipleTasks.cmmn" }) ]
//        public virtual void testPagination()
//        {
//            // given
//            var caseDefinitionId = ICaseDefinition.Id;

//            createCaseByKey(5, "case");

//            // when
//            var statistics = historyService.CreateHistoricCaseActivityStatisticsQuery(c => c.Id == caseDefinitionId).ToList();
               
//               ;

//            // then
//            Assert.That(statistics.Count, Is.EqualTo(1));
//            Assert.That(statistics.First().Id, Is.EqualTo("COMPLETED"));
//        }

//        protected internal virtual void AssertStatisitcs(IHistoricCaseActivityStatistics statistics, string id,
//            long active, long availabe, long completed, long disabled, long enabled, long terminated)
//        {
//            Assert.That(statistics.Id, Is.EqualTo(id));
//            Assert.AreEqual(active, statistics.Active);
//            Assert.AreEqual(availabe, statistics.Available);
//            Assert.AreEqual(completed, statistics.Completed);
//            Assert.AreEqual(disabled, statistics.Disabled);
//            Assert.AreEqual(enabled, statistics.Enabled);
//            Assert.AreEqual(terminated, statistics.Terminated);
//        }

//        protected internal virtual void createCaseByKey(int numberOfInstances, string key)
//        {
//            for (var i = 0; i < numberOfInstances; i++)
//                caseService.CreateCaseInstanceByKey(key);
//        }

//        //protected internal virtual ICaseDefinition ICaseDefinition
//        //{
//        //    get
//        //    {
//        //        return repositoryService.CreateCaseDefinitionQuery(null)
//        //            .First();
//        //    }
//        //}

//        //protected internal virtual ICaseDefinition getCaseDefinition(string key)
//        //{
//        //    return repositoryService.CreateCaseDefinitionQuery(null)
//        //        //.CaseDefinitionKey(key)
//        //        .First();
//        //}

//        protected internal virtual IList<ICaseExecution> getCaseExecutionsByActivity(string activityId)
//        {
//            return caseService.CreateCaseExecutionQuery()
//             //   .ActivityId(activityId)
//               // 
//                .ToList();
//        }

//        protected internal virtual void disableByActivity(string activityId)
//        {
//            var executions = getCaseExecutionsByActivity(activityId);
//            foreach (var caseExecution in executions)
//                caseService.DisableCaseExecution(caseExecution.Id);
//        }

//        protected internal virtual void reenableByActivity(string activityId)
//        {
//            var executions = getCaseExecutionsByActivity(activityId);
//            foreach (var caseExecution in executions)
//                caseService.ReenableCaseExecution(caseExecution.Id);
//        }

//        protected internal virtual void manuallyStartByActivity(string activityId)
//        {
//            var executions = getCaseExecutionsByActivity(activityId);
//            foreach (var caseExecution in executions)
//                caseService.ManuallyStartCaseExecution(caseExecution.Id);
//        }

//        protected internal virtual void completeByActivity(string activityId)
//        {
//            var executions = getCaseExecutionsByActivity(activityId);
//            foreach (var caseExecution in executions)
//                caseService.CompleteCaseExecution(caseExecution.Id);
//        }

//        protected internal virtual void terminateByActivity(string activityId)
//        {
//            var executions = getCaseExecutionsByActivity(activityId);
//            foreach (var caseExecution in executions)
//                caseService.TerminateCaseExecution(caseExecution.Id);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testMultipleTasks()
//        {
//            // given
//            var caseDefinitionId = ICaseDefinition.Id;

//            createCaseByKey(5, "case");

//            disableByActivity("DISABLED");
//            completeByActivity("COMPLETED");
//            terminateByActivity("TERMINATED");

//            // when
//            var query = historyService.CreateHistoricCaseActivityStatisticsQuery(c => c.Id == caseDefinitionId);

//            // then
//            var statistics = query.ToList();
//                .ToList();
//            Assert.That(statistics.Count(), Is.EqualTo(6));
//            Assert.AreEqual(query.Count(), 6);

//            AssertStatisitcs(statistics[0], "ACTIVE", 5, 0, 0, 0, 0, 0);
//            AssertStatisitcs(statistics[1], "AVAILABLE", 0, 5, 0, 0, 0, 0);
//            AssertStatisitcs(statistics[2], "COMPLETED", 0, 0, 5, 0, 0, 0);
//            AssertStatisitcs(statistics[3], "DISABLED", 0, 0, 0, 5, 0, 0);
//            AssertStatisitcs(statistics[4], "ENABLED", 0, 0, 0, 0, 5, 0);
//            AssertStatisitcs(statistics[5], "TERMINATED", 0, 0, 0, 0, 0, 5);
//        }
//    }
//}