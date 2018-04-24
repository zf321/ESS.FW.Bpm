using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;

namespace ESS.FW.Bpm.Engine.Query
{
    /// <summary>
    ///     Describes basic methods for creating a report.
    ///     
    ///     
    /// </summary>
    public interface IReport
    {
        /// <summary>
        ///     <para>
        ///         Executes the duration report query and returns a list of
        ///         <seealso cref="DurationReportResult" />s.
        ///     </para>
        ///     <para>
        ///         Be aware that the resulting report must be interpreted by the
        ///         caller itself.
        ///     </para>
        /// </summary>
        /// <param name="periodUnit">
        ///     A <seealso cref="PeriodUnit period unit" /> to define
        ///     the granularity of the report.
        /// </param>
        /// <returns>
        ///     a list of <seealso cref="IDurationReportResult" />s
        /// </returns>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permission#READ_HISTORY" /> permission
        ///     on any <seealso cref="Resource#PROCESS_DEFINITION" />.
        /// </exception>
        /// <exception cref="NotValidException">
        ///     When the given period unit is null.
        /// </exception>
        IList<IDurationReportResult> Duration(PeriodUnit periodUnit);
    }
}