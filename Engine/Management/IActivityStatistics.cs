using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Management
{
    public interface IActivityStatistics
    {
        /// <summary>
        ///     The activity id.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The number of all instances of the activity.
        /// </summary>
        int Instances { get; }

        /// <summary>
        ///     The number of all failed jobs for the activity.
        /// </summary>
        int FailedJobs { get; }

        /// <summary>
        ///     Returns a list of incident statistics.
        /// </summary>
        IList<IIncidentStatistics> IncidentStatistics { get; }
    }
}