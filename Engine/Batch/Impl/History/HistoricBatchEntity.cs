using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Batch.Impl.History
{
    [Serializable]
    public class HistoricBatchEntity : HistoryEvent, IHistoricBatch, IDbEntity
    {
        
        protected internal string batchJobDefinitionId;
        protected internal int batchJobsPerSeed;
        protected internal DateTime endTime;

        protected internal new string id;
        protected internal int invocationsPerBatchJob;
        protected internal string monitorJobDefinitionId;

        protected internal string seedJobDefinitionId;

        protected internal DateTime startTime;

        protected internal string tenantId;

        protected internal int totalJobs;
        protected internal string type;

        public virtual string Type
        {
            get { return type; }
            set { type = value; }
        }


        public virtual int TotalJobs
        {
            get { return totalJobs; }
            set { totalJobs = value; }
        }


        public virtual int BatchJobsPerSeed
        {
            get { return batchJobsPerSeed; }
            set { batchJobsPerSeed = value; }
        }


        public virtual int InvocationsPerBatchJob
        {
            get { return invocationsPerBatchJob; }
            set { invocationsPerBatchJob = value; }
        }


        public virtual string SeedJobDefinitionId
        {
            get { return seedJobDefinitionId; }
            set { seedJobDefinitionId = value; }
        }


        public virtual string MonitorJobDefinitionId
        {
            get { return monitorJobDefinitionId; }
            set { monitorJobDefinitionId = value; }
        }


        public virtual string BatchJobDefinitionId
        {
            get { return batchJobDefinitionId; }
            set { batchJobDefinitionId = value; }
        }


        public virtual string TenantId
        {
            get { return tenantId; }
            set { tenantId = value; }
        }


        public virtual DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }


        public virtual DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }


        public override object GetPersistentState()
        {
                IDictionary<string, object> persistentState = new Dictionary<string, object>();

                persistentState["endTime"] = endTime;

                return persistentState;
        }

        public virtual void Delete()
        {
            //HistoricIncidentManager historicIncidentManager = Context.CommandContext.HistoricIncidentManager;
            //historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(seedJobDefinitionId);
            //historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(monitorJobDefinitionId);
            //historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(batchJobDefinitionId);

            //HistoricJobLogManager historicJobLogManager = Context.CommandContext.HistoricJobLogManager;
            //historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(seedJobDefinitionId);
            //historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(monitorJobDefinitionId);
            //historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(batchJobDefinitionId);

            //Context.CommandContext.HistoricBatchManager.delete(this);
        }
    }
}