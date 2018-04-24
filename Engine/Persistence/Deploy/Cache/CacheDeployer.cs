using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{
    /// <summary>
    /// </summary>
    public class CacheDeployer
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        protected internal IList<IDeployer> deployers;

        public CacheDeployer()
        {
            deployers = new List<IDeployer>();
        }

        public virtual IList<IDeployer> Deployers
        {
            set { deployers = value; }
        }

        public virtual void Deploy(DeploymentEntity deployment)
        {
            Context.CommandContext.RunWithoutAuthorization<object>(() =>
            {
                //包含了BPmn，dmn的DecisionRequirementsDefinition和DecisionDefinition 3种部署，按名Name后缀区分
                foreach (var deployer in deployers)
                    deployer.Deploy(deployment);
                return null;
            });
        }

        public virtual void DeployOnlyGivenResourcesOfDeployment(DeploymentEntity deployment,
            params string[] resourceNames)
        {
            InitDeployment(deployment, resourceNames);
            Context.CommandContext.RunWithoutAuthorization<object>(() =>
            {
                foreach (var deployer in deployers)
                    deployer.Deploy(deployment);
                return null;
            });
        }

        protected internal virtual void InitDeployment(DeploymentEntity deployment, params string[] resourceNames)
        {
            deployment.ClearResources();
            foreach (var resourceName in resourceNames)
                if (resourceName != null)
                {
                    // with the given resource we prevent the deployment of querying
                    // the database which means using all resources that were utilized during the deployment
                    var resource =
                        Context.CommandContext.ResourceManager.FindResourceByDeploymentIdAndResourceName(deployment.Id,
                            resourceName);

                    deployment.AddResource(resource);
                }
        }


    }
}