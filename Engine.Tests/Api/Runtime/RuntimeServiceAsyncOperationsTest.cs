using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Api.Runtime.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class RuntimeServiceAsyncOperationsTest : AbstractAsyncOperationsTest
    {
        [SetUp]
        public override void initServices()
        {
            runtimeService = engineRule.RuntimeService;
            managementService = engineRule.ManagementService;
            historyService = engineRule.HistoryService;
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

        [SetUp]
        public virtual void storeEngineSettings()
        {
            var configuration = engineRule.ProcessEngineConfiguration;
            defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
            defaultInvocationsPerBatchJob = configuration.InvocationsPerBatchJob;
        }

        [TearDown]
        public virtual void restoreEngineSettings()
        {
            var configuration = engineRule.ProcessEngineConfiguration;
            configuration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
            configuration.InvocationsPerBatchJob = defaultInvocationsPerBatchJob;
        }

        private int defaultBatchJobsPerSeed;
        private int defaultInvocationsPerBatchJob;


        public new ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        private readonly bool InstanceFieldsInitialized;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public ExpectedException thrown = ExpectedException.None();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain migrationChain = org.junit.Rules.RuleChain.outerRule(testRule).around(migrationRule);
        //public RuleChain migrationChain;

        protected internal MigrationTestRule migrationRule;
        public new ProcessEngineTestRule testRule;

        public RuntimeServiceAsyncOperationsTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(engineRule);
            migrationRule = new MigrationTestRule(engineRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
            //migrationChain = RuleChain.outerRule(testRule)
            //.around(migrationRule);
        }

        [Test][Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml"})]
        public virtual void testDeleteProcessInstancesAsyncWithList()
        {
            // given
            var processIds = startTestProcesses(2);

            // when
            var batch = runtimeService.DeleteProcessInstancesAsync(processIds, null, TESTING_INSTANCE_DELETE);

            executeSeedJob(batch);
            executeBatchJobs(batch);

            // then
            AssertHistoricTaskDeletionPresent(processIds, TESTING_INSTANCE_DELETE, testRule);
            AssertHistoricBatchExists(testRule);
            AssertProcessInstancesAreDeleted();
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        public virtual void testDeleteProcessInstancesAsyncWithListOnly()
        {
            // given
            var processIds = startTestProcesses(2);

            // when
            var batch = runtimeService.DeleteProcessInstancesAsync(processIds, TESTING_INSTANCE_DELETE);

            executeSeedJob(batch);
            executeBatchJobs(batch);

            // then
            AssertHistoricTaskDeletionPresent(processIds, TESTING_INSTANCE_DELETE, testRule);
            AssertHistoricBatchExists(testRule);
            AssertProcessInstancesAreDeleted();
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        public virtual void testDeleteProcessInstancesAsyncWithNonExistingId()
        {
            // given
            var processIds = startTestProcesses(2);
            processIds.Add("unknown");

            // when
            var batch = runtimeService.DeleteProcessInstancesAsync(processIds, null, TESTING_INSTANCE_DELETE);

            executeSeedJob(batch);
            var exceptions = executeBatchJobs(batch);

            // then
            Assert.AreEqual(1, exceptions.Count);

            var e = exceptions[0];
            Assert.True(e.Message.StartsWith("No process instance found for id 'unknown'"));

            //Assert.That(managementService.CreateJobQuery()
            //    .WithException()
            //    
            //    .Count(), Is.EqualTo(1));

            processIds.Remove("unknown");
            AssertHistoricTaskDeletionPresent(processIds, TESTING_INSTANCE_DELETE, testRule);
            AssertHistoricBatchExists(testRule);
            AssertProcessInstancesAreDeleted();
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testDeleteProcessInstancesAsyncWithNullList()
        {
            //thrown.Expect(typeof(ProcessEngineException));
            //thrown.ExpectMessage("processInstanceIds is empty");

            runtimeService.DeleteProcessInstancesAsync(null, null, TESTING_INSTANCE_DELETE);
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testDeleteProcessInstancesAsyncWithEmptyList()
        {
            //thrown.Expect(typeof(ProcessEngineException));
            //thrown.ExpectMessage("processInstanceIds is empty");

            runtimeService.DeleteProcessInstancesAsync(new List<string>(), null, TESTING_INSTANCE_DELETE);
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        public virtual void testDeleteProcessInstancesAsyncWithQuery()
        {
            // given
            var processIds = startTestProcesses(2);
            var processInstanceQuery = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessInstanceIds(new HashSet<string>(processIds))
                ;

            // when
            var batch = runtimeService.DeleteProcessInstancesAsync(null, processInstanceQuery, TESTING_INSTANCE_DELETE);

            executeSeedJob(batch);
            executeBatchJobs(batch);

            // then
            AssertHistoricTaskDeletionPresent(processIds, TESTING_INSTANCE_DELETE, testRule);
            AssertHistoricBatchExists(testRule);
            AssertProcessInstancesAreDeleted();
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        public virtual void testDeleteProcessInstancesAsyncWithQueryOnly()
        {
            // given
            var processIds = startTestProcesses(2);
            var processInstanceQuery = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessInstanceIds(new HashSet<string>(processIds))
                ;

            // when
            //var batch = runtimeService.DeleteProcessInstancesAsync(processInstanceQuery, TESTING_INSTANCE_DELETE);

            //executeSeedJob(batch);
            //executeBatchJobs(batch);

            // then
            AssertHistoricTaskDeletionPresent(processIds, TESTING_INSTANCE_DELETE, testRule);
            AssertHistoricBatchExists(testRule);
            AssertProcessInstancesAreDeleted();
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        public virtual void testDeleteProcessInstancesAsyncWithQueryWithoutDeleteReason()
        {
            // given
            var processIds = startTestProcesses(2);
            var processInstanceQuery = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessInstanceIds(new HashSet<string>(processIds))
                ;

            // when
            var batch = runtimeService.DeleteProcessInstancesAsync(null, processInstanceQuery, null);

            executeSeedJob(batch);
            executeBatchJobs(batch);

            // then
            AssertHistoricTaskDeletionPresent(processIds, "deleted", testRule);
            AssertHistoricBatchExists(testRule);
            AssertProcessInstancesAreDeleted();
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testDeleteProcessInstancesAsyncWithNullQueryParameter()
        {
            //thrown.Expect(typeof(ProcessEngineException));
            //thrown.ExpectMessage("processInstanceIds is empty");

            runtimeService.DeleteProcessInstancesAsync(null, null, TESTING_INSTANCE_DELETE);
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testDeleteProcessInstancesAsyncWithInvalidQueryParameter()
        {
            // given
            startTestProcesses(2);
            var query = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessInstanceBusinessKey("invalid")
                ;

            //thrown.Expect(typeof(ProcessEngineException));
            //thrown.ExpectMessage("processInstanceIds is empty");

            // when
            runtimeService.DeleteProcessInstancesAsync(null, query, TESTING_INSTANCE_DELETE);
        }

        protected internal virtual void AssertProcessInstancesAreDeleted()
        {
            Assert.That(runtimeService.CreateProcessInstanceQuery()
                
                .Count(), Is.EqualTo(0));
        }

        [Test]
        public virtual void testDeleteProcessInstancesAsyncWithSkipCustomListeners()
        {
            // given
            IncrementCounterListener.Counter = 0;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var instance = ProcessModels.NewModel(OneTaskProcess)
                .StartEvent()
                .UserTask()
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(IncrementCounterListener).FullName)
                .EndEvent()
                .Done();

            testRule.Deploy(instance);
            var processIds = startTestProcesses(1);

            // when
            //var batch = runtimeService.DeleteProcessInstancesAsync(processIds, null, TESTING_INSTANCE_DELETE, true);
            //executeSeedJob(batch);
            //executeBatchJobs(batch);

            //// then
            //Assert.That(IncrementCounterListener.Counter, Is.EqualTo(0));
        }

        [Test]
        public virtual void testInvokeListenersWhenDeletingProcessInstancesAsync()
        {
            // given
            IncrementCounterListener.Counter = 0;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var instance = ProcessModels.NewModel(OneTaskProcess)
                .StartEvent()
                .UserTask()
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(IncrementCounterListener).FullName)
                .EndEvent()
                .Done();

            migrationRule.Deploy(instance);
            var processIds = startTestProcesses(1);

            // when
            var batch = runtimeService.DeleteProcessInstancesAsync(processIds, TESTING_INSTANCE_DELETE);
            executeSeedJob(batch);
            executeBatchJobs(batch);

            // then
            Assert.That(IncrementCounterListener.Counter, Is.EqualTo(1));
        }

        [Test]
        public virtual void testDeleteProcessInstancesAsyncWithListInDifferentDeployments()
        {
            // given
            var sourceDefinition1 =
                testRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .ChangeElementId(ProcessModels.ProcessKey, "OneTaskProcess"));
            var sourceDefinition2 =
                testRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.TwoTasksProcess)
                    .ChangeElementId(ProcessModels.ProcessKey, "TWO_TASKS_PROCESS"));
            var processInstanceIds = createProcessInstances(sourceDefinition1, sourceDefinition2, 15, 10);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String firstDeploymentId = sourceDefinition1.GetDeploymentId();
            var firstDeploymentId = sourceDefinition1.DeploymentId;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String secondDeploymentId = sourceDefinition2.GetDeploymentId();
            var secondDeploymentId = sourceDefinition2.DeploymentId;

            var processInstanceIdsFromFirstDeployment = getProcessInstanceIdsByDeploymentId(firstDeploymentId);
            var processInstanceIdsFromSecondDeployment = getProcessInstanceIdsByDeploymentId(secondDeploymentId);

            engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
            engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = 3;

            // when
            var batch = runtimeService.DeleteProcessInstancesAsync(processInstanceIds, null, "test_reason");

            var seedJobDefinitionId = batch.SeedJobDefinitionId;
            // seed jobs
            var expectedSeedJobsCount = 5;
            createAndExecuteSeedJobs(seedJobDefinitionId, expectedSeedJobsCount);

            // then
            var jobs = managementService.CreateJobQuery(c=>c.JobDefinitionId ==batch.BatchJobDefinitionId)
                
                .ToList();

            // execute jobs related to the first deployment
            var jobIdsForFirstDeployment = getJobIdsByDeployment(jobs, firstDeploymentId);
            Assert.NotNull(jobIdsForFirstDeployment);
            foreach (var jobId in jobIdsForFirstDeployment)
                managementService.ExecuteJob(jobId);

            // the process instances related to the first deployment should be deleted
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                //.SetDeploymentId(firstDeploymentId)
                .Count());
            AssertHistoricTaskDeletionPresent(processInstanceIdsFromFirstDeployment, "test_reason", testRule);
            // and process instances related to the second deployment should not be deleted
            Assert.AreEqual(processInstanceIdsFromSecondDeployment.Count, runtimeService.CreateProcessInstanceQuery()
                //.SetDeploymentId(secondDeploymentId)
                .Count());
            AssertHistoricTaskDeletionPresent(processInstanceIdsFromSecondDeployment, null, testRule);

            // execute jobs related to the second deployment
            var jobIdsForSecondDeployment = getJobIdsByDeployment(jobs, secondDeploymentId);
            Assert.NotNull(jobIdsForSecondDeployment);
            foreach (var jobId in jobIdsForSecondDeployment)
                managementService.ExecuteJob(jobId);

            // all of the process instances should be deleted
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        private IList<string> createProcessInstances(IProcessDefinition sourceDefinition1,
            IProcessDefinition sourceDefinition2, int instanceCountDef1, int instanceCountDef2)
        {
            IList<string> processInstanceIds = new List<string>();
            for (var i = 0; i < instanceCountDef1; i++)
            {
                var processInstance1 = runtimeService.StartProcessInstanceById(sourceDefinition1.Id);
                processInstanceIds.Add(processInstance1.Id);
                if (i < instanceCountDef2)
                {
                    var processInstance2 = runtimeService.StartProcessInstanceById(sourceDefinition2.Id);
                    processInstanceIds.Add(processInstance2.Id);
                }
            }
            return processInstanceIds;
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private List<String> getProcessInstanceIdsByDeploymentId(final String deploymentId)
        private IList<string> getProcessInstanceIdsByDeploymentId(string deploymentId)
        {
            var processInstances = runtimeService.CreateProcessInstanceQuery()
                //.SetDeploymentId(deploymentId)
                
                .ToList();
            IList<string> processInstanceIds = new List<string>();
            foreach (var processInstance in processInstances)
                processInstanceIds.Add(processInstance.Id);
            return processInstanceIds;
        }

        private IList<string> getJobIdsByDeployment(IList<IJob> jobs, string deploymentId)
        {
            IList<string> jobIdsForDeployment = new List<string>();
            for (var i = 0; i < jobs.Count; i++)
                if (jobs[i].DeploymentId.Equals(deploymentId))
                    jobIdsForDeployment.Add(jobs[i].Id);
            return jobIdsForDeployment;
        }

        private void createAndExecuteSeedJobs(string seedJobDefinitionId, int expectedSeedJobsCount)
        {
            for (var i = 0; i <= expectedSeedJobsCount; i++)
            {
                var seedJob = managementService.CreateJobQuery(c=>c.JobDefinitionId ==seedJobDefinitionId)
                    .First();
                if (i != expectedSeedJobsCount)
                {
                    Assert.NotNull(seedJob);
                    managementService.ExecuteJob(seedJob.Id);
                }
                else
                {
                    //the last seed job should not trigger another seed job
                    Assert.IsNull(seedJob);
                }
            }
        }
    }
}