using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class BatchModificationAuthorizationTest
    [TestFixture]
    public class BatchModificationAuthorizationTest
    {
        [SetUp]
        public virtual void deployProcess()
        {
            processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
        }

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

        protected internal AuthorizationTestRule authRule;


        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal BatchModificationHelper helper;
        private readonly bool InstanceFieldsInitialized;

        protected internal IProcessDefinition processDefinition;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testRule);
        //public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public api.authorization.util.AuthorizationScenario scenario;
        public static AuthorizationScenario scenario;
        protected internal ProcessEngineTestRule testRule;

        public BatchModificationAuthorizationTest()
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
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Create),
                    AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance1", "userId", Permissions.Read,
                        Permissions.Update),
                    AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance2", "userId", Permissions.Read,
                        Permissions.Update))
                .Succeeds(), scenario
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Create),
                    AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance1", "userId", Permissions.Read,
                        Permissions.Update),
                    AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance2", "userId", Permissions.Read))
                .FailsDueToRequired(
                    AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance2", "userId", Permissions.Update),
                    AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinition", "userId",
                        Permissions.UpdateInstance))
                .Succeeds());
        }

        [Test]
        public virtual void executeAsyncModification()
        {
            //given
            var processInstance1 = engineRule.RuntimeService.StartProcessInstanceByKey(ProcessModels.ProcessKey);
            var processInstance2 = engineRule.RuntimeService.StartProcessInstanceByKey(ProcessModels.ProcessKey);

            authRule.Init(scenario)
                .WithUser("userId")
                .BindResource("processInstance1", processInstance1.Id)
                .BindResource("processInstance2", processInstance2.Id)
                .BindResource("processDefinition", ProcessModels.ProcessKey)
                .BindResource("batchId", "*")
                .Start();

            var batch = engineRule.RuntimeService.CreateModification(processDefinition.Id)
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .StartAfterActivity("userTask2")
                .ExecuteAsync();

            var job = engineRule.ManagementService.CreateJobQuery(c=>c.JobDefinitionId ==batch.SeedJobDefinitionId)
                .First();

            //seed job
            engineRule.ManagementService.ExecuteJob(job.Id);

            foreach (var pending in engineRule.ManagementService.CreateJobQuery(c=>c.JobDefinitionId ==batch.BatchJobDefinitionId)
                
                .ToList())
                engineRule.ManagementService.ExecuteJob(pending.Id);

            // then
            authRule.AssertScenario(scenario);
        }

        [Test]
        public virtual void executeModification()
        {
            //given
            var processInstance1 = engineRule.RuntimeService.StartProcessInstanceByKey(ProcessModels.ProcessKey);
            var processInstance2 = engineRule.RuntimeService.StartProcessInstanceByKey(ProcessModels.ProcessKey);

            authRule.Init(scenario)
                .WithUser("userId")
                .BindResource("processInstance1", processInstance1.Id)
                .BindResource("processInstance2", processInstance2.Id)
                .BindResource("processDefinition", ProcessModels.ProcessKey)
                .BindResource("batchId", "*")
                .Start();

            // when
            engineRule.RuntimeService.CreateModification(processDefinition.Id)
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .StartAfterActivity("userTask2")
                .Execute();

            // then
            authRule.AssertScenario(scenario);
        }
    }
}