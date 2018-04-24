namespace ESS.FW.Bpm.Engine.Migration
{
    /// <summary>
    ///     Represents an instruction to migrate instances of one activity to another activity.
    ///     Migration instructions are always contained in a <seealso cref="IMigrationPlan" />.
    ///     
    /// </summary>
    public interface IMigrationInstruction
    {
        /// <returns>
        ///     the id of the activity of the source process definition that this
        ///     instruction maps instances from
        /// </returns>
        string SourceActivityId { get; }

        /// <returns>
        ///     the id of the activity of the target process definition that this
        ///     instruction maps instances to
        /// </returns>
        string TargetActivityId { get; }

        /// <returns>
        ///     whether this flow node's event trigger is going to be updated during
        ///     migration. Can only be true for flow nodes that define a persistent event trigger.
        ///     See <seealso cref="IMigrationInstructionBuilder#updateEventTrigger()" /> for details
        /// </returns>
        bool UpdateEventTrigger { get; }
    }
}