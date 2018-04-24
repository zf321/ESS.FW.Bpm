using System;

namespace ESS.FW.Bpm.Engine.Externaltask
{
    /// <summary>
    ///     Represents an instance of an external ITask that is created when
    ///     a service-ITask like activity (i.e. service ITask, send ITask, ...) with
    ///     attribute <code>camunda:type="external"</code> is executed.
    ///     
    /// </summary>
    public interface IExternalTask
    {
        /// <returns> the id of the ITask </returns>
        string Id { get; }

        /// <returns> the name of the topic the ITask belongs to </returns>
        string TopicName { get; }

        /// <returns> the id of the worker that has locked the ITask </returns>
        string WorkerId { get; }

        /// <returns> the absolute time at which the lock expires </returns>
        DateTime LockExpirationTime { get; }

        /// <returns> the id of the process instance the ITask exists in </returns>
        string ProcessInstanceId { get; }

        /// <returns> the id of the execution that the ITask is assigned to </returns>
        string ExecutionId { get; }

        /// <returns> the id of the activity for which the ITask is created </returns>
        string ActivityId { get; }

        /// <returns> the id of the activity instance in which context the ITask exists </returns>
        string ActivityInstanceId { get; }

        /// <returns> the id of the process definition the ITask's activity belongs to </returns>
        string ProcessDefinitionId { get; }

        /// <returns> the key of the process definition the ITask's activity belongs to </returns>
        string ProcessDefinitionKey { get; }

        /// <returns>
        ///     the number of retries left. The number of retries is provided by
        ///     a ITask client, therefore the initial value is <code>null</code>.
        /// </returns>
        int? Retries { get; }

        /// <returns>
        ///     short error message submitted with the latest reported failure executing this ITask;
        ///     <code>null</code> if no failure was reported previously or if no error message
        ///     was submitted
        /// </returns>
        /// <seealso cref= ExternalTaskService# handleFailure( String, String, String, String, int, long)
        ///     To get the full error details,
        ///     use
        /// <seealso cref="ExternalTaskService#getExternalTaskErrorDetails(String)" />
        /// </seealso>
        string ErrorMessage { get; }

        /// <returns>
        ///     true if the external ITask is suspended; a suspended external ITask
        ///     cannot be completed, thereby preventing process continuation
        /// </returns>
        bool Suspended { get; }

        /// <returns>
        ///     the id of the tenant the ITask belongs to. Can be <code>null</code>
        ///     if the ITask belongs to no single tenant.
        /// </returns>
        string TenantId { get; }

        /// <summary>
        ///     Returns the priority of the external ITask.
        /// </summary>
        /// <returns> the priority of the external ITask </returns>
        long Priority { get; }
    }
}