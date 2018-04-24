//using ESS.FW.Bpm.Engine.Query;

//namespace ESS.FW.Bpm.Engine.Management
//{
//    public interface IQueryable<IProcessDefinitionStatistics> :
//        IQuery<IQueryable<IProcessDefinitionStatistics>, IProcessDefinitionStatistics>
//    {
//        /// <summary>
//        ///     Include an aggregation of failed jobs in the result.
//        /// </summary>
//        IQueryable<IProcessDefinitionStatistics> IncludeFailedJobs();

//        /// <summary>
//        ///     Include an aggregation of incidents in the result.
//        /// </summary>
//        IQueryable<IProcessDefinitionStatistics> IncludeIncidents();

//        /// <summary>
//        ///     Include an aggregation of incidents of the assigned incidentType in the result.
//        /// </summary>
//        IQueryable<IProcessDefinitionStatistics> IncludeIncidentsForType(string incidentType);
//    }
//}