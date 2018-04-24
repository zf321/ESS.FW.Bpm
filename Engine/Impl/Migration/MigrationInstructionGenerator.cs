using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.migration.validation.activity;
using ESS.FW.Bpm.Engine.Impl.migration.validation.instruction;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration
{
    /// <summary>
    ///     Generates all migration instructions which represent a direct one
    ///     to one mapping of mapped entities in two process definitions. See
    ///     also <seealso cref="IMigrationActivityMatcher" />.
    ///     
    /// </summary>
    public interface IMigrationInstructionGenerator
    {
        /// <summary>
        ///     Sets the list of migration activity validators which validate that a activity
        ///     is a candidate for the migration.
        /// </summary>
        /// <param name="migrationActivityValidators"> the list of validators to check </param>
        /// <returns> this generator instance </returns>
        IMigrationInstructionGenerator MigrationActivityValidators(
            IList<IMigrationActivityValidator> migrationActivityValidators);

        /// <summary>
        ///     Sets the list of migration instruction validators currently used by the process engine.
        ///     Implementations may use these to restrict the search space.
        /// </summary>
        /// <returns> this </returns>
        IMigrationInstructionGenerator MigrationInstructionValidators(
            IList<IMigrationInstructionValidator> migrationInstructionValidators);

        /// <summary>
        ///     Generate all migration instructions for mapped activities between two process definitions. A activity can be mapped
        ///     if the <seealso cref="IMigrationActivityMatcher" /> matches it with an activity from the target process definition.
        /// </summary>
        /// <param name="sourceProcessDefinition"> the source process definition </param>
        /// <param name="targetProcessDefinition"> the target process definiton </param>
        /// <returns> the list of generated instructions </returns>
        ValidatingMigrationInstructions Generate(ProcessDefinitionImpl sourceProcessDefinition,
            ProcessDefinitionImpl targetProcessDefinition, bool updateEventTriggers);
    }
}