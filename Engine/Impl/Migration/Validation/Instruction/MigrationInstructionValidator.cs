namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    /// <summary>
    ///     Checks that a migration instruction is valid for the
    ///     migration plan. For example if the instruction migrates
    ///     an activity to a different type.
    /// </summary>
    public interface IMigrationInstructionValidator
    {
        /// <summary>
        ///     Check that a migration instruction is valid for a migration plan. If it is invalid
        ///     a failure has to added to the validation report.
        /// </summary>
        /// <param name="instruction"> the instruction to validate </param>
        /// <param name="instructions"> the complete migration plan to validate </param>
        /// <param name="report"> the validation report </param>
        void Validate(IValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions,
            MigrationInstructionValidationReportImpl report);
    }
}