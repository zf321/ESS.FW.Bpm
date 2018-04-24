


using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.oplog
{
    /// <summary>
    ///     One op log context entry represents an operation on a set of entities of the same type (see entityType field).
    ///     It consist multiple <seealso cref="PropertyChange" />s that end up as multiple history events.
    ///     
    /// </summary>
    public class UserOperationLogContextEntry
    {
        protected internal string batchId;
        protected internal string caseDefinitionId;
        protected internal string caseExecutionId;
        protected internal string caseInstanceId;

        protected internal string deploymentId;
        protected internal string entityType;
        protected internal string executionId;
        protected internal string jobDefinitionId;
        protected internal string jobId;
        protected internal string operationType;
        protected internal string processDefinitionId;
        protected internal string processDefinitionKey;
        protected internal string processInstanceId;
        protected internal IList<PropertyChange> propertyChanges;
        protected internal string taskId;

        public UserOperationLogContextEntry(string operationType, string entityType)
        {
            this.operationType = operationType;
            this.entityType = entityType;
        }

        public virtual string DeploymentId
        {
            get { return deploymentId; }
            set { deploymentId = value; }
        }


        public virtual string ProcessDefinitionId
        {
            get { return processDefinitionId; }
            set { processDefinitionId = value; }
        }


        public virtual string ProcessInstanceId
        {
            get { return processInstanceId; }
            set { processInstanceId = value; }
        }


        public virtual string ExecutionId
        {
            get { return executionId; }
            set { executionId = value; }
        }


        public virtual string CaseDefinitionId
        {
            get { return caseDefinitionId; }
            set { caseDefinitionId = value; }
        }


        public virtual string CaseInstanceId
        {
            get { return caseInstanceId; }
            set { caseInstanceId = value; }
        }


        public virtual string CaseExecutionId
        {
            get { return caseExecutionId; }
            set { caseExecutionId = value; }
        }


        public virtual string TaskId
        {
            get { return taskId; }
            set { taskId = value; }
        }


        public virtual string OperationType
        {
            get { return operationType; }
            set { operationType = value; }
        }


        public virtual string EntityType
        {
            get { return entityType; }
            set { entityType = value; }
        }


        public virtual IList<PropertyChange> PropertyChanges
        {
            get { return propertyChanges; }
            set { propertyChanges = value; }
        }


        public virtual string ProcessDefinitionKey
        {
            get { return processDefinitionKey; }
            set { processDefinitionKey = value; }
        }


        public virtual string JobDefinitionId
        {
            get { return jobDefinitionId; }
            set { jobDefinitionId = value; }
        }


        public virtual string JobId
        {
            get { return jobId; }
            set { jobId = value; }
        }


        public virtual string BatchId
        {
            get { return batchId; }
            set { batchId = value; }
        }
    }
}