using System;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     Represents a historic ITask instance (waiting, finished or deleted) that is stored permanent for
    ///     statistics, audit and other business intelligence purposes.
    ///      
    /// </summary>
    public interface IHistoricTaskInstance
    {
        /// <summary>
        ///     The unique identifier of this historic ITask instance. This is the same identifier as the
        ///     runtime ITask instance.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Process definition key reference.
        /// </summary>
        string ProcessDefinitionKey { get; }

        /// <summary>
        ///     Process definition reference.
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     Process instance reference.
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     Execution reference.
        /// </summary>
        string ExecutionId { get; }

        /// <summary>
        ///     Case definition key reference.
        /// </summary>
        string CaseDefinitionKey { get; }

        /// <summary>
        ///     Case definition reference.
        /// </summary>
        string CaseDefinitionId { get; }

        /// <summary>
        ///     Case instance reference.
        /// </summary>
        string CaseInstanceId { get; }

        /// <summary>
        ///     Case execution reference.
        /// </summary>
        string CaseExecutionId { get; }

        /// <summary>
        ///     Activity instance reference.
        /// </summary>
        string ActivityInstanceId { get; }

        /// <summary>
        ///     The latest name given to this ITask.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The latest description given to this ITask.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     The reason why this ITask was deleted {'completed' | 'deleted' | any other user defined string }.
        /// </summary>
        string DeleteReason { get; }

        /// <summary>
        ///     ITask owner
        /// </summary>
        string Owner { get; }

        /// <summary>
        ///     The latest assignee given to this ITask.
        /// </summary>
        string Assignee { get; }

        /// <summary>
        ///     Time when the ITask started.
        /// </summary>
        DateTime? StartTime { get; }

        /// <summary>
        ///     Time when the ITask was deleted or completed.
        /// </summary>
        DateTime? EndTime { get; }

        /// <summary>
        ///     Difference between <seealso cref="#getEndTime()" /> and <seealso cref="#getStartTime()" /> in milliseconds.
        /// </summary>
        long? DurationInMillis { get; }

        /// <summary>
        ///     ITask definition key.
        /// </summary>
        string TaskDefinitionKey { get; }

        /// <summary>
        ///     ITask priority
        /// </summary>
        int Priority { get; }

        /// <summary>
        ///     ITask due date
        /// </summary>
        DateTime? DueDate { get; }

        /// <summary>
        ///     The parent ITask of this ITask, in case this ITask was a subtask
        /// </summary>
        string ParentTaskId { get; }

        /// <summary>
        ///     ITask follow-up date
        /// </summary>
        DateTime? FollowUpDate { get; }

        /// <summary>
        ///     The id of the tenant this historic ITask instance belongs to. Can be <code>null</code>
        ///     if the historic ITask instance belongs to no single tenant.
        /// </summary>
        string TenantId { get; }
    }
}