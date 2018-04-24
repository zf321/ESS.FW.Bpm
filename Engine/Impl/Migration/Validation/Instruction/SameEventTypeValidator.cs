using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    /// <summary>
    ///     
    /// </summary>
    public class SameEventTypeValidator : IMigrationInstructionValidator
    {
        public virtual void Validate(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            var sourceActivity = instruction.SourceActivity;
            var targetActivity = instruction.TargetActivity;

            if (IsEvent((ActivityImpl) sourceActivity) && IsEvent((ActivityImpl) targetActivity))
            {
                var sourceType = sourceActivity.Properties.Get(BpmnProperties.Type);
                var targetType = targetActivity.Properties.Get(BpmnProperties.Type);

                if (!sourceType.Equals(targetType))
                    report.AddFailure("Events are not of the same type (" + sourceType + " != " + targetType + ")");
            }
        }

        protected internal virtual bool IsEvent(ActivityImpl activity)
        {
            var behavior = (IActivityBehavior) activity.ActivityBehavior;
            return behavior is BoundaryEventActivityBehavior || behavior is EventSubProcessStartEventActivityBehavior;
        }
    }
}