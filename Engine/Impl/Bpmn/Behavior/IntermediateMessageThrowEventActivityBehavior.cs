using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Common.ServiceBus;
using System;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    public class IntermediateMessageThrowEventActivityBehavior : AbstractBpmnActivityBehavior
    {
        private IExpression _eventName;
        public IntermediateMessageThrowEventActivityBehavior(IExpression eventName)
        {
            _eventName = eventName;
        }
        public override void Leave(IActivityExecution execution)
        {
            var value = _eventName.GetValue(execution);
            if (value != null)
            {
                var eventName = value.ToString();
                var scope = Context.CommandContext.Scope;
                IBus bus = scope.Resolve<IBus>();
                //bus.Send("MessageThrowEvent" + "-" + Environment.MachineName, new MessageThrowEvent() { EventName = value.ToString(), Variables = execution.Variables }, true);
            };
            base.Leave(execution);
        }
    }
}