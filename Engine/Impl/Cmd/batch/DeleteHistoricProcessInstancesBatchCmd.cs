using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd.Batch
{

    
    /// <summary>
    ///     
    /// </summary>
    public class DeleteHistoricProcessInstancesBatchCmd : AbstractIdBasedBatchCmd<IBatch>
    {
        protected internal readonly string DeleteReason;
        protected internal IList<string> historicProcessInstanceIds;
        protected internal IQueryable<IHistoricProcessInstance> HistoricProcessInstanceQuery;

        public DeleteHistoricProcessInstancesBatchCmd(IList<string> historicProcessInstanceIds,
            IQueryable<IHistoricProcessInstance> historicProcessInstanceQuery, string deleteReason)
        {
            this.historicProcessInstanceIds = historicProcessInstanceIds;
            this.HistoricProcessInstanceQuery = historicProcessInstanceQuery;
            this.DeleteReason = deleteReason;
        }

        public virtual IList<string> HistoricProcessInstanceIds
        {
            get { return historicProcessInstanceIds; }
        }

        protected internal virtual IList<string> CollectHistoricProcessInstanceIds()
        {
            throw new NotImplementedException();
//            ISet<string> collectedProcessInstanceIds = new HashSet<string>();

//            var processInstanceIds = HistoricProcessInstanceIds;
//            if (processInstanceIds != null)
//            {
//                //collectedProcessInstanceIds.addAll(processInstanceIds);
//            }

//            var processInstanceQuery = (HistoricProcessInstanceQueryImpl) HistoricProcessInstanceQuery;
//            if (processInstanceQuery != null)
//                foreach (var hpi in processInstanceQuery.List())
//                    collectedProcessInstanceIds.Add(hpi.Id);

//            return new List<string>(collectedProcessInstanceIds);
        }

        public override IBatch Execute(CommandContext commandContext)
        {
            var processInstanceIds = CollectHistoricProcessInstanceIds();

            EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "historicProcessInstanceIds", processInstanceIds.ToList());
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

        protected internal virtual void WriteUserOperationLog(CommandContext commandContext, string deleteReason,
            int numInstances, bool async)
        {
            IList<PropertyChange> propertyChanges = new List<PropertyChange>();
            propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
            propertyChanges.Add(new PropertyChange("async", null, async));
            propertyChanges.Add(new PropertyChange("type", null, "history"));
            propertyChanges.Add(new PropertyChange("deleteReason", null, deleteReason));

            commandContext.OperationLogManager.LogProcessInstanceOperation(UserOperationLogEntryFields.OperationTypeDelete, null, null, null, propertyChanges);
        }

        protected internal override BatchConfiguration GetAbstractIdsBatchConfiguration(IList<string> processInstanceIds)
        {
            return new BatchConfiguration(processInstanceIds);
        }

        protected internal override IBatchJobHandler GetBatchJobHandler(
            ProcessEngineConfigurationImpl processEngineConfiguration)
        {

            return
                processEngineConfiguration.BatchHandlers[BatchFields.TypeHistoricProcessInstanceDeletion];
        }
    }
}