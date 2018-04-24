using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ManagementServiceAsyncOperationsTest : AbstractAsyncOperationsTest
    {
        [SetUp]
        public override void initServices()
        {
            base.initServices();
            prepareData();
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

        private readonly bool InstanceFieldsInitialized;

        public ManagementServiceAsyncOperationsTest()
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

        protected internal const int RETRIES = 5;
        protected internal const string TEST_PROCESS = "exceptionInJobExecution";

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public ExpectedException thrown = ExpectedException.None();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

        protected internal IList<string> processInstanceIds;
        protected internal IList<string> ids;

        public virtual void prepareData()
        {
            testRule.Deploy("resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml");
            processInstanceIds = startTestProcesses(2);
            ids = AllJobIds;
        }

        protected internal virtual IList<string> AllJobIds
        {
            get
            {
                var result = new List<string>();
                foreach (var job in managementService.CreateJobQuery()
                    
                    .ToList())
                    if (job.ProcessInstanceId != null)
                        result.Add(job.Id);
                return result;
            }
        }

        protected internal override IList<string> startTestProcesses(int numberOfProcesses)
        {
            var ids = new List<string>();

            for (var i = 0; i < numberOfProcesses; i++)
                ids.Add(runtimeService.StartProcessInstanceByKey(TEST_PROCESS)
                    .ProcessInstanceId);

            return ids;
        }

        protected internal virtual void AssertRetries(IList<string> allJobIds, int i)
        {
            foreach (var id in allJobIds)
                Assert.That(managementService.CreateJobQuery(c=>c.Id==id)
                    .First()
                    .Retries, Is.EqualTo(i));
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testSetJobsRetryAsyncWithEmptyJobList()
        {
            //expect
            //thrown.Expect(typeof(ProcessEngineException));

            //when
            managementService.SetJobRetriesAsync(new List<string>(), RETRIES);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testSetJobsRetryAsyncWithEmptyJobQuery()
        {
            //expect
            //thrown.Expect(typeof(ProcessEngineException));

            //given
            var query =
                managementService.CreateJobQuery(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            //when
            //managementService.SetJobRetriesAsync(query, RETRIES);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testSetJobsRetryAsyncWithEmptyProcessList()
        {
            //expect
            //thrown.Expect(typeof(ProcessEngineException));

            //when
            managementService.SetJobRetriesAsync(new List<string>(), ( IQueryable<IProcessInstance>) null, RETRIES);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testSetJobsRetryAsyncWithEmptyProcessQuery()
        {
            //expect
            //thrown.Expect(typeof(ProcessEngineException));

            //given
            var query = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            //when
            managementService.SetJobRetriesAsync(null, query, RETRIES);
        }

        [Test]
        public virtual void testSetJobsRetryAsyncWithJobList()
        {
            //when
            var batch = managementService.SetJobRetriesAsync(ids, RETRIES);
            executeSeedJob(batch);
            var exceptions = executeBatchJobs(batch);

            // then
            Assert.That(exceptions.Count, Is.EqualTo(0));
            AssertRetries(ids, RETRIES);
            AssertHistoricBatchExists(testRule);
        }

        [Test]
        public virtual void testSetJobsRetryAsyncWithJobQuery()
        {
            //given
            var query = managementService.CreateJobQuery();

            //when
            //var batch = managementService.SetJobRetriesAsync(query, RETRIES);
            //executeSeedJob(batch);
            //var exceptions = executeBatchJobs(batch);

            // then
            //Assert.That(exceptions.Count, Is.EqualTo(0));
            AssertRetries(ids, RETRIES);
            AssertHistoricBatchExists(testRule);
        }

        [Test]
        public virtual void testSetJobsRetryAsyncWithJobQueryAndList()
        {
            //given
            var extraPi = startTestProcesses(1);
            var query = managementService.CreateJobQuery(c=>c.ProcessInstanceId== extraPi[0]);

            //when
            //var batch = managementService.SetJobRetriesAsync(ids, query, RETRIES);
            //executeSeedJob(batch);
            //var exceptions = executeBatchJobs(batch);

            // then
            //Assert.That(exceptions.Count, Is.EqualTo(0));
            AssertRetries(AllJobIds, RETRIES);
            AssertHistoricBatchExists(testRule);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testSetJobsRetryAsyncWithNegativeRetries()
        {
            //given
            var query = managementService.CreateJobQuery();

            //when
            //thrown.Expect(typeof(ProcessEngineException));
            //managementService.SetJobRetriesAsync(query, -1);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testSetJobsRetryAsyncWithNonExistingIDAsJobQuery()
        {
            //expect
            //thrown.Expect(typeof(ProcessEngineException));

            //given
            var query = managementService.CreateJobQuery(c=>c.Id == ids[0] && c.Id == "aFake");

            //when
            //managementService.SetJobRetriesAsync(query, RETRIES);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testSetJobsRetryAsyncWithNonExistingIDAsProcessQuery()
        {
            //expect
            //thrown.Expect(typeof(ProcessEngineException));

            //given
            var query = runtimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId == "aFake");

            //when
            managementService.SetJobRetriesAsync(null, query, RETRIES);
        }

        [Test]
        public virtual void testSetJobsRetryAsyncWithNonExistingJobID()
        {
            //given
            ids.Add("aFake");

            //when
            var batch = managementService.SetJobRetriesAsync(ids, RETRIES);
            executeSeedJob(batch);
            var exceptions = executeBatchJobs(batch);

            //then
            Assert.That(exceptions.Count, Is.EqualTo(1));
            AssertRetries(AllJobIds, RETRIES);
            AssertHistoricBatchExists(testRule);
        }

        [Test]
        public virtual void testSetJobsRetryAsyncWithNonExistingProcessID()
        {
            //given
            processInstanceIds.Add("aFake");

            //when
            var batch = managementService.SetJobRetriesAsync(processInstanceIds, ( IQueryable<IProcessInstance>) null, RETRIES);
            executeSeedJob(batch);
            var exceptions = executeBatchJobs(batch);

            //then
            Assert.That(exceptions.Count, Is.EqualTo(0));
            AssertRetries(AllJobIds, RETRIES);
            AssertHistoricBatchExists(testRule);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testSetJobsRetryAsyncWithNullJobList()
        {
            //expect
            //thrown.Expect(typeof(ProcessEngineException));

            //when
            managementService.SetJobRetriesAsync(new List<string>(), RETRIES);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testSetJobsRetryAsyncWithNullJobQuery()
        {
            //expect
            //thrown.Expect(typeof(ProcessEngineException));

            //when
            //managementService.SetJobRetriesAsync((IQueryable<IJob>) null, RETRIES);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testSetJobsRetryAsyncWithNullProcessQuery()
        {
            //expect
            //thrown.Expect(typeof(ProcessEngineException));

            //when
            managementService.SetJobRetriesAsync(null, ( IQueryable<IProcessInstance>) null, RETRIES);
        }

        [Test]
        public virtual void testSetJobsRetryAsyncWithProcessList()
        {
            //when
            var batch = managementService.SetJobRetriesAsync(processInstanceIds, ( IQueryable<IProcessInstance>) null, RETRIES);
            executeSeedJob(batch);
            var exceptions = executeBatchJobs(batch);

            // then
            Assert.That(exceptions.Count, Is.EqualTo(0));
            AssertRetries(ids, RETRIES);
            AssertHistoricBatchExists(testRule);
        }

        [Test]
        public virtual void testSetJobsRetryAsyncWithProcessQuery()
        {
            //given
            var query = runtimeService.CreateProcessInstanceQuery();

            //when
            var batch = managementService.SetJobRetriesAsync(null, query, RETRIES);
            executeSeedJob(batch);
            var exceptions = executeBatchJobs(batch);

            // then
            Assert.That(exceptions.Count, Is.EqualTo(0));
            AssertRetries(ids, RETRIES);
            AssertHistoricBatchExists(testRule);
        }

        [Test]
        public virtual void testSetJobsRetryAsyncWithProcessQueryAndList()
        {
            //given
            var extraPi = startTestProcesses(1);
            var query = runtimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId == extraPi[0]);

            //when
            var batch = managementService.SetJobRetriesAsync(processInstanceIds, query, RETRIES);
            executeSeedJob(batch);
            var exceptions = executeBatchJobs(batch);

            // then
            Assert.That(exceptions.Count, Is.EqualTo(0));
            AssertRetries(AllJobIds, RETRIES);
            AssertHistoricBatchExists(testRule);
        }
    }
}