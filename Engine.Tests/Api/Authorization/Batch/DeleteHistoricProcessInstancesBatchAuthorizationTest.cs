using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Batch
{
    /// <summary>
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT) public class DeleteHistoricProcessInstancesBatchAuthorizationTest extends AbstractBatchAuthorizationTest
    public class DeleteHistoricProcessInstancesBatchAuthorizationTest : AbstractBatchAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public DeleteHistoricProcessInstancesBatchAuthorizationTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            //ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        }


        protected internal const long BATCH_OPERATIONS = 4;
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain ruleChain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter public api.authorization.util.AuthorizationScenarioWithCount scenario;
        public AuthorizationScenarioWithCount scenario;

        protected internal IHistoryService historyService;

        [SetUp]
        public virtual void setupHistoricService()
        {
            historyService = engineRule.HistoryService;
        }

        public override void cleanBatch()
        {
            base.cleanBatch();
            IList<IHistoricProcessInstance> list = historyService.CreateHistoricProcessInstanceQuery()
                .ToList();

            if (list.Count > 0)
            {
                IList<string> instances = new List<string>();
                foreach (IHistoricProcessInstance hpi in list)
                {
                    instances.Add(hpi.Id);
                }
                historyService.DeleteHistoricProcessInstances(instances);
            }
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenarioWithCount.scenario().withCount(2L)
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process_1", "userId", Permissions.ReadHistory, Permissions.DeleteHistory),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process_2", "userId", Permissions.ReadHistory)).FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process_2", "userId", Permissions.DeleteHistory)), AuthorizationScenarioWithCount.scenario().withCount(0L).WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process_1", "userId", Permissions.ReadHistory, Permissions.DeleteHistory),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process_2", "userId", Permissions.ReadHistory, Permissions.DeleteHistory)).Succeeds());
        }

        [Test]
        public virtual void testWithTwoInvocationsProcessInstancesList()
        {
            engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
            setupAndExecuteHistoricProcessInstancesListTest();

            // then
            AssertScenario();

            Assert.That(historyService.CreateHistoricProcessInstanceQuery().Count(), Is.EqualTo(AuthorizationScenarioWithCount.scenario().Count));
        }

        [Test]
        public virtual void testProcessInstancesList()
        {
            setupAndExecuteHistoricProcessInstancesListTest();
            // then
            AssertScenario();
        }

        protected internal virtual void setupAndExecuteHistoricProcessInstancesListTest()
        {
            //given
            IList<string> processInstanceIds = new List<string> { processInstance.Id, processInstance2.Id };
            runtimeService.DeleteProcessInstances(processInstanceIds, null, true, false);

            IList<string> historicProcessInstances = new List<string>();
            foreach (IHistoricProcessInstance hpi in historyService.CreateHistoricProcessInstanceQuery().ToList())
            {
                historicProcessInstances.Add(hpi.Id);
            }

            authRule.Init(scenario).WithUser("userId").BindResource("Process_1", sourceDefinition.Key).BindResource("Process_2", sourceDefinition2.Key).Start();

            // when
            batch = historyService.DeleteHistoricProcessInstancesAsync(historicProcessInstances, TEST_REASON);

            executeSeedAndBatchJobs();
        }



        protected internal override AuthorizationScenario Scenario
        {
            get
            {
                return scenario;
            }
        }

        protected internal virtual void AssertScenario()
        {
            if (authRule.AssertScenario(Scenario))
            {
                if (testHelper.HistoryLevelFull)
                {
                    Assert.That(engineRule.HistoryService.CreateUserOperationLogQuery().Count(), Is.EqualTo(BATCH_OPERATIONS));
                }
                Assert.That(historyService.CreateHistoricProcessInstanceQuery().Count(), Is.EqualTo(0L));
            }
        }
    }

}