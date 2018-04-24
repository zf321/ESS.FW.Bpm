using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Migration
{
    /// <summary>
    ///     Collects all failures for a migrating activity instance.
    /// </summary>
    public interface IMigratingActivityInstanceValidationReport
    {
        /// <returns> the id of the source scope of the migrated activity instance </returns>
        string SourceScopeId { get; }

        /// <returns> the activity instance id of this report </returns>
        string ActivityInstanceId { get; }

        /// <returns> the migration instruction that cannot be applied </returns>
        IMigrationInstruction MigrationInstruction { get; }

        /// <returns> the list of failures </returns>
        IList<string> Failures { get; }

        /// <returns> true if the reports contains failures, false otherwise </returns>
        bool HasFailures();
    }
}