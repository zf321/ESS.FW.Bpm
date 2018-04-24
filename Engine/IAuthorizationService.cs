using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     <para>The authorization service allows managing <seealso cref="IAuthorization" />.</para>
    ///     <h2>Creating an authorization</h2>
    ///     <para>
    ///         An authorization is created between a user/group and a resource. It describes
    ///         the user/group's <em>permissions</em> to access that resource. An authorization may
    ///         express different permissions, such as the permission to READ, WRITE, DELETE the
    ///         resource. (See <seealso cref="IAuthorization" /> for details).
    ///     </para>
    ///     <h2>Granting / revoking permissions</h2>
    ///     <para>
    ///         In order to grant the permission to access a certain resource, an authorization
    ///         object is created:
    ///         <pre>
    ///             Authorization auth = authorizationService.createNewAuthorization();
    ///             //... configure auth
    ///             authorizationService.saveAuthorization(auth);
    ///         </pre>
    ///         The authorization object can be configured either for a user or a group:
    ///         <pre>
    ///             auth.setUserId("john");
    ///             -OR-
    ///             auth.setGroupId("management");
    ///         </pre>
    ///         and a resource:
    ///         <pre>
    ///             auth.setResource("processDefinition");
    ///             auth.setResourceId("2313");
    ///         </pre>
    ///         finally the permissions to access that resource can be assigned:
    ///         <pre>
    ///             auth.addPermission(Permissions.READ);
    ///         </pre>
    ///         and the authorization object is saved:
    ///         <pre>
    ///             authorizationService.saveAuthorization(auth);
    ///         </pre>
    ///         As a result, the given user or group will have permission to READ the referenced process definition.
    ///     </para>
    ///     
    ///     
    /// </summary>
    public interface IAuthorizationService
    {
        // Authorization CRUD //////////////////////////////////////

        /// <summary>
        ///     <para>
        ///         Returns a new (transient) <seealso cref="IAuthorization" /> object. The Object is not
        ///         yet persistent and must be saved using the <seealso cref="#saveAuthorization(Authorization)" />
        ///         method.
        ///     </para>
        /// </summary>
        /// <param name="authorizationType">
        ///     the type of the authorization. Legal values: <seealso cref="IAuthorization#AUTH_TYPE_GLOBAL" />,
        ///     <seealso cref="IAuthorization#AUTH_TYPE_GRANT" />, <seealso cref="IAuthorization#AUTH_TYPE_REVOKE" />
        /// </param>
        /// <returns> an non-persistent Authorization object. </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permissions on
        ///     <seealso cref="Resources#AUTHORIZATION" />.
        /// </exception>
        IAuthorization CreateNewAuthorization(int authorizationType);

        /// <summary>
        ///     Allows saving an <seealso cref="IAuthorization" /> object. Use this method for persisting new
        ///     transient <seealso cref="IAuthorization" /> objects obtained through
        ///     <seealso cref="#createNewAuthorization(int)" />
        ///     or
        ///     for updating persistent objects.
        /// </summary>
        /// <param name="authorization"> a Authorization object. </param>
        /// <returns> the authorization object. </returns>
        /// <exception cref="ProcessEngineException"> in case an internal error occurs </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no
        ///     <seealso cref="Permissions#CREATE" /> permissions (in case of persisting a transient object) or no
        ///     <seealso cref="Permissions#UPDATE" /> permissions (in case of updating a persistent object)
        ///     on <seealso cref="Resources#AUTHORIZATION" />
        /// </exception>
        IAuthorization SaveAuthorization(IAuthorization authorization);

        /// <summary>
        ///     Allows deleting a persistent <seealso cref="IAuthorization" /> object.
        /// </summary>
        /// <param name="authorizationId"> the id of the Authorization object to delete. </param>
        /// <exception cref="ProcessEngineException"> if no such authorization exists or if an internal error occurs. </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#DELETE" /> permissions on
        ///     <seealso cref="Resources#AUTHORIZATION" />.
        /// </exception>
        void DeleteAuthorization(string authorizationId);

        /// <summary>
        ///     Constructs an authorization query.
        /// </summary>
        IQueryable<IAuthorization> CreateAuthorizationQuery(Expression<Func<AuthorizationEntity, bool>> expression =null);

        // Authorization Checks ////////////////////////////////

        /// <summary>
        ///     <para>Allows performing an authorization check.</para>
        ///     <para>
        ///         Returns true if the given user has permissions for interacting with the resource is the
        ///         requested way.
        ///     </para>
        ///     <para>This method checks for the resource type, see <seealso cref="IAuthorization#ANY" /></para>
        /// </summary>
        /// <param name="userId"> the id of the user for which the check is performed. </param>
        /// <param name="groupIds"> a list of group ids the user is member of </param>
        /// <param name="permission"> the permission(s) to check for. </param>
        /// <param name="resource"> the resource for which the authorization is checked. </param>
        bool IsUserAuthorized(string userId, IList<string> groupIds, Permissions permission, Resources resource);

        /// <summary>
        ///     <para>Allows performing an authorization check.</para>
        ///     <para>
        ///         Returns true if the given user has permissions for interacting with the resource is the
        ///         requested way.
        ///     </para>
        /// </summary>
        /// <param name="userId"> the id of the user for which the check is performed. </param>
        /// <param name="groupIds"> a list of group ids the user is member of </param>
        /// <param name="permission"> the permission(s) to check for. </param>
        /// <param name="resource"> the resource for which the authorization is checked. </param>
        /// <param name="resourceId"> the resource id for which the authorization check is performed. </param>
        bool IsUserAuthorized(string userId, IList<string> groupIds, Permissions permission, Resources resource,
            string resourceId);
    }
}