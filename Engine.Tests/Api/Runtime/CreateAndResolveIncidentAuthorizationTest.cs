using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{

    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class CreateAndResolveIncidentAuthorizationTest
    [TestFixture]
    public class CreateAndResolveIncidentAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public CreateAndResolveIncidentAuthorizationTest()
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
            //ruleChain = RuleChain.OuterRule(engineRule).Around(authRule).Around(testRule);
        }


        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal AuthorizationTestRule authRule;
        protected internal ProcessEngineTestRule testRule;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testRule);
        //public RuleChain ruleChain;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
        public AuthorizationScenario scenario;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
        //public static ICollection<AuthorizationScenario[]> Scenarios()
        //{
        //    return AuthorizationTestRule.AsParameters(Scenario().WithoutAuthorizations().FailsDueToRequired(Grant(Resources.PROCESS_INSTANCE, "processInstance", "userId", Permissions.UPDATE), Grant(Resources.PROCESS_DEFINITION, "Process", "userId", Permissions.UPDATE_INSTANCE)), Scenario().WithAuthorizations(Grant(Resources.PROCESS_INSTANCE, "processInstance", "userId", Permissions.UPDATE)).Succeeds(), Scenario().WithAuthorizations(Grant(Resources.PROCESS_DEFINITION, "Process", "userId", Permissions.UPDATE_INSTANCE)).Succeeds());
        //}
        [TearDown]
        public virtual void TearDown()
        {
            authRule.DeleteUsersAndGroups();
        }
        [Test]
        public virtual void CreateIncident()
        {
            //given
            testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);

            IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("Process");
            ExecutionEntity execution = (ExecutionEntity)engineRule.RuntimeService.CreateExecutionQuery(c=>c.IsActive).First();

            authRule.Init(scenario).WithUser("userId").BindResource("processInstance", processInstance.Id).BindResource("processDefinition", "Process").Start();

            engineRule.RuntimeService.CreateIncident("foo", execution.Id, execution.ActivityId, "bar");

            // then
            authRule.AssertScenario(scenario);
        }

        [Test]
        public virtual void ResolveIncident()
        {
            testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);

            IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("Process");
            ExecutionEntity execution = (ExecutionEntity)engineRule.RuntimeService.CreateExecutionQuery(c=>c.IsActive).First();

            authRule.DisableAuthorization();
            IIncident incident = engineRule.RuntimeService.CreateIncident("foo", execution.Id, execution.ActivityId, "bar");

            authRule.Init(scenario).WithUser("userId").BindResource("processInstance", processInstance.Id).BindResource("processDefinition", "Process").Start();

            // when
            engineRule.RuntimeService.ResolveIncident(incident.Id);

            // then
            authRule.AssertScenario(scenario);
        }
    }
}
