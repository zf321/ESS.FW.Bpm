using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    public class SameEventScopeInstructionValidator : IMigrationInstructionValidator
    {
        public virtual void Validate(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            var sourceActivity = instruction.SourceActivity;
            if (IsCompensationBoundaryEvent(sourceActivity))
                return;

            var sourceEventScope = (ScopeImpl) instruction.SourceActivity.EventScope;
            var targetEventScope = (ScopeImpl) instruction.TargetActivity.EventScope;

            if ((sourceEventScope == null) || (sourceEventScope == sourceActivity.FlowScope))
                return;

            if (targetEventScope == null)
            {
                report.AddFailure("The source activity's event scope (" + sourceEventScope.Id +
                                  ") must be mapped but the " + "target activity has no event scope");
            }
            else
            {
                var mappedSourceEventScope = FindMappedEventScope(sourceEventScope, instruction, instructions);
                if ((mappedSourceEventScope == null) || !mappedSourceEventScope.Id.Equals(targetEventScope.Id))
                    report.AddFailure("The source activity's event scope (" + sourceEventScope.Id + ") " +
                                      "must be mapped to the target activity's event scope (" + targetEventScope.Id +
                                      ")");
            }
        }

        protected internal virtual bool IsCompensationBoundaryEvent(IPvmScope sourceActivity)
        {
            var activityType = sourceActivity.Properties.Get(BpmnProperties.Type);
            return ActivityTypes.BoundaryCompensation.Equals(activityType);
        }

        protected internal virtual IPvmScope FindMappedEventScope(ScopeImpl sourceEventScope,
            IValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions)
        {
            if (sourceEventScope != null)
            {
                if (sourceEventScope == sourceEventScope.ProcessDefinition)
                    return (ScopeImpl) instruction.TargetActivity.ProcessDefinition;
                var eventScopeInstructions = instructions.GetInstructionsBySourceScope(sourceEventScope);
                if (eventScopeInstructions.Count > 0)
                    return eventScopeInstructions[0].TargetActivity;
            }
            return null;
        }

        protected internal virtual void AddFailure(IValidatingMigrationInstruction instruction,
            MigrationInstructionValidationReportImpl report, string sourceScopeId, string targetScopeId)
        {
            report.AddFailure("The source activity's event scope (" + sourceScopeId + ") " +
                              "must be mapped to the target activity's event scope (" + targetScopeId + ")");
        }
    }
}