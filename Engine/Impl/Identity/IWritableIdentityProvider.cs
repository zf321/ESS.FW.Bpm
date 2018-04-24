
using System.Text.RegularExpressions;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Identity
{
    /// <summary>
    ///     <para>
    ///         SPI Interface for identity service implementations which offer
    ///         read / write access to the user database.
    ///     </para>
    ///     
    /// </summary>
    public interface IWritableIdentityProvider /*: ISession*/
    {
        // users /////////////////////////////////////////////////

        /// <summary>
        ///     <para>
        ///         Returns a new (transient) <seealso cref="User" /> object. The Object is not
        ///         yet persistent and must be saved using the <seealso cref="#saveUser(User)" />
        ///         method.
        ///     </para>
        ///     <para>
        ///         NOTE: the implementation does not validate the uniqueness of the userId
        ///         parameter at this time.
        ///     </para>
        /// </summary>
        /// <param name="userId"> </param>
        /// <returns> an non-persistent user object. </returns>
        IUser CreateNewUser(string userId);

        /// <summary>
        ///     Allows saving or updates a <seealso cref="User" /> object
        /// </summary>
        /// <param name="user"> a User object. </param>
        /// <returns> the User object. </returns>
        /// <exception cref="IdentityProviderException"> in case an internal error occurs </exception>
        IUser SaveUser(IUser user);

        /// <summary>
        ///     Allows deleting a persistent <seealso cref="User" /> object.
        /// </summary>
        /// <param name="UserId"> the id of the User object to delete. </param>
        /// <exception cref="IdentityProviderException"> in case an internal error occurs </exception>
        void DeleteUser(string userId);


        // groups /////////////////////////////////////////////////

        /// <summary>
        ///     <para>
        ///         Returns a new (transient) <seealso cref="Group" /> object. The Object is not
        ///         yet persistent and must be saved using the <seealso cref="#saveGroup(Group)" />
        ///         method.
        ///     </para>
        ///     <para>
        ///         NOTE: the implementation does not validate the uniqueness of the groupId
        ///         parameter at this time.
        ///     </para>
        /// </summary>
        /// <param name="groupId"> </param>
        /// <returns> an non-persistent group object. </returns>
        IGroup CreateNewGroup(string groupId);

        /// <summary>
        ///     Allows saving a <seealso cref="IGroup" /> object which is not yet persistent.
        /// </summary>
        /// <param name="group"> a group object. </param>
        /// <returns> the persistent group object. </returns>
        /// <exception cref="IdentityProviderException"> in case an internal error occurs </exception>
        IGroup SaveGroup(IGroup group);

        /// <summary>
        ///     Allows deleting a persistent <seealso cref="Group" /> object.
        /// </summary>
        /// <param name="groupId"> the id of the group object to delete.   </param>
        /// <exception cref="IdentityProviderException"> in case an internal error occurs </exception>
        void DeleteGroup(string groupId);

        /// <summary>
        ///     <para>
        ///         Returns a new (transient) <seealso cref="Tenant" /> object. The Object is not yet
        ///         persistent and must be saved using the <seealso cref="#saveTenant(Tenant)" /> method.
        ///     </para>
        ///     <para>
        ///         NOTE: the implementation does not validate the uniqueness of the tenantId
        ///         parameter at this time.
        ///     </para>
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the new tenant
        /// </param>
        /// <returns> an non-persistent tenant object. </returns>
        ITenant CreateNewTenant(string tenantId);

        /// <summary>
        ///     Allows saving a <seealso cref="Tenant" /> object which is not yet persistent.
        /// </summary>
        /// <param name="tenant">
        ///     the tenant object to save.
        /// </param>
        /// <returns> the persistent tenant object. </returns>
        /// <exception cref="IdentityProviderException">
        ///     in case an internal error occurs
        /// </exception>
        ITenant SaveTenant(ITenant tenant);

        /// <summary>
        ///     Allows deleting a persistent <seealso cref="Tenant" /> object.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant object to delete.
        /// </param>
        /// <exception cref="IdentityProviderException">
        ///     in case an internal error occurs
        /// </exception>
        void DeleteTenant(string tenantId);

        // Membership ///////////////////////////////////////////////

        /// <summary>
        ///     Creates a membership relation between a user and a group. If the user is already part of that group,
        ///     IdentityProviderException is thrown.
        /// </summary>
        /// <param name="userId"> the id of the user </param>
        /// <param name="groupId"> id of the group </param>
        /// <exception cref="IdentityProviderException"> </exception>
        void CreateMembership(string userId, string groupId);

        /// <summary>
        ///     Deletes a membership relation between a user and a group.
        /// </summary>
        /// <param name="userId"> the id of the user </param>
        /// <param name="groupId"> id of the group </param>
        /// <exception cref="IdentityProviderException"> </exception>
        void DeleteMembership(string userId, string groupId);

        /// <summary>
        ///     Creates a membership relation between a tenant and a user.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant
        /// </param>
        /// <param name="userId">
        ///     the id of the user
        /// </param>
        void CreateTenantUserMembership(string tenantId, string userId);

        /// <summary>
        ///     Creates a membership relation between a tenant and a group.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant
        /// </param>
        /// <param name="groupId">
        ///     the id of the group
        /// </param>
        void CreateTenantGroupMembership(string tenantId, string groupId);

        /// <summary>
        ///     Deletes a membership relation between a tenant and a user.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant
        /// </param>
        /// <param name="userId">
        ///     the id of the user
        /// </param>
        void DeleteTenantUserMembership(string tenantId, string userId);

        /// <summary>
        ///     Deletes a membership relation between a tenant and a group.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant
        /// </param>
        /// <param name="groupId">
        ///     the id of the group
        /// </param>
        void DeleteTenantGroupMembership(string tenantId, string groupId);
    }
}