using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using NUnit.Framework;

namespace Engine.Tests.Api.History
{
    /// <summary>
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT) public class HistoryServiceAsyncOperationsTest extends api.AbstractAsyncOperationsTest
    [TestFixture]
    public class HistoryServiceAsyncOperationsTest : AbstractAsyncOperationsTest
    {
        [SetUp]
        public override void initServices()
        {
            base.initServices();
            taskService = engineRule.TaskService;
            prepareData();
        }

        private readonly bool InstanceFieldsInitialized;

        public HistoryServiceAsyncOperationsTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
        }


        protected internal const string TEST_REASON = "test reason";

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public ExpectedException thrown = ExpectedException.None();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

        protected internal ITaskService taskService;
        protected internal IList<string> historicProcessInstances;

        public virtual void prepareData()
        {
            testRule.Deploy("resources/api/oneTaskProcess.bpmn20.xml");
            startTestProcesses(2);

            foreach (var activeTask in taskService.CreateTaskQuery()
                
                .ToList())
                taskService.Complete(activeTask.Id);

            historicProcessInstances = new List<string>();
            foreach (var pi in historyService.CreateHistoricProcessInstanceQuery()
                
                .ToList())
                historicProcessInstances.Add(pi.Id);
        }

        [TearDown]
        public virtual void cleanBatch()
        {
            var batch = managementService.CreateBatchQuery()
                .First();
            if (batch != null)
                managementService.DeleteBatch(batch.Id, true);

            var historicBatch = historyService.CreateHistoricBatchQuery()
                .First();
            if (historicBatch != null)
                historyService.DeleteHistoricBatch(historicBatch.Id);
        }
        [Test]
        public virtual void testDeleteHistoryProcessInstancesAsyncWithList()
        {
            //when
            var batch = historyService.DeleteHistoricProcessInstancesAsync(historicProcessInstances, TEST_REASON);

            executeSeedJob(batch);
            var exceptions = executeBatchJobs(batch);

            // then
            Assert.That(exceptions.Count, Is.EqualTo(0));
            AssertNoHistoryForTasks();
            AssertHistoricBatchExists(testRule);
            AssertAllHistoricProcessInstancesAreDeleted();
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testDeleteHistoryProcessInstancesAsyncWithEmptyList()
        {
            //expect
            //thrown.Expect(typeof(ProcessEngineException));

            //when
            historyService.DeleteHistoricProcessInstancesAsync(new List<string>(), TEST_REASON);
        }

        [Test]
        public virtual void testDeleteHistoryProcessInstancesAsyncWithNonExistingID()
        {
            //given
            var processInstanceIds = new List<string>();
            processInstanceIds.Add(historicProcessInstances[0]);
            processInstanceIds.Add("aFakeId");

            //when
            var batch = historyService.DeleteHistoricProcessInstancesAsync(processInstanceIds, TEST_REASON);
            executeSeedJob(batch);
            var exceptions = executeBatchJobs(batch);

            //then
            Assert.That(exceptions.Count, Is.EqualTo(1));
            AssertHistoricBatchExists(testRule);
        }


        [Test]
        public virtual void testDeleteHistoryProcessInstancesAsyncWithQueryAndList()
        {
            //given
            var query = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId== historicProcessInstances[0]);
            var batch = historyService.DeleteHistoricProcessInstancesAsync(historicProcessInstances.ToList()
                .GetRange(1, historicProcessInstances.Count), query, TEST_REASON);
            executeSeedJob(batch);

            //when
            var exceptions = executeBatchJobs(batch);

            // then
            Assert.That(exceptions.Count, Is.EqualTo(0));
            AssertNoHistoryForTasks();
            AssertHistoricBatchExists(testRule);
            AssertAllHistoricProcessInstancesAreDeleted();
        }

        [Test]
        public virtual void testDeleteHistoryProcessInstancesAsyncWithQuery()
        {
            //given
            var query = historyService.CreateHistoricProcessInstanceQuery()
                ;//.ProcessInstanceIds(new HashSet<string>(historicProcessInstances));
            var batch = historyService.DeleteHistoricProcessInstancesAsync(query, TEST_REASON);
            executeSeedJob(batch);

            //when
            var exceptions = executeBatchJobs(batch);

            // then
            Assert.That(exceptions.Count, Is.EqualTo(0));
            AssertNoHistoryForTasks();
            AssertHistoricBatchExists(testRule);
            AssertAllHistoricProcessInstancesAreDeleted();
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testDeleteHistoryProcessInstancesAsyncWithEmptyQuery()
        {
            //expect
            //thrown.Expect(typeof(ProcessEngineException));
            //given
            var query = historyService.CreateHistoricProcessInstanceQuery()
                /*.Unfinished()*/;
            //when
            historyService.DeleteHistoricProcessInstancesAsync(query, TEST_REASON);
        }

        [Test]
        public virtual void testDeleteHistoryProcessInstancesAsyncWithNonExistingIDAsQuery()
        {
            //given
            var processInstanceIds = new List<string>();
            processInstanceIds.Add(historicProcessInstances[0]);
            processInstanceIds.Add("aFakeId");
            var query = historyService.CreateHistoricProcessInstanceQuery()
               ;// .ProcessInstanceIds(new HashSet<string>(processInstanceIds));

            //when
            var batch = historyService.DeleteHistoricProcessInstancesAsync(query, TEST_REASON);
            executeSeedJob(batch);
            executeBatchJobs(batch);

            //then
            AssertHistoricBatchExists(testRule);
        }

        [Test]
        public virtual void testDeleteHistoryProcessInstancesAsyncWithoutDeleteReason()
        {
            //when
            var batch = historyService.DeleteHistoricProcessInstancesAsync(historicProcessInstances, null);
            executeSeedJob(batch);
            var exceptions = executeBatchJobs(batch);

            //then
            Assert.That(exceptions.Count, Is.EqualTo(0));
            AssertNoHistoryForTasks();
            AssertHistoricBatchExists(testRule);
            AssertAllHistoricProcessInstancesAreDeleted();
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testDeleteHistoryProcessInstancesAsyncWithNullList()
        {
            //thrown.Expect(typeof(ProcessEngineException));
            historyService.DeleteHistoricProcessInstancesAsync((IList<string>) null, TEST_REASON);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testDeleteHistoryProcessInstancesAsyncWithNullQuery()
        {
            //thrown.Expect(typeof(ProcessEngineException));
            historyService.DeleteHistoricProcessInstancesAsync((IQueryable<IHistoricProcessInstance>) null, TEST_REASON);
        }

        protected internal virtual void AssertNoHistoryForTasks()
        {
            if (!testRule.HistoryLevelNone)
                Assert.That(historyService.CreateHistoricTaskInstanceQuery()
                    .Count(), Is.EqualTo(0L));
        }

        protected internal virtual void AssertAllHistoricProcessInstancesAreDeleted()
        {
            Assert.That(historyService.CreateHistoricProcessInstanceQuery()
                .Count(), Is.EqualTo(0L));
        }
    }
}