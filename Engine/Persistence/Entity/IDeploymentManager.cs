using ESS.FW.DataAccess;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IDeploymentManager:IRepository<DeploymentEntity,string>
    {
        void DeleteDeployment(string deploymentId, bool cascade);
        void DeleteDeployment(string deploymentId, bool cascade, bool skipCustomListeners);
        DeploymentEntity FindDeploymentById(string deploymentId);
        IList<DeploymentEntity> FindDeploymentsByIds(params string[] deploymentsIds);
        DeploymentEntity FindLatestDeploymentByName(string deploymentName);
        IList<string> GetDeploymentResourceNames(string deploymentId);
        void InsertDeployment(DeploymentEntity deployment);
    }
}