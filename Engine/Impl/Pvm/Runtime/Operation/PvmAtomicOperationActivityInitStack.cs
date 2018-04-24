using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     Instantiates the next activity on the stack of the current execution's start context.
    ///     
    /// </summary>
    public class PvmAtomicOperationActivityInitStack : IPvmAtomicOperation
    {
        protected internal IPvmAtomicOperation OperationOnScopeInitialization;

        public PvmAtomicOperationActivityInitStack(IPvmAtomicOperation operationOnScopeInitialization)
        {
            this.OperationOnScopeInitialization = operationOnScopeInitialization;
        }

        public virtual string CanonicalName
        {
            get { return "activity-stack-init"; }
        }

        public virtual void Execute(PvmExecutionImpl execution)
        {
            var executionStartContext = execution.ExecutionStartContext;

            var instantiationStack = executionStartContext.InstantiationStack;
            var activityStack = instantiationStack.Activities;
            var currentActivity = activityStack[0];
            activityStack.RemoveAt(0);

            var propagatingExecution = execution;
            if (currentActivity.IsScope)
            {
                propagatingExecution = (PvmExecutionImpl) execution.CreateExecution();
                execution.IsActive = false;
                propagatingExecution.Activity=(ActivityImpl) (currentActivity);
                propagatingExecution.Initialize();
            }
            else
            {
                propagatingExecution.Activity=(ActivityImpl) (currentActivity);
            }

            // notify listeners for the instantiated activity
            propagatingExecution.PerformOperation(OperationOnScopeInitialization);
        }

        public virtual bool IsAsync(PvmExecutionImpl instance)
        {
            return false;
        }

        public virtual bool AsyncCapable
        {
            get { return false; }
        }

        public virtual PvmExecutionImpl GetStartContextExecution(PvmExecutionImpl execution)
        {
            return execution;
        }
    }
}