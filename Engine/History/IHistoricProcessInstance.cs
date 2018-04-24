using System;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     A single execution of a whole process definition that is stored permanently.
    ///     
    ///     
    /// </summary>
    public interface IHistoricProcessInstance:IHistoryEvent
    {
        /// <summary>
        ///     The process instance id (== as the id for the runtime <seealso cref="IProcessInstance" />).
        /// </summary>
        //string Id { get; }

        /// <summary>
        ///     The user provided unique reference to this process instance.
        /// </summary>
        string BusinessKey { get; }

        /// <summary>
        ///     The process definition key reference.
        /// </summary>
        //string ProcessDefinitionKey { get; }

        /// <summary>
        ///     The process definition reference.
        /// </summary>
        //string ProcessDefinitionId { get; }

        /// <summary>
        ///     The process definition name.
        /// </summary>
       // string ProcessDefinitionName { get; }

        /// <summary>
        ///     The process definition version.
        /// </summary>
        //int? ProcessDefinitionVersion { get; }

        /// <summary>
        ///     The time the process was started.
        /// </summary>
        DateTime? StartTime { get; }

        /// <summary>
        ///     The time the process was ended.
        /// </summary>
        DateTime? EndTime { get; }

        /// <summary>
        ///     The difference between <seealso cref="#getEndTime()" /> and <seealso cref="#getStartTime()" /> .
        /// </summary>
        long? DurationInMillis { get; }

        /// <summary>
        ///     Reference to the activity in which this process instance ended.
        ///     Note that a process instance can have multiple end events, in this case it might not be deterministic
        ///     which activity id will be referenced here. Use a <seealso cref="IHistoricActivityInstanceQuery" /> instead to query
        ///     for end events of the process instance (use the activityTYpe attribute)
        /// </summary>
        [Obsolete]
        string EndActivityId { get; }

        /// <summary>
        ///     The authenticated user that started this process instance.
        /// </summary>
        /// <seealso cref= IdentityService# setAuthenticatedUserId( String
        /// )
        /// </seealso>
        string StartUserId { get; }

        /// <summary>
        ///     The start activity.
        /// </summary>
        string StartActivityId { get; }

        /// <summary>
        ///     Obtains the reason for the process instance's deletion.
        /// </summary>
        string DeleteReason { get; }

        /// <summary>
        ///     The process instance id of a potential super process instance or null if no super process instance exists
        /// </summary>
        string SuperProcessInstanceId { get; }

        /// <summary>
        ///     The case instance id of a potential super case instance or null if no super case instance exists
        /// </summary>
        string SuperCaseInstanceId { get; }

        /// <summary>
        ///     The case instance id of a potential super case instance or null if no super case instance exists
        /// </summary>
        //string CaseInstanceId { get; }

        /// <summary>
        ///     The id of the tenant this historic process instance belongs to. Can be <code>null</code>
        ///     if the historic process instance belongs to no single tenant.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        ///     Return current state of HistoricProcessInstance, following values are recognized during process engine operations:
        ///     STATE_ACTIVE - running process instance
        ///     STATE_SUSPENDED - suspended process instances
        ///     STATE_COMPLETED - completed through normal end event
        ///     STATE_EXTERNALLY_TERMINATED - terminated externally, for instance through REST API
        ///     STATE_INTERNALLY_TERMINATED - terminated internally, for instance by terminating boundary event
        /// </summary>
        string State { get; }
    }

    public static class HistoricProcessInstanceFields
    {
        public const string StateActive = "ACTIVE";
        public const string StateSuspended = "SUSPENDED";
        public const string StateCompleted = "COMPLETED";
        public const string StateExternallyTerminated = "EXTERNALLY_TERMINATED";
        public const string StateInternallyTerminated = "INTERNALLY_TERMINATED";
    }
}