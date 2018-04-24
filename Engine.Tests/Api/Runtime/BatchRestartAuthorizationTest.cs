using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class BatchRestartAuthorizationTest
    {
        [TearDown]
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
        }

        [TearDown]
        public virtual void cleanBatch()
        {
            var batch = engineRule.ManagementService.CreateBatchQuery()
                .First();
            if (batch != null)
                engineRule.ManagementService.DeleteBatch(batch.Id, true);

            var historicBatch = engineRule.HistoryService.CreateHistoricBatchQuery()
                .First();
            if (historicBatch != null)
                engineRule.HistoryService.DeleteHistoricBatch(historicBatch.Id);
        }

        [TearDown]
        public virtual void removeBatches()
        {
            helper.RemoveAllRunningAndHistoricBatches();
        }

        protected internal const string TEST_REASON = "test reason";
        protected internal AuthorizationTestRule authRule;

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal BatchModificationHelper helper;
        private readonly bool InstanceFieldsInitialized;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testRule);
        //public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public api.authorization.util.AuthorizationScenario scenario;
        public static AuthorizationScenario scenario;
        protected internal ProcessEngineTestRule testRule;

        public BatchRestartAuthorizationTest()
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
            helper = new BatchModificationHelper(engineRule);
            ////ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testRule);
        }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(scenario
                    .WithoutAuthorizations()
                    .FailsDueToRequired(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create)),
                scenario
                    .WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create))
                    .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process", "userId",
                        Permissions.ReadHistory)),
                scenario
                    .WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                        AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process", "userId",
                            Permissions.ReadHistory))
                    .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessInstance, "*", "userId",
                        Permissions.Create)), scenario
                    .WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                        AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process", "userId",
                            Permissions.ReadHistory,
                            Permissions.CreateInstance),
                        AuthorizationSpec.Grant(Resources.ProcessInstance, "*", "userId", Permissions.Create))
                    .Succeeds());
        }

        [Test]
        public virtual void executeBatch()
        {
            //given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);

            var processInstance1 = engineRule.RuntimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = engineRule.RuntimeService.StartProcessInstanceByKey("Process");
            engineRule.RuntimeService.DeleteProcessInstance(processInstance1.Id, TEST_REASON);
            engineRule.RuntimeService.DeleteProcessInstance(processInstance2.Id, TEST_REASON);

            authRule.Init(scenario)
                .WithUser("userId")
                .BindResource("processInstance1", processInstance1.Id)
                .BindResource("restartedProcessInstance", "*")
                .BindResource("processInstance2", processInstance2.Id)
                .BindResource("processDefinition", "Process")
                .BindResource("batchId", "*")
                .Start();

            var batch = engineRule.RuntimeService.RestartProcessInstances(processDefinition.Id)
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .StartAfterActivity("userTask1")
                .ExecuteAsync();

            if (batch != null)
            {
                var job = engineRule.ManagementService.CreateJobQuery(c=>c.JobDefinitionId ==batch.SeedJobDefinitionId)
                    .First();

                // seed job
                engineRule.ManagementService.ExecuteJob(job.Id);

                foreach (var pending in engineRule.ManagementService.CreateJobQuery(c=>c.JobDefinitionId ==batch.BatchJobDefinitionId)
                    
                    .ToList())
                    engineRule.ManagementService.ExecuteJob(pending.Id);
            }
            // then
            authRule.AssertScenario(scenario);
        }
    }
}