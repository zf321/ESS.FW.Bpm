using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    public class AdditionalFlowScopeInstructionValidator : IMigrationInstructionValidator
    {
        public virtual void Validate(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            var ancestorScopeInstruction = GetClosestPreservedAncestorScopeMigrationInstruction(instruction,
                instructions);
            ScopeImpl targetScope = (ScopeImpl) instruction.TargetActivity;

            if ((ancestorScopeInstruction != null) && (targetScope != null) &&
                (targetScope != targetScope.ProcessDefinition))
            {
                ScopeImpl parentInstanceTargetScope = (ScopeImpl) ancestorScopeInstruction.TargetActivity;
                if ((parentInstanceTargetScope != null) && !parentInstanceTargetScope.IsAncestorFlowScopeOf(targetScope))
                    report.AddFailure("The closest mapped ancestor '" + ancestorScopeInstruction.SourceActivity.Id +
                                      "' is mapped to scope '" + parentInstanceTargetScope.Id +
                                      "' which is not an ancestor of target scope '" + targetScope.Id + "'");
            }
        }

        protected internal virtual IValidatingMigrationInstruction GetClosestPreservedAncestorScopeMigrationInstruction(
            IValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions)
        {
            var parent = instruction.SourceActivity.FlowScope;

            while ((parent != null) && (instructions.GetInstructionsBySourceScope(parent).Count == 0))
                parent = parent.FlowScope;

            if (parent != null)
                return instructions.GetInstructionsBySourceScope(parent)[0];
            return null;
        }
    }
}