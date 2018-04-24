using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.activity
{
    /// <summary>
    ///     Check if an activity can be migrated. For example
    ///     if the activity type is supported by the migration.
    /// </summary>
    public interface IMigrationActivityValidator
    {
        /// <summary>
        ///     Check that an activity can be migrated.
        /// </summary>
        /// <param name="activity"> the activity to migrate </param>
        /// <returns> true if the activity can be migrated, false otherwise </returns>
        bool Valid(ActivityImpl activity);
    }
}