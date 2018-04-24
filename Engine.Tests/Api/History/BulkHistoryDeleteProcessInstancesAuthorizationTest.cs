using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;

namespace Engine.Tests.Api.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class BulkHistoryDeleteProcessInstancesAuthorizationTest
    {
        protected internal const string ONE_TASK_PROCESS = "oneTaskProcess";
        public AuthorizationTestRule authRule;

        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain chain;

        private IHistoryService historyService;
        private readonly bool InstanceFieldsInitialized;
        private IRuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public api.authorization.util.AuthorizationScenario scenario;
        public static AuthorizationScenario scenario;
        public ProcessEngineTestRule testHelper;

        public BulkHistoryDeleteProcessInstancesAuthorizationTest()
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
            //chain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        }

        [SetUp]
        public virtual void init()
        {
            runtimeService = engineRule.RuntimeService;
            historyService = engineRule.HistoryService;

            authRule.CreateUserAndGroup("demo", "groupId");
        }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(scenario
                    .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "*", "demo",
                        Permissions.DeleteHistory)),
                scenario
                    .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "someId", "demo",
                        Permissions.DeleteHistory))
                    .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "*", "demo",
                        Permissions.DeleteHistory)),
                scenario
                    .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "*", "demo",
                        Permissions.DeleteHistory))
                    .Succeeds());
        }

        [TearDown]
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
        }

        [Test][Deployment(  "resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testCleanupHistory()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();
            runtimeService.DeleteProcessInstances(ids, null, true, true);

            // when
            authRule.Init(scenario)
                .WithUser("demo")
                .Start();

            //when
            historyService.DeleteHistoricProcessInstancesBulk(ids);

            if (authRule.AssertScenario(scenario))
                Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.CaseDefinitionKey ==ONE_TASK_PROCESS)
                    .Count());
        }

        private IList<string> prepareHistoricProcesses()
        {
            return prepareHistoricProcesses(ONE_TASK_PROCESS);
        }

        private IList<string> prepareHistoricProcesses(string businessKey)
        {
            return prepareHistoricProcesses(businessKey, null);
        }

        private IList<string> prepareHistoricProcesses(string businessKey, IVariableMap variables)
        {
            IList<string> processInstanceIds = new List<string>();

            for (var i = 0; i < 5; i++)
            {
                var processInstance = runtimeService.StartProcessInstanceByKey(businessKey, variables);
                processInstanceIds.Add(processInstance.Id);
            }

            return processInstanceIds;
        }
    }
}