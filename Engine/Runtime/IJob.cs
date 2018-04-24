using System;
using ESS.FW.Bpm.Engine.Management;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     Represents one job (timer, message, etc.).
    ///     
    ///     
    /// </summary>
    public interface IJob
    {
        /// <summary>
        ///     Returns the unique identifier for this job.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Returns the date on which this job is supposed to be processed.
        /// </summary>
        DateTime? Duedate { get; }

        /// <summary>
        ///     Returns the id of the process instance which execution created the job.
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     Returns the id of the process definition which created the job.
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     Returns the key of the process definition which created the job.
        /// </summary>
        string ProcessDefinitionKey { get; }

        /// <summary>
        ///     Returns the specific execution on which the job was created.
        /// </summary>
        string ExecutionId { get; }

        /// <summary>
        ///     Returns the number of retries this job has left.
        ///     Whenever the jobexecutor fails to execute the job, this value is decremented.
        ///     When it hits zero, the job is supposed to be dead and not retried again
        ///     (ie a manual retry is required then).
        /// </summary>
        int Retries { get; }

        /// <summary>
        ///     Returns the message of the exception that occurred, the last time the job was
        ///     executed. Returns null when no exception occurred.
        ///     To get the full exception stacktrace,
        ///     use <seealso cref="IManagementService#getJobExceptionStacktrace(String)" />
        /// </summary>
        string ExceptionMessage { get; }

        /// <summary>
        ///     Returns the id of the deployment in which context the job was created.
        /// </summary>
        string DeploymentId { get; }

        /// <summary>
        ///     The id of the <seealso cref="IJobDefinition" /> for this job.
        /// </summary>
        string JobDefinitionId { get; }

        /// <summary>
        ///     Indicates whether this job is suspended. If a job is suspended,
        ///     the job will be not acquired by the job executor.
        /// </summary>
        /// <returns> true if this Job is currently suspended. </returns>
        bool Suspended { get; }

        /// <summary>
        ///     The job's priority that is a hint to job acquisition.
        ///     
        /// </summary>
        long Priority { get; }

        /// <summary>
        ///     The id of the tenant this job belongs to. Can be <code>null</code>
        ///     if the job belongs to no single tenant.
        /// </summary>
        string TenantId { get; }
        int SuspensionState { get; set; }
    }
}