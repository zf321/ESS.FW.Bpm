using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    public class FoxAtomicOperationDeleteCascadeFireActivityEnd : PvmAtomicOperationDeleteCascadeFireActivityEnd
    {
        protected internal override void EventNotificationsCompleted(PvmExecutionImpl execution)
        {
            IPvmActivity activity = execution.Activity;
            if (execution.IsScope && (activity != null) && !activity.IsScope)
            {
                execution.Activity=(ActivityImpl) activity.FlowScope;
                execution.PerformOperation(this);
            }
            else
            {
                if (execution.IsScope)
                    execution.Destroy();

                execution.Remove();
            }
        }

        public override void Execute(PvmExecutionImpl execution)
        {
            var scope = GetScope(execution);
            var executionListenerIndex = execution.ListenerIndex;
            var executionListeners = scope.GetListeners(EventName);
            foreach (var listener in executionListeners)
            {
                execution.EventName = EventName;
                execution.EventSource = scope;
                //try
                //{
                    execution.InvokeListener(listener);
                //}
                //catch (Exception e)
                //{
                //    throw e;
                //}
                //catch (Exception e)
                //{
                //    throw new PvmException("couldn't execute event listener : " + e.Message, e);
                //}
                executionListenerIndex += 1;
                execution.ListenerIndex = executionListenerIndex;
            }
            execution.ListenerIndex = 0;
            execution.EventName = null;
            execution.EventSource = null;

            EventNotificationsCompleted(execution);
        }
    }
}