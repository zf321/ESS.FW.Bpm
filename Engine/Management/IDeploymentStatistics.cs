using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Management
{
    public interface IDeploymentStatistics : IDeployment
    {
        /// <summary>
        ///     The number of all process instances of the process definitions contained in this deployment.
        /// </summary>
        int Instances { get; }

        /// <summary>
        ///     The number of all failed jobs of process instances of definitions contained in this deployment.
        /// </summary>
        int FailedJobs { get; }

        /// <summary>
        ///     Returns a list of incident statistics.
        /// </summary>
        IList<IIncidentStatistics> IncidentStatistics { get; }
    }
}