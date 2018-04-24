using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cfg.Auth;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Repository;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.DataAccess.EF;
using ESS.FW.Common.Components;
using org.camunda.bpm.engine.impl.history.@event;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{


    /// <summary>
    ///  
    /// 
    /// 
    /// </summary>
    [Component]
    public class DeploymentManager : AbstractManagerNet<DeploymentEntity>, IDeploymentManager
    {
        private readonly IResourceManager _resourceManager;
        private readonly IProcessDefinitionManager _processDefinitionManager;
        private readonly IExecutionManager _executionManager;
        private readonly IHistoricJobLogManager _historicJobLogManager;
        private readonly IDecisionDefinitionManager _decisionDefinitionManager;
        private readonly IHistoricDecisionInstanceManager _historicDecisionInstanceManager;
        public DeploymentManager(DbContext dbContex,
            ILoggerFactory loggerFactory,
            IDGenerator idGenerator,
            IResourceManager resourceManager,
            IProcessDefinitionManager processDefinitionManager,
            IExecutionManager executionManager,
            IDecisionDefinitionManager decisionDefinitionManager,
            IHistoricJobLogManager historicJobLogManager,
            IHistoricDecisionInstanceManager historicDecisionInstanceManager
            ) : base(dbContex, loggerFactory, idGenerator)
        {
            _resourceManager = resourceManager;
            _processDefinitionManager = processDefinitionManager;
            _executionManager = executionManager;
            _historicJobLogManager = historicJobLogManager;
            _decisionDefinitionManager = decisionDefinitionManager;
            _historicDecisionInstanceManager = historicDecisionInstanceManager;
        }

        public virtual void InsertDeployment(DeploymentEntity deployment)
        {
            Add(deployment);
            CreateDefaultAuthorizations(deployment);

            foreach (ResourceEntity resource in deployment.Resources.Values)
            {
                resource.DeploymentId = deployment.Id;
                _resourceManager.InsertResource(resource);
            }

            context.Impl.Context.ProcessEngineConfiguration.DeploymentCache.Deploy(deployment);
        }

        public virtual void DeleteDeployment(string deploymentId, bool cascade)
        {
            DeleteDeployment(deploymentId, cascade, false);
        }

        public virtual void DeleteDeployment(string deploymentId, bool cascade, bool skipCustomListeners)
        {
            IList<ProcessDefinitionEntity> processDefinitions = _processDefinitionManager.FindProcessDefinitionsByDeploymentId(deploymentId);
            if (cascade)
            {
                // *NOTE*:
                // The process instances of ALL process definitions must be
                // deleted, before every process definition can be deleted!
                //
                // On deletion of all process instances, the task listeners will
                // be deleted as well. Deletion of tasks and listeners needs
                // the redeployment of deployments, which can cause to problems if
                // is done sequential with deletion of process definition.
                //
                // For example:
                // Deployment contains two process definiton. First process definition
                // and instances will be removed, also cleared from the cache.
                // Second process definition will be removed and his instances.
                // Deletion of instances will cause redeployment this deploys again
                // first into the cache. Only the second will be removed from cache and
                // first remains in the cache after the deletion process.
                //
                // Thats why we have to clear up all instances at first, after that
                // we can cleanly remove the process definitions.
                foreach (ProcessDefinitionEntity processDefinition in processDefinitions)
                {
                    string processDefinitionId = processDefinition.Id;
                    _executionManager.DeleteProcessInstancesByProcessDefinition(processDefinitionId, "deleted deployment", true, skipCustomListeners);
                }
                // delete historic job logs (for example for timer start event jobs)
                _historicJobLogManager.DeleteHistoricJobLogsByDeploymentId(deploymentId);
            }

            foreach (ProcessDefinitionEntity processDefinition in processDefinitions)
            {
                string processDefinitionId = processDefinition.Id;
                // Process definition cascade true deletes the history and
                // process instances if instances flag is set as well to true.
                // Problem as described above, redeployes the deployment.
                // Represents no problem if only one process definition is deleted
                // in a transaction! We have to set the instances flag to false.
                _processDefinitionManager.DeleteProcessDefinition(processDefinition, processDefinitionId, cascade, false, skipCustomListeners);
            }

            DeleteCaseDeployment(deploymentId, cascade);

            DeleteDecisionDeployment(deploymentId, cascade);
            DeleteDecisionRequirementDeployment(deploymentId);

            _resourceManager.DeleteResourcesByDeploymentId(deploymentId);

            DeleteAuthorizations(Resources.Deployment, deploymentId);
            Delete(deploymentId);
        }

        protected internal virtual void DeleteCaseDeployment(string deploymentId, bool cascade)
        {

            ProcessEngineConfigurationImpl processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            if (processEngineConfiguration.CmmnEnabled)
            {
                throw new NotImplementedException("不启用Cmmn");
                //IList<ICaseDefinition> caseDefinitions = CaseDefinitionManager.FindCaseDefinitionByDeploymentId(deploymentId);

                //if (cascade)
                //{

                //    // delete case instances
                //    foreach (ICaseDefinition caseDefinition in caseDefinitions)
                //    {
                //        string caseDefinitionId = caseDefinition.Id;

                //        //CaseInstanceManager.DeleteCaseInstancesByCaseDefinition(caseDefinitionId, "deleted deployment", true);

                //    }
                //}

                //// delete case definitions from db
                ////CaseDefinitionManager.DeleteCaseDefinitionsByDeploymentId(deploymentId);

                //foreach (ICaseDefinition caseDefinition in caseDefinitions)
                //{
                //    string processDefinitionId = caseDefinition.Id;

                //    // remove case definitions from cache:
                //    context.Impl.Context.ProcessEngineConfiguration.DeploymentCache.RemoveCaseDefinition(processDefinitionId);
                //}
            }
        }

        protected internal virtual void DeleteDecisionDeployment(string deploymentId, bool cascade)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            if (processEngineConfiguration.DmnEnabled)
            {
                IList<IDecisionDefinition> decisionDefinitions = _decisionDefinitionManager.FindDecisionDefinitionByDeploymentId(deploymentId);

                if (cascade)
                {
                    // delete historic decision instances
                    foreach (IDecisionDefinition decisionDefinition in decisionDefinitions)
                    {
                        _historicDecisionInstanceManager.DeleteHistoricDecisionInstancesByDecisionDefinitionId(decisionDefinition.Id);
                    }
                }

                // delete decision definitions from db
                _decisionDefinitionManager.DeleteDecisionDefinitionsByDeploymentId(deploymentId);

                DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

                foreach (IDecisionDefinition decisionDefinition in decisionDefinitions)
                {
                    string decisionDefinitionId = decisionDefinition.Id;

                    // remove decision definitions from cache:
                    deploymentCache.RemoveDecisionDefinition(decisionDefinitionId);
                }
            }
        }

        protected internal virtual void DeleteDecisionRequirementDeployment(string deploymentId)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            if (processEngineConfiguration.DmnEnabled)
            {
                IDecisionRequirementsDefinitionManager manager = CommandContext.DecisionRequirementsDefinitionManager;
                IList<IDecisionRequirementsDefinition> decisionRequirementsDefinitions = manager.FindDecisionRequirementsDefinitionByDeploymentId(deploymentId);

                // delete decision requirements definitions from db
                manager.DeleteDecisionRequirementsDefinitionsByDeploymentId(deploymentId);

                DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

                foreach (IDecisionRequirementsDefinition decisionRequirementsDefinition in decisionRequirementsDefinitions)
                {
                    string decisionDefinitionId = decisionRequirementsDefinition.Id;

                    // remove decision requirements definitions from cache:
                    deploymentCache.RemoveDecisionRequirementsDefinition(decisionDefinitionId);
                }

            }
        }

        public virtual DeploymentEntity FindLatestDeploymentByName(string deploymentName)
        {
            //JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
            //ORIGINAL LINE: java.Util.List<?> list = getDbEntityManager().selectList("selectDeploymentsByName", deploymentName, 0, 1);
            //IList list = Find("selectDeploymentsByName", deploymentName, 0, 1);

            //if (list != null && list.Count > 0)
            //{
            //  return (DeploymentEntity) list[0];
            //}
            //return null;
            //重写查询
            return Find(m => m.Name == deploymentName).OrderByDescending(m => m.DeploymentTime).FirstOrDefault();
        }

        public virtual DeploymentEntity FindDeploymentById(string deploymentId)
        {
            //return Get<DeploymentEntity>(typeof(DeploymentEntity), deploymentId);
            return Get(deploymentId);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<DeploymentEntity> findDeploymentsByIds(String... deploymentsIds)
        public virtual IList<DeploymentEntity> FindDeploymentsByIds(params string[] deploymentsIds)
        {
            //return Find("selectDeploymentsByIds", deploymentsIds);
            return Find(m => deploymentsIds.Contains(m.Id)).ToList();
        }

        //public virtual long FindDeploymentCountByQueryCriteria(DeploymentQueryImpl deploymentQuery)
        //{

        //    ConfigureQuery(deploymentQuery);
        //    return (long?)First("selectDeploymentCountByQueryCriteria", deploymentQuery);
        //}

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.repository.Deployment> findDeploymentsByQueryCriteria(org.camunda.bpm.engine.impl.DeploymentQueryImpl deploymentQuery, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IDeployment> FindDeploymentsByQueryCriteria(DeploymentQueryImpl deploymentQuery, Page page)
        //{
        //    throw new NotImplementedException();
        //    ConfigureQuery(deploymentQuery);
        //    //return Find("selectDeploymentsByQueryCriteria", deploymentQuery, page);
        //}

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<String> getDeploymentResourceNames(String deploymentId)
        public virtual IList<string> GetDeploymentResourceNames(string deploymentId)
        {
            //return commons.ListExt.ConvertToListT<string>( Find("selectResourceNamesByDeploymentId", deploymentId));
            return First(m => m.Id == deploymentId).Resources.Select(m => m.Key).ToList();
        }

        //public override void Close()
        //{
        //}

        //public override void Flush()
        //{
        //    //DbEntityManager.Flush();
        //}

        // helper /////////////////////////////////////////////////

        protected internal virtual void CreateDefaultAuthorizations(DeploymentEntity deployment)
        {
            if (AuthorizationEnabled)
            {
                IResourceAuthorizationProvider provider = ResourceAuthorizationProvider;
                AuthorizationEntity[] authorizations = provider.NewDeployment(deployment);
                SaveDefaultAuthorizations(authorizations);
            }

        }

        //protected internal virtual void ConfigureQuery(DeploymentQueryImpl query)
        //{
        //    AuthorizationManager.ConfigureDeploymentQuery(query);
        //    TenantManager.configureQuery(query);
        //}

    }

}