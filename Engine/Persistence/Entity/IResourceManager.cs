using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IResourceManager
    {
        void DeleteResourcesByDeploymentId(string deploymentId);
        IDictionary<string, ResourceEntity> FindLatestResourcesByDeploymentName(string deploymentName, ISet<string> resourcesToFind, string source, string tenantId);
        ResourceEntity FindResourceByDeploymentIdAndResourceId(string deploymentId, string resourceId);
        IList<ResourceEntity> FindResourceByDeploymentIdAndResourceIds(string deploymentId, params string[] resourceIds);
        ResourceEntity FindResourceByDeploymentIdAndResourceName(string deploymentId, string resourceName);
        IList<ResourceEntity> FindResourceByDeploymentIdAndResourceNames(string deploymentId, params string[] resourceNames);
        IList<ResourceEntity> FindResourcesByDeploymentId(string deploymentId);
        void InsertResource(ResourceEntity resource);
    }
}