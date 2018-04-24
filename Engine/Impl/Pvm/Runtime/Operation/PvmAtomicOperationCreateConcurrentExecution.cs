using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     <para>
    ///         Base atomic operation used for implementing atomic operations which
    ///         create a new concurrent execution for executing an activity. This atomic
    ///         operation makes sure the execution is created under the correct parent.
    ///     </para>
    ///     
    ///     
    ///     
    /// </summary>
    public abstract class PvmAtomicOperationCreateConcurrentExecution : IPvmAtomicOperation
    {
        public abstract bool AsyncCapable { get; }

        public virtual string CanonicalName
        {
            get { throw new NotImplementedException(); }
        }

        public virtual void Execute(PvmExecutionImpl execution)
        {
            // Invariant: execution is the Scope Execution for the activity's flow scope.

            var activityToStart = execution.NextActivity;
            execution.NextActivity = null;

            var propagatingExecution = execution.CreateConcurrentExecution();

            // set next activity on propagating execution
            propagatingExecution.Activity= (activityToStart);
            ConcurrentExecutionCreated((PvmExecutionImpl) propagatingExecution);
        }

        public virtual bool IsAsync(PvmExecutionImpl execution)
        {
            return false;
        }

        protected internal abstract void ConcurrentExecutionCreated(PvmExecutionImpl propagatingExecution);
    }
}