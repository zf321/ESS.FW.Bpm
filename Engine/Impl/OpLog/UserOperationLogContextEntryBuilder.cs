using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository.Impl;

namespace ESS.FW.Bpm.Engine.Impl.oplog
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE;

    public class UserOperationLogContextEntryBuilder
    {
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal UserOperationLogContextEntry EntryRenamed;

        public static UserOperationLogContextEntryBuilder Entry(string operationType, string entityType)
        {
            var builder = new UserOperationLogContextEntryBuilder();
            builder.EntryRenamed = new UserOperationLogContextEntry(operationType, entityType);
            return builder;
        }

        public virtual UserOperationLogContextEntryBuilder InContextOf(JobEntity job)
        {
            EntryRenamed.JobDefinitionId = job.JobDefinitionId;
            EntryRenamed.ProcessInstanceId = job.ProcessInstanceId;
            EntryRenamed.ProcessDefinitionId = job.ProcessDefinitionId;
            EntryRenamed.ProcessDefinitionKey = job.ProcessDefinitionKey;
            EntryRenamed.DeploymentId = job.DeploymentId;

            return this;
        }

        public virtual UserOperationLogContextEntryBuilder InContextOf(JobDefinitionEntity jobDefinition)
        {
            EntryRenamed.JobDefinitionId = jobDefinition.Id;
            EntryRenamed.ProcessDefinitionId = jobDefinition.ProcessDefinitionId;
            EntryRenamed.ProcessDefinitionKey = jobDefinition.ProcessDefinitionKey;

            if (!ReferenceEquals(jobDefinition.ProcessDefinitionId, null))
            {
                ProcessDefinitionEntity processDefinition =
                context.Impl.Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(
                    jobDefinition.ProcessDefinitionId);
                EntryRenamed.DeploymentId = processDefinition.DeploymentId;
            }

            return this;
        }

        public virtual UserOperationLogContextEntryBuilder InContextOf(ExecutionEntity execution)
        {
            EntryRenamed.ProcessInstanceId = execution.ProcessInstanceId;
            EntryRenamed.ProcessDefinitionId = execution.ProcessDefinitionId;

            ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity)execution.ProcessDefinition;
            EntryRenamed.ProcessDefinitionKey = processDefinition.Key;
            EntryRenamed.DeploymentId = processDefinition.DeploymentId;

            return this;
        }

        public virtual UserOperationLogContextEntryBuilder InContextOf(ProcessDefinitionEntity processDefinition)
        {
            EntryRenamed.ProcessDefinitionId = processDefinition.Id;
            EntryRenamed.ProcessDefinitionKey = processDefinition.Key;
            EntryRenamed.DeploymentId = processDefinition.DeploymentId;

            return this;
        }

        public virtual UserOperationLogContextEntryBuilder InContextOf(TaskEntity task,
            IList<PropertyChange> propertyChanges)
        {
            if (propertyChanges == null || propertyChanges.Count == 0)
            {
                if (UserOperationLogEntryFields.OperationTypeCreate.Equals(EntryRenamed.OperationType))
                {
                    propertyChanges =new List<PropertyChange>() { PropertyChange.EmptyChange };;
                }
            }
            EntryRenamed.PropertyChanges = propertyChanges;

            IResourceDefinitionEntity definition = task.ProcessDefinition;
            if (definition != null)
            {
                EntryRenamed.ProcessDefinitionKey = definition.Key;
                EntryRenamed.DeploymentId = definition.DeploymentId;
            }
            else if (!ReferenceEquals(task.CaseDefinitionId, null))
            {
                throw new NotImplementedException();
                //definition = task.CaseDefinition;
                EntryRenamed.DeploymentId = definition.DeploymentId;
            }

            EntryRenamed.ProcessDefinitionId = task.ProcessDefinitionId;
            EntryRenamed.ProcessInstanceId = task.ProcessInstanceId;
            EntryRenamed.ExecutionId = task.ExecutionId;
            EntryRenamed.CaseDefinitionId = task.CaseDefinitionId;
            EntryRenamed.CaseInstanceId = task.CaseInstanceId;
            EntryRenamed.CaseExecutionId = task.CaseExecutionId;
            EntryRenamed.TaskId = task.Id;

            return this;
        }

        public virtual UserOperationLogContextEntryBuilder InContextOf(ExecutionEntity processInstance,
            IList<PropertyChange> propertyChanges)
        {
            if (propertyChanges == null || propertyChanges.Count == 0)
            {
                if (UserOperationLogEntryFields.OperationTypeCreate.Equals(EntryRenamed.OperationType))
                {
                    propertyChanges =new List<PropertyChange>() {PropertyChange.EmptyChange}; ;
                }
            }
            EntryRenamed.PropertyChanges = propertyChanges;
            EntryRenamed.ProcessInstanceId = processInstance.ProcessInstanceId;
            EntryRenamed.ProcessDefinitionId = processInstance.ProcessDefinitionId;
            EntryRenamed.ExecutionId = processInstance.Id;
            EntryRenamed.CaseInstanceId = processInstance.CaseInstanceId;

            IResourceDefinitionEntity definition =(IResourceDefinitionEntity) processInstance.ProcessDefinition;
            if (definition != null)
            {
                EntryRenamed.ProcessDefinitionKey = definition.Key;
                EntryRenamed.DeploymentId = definition.DeploymentId;
            }

            return this;
        }

        public virtual UserOperationLogContextEntryBuilder PropertyChanges(IList<PropertyChange> propertyChanges)
        {
            EntryRenamed.PropertyChanges = propertyChanges;
            return this;
        }

        public virtual UserOperationLogContextEntryBuilder PropertyChanges(PropertyChange propertyChange)
        {
            IList<PropertyChange> propertyChanges = new List<PropertyChange>();
            propertyChanges.Add(propertyChange);
            EntryRenamed.PropertyChanges = propertyChanges;
            return this;
        }

        public virtual UserOperationLogContextEntry Create()
        {
            return EntryRenamed;
        }

        public virtual UserOperationLogContextEntryBuilder JobId(string jobId)
        {
            EntryRenamed.JobId = jobId;
            return this;
        }

        public virtual UserOperationLogContextEntryBuilder JobDefinitionId(string jobDefinitionId)
        {
            EntryRenamed.JobDefinitionId = jobDefinitionId;
            return this;
        }

        public virtual UserOperationLogContextEntryBuilder ProcessDefinitionId(string processDefinitionId)
        {
            EntryRenamed.ProcessDefinitionId = processDefinitionId;
            return this;
        }

        public virtual UserOperationLogContextEntryBuilder ProcessDefinitionKey(string processDefinitionKey)
        {
            EntryRenamed.ProcessDefinitionKey = processDefinitionKey;
            return this;
        }

        public virtual UserOperationLogContextEntryBuilder ProcessInstanceId(string processInstanceId)
        {
            EntryRenamed.ProcessInstanceId = processInstanceId;
            return this;
        }

        public virtual UserOperationLogContextEntryBuilder DeploymentId(string deploymentId)
        {
            EntryRenamed.DeploymentId = deploymentId;
            return this;
        }

        public virtual UserOperationLogContextEntryBuilder BatchId(string batchId)
        {
            EntryRenamed.BatchId = batchId;
            return this;
        }
    }
}