using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    public class GetDeployedProcessDefinitionCmd : ICommand<ProcessDefinitionEntity>
    {
        protected internal readonly bool CheckReadPermission;
        protected internal bool IsTenantIdSet;

        protected internal string ProcessDefinitionId;
        protected internal string ProcessDefinitionKey;

        protected internal string ProcessDefinitionTenantId;

        public GetDeployedProcessDefinitionCmd(string processDefinitionId, bool checkReadPermission)
        {
            this.ProcessDefinitionId = processDefinitionId;
            this.CheckReadPermission = checkReadPermission;
        }

        public GetDeployedProcessDefinitionCmd(ProcessInstantiationBuilderImpl instantiationBuilder,
            bool checkReadPermission)
        {
            ProcessDefinitionId = instantiationBuilder.ProcessDefinitionId;
            ProcessDefinitionKey = instantiationBuilder.ProcessDefinitionKey;
            ProcessDefinitionTenantId = instantiationBuilder.ProcessDefinitionTenantId;
            IsTenantIdSet = instantiationBuilder.IsTenantIdSet;
            this.CheckReadPermission = checkReadPermission;
        }
        /// <summary>
        /// 查询已部署的流程定义
        /// </summary>
        /// <param name="commandContext"></param>
        /// <returns></returns>
        public virtual ProcessDefinitionEntity Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureOnlyOneNotNull("either process definition id or key must be set", ProcessDefinitionId,
                ProcessDefinitionKey);

            var processDefinition = Find(commandContext);

            if (CheckReadPermission)
            {
                foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                {
                    checker.CheckReadProcessDefinition(processDefinition);
                }
            }

            return processDefinition;
        }

        protected internal virtual ProcessDefinitionEntity Find(CommandContext commandContext)
        {
            DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;

            if (ProcessDefinitionId!= null)
            {
                return FindById(deploymentCache, ProcessDefinitionId);
            }
            return FindByKey(deploymentCache, ProcessDefinitionKey);
        }

        protected internal virtual ProcessDefinitionEntity FindById(DeploymentCache deploymentCache,
            string processDefinitionId)
        {
            return deploymentCache.FindDeployedProcessDefinitionById(processDefinitionId);
        }

        protected internal virtual ProcessDefinitionEntity FindByKey(DeploymentCache deploymentCache,
            string processDefinitionKey)
        {
            if (IsTenantIdSet)
            {
                return deploymentCache.FindDeployedLatestProcessDefinitionByKeyAndTenantId(processDefinitionKey,
                    ProcessDefinitionTenantId);
            }
            return deploymentCache.FindDeployedLatestProcessDefinitionByKey(processDefinitionKey);
        }
    }
}