using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{


    /// <summary>
    ///     
    /// </summary>
    public class DeleteBatchCmd : ICommand<object>
    {
        protected internal string batchId;

        protected internal bool cascadeToHistory;

        public DeleteBatchCmd(string batchId, bool cascadeToHistory)
        {
            this.batchId = batchId;
            this.cascadeToHistory = cascadeToHistory;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Batch id must not be null", "batch id", batchId);

            BatchEntity batchEntity = commandContext.BatchManager.FindBatchById(batchId);
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Batch for id '" + batchId + "' cannot be found",
                "batch", batchEntity);

            CheckAccess(commandContext, batchEntity);

            batchEntity.Delete(cascadeToHistory);
            return null;
        }

        protected internal virtual void CheckAccess(CommandContext commandContext, BatchEntity batch)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckDeleteBatch(batch);
        }
    }
}