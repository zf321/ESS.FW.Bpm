using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     <para>
    ///         A Job Definition provides details about asynchronous background
    ///         processing ("Jobs") performed by the process engine.
    ///     </para>
    ///     <para>
    ///         Each Job Definition corresponds to a Timer or Asynchronous continuation
    ///         job installed in the process engine. Jobs definitions are installed when
    ///         BPMN 2.0 processes containing timer activities or asynchronous continuations
    ///         are deployed.
    ///     </para>
    ///     
    /// </summary>
    public interface IJobDefinition
    {
        /// <returns> the Id of the job definition. </returns>
        string Id { get; }

        /// <returns> the id of the <seealso cref="IProcessDefinition" /> this job definition is associated with. </returns>
        string ProcessDefinitionId { get; }

        /// <returns> the key of the <seealso cref="IProcessDefinition" /> this job definition is associated with. </returns>
        string ProcessDefinitionKey { get; }

        /// <summary>
        ///     The Type of a job. Asynchronous continuation, timer, ...
        /// </summary>
        /// <returns> the type of a Job. </returns>
        string JobType { get; }

        /// <summary>
        ///     The configuration of a job definition provides details about the jobs which will be created.
        ///     For timer jobs this method returns the timer configuration.
        /// </summary>
        /// <returns> the configuration of this job definition. </returns>
        string JobConfiguration { get; }

        /// <summary>
        ///     The Id of the activity (from BPMN 2.0 Xml) this Job Definition is associated with.
        /// </summary>
        /// <returns> the activity id for this Job Definition. </returns>
        string ActivityId { get; }


        /// <summary>
        ///     Indicates whether this job definition is suspended. If a job Definition is suspended,
        ///     No Jobs created form the job definition will be acquired by the job executor.
        /// </summary>
        /// <returns> true if this Job Definition is currently suspended. </returns>
        bool Suspended { get; }

        /// <summary>
        ///     <para>
        ///         Returns the execution priority for jobs of this definition, if it was set using the
        ///         <seealso cref="IManagementService" /> API. When a job is assigned a priority, the job definition's overriding
        ///         priority (if set) is used instead of the values defined in the BPMN XML.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     the priority that overrides the default/BPMN XML priority or <code>null</code> if
        ///     no overriding priority is set
        ///     
        /// </returns>
        long? OverridingJobPriority { get; }

        /// <summary>
        ///     The id of the tenant this job definition belongs to. Can be <code>null</code>
        ///     if the definition belongs to no single tenant.
        /// </summary>
        string TenantId { get; }
        
        int SuspensionState { get; set; }
    }
}