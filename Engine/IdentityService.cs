using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Service to manage <seealso cref="IUser" />s and <seealso cref="IGroup" />s.
    ///      
    ///     
    /// </summary>
    public interface IIdentityService
    {
        /// <summary>
        ///     <para>
        ///         Allows to inquire whether this identity service implementation provides
        ///         read-only access to the user repository, false otherwise.
        ///     </para>
        ///     Read only identity service implementations do not support the following methods:
        ///     <ul>
        ///         <li>
        ///             <seealso cref="#newUser(String)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#saveUser(User)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#deleteUser(String)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#newGroup(String)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#saveGroup(Group)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#deleteGroup(String)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#newTenant(String)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#saveTenant(Tenant)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#deleteTenant(String)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#createMembership(String, String)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#deleteMembership(String, String)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#createTenantUserMembership(String, String)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#createTenantGroupMembership(String, String)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#deleteTenantUserMembership(String, String)" />
        ///         </li>
        ///         <li>
        ///             <seealso cref="#deleteTenantGroupMembership(String, String)" />
        ///         </li>
        ///     </ul>
        ///     <para>
        ///         If these methods are invoked on a read-only identity service implementation,
        ///         the invocation will throw an <seealso cref="UnsupportedOperationException" />.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     true if this identity service implementation provides read-only
        ///     access to the user repository, false otherwise.
        /// </returns>
        bool ReadOnly { get; }

        /// <summary>
        ///     Passes the authenticated user id for this thread.
        ///     All service method (from any service) invocations done by the same
        ///     thread will have access to this authenticatedUserId. Should be followed by
        ///     a call to <seealso cref="#clearAuthentication()" /> once the interaction is terminated.
        /// </summary>
        /// <param name="authenticatedUserId"> the id of the current user. </param>
        string AuthenticatedUserId { set; }

        /// <param name="currentAuthentication"> </param>
        Authentication Authentication { set; }

        /// <returns> the current authentication for this process engine. </returns>
        Authentication CurrentAuthentication { get; }

        /// <summary>
        ///     Creates a new user. The user is transient and must be saved using
        ///     <seealso cref="#saveUser(User)" />.
        /// </summary>
        /// <param name="userId"> id for the new user, cannot be null. </param>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permissions on
        ///     <seealso cref="Resources#USER" />.
        /// </exception>
        IUser NewUser(string userId);

        /// <summary>
        ///     Saves the user. If the user already existed, the user is updated.
        /// </summary>
        /// <param name="user"> user to save, cannot be null. </param>
        /// <exception cref="RuntimeException"> when a user with the same name already exists. </exception>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#UPDATE" /> permissions on <seealso cref="Resources#USER" /> (update
        ///     existing user)
        ///     or if user has no <seealso cref="Permissions#CREATE" /> permissions on <seealso cref="Resources#USER" /> (save new
        ///     user).
        /// </exception>
        void SaveUser(IUser user);

        /// <summary>
        ///     Creates a <seealso cref="UserQuery" /> that allows to programmatically query the users.
        /// </summary>
        IQueryable<IUser> CreateUserQuery(Expression<Func<UserEntity, bool>> expression =null );

        /// <param name="userId">
        ///     id of user to delete, cannot be null. When an id is passed
        ///     for an unexisting user, this operation is ignored.
        /// </param>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#DELETE" /> permissions on
        ///     <seealso cref="Resources#USER" />.
        /// </exception>
        void DeleteUser(string userId);

        /// <summary>
        ///     Creates a new group. The group is transient and must be saved using
        ///     <seealso cref="#saveGroup(Group)" />.
        /// </summary>
        /// <param name="groupId"> id for the new group, cannot be null. </param>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permissions on
        ///     <seealso cref="Resources#GROUP" />.
        /// </exception>
        IGroup NewGroup(string groupId);

        /// <summary>
        ///     Creates a <seealso cref="GroupQuery" /> thats allows to programmatically query the groups.
        /// </summary>
        IQueryable<IGroup> CreateGroupQuery();

        /// <summary>
        ///     Saves the group. If the group already existed, the group is updated.
        /// </summary>
        /// <param name="group"> group to save. Cannot be null. </param>
        /// <exception cref="RuntimeException"> when a group with the same name already exists. </exception>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#UPDATE" /> permissions on <seealso cref="Resources#GROUP" /> (update
        ///     existing group)
        ///     or if user has no <seealso cref="Permissions#CREATE" /> permissions on <seealso cref="Resources#GROUP" /> (save new
        ///     group).
        /// </exception>
        void SaveGroup(IGroup group);

        /// <summary>
        ///     Deletes the group. When no group exists with the given id, this operation
        ///     is ignored.
        /// </summary>
        /// <param name="groupId"> id of the group that should be deleted, cannot be null. </param>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#DELETE" /> permissions on
        ///     <seealso cref="Resources#GROUP" />.
        /// </exception>
        void DeleteGroup(string groupId);

        /// <param name="userId"> the userId, cannot be null. </param>
        /// <param name="groupId"> the groupId, cannot be null. </param>
        /// <exception cref="RuntimeException">
        ///     when the given user or group doesn't exist or when the user
        ///     is already member of the group.
        /// </exception>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permissions on
        ///     <seealso cref="Resources#GROUP_MEMBERSHIP" />.
        /// </exception>
        void CreateMembership(string userId, string groupId);

        /// <summary>
        ///     Delete the membership of the user in the group. When the group or user don't exist
        ///     or when the user is not a member of the group, this operation is ignored.
        /// </summary>
        /// <param name="userId"> the user's id, cannot be null. </param>
        /// <param name="groupId"> the group's id, cannot be null. </param>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#DELETE" /> permissions on
        ///     <seealso cref="Resources#GROUP_MEMBERSHIP" />.
        /// </exception>
        void DeleteMembership(string userId, string groupId);

        /// <summary>
        ///     Creates a new tenant. The tenant is transient and must be saved using
        ///     <seealso cref="#saveTenant(Tenant)" />.
        /// </summary>
        /// <param name="tenantId">
        ///     id for the new tenant, cannot be <code>null</code>.
        /// </param>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permissions on
        ///     <seealso cref="Resources#TENANT" />.
        /// </exception>
        ITenant NewTenant(string tenantId);

        /// <summary>
        ///     Creates a <seealso cref="TenantQuery" /> thats allows to programmatically query the
        ///     tenants.
        /// </summary>
        IQueryable<ITenant> CreateTenantQuery(Expression<Func<TenantEntity,bool>> expression=null );

        /// <summary>
        ///     Saves the tenant. If the tenant already existed, it is updated.
        /// </summary>
        /// <param name="tenant">
        ///     the tenant to save. Cannot be <code>null</code>.
        /// </param>
        /// <exception cref="RuntimeException">
        ///     when a tenant with the same name already exists.
        /// </exception>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#UPDATE" /> permissions on
        ///     <seealso cref="Resources#TENANT" /> (update existing tenant) or if user has
        ///     no <seealso cref="Permissions#CREATE" /> permissions on
        ///     <seealso cref="Resources#TENANT" /> (save new tenant).
        /// </exception>
        void SaveTenant(ITenant tenant);

        /// <summary>
        ///     Deletes the tenant. When no tenant exists with the given id, this operation
        ///     is ignored.
        /// </summary>
        /// <param name="tenantId">
        ///     id of the tenant that should be deleted, cannot be
        ///     <code>null</code>.
        /// </param>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#DELETE" /> permissions on
        ///     <seealso cref="Resources#TENANT" />.
        /// </exception>
        void DeleteTenant(string tenantId);

        /// <summary>
        ///     Creates a new membership between the given user and tenant.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant, cannot be null.
        /// </param>
        /// <param name="userId">
        ///     the id of the user, cannot be null.
        /// </param>
        /// <exception cref="RuntimeException">
        ///     when the given tenant or user doesn't exist or the user is
        ///     already a member of this tenant.
        /// </exception>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permissions on
        ///     <seealso cref="Resources#TENANT_MEMBERSHIP" />.
        /// </exception>
        void CreateTenantUserMembership(string tenantId, string userId);

        /// <summary>
        ///     Creates a new membership between the given group and tenant.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant, cannot be null.
        /// </param>
        /// <param name="groupId">
        ///     the id of the group, cannot be null.
        /// </param>
        /// <exception cref="RuntimeException">
        ///     when the given tenant or group doesn't exist or when the group
        ///     is already a member of this tenant.
        /// </exception>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permissions on
        ///     <seealso cref="Resources#TENANT_MEMBERSHIP" />.
        /// </exception>
        void CreateTenantGroupMembership(string tenantId, string groupId);

        /// <summary>
        ///     Deletes the membership between the given user and tenant. The operation is
        ///     ignored when the given user, tenant or membership don't exist.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant, cannot be null.
        /// </param>
        /// <param name="userId">
        ///     the id of the user, cannot be null.
        /// </param>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#DELETE" /> permissions on
        ///     <seealso cref="Resources#TENANT_MEMBERSHIP" />.
        /// </exception>
        void DeleteTenantUserMembership(string tenantId, string userId);

        /// <summary>
        ///     Deletes the membership between the given group and tenant. The operation is
        ///     ignored when the given group, tenant or membership don't exist.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant, cannot be null.
        /// </param>
        /// <param name="groupId">
        ///     the id of the group, cannot be null.
        /// </param>
        /// <exception cref="UnsupportedOperationException">
        ///     if identity service implementation is read only. See
        ///     <seealso cref="#isReadOnly()" />
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#DELETE" /> permissions on
        ///     <seealso cref="Resources#TENANT_MEMBERSHIP" />.
        /// </exception>
        void DeleteTenantGroupMembership(string tenantId, string groupId);

        /// <summary>
        ///     Checks if the password is valid for the given user. Arguments userId
        ///     and password are nullsafe.
        /// </summary>
        bool CheckPassword(string userId, string password);

        /// <summary>
        ///     Passes the authenticated user id and groupIds for this thread.
        ///     All service method (from any service) invocations done by the same
        ///     thread will have access to this authentication. Should be followed by
        ///     a call to <seealso cref="#clearAuthentication()" /> once the interaction is terminated.
        /// </summary>
        /// <param name="authenticatedUserId"> the id of the current user. </param>
        /// <param name="groups"> the groups of the current user. </param>
        void SetAuthentication(string userId, IList<string> groups);

        /// <summary>
        ///     Passes the authenticated user id, group ids and tenant ids for this thread.
        ///     All service method (from any service) invocations done by the same
        ///     thread will have access to this authentication. Should be followed by
        ///     a call to <seealso cref="#clearAuthentication()" /> once the interaction is terminated.
        /// </summary>
        /// <param name="userId"> the id of the current user. </param>
        /// <param name="groups"> the groups of the current user. </param>
        /// <param name="tenantIds"> the tenants of the current user. </param>
        void SetAuthentication(string userId, IList<string> groups, IList<string> tenantIds);

        /// <summary>
        ///     Allows clearing the current authentication. Does not throw exception if
        ///     no authentication exists.
        /// </summary>
        void ClearAuthentication();

        /// <summary>
        ///     Sets the picture for a given user.
        /// </summary>
        /// <exception cref="ProcessEngineException"> if the user doesn't exist. </exception>
        /// <param name="picture"> can be null to delete the picture.  </param>
        void SetUserPicture(string userId, Picture picture);

        /// <summary>
        ///     Retrieves the picture for a given user.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     if the user doesn't exist.
        ///     @returns null if the user doesn't have a picture.
        /// </exception>
        Picture GetUserPicture(string userId);

        /// <summary>
        ///     Deletes the picture for a given user. If the user does not have a picture or if the user doesn't exists the call is
        ///     ignored.
        /// </summary>
        /// <exception cref="ProcessEngineException"> if the user doesn't exist.  </exception>
        void DeleteUserPicture(string userId);

        /// <summary>
        ///     Generic extensibility key-value pairs associated with a user
        /// </summary>
        void SetUserInfo(string userId, string key, string value);

        /// <summary>
        ///     Generic extensibility key-value pairs associated with a user
        /// </summary>
        string GetUserInfo(string userId, string key);

        /// <summary>
        ///     Generic extensibility keys associated with a user
        /// </summary>
        IList<string> GetUserInfoKeys(string userId);

        /// <summary>
        ///     Delete an entry of the generic extensibility key-value pairs associated with a user
        /// </summary>
        void DeleteUserInfo(string userId, string key);

        /// <summary>
        ///     Store account information for a remote system
        /// </summary>
        [Obsolete]
        void SetUserAccount(string userId, string userPassword, string accountName, string accountUsername,
            string accountPassword, IDictionary<string, string> accountDetails);

        /// <summary>
        ///     Get account names associated with the given user
        /// </summary>
        [Obsolete]
        IList<string> GetUserAccountNames(string userId);

        /// <summary>
        ///     Get account information associated with a user
        /// </summary>
        [Obsolete]
        IAccount GetUserAccount(string userId, string userPassword, string accountName);

        /// <summary>
        ///     Delete an entry of the generic extensibility key-value pairs associated with a user
        /// </summary>
        [Obsolete]
        void DeleteUserAccount(string userId, string accountName);
    }
}