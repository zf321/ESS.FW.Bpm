using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Batch.Impl.Deletion;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd.Batch
{

    /// <summary>
    ///     
    /// </summary>
    public class DeleteProcessInstanceBatchCmd : AbstractIdBasedBatchCmd<IBatch>
    {
        protected internal readonly string DeleteReason;
        protected internal IList<string> processInstanceIds;
        protected internal IQueryable<IProcessInstance> ProcessInstanceQuery;

        public DeleteProcessInstanceBatchCmd(IList<string> processInstances, IQueryable<IProcessInstance> processInstanceQuery,
            string deleteReason)
        {
            processInstanceIds = processInstances;
            this.ProcessInstanceQuery = processInstanceQuery;
            this.DeleteReason = deleteReason;
        }

        public virtual IList<string> ProcessInstanceIds
        {
            get { return processInstanceIds; }
        }

        protected internal virtual IList<string> CollectProcessInstanceIds()
        {
            ISet<string> collectedProcessInstanceIds = new HashSet<string>();

            var processInstanceIds = ProcessInstanceIds;
            if (processInstanceIds != null)
            {
                //collectedProcessInstanceIds.addAll(processInstanceIds);
            }
            
            var processInstanceQuery =  this.ProcessInstanceQuery;
            if (processInstanceQuery != null)
            {
                //collectedProcessInstanceIds.addAll(processInstanceQuery.listIds());
            }

            return new List<string>(collectedProcessInstanceIds);
        }

        public override IBatch Execute(CommandContext commandContext)
        {
            var processInstanceIds = CollectProcessInstanceIds();

            EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "processInstanceIds", processInstanceIds.ToList());
            CheckAuthorizations(commandContext);
            WriteUserOperationLog(commandContext, DeleteReason, processInstanceIds.Count, true);

            var batch = CreateBatch(commandContext, processInstanceIds);

            batch.CreateSeedJobDefinition();
            batch.CreateMonitorJobDefinition();
            batch.CreateBatchJobDefinition();

            batch.FireHistoricStartEvent();

            batch.CreateSeedJob();

            return batch;
        }

        protected internal override BatchConfiguration GetAbstractIdsBatchConfiguration(IList<string> processInstanceIds)
        {
            return new DeleteProcessInstanceBatchConfiguration(processInstanceIds, DeleteReason);
        }
        protected internal override IBatchJobHandler GetBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            return processEngineConfiguration.BatchHandlers[BatchFields.TypeProcessInstanceDeletion];
        }

        protected internal virtual void WriteUserOperationLog(CommandContext commandContext, string deleteReason,
            int numInstances, bool async)
        {
            IList<PropertyChange> propertyChanges = new List<PropertyChange>();
            propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
            propertyChanges.Add(new PropertyChange("async", null, async));
            propertyChanges.Add(new PropertyChange("deleteReason", null, deleteReason));

            commandContext.OperationLogManager.LogProcessInstanceOperation(UserOperationLogEntryFields.OperationTypeDelete, null, null, null, propertyChanges);
        }
    }
}