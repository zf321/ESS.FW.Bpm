using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Migration
{
    /// <summary>
    ///     <para>
    ///         Specifies how process instances from one process definition (the <i>source process definition</i>)
    ///         should be migrated to another process definition (the <i>target process definition</i>).
    ///     </para>
    ///     <para>
    ///         A migration plan consists of a number of <seealso cref="IMigrationInstruction" />s that tell which
    ///         activity maps to which. The set of instructions is complete, i.e. the migration logic does not perform
    ///         migration steps that are not given by the instructions
    ///         
    ///     </para>
    /// </summary>
    public interface IMigrationPlan
    {
        /// <returns> the list of instructions that this plan consists of </returns>
        IList<IMigrationInstruction> Instructions { get; }

        /// <returns> the id of the process definition that is migrated from </returns>
        string SourceProcessDefinitionId { get; }

        /// <returns> the id of the process definition that is migrated to </returns>
        string TargetProcessDefinitionId { get; }
    }
}