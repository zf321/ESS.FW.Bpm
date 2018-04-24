using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    /// <summary>
    ///     <para>
    ///         For synchronizing gateways (inclusive; parallel), the situation in which
    ///         more tokens end up at the target gateway than there are incoming sequence flows
    ///         must be avoided. Else, the migrated process instance may appear as broken to users
    ///         since the migration logic cannot trigger these gateways immediately.
    ///     </para>
    ///     <para>
    ///         Such situations can be avoided by enforcing that
    ///         <ul>
    ///             <li>
    ///                 the target gateway has at least the same number of incoming sequence flows
    ///                 <li>
    ///                     the target gateway's flow scope is not removed
    ///                     <li>
    ///                         there is not more than one instruction that maps to the target gateway
    ///                         
    ///     </para>
    /// </summary>
    public class GatewayMappingValidator : IMigrationInstructionValidator
    {
        public virtual void Validate(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            ActivityImpl targetActivity = (ActivityImpl) instruction.TargetActivity;

            if (IsWaitStateGateway(targetActivity))
            {
                ValidateIncomingSequenceFlows(instruction, instructions, report);
                ValidateParentScopeMigrates(instruction, instructions, report);
                ValidateSingleInstruction(instruction, instructions, report);
            }
        }


        protected internal virtual void ValidateIncomingSequenceFlows(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            var sourceActivity = instruction.SourceActivity;
            var targetActivity = instruction.TargetActivity;

            var numSourceIncomingFlows = sourceActivity.IncomingTransitions.Count;
            var numTargetIncomingFlows = targetActivity.IncomingTransitions.Count;

            if (numSourceIncomingFlows > numTargetIncomingFlows)
                report.AddFailure("The target gateway must have at least the same number " +
                                  "of incoming sequence flows that the source gateway has");
        }

        protected internal virtual void ValidateParentScopeMigrates(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            var sourceActivity = instruction.SourceActivity;
            var flowScope = sourceActivity.FlowScope;

            if (flowScope != flowScope.ProcessDefinition)
                if (instructions.GetInstructionsBySourceScope(flowScope).Count == 0)
                    report.AddFailure("The gateway's flow scope '" + flowScope.Id + "' must be mapped");
        }

        protected internal virtual void ValidateSingleInstruction(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            var targetActivity = instruction.TargetActivity;
            var instructionsToTargetGateway = instructions.GetInstructionsByTargetScope(targetActivity);

            if (instructionsToTargetGateway.Count > 1)
                report.AddFailure("Only one gateway can be mapped to gateway '" + targetActivity.Id + "'");
        }

        protected internal virtual bool IsWaitStateGateway(ActivityImpl activity)
        {
            var behavior = (IActivityBehavior) activity.ActivityBehavior;
            return behavior is ParallelGatewayActivityBehavior || behavior is InclusiveGatewayActivityBehavior;
        }
    }
}