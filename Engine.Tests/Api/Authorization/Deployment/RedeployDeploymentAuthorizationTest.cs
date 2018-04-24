using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Deployment
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class RedeployDeploymentAuthorizationTest
    public class RedeployDeploymentAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public RedeployDeploymentAuthorizationTest()
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
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario().WithoutAuthorizations().FailsDueToRequired(AuthorizationSpec.Grant(Resources.Deployment, "*", "userId", Permissions.Create)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Deployment, "*", "userId", Permissions.Create)).FailsDueToRequired(AuthorizationSpec.Grant(Resources.Deployment, "deploymentId", "userId", Permissions.Read)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Deployment, "deploymentId", "userId", Permissions.Read), AuthorizationSpec.Grant(Resources.Deployment, "*", "userId", Permissions.Create)).Succeeds());
        }

        [SetUp]
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
        public virtual void testRedeploy()
        {
            // given
            IRepositoryService repositoryService = engineRule.RepositoryService;

            IBpmnModelInstance model1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").Done();
            IBpmnModelInstance model2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").Done();

            // first deployment
            IDeployment deployment1 = repositoryService.CreateDeployment().AddModelInstance("process1.bpmn", model1).AddModelInstance("process2.bpmn", model2).Deploy();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("deploymentId", deployment1.Id).Start();

            IDeployment deployment2 = repositoryService.CreateDeployment().AddDeploymentResources(deployment1.Id).Deploy();

            // then
            if (authRule.AssertScenario(scenario))
            {
                Assert.AreEqual(2, repositoryService.CreateDeploymentQuery().Count());
                deleteDeployments(deployment2);
                deleteAuthorizations();
            }

            deleteDeployments(deployment1);
        }

        protected internal virtual void deleteDeployments(params IDeployment[] deployments)
        {
            foreach (IDeployment deployment in deployments)
            {
                engineRule.RepositoryService.DeleteDeployment(deployment.Id, true);
            }
        }

        protected internal virtual void deleteAuthorizations()
        {
            IAuthorizationService authorizationService = engineRule.IAuthorizationService;
            foreach (IAuthorization authorization in authorizationService.CreateAuthorizationQuery().ToList())
            {
                authorizationService.DeleteAuthorization(authorization.Id);
            }
        }

    }

}