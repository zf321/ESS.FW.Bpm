namespace ESS.FW.Bpm.Engine.Batch.Impl
{
    public class BatchStatisticsEntity : BatchEntity, IBatchStatistics
    {
        protected internal int failedJobs;

        protected internal int remainingJobs;


        public virtual int JobsToCreate
        {
            get { return TotalJobs - JobsCreated; }
        }

        public virtual int RemainingJobs
        {
            get { return remainingJobs + JobsToCreate; }
            set { remainingJobs = value; }
        }


        public virtual int CompletedJobs
        {
            get { return TotalJobs - RemainingJobs; }
        }

        public virtual int FailedJobs
        {
            get { return failedJobs; }
            set { failedJobs = value; }
        }

        public override string ToString()
        {
            return "BatchStatisticsEntity{" + "batchHandler=" + BatchJobHandler + ", id='" + Id + '\'' + ", type='" +
                   Type + '\'' + ", size=" + TotalJobs + ", jobCreated=" + JobsCreated + ", remainingJobs=" +
                   remainingJobs + ", failedJobs=" + failedJobs + ", batchJobsPerSeed=" + BatchJobsPerSeed +
                   ", invocationsPerBatchJob=" + InvocationsPerBatchJob + ", seedJobDefinitionId='" +
                   SeedJobDefinitionId + '\'' + ", monitorJobDefinitionId='" + SeedJobDefinitionId + '\'' +
                   ", batchJobDefinitionId='" + BatchJobDefinitionId + '\'' + ", configurationId='";
            // configuration.ByteArrayId + '\'' + '}';
        }
    }
}