using ESS.FW.Bpm.Engine.Impl.JobExecutor;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{

    /// <summary>
    ///     A batch job handler manages batch jobs based
    ///     on the configuration <seealso cref="T" />.
    ///     Used by a seed job to manage lifecycle of execution jobs.
    /// </summary>
    public interface IBatchJobHandler : IJobHandler
    {
        /// <summary>
        ///     Converts the configuration of the batch to a byte array.
        /// </summary>
        /// <param name="configuration"> the configuration object </param>
        /// <returns> the serialized configuration </returns>
        byte[] WriteConfiguration(IJobHandlerConfiguration configuration);

        /// <summary>
        ///     Read the serialized configuration of the batch.
        /// </summary>
        /// <param name="serializedConfiguration"> the serialized configuration </param>
        /// <returns> the deserialized configuration object </returns>
        IJobHandlerConfiguration ReadConfiguration(byte[] serializedConfiguration);


        /**
   * Get the job declaration for batch jobs.
   *
   * @return the batch job declaration
   */
        IJobDeclaration JobDeclaration { get; }
        /// <summary>
        ///     Get the job declaration for batch jobs.
        /// </summary>
        /// <returns> the batch job declaration </returns>
        /// <summary>
        ///     Creates batch jobs for a batch.
        /// </summary>
        /// <param name="batch"> the batch to create jobs for </param>
        /// <returns> true of no more jobs have to be created for this batch, false otherwise </returns>
        bool CreateJobs(BatchEntity batch);

        /// <summary>
        ///     Delete all jobs for a batch.
        /// </summary>
        /// <param name="batch"> the batch to delete jobs for </param>
        void DeleteJobs(BatchEntity batch);
    }
}