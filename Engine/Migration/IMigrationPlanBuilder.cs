using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Migration
{
    /// <summary>
    ///     
    /// </summary>
    public interface IMigrationPlanBuilder
    {
        /// <summary>
        ///     Automatically adds a set of instructions for activities that are <em>equivalent</em> in both
        ///     process definitions. By default, this is given if two activities are both user tasks, are on the same
        ///     level of sub process, and have the same id.
        /// </summary>
        IMigrationInstructionsBuilder MapEqualActivities();

        /// <summary>
        ///     Adds a migration instruction that maps activity instances of the source activity (of the source process definition)
        ///     to activity instances of the target activity (of the target process definition)
        /// </summary>
        IMigrationInstructionBuilder MapActivities(string sourceActivityId, string targetActivityId);

        /// <returns>
        ///     a migration plan with all previously specified instructions
        /// </returns>
        /// <exception cref="MigrationPlanValidationException"> if the migration plan contains instructions that are not valid </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#READ" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />
        ///     for both, source and target process definition.
        /// </exception>
        IMigrationPlan Build();
    }
}