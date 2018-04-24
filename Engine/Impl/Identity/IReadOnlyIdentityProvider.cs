using System.Linq;
using System.Text.RegularExpressions;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Identity
{
    /// <summary>
    ///     <para>interface for read-only identity Service Providers.</para>
    ///     <para>
    ///         This interface provides access to a read-only user / group
    ///         repository
    ///     </para>
    ///     
    /// </summary>
    public interface IReadOnlyIdentityProvider/* : ISession*/
    {
        // users ////////////////////////////////////////

        /// <returns> a <seealso cref="User" /> object for the given user id or null if no such user exists. </returns>
        /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
        IUser FindUserById(string userId);

        /// <summary>
        /// 兼容jzt wf
        /// </summary>
        /// <param name="workflowid"></param>
        /// <param name="state"></param>
        /// <param name="branchId"></param>
        /// <param name="originator"></param>
        /// <returns></returns>
        IList<string> FindAuditUserId(string workflowid,string state,string branchId,string originator);

        /// <returns> a <seealso cref="UserQuery" /> object which can be used for querying for users. </returns>
        /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
        IQueryable<IUser> CreateUserQuery();

        /// <returns> a <seealso cref="UserQuery" /> object which can be used in the current command context </returns>
        /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
        //IQueryable<IUser> CreateUserQuery(CommandContext commandContext);

        /// <returns> 'true' if the password matches the </returns>
        /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
        bool CheckPassword(string userId, string password);

        // groups //////////////////////////////////////

        /// <returns> a <seealso cref="Group" /> object for the given group id or null if no such group exists. </returns>
        /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
        IGroup FindGroupById(string groupId);

        /// <returns> a <seealso cref="GroupQuery" /> object which can be used for querying for groups. </returns>
        /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
        IQueryable<IGroup> CreateGroupQuery();

        /// <returns>
        ///     a <seealso cref="GroupQuery" /> object which can be used for querying for groups and can be reused in the
        ///     current command context.
        /// </returns>
        /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
        IQueryable<IGroup> CreateGroupQuery(CommandContext commandContext);

        // tenants //////////////////////////////////////

        /// <returns>
        ///     a <seealso cref="Tenant" /> object for the given id or null if no such tenant
        ///     exists.
        /// </returns>
        /// <exception cref="IdentityProviderException">
        ///     in case an error occurs
        /// </exception>
        ITenant FindTenantById(string tenantId);

        /// <returns>
        ///     a <seealso cref="TenantQuery" /> object which can be used for querying for
        ///     tenants.
        /// </returns>
        /// <exception cref="IdentityProviderException">
        ///     in case an error occurs
        /// </exception>
        IQueryable<ITenant> CreateTenantQuery();

        /// <returns>
        ///     a <seealso cref="TenantQuery" /> object which can be used for querying for
        ///     tenants and can be reused in the current command context.
        /// </returns>
        /// <exception cref="IdentityProviderException">
        ///     in case an error occurs
        /// </exception>
        IQueryable<ITenant> CreateTenantQuery(CommandContext commandContext);
    }
}