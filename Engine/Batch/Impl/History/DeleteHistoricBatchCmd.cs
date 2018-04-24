using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Batch.Impl.History
{
    public class DeleteHistoricBatchCmd : ICommand<object>
    {
        protected internal string BatchId;

        public DeleteHistoricBatchCmd(string batchId)
        {
            this.BatchId = batchId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Historic batch id must not be null",
                "historic batch id", BatchId);

            //HistoricBatchEntity historicBatch = commandContext.HistoricBatchManager.findHistoricBatchById(batchId);
            //EnsureUtil.EnsureNotNull(typeof (BadUserRequestException),
            //    "Historic batch for id '" + batchId + "' cannot be found", "historic batch", historicBatch);

            //checkAccess(commandContext, historicBatch);

            //historicBatch.delete();

            return null;
        }

        protected internal virtual void CheckAccess(CommandContext commandContext, HistoricBatchEntity batch)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckDeleteHistoricBatch(batch);
        }
    }
}