using System;
using System.IO;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetDeploymentResourceCmd : ICommand<Stream>
    {
        private const long SerialVersionUid = 1L;
        protected internal string DeploymentId;
        protected internal string ResourceName;

        public GetDeploymentResourceCmd(string deploymentId, string resourceName)
        {
            this.DeploymentId = deploymentId;
            this.ResourceName = resourceName;
        }

        public virtual Stream Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("deploymentId", DeploymentId);
            EnsureUtil.EnsureNotNull("resourceName", ResourceName);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadDeployment(DeploymentId);

            ResourceEntity resource = commandContext.ResourceManager.FindResourceByDeploymentIdAndResourceName(DeploymentId, ResourceName);
            EnsureUtil.EnsureNotNull(typeof(DeploymentResourceNotFoundException), "no resource found with name '" + ResourceName + "' in deployment '" + DeploymentId + "'", "resource", resource);
            return new System.IO.MemoryStream(resource.Bytes);
        }
    }
}