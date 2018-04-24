using System;
using System.IO;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetDeploymentResourceForIdCmd : ICommand<Stream>
    {
        private const long SerialVersionUid = 1L;
        protected internal string DeploymentId;
        protected internal string ResourceId;

        public GetDeploymentResourceForIdCmd(string deploymentId, string resourceId)
        {
            this.DeploymentId = deploymentId;
            this.ResourceId = resourceId;
        }

        public virtual Stream Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("deploymentId", DeploymentId);
            EnsureUtil.EnsureNotNull("resourceId", ResourceId);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadDeployment(DeploymentId);

            ResourceEntity resource = commandContext.ResourceManager.FindResourceByDeploymentIdAndResourceId(DeploymentId, ResourceId);
            EnsureUtil.EnsureNotNull("no resource found with id '" + ResourceId + "' in deployment '" + DeploymentId + "'", "resource", resource);
            return new System.IO.MemoryStream(resource.Bytes);
        }
    }
}