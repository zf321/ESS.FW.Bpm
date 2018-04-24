namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     Represents a statistic which the aggregate number of incidents to
    ///     the corresponding incident type.
    ///     
    /// </summary>
    public interface IIncidentStatistics
    {
        /// <summary>
        ///     Returns the type of the incidents.
        /// </summary>
        string IncidentType { get; }

        /// <summary>
        ///     Returns the number of incidents to the corresponding
        ///     incidentType.
        /// </summary>
        int IncidentCount { get; }
    }
}