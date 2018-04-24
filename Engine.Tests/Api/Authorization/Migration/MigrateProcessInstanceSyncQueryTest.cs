using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Migration
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class MigrateProcessInstanceSyncQueryTest
    {
        private bool InstanceFieldsInitialized = false;

        public MigrateProcessInstanceSyncQueryTest()
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


        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public AuthorizationTestRule authRule;
        public ProcessEngineTestRule testHelper;

        protected internal IList<IAuthorization> authorizations;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain chain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        public virtual void setUp()
        {
            authorizations = new List<IAuthorization>();
            authRule.CreateUserAndGroup("userId", "groupId");
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            foreach (IAuthorization authorization in authorizations)
            {
                engineRule.IAuthorizationService.DeleteAuthorization(authorization.Id);
            }

            authRule.DeleteUsersAndGroups();
        }

        [Test]
        public virtual void testMigrateWithQuery()
        {
            // given
            IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess).ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey));

            IProcessInstance instance1 = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
            IProcessInstance instance2 = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

            grantAuthorization("user", Resources.ProcessInstance, instance2.Id, Permissions.Read);
            grantAuthorization("user", Resources.ProcessDefinition, "*", Permissions.MigrateInstance);

            IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

             IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();

            // when
            authRule.EnableAuthorization("user");
            engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceQuery(query).Execute();

            authRule.DisableAuthorization();


            // then
            IProcessInstance instance1AfterMigration = engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == instance1.Id).First();

            Assert.AreEqual(sourceDefinition.Id, instance1AfterMigration.ProcessDefinitionId);
        }

        protected internal virtual void grantAuthorization(string userId, Resources resource, string resourceId, Permissions permission)
        {
            IAuthorization authorization = engineRule.IAuthorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
            authorization.Resource = resource;
            authorization.ResourceId = resourceId;
            authorization.AddPermission(permission);
            authorization.UserId = userId;
            engineRule.IAuthorizationService.SaveAuthorization(authorization);
            authorizations.Add(authorization);
        }
    }

}