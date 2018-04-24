using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.JobDefinition
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class SetJobDefinitionPriorityCascadeAuthorizationTest
    public class SetJobDefinitionPriorityCascadeAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public SetJobDefinitionPriorityCascadeAuthorizationTest()
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
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario().WithoutAuthorizations()
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinitionKey", "userId", 
                Permissions.Update)), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinitionKey", "userId",
                Permissions.Update)).FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessInstance, "*", "userId", 
                Permissions.Update), AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinitionKey", "userId", 
                Permissions.UpdateInstance)), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinitionKey", "userId", 
                Permissions.Update), AuthorizationSpec.Grant(Resources.ProcessInstance, "ProcessInstanceId", "userId", 
                Permissions.Update)).FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessInstance, "*", "userId", 
                Permissions.Update), AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinitionKey", "userId", 
                Permissions.UpdateInstance)), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinitionKey", "userId", 
                Permissions.Update), AuthorizationSpec.Grant(Resources.ProcessInstance, "*", "userId", Permissions.Update)).Succeeds(),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinitionKey", "userId", 
                Permissions.Update), AuthorizationSpec.Grant(Resources.ProcessDefinition, "*", "userId", Permissions.UpdateInstance))
                .Succeeds(), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "processDefinitionKey", "userId", 
                Permissions.Update, Permissions.UpdateInstance)).Succeeds());
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
        [Deployment(new string[] { "resources/api/authorization/oneIncidentProcess.bpmn20.xml" })]
        public virtual void testSetJobDefinitionPriority()
        {

            // given
            IJobDefinition jobDefinition = engineRule.ManagementService.CreateJobDefinitionQuery().First();
            IProcessInstance instance = engineRule.RuntimeService.StartProcessInstanceByKey("process");
            ESS.FW.Bpm.Engine.Runtime.IJob job = engineRule.ManagementService.CreateJobQuery().First();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("processDefinitionKey", "process").BindResource("ProcessInstanceId", instance.Id).Start();

            engineRule.ManagementService.SetOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42, true);

            // then
            if (authRule.AssertScenario(scenario))
            {
                jobDefinition = engineRule.ManagementService.CreateJobDefinitionQuery().First();
                Assert.AreEqual(42L, (long)jobDefinition.OverridingJobPriority);

                job = engineRule.ManagementService.CreateJobQuery().First();
                Assert.AreEqual(42L, job.Priority);
            }

        }

    }

}