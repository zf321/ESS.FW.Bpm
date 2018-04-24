using System;

namespace ESS.FW.Bpm.Engine.Batch.History
{
    /// <summary>
    ///     Historic representation of a <seealso cref="IBatch" />.
    /// </summary>
    public interface IHistoricBatch
    {
        /// <returns> the id of the batch </returns>
        string Id { get; }

        /// <returns> the type of the batch </returns>
        string Type { get; }

        /// <returns> the number of batch execution jobs required to complete the batch </returns>
        int TotalJobs { get; }

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

        /// <returns> the date the batch was started </returns>
        DateTime StartTime { get; }

        /// <returns> the date the batch was completed </returns>
        DateTime EndTime { get; }
    }
}