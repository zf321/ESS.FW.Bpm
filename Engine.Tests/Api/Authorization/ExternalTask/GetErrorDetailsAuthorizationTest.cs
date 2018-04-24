using System.Collections.Generic;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.ExternalTask
{

    /// <summary>
    /// Please note that if you want to reuse Rule and other fields you should create abstract class
    /// and pack it there.
    /// </summary>
    /// <seealso cref= HandleExternalTaskAuthorizationTest
    /// 
    ///  </seealso>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class GetErrorDetailsAuthorizationTest
    public class GetErrorDetailsAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public GetErrorDetailsAuthorizationTest()
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

        private const string ERROR_DETAILS = "theDetails";
        protected internal string deploymentId;
        private string currentDetails;

        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public AuthorizationTestRule authRule;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule);
        //public RuleChain chain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter public api.authorization.util.AuthorizationScenario scenario;
        public AuthorizationScenario scenario;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario().WithoutAuthorizations()
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessInstance, "ProcessInstanceId", "userId", 
                Permissions.Read), AuthorizationSpec.Grant(Resources.ProcessDefinition, "oneExternalTaskProcess", "userId", 
                Permissions.ReadInstance)), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessInstance, "ProcessInstanceId", "userId",
                Permissions.Read)).Succeeds(), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessInstance, "*", "userId", Permissions.Read)).Succeeds(),
                AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinitionKey", "userId", 
                Permissions.ReadInstance)).Succeeds(), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "*", "userId", Permissions.ReadInstance))
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
        public virtual void testCompleteExternalTask()
        {

            // given
            IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
            IList<ILockedExternalTask> tasks = engineRule.ExternalTaskService.FetchAndLock(5, "workerId") as IList<ILockedExternalTask>
                ;//.Topic("externalTaskTopic", 5000L).Execute();

            ILockedExternalTask task = tasks[0];

            //preconditions method
            engineRule.ExternalTaskService.HandleFailure(task.Id, task.WorkerId, "anError", ERROR_DETAILS, 1, 1000L);

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("ProcessInstanceId", processInstance.Id).BindResource("processDefinitionKey", "oneExternalTaskProcess").Start();

            //execution method
            currentDetails = engineRule.ExternalTaskService.GetExternalTaskErrorDetails(task.Id);

            // then
            if (authRule.AssertScenario(scenario))
            {
                //Assertion method
                Assert.That(currentDetails, Is.EqualTo(ERROR_DETAILS));
            }
        }

    }

}