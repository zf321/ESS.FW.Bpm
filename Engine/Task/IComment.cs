using System;

namespace ESS.FW.Bpm.Engine.Task
{
    /// <summary>
    ///     User comments that form discussions around tasks.
    /// </summary>
    /// <seealso cref= {@ link TaskService# getTaskComments( String)
    ///     
    /// </seealso>
    public interface IComment
    {
        /// <summary>
        ///     comment id
        /// </summary>
        string Id { get; }

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

        /// <summary>
        ///     the full comment message the user had related to the task and/or process instance
        /// </summary>
        /// <seealso cref= TaskService# getTaskComments( String
        /// )
        /// </seealso>
        string FullMessage { get; }
    }
}