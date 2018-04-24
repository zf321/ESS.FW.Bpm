using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.oplog;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IUserOperationLogManager
    {
        void DeleteOperationLogEntryById(string entryId);
        /// <summary>
        /// 先查缓存
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        IUserOperationLogEntry FindOperationLogById(string entryId);
        void LogAttachmentOperation(string operation, ExecutionEntity processInstance, PropertyChange propertyChange);
        void LogAttachmentOperation(string operation, TaskEntity task, PropertyChange propertyChange);
        void LogBatchOperation(string operation, string batchId, PropertyChange propertyChange);
        void LogDeploymentOperation(string operation, string deploymentId, IList<PropertyChange> propertyChanges);
        void LogJobDefinitionOperation(string operation, string jobDefinitionId, string processDefinitionId, string processDefinitionKey, PropertyChange propertyChange);
        void LogJobOperation(string operation, string jobId, string jobDefinitionId, string processInstanceId, string processDefinitionId, string processDefinitionKey, PropertyChange propertyChange);
        void LogLinkOperation(string operation, TaskEntity task, PropertyChange propertyChange);
        void LogProcessDefinitionOperation(string operation, string processDefinitionId, string processDefinitionKey, PropertyChange propertyChange);
        void LogProcessInstanceOperation(string operation, string processInstanceId, string processDefinitionId, string processDefinitionKey, IList<PropertyChange> propertyChanges);
        void LogTaskOperations(string operation, TaskEntity task, IList<PropertyChange> propertyChanges);
        void LogUserOperations(UserOperationLogContext context);
        void LogVariableOperation(string operation, string executionId, string taskId, PropertyChange propertyChange);
    }
}