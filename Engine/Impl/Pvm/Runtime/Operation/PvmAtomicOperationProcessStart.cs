using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    /// </summary>
    public class PvmAtomicOperationProcessStart : AbstractPvmEventAtomicOperation
    {
        public override bool AsyncCapable => true;

        protected internal override string EventName => ExecutionListenerFields.EventNameStart;

        public override string CanonicalName => "process-start";

        public override bool IsAsync(PvmExecutionImpl execution)
        {
            var startContext = execution.ProcessInstanceStartContext;
            return (startContext != null) && startContext.IsAsync;
        }

        protected internal override CoreModelElement GetScope(PvmExecutionImpl execution)
        {
            return execution.ProcessDefinition;
        }

        protected internal override PvmExecutionImpl EventNotificationsStarted(PvmExecutionImpl execution)
        {
            // Note: the following method call initializes the property
            // "processInstanceStartContext" on the given execution.
            // Do not remove it!
            var executionProcessInstanceStartContext = execution.ProcessInstanceStartContext;
            return execution;
        }

        protected internal override void EventNotificationsCompleted(PvmExecutionImpl execution)
        {
            execution.ContinueIfExecutionDoesNotAffectNextOperation((e)=> e.DispatchEvent(null),
                (e)=>{
                    var processInstanceStartContext = e.ProcessInstanceStartContext;
                    var instantiationStack = processInstanceStartContext.InstantiationStack;

                    if (instantiationStack.Activities.Count == 0)
                    {
                        e.Activity = instantiationStack.TargetActivity;
                        e.PerformOperation(PvmAtomicOperationFields.ActivityStartCreateScope);
                    }
                    else
                    {
                        // initialize the activity instance id
                        e.ActivityInstanceId = e.Id;
                        e.PerformOperation(PvmAtomicOperationFields.ActivityInitStack);
                    }
                },execution);
        }
        
    }
}