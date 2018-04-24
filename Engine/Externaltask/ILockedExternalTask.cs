using System;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Externaltask
{
    /// <summary>
    ///     Represents an external task that was fetched and locked.
    ///     
    /// </summary>
    public interface ILockedExternalTask
    {
        /// <returns> the id of the task </returns>
        string Id { get; }

        /// <returns> the name of the topic the task belongs to </returns>
        string TopicName { get; }

        /// <returns> the id of the worker that has locked the task </returns>
        string WorkerId { get; }

        /// <returns> the absolute time at which the lock expires </returns>
        DateTime LockExpirationTime { get; }

        /// <returns> the id of the process instance the task exists in </returns>
        string ProcessInstanceId { get; }

        /// <returns> the id of the execution that the task is assigned to </returns>
        string ExecutionId { get; }

        /// <returns> the id of the activity for which the task is created </returns>
        string ActivityId { get; }

        /// <returns> the id of the activity instance in which context the task exists </returns>
        string ActivityInstanceId { get; }

        /// <returns> the id of the process definition the task's activity belongs to </returns>
        string ProcessDefinitionId { get; }

        /// <returns> the key of the process definition the task's activity belongs to </returns>
        string ProcessDefinitionKey { get; }

        /// <returns>
        ///     the number of retries left. The number of retries is provided by
        ///     a task client, therefore the initial value is <code>null</code>.
        /// </returns>
        int? Retries { get; }

        /// <returns>
        ///     the full error message submitted with the latest reported failure executing this task;
        ///     <code>null</code> if no failure was reported previously or if no error message
        ///     was submitted
        /// </returns>
        /// <seealso cref= ExternalTaskService# handleFailure( String, String, String, int, long
        /// )
        /// </seealso>
        string ErrorMessage { get; }

        /// <returns>
        ///     error details submitted with the latest reported failure executing this task;
        ///     <code>null</code> if no failure was reported previously or if no error details
        ///     was submitted
        /// </returns>
        /// <seealso cref= ExternalTaskService# handleFailure( String, String, String, String, int, long
        /// )
        /// </seealso>
        string ErrorDetails { get; }

        /// <returns>
        ///     a map of variables that contains an entry for every variable
        ///     that was specified at fetching time, if such a variable exists in the task's
        ///     ancestor execution hierarchy.
        /// </returns>
        IVariableMap Variables { get; }

        /// <returns>
        ///     the id of the tenant the task belongs to. Can be <code>null</code>
        ///     if the task belongs to no single tenant.
        /// </returns>
        string TenantId { get; }

        /// <summary>
        ///     Returns the priority of the locked external task.
        ///     The default priority is 0.
        /// </summary>
        /// <returns> the priority of the external task </returns>
        long Priority { get; }
    }
}