using System.Text.RegularExpressions;
using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cfg.Auth
{
    /// <summary>
    ///     <para>
    ///         Manages (create/update/delete) default authorization when an entity is
    ///         changed
    ///     </para>
    ///     <para>
    ///         Implementations should throw an exception when a specific resource's id is <code>*</code>, as
    ///         <code>*</code> represents access to all resources/by all users.
    ///     </para>
    ///     
    /// </summary>
    public interface IResourceAuthorizationProvider
    {
        // Users /////////////////////////////////////////////

        /// <summary>
        ///     <para>Invoked whenever a new user is created</para>
        /// </summary>
        /// <param name="user">
        ///     a newly created user
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new user
        ///     is created.
        /// </returns>
        AuthorizationEntity[] NewUser(IUser user);

        /// <summary>
        ///     <para>Invoked whenever a new group is created</para>
        /// </summary>
        /// <param name="group">
        ///     a newly created <seealso cref="Group" />
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     <seealso cref="Group" /> is created.
        /// </returns>
        AuthorizationEntity[] NewGroup(IGroup group);

        /// <summary>
        ///     <para>
        ///         Invoked whenever a new tenant is created
        ///     </para>
        /// </summary>
        /// <param name="tenant">
        ///     a newly created <seealso cref="Tenant" />
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     <seealso cref="Tenant" /> is created.
        /// </returns>
        AuthorizationEntity[] NewTenant(ITenant tenant);

        /// <summary>
        ///     <para>Invoked whenever a user is added to a group</para>
        /// </summary>
        /// <param name="userId">
        ///     the id of the user who is added to a group a newly created
        ///     <seealso cref="User" />
        /// </param>
        /// <param name="groupId">
        ///     the id of the group to which the user is added
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     <seealso cref="User" /> is created.
        /// </returns>
        AuthorizationEntity[] GroupMembershipCreated(string groupId, string userId);

        /// <summary>
        ///     <para>Invoked whenever an user is added to a tenant.</para>
        /// </summary>
        /// <param name="tenant">
        ///     the id of the tenant
        /// </param>
        /// <param name="userId">
        ///     the id of the user
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     membership is created.
        /// </returns>
        AuthorizationEntity[] TenantMembershipCreated(ITenant tenant, IUser user);

        /// <summary>
        ///     <para>Invoked whenever a group is added to a tenant.</para>
        /// </summary>
        /// <param name="tenant">
        ///     the id of the tenant
        /// </param>
        /// <param name="groupId">
        ///     the id of the group
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     membership is created.
        /// </returns>
        AuthorizationEntity[] TenantMembershipCreated(ITenant tenant, IGroup group);

        // Filter ////////////////////////////////////////////////

        /// <summary>
        ///     <para>Invoked whenever a new filter is created</para>
        /// </summary>
        /// <param name="filter"> the newly created filter </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     <seealso cref="IFilter" /> is created.
        /// </returns>
        AuthorizationEntity[] NewFilter(IFilter filter);

        // Deployment //////////////////////////////////////////////

        /// <summary>
        ///     <para>Invoked whenever a new deployment is created</para>
        /// </summary>
        /// <param name="deployment"> the newly created deployment </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     <seealso cref="Deployment" /> is created.
        /// </returns>
        AuthorizationEntity[] NewDeployment(IDeployment deployment);

        // Process Definition //////////////////////////////////////

        /// <summary>
        ///     <para>Invoked whenever a new process definition is created</para>
        /// </summary>
        /// <param name="processDefinition"> the newly created process definition </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     <seealso cref="IProcessDefinition" /> is created.
        /// </returns>
        AuthorizationEntity[] NewProcessDefinition(IProcessDefinition processDefinition);

        // Process Instance ///////////////////////////////////////

        /// <summary>
        ///     <para>Invoked whenever a new process instance is started</para>
        /// </summary>
        /// <param name="processInstance"> the newly started process instance </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     <seealso cref="IProcessInstance" /> is started.
        /// </returns>
        AuthorizationEntity[] NewProcessInstance(IProcessInstance processInstance);

        // ITask /////////////////////////////////////////////////

        /// <summary>
        ///     <para>Invoked whenever a new ITask is created</para>
        /// </summary>
        /// <param name="ITask"> the newly created ITask </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     <seealso cref="ITask" /> is created.
        /// </returns>
        AuthorizationEntity[] NewTask(ITask ITask);

        /// <summary>
        ///     <para>Invoked whenever an user has been assigned to a ITask.</para>
        /// </summary>
        /// <param name="ITask"> the ITask on which the assignee has been changed </param>
        /// <param name="oldAssignee"> the old assignee of the ITask </param>
        /// <param name="newAssignee">
        ///     the new assignee of the ITask
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when an
        ///     assignee of a ITask changes.
        /// </returns>
        AuthorizationEntity[] NewTaskAssignee(ITask ITask, string oldAssignee, string newAssignee);

        /// <summary>
        ///     <para>Invoked whenever an user has been set as the owner of a ITask.</para>
        /// </summary>
        /// <param name="ITask"> the ITask on which the owner has been changed </param>
        /// <param name="oldOwner"> the old owner of the ITask </param>
        /// <param name="newOwner">
        ///     the new owner of the ITask
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when the
        ///     owner of a ITask changes.
        /// </returns>
        AuthorizationEntity[] NewTaskOwner(ITask ITask, string oldOwner, string newOwner);

        /// <summary>
        ///     <para>Invoked whenever a new user identity link has been added to a ITask.</para>
        /// </summary>
        /// <param name="ITask"> the ITask on which a new identity link has been added </param>
        /// <param name="userId"> the user for which the identity link has been created </param>
        /// <param name="type">
        ///     the type of the identity link (e.g. <seealso cref="IdentityLinkType#CANDIDATE" />)
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when
        ///     a new user identity link has been added.
        /// </returns>
        AuthorizationEntity[] NewTaskUserIdentityLink(ITask ITask, string userId, string type);

        /// <summary>
        ///     <para>Invoked whenever a new group identity link has been added to a ITask.</para>
        /// </summary>
        /// <param name="ITask"> the ITask on which a new identity link has been added </param>
        /// <param name="groupId"> the group for which the identity link has been created </param>
        /// <param name="type">
        ///     the type of the identity link (e.g. <seealso cref="IdentityLinkType#CANDIDATE" />)
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when
        ///     a new group identity link has been added.
        /// </returns>
        AuthorizationEntity[] NewTaskGroupIdentityLink(ITask ITask, string groupId, string type);

        /// <summary>
        ///     <para>Invoked whenever a user identity link of a ITask has been deleted.</para>
        /// </summary>
        /// <param name="ITask"> the ITask on which the identity link has been deleted </param>
        /// <param name="userId"> the user for which the identity link has been deleted </param>
        /// <param name="type">
        ///     the type of the identity link (e.g. <seealso cref="IdentityLinkType#CANDIDATE" />)
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically deleted when
        ///     a user identity link has been deleted.
        /// </returns>
        AuthorizationEntity[] DeleteTaskUserIdentityLink(ITask ITask, string userId, string type);

        /// <summary>
        ///     <para>Invoked whenever a group identity link of a ITask has been deleted.</para>
        /// </summary>
        /// <param name="ITask"> the ITask on which the identity link has been deleted </param>
        /// <param name="groupId"> the group for which the identity link has been deleted </param>
        /// <param name="type">
        ///     the type of the identity link (e.g. <seealso cref="IdentityLinkType#CANDIDATE" />)
        /// </param>
        /// <returns>
        ///     a list of authorizations to be automatically deleted when
        ///     a group identity link has been deleted.
        /// </returns>
        AuthorizationEntity[] DeleteTaskGroupIdentityLink(ITask ITask, string groupId, string type);

        /// <summary>
        ///     <para>Invoked whenever a new decision definition is created.</para>
        /// </summary>
        /// <param name="decisionDefinition"> the newly created decision definition </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     <seealso cref="IDecisionDefinition" /> is created.
        /// </returns>
        AuthorizationEntity[] NewDecisionDefinition(IDecisionDefinition decisionDefinition);

        /// <summary>
        ///     <para>Invoked whenever a new decision requirements definition is created.</para>
        /// </summary>
        /// <param name="decisionRequirementsDefinition"> the newly created decision requirements definition </param>
        /// <returns>
        ///     a list of authorizations to be automatically added when a new
        ///     <seealso cref="IDecisionRequirementsDefinition" /> is created.
        /// </returns>
        AuthorizationEntity[] NewDecisionRequirementsDefinition(
            IDecisionRequirementsDefinition decisionRequirementsDefinition);
    }
}