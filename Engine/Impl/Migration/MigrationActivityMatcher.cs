using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration
{
    /// <summary>
    ///     Check if two activities are equal in the context
    ///     of the migration of two process definitions.
    ///     It is required that the matcher for each activity of the source
    ///     process definition at most one activity of the target process
    ///     definition matches.
    ///     For example the matching criterion should not be solely base on
    ///     the activity name which is a non unique attribute.
    /// </summary>
    public interface IMigrationActivityMatcher
    {
        /// <summary>
        ///     Checks if an activity from the source process definition of
        ///     a migration matches an activity from the target process
        ///     definition.
        /// </summary>
        /// <param name="source"> the activity from the source process definition </param>
        /// <param name="target"> the activity from the target process definition </param>
        /// <returns>
        ///     true if the source activity matches the target activity
        ///     in the context of the migration, false otherwise
        /// </returns>
        bool MatchActivities(ActivityImpl source, ActivityImpl target);
    }
}