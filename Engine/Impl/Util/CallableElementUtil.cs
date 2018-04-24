using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///     
    /// </summary>
    public class CallableElementUtil
    {
        public static DeploymentCache DeploymentCache
        {
            get { return Context.ProcessEngineConfiguration.DeploymentCache; }
        }

        public static ProcessDefinitionImpl GetProcessDefinitionToCall(IVariableScope execution,
            BaseCallableElement callableElement)
        {
            var processDefinitionKey = callableElement.GetDefinitionKey(execution);
            var tenantId = callableElement.GetDefinitionTenantId(execution);

            DeploymentCache deploymentCache = DeploymentCache;

            ProcessDefinitionImpl processDefinition = null;

            if (callableElement.LatestBinding)
            {
                processDefinition =
                    deploymentCache.FindDeployedLatestProcessDefinitionByKeyAndTenantId(processDefinitionKey, tenantId);
            }
            else if (callableElement.DeploymentBinding)
            {
                var deploymentId = callableElement.DeploymentId;
                processDefinition = deploymentCache.FindDeployedProcessDefinitionByDeploymentAndKey(deploymentId,
                    processDefinitionKey);
            }
            else if (callableElement.VersionBinding)
            {
                var version = callableElement.GetVersion(execution);
                processDefinition =
                    deploymentCache.FindDeployedProcessDefinitionByKeyVersionAndTenantId(processDefinitionKey, version,
                        tenantId);
            }

            return processDefinition;
        }

        //public static CmmnCaseDefinition getCaseDefinitionToCall(IVariableScope execution, BaseCallableElement callableElement)
        //{
        //    string caseDefinitionKey = callableElement.getDefinitionKey(execution);
        //    string tenantId = callableElement.getDefinitionTenantId(execution);

        //    DeploymentCache deploymentCache = DeploymentCache;

        //    CmmnCaseDefinition caseDefinition = null;
        //    if (callableElement.LatestBinding)
        //    {
        //        caseDefinition = deploymentCache.findDeployedLatestCaseDefinitionByKeyAndTenantId(caseDefinitionKey, tenantId);

        //    }
        //    else if (callableElement.DeploymentBinding)
        //    {
        //        string deploymentId = callableElement.DeploymentId;
        //        caseDefinition = deploymentCache.findDeployedCaseDefinitionByDeploymentAndKey(deploymentId, caseDefinitionKey);

        //    }
        //    else if (callableElement.VersionBinding)
        //    {
        //        int? version = callableElement.getVersion(execution);
        //        caseDefinition = deploymentCache.findDeployedCaseDefinitionByKeyVersionAndTenantId(caseDefinitionKey, version, tenantId);
        //    }

        //    return caseDefinition;
        //}

        public static IDecisionDefinition GetDecisionDefinitionToCall(IVariableScope execution,
            BaseCallableElement callableElement)
        {
            var decisionDefinitionKey = callableElement.GetDefinitionKey(execution);
            var tenantId = callableElement.GetDefinitionTenantId(execution);

            DeploymentCache deploymentCache = DeploymentCache;

            IDecisionDefinition decisionDefinition = null;
            if (callableElement.LatestBinding)
            {
                decisionDefinition =
                    deploymentCache.FindDeployedLatestDecisionDefinitionByKeyAndTenantId(decisionDefinitionKey, tenantId);
            }
            else if (callableElement.DeploymentBinding)
            {
                var deploymentId = callableElement.DeploymentId;
                decisionDefinition = deploymentCache.FindDeployedDecisionDefinitionByDeploymentAndKey(deploymentId,
                    decisionDefinitionKey);
            }
            else if (callableElement.VersionBinding)
            {
                var version = callableElement.GetVersion(execution);
                decisionDefinition =
                    deploymentCache.FindDeployedDecisionDefinitionByKeyVersionAndTenantId(decisionDefinitionKey, version,
                        tenantId);
            }

            return decisionDefinition;
        }
    }
}