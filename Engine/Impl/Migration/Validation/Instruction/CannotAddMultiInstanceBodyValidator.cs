using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.tree;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    /// <summary>
    ///     Validates that the target process definition cannot add a migrating multi-instance body.
    ///     
    /// </summary>
    public class CannotAddMultiInstanceBodyValidator : IMigrationInstructionValidator
    {
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void validate(ValidatingMigrationInstruction instruction, final ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        public virtual void Validate(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            var targetActivity = instruction.TargetActivity;

            var flowScopeWalker = new FlowScopeWalker(targetActivity.FlowScope);
            var miBodyCollector = new MiBodyCollector();
            flowScopeWalker.AddPreVisitor(miBodyCollector);

            // walk until a target scope is found that is mapped
            //flowScopeWalker.walkWhile(new WalkConditionAnonymousInnerClass(this, instructions));

            if (miBodyCollector.FirstMiBody != null)
                report.AddFailure("Target activity '" + targetActivity.Id + "' is a descendant of multi-instance body '" +
                                  miBodyCollector.FirstMiBody.Id +
                                  "' that is not mapped from the source process definition.");
        }

        //private class WalkConditionAnonymousInnerClass : WalkCondition<ScopeImpl>
        //{
        //    private readonly CannotAddMultiInstanceBodyValidator outerInstance;

        //    private readonly ValidatingMigrationInstructions instructions;

        //    public WalkConditionAnonymousInnerClass(CannotAddMultiInstanceBodyValidator outerInstance,
        //        ValidatingMigrationInstructions instructions)
        //    {
        //        this.outerInstance = outerInstance;
        //        this.instructions = instructions;
        //    }

        //    public virtual bool isFulfilled(ScopeImpl element)
        //    {
        //        return element == null || instructions.getInstructionsByTargetScope(element).Count > 0;
        //    }
        //}

        public class MiBodyCollector : ITreeVisitor<ScopeImpl>
        {
            protected internal ScopeImpl FirstMiBody;

            public virtual void Visit(ScopeImpl obj)
            {
                if ((FirstMiBody == null) && (obj != null) && IsMiBody(obj))
                    FirstMiBody = obj;
            }

            protected internal virtual bool IsMiBody(ScopeImpl scope)
            {
                return scope.ActivityBehavior is MultiInstanceActivityBehavior;
            }
        }
    }
}