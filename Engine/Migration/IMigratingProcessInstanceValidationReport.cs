using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Migration
{
    /// <summary>
    ///     Collects general failures  and the migrating activity instance validation
    ///     reports for a migrating process instance.
    ///     A general failures is that the state of the process instance doesn't allow
    ///     the migration independent from an specific activity instance. For example
    ///     if non migrated jobs exist.
    /// </summary>
    public interface IMigratingProcessInstanceValidationReport
    {
        /// <returns> the id of the process instance that the migration plan is applied to </returns>
        string ProcessInstanceId { get; }

        /// <returns> the list of general failures of the migrating process instance </returns>
        IList<string> Failures { get; }

        /// <returns> the list of activity instance validation reports </returns>
        IList<IMigratingActivityInstanceValidationReport> ActivityInstanceReports { get; }

        /// <returns> the list of transition instance validation reports </returns>
        IList<IMigratingTransitionInstanceValidationReport> TransitionInstanceReports { get; }

        /// <returns> true if general failures or activity instance validation reports exist, false otherwise </returns>
        bool HasFailures();
    }
}