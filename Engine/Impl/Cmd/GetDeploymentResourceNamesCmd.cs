using System;
using System.Collections;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{


    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetDeploymentResourceNamesCmd : ICommand<IList<string>>
    {
        private const long SerialVersionUid = 1L;
        protected internal string DeploymentId;

        public GetDeploymentResourceNamesCmd(string deploymentId)
        {
            this.DeploymentId = deploymentId;
        }

        public virtual IList<string> Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("deploymentId", DeploymentId);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadDeployment(DeploymentId);
            return context.Impl.Context.CommandContext.DeploymentManager.GetDeploymentResourceNames(DeploymentId) ;
        }
    }
}