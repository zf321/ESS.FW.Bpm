using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///      
    /// </summary>
    public class PvmAtomicOperationDeleteCascade : IPvmAtomicOperation
    {
        public virtual bool IsAsync(PvmExecutionImpl execution)
        {
            return false;
        }

        public virtual bool AsyncCapable
        {
            get { return false; }
        }

        public virtual void Execute(PvmExecutionImpl execution)
        {
            var firstLeaf = FindFirstLeaf(execution);

            // propagate skipCustomListeners property
            var deleteRoot = GetDeleteRoot(execution);
            if (deleteRoot != null)
            {
                firstLeaf.SkipCustomListeners = deleteRoot.SkipCustomListeners;
                firstLeaf.SkipIoMappings = deleteRoot.SkipIoMappings;
            }

            if (firstLeaf.SubProcessInstance != null)
                firstLeaf.SubProcessInstance.DeleteCascade(execution.DeleteReason, firstLeaf.SkipCustomListeners,false);

            ((PvmExecutionImpl)firstLeaf).PerformOperation(PvmAtomicOperationFields.DeleteCascadeFireActivityEnd);
        }

        public virtual string CanonicalName
        {
            get { return "delete-cascade"; }
        }

        protected internal virtual IActivityExecution FindFirstLeaf(IActivityExecution execution)
        {
            if (execution.HasChildren())
                return FindFirstLeaf(execution.Executions[0]);
            return execution;
        }

        protected internal virtual IActivityExecution GetDeleteRoot(IActivityExecution execution)
        {
            if (execution == null)
                return null;
            if (execution.IsDeleteRoot)
                return execution;
            return GetDeleteRoot(execution.Parent);
        }
    }
}