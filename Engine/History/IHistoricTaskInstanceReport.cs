using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Query;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     .
    /// </summary>
    public interface IHistoricTaskInstanceReport : IReport
    {
        /// <summary>
        ///     <para>
        ///         Sets the completed after date for constraining the query to search for all tasks
        ///         which are completed after a certain date.
        ///     </para>
        /// </summary>
        /// <param name="completedAfter">
        ///     A <seealso cref="Date" /> to define the granularity of the report
        /// </param>
        /// <exception cref="NotValidException">
        ///     When the given date is null.
        /// </exception>
        IHistoricTaskInstanceReport CompletedAfter(DateTime completedAfter);

        /// <summary>
        ///     <para>
        ///         Sets the completed before date for constraining the query to search for all tasks
        ///         which are completed before a certain date.
        ///     </para>
        /// </summary>
        /// <param name="completedBefore">
        ///     A <seealso cref="Date" /> to define the granularity of the report
        /// </param>
        /// <exception cref="NotValidException">
        ///     When the given date is null.
        /// </exception>
        IHistoricTaskInstanceReport CompletedBefore(DateTime completedBefore);

        /// <summary>
        ///     <para>Executes the ITask report query and returns a list of <seealso cref="IHistoricTaskInstanceReportResult" />s</para>
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#READ_HISTORY" /> permission
        ///     on any <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        /// <returns> a list of <seealso cref="IHistoricTaskInstanceReportResult" />s </returns>
        IList<IHistoricTaskInstanceReportResult> CountByProcessDefinitionKey();

        /// <summary>
        ///     <para>Executes the ITask report query and returns a list of <seealso cref="IHistoricTaskInstanceReportResult" />s</para>
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions#READ_HISTORY" /> permission
        ///     on any <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        /// <returns> a list of <seealso cref="IHistoricTaskInstanceReportResult" />s </returns>
        IList<IHistoricTaskInstanceReportResult> CountByTaskName();
    }
}