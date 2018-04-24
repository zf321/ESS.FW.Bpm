
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    /// <summary>
    ///      
    /// </summary>
    public class ConditionalEventUpdateEventTriggerValidator : IMigrationInstructionValidator
    {
        public const string MigrationConditionalValidationErrorMsg =
            "Conditional event has to migrate with update event trigger.";

        public virtual void Validate(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            var sourceActivity = instruction.SourceActivity;

            if (sourceActivity.ActivityBehavior is IConditionalEventBehavior && !instruction.UpdateEventTrigger)
                report.AddFailure(MigrationConditionalValidationErrorMsg);
        }
    }
}

