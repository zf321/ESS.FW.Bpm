//using ESS.FW.Bpm.Engine.Query;

//namespace ESS.FW.Bpm.Engine.Management
//{
//    /// <summary>
//    ///     Allows programmatic querying of <seealso cref="IJobDefinition" />s.
//    ///     
//    /// </summary>
//    public interface IQueryable<IJobDefinition> : IQuery<IQueryable<IJobDefinition>, IJobDefinition>
//    {
//        /// <summary>
//        ///     Only select job definitions with the given id
//        /// </summary>
//        IQueryable<IJobDefinition> JobDefinitionId(string jobDefinitionId);

//        /// <summary>
//        ///     Only select job definitions which exist for the listed activity ids
//        /// </summary>
//        IQueryable<IJobDefinition> ActivityIdIn(params string[] activityIds);

//        /// <summary>
//        ///     Only select job definitions which exist for the given process definition id.
//        /// </summary>
//        IQueryable<IJobDefinition> ProcessDefinitionId(string processDefinitionId);

//        /// <summary>
//        ///     Only select job definitions which exist for the given process definition key.
//        /// </summary>
//        IQueryable<IJobDefinition> ProcessDefinitionKey(string processDefinitionKey);

//        /// <summary>
//        ///     Only select job definitions which have the given job type.
//        /// </summary>
//        IQueryable<IJobDefinition> JobType(string jobType);

//        /// <summary>
//        ///     Only select job definitions which contain the configuration.
//        /// </summary>
//        IQueryable<IJobDefinition> JobConfiguration(string jobConfiguration);

//        /// <summary>
//        ///     Only selects job definitions which are active
//        /// </summary>
//        IQueryable<IJobDefinition> Active();

//        /// <summary>
//        ///     Only selects job definitions which are suspended
//        /// </summary>
//        IQueryable<IJobDefinition> Suspended();

//        /// <summary>
//        ///     Only selects job definitions which have a job priority defined.
//        ///     
//        /// </summary>
//        IQueryable<IJobDefinition> WithOverridingJobPriority();

//        /// <summary>
//        ///     Only select job definitions that belong to one of the given tenant ids.
//        /// </summary>
//        IQueryable<IJobDefinition> TenantIdIn(params string[] tenantIds);

//        /// <summary>
//        ///     Only select job definitions which have no tenant id.
//        /// </summary>
//        IQueryable<IJobDefinition> WithoutTenantId();

//        /// <summary>
//        ///     Select job definitions which have no tenant id. Can be used in combination
//        ///     with <seealso cref="#tenantIdIn(String...)" />.
//        /// </summary>
//        IQueryable<IJobDefinition> IncludeJobDefinitionsWithoutTenantId();

//        /// <summary>
//        ///     Order by id (needs to be followed by <seealso cref="#asc()" /> or <seealso cref="#desc()" />).
//        /// </summary>
//        IQueryable<IJobDefinition> OrderByJobDefinitionId();

//        /// <summary>
//        ///     Order by activty id (needs to be followed by <seealso cref="#asc()" /> or <seealso cref="#desc()" />).
//        /// </summary>
//        IQueryable<IJobDefinition> OrderByActivityId();

//        /// <summary>
//        ///     Order by process defintion id (needs to be followed by <seealso cref="#asc()" /> or <seealso cref="#desc()" />).
//        /// </summary>
//        IQueryable<IJobDefinition> OrderByProcessDefinitionId();

//        /// <summary>
//        ///     Order by process definition key (needs to be followed by <seealso cref="#asc()" /> or <seealso cref="#desc()" />).
//        /// </summary>
//        IQueryable<IJobDefinition> OrderByProcessDefinitionKey();

//        /// <summary>
//        ///     Order by job type (needs to be followed by <seealso cref="#asc()" /> or <seealso cref="#desc()" />).
//        /// </summary>
//        IQueryable<IJobDefinition> OrderByJobType();

//        /// <summary>
//        ///     Order by job configuration (needs to be followed by <seealso cref="#asc()" /> or <seealso cref="#desc()" />).
//        /// </summary>
//        IQueryable<IJobDefinition> OrderByJobConfiguration();

//        /// <summary>
//        ///     Order by tenant id (needs to be followed by <seealso cref="#asc()" /> or <seealso cref="#desc()" />).
//        ///     Note that the ordering of job definitions without tenant id is database-specific.
//        /// </summary>
//        IQueryable<IJobDefinition> OrderByTenantId();
//    }
//}