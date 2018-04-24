using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Migration
{
    /// <summary>
    ///     Collects the validation failures for a single migration
    ///     instruction.
    /// </summary>
    public interface IMigrationInstructionValidationReport
    {
        /// <returns> the migration instruction of this report </returns>
        IMigrationInstruction MigrationInstruction { get; }

        /// <returns> the list of failure messages </returns>
        IList<string> Failures { get; }

        /// <returns> true if the report contains failures, false otherwise </returns>
        bool HasFailures();
    }
}