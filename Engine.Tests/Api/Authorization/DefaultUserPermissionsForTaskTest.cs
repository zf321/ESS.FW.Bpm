using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Identity;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{
    [TestFixture]
    public class DefaultUserPermissionsForTaskTest : AuthorizationTest
    {

        protected internal string userId2 = "demo";
        protected internal IUser user2;

        protected internal string groupId2 = "accounting2";
        protected internal IGroup group2;

        protected internal string defaultTaskPermissionValue;

        [TearDown]
        public void tearDown()
        {
            // reset default permission
            processEngineConfiguration.DefaultUserPermissionForTask = Permissions.Update;
            base.TearDown();
        }

        public virtual void testShouldGrantTaskWorkOnAssign()
        {

            // given
            processEngineConfiguration.DefaultUserPermissionForTask = Permissions.TaskWork;

            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            ProcessEngine.TaskService.SetAssignee(taskId, userId2);

            // then
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.Read, Resources.Task, taskId));
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.TaskWork, Resources.Task, taskId));
            Assert.AreEqual(false, authorizationService.IsUserAuthorized(userId2, null, Permissions.Update, Resources.Task, taskId));

            deleteTask(taskId, true);
        }

        public virtual void testShouldGrantUpdateOnAssign()
        {

            // given
            processEngineConfiguration.DefaultUserPermissionForTask = Permissions.Update;

            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            ProcessEngine.TaskService.SetAssignee(taskId, userId2);

            // then
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.Read, Resources.Task, taskId));
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.Update, Resources.Task, taskId));

            deleteTask(taskId, true);
        }

        public virtual void testShouldGrantTaskWorkOnSetCandidateUser()
        {

            // given
            processEngineConfiguration.DefaultUserPermissionForTask = Permissions.TaskWork;

            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            ProcessEngine.TaskService.AddCandidateUser(taskId, userId2);

            // then
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.Read, Resources.Task, taskId));
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.TaskWork, Resources.Task, taskId));
            Assert.AreEqual(false, authorizationService.IsUserAuthorized(userId2, null, Permissions.Update, Resources.Task, taskId));

            deleteTask(taskId, true);
        }

        public virtual void testShouldGrantUpdateOnSetCandidateUser()
        {

            // given
            processEngineConfiguration.DefaultUserPermissionForTask = Permissions.Update;

            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            ProcessEngine.TaskService.AddCandidateUser(taskId, userId2);

            // then
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.Read, Resources.Task, taskId));
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.Update, Resources.Task, taskId));

            deleteTask(taskId, true);
        }

        public virtual void testShouldGrantTaskWorkOnSetOwner()
        {

            // given
            processEngineConfiguration.DefaultUserPermissionForTask = Permissions.TaskWork;

            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            ProcessEngine.TaskService.SetOwner(taskId, userId2);

            // then
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.Read, Resources.Task, taskId));
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.TaskWork, Resources.Task, taskId));
            Assert.AreEqual(false, authorizationService.IsUserAuthorized(userId2, null, Permissions.Update, Resources.Task, taskId));

            deleteTask(taskId, true);
        }

        public virtual void testShouldGrantUpdateOnSetOwner()
        {

            // given
            processEngineConfiguration.DefaultUserPermissionForTask = Permissions.Update;

            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            ProcessEngine.TaskService.SetOwner(taskId, userId2);

            // then
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.Read, Resources.Task, taskId));
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, null, Permissions.Update, Resources.Task, taskId));

            deleteTask(taskId, true);
        }


        public virtual void testShouldGrantTaskWorkOnSetCandidateGroup()
        {

            // given
            processEngineConfiguration.DefaultUserPermissionForTask = Permissions.TaskWork;

            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            ProcessEngine.TaskService.AddCandidateGroup(taskId, groupId);

            // then
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, new List<string> { groupId}, Permissions.Read, Resources.Task, taskId));
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, new List<string> { groupId }, Permissions.TaskWork, Resources.Task, taskId));
            Assert.AreEqual(false, authorizationService.IsUserAuthorized(userId2, new List<string> { groupId }, Permissions.Update, Resources.Task, taskId));

            deleteTask(taskId, true);
        }

        public virtual void testShouldGrantUpdateOnSetCandidateGroup()
        {

            // given
            processEngineConfiguration.DefaultUserPermissionForTask = Permissions.Update;

            string taskId = "myTask";
            createTask(taskId);
            createGrantAuthorization(Resources.Task, taskId, userId, Permissions.Update);

            // when
            ProcessEngine.TaskService.AddCandidateGroup(taskId, groupId);

            // then
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, new List<string> { groupId }, Permissions.Read, Resources.Task, taskId));
            Assert.AreEqual(true, authorizationService.IsUserAuthorized(userId2, new List<string> { groupId }, Permissions.Update, Resources.Task, taskId));

            deleteTask(taskId, true);
        }

    }

}