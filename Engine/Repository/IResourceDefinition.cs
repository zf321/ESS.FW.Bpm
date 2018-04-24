namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Definition of a resource which was deployed
    /// </summary>
    public interface IResourceDefinition
    {
        /// <summary>
        ///     unique identifier
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     category name which is derived from the targetNamespace attribute in the definitions element
        /// </summary>
        string Category { get; }

        /// <summary>
        ///     label used for display purposes
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     unique name for all versions this definition
        /// </summary>
        string Key { get; }

        /// <summary>
        ///     version of this definition
        /// </summary>
        int Version { get; }

        /// <summary>
        ///     name of <seealso cref="IRepositoryService#getResourceAsStream(String, String) the resource" /> of this definition
        /// </summary>
        string ResourceName { get; }

        /// <summary>
        ///     The deployment in which this definition is contained.
        /// </summary>
        string DeploymentId { get; }

        /// <summary>
        ///     The diagram resource name for this definition if exist
        /// </summary>
        string DiagramResourceName { get; }

        /// <summary>
        ///     The id of the tenant this definition belongs to. Can be <code>null</code>
        ///     if the definition belongs to no single tenant.
        /// </summary>
        string TenantId { get; }

        int HistoryTimeToLive { get; }
    }
}