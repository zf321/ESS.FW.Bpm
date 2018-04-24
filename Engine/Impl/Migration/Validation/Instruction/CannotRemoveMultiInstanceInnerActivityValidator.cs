using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    /// <summary>
    ///     Validates that the target process definition cannot add a remove the inner activity of a
    ///     migrating multi-instance body.
    ///     
    /// </summary>
    public class CannotRemoveMultiInstanceInnerActivityValidator : IMigrationInstructionValidator
    {
        public virtual void Validate(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            ActivityImpl sourceActivity = (ActivityImpl) instruction.SourceActivity;

            if (IsMultiInstance(sourceActivity))
            {
                var innerActivity = GetInnerActivity(sourceActivity);

                if (instructions.GetInstructionsBySourceScope(innerActivity).Count == 0)
                    report.AddFailure(
                        "Cannot remove the inner activity of a multi-instance body when the body is mapped");
            }
        }

        protected internal virtual bool IsMultiInstance(ScopeImpl flowScope)
        {
            return flowScope.ActivityBehavior is MultiInstanceActivityBehavior;
        }

        protected internal virtual ActivityImpl GetInnerActivity(ActivityImpl multiInstanceBody)
        {
            var activityBehavior = (MultiInstanceActivityBehavior) multiInstanceBody.ActivityBehavior;
            return activityBehavior.GetInnerActivity(multiInstanceBody);
        }
    }
}