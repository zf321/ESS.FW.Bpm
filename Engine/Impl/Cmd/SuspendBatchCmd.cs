using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using BatchEntity = ESS.FW.Bpm.Engine.Batch.Impl.BatchEntity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    public class SuspendBatchCmd : AbstractSetBatchStateCmd
    {
        public SuspendBatchCmd(string batchId) : base(batchId)
        {
        }

        protected internal override ISuspensionState NewSuspensionState
        {
            get { return SuspensionStateFields.Suspended; }
        }

        protected internal override string UserOperationType
        {
            get { return UserOperationLogEntryFields.OperationTypeSuspendBatch; }
        }

        protected internal override void CheckAccess(ICommandChecker checker, BatchEntity batch)
        {
            checker.CheckSuspendBatch(batch);
        }

        protected internal override AbstractSetJobDefinitionStateCmd CreateSetJobDefinitionStateCommand(
            UpdateJobDefinitionSuspensionStateBuilderImpl builder)
        {
            return new SuspendJobDefinitionCmd(builder);
        }
    }
}