using ESS.FW.Bpm.Engine.History.Impl.Event;
using System;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     Base class for all kinds of information that is related to
    ///     either a <seealso cref="HistoricProcessInstance" /> or a <seealso cref="HistoricActivityInstance" />.
    ///      
    /// </summary>
    public interface IHistoricDetail:IHistoryEvent
    {
        /// <summary>
        ///     The unique DB id for this historic detail
        /// </summary>
        //string Id { get; }

        /// <summary>
        ///     The process definition key reference.
        /// </summary>
        //string ProcessDefinitionKey { get; }

        /// <summary>
        ///     The process definition reference.
        /// </summary>
        //string ProcessDefinitionId { get; }

        /// <summary>
        ///     The process instance reference.
        /// </summary>
        //string ProcessInstanceId { get; }

        /// <summary>
        ///     The activity reference in case this detail is related to an activity instance.
        /// </summary>
        string ActivityInstanceId { get; }

        /// <summary>
        ///     The identifier for the path of execution.
        /// </summary>
        //string ExecutionId { get; }

        /// <summary>
        ///     The case definition key reference.
        /// </summary>
        //string CaseDefinitionKey { get; }

        /// <summary>
        ///     The case definition reference.
        /// </summary>
        //string CaseDefinitionId { get; }

        /// <summary>
        ///     The case instance reference.
        /// </summary>
        //string CaseInstanceId { get; }

        /// <summary>
        ///     The case execution reference.
        /// </summary>
        //string CaseExecutionId { get; }

        /// <summary>
        ///     The identifier for the ITask.
        /// </summary>
        string TaskId { get; }

        /// <summary>
        ///     The time when this detail occurred
        /// </summary>
        DateTime Time { get; }

        /// <summary>
        ///     The id of the tenant this historic detail belongs to. Can be <code>null</code>
        ///     if the historic detail belongs to no single tenant.
        /// </summary>
        string TenantId { get; }
        string UserOperationId { get; }
    }
}