using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{

    /// 
    /// <summary>
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class DeleteProcessDefinitionAuthorizationTest
    public class DeleteProcessDefinitionAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public DeleteProcessDefinitionAuthorizationTest()
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


        public const string PROCESS_DEFINITION_KEY = "one";

        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public AuthorizationTestRule authRule;
        public ProcessEngineTestRule testHelper;
        protected internal IRepositoryService repositoryService;
        protected internal IRuntimeService runtimeService;
        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain chain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter public api.authorization.util.AuthorizationScenario scenario;
        public AuthorizationScenario scenario;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario().WithoutAuthorizations()
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, PROCESS_DEFINITION_KEY, "userId", Permissions.Delete)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, PROCESS_DEFINITION_KEY, "userId", Permissions.Delete)).Succeeds(),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "*", "userId", Permissions.Delete)).Succeeds());
        }

        [SetUp]
        public virtual void setUp()
        {
            authRule.CreateUserAndGroup("userId", "groupId");
            repositoryService = engineRule.RepositoryService;
            runtimeService = engineRule.RuntimeService;
            processEngineConfiguration = engineRule.ProcessEngineConfiguration;
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
            repositoryService = null;
            runtimeService = null;
            processEngineConfiguration = null;
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDeleteProcessDefinition()
        public virtual void testDeleteProcessDefinition()
        {
            testHelper.Deploy("resources/repository/twoProcesses.bpmn20.xml");
            IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery()
                .ToList();

            authRule.Init(scenario).WithUser("userId").Start();

            //when a process definition is been deleted
            repositoryService.DeleteProcessDefinition(processDefinitions[0].Id);

            //then only one process definition should remain
            if (authRule.AssertScenario(scenario))
            {
                Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());
            }
        }


        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDeleteProcessDefinitionCascade()
        public virtual void testDeleteProcessDefinitionCascade()
        {
            // given process definition and a process instance
            IBpmnModelInstance bpmnModel = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().EndEvent().Done();
            testHelper.Deploy(bpmnModel);

            IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY).First();
            runtimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).ExecuteWithVariablesInReturn();

            authRule.Init(scenario).WithUser("userId").Start();

            //when the corresponding process definition is cascading deleted from the deployment
            repositoryService.DeleteProcessDefinition(processDefinition.Id, true);

            //then exist no process instance and no definition
            if (authRule.AssertScenario(scenario))
            {
                Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
                Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery().Count());
                if (processEngineConfiguration.HistoryLevel.Id >= HistoryLevelFields.HistoryLevelActivity.Id)
                {
                    Assert.AreEqual(0, engineRule.HistoryService.CreateHistoricActivityInstanceQuery().Count());
                }
            }
        }
    }

}