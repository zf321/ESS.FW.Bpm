using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Management
{
    public interface IProcessDefinitionStatistics : IProcessDefinition
    {
        /// <summary>
        ///     The number of all process instances of the process definition.
        /// </summary>
        int Instances { get; }

        /// <summary>
        ///     The number of all failed jobs of all process instances.
        /// </summary>
        int FailedJobs { get; }

        /// <summary>
        ///     Returns a list of incident statistics.
        /// </summary>
        IList<IIncidentStatistics> IncidentStatistics { get; }
    }
}