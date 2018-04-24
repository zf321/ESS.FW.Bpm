namespace ESS.FW.Bpm.Engine.Task
{
    /// <summary>
    ///     An identity link is used to associate a task with a certain identity.
    ///     For example:
    ///     - a user can be an assignee (= identity link type) for a task
    ///     - a group can be a candidate-group (= identity link type) for a task
    ///     
    /// </summary>
    public interface IIdentityLink
    {
        /// <summary>
        ///     Get the Id of identityLink
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Returns the type of link.
        ///     See <seealso cref="IdentityLinkType" /> for the native supported types by the process engine.
        /// </summary>
        string Type { get; }

        /// <summary>
        ///     If the identity link involves a user, then this will be a non-null id of a user.
        ///     That userId can be used to query for user information through the <seealso cref="UserQuery" /> API.
        /// </summary>
        string UserId { get; }

        /// <summary>
        ///     If the identity link involves a group, then this will be a non-null id of a group.
        ///     That groupId can be used to query for user information through the <seealso cref="GroupQuery" /> API.
        /// </summary>
        string GroupId { get; }

        /// <summary>
        ///     The id of the task associated with this identity link.
        /// </summary>
        string TaskId { get; }

        /// <summary>
        ///     Get the process definition id
        /// </summary>
        string ProcessDefId { get; }

        /// <summary>
        ///     The id of the tenant associated with this identity link.
        ///     
        /// </summary>
        string TenantId { get; }
    }
}