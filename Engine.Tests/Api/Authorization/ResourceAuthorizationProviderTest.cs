using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ResourceAuthorizationProviderTest : ResourceProcessEngineTestCase
    {

        protected internal string userId = "test";
        protected internal string groupId = "accounting";
        protected internal IUser user;
        protected internal IGroup group;

        public ResourceAuthorizationProviderTest() : base("resources/api/authorization/resource.authorization.provider.camunda.cfg.xml")
        {
        }

        protected internal virtual void initializeProcessEngine()
        {
            //base.initializeProcessEngine();

            processEngineConfiguration = (ProcessEngineConfigurationImpl)ProcessEngine.ProcessEngineConfiguration;
            processEngineConfiguration.ResourceAuthorizationProvider = new MyResourceAuthorizationProvider();

            identityService = processEngineConfiguration.IdentityService;
            authorizationService = processEngineConfiguration.AuthorizationService;

            user = createUser(userId);
            group = createGroup(groupId);

            identityService.CreateMembership(userId, groupId);

            identityService.SetAuthentication(userId, new List<string>() { groupId });
            processEngineConfiguration.SetAuthorizationEnabled(true);
        }

        public virtual void tearDown()
        {
            processEngineConfiguration.SetAuthorizationEnabled(false);
            foreach (IUser user in identityService.CreateUserQuery().ToList())
            {
                identityService.DeleteUser(user.Id);
            }
            foreach (IGroup group in identityService.CreateGroupQuery().ToList())
            {
                identityService.DeleteGroup(group.Id);
            }
            foreach (IAuthorization authorization in authorizationService.CreateAuthorizationQuery().ToList())
            {
                authorizationService.DeleteAuthorization(authorization.Id);
            }
        }

        public virtual void testNewTaskAssignee()
        {
            // given
            MyResourceAuthorizationProvider.ClearProperties();

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, Permissions.All, userId);

            string taskId = "myTask";
            ITask NewTask = taskService.NewTask(taskId);
            taskService.SaveTask(NewTask);

            // when (1)
            taskService.SetAssignee(taskId, "demo");

            // then (1)
            Assert.IsNull(MyResourceAuthorizationProvider.OldAssignee);
            Assert.AreEqual("demo", MyResourceAuthorizationProvider.NewAssignee);

            MyResourceAuthorizationProvider.ClearProperties();

            // when (2)
            taskService.SetAssignee(taskId, userId);

            // then (2)
            Assert.AreEqual("demo", MyResourceAuthorizationProvider.OldAssignee);
            Assert.AreEqual(userId, MyResourceAuthorizationProvider.NewAssignee);

            taskService.DeleteTask(taskId, true);
        }

        public virtual void testNewTaskOwner()
        {
            // given
            MyResourceAuthorizationProvider.ClearProperties();

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, Permissions.All, userId);

            string taskId = "myTask";
            ITask NewTask = taskService.NewTask(taskId);
            taskService.SaveTask(NewTask);

            // when (1)
            taskService.SetOwner(taskId, "demo");

            // then (1)
            Assert.IsNull(MyResourceAuthorizationProvider.OldOwner);
            Assert.AreEqual("demo", MyResourceAuthorizationProvider.NewOwner);

            MyResourceAuthorizationProvider.ClearProperties();

            // when (2)
            taskService.SetOwner(taskId, userId);

            // then (2)
            Assert.AreEqual("demo", MyResourceAuthorizationProvider.OldOwner);
            Assert.AreEqual(userId, MyResourceAuthorizationProvider.NewOwner);

            taskService.DeleteTask(taskId, true);
        }

        public virtual void testAddCandidateUser()
        {
            // given
            MyResourceAuthorizationProvider.ClearProperties();

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, Permissions.All, userId);

            string taskId = "myTask";
            ITask NewTask = taskService.NewTask(taskId);
            taskService.SaveTask(NewTask);

            // when
            taskService.AddCandidateUser(taskId, "demo");

            // then
            Assert.AreEqual(IdentityLinkType.Candidate, MyResourceAuthorizationProvider.AddUserIdentityLinkType);
            Assert.AreEqual("demo", MyResourceAuthorizationProvider.AddUserIdentityLinkUser);

            taskService.DeleteTask(taskId, true);
        }

        public virtual void testAddUserIdentityLink()
        {
            // given
            MyResourceAuthorizationProvider.ClearProperties();

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, Permissions.All, userId);

            string taskId = "myTask";
            ITask NewTask = taskService.NewTask(taskId);
            taskService.SaveTask(NewTask);

            // when
            taskService.AddUserIdentityLink(taskId, "demo", "myIdentityLink");

            // then
            Assert.AreEqual("myIdentityLink", MyResourceAuthorizationProvider.AddUserIdentityLinkType);
            Assert.AreEqual("demo", MyResourceAuthorizationProvider.AddUserIdentityLinkUser);

            taskService.DeleteTask(taskId, true);
        }

        public virtual void testAddCandidateGroup()
        {
            // given
            MyResourceAuthorizationProvider.ClearProperties();

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, Permissions.All, userId);

            string taskId = "myTask";
            ITask NewTask = taskService.NewTask(taskId);
            taskService.SaveTask(NewTask);

            // when
            taskService.AddCandidateGroup(taskId, "management");

            // then
            Assert.AreEqual(IdentityLinkType.Candidate, MyResourceAuthorizationProvider.AddGroupIdentityLinkType);
            Assert.AreEqual("management", MyResourceAuthorizationProvider.AddGroupIdentityLinkGroup);

            taskService.DeleteTask(taskId, true);
        }

        public virtual void testAddGroupIdentityLink()
        {
            // given
            MyResourceAuthorizationProvider.ClearProperties();

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, Permissions.All, userId);

            string taskId = "myTask";
            ITask NewTask = taskService.NewTask(taskId);
            taskService.SaveTask(NewTask);

            // when
            taskService.AddGroupIdentityLink(taskId, "management", "myIdentityLink");

            // then
            Assert.AreEqual("myIdentityLink", MyResourceAuthorizationProvider.AddGroupIdentityLinkType);
            Assert.AreEqual("management", MyResourceAuthorizationProvider.AddGroupIdentityLinkGroup);

            taskService.DeleteTask(taskId, true);
        }

        public virtual void testDeleteUserIdentityLink()
        {
            // given
            MyResourceAuthorizationProvider.ClearProperties();

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, Permissions.All, userId);

            string taskId = "myTask";
            ITask NewTask = taskService.NewTask(taskId);
            taskService.SaveTask(NewTask);
            taskService.AddCandidateUser(taskId, "demo");

            // when
            taskService.DeleteCandidateUser(taskId, "demo");

            // then
            Assert.AreEqual(IdentityLinkType.Candidate, MyResourceAuthorizationProvider.DeleteUserIdentityLinkType);
            Assert.AreEqual("demo", MyResourceAuthorizationProvider.DeleteUserIdentityLinkUser);

            taskService.DeleteTask(taskId, true);
        }

        public virtual void testDeleteGroupIdentityLink()
        {
            // given
            MyResourceAuthorizationProvider.ClearProperties();

            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, Permissions.All, userId);

            string taskId = "myTask";
            ITask NewTask = taskService.NewTask(taskId);
            taskService.SaveTask(NewTask);
            taskService.AddCandidateGroup(taskId, "management");

            // when
            taskService.DeleteCandidateGroup(taskId, "management");

            // then
            Assert.AreEqual(IdentityLinkType.Candidate, MyResourceAuthorizationProvider.DeleteGroupIdentityLinkType);
            Assert.AreEqual("management", MyResourceAuthorizationProvider.DeleteGroupIdentityLinkGroup);

            taskService.DeleteTask(taskId, true);
        }

        // user ////////////////////////////////////////////////////////////////

        protected internal virtual IUser createUser(string userId)
        {
            IUser user = identityService.NewUser(userId);
            identityService.SaveUser(user);

            // give user all permission to manipulate authorizations
            IAuthorization authorization = createGrantAuthorization(Resources.Authorization, AuthorizationFields.Any);
            authorization.UserId = userId;
            authorization.AddPermission(Permissions.All);
            saveAuthorization(authorization);

            // give user all permission to manipulate users
            authorization = createGrantAuthorization(Resources.User, AuthorizationFields.Any);
            authorization.UserId = userId;
            authorization.AddPermission(Permissions.All);
            saveAuthorization(authorization);

            return user;
        }

        // group //////////////////////////////////////////////////////////////

        protected internal virtual IGroup createGroup(string groupId)
        {
            IGroup group = identityService.NewGroup(groupId);
            identityService.SaveGroup(group);
            return group;
        }

        // authorization ///////////////////////////////////////////////////////

        protected internal virtual void createGrantAuthorization(Resources resource, string resourceId, Permissions permission, string userId)
        {
            IAuthorization authorization = createGrantAuthorization(resource, resourceId);
            authorization.UserId = userId;
            authorization.AddPermission(permission);
            saveAuthorization(authorization);
        }

        protected internal virtual IAuthorization createGrantAuthorization(Resources resource, string resourceId)
        {
            IAuthorization authorization = createAuthorization(AuthorizationFields.AuthTypeGrant, resource, resourceId);
            return authorization;
        }

        protected internal virtual IAuthorization createAuthorization(int type, Resources resource, string resourceId)
        {
            IAuthorization authorization = authorizationService.CreateNewAuthorization(type);

            authorization.Resource = resource;
            if (!string.ReferenceEquals(resourceId, null))
            {
                authorization.ResourceId = resourceId;
            }

            return authorization;
        }

        protected internal virtual void saveAuthorization(IAuthorization authorization)
        {
            authorizationService.SaveAuthorization(authorization);
        }

    }

}