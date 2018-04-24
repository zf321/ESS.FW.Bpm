using System;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Batch.Impl;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    // 
    /// <summary>
    /// 
    /// 
    /// </summary>
    [Serializable]
    public class JobDefinitionEntity : IJobDefinition, IHasDbRevision, IDbEntity
    {

        public JobDefinitionEntity()
        {
        }

        public JobDefinitionEntity(IJobDeclaration jobDeclaration)
        {
            ActivityId = jobDeclaration.ActivityId;
            JobConfiguration = jobDeclaration.JobConfiguration;
            JobType = jobDeclaration.JobHandlerType;
        }

        public static JobDefinitionEntity CreateJobDefinitionEntity(IJobDeclaration jobDeclaration)
        {
            JobDefinitionEntity t = new JobDefinitionEntity();
            t.ActivityId = jobDeclaration.ActivityId;
            t.JobConfiguration = jobDeclaration.JobConfiguration;
            t.JobType = jobDeclaration.JobHandlerType;
            return t;
        }

        public virtual object GetPersistentState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            state["processDefinitionId"] = ProcessDefinitionId;
            state["processDefinitionKey"] = ProcessDefinitionKey;
            state["activityId"] = ActivityId;
            state["jobType"] = JobType;
            state["jobConfiguration"] = JobConfiguration;
            state["suspensionState"] = SuspensionState;
            state["jobPriority"] = JobPriority;
            state["tenantId"] = TenantId;
            return state;
        }

        // // getters / setters /////////////////////////////////
        public string Id { get; set; }
        public virtual int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }
        public virtual int Revision { get; set; }


        public virtual bool Suspended
        {
            get
            {
                return SuspensionStateFields.Suspended.StateCode == SuspensionState;
            }
        }

        public  string ProcessDefinitionId { get; set; }

        /* Note: this is the id of the activity which is the cause that a Job is created.
         * If the Job corresponds to an event scope, it may or may not correspond to the
         * activity which defines the event scope.
         *
         * Example:
         * user task with attached timer event:
         * - timer event scope = user task
         * - activity which causes the job to be created = timer event.
         * => Job definition activityId will be activityId of the timer event, not the activityId of the user task.
         */
        public  string ActivityId { get; set; }

        /// <summary>
        /// timer, message, ... </summary>
        public string JobType { get; set; }


        public string JobConfiguration { get; set; }


        public  string ProcessDefinitionKey { get; set; }


        public virtual int SuspensionState { get; set; }= SuspensionStateFields.Active.StateCode;


        public virtual long? OverridingJobPriority
        {
            get
            {
                return JobPriority;
            }
        }
        // job definition is active by default

        public long? JobPriority { get; set; }

        public virtual string TenantId { get; set; }

        //public virtual ICollection<BatchEntity> JobDefinitionBatchs { get; set; }
        //public virtual ICollection<BatchEntity> MonitorJobDefBatchs { get; set; }
        //public virtual ICollection<BatchEntity> SeedJobDefBatch { get; set; }
        //public virtual ICollection<IncidentEntity> Incidents { get; set; }
    }

}