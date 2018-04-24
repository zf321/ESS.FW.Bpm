using System.Collections.Generic;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{


    /// <summary>
    /// @author Stefan Hentschel.
    /// </summary>
    public class TaskCountByCandidateGroupAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public TaskCountByCandidateGroupAuthorizationTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            processEngineTestRule = new ProcessEngineTestRule(processEngineRule);
            //ruleChain = RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
        }


        public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
        public ProcessEngineTestRule processEngineTestRule;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
        //public RuleChain ruleChain;


        protected internal ITaskService taskService;
        protected internal IIdentityService identityService;
        protected internal IAuthorizationService authorizationService;
        protected internal ProcessEngineConfiguration processEngineConfiguration;

        protected internal string userId = "user";

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        public virtual void setUp()
        {
            taskService = processEngineRule.TaskService;
            identityService = processEngineRule.IdentityService;
            authorizationService = processEngineRule.IAuthorizationService;
            processEngineConfiguration = processEngineRule.ProcessEngineConfiguration;
        }

        [Test]
        public virtual void shouldFetchTaskCountWithAuthorization()
        {
            // given
            IUser user = identityService.NewUser(userId);
            identityService.SaveUser(user);

            IAuthorization authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
            authorization.AddPermission(Permissions.Read);
            authorization.Resource = Resources.Task;
            authorization.ResourceId = AuthorizationFields.Any;
            authorization.UserId = userId;
            authorizationService.SaveAuthorization(authorization);

            processEngineConfiguration.SetAuthorizationEnabled(true);
            authenticate();

            // when
            IList<ITaskCountByCandidateGroupResult> results = taskService.CreateTaskReport().TaskCountByCandidateGroup();
            processEngineConfiguration.SetAuthorizationEnabled(false);
            authorizationService.DeleteAuthorization(authorization.Id);
            identityService.DeleteUser(userId);

            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public virtual void shouldFailToFetchTaskCountWithMissingAuthorization()
        {
            // given
            bool testFailed = false;
            processEngineConfiguration.SetAuthorizationEnabled(true);
            authenticate();

            // when
            try
            {
                taskService.CreateTaskReport().TaskCountByCandidateGroup();
                testFailed = true;

            }
            catch (AuthorizationException aex)
            {
                if (!aex.Message.Contains(userId + "' does not have 'Permissions.Read' permission on resource '*' of type 'Task'"))
                {
                    testFailed = true;
                }
            }

            // then
            processEngineConfiguration.SetAuthorizationEnabled(false);

            if (testFailed)
            {
                Assert.Fail("There should be an authorization exception for '" + userId + "' because of a missing 'Permissions.Read' permission on 'Task'.");
            }
        }

        protected internal virtual void authenticate()
        {
            identityService.SetAuthentication(userId, null, null);
        }
    }

}