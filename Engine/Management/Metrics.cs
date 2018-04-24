namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     
    /// </summary>
    public class Metrics
    {
        public const string ActivtyInstanceStart = "activity-instance-start";
        public const string ActivtyInstanceEnd = "activity-instance-end";

        /// <summary>
        ///     decimal of times job acqusition is performed
        /// </summary>
        public const string JobAcquisitionAttempt = "job-acquisition-attempt";

        /// <summary>
        ///     decimal of jobs successfully acquired (i.e. selected + locked)
        /// </summary>
        public const string JobAcquiredSuccess = "job-acquired-success";

        /// <summary>
        ///     decimal of jobs attempted to acquire but with failure (i.e. selected + lock failed)
        /// </summary>
        public const string JobAcquiredFailure = "job-acquired-failure";

        /// <summary>
        ///     decimal of jobs that were submitted for execution but were rejected due to
        ///     resource shortage. In the default job executor, this is the case when
        ///     the execution queue is full.
        /// </summary>
        public const string JobExecutionRejected = "job-execution-rejected";

        public const string JobSuccessful = "job-successful";
        public const string JobFailed = "job-failed";

        /// <summary>
        ///     decimal of jobs that are immediately locked and executed because they are exclusive
        ///     and created in the context of job execution
        /// </summary>
        public const string JobLockedExclusive = "job-locked-exclusive";

        /// <summary>
        ///     decimal of executed decision elements in the DMN engine.
        /// </summary>
        public const string ExecutedDecisionElements = "executed-decision-elements";
    }
}