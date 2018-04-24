using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Filter
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class FilterAuthorizationsTest : PluggableProcessEngineTestCase
    {
        protected internal IUser testUser;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IAuthorization createAuthorization_Renamed;
        protected internal IAuthorization updateAuthorization;
        protected internal IAuthorization readAuthorization;
        protected internal IAuthorization deleteAuthorization;

        [SetUp]
        public void setUp()
        {
            testUser = createTestUser("test");

            createAuthorization_Renamed = createAuthorization(Permissions.Create, AuthorizationFields.Any);
            updateAuthorization = createAuthorization(Permissions.Update, null);
            readAuthorization = createAuthorization(Permissions.Read, null);
            deleteAuthorization = createAuthorization(Permissions.Delete, null);

            processEngineConfiguration.SetAuthorizationEnabled(true);
            identityService.AuthenticatedUserId = testUser.Id;
        }
        [TearDown]
        public void tearDown()
        {
            processEngineConfiguration.SetAuthorizationEnabled(false);
            foreach (var filter in filterService.CreateFilterQuery()
                
                .ToList())
                filterService.DeleteFilter(filter.Id);
            foreach (var user in identityService.CreateUserQuery()
                
                .ToList())
                identityService.DeleteUser(user.Id);
            foreach (var authorization in authorizationService.CreateAuthorizationQuery()
                
                .ToList())
                authorizationService.DeleteAuthorization(authorization.Id);
        }
        [Test]
        public virtual void testCreateFilterNotPermitted()
        {
            try
            {
                filterService.NewTaskFilter();
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException)
            {
                // expected
            }
        }

        [Test]
        public virtual void testCreateFilterPermitted()
        {
            grantCreateFilter();
            var filter = filterService.NewTaskFilter();
            Assert.NotNull(filter);
        }

        [Test]
        public virtual void testSaveFilterNotPermitted()
        {
            IFilter filter = new FilterEntity(Resources.Task.ToString());
            try
            {
                filterService.SaveFilter(filter);
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException)
            {
                // expected
            }
        }

        [Test]
        public virtual void testSaveFilterPermitted()
        {
            var filter = new FilterEntity(Resources.Task.ToString()).SetName("testFilter");

            grantCreateFilter();

            filterService.SaveFilter(filter);

            Assert.NotNull(filter.Id);
        }

        [Test]
        public virtual void testUpdateFilterNotPermitted()
        {
            var filter = createTestFilter();

            filter.SetName("anotherName");

            try
            {
                filterService.SaveFilter(filter);
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException)
            {
                // expected
            }
        }

        [Test]
        public virtual void testUpdateFilterPermitted()
        {
            var filter = createTestFilter();

            filter.SetName("anotherName");

            grantUpdateFilter(filter.Id);

            filter = filterService.SaveFilter(filter);
            Assert.AreEqual("anotherName", filter.Name);
        }

        [Test]
        public virtual void testDeleteFilterNotPermitted()
        {
            var filter = createTestFilter();

            try
            {
                filterService.DeleteFilter(filter.Id);
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException)
            {
                // expected
            }
        }

        [Test]
        public virtual void testDeleteFilterPermitted()
        {
            var filter = createTestFilter();

            grantDeleteFilter(filter.Id);

            filterService.DeleteFilter(filter.Id);

            var Count = filterService.CreateFilterQuery()
                .Count();
            Assert.AreEqual(0, Count);
        }

        [Test]
        public virtual void testReadFilterNotPermitted()
        {
            var filter = createTestFilter();

            var Count = filterService.CreateFilterQuery()
                .Count();
            Assert.AreEqual(0, Count);

            var returnedFilter = filterService.CreateFilterQuery()
                //.FilterId(filter.Id)
                .First();
            Assert.IsNull(returnedFilter);

            try
            {
                filterService.GetFilter(filter.Id);
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException)
            {
                // expected
            }

            try
            {
                //filterService.SingleResult(filter.Id);
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException)
            {
                // expected
            }

            try
            {
                //filterService.List(filter.Id);
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException)
            {
                // expected
            }

            try
            {
                //filterService.ListPage(filter.Id, 1, 2);
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException)
            {
                // expected
            }

            try
            {
                filterService.Count(filter.Id);
                Assert.Fail("Exception expected");
            }
            catch (AuthorizationException)
            {
                // expected
            }
        }

        [Test]
        public virtual void testReadFilterPermitted()
        {
            var filter = createTestFilter();

            grantReadFilter(filter.Id);

            var Count = filterService.CreateFilterQuery()
                .Count();
            Assert.AreEqual(1, Count);

            var returnedFilter = filterService.CreateFilterQuery()
                //.FilterId(filter.Id)
                .First();
            Assert.NotNull(returnedFilter);

            returnedFilter = filterService.GetFilter(filter.Id);
            Assert.NotNull(returnedFilter);

            // create test Task
            var task = taskService.NewTask("test");
            taskService.SaveTask(task);

            var result = filterService.SingleResult<ITask>(filter.Id);
            Assert.NotNull(result);
            Assert.AreEqual(task.Id, result.Id);

            IList<ITask> resultList = filterService.List<ITask>(filter.Id);
            Assert.NotNull(resultList);
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(task.Id, resultList[0].Id);

            //resultList = filterService.ListPage(filter.Id, 0, 2);
            Assert.NotNull(resultList);
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(task.Id, resultList[0].Id);

            Count = (int) filterService.Count(filter.Id).Value;
            Assert.AreEqual(1, Count);

            // remove Task
            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        public virtual void testReadFilterPermittedWithMultiple()
        {
            var filter = createTestFilter();

            grantReadFilter(filter.Id);
            var authorization =
                ProcessEngine.AuthorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
            authorization.AddPermission(Permissions.Read);
            authorization.UserId = AuthorizationFields.Any;
            authorization.Resource = Resources.Filter;
            authorization.ResourceId = AuthorizationFields.Any;
            ProcessEngine.AuthorizationService.SaveAuthorization(authorization);

            var Count = filterService.CreateFilterQuery()
                .Count();
            Assert.AreEqual(1, Count);

            var returnedFilter = filterService.CreateFilterQuery()
                //.FilterId(filter.Id)
                .First();
            Assert.NotNull(returnedFilter);

            returnedFilter = filterService.GetFilter(filter.Id);
            Assert.NotNull(returnedFilter);

            ProcessEngine.AuthorizationService.DeleteAuthorization(authorization.Id);
        }

        [Test]
        public virtual void testDefaultFilterAuthorization()
        {
            // create two other users beside testUser
            var ownerUser = createTestUser("ownerUser");
            var anotherUser = createTestUser("anotherUser");

            // grant testUser create permission
            grantCreateFilter();

            // create a new filter with ownerUser as owner
            var filter = filterService.NewTaskFilter("testFilter");
            filter.SetOwner(ownerUser.Id);
            filterService.SaveFilter(filter);

            AssertFilterPermission(Permissions.Create, testUser, null, true);
            AssertFilterPermission(Permissions.Create, ownerUser, null, false);
            AssertFilterPermission(Permissions.Create, anotherUser, null, false);

            AssertFilterPermission(Permissions.Update, testUser, filter.Id, false);
            AssertFilterPermission(Permissions.Update, ownerUser, filter.Id, true);
            AssertFilterPermission(Permissions.Update, anotherUser, filter.Id, false);

            AssertFilterPermission(Permissions.Read, testUser, filter.Id, false);
            AssertFilterPermission(Permissions.Read, ownerUser, filter.Id, true);
            AssertFilterPermission(Permissions.Read, anotherUser, filter.Id, false);

            AssertFilterPermission(Permissions.Delete, testUser, filter.Id, false);
            AssertFilterPermission(Permissions.Delete, ownerUser, filter.Id, true);
            AssertFilterPermission(Permissions.Delete, anotherUser, filter.Id, false);
        }

        [Test]
        public virtual void testCreateFilterGenericOwnerId()
        {
            grantCreateFilter();

            var filter = filterService.NewTaskFilter("someName");
            filter.SetOwner("*");

            try
            {
                filterService.SaveFilter(filter);
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent(
                    "Cannot create default authorization for filter owner *: " +
                    "id cannot be *. * is a reserved identifier.", e.Message);
            }
        }

        /// <summary>
        ///     CAM-4889
        /// </summary>
        [Test]
        public virtual void FAILING_testUpdateFilterGenericOwnerId()
        {
            grantCreateFilter();

            var filter = filterService.NewTaskFilter("someName");
            filterService.SaveFilter(filter);

            grantUpdateFilter(filter.Id);
            filter.SetOwner("*");

            try
            {
                filterService.SaveFilter(filter);
                Assert.Fail("it should not be possible to save a filter with the generic owner id");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("foo", e.Message);
            }
        }

        protected internal virtual IUser createTestUser(string userId)
        {
            var user = identityService.NewUser(userId);
            identityService.SaveUser(user);

            // give user all permission to manipulate authorisations
            var authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
            authorization.UserId = user.Id;
            authorization.Resource = Resources.Authorization;
            authorization.ResourceId = AuthorizationFields.Any;
            authorization.AddPermission(Permissions.All);
            authorizationService.SaveAuthorization(authorization);

            // give user all permission to manipulate users
            authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
            authorization.UserId = user.Id;
            authorization.Resource = Resources.User;
            authorization.ResourceId = AuthorizationFields.Any;
            authorization.AddPermission(Permissions.All);
            authorizationService.SaveAuthorization(authorization);

            authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
            authorization.UserId = user.Id;
            authorization.Resource = Resources.Task;
            authorization.ResourceId = AuthorizationFields.Any;
            authorization.AddPermission(Permissions.All);
            authorizationService.SaveAuthorization(authorization);

            return user;
        }

        protected internal virtual IFilter createTestFilter()
        {
            grantCreateFilter();
            var filter = filterService.NewTaskFilter("testFilter");
            return filterService.SaveFilter(filter);
        }

        protected internal virtual IAuthorization createAuthorization(Permissions permission, string resourceId)
        {
            var authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
            authorization.UserId = testUser.Id;
            authorization.Resource = Resources.Filter;
            authorization.AddPermission(permission);
            if (!ReferenceEquals(resourceId, null))
                authorization.ResourceId = resourceId;
            return authorization;
        }

        protected internal virtual void grantCreateFilter()
        {
            grantFilterPermission(createAuthorization_Renamed, null);
            AssertFilterPermission(Permissions.Create, testUser, null, true);
        }

        protected internal virtual void grantUpdateFilter(string filterId)
        {
            grantFilterPermission(updateAuthorization, filterId);
            AssertFilterPermission(Permissions.Update, testUser, filterId, true);
        }

        protected internal virtual void grantReadFilter(string filterId)
        {
            grantFilterPermission(readAuthorization, filterId);
            AssertFilterPermission(Permissions.Read, testUser, filterId, true);
        }

        protected internal virtual void grantDeleteFilter(string filterId)
        {
            grantFilterPermission(deleteAuthorization, filterId);
            AssertFilterPermission(Permissions.Delete, testUser, filterId, true);
        }

        protected internal virtual void grantFilterPermission(IAuthorization authorization, string filterId)
        {
            if (!ReferenceEquals(filterId, null))
                authorization.ResourceId = filterId;
            authorizationService.SaveAuthorization(authorization);
        }

        protected internal virtual void AssertFilterPermission(Permissions permission, IUser user, string filterId,
            bool expected)
        {
            bool result;
            if (!ReferenceEquals(filterId, null))
                result = authorizationService.IsUserAuthorized(user.Id, null, permission, Resources.Filter, filterId);
            else
                result = authorizationService.IsUserAuthorized(user.Id, null, permission, Resources.Filter);
            Assert.AreEqual(expected, result);
        }
    }
}