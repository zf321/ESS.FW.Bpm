using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.activity
{
    /// <summary>
    ///     Validator for events that passively wait for an event, i.e. without being activated by sequence flow (e.g. boundary
    ///     events
    ///     and event subprocess start events but not intermediate catch events).
    ///     
    /// </summary>
    public class SupportedPassiveEventTriggerActivityValidator : IMigrationActivityValidator
    {
        public static SupportedPassiveEventTriggerActivityValidator Instance =
            new SupportedPassiveEventTriggerActivityValidator();

        public static readonly IList<string> SupportedTypes = new List<string>  
        {
            ActivityTypes.BoundaryMessage,
            ActivityTypes.BoundarySignal,
            ActivityTypes.BoundaryTimer,
            ActivityTypes.BoundaryCompensation,
            ActivityTypes.BoundaryConditional,
            ActivityTypes.StartEventMessage,
            ActivityTypes.StartEventSignal,
            ActivityTypes.StartEventTimer,
            ActivityTypes.StartEventCompensation,
            ActivityTypes.StartEventConditional
        };

        public virtual bool Valid(ActivityImpl activity)
        {
            return (activity != null) && (!IsPassivelyWaitingEvent(activity) || IsSupportedEventType(activity));
        }

        public virtual bool IsPassivelyWaitingEvent(ActivityImpl activity)
        {
            return activity.ActivityBehavior is BoundaryEventActivityBehavior ||
                   activity.ActivityBehavior is EventSubProcessStartEventActivityBehavior;
        }

        public virtual bool IsSupportedEventType(ActivityImpl activity)
        {
            return SupportedTypes.Contains(activity.Properties.Get(BpmnProperties.Type));
        }
    }
}