//using ESS.FW.Bpm.Engine.Query;

//namespace ESS.FW.Bpm.Engine.Management
//{
//    public interface IQueryable<IActivityStatistics> : IQuery<IQueryable<IActivityStatistics>, IActivityStatistics>
//    {
//        /// <summary>
//        ///     Include an aggregation of failed jobs in the result.
//        /// </summary>
//        IQueryable<IActivityStatistics> IncludeFailedJobs();

//        /// <summary>
//        ///     Include an aggregation of incidents in the result.
//        /// </summary>
//        IQueryable<IActivityStatistics> IncludeIncidents();

//        /// <summary>
//        ///     Include an aggregation of incidents of the assigned incidentType in the result.
//        /// </summary>
//        IQueryable<IActivityStatistics> IncludeIncidentsForType(string incidentType);
//    }
//}