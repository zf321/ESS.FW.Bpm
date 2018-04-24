using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Event
{
    


    /// <summary>
    ///     
    /// </summary>
    public class CompensationEventHandler : IEventHandler
    {
        public virtual string EventHandlerType
        {
            get { return EventType.Compensate.Name; }
        }

        public virtual void HandleEvent(EventSubscriptionEntity eventSubscription, object payload,
            CommandContext commandContext)
        {
            eventSubscription.Delete();

            var configuration = eventSubscription.Configuration;
            EnsureUtil.EnsureNotNull(
                "Compensating execution not set for compensate event subscription with id " + eventSubscription.Id,
                "configuration", configuration);

            ExecutionEntity compensatingExecution = commandContext.ExecutionManager.FindExecutionById(configuration);

            ActivityImpl compensationHandler = eventSubscription.Activity;

            //activate execution
            compensatingExecution.IsActive = true;

            if (compensatingExecution.GetActivity().ActivityBehavior is ICompositeActivityBehavior)
            {
                compensatingExecution.Parent.ActivityInstanceId = compensatingExecution.ActivityInstanceId;
            }

            if (compensationHandler.IsScope && !compensationHandler.CompensationHandler)
            {
                // descend into scope:
                IList<EventSubscriptionEntity> eventsForThisScope = compensatingExecution.CompensateEventSubscriptions;
                CompensationUtil.ThrowCompensationEvent(eventsForThisScope, compensatingExecution, false);
            }
            else
            {
                try
                {
                    if (compensationHandler.SubProcessScope && compensationHandler.TriggeredByEvent)
                    {
                        compensatingExecution.ExecuteActivity(compensationHandler);
                    }
                    else
                    {
                        // since we already have a scope execution, we don't need to create another one
                        // for a simple scoped compensation handler
                        compensatingExecution.SetActivity(compensationHandler);
                        compensatingExecution.PerformOperation(PvmAtomicOperationFields.ActivityStart);
                    }
                }
                //TODO 暂时取消异常拦截
                //catch (Exception e)
                //{
                //    throw new ProcessEngineException("Error while handling compensation event " + eventSubscription, e);
                //}
                finally
                {
                    
                }
            }
        }
    }
}