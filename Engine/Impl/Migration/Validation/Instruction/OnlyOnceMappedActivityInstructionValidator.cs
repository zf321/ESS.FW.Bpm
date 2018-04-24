using ESS.FW.Bpm.Engine.Impl.Pvm;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    public class OnlyOnceMappedActivityInstructionValidator : IMigrationInstructionValidator
    {
        public virtual void Validate(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            IPvmScope sourceActivity = instruction.SourceActivity;
            var instructionsForSourceActivity = instructions.GetInstructionsBySourceScope(sourceActivity);

            if (instructionsForSourceActivity.Count > 1)
            {
//addFailure(sourceActivity.Id, instructionsForSourceActivity, report);
            }
        }

//protected internal virtual void addFailure(string sourceActivityId,
//    IList<ValidatingMigrationInstruction> migrationInstructions, MigrationInstructionValidationReportImpl report)
//{
//    report.addFailure("There are multiple mappings for source activity id '" + sourceActivityId + "': " +
//                      StringUtil.join(new StringIteratorAnonymousInnerClass(this,
//                          migrationInstructions.GetEnumerator())));
//}

//private class StringIteratorAnonymousInnerClass : StringUtil.StringIterator<ValidatingMigrationInstruction>
//{
//    private readonly OnlyOnceMappedActivityInstructionValidator outerInstance;

//    public StringIteratorAnonymousInnerClass(OnlyOnceMappedActivityInstructionValidator outerInstance,
//        UnknownType iterator) : base(iterator)
//    {
//        this.outerInstance = outerInstance;
//    }

//    public virtual string next()
//    {
//        return iterator.next().ToString();
//    }
//}
    }
}