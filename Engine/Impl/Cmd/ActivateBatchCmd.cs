using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using BatchEntity = ESS.FW.Bpm.Engine.Batch.Impl.BatchEntity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    public class ActivateBatchCmd : AbstractSetBatchStateCmd
    {
        public ActivateBatchCmd(string batchId) : base(batchId)
        {
        }

        protected internal override ISuspensionState NewSuspensionState
        {
            get { return SuspensionStateFields.Active; }
        }

        protected internal override string UserOperationType
        {
            get { return UserOperationLogEntryFields.OperationTypeActivateBatch; }
        }

        protected internal override void CheckAccess(ICommandChecker checker, BatchEntity batch)
        {
            checker.CheckActivateBatch(batch);
        }

        protected internal override AbstractSetJobDefinitionStateCmd CreateSetJobDefinitionStateCommand(
            UpdateJobDefinitionSuspensionStateBuilderImpl builder)
        {
            return new ActivateJobDefinitionCmd(builder);
        }
    }
}