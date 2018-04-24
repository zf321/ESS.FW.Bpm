using System;
using System.Collections;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{


    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetDeploymentResourcesCmd : ICommand<IList>
    {
        private const long SerialVersionUid = 1L;
        protected internal string DeploymentId;

        public GetDeploymentResourcesCmd(string deploymentId)
        {
            this.DeploymentId = deploymentId;
        }

        public virtual IList Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("deploymentId", DeploymentId);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadDeployment(DeploymentId);
            return ListExt.ConvertToIlist(context.Impl.Context.CommandContext.ResourceManager.FindResourcesByDeploymentId(DeploymentId));
        }
    }
}