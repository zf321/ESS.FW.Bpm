using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;

namespace ESS.FW.Bpm.Engine.Task
{
    /// <summary>
    ///     Exposes twitter-like feeds for tasks and process instances.
    ///     <para>
    ///         <strong>Deprecation</strong>
    ///         This class has been deprecated as of camunda BPM 7.1. It has been replaced with
    ///         the operation log. See <seealso cref="UserOperationLogEntry" /> and <seealso cref="UserOperationLogQuery" />.
    ///     </para>
    /// </summary>
    /// <seealso cref= {@ link TaskService# getTaskEvents( String)
    ///     
    /// </seealso>
    [Obsolete]
    public interface IEvent
    {
        /// <summary>
        ///     A user identity link was added with following message parts:
        ///     [0] userId
        ///     [1] identity link type (aka role)
        /// </summary>
        /// <summary>
        ///     A user identity link was added with following message parts:
        ///     [0] userId
        ///     [1] identity link type (aka role)
        /// </summary>
        /// <summary>
        ///     A group identity link was added with following message parts:
        ///     [0] groupId
        ///     [1] identity link type (aka role)
        /// </summary>
        /// <summary>
        ///     A group identity link was added with following message parts:
        ///     [0] groupId
        ///     [1] identity link type (aka role)
        /// </summary>
        /// <summary>
        ///     An user comment was added with the short version of the comment as message.
        /// </summary>
        /// <summary>
        ///     An attachment was added with the attachment name as message.
        /// </summary>
        /// <summary>
        ///     An attachment was deleted with the attachment name as message.
        /// </summary>
        /// <summary>
        ///     Indicates the type of of action and also indicates the meaning of the parts as exposed in
        ///     <seealso cref="#getMessageParts()" />
        /// </summary>
        string Action { get; }

        /// <summary>
        ///     The meaning of the message parts is defined by the action as you can find in <seealso cref="#getAction()" />
        /// </summary>
        IList<string> MessageParts { get; }

        /// <summary>
        ///     The message that can be used in case this action only has a single message part.
        /// </summary>
        string Message { get; }

        /// <summary>
        ///     reference to the user that made the comment
        /// </summary>
        string UserId { get; }

        /// <summary>
        ///     time and date when the user made the comment
        /// </summary>
        DateTime Time { get; }

        /// <summary>
        ///     reference to the task on which this comment was made
        /// </summary>
        string TaskId { get; }

        /// <summary>
        ///     reference to the process instance on which this comment was made
        /// </summary>
        string ProcessInstanceId { get; }
    }

    public static class EventFields
    {
        public const string ActionAddUserLink = UserOperationLogEntryFields.OperationTypeAddUserLink;
        public const string ActionDeleteUserLink = UserOperationLogEntryFields.OperationTypeDeleteUserLink;
        public const string ActionAddGroupLink = UserOperationLogEntryFields.OperationTypeAddGroupLink;
        public const string ActionDeleteGroupLink = UserOperationLogEntryFields.OperationTypeDeleteGroupLink;
        public const string ActionAddComment = "AddComment";
        public const string ActionAddAttachment = UserOperationLogEntryFields.OperationTypeAddAttachment;
        public const string ActionDeleteAttachment = UserOperationLogEntryFields.OperationTypeDeleteAttachment;
    }
}