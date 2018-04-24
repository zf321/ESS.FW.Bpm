namespace ESS.FW.Bpm.Engine.Migration
{
    /// <summary>
    ///     
    /// </summary>
    public interface IMigrationInstructionsBuilder : IMigrationPlanBuilder
    {
        /// <summary>
        ///     Toggle whether the instructions should include updating of the respective event triggers
        ///     where appropriate. See <seealso cref="IMigrationInstructionBuilder#updateEventTrigger()" /> for details
        ///     what updating the event trigger means for a single instruction.
        /// </summary>
        /// <returns> this builder </returns>
        IMigrationInstructionsBuilder UpdateEventTriggers();
    }
}