using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Batch
{
    
    /// <summary>
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class DeleteProcessInstancesBatchAuthorizationTest extends AbstractBatchAuthorizationTest
    public class DeleteProcessInstancesBatchAuthorizationTest : AbstractBatchAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public DeleteProcessInstancesBatchAuthorizationTest()
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


        protected internal const long BATCH_OPERATIONS = 3L;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain ruleChain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter public api.authorization.util.AuthorizationScenario scenario;
        public AuthorizationScenario scenario;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance1", "userId", Permissions.Read, Permissions.Delete),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance2", "userId", Permissions.Read)).FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance2", "userId", Permissions.Delete),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process_2", "userId", Permissions.DeleteInstance)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create), AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance1", "userId", Permissions.All),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance2", "userId", Permissions.All)).Succeeds(),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process_2", "userId", Permissions.ReadInstance, Permissions.DeleteInstance),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process_1", "userId", Permissions.ReadInstance, Permissions.DeleteInstance)).Succeeds());
        }

        [Test]
        public virtual void testWithTwoInvocationsProcessInstancesList()
        {
            engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
            setupAndExecuteProcessInstancesListTest();

            // then
            AssertScenario();
        }

        [Test]
        public virtual void testProcessInstancesList()
        {
            setupAndExecuteProcessInstancesListTest();
            // then
            AssertScenario();
        }

        [Test]
        public virtual void testWithQuery()
        {
            //given
             IQueryable<IProcessInstance> processInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>(new HashSet<string>() { processInstance.Id, processInstance2.Id}.Contains(c.ProcessDefinitionId)));

            authRule.Init(scenario).WithUser("userId").BindResource("processInstance1", processInstance.Id).BindResource("processInstance2", processInstance2.Id).BindResource("Process_2", sourceDefinition2.Key).Start();

            // when

            batch = runtimeService.DeleteProcessInstancesAsync(null, processInstanceQuery, TEST_REASON);
            executeSeedAndBatchJobs();

            // then
            if (authRule.AssertScenario(scenario))
            {
                if (testHelper.HistoryLevelFull)
                {
                    Assert.That(engineRule.HistoryService.CreateUserOperationLogQuery().Count(), Is.EqualTo(BATCH_OPERATIONS));
                }
            }
        }

        protected internal virtual void setupAndExecuteProcessInstancesListTest()
        {
            //given
            IList<string> processInstanceIds = new List<string> { processInstance.Id, processInstance2.Id};
            authRule.Init(scenario).WithUser("userId").BindResource("processInstance1", processInstance.Id).BindResource("processInstance2", processInstance2.Id).BindResource("Process_2", sourceDefinition2.Key).BindResource("Process_1", sourceDefinition.Key).Start();

            // when
            batch = runtimeService.DeleteProcessInstancesAsync(processInstanceIds, null, TEST_REASON);

            executeSeedAndBatchJobs();
        }

        protected internal virtual void AssertScenario()
        {
            if (authRule.AssertScenario(Scenario))
            {
                if (testHelper.HistoryLevelFull)
                {
                    Assert.That(engineRule.HistoryService.CreateUserOperationLogQuery().Count(), Is.EqualTo(BATCH_OPERATIONS));
                }

                if (authRule.ScenarioSucceeded())
                {
                    Assert.That(runtimeService.CreateProcessInstanceQuery().Count(), Is.EqualTo(0L));
                }
            }
        }

        protected internal override AuthorizationScenario Scenario
        {
            get { return scenario;}
        }

    }

}