using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.ExternalTask
{

    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class SetExternalTasksRetriesBatchAuthorizationTest
    public class SetExternalTasksRetriesBatchAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public SetExternalTasksRetriesBatchAuthorizationTest()
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
            testRule = new ProcessEngineTestRule(engineRule);
            //chain = RuleChain.outerRule(engineRule).around(authRule).around(testRule);
        }


        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public AuthorizationTestRule authRule;
        protected internal ProcessEngineTestRule testRule;


        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testRule);
        //public RuleChain chain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter public api.authorization.util.AuthorizationScenario scenario;
        public AuthorizationScenario scenario;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario().WithoutAuthorizations().FailsDueToRequired(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Create)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinition", "userId", Permissions.UpdateInstance)).Succeeds(), AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Create), AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance1", "userId", Permissions.Update)).Succeeds(), AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Create)).FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinition", "userId", Permissions.UpdateInstance), AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance1", "userId", Permissions.Update)));
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        public virtual void setUp()
        {
            authRule.CreateUserAndGroup("userId", "groupId");
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
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

        [Test]
        public virtual void testSetRetriesAsync()
        {

            // given
            IProcessDefinition processDefinition = testRule.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            IProcessInstance processInstance1 = engineRule.RuntimeService.StartProcessInstanceByKey("Process");
            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks = engineRule.ExternalTaskService.CreateExternalTaskQuery()
                .ToList();

            List<string> externalTaskIds = new List<string>();

            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
            {
                externalTaskIds.Add(task.Id);
            }

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("batchId", "*").BindResource("processInstance1", processInstance1.Id).BindResource("processDefinition", processDefinition.Key).Start();

            IBatch batch = engineRule.ExternalTaskService.SetRetriesAsync(externalTaskIds, null, 5);
            if (batch != null)
            {
                executeSeedAndBatchJobs(batch);
            }

            // then
            if (authRule.AssertScenario(scenario))
            {
                externalTasks = engineRule.ExternalTaskService.CreateExternalTaskQuery()
                    .ToList();
                foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
                {
                    Assert.AreEqual(5, (int)task.Retries);
                }
            }
        }

        [Test]
        public virtual void testSetRetriesWithQueryAsync()
        {

            // given
            IProcessDefinition processDefinition = testRule.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            IProcessInstance processInstance1 = engineRule.RuntimeService.StartProcessInstanceByKey("Process");
            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTasks;

            IQueryable<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> externalTaskQuery = engineRule.ExternalTaskService.CreateExternalTaskQuery();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("batchId", "*").BindResource("processInstance1", processInstance1.Id).BindResource("processDefinition", processDefinition.Key).Start();

            IBatch batch = engineRule.ExternalTaskService.SetRetriesAsync(null, externalTaskQuery, 5);
            if (batch != null)
            {
                executeSeedAndBatchJobs(batch);
            }

            // then
            if (authRule.AssertScenario(scenario))
            {
                externalTasks = engineRule.ExternalTaskService.CreateExternalTaskQuery()
                    .ToList();
                foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in externalTasks)
                {
                    Assert.AreEqual(5, (int)task.Retries);
                }
            }
        }

        public virtual void executeSeedAndBatchJobs(IBatch batch)
        {
            ESS.FW.Bpm.Engine.Runtime.IJob job = engineRule.ManagementService.CreateJobQuery(c=>c.JobDefinitionId==batch.SeedJobDefinitionId).First();
            // seed job
            engineRule.ManagementService.ExecuteJob(job.Id);

            foreach (ESS.FW.Bpm.Engine.Runtime.IJob pending in engineRule.ManagementService.CreateJobQuery(c=>c.JobDefinitionId==batch.BatchJobDefinitionId).ToList())
            {
                engineRule.ManagementService.ExecuteJob(pending.Id);
            }
        }
    }

}