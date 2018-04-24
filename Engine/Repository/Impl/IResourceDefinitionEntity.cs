namespace ESS.FW.Bpm.Engine.Repository.Impl
{
    /// <summary>
    ///     Entity of a deployed resource definition
    /// </summary>
    public interface IResourceDefinitionEntity : IResourceDefinition
    {
        string Id { get; set; }

        string Category { get; set; }

        string Name { get; set; }

        string Key { get; set; }

        int Version { get; set; }

        string ResourceName { get; set; }

        string DeploymentId { get; set; }

        string DiagramResourceName { get; set; }

        string TenantId { get; set; }

        IResourceDefinitionEntity PreviousDefinition { get; }
    }
}