using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Batch
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractBatchAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public AbstractBatchAuthorizationTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            authRule = new AuthorizationTestRule(engineRule);
            testHelper = new ProcessEngineTestRule(engineRule);
        }

        protected internal const string TEST_REASON = "test reason";

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal AuthorizationTestRule authRule;
        protected internal ProcessEngineTestRule testHelper;

        protected internal IProcessDefinition sourceDefinition;
        protected internal IProcessDefinition sourceDefinition2;
        protected internal IProcessInstance processInstance;
        protected internal IProcessInstance processInstance2;
        protected internal IBatch batch;
        protected internal IRuntimeService runtimeService;
        protected internal IManagementService managementService;
        protected internal int invocationsPerBatchJob;

        [SetUp]
        public virtual void setUp()
        {
            authRule.CreateUserAndGroup("userId", "groupId");
            runtimeService = engineRule.RuntimeService;
            managementService = engineRule.ManagementService;
            invocationsPerBatchJob = engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob;
        }

        // Todo: ModifiableBpmnModelInstance.Modify(..)
        // Todo: ProcessModels
        [SetUp]
        public virtual void deployProcesses()
        {
            //sourceDefinition = testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "ONE_TASK_PROCESS"));
            //sourceDefinition2 = testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.modify(ProcessModels.TWO_TASKS_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "TWO_TASKS_PROCESS"));
            //processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
            //processInstance2 = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition2.Id);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
            engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = invocationsPerBatchJob;
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void cleanBatch()
        public virtual void cleanBatch()
        {
            IBatch batch = engineRule.ManagementService.CreateBatchQuery().First();
            if (batch != null)
            {
                engineRule.ManagementService.DeleteBatch(batch.Id, true);
            }

            IHistoricBatch historicBatch = engineRule.HistoryService.CreateHistoricBatchQuery().First();
            if (historicBatch != null)
            {
                engineRule.HistoryService.DeleteHistoricBatch(historicBatch.Id);
            }
        }

        protected internal virtual void executeSeedAndBatchJobs()
        {
            ESS.FW.Bpm.Engine.Runtime.IJob job = engineRule.ManagementService.CreateJobQuery(c=>c.JobDefinitionId==batch.SeedJobDefinitionId).First();
            //seed job
            managementService.ExecuteJob(job.Id);

            foreach (ESS.FW.Bpm.Engine.Runtime.IJob pending in managementService.CreateJobQuery(c=>c.JobDefinitionId == batch.BatchJobDefinitionId).ToList())
            {
                managementService.ExecuteJob(pending.Id);
            }
        }

        protected internal abstract AuthorizationScenario Scenario { get; }
    }

}