namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     
    /// </summary>
    public interface IHistoricActivityStatistics
    {
        /// <summary>
        ///     The activity id.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The number of all running instances of the activity.
        /// </summary>
        long Instances { get; }

        /// <summary>
        ///     The number of all finished instances of the activity.
        /// </summary>
        long Finished { get; }

        /// <summary>
        ///     The number of all canceled instances of the activity.
        /// </summary>
        long Canceled { get; }

        /// <summary>
        ///     The number of all instances, which complete a scope (ie. in bpmn manner: an activity
        ///     which consumed a token and did not produced a new one), of the activity.
        /// </summary>
        long CompleteScope { get; }
    }
}