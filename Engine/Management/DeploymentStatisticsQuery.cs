//using ESS.FW.Bpm.Engine.Query;

//namespace ESS.FW.Bpm.Engine.Management
//{
//    public interface IQueryable<IDeploymentStatistics> : IQuery<IQueryable<IDeploymentStatistics>, IDeploymentStatistics>
//    {
//        /// <summary>
//        ///     Include an aggregation of failed jobs in the result.
//        /// </summary>
//        IQueryable<IDeploymentStatistics> IncludeFailedJobs();

//        /// <summary>
//        ///     Include an aggregation of incidents in the result.
//        /// </summary>
//        IQueryable<IDeploymentStatistics> IncludeIncidents();

//        /// <summary>
//        ///     Include an aggregation of incidents of the assigned incidentType in the result.
//        /// </summary>
//        IQueryable<IDeploymentStatistics> IncludeIncidentsForType(string incidentType);
//    }
//}