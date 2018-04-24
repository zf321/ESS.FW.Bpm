using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Util;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{
    public class BatchEntity : IBatch, IDbEntity, IHasDbRevision,INameable
    {
        public static readonly BatchSeedJobDeclaration BatchSeedJobDeclaration = new BatchSeedJobDeclaration();

        public static readonly BatchMonitorJobDeclaration BatchMonitorJobDeclaration =
            new BatchMonitorJobDeclaration();

        private readonly bool _instanceFieldsInitialized;

        protected internal JobDefinitionEntity batchJobDefinition;
        
        protected internal IBatchJobHandler batchJobHandler;

        protected internal ByteArrayField configuration;

        // persistent
        protected internal JobDefinitionEntity monitorJobDefinition;

        // transient
        protected internal JobDefinitionEntity seedJobDefinition;



        public BatchEntity()
        {
            if (!_instanceFieldsInitialized)
                _instanceFieldsInitialized = true;
        }

        public virtual string Id { get; set; }

        public virtual string Type { get; set; }


        public virtual int TotalJobs { get; set; }


        public virtual int JobsCreated { get; set; }


        public virtual int BatchJobsPerSeed { get; set; }

        public virtual int InvocationsPerBatchJob { get; set; }


        public virtual string SeedJobDefinitionId { get; set; }


        public virtual string MonitorJobDefinitionId { get; set; }


        public virtual string BatchJobDefinitionId { get; set; }


        public virtual string TenantId { get; set; }

        public virtual bool Suspended
        {
            get { return SuspensionState == SuspensionStateFields.Suspended.StateCode; }
        }

        public virtual int Revision { get; set; }
        public virtual object GetPersistentState()
        {
            var persistentState = new Dictionary<string, object>();
            persistentState["jobsCreated"] = JobsCreated;
            return persistentState;
        }

        public virtual int RevisionNext
        {
            get { return Revision + 1; }
        }

        private void InitializeInstanceFields()
        {
            configuration = new ByteArrayField(this);
        }
        public virtual string Name
        {
            get { return Id; }
        }


        public virtual string Configuration
        {
            get { return configuration.ByteArrayId; }
            set { configuration.ByteArrayId = value; }
        }


        public virtual int SuspensionState { get; set; }= SuspensionStateFields.Active.StateCode;

        // transient

        public virtual JobDefinitionEntity SeedJobDefinition
        {
            get
            {
                if (seedJobDefinition == null && !ReferenceEquals(SeedJobDefinitionId, null))
                {
                    seedJobDefinition = Context.CommandContext.JobDefinitionManager.FindById(SeedJobDefinitionId);
                }

                return seedJobDefinition;
            }
        }

        public virtual JobDefinitionEntity MonitorJobDefinition
        {
            get
            {
                if (monitorJobDefinition == null && !ReferenceEquals(MonitorJobDefinitionId, null))
                {
                    monitorJobDefinition = Context.CommandContext.JobDefinitionManager.FindById(MonitorJobDefinitionId);
                }

                return monitorJobDefinition;
            }
        }

        public virtual JobDefinitionEntity BatchJobDefinition
        {
            get
            {
                if (batchJobDefinition == null && !ReferenceEquals(BatchJobDefinitionId, null))
                {
                    batchJobDefinition = Context.CommandContext.JobDefinitionManager.FindById(BatchJobDefinitionId);
                }

                return batchJobDefinition;
            }
        }

        public virtual byte[] ConfigurationBytes
        {
            get { return configuration.GetByteArrayValue(); }
            set { configuration.SetByteArrayValue(value); }
        }
        
        public virtual IBatchJobHandler BatchJobHandler
        {
            get
            {
                if (batchJobHandler == null)
                {
                    batchJobHandler = Context.CommandContext.ProcessEngineConfiguration.BatchHandlers[Type];
                }

                return batchJobHandler;
            }
        }

        public virtual bool Completed
        {
            get
            {
                return
                    !Context.CommandContext.GetDbEntityManager<JobEntity>().Find/*.ProcessEngineConfiguration.ManagementService.CreateJobQuery*/(c=>c.JobDefinitionId == BatchJobDefinitionId).Any();
            }
        }

        public virtual void DeleteMonitorJob()
        {
            IList<JobEntity> monitorJobs =
                Context.CommandContext.JobManager.FindJobsByJobDefinitionId(MonitorJobDefinitionId);

            foreach (var monitorJob in monitorJobs)
            {
                monitorJob.Delete();
            }
        }

        public virtual void Delete(bool cascadeToHistory)
        {
            CommandContext commandContext = context.Impl.Context.CommandContext;
            throw new NotImplementedException();
            //deleteSeedJob();
            //deleteMonitorJob();
            //BatchJobHandler.deleteJobs(this);

            //JobDefinitionManager jobDefinitionManager = commandContext.JobDefinitionManager;
            //jobDefinitionManager.delete(SeedJobDefinition);
            //jobDefinitionManager.delete(MonitorJobDefinition);
            //jobDefinitionManager.delete(BatchJobDefinition);

            //commandContext.BatchManager.delete(this);
            //configuration.deleteByteArrayValue();

            //fireHistoricEndEvent();

            //if (cascadeToHistory)
            //{
            //    HistoricIncidentManager historicIncidentManager = commandContext.HistoricIncidentManager;
            //    historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(seedJobDefinitionId);
            //    historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(monitorJobDefinitionId);
            //    historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(batchJobDefinitionId);

            //    HistoricJobLogManager historicJobLogManager = commandContext.HistoricJobLogManager;
            //    historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(seedJobDefinitionId);
            //    historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(monitorJobDefinitionId);
            //    historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(batchJobDefinitionId);

            //    commandContext.HistoricBatchManager.deleteHistoricBatchById(id);
            //}
        }

        public virtual void FireHistoricStartEvent()
        {
            Context.CommandContext.HistoricBatchManager.CreateHistoricBatch(this);
        }

        public virtual void FireHistoricEndEvent()
        {
            Context.CommandContext.HistoricBatchManager.CompleteHistoricBatch(this);
        }

        public override string ToString()
        {
            return "BatchEntity{" + "batchHandler=" + batchJobHandler + ", id='" + Id + '\'' + ", type='" + Type + '\'' +
                   ", size=" + TotalJobs + ", jobCreated=" + JobsCreated + ", batchJobsPerSeed=" + BatchJobsPerSeed +
                   ", invocationsPerBatchJob=" + InvocationsPerBatchJob + ", seedJobDefinitionId='" +
                   SeedJobDefinitionId + '\'' + ", monitorJobDefinitionId='" + SeedJobDefinitionId + '\'' +
                   ", batchJobDefinitionId='" + BatchJobDefinitionId + '\'' + ", configurationId='" +
                   configuration.ByteArrayId + '\'' + '}';
        }

       

        public virtual JobDefinitionEntity CreateSeedJobDefinition()
        {
            seedJobDefinition = new JobDefinitionEntity(BatchSeedJobDeclaration);
            seedJobDefinition.JobConfiguration = Id;
            seedJobDefinition.TenantId = TenantId;

            Context.CommandContext.JobDefinitionManager.Add(seedJobDefinition);

            SeedJobDefinitionId = seedJobDefinition.Id;

            return seedJobDefinition;
        }

        public virtual JobDefinitionEntity CreateMonitorJobDefinition()
        {
            monitorJobDefinition = new JobDefinitionEntity(BatchMonitorJobDeclaration);
            monitorJobDefinition.JobConfiguration = Id;
            monitorJobDefinition.TenantId = TenantId;

            Context.CommandContext.JobDefinitionManager.Add(monitorJobDefinition);

            MonitorJobDefinitionId = monitorJobDefinition.Id;

            return monitorJobDefinition;
        }

        public virtual JobDefinitionEntity CreateBatchJobDefinition()
        {
            batchJobDefinition = new JobDefinitionEntity(BatchJobHandler.JobDeclaration);
            batchJobDefinition.JobConfiguration = Id;
            batchJobDefinition.TenantId = TenantId;

            Context.CommandContext.JobDefinitionManager.Add(batchJobDefinition);

            BatchJobDefinitionId = batchJobDefinition.Id;

            return batchJobDefinition;
        }

        public virtual JobEntity CreateSeedJob()
        {
            JobEntity seedJob = BatchSeedJobDeclaration.CreateJobInstance(this);

            Context.CommandContext.JobManager.InsertAndHintJobExecutor(seedJob);

            return seedJob;
        }

        public virtual void DeleteSeedJob()
        {
            IList<JobEntity> seedJobs = Context.CommandContext.JobManager.FindJobsByJobDefinitionId(SeedJobDefinitionId);

            foreach (var job in seedJobs)
            {
                job.Delete();
            }
        }

        public virtual JobEntity CreateMonitorJob(bool setDueDate)
        {
            //Maybe use an other job declaration
            JobEntity monitorJob = BatchMonitorJobDeclaration.CreateJobInstance(this);
            if (setDueDate)
            {
                monitorJob.Duedate = CalculateMonitorJobDueDate();
            }

            Context.CommandContext.JobManager.InsertAndHintJobExecutor(monitorJob);

            return monitorJob;
        }

        protected internal virtual DateTime CalculateMonitorJobDueDate()
        {
            var pollTime = context.Impl.Context.CommandContext.ProcessEngineConfiguration.BatchPollTime;
            var dueTime = ClockUtil.CurrentTime.Ticks + pollTime * 1000;
            return new DateTime(dueTime);
        }
        [NotMapped]////[ForeignKey("BatchJobDefId")]
        public virtual JobDefinitionEntity BatchJobDefinitions { get; set; }
    }
}