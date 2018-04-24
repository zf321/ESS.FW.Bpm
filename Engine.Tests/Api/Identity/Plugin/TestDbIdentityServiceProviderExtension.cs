/// 

using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;
using ESS.FW.Bpm.Engine.Identity.Db;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;

namespace Engine.Tests.Api.Identity.Plugin
{
    /// <summary>
    ///     To create a testcase, that tests the write a Member/Group/Membership in
    ///     on step, an entry Point into the write option within the same Command Context is needed.
    ///     This is done by extending the to-test class and overriding a not in scope Method.
    ///     This Method will trigger the write of Member/Group/Membership in one step.
    ///     <br>
    ///         <br>
    ///             The IGroup will be the userId extended by _group
    ///             <br>
    ///                 <br>
    ///                     The checkPassword method must return true, because exactly the requested user with the
    ///                     requested Password will be created within this Method
    /// </summary>
    public class TestDbIdentityServiceProviderExtension : DbIdentityServiceProvider
    {
        public TestDbIdentityServiceProviderExtension( DbContext dbContext,
            ILoggerFactory loggerFactory, IDGenerator idGenerator,
            IRepository<GroupEntity, string> groupRepository) : base(dbContext, loggerFactory, idGenerator, groupRepository)
        {
        }

        public override bool CheckPassword(string userId, string password)
        {
            // Create and Save a User
            var user = CreateNewUser(userId);
            user.Password = password;
            SaveUser(user);

            // Create and Save a Group
            var groupId = userId + "_group";
            var group = CreateNewGroup(groupId);
            group.Name = groupId;
            SaveGroup(group);

            // Create the corresponding Membership
            CreateMembership(userId, groupId);

            return base.CheckPassword(userId, password);
        }
    }
}