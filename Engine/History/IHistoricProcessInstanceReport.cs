using System;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Query;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     <para>Defines a report query for <seealso cref="IHistoricProcessInstance" />s.</para>
    ///     
    /// </summary>
    public interface IHistoricProcessInstanceReport : IReport
    {
        /// <summary>
        ///     Only takes historic process instances into account that were started before the given date.
        /// </summary>
        /// <exception cref="NotValidException">
        ///     if the given started before date is null
        /// </exception>
        IHistoricProcessInstanceReport StartedBefore(DateTime startedBefore);

        /// <summary>
        ///     Only takes historic process instances into account that were started after the given date.
        /// </summary>
        /// <exception cref="NotValidException"> if the given started after date is null </exception>
        IHistoricProcessInstanceReport StartedAfter(DateTime startedAfter);

        /// <summary>
        ///     Only takes historic process instances into account for the given process definition ids.
        /// </summary>
        /// <exception cref="NotValidException"> if one of the given ids is null </exception>
        IHistoricProcessInstanceReport ProcessDefinitionIdIn(params string[] processDefinitionIds);

        /// <summary>
        ///     Only takes historic process instances into account for the given process definition keys.
        /// </summary>
        /// <exception cref="NotValidException"> if one of the given ids is null </exception>
        IHistoricProcessInstanceReport ProcessDefinitionKeyIn(params string[] processDefinitionKeys);
    }
}