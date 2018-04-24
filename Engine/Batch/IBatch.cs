namespace ESS.FW.Bpm.Engine.Batch
{
    /// <summary>
    ///     <para>
    ///         A batch represents a number of jobs which
    ///         execute a number of commands asynchronously.
    ///     </para>
    ///     <para>
    ///     </para>
    ///     <para>
    ///         Batches have three types of jobs:
    ///         <ul>
    ///             <li>
    ///                 Seed jobs: Create execution jobs
    ///                 <li>
    ///                     Execution jobs: Execute the actual action
    ///                     <li>
    ///                         Monitor jobs: Manage the batch once all execution jobs have been created
    ///                         (e.g. responsible for deletion of the Batch after completion).
    ///         </ul>
    ///     </para>
    ///     <para>
    ///     </para>
    ///     <para>
    ///         All three job types have independent job definitions. They can be controlled individually
    ///         (e.g. suspension) and are independently represented in the historic job log.
    ///     </para>
    /// </summary>
    public interface IBatch
    {
        /// <returns> the id of the batch </returns>
        string Id { get; }

        /// <returns> the type of the batch </returns>
        string Type { get; }

        /// <returns> the number of batch execution jobs required to complete the batch </returns>
        int TotalJobs { get; }

        /// <returns> the number of batch execution jobs already created by the seed job </returns>
        int JobsCreated { get; }

        /// <returns> number of batch jobs created per batch seed job invocation </returns>
        int BatchJobsPerSeed { get; }

        /// <returns> the number of invocations executed per batch job </returns>
        int InvocationsPerBatchJob { get; }

        /// <returns> the id of the batch seed job definition </returns>
        string SeedJobDefinitionId { get; }

        /// <returns> the id of the batch monitor job definition </returns>
        string MonitorJobDefinitionId { get; }

        /// <returns> the id of the batch job definition </returns>
        string BatchJobDefinitionId { get; }

        /// <returns> the batch's tenant id or null </returns>
        string TenantId { get; }

        /// <summary>
        ///     <para>
        ///         Indicates whether this batch is suspended. If a batch is suspended,
        ///         the batch jobs will not be acquired by the job executor.
        ///     </para>
        ///     <para>
        ///     </para>
        ///     <para>
        ///         <strong>Note:</strong> It is still possible to manually suspend and activate
        ///         jobs and job definitions using the <seealso cref="IManagementService" />, which will
        ///         not change the suspension state of the batch.
        ///     </para>
        /// </summary>
        /// <returns> true if this batch is currently suspended, false otherwise </returns>
        /// <seealso cref= ManagementService# suspendBatchById( String
        /// )
        /// </seealso>
        /// <seealso cref= ManagementService# activateBatchById( String
        /// )
        /// </seealso>
        bool Suspended { get; }
    }

    public static class BatchFields
    {

        public const string TypeProcessInstanceMigration = "instance-migration";
        public const string TypeProcessInstanceModification = "instance-modification";
        public const string TypeProcessInstanceRestart = "instance-restart";
        public const string TypeProcessInstanceDeletion = "instance-deletion";
        public const string TypeHistoricProcessInstanceDeletion = "historic-instance-deletion";
        public const string TypeSetJobRetries = "set-job-retries";
        public const string TypeSetExternalTaskRetries = "set-external-task-retries";
    }
}