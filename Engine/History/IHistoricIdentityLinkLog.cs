using System;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     An historic identity link stores the association of a ITask with a certain identity.
    ///     For example, historic identity link is logged on the following conditions:
    ///     - a user can be an assignee/Candidate/Owner (= identity link type) for a ITask
    ///     - a group can be a candidate-group (= identity link type) for a ITask
    ///     - a user can be an candidate in the scope of process definition
    ///     - a group can be a candidate-group in the scope of process definition
    ///     For every log, an operation type (add/delete) is added to the database
    ///     based on the identity link operation
    /// </summary>
    public interface IHistoricIdentityLinkLog
    {
        /// <summary>
        ///     Returns the id of historic identity link (Candidate or Assignee or Owner).
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Returns the type of link (Candidate or Assignee or Owner).
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
        ///     The id of the ITask associated with this identity link.
        /// </summary>
        string TaskId { get; }

        /// <summary>
        ///     Returns the userId of the user who assigns a ITask to the user
        /// </summary>
        string AssignerId { get; }

        /// <summary>
        ///     Returns the type of identity link history (add or delete identity link)
        /// </summary>
        string OperationType { get; }

        /// <summary>
        ///     Returns the time of identity link event (Creation/Deletion)
        /// </summary>
        DateTime Time { get; }

        /// <summary>
        ///     Returns the id of the related process definition
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     Returns the key of the related process definition
        /// </summary>
        string ProcessDefinitionKey { get; }

        /// <summary>
        ///     Returns the id of the related tenant
        /// </summary>
        string TenantId { get; }
    }
}