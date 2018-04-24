using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    /// <summary>
    ///     Validates that the target process definition cannot add a new inner activity to a migrating multi-instance body.
    ///     
    /// </summary>
    public class CannotAddMultiInstanceInnerActivityValidator : IMigrationInstructionValidator
    {
        public virtual void Validate(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            ActivityImpl targetActivity = (ActivityImpl) instruction.TargetActivity;

            if (IsMultiInstance(targetActivity))
            {
                var innerActivity = GetInnerActivity(targetActivity);

                if (instructions.GetInstructionsByTargetScope(innerActivity).Count == 0)
                    report.AddFailure("Must map the inner activity of a multi-instance body when the body is mapped");
            }
        }

        protected internal virtual bool IsMultiInstance(ScopeImpl scope)
        {
            return scope.ActivityBehavior is MultiInstanceActivityBehavior;
        }

        protected internal virtual ActivityImpl GetInnerActivity(ActivityImpl multiInstanceBody)
        {
            var activityBehavior = (MultiInstanceActivityBehavior) multiInstanceBody.ActivityBehavior;
            return activityBehavior.GetInnerActivity(multiInstanceBody);
        }
    }
}