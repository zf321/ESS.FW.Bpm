

using System;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     Represents one execution of an activity and it's stored permanent for statistics, audit and other business
    ///     intelligence purposes.
    ///     
    /// </summary>
    public interface IHistoricActivityInstance
    {
        /// <summary>
        ///     The unique identifier of this historic activity instance.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     return the id of the parent activity instance
        /// </summary>
        string ParentActivityInstanceId { get; }

        /// <summary>
        ///     The unique identifier of the activity in the process
        /// </summary>
        string ActivityId { get; }

        /// <summary>
        ///     The display name for the activity
        /// </summary>
        string ActivityName { get; }

        /// <summary>
        ///     The activity type of the activity.
        ///     Typically the activity type correspond to the XML tag used in the BPMN 2.0 process definition file.
        ///     All activity types are available in <seealso cref="ActivityTypes" />
        /// </summary>
        /// <seealso cref= org.camunda.bpm.engine.ActivityTypes
        /// </seealso>
        string ActivityType { get; }

        /// <summary>
        ///     Process definition key reference
        /// </summary>
        string ProcessDefinitionKey { get; }

        /// <summary>
        ///     Process definition reference
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     Process instance reference
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     Execution reference
        /// </summary>
        string ExecutionId { get; }

        /// <summary>
        ///     The corresponding ITask in case of ITask activity
        /// </summary>
        string TaskId { get; }

        /// <summary>
        ///     The called process instance in case of call activity
        /// </summary>
        string CalledProcessInstanceId { get; }

        /// <summary>
        ///     The called case instance in case of (case) call activity
        /// </summary>
        string CalledCaseInstanceId { get; }

        /// <summary>
        ///     Assignee in case of user ITask activity
        /// </summary>
        string Assignee { get; }

        /// <summary>
        ///     Time when the activity instance started
        /// </summary>
        DateTime? StartTime { get; }

        /// <summary>
        ///     Time when the activity instance ended
        /// </summary>
        DateTime? EndTime { get; }

        /// <summary>
        ///     Difference between <seealso cref="#getEndTime()" /> and <seealso cref="#getStartTime()" />.
        /// </summary>
        long? DurationInMillis { get; }

        /// <summary>
        ///     Did this activity instance complete a BPMN 2.0 scope
        /// </summary>
        bool CompleteScope { get; }

        /// <summary>
        ///     Was this activity instance canceled
        /// </summary>
        bool Canceled { get; }

        /// <summary>
        ///     The id of the tenant this historic activity instance belongs to. Can be <code>null</code>
        ///     if the historic activity instance belongs to no single tenant.
        /// </summary>
        string TenantId { get; }
        int ActivityInstanceState { get; set; }
        string TaskAssignee { get; set; }
        string ActivityInstanceId { get; set; }
    }
}