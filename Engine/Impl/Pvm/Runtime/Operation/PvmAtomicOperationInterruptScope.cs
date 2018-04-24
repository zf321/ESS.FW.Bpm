using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public abstract class PvmAtomicOperationInterruptScope : IPvmAtomicOperation
    {
        public virtual void Execute(PvmExecutionImpl execution)
        {
            var interruptingActivity = GetInterruptingActivity(execution);

            var scopeExecution = !execution.IsScope ? execution.Parent : execution;

            if (scopeExecution != execution)
                execution.Remove();

            scopeExecution.Interrupt("Interrupting activity " + interruptingActivity + " executed.");

            scopeExecution.Activity=(ActivityImpl) (interruptingActivity);
            scopeExecution.IsActive = true;
            scopeExecution.Transition=(execution.Transition);
            ScopeInterrupted(scopeExecution);
        }

        public virtual bool IsAsync(PvmExecutionImpl execution)
        {
            return false;
        }

        public virtual bool AsyncCapable
        {
            get { return false; }
        }

        public string CanonicalName
        {
            get { throw new NotImplementedException(); }
        }

        protected internal abstract void ScopeInterrupted(IActivityExecution execution);

        protected internal abstract IPvmActivity GetInterruptingActivity(PvmExecutionImpl execution);
    }
}