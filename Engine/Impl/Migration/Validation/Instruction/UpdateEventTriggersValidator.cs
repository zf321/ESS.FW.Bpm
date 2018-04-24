using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    /// <summary>
    ///     Ensure that the option updateEventTriggers can only be used activities defining an event trigger
    ///     
    /// </summary>
    public class UpdateEventTriggersValidator : IMigrationInstructionValidator
    {
        public virtual void Validate(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            var sourceActivity = instruction.SourceActivity;

            if (instruction.UpdateEventTrigger && !DefinesPersistentEventTrigger(sourceActivity))
                report.AddFailure(
                    "Cannot update event trigger because the activity does not define a persistent event trigger");
        }

        public static bool DefinesPersistentEventTrigger(IPvmScope activity)
        {
            var eventScope = (ScopeImpl) activity.EventScope;

            if (eventScope != null)
                return TimerDeclarationImpl.GetDeclarationsForScope(eventScope).ContainsKey(activity.Id) ||
                       EventSubscriptionDeclaration.GetDeclarationsForScope(eventScope).ContainsKey(activity.Id);
            return false;
        }
    }
}