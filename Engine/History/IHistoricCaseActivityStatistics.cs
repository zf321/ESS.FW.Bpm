namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     
    /// </summary>
    public interface IHistoricCaseActivityStatistics
    {
        /// <summary>
        ///     The case activity id.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The number of available case activity instances.
        /// </summary>
        long Available { get; }

        /// <summary>
        ///     The number of enabled case activity instances.
        /// </summary>
        long Enabled { get; }

        /// <summary>
        ///     The number of disabled case activity instances.
        /// </summary>
        long Disabled { get; }

        /// <summary>
        ///     The number of active case activity instances.
        /// </summary>
        long Active { get; }

        /// <summary>
        ///     The number of completed case activity instances.
        /// </summary>
        long Completed { get; }

        /// <summary>
        ///     The number of terminated case activity instances.
        /// </summary>
        long Terminated { get; }
    }
}