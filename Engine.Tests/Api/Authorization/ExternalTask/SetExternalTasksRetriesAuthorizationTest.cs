using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.ExternalTask
{


    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class SetExternalTasksRetriesAuthorizationTest
    public class SetExternalTasksRetriesAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public SetExternalTasksRetriesAuthorizationTest()
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
            //chain = RuleChain.outerRule(engineRule).around(authRule);
        }

        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public AuthorizationTestRule authRule;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule);
        //public RuleChain chain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter public api.authorization.util.AuthorizationScenario scenario;
        public AuthorizationScenario scenario;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstanceId2", "userId", 
                Permissions.Update)).WithoutAuthorizations()
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstanceId1", "userId", 
                Permissions.Update), AuthorizationSpec.Grant(Resources.ProcessDefinition, "oneExternalTaskProcess", "userId", 
                Permissions.UpdateInstance)), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstanceId1", "userId", 
                Permissions.Update), AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstanceId2", "userId", 
                Permissions.Update)).Succeeds(), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessInstance, "*", "userId", Permissions.Update))
                .Succeeds(), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinitionKey", "userId", 
                Permissions.UpdateInstance)).Succeeds(), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "*", "userId", Permissions.UpdateInstance))
                .Succeeds());
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

        [Test]
        [Deployment(new string[] { "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml" })]
        public virtual void testSetRetrieSync()
        {

            // given
            IProcessInstance processInstance1 = engineRule.RuntimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
            IProcessInstance processInstance2 = engineRule.RuntimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> tasks = engineRule.ExternalTaskService.CreateExternalTaskQuery()
                .ToList();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("processInstanceId1", processInstance1.Id).BindResource("processInstanceId2", processInstance2.Id).BindResource("processDefinitionKey", "oneExternalTaskProcess").Start();

            List<string> externalTaskIds = new List<string>();
            externalTaskIds.Add(tasks[0].Id);
            externalTaskIds.Add(tasks[1].Id);
            foreach (var taskId in externalTaskIds)
            {
                engineRule.ExternalTaskService.SetRetries(taskId, 5);
            }
            //engineRule.ExternalTaskService.SetRetries(externalTaskIds, 5);

            // then
            if (authRule.AssertScenario(scenario))
            {
                tasks = engineRule.ExternalTaskService.CreateExternalTaskQuery().ToList();
                Assert.AreEqual(5, (int)tasks[0].Retries);
                Assert.AreEqual(5, (int)tasks[1].Retries);
            }

        }
    }
}