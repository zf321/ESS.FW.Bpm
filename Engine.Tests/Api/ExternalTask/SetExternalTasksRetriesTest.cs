using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.ExternalTask
{

    public class SetExternalTasksRetriesTest
    {
        private bool InstanceFieldsInitialized = false;

        public SetExternalTasksRetriesTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new ProcessEngineTestRule(engineRule);
            ////ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
        }


        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal ProcessEngineTestRule testHelper;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
        //public RuleChain ruleChain;

        private static string PROCESS_DEFINITION_KEY = "oneExternalTaskProcess";
        private static string PROCESS_DEFINITION_KEY_2 = "twoExternalTaskWithPriorityProcess";

        protected internal IRuntimeService runtimeService;
        protected internal IRepositoryService repositoryService;
        protected internal IManagementService managementService;
        protected internal IExternalTaskService externalTaskService;

        protected internal IList<string> processInstanceIds;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void initServices()
        public virtual void initServices()
        {
            runtimeService = engineRule.RuntimeService;
            externalTaskService = engineRule.ExternalTaskService;
            managementService = engineRule.ManagementService;
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void deployTestProcesses() throws Exception
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        public virtual void deployTestProcesses()
        {
            IDeployment deployment = engineRule.RepositoryService.CreateDeployment().AddClasspathResource("resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml").AddClasspathResource("resources/api/externaltask/externalTaskPriorityExpression.bpmn20.xml").Deploy();

            engineRule.ManageDeployment(deployment);

            IRuntimeService runtimeService = engineRule.RuntimeService;
            processInstanceIds = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                processInstanceIds.Add(runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, i + "").Id);
            }
            processInstanceIds.Add(runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY_2).Id);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void cleanBatch()
        public virtual void cleanBatch()
        {
            IBatch batch = managementService.CreateBatchQuery().First();
            if (batch != null)
            {
                managementService.DeleteBatch(batch.Id, true);
            }

            IHistoricBatch historicBatch = engineRule.HistoryService.CreateHistoricBatchQuery().First();
            if (historicBatch != null)
            {
                engineRule.HistoryService.DeleteHistoricBatch(historicBatch.Id);
            }
        }

        [Test]
        public virtual void shouldSetExternalTaskRetriesSync()
        {

            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks = externalTaskService.CreateExternalTaskQuery()
                .ToList();;
            List<string> externalTaskIds = new List<string>();
            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
            {
                externalTaskIds.Add(task.Id);
            }
            // when  
            externalTaskService.SetRetries(externalTaskIds, 10);

            // then     
            externalTasks = externalTaskService.CreateExternalTaskQuery()
                    .ToList();
                ;foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
            {
                Assert.AreEqual(10, (int)task.Retries);
            }
        }

        [Test]
        public virtual void shouldFailForNonExistingExternalTaskIdSync()
        {

            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks = externalTaskService.CreateExternalTaskQuery()
                    .ToList();
                ;List<string> externalTaskIds = new List<string>();
            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
            {
                externalTaskIds.Add(task.Id);
            }

            externalTaskIds.Add("nonExistingExternalTaskId");

            try
            {
                externalTaskService.SetRetries(externalTaskIds, 10);
                Assert.Fail("exception expected");
            }
            catch (NotFoundException e)
            {
                Assert.That(e.Message, Does.Contain("Cannot find external task with id nonExistingExternalTaskId"));
            }
        }

        [Test]
        public virtual void shouldFailForNullExternalTaskIdSync()
        {

            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks = externalTaskService.CreateExternalTaskQuery().ToList();
            List<string> externalTaskIds = new List<string>();
            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
            {
                externalTaskIds.Add(task.Id);
            }

            externalTaskIds.Add(null);

            try
            {
                externalTaskService.SetRetries(externalTaskIds, 10);
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("External task id cannot be null"));
            }
        }

        [Test]
        public virtual void shouldFailForNullExternalTaskIdsSync()
        {
            try
            {
                externalTaskService.SetRetries((IList<string>)null, 10);
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("externalTaskIds is empty"));
            }
        }

        [Test]
        public virtual void shouldFailForNonExistingExternalTaskIdAsync()
        {

            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks = externalTaskService.CreateExternalTaskQuery().ToList();
            List<string> externalTaskIds = new List<string>();
            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
            {
                externalTaskIds.Add(task.Id);
            }

            externalTaskIds.Add("nonExistingExternalTaskId");
            IBatch batch = externalTaskService.SetRetriesAsync(externalTaskIds, null, 10);

            try
            {
                executeSeedAndBatchJobs(batch);
                Assert.Fail("exception expected");
            }
            catch (NotFoundException e)
            {
                Assert.That(e.Message, Does.Contain("Cannot find external task with id nonExistingExternalTaskId"));
            }
        }

        [Test]
        public virtual void shouldFailForNullExternalTaskIdAsync()
        {

            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks = externalTaskService.CreateExternalTaskQuery().ToList();
            List<string> externalTaskIds = new List<string>();
            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
            {
                externalTaskIds.Add(task.Id);
            }

            externalTaskIds.Add(null);
            IBatch batch = null;

            try
            {
                batch = externalTaskService.SetRetriesAsync(externalTaskIds, null, 10);
                executeSeedAndBatchJobs(batch);
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("External task id cannot be null"));
            }
        }

        [Test]
        public virtual void shouldFailForNullExternalTaskIdsAsync()
        {
            try
            {
                externalTaskService.SetRetriesAsync((IList<string>)null, null, 10);
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("externalTaskIds is empty"));
            }
        }

        [Test]
        public virtual void shouldFailForNegativeRetriesSync()
        {

            IList<string> externalTaskIds = new List<string> { "externalTaskId"};

            try
            {
                externalTaskService.SetRetries(externalTaskIds, -10);
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("The number of retries cannot be negative"));
            }
        }

        [Test]
        public virtual void shouldFailForNegativeRetriesAsync()
        {

            IList<string> externalTaskIds = new List<string> { "externalTaskId"};

            try
            {
                IBatch batch = externalTaskService.SetRetriesAsync(externalTaskIds, null, -10);
                executeSeedAndBatchJobs(batch);
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("The number of retries cannot be negative"));
            }
        }

        [Test]
        public virtual void shouldSetExternalTaskRetriesWithQueryAsync()
        {

            IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTaskQuery = engineRule.ExternalTaskService.CreateExternalTaskQuery();

            // when  
            IBatch batch = externalTaskService.SetRetriesAsync(null, externalTaskQuery, 5);

            // then
            executeSeedAndBatchJobs(batch);

            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTaskQuery.ToList())
            {
                Assert.AreEqual(5, (int)task.Retries);
            }
        }

        [Test]
        public virtual void shouldSetExternalTaskRetriesWithListAsync()
        {

            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks = externalTaskService.CreateExternalTaskQuery().ToList();
            List<string> externalTaskIds = new List<string>();
            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
            {
                externalTaskIds.Add(task.Id);
            }
            // when  
            IBatch batch = externalTaskService.SetRetriesAsync(externalTaskIds, null, 5);

            // then
            executeSeedAndBatchJobs(batch);

            externalTasks = externalTaskService.CreateExternalTaskQuery()
                    .ToList();
                ;foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
            {
                Assert.AreEqual(5, (int)task.Retries);
            }
        }

        [Test]
        public virtual void shouldSetExternalTaskRetriesWithListAndQueryAsync()
        {

            IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTaskQuery = externalTaskService.CreateExternalTaskQuery();
            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks = externalTaskQuery.ToList();

                ;List<string> externalTaskIds = new List<string>();
            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
            {
                externalTaskIds.Add(task.Id);
            }
            // when  
            IBatch batch = externalTaskService.SetRetriesAsync(externalTaskIds, externalTaskQuery, 5);

            // then
            executeSeedAndBatchJobs(batch);

            externalTasks = externalTaskService.CreateExternalTaskQuery()
                    .ToList();
                ;foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
            {
                Assert.AreEqual(5, (int)task.Retries);
            }
        }

        [Test]
        public virtual void shouldSetExternalTaskRetriesWithDifferentListAndQueryAsync()
        {
            // given
            IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTaskQuery = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId == processInstanceIds[0]);
            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId ==processInstanceIds[processInstanceIds.Count - 1])
                    .ToList();
                ;List<string> externalTaskIds = new List<string>();
            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask t in externalTasks)
            {
                externalTaskIds.Add(t.Id);
            }

            // when
            IBatch batch = externalTaskService.SetRetriesAsync(externalTaskIds, externalTaskQuery, 8);
            executeSeedAndBatchJobs(batch);

            // then
            ESS.FW.Bpm.Engine.Externaltask.IExternalTask task = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId == processInstanceIds[0]).First();
            Assert.AreEqual(8, (int)task.Retries);
            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> tasks = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId ==processInstanceIds[processInstanceIds.Count - 1])
                    .ToList();
                ;foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask t in tasks)
            {
                Assert.AreEqual(8, (int)t.Retries);
            }
        }

        public virtual void executeSeedAndBatchJobs(IBatch batch)
        {
            IJob job = engineRule.ManagementService.CreateJobQuery(c=>c.JobDefinitionId==batch.SeedJobDefinitionId).First();
            // seed job
            managementService.ExecuteJob(job.Id);

            foreach (IJob pending in managementService.CreateJobQuery(c=>c.JobDefinitionId==batch.BatchJobDefinitionId).ToList())
            {
                managementService.ExecuteJob(pending.Id);
            }
        }
    }

}