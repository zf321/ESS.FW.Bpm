using ESS.FW.Bpm.Engine.Query;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     This interface defines basic methods for resulting reports.
    ///     
    /// </summary>
    public interface IReportResult
    {
        /// <summary>
        ///     <para>Returns a period which specifies a time span within a year.</para>
        ///     <para>
        ///         The returned period must be interpreted in conjunction
        ///         with the returned <seealso cref="PeriodUnit" /> of <seealso cref="#getPeriodUnit()" />.
        ///     </para>
        ///     </p>For example:</p>
        ///     <ul>
        ///         <li>
        ///             <seealso cref="#getPeriodUnit()" /> returns <seealso cref="PeriodUnit#MONTH" />
        ///             <li><seealso cref="#getPeriod()" /> returns <code>3</code>
        ///     </ul>
        ///     <para>
        ///         The returned period <code>3</code> must be interpreted as
        ///         the third <code>month</code> of the year (i.e. it represents
        ///         the month March).
        ///     </para>
        ///     <para>
        ///         If the <seealso cref="#getPeriodUnit()" /> returns <seealso cref="PeriodUnit#QUARTER" />,
        ///         then the returned period <code>3</code> must be interpreted as the third
        ///         <code>quarter</code> of the year.
        ///     </para>
        /// </summary>
        /// <returns> an integer representing span of time within a year </returns>
        int Period { get; }

        /// <summary>
        ///     <para>Returns the unit of the period.</para>
        /// </summary>
        /// <returns>
        ///     a <seealso cref="PeriodUnit" />
        /// </returns>
        /// <seealso cref= # getPeriod
        /// (
        /// )
        /// </seealso>
        PeriodUnit PeriodUnit { get; }
    }
}