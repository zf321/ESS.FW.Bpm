namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     <para>Represents a report result about duration of completed instances for a given period.</para>
    ///     <para>The result must be interpreted in conjunction with the executed report.</para>
    ///     
    /// </summary>
    public interface IDurationReportResult : IReportResult
    {
        /// <summary>
        ///     <para>
        ///         Returns the smallest duration of all completed instances,
        ///         which have been started in the given period.
        ///     </para>
        /// </summary>
        long Minimum { get; }

        /// <summary>
        ///     <para>
        ///         Returns the greatest duration of all completed instances,
        ///         which have been started in the given period.
        ///     </para>
        /// </summary>
        long Maximum { get; }

        /// <summary>
        ///     <para>
        ///         Returns the average duration of all completed instances,
        ///         which have been started in the given period.
        ///     </para>
        /// </summary>
        long Average { get; }
    }
}