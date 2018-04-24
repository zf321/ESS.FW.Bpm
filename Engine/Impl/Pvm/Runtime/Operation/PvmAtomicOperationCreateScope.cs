using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class PvmAtomicOperationCreateScope : IPvmAtomicOperation
    {
        private static readonly PvmLogger Log = ProcessEngineLogger.PvmLogger;
        public abstract bool AsyncCapable { get; }

        public abstract string CanonicalName { get; }

        public virtual void Execute(PvmExecutionImpl execution)
        {
            // reset activity instance id before creating the scope
            execution.ActivityInstanceId = execution.ParentActivityInstanceId;

            PvmExecutionImpl propagatingExecution = null;
            IPvmActivity activity = execution.Activity;
            if (activity != null && activity.IsScope)
            {
                propagatingExecution = (PvmExecutionImpl) execution.CreateExecution();
                propagatingExecution.Activity=activity;
                propagatingExecution.Transition=(execution.Transition);
                execution.Transition=(null);
                execution.IsActive = false;
                execution.Activity=(null);
                Log.CreateScope(execution, propagatingExecution);
                propagatingExecution.Initialize();
            }
            else
            {
                propagatingExecution = execution;
            }


            ScopeCreated(propagatingExecution);
        }

        public abstract bool IsAsync(PvmExecutionImpl instance);

        /// <summary>
        ///     Called with the propagating execution
        /// </summary>
        protected internal abstract void ScopeCreated(PvmExecutionImpl execution);
    }
}