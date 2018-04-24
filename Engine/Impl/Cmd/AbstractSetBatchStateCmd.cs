using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using BatchEntity = ESS.FW.Bpm.Engine.Batch.Impl.BatchEntity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    public abstract class AbstractSetBatchStateCmd : ICommand<object>
    {
        public const string SuspensionStateProperty = "suspensionState";

        protected internal string BatchId;

        public AbstractSetBatchStateCmd(string batchId)
        {
            this.BatchId = batchId;
        }

        protected internal abstract ISuspensionState NewSuspensionState { get; }

        protected internal abstract string UserOperationType { get; }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Batch id must not be null", "batch id", BatchId);

            BatchManager batchManager = commandContext.BatchManager as BatchManager;

            BatchEntity batch = batchManager.FindBatchById(BatchId);
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Batch for id '" + BatchId + "' cannot be found", "batch", batch);

            CheckAccess(commandContext, batch);

            SetJobDefinitionState(commandContext, batch.SeedJobDefinitionId);
            SetJobDefinitionState(commandContext, batch.MonitorJobDefinitionId);
            SetJobDefinitionState(commandContext, batch.BatchJobDefinitionId);

            batchManager.UpdateBatchSuspensionStateById(BatchId, NewSuspensionState);

            LogUserOperation(commandContext);
            return null;
        }

        protected internal virtual void CheckAccess(CommandContext commandContext, BatchEntity batch)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                CheckAccess(checker, batch);
        }

        protected internal abstract void CheckAccess(ICommandChecker checker, BatchEntity batch);

        protected internal virtual void SetJobDefinitionState(CommandContext commandContext, string jobDefinitionId)
        {
            CreateSetJobDefinitionStateCommand(jobDefinitionId).Execute(commandContext);
        }

        protected internal virtual AbstractSetJobDefinitionStateCmd CreateSetJobDefinitionStateCommand(
            string jobDefinitionId)
        {
            var suspendJobDefinitionCmd =
                CreateSetJobDefinitionStateCommand((UpdateJobDefinitionSuspensionStateBuilderImpl) new UpdateJobDefinitionSuspensionStateBuilderImpl()
                    .ByJobDefinitionId(jobDefinitionId).SetIncludeJobs(true));
            suspendJobDefinitionCmd.DisableLogUserOperation();
            return suspendJobDefinitionCmd;
        }

        protected internal abstract AbstractSetJobDefinitionStateCmd CreateSetJobDefinitionStateCommand(
            UpdateJobDefinitionSuspensionStateBuilderImpl builder);

        protected internal virtual void LogUserOperation(CommandContext commandContext)
        {
            PropertyChange propertyChange = new PropertyChange(SuspensionStateProperty, null, NewSuspensionState.Name);
            commandContext.OperationLogManager.LogBatchOperation(UserOperationType, BatchId, propertyChange);
        }
    }
}