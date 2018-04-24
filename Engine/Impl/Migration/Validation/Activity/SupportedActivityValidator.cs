using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.activity
{
    /// <summary>
    ///     Supported* refers to whether an activity instance of a certain activity type can be migrated.
    ///     This validator is irrelevant for transition instances which can be migrated at any activity type.
    ///     Thus, this validator is only used during migration instruction generation and migrating activity instance
    ///     validation,
    ///     not during migration instruction validation.
    /// </summary>
    public class SupportedActivityValidator : IMigrationActivityValidator
    {
        public static SupportedActivityValidator Instance = new SupportedActivityValidator();

        public static IList<Type> SupportedActivityBehaviors = new List<Type>();

        static SupportedActivityValidator()
        {
            SupportedActivityBehaviors.Add(typeof(SubProcessActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(UserTaskActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(BoundaryEventActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(ParallelMultiInstanceActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(SequentialMultiInstanceActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(ReceiveTaskActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(CallActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(CaseCallActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(IntermediateCatchEventActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(EventBasedGatewayActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(EventSubProcessActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(EventSubProcessStartEventActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(ExternalTaskActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(ParallelGatewayActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(InclusiveGatewayActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(IntermediateConditionalEventBehavior));
            SupportedActivityBehaviors.Add(typeof(BoundaryConditionalEventActivityBehavior));
            SupportedActivityBehaviors.Add(typeof(EventSubProcessStartConditionalEventActivityBehavior));
        }

        public virtual bool Valid(ActivityImpl activity)
        {
            return (activity != null) && (IsSupportedActivity(activity) || IsAsync(activity));
        }

        public virtual bool IsSupportedActivity(ActivityImpl activity)
        {
            return SupportedActivityBehaviors.Contains(activity.ActivityBehavior.GetType());
        }

        protected internal virtual bool IsAsync(ActivityImpl activity)
        {
            return activity.AsyncBefore || activity.AsyncAfter;
        }
    }
}