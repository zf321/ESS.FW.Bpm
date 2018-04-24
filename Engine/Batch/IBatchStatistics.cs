namespace ESS.FW.Bpm.Engine.Batch
{
    /// <summary>
    ///     <para>
    ///         Additional statistics for a single batch.
    ///     </para>
    ///     <para>
    ///         Contains the number of remaining jobs, completed and failed batch
    ///         execution jobs. The following relation between these exists:
    ///         <code>
    ///     batch total jobs = remaining jobs + completed jobs
    ///   </code>
    ///     </para>
    /// </summary>
    public interface IBatchStatistics : IBatch
    {
        /// <summary>
        ///     <para>
        ///         The number of remaining batch execution jobs.
        ///         This does include failed batch execution jobs and
        ///         batch execution jobs which still have to be created by the seed job.
        ///     </para>
        ///     <para>
        ///         See
        ///         <seealso cref="#getTotalJobs()" /> for the number of all batch execution jobs,
        ///         <seealso cref="#getCompletedJobs()" /> for the number of completed batch execution jobs and
        ///         <seealso cref="#getFailedJobs()" /> for the number of failed batch execution jobs.
        ///     </para>
        /// </summary>
        /// <returns> the number of remaining batch execution jobs </returns>
        int RemainingJobs { get; }

        /// <summary>
        ///     <para>
        ///         The number of completed batch execution jobs.
        ///         This does include aborted/deleted batch execution jobs.
        ///     </para>
        ///     <para>
        ///         See
        ///         <seealso cref="#getTotalJobs()" /> for the number of all batch execution jobs,
        ///         <seealso cref="#getRemainingJobs()" /> ()} for the number of remaining batch execution jobs and
        ///         <seealso cref="#getFailedJobs()" /> for the number of failed batch execution jobs.
        ///     </para>
        /// </summary>
        /// <returns> the number of completed batch execution jobs </returns>
        int CompletedJobs { get; }

        /// <summary>
        ///     <para>
        ///         The number of failed batch execution jobs.
        ///         This does not include aborted or deleted batch execution jobs.
        ///     </para>
        ///     <para>
        ///         See
        ///         <seealso cref="#getTotalJobs()" /> for the number of all batch execution jobs,
        ///         <seealso cref="#getRemainingJobs()" /> ()} for the number of remaining batch execution jobs and
        ///         <seealso cref="#getCompletedJobs()" /> ()} for the number of completed batch execution jobs.
        ///     </para>
        /// </summary>
        /// <returns> the number of failed batch execution jobs </returns>
        int FailedJobs { get; }
    }
}