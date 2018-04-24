using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{



    /// <summary>
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE:  @RunWith(Parameterized.class) public class DeleteHistoricProcessInstancesAuthorizationTest

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
    public class DeleteHistoricProcessInstancesAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public DeleteHistoricProcessInstancesAuthorizationTest()
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
            //ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        }


        protected internal const string PROCESS_KEY = "oneTaskProcess";

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal AuthorizationTestRule authRule;
        protected internal ProcessEngineTestRule testHelper;

        protected internal IProcessInstance processInstance;
        protected internal IProcessInstance processInstance2;

        protected internal IHistoricProcessInstance historicProcessInstance;
        protected internal IHistoricProcessInstance historicProcessInstance2;

        protected internal IRuntimeService runtimeService;
        protected internal IHistoryService historyService;
        protected internal IManagementService managementService;

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
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process", "userId",
                Permissions.ReadHistory)).FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process", "userId",
                Permissions.DeleteHistory)), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process", "userId",
                Permissions.ReadHistory, Permissions.DeleteHistory)).Succeeds());
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        public virtual void setUp()
        {
            authRule.CreateUserAndGroup("userId", "groupId");
            runtimeService = engineRule.RuntimeService;
            managementService = engineRule.ManagementService;
            historyService = engineRule.HistoryService;

            deployAndCompleteProcesses();
        }

        public virtual void deployAndCompleteProcesses()
        {
            IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
            processInstance2 = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

            IList<string> processInstanceIds = (new string[] { processInstance.Id, processInstance2.Id });
            runtimeService.DeleteProcessInstances(processInstanceIds, null, false, false);

            historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

            historicProcessInstance2 = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance2.Id).First();
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
        }

        [Test]
        public virtual void testProcessInstancesList()
        {
            //given
            IList<string> processInstanceIds = new List<string> { historicProcessInstance.Id, historicProcessInstance2.Id };
            authRule.Init(scenario).WithUser("userId").BindResource("processInstance1", processInstance.Id).BindResource("processInstance2", processInstance2.Id).Start();

            // when
            historyService.DeleteHistoricProcessInstances(processInstanceIds);

            // then
            if (authRule.AssertScenario(scenario))
            {
                Assert.That(historyService.CreateHistoricProcessInstanceQuery().Count(), Is.EqualTo(0L));
            }
        }
    }

}