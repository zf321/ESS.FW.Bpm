using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Migration
{
    /// <summary>
    ///     Collects the migration instruction validation reports for
    ///     all instructions of the migration plan which contain failures.
    /// </summary>
    public interface IMigrationPlanValidationReport
    {
        /// <returns> the migration plan of the validation report </returns>
        IMigrationPlan MigrationPlan { get; }

        /// <returns> all instruction reports </returns>
        IList<IMigrationInstructionValidationReport> InstructionReports { get; }

        /// <returns> true if instructions reports exist, false otherwise </returns>
        bool HasInstructionReports();
    }
}