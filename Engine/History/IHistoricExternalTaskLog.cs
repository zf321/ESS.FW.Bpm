using System;
using ESS.FW.Bpm.Engine.Externaltask;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     <para>
    ///         The <seealso cref="IHistoricExternalTaskLog" /> is used to have a log containing
    ///         information about <seealso cref="IExternalTask" /> execution. The log provides
    ///         details about the last lifecycle state of a <seealso cref="IExternalTask" />:
    ///     </para>
    ///     An instance of <seealso cref="IHistoricExternalTaskLog" /> represents the latest historic
    ///     state in the lifecycle of a <seealso cref="IExternalTask" />.
    ///     
    /// </summary>
    public interface IHistoricExternalTaskLog
    {
        /// <summary>
        ///     Returns the unique identifier for <code>this</code> historic external ITask log.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Returns the time when <code>this</code> log occurred.
        /// </summary>
        DateTime TimeStamp { get; }

        /// <summary>
        ///     Returns the id of the associated external ITask.
        /// </summary>
        string ExternalTaskId { get; }

        /// <summary>
        ///     Returns the retries of the associated external ITask before the associated external ITask has
        ///     been executed and when <code>this</code> log occurred.
        /// </summary>
        int? Retries { get; }

        /// <summary>
        ///     Returns the priority of the associated external ITask when <code>this</code> log entry was created.
        /// </summary>
        long Priority { get; }

        /// <summary>
        ///     Returns the topic name of the associated external ITask.
        /// </summary>
        string TopicName { get; }

        /// <summary>
        ///     Returns the id of the worker that fetched the external ITask most recently.
        /// </summary>
        string WorkerId { get; }

        /// <summary>
        ///     Returns the message of the error that occurred by executing the associated external ITask.
        ///     To get the full error details,
        ///     use <seealso cref="IHistoryService#getHistoricExternalTaskLogErrorDetails(String)" />
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        ///     Returns the id of the activity which the external ITask associated with.
        /// </summary>
        string ActivityId { get; }

        /// <summary>
        ///     Returns the id of the activity instance on which the associated external ITask was created.
        /// </summary>
        string ActivityInstanceId { get; }

        /// <summary>
        ///     Returns the specific execution id on which the associated external ITask was created.
        /// </summary>
        string ExecutionId { get; }

        /// <summary>
        ///     Returns the specific process instance id on which the associated external ITask was created.
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     Returns the specific process definition id on which the associated external ITask was created.
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     Returns the specific process definition key on which the associated external ITask was created.
        /// </summary>
        string ProcessDefinitionKey { get; }

        /// <summary>
        ///     Returns the id of the tenant this external ITask log entry belongs to. Can be <code>null</code>
        ///     if the external ITask log entry belongs to no single tenant.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        ///     Returns <code>true</code> when <code>this</code> log represents
        ///     the creation of the associated external ITask.
        /// </summary>
        bool CreationLog { get; }

        /// <summary>
        ///     Returns <code>true</code> when <code>this</code> log represents
        ///     the failed execution of the associated external ITask.
        /// </summary>
        bool FailureLog { get; }

        /// <summary>
        ///     Returns <code>true</code> when <code>this</code> log represents
        ///     the successful execution of the associated external ITask.
        /// </summary>
        bool SuccessLog { get; }

        /// <summary>
        ///     Returns <code>true</code> when <code>this</code> log represents
        ///     the deletion of the associated external ITask.
        /// </summary>
        bool DeletionLog { get; }
    }
}