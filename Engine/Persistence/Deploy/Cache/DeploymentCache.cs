using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common.Cache;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{
    /// <summary>
    ///     
    /// </summary>
    public class DeploymentCache
    {
        private readonly BpmnModelInstanceCache _bpmnModelInstanceCache;
        private readonly CacheDeployer _cacheDeployer = new CacheDeployer();
        private readonly CaseDefinitionCache _caseDefinitionCache;
        private readonly DecisionDefinitionCache _decisionDefinitionCache;
        private readonly DecisionRequirementsDefinitionCache _decisionRequirementsDefinitionCache;
        //protected internal CmmnModelInstanceCache CmmnModelInstanceCache;
        private readonly DmnModelInstanceCache _dmnModelInstanceCache;

        private readonly ProcessDefinitionCache _processDefinitionEntityCache;

        public DeploymentCache(ICacheFactory factory, int cacheCapacity)
        {
            //
            _processDefinitionEntityCache = new ProcessDefinitionCache(factory, cacheCapacity, _cacheDeployer);
            _caseDefinitionCache = new CaseDefinitionCache(factory, cacheCapacity, _cacheDeployer);
            _decisionDefinitionCache = new DecisionDefinitionCache(factory, cacheCapacity, _cacheDeployer);
            _decisionRequirementsDefinitionCache = new DecisionRequirementsDefinitionCache(factory, cacheCapacity,
                _cacheDeployer);

            _bpmnModelInstanceCache = new BpmnModelInstanceCache(factory, cacheCapacity, _processDefinitionEntityCache);
            //cmmnModelInstanceCache = new CmmnModelInstanceCache(factory, cacheCapacity, caseDefinitionCache);
            _dmnModelInstanceCache = new DmnModelInstanceCache(factory, cacheCapacity, _decisionDefinitionCache);
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual ICache<string, IBpmnModelInstance> BpmnModelInstanceCache
        {
            get { return _bpmnModelInstanceCache.Cache; }
        }

        ////public virtual ICache<string, CmmnModelInstance> CmmnModelInstanceCache
        ////{
        //// get
        //// {
        //return cmmnModelInstanceCache.Cache;
        //// }
        ////}

        public virtual ICache<string, IDmnModelInstance> DmnDefinitionCache
        {
            get { return _dmnModelInstanceCache.Cache; }
        }

        public virtual ICache<string, DecisionDefinitionEntity> DecisionDefinitionCache
        {
            get { return _decisionDefinitionCache.Cache; }
        }

        public virtual ICache<string, DecisionRequirementsDefinitionEntity> DecisionRequirementsDefinitionCache
        {
            get { return _decisionRequirementsDefinitionCache.Cache; }
        }

        public virtual ICache<string, ProcessDefinitionEntity> ProcessDefinitionCache
        {
            get { return _processDefinitionEntityCache.Cache; }
        }

        public virtual IList<IDeployer> Deployers
        {
            set { _cacheDeployer.Deployers = value; }
        }

        public virtual void Deploy(DeploymentEntity deployment)
        {
            _cacheDeployer.Deploy(deployment);
        }

        // PROCESS DEFINITION ////////////////////////////////////////////////////////////////////////////////

        public virtual ProcessDefinitionEntity FindProcessDefinitionFromCache(string processDefinitionId)
        {
            return _processDefinitionEntityCache.FindDefinitionFromCache(processDefinitionId);
        }

        public virtual ProcessDefinitionEntity FindDeployedProcessDefinitionById(string processDefinitionId)
        {
            //
            return _processDefinitionEntityCache.FindDeployedDefinitionById(processDefinitionId);
        }

        /// <returns> the latest version of the process definition with the given key (from any tenant) </returns>
        /// <exception cref="ProcessEngineException"> if more than one tenant has a process definition with the given key </exception>
        /// <seealso cref= # findDeployedLatestProcessDefinitionByKeyAndTenantId( String, String
        /// )
        /// </seealso>
        public virtual ProcessDefinitionEntity FindDeployedLatestProcessDefinitionByKey(string processDefinitionKey)
        {
            return _processDefinitionEntityCache.FindDeployedLatestDefinitionByKey(processDefinitionKey);
        }

        /// <returns> the latest version of the process definition with the given key and tenant id </returns>
        public virtual ProcessDefinitionEntity FindDeployedLatestProcessDefinitionByKeyAndTenantId(
            string processDefinitionKey, string tenantId)
        {
            return _processDefinitionEntityCache.FindDeployedLatestDefinitionByKeyAndTenantId(processDefinitionKey,
                tenantId);
        }

        public virtual ProcessDefinitionEntity FindDeployedProcessDefinitionByKeyVersionAndTenantId(
            string processDefinitionKey, int? processDefinitionVersion, string tenantId)
        {
            return _processDefinitionEntityCache.FindDeployedDefinitionByKeyVersionAndTenantId(processDefinitionKey,
                processDefinitionVersion, tenantId);
        }

        public virtual ProcessDefinitionEntity FindDeployedProcessDefinitionByDeploymentAndKey(string deploymentId,
            string processDefinitionKey)
        {
            return _processDefinitionEntityCache.FindDeployedDefinitionByDeploymentAndKey(deploymentId,
                processDefinitionKey);
        }

        public virtual ProcessDefinitionEntity ResolveProcessDefinition(ProcessDefinitionEntity processDefinition)
        {
            return _processDefinitionEntityCache.ResolveDefinition(processDefinition);
        }

        public virtual IBpmnModelInstance FindBpmnModelInstanceForProcessDefinition(
            ProcessDefinitionEntity processDefinitionEntity)
        {
            return _bpmnModelInstanceCache.FindBpmnModelInstanceForDefinition(processDefinitionEntity);
        }

        public virtual IBpmnModelInstance FindBpmnModelInstanceForProcessDefinition(string processDefinitionId)
        {
            return _bpmnModelInstanceCache.FindBpmnModelInstanceForDefinition(processDefinitionId);
        }

        public virtual void AddProcessDefinition(ProcessDefinitionEntity processDefinition)
        {
            //
            _processDefinitionEntityCache.AddDefinition(processDefinition);
        }

        public virtual void RemoveProcessDefinition(string processDefinitionId)
        {
            _processDefinitionEntityCache.RemoveDefinitionFromCache(processDefinitionId);
            _bpmnModelInstanceCache.Remove(processDefinitionId);
        }

        public virtual void DiscardProcessDefinitionCache()
        {
            _processDefinitionEntityCache.Clear();
            _bpmnModelInstanceCache.Clear();
        }

        // CASE DEFINITION ////////////////////////////////////////////////////////////////////////////////

        //public virtual CaseDefinitionEntity FindCaseDefinitionFromCache(string caseDefinitionId)
        //{

        //    return CaseDefinitionCache.FindDefinitionFromCache(caseDefinitionId);
        //}

        //public virtual CaseDefinitionEntity FindDeployedCaseDefinitionById(string caseDefinitionId)
        //{
        //         
        //         return caseDefinitionCache.FindDeployedDefinitionById(caseDefinitionId);
        //}

        ///// <returns> the latest version of the case definition with the given key (from any tenant) </returns>
        ///// <exception cref="ProcessEngineException"> if more than one tenant has a case definition with the given key </exception>
        ///// <seealso cref= #findDeployedLatestCaseDefinitionByKeyAndTenantId(String, String) </seealso>
        //public virtual CaseDefinitionEntity FindDeployedLatestCaseDefinitionByKey(string caseDefinitionKey)
        //{
        //         
        //         return caseDefinitionCache.FindDeployedLatestDefinitionByKey(caseDefinitionKey);
        //}

        ///// <returns> the latest version of the case definition with the given key and tenant id </returns>
        //public virtual CaseDefinitionEntity FindDeployedLatestCaseDefinitionByKeyAndTenantId(string caseDefinitionKey, string tenantId)
        //{
        //         
        //         return caseDefinitionCache.FindDeployedLatestDefinitionByKeyAndTenantId(caseDefinitionKey, tenantId);
        //}

        //public virtual CaseDefinitionEntity FindDeployedCaseDefinitionByKeyVersionAndTenantId(string caseDefinitionKey, int? caseDefinitionVersion, string tenantId)
        //{
        //         
        //         return caseDefinitionCache.FindDeployedDefinitionByKeyVersionAndTenantId(caseDefinitionKey, caseDefinitionVersion, tenantId);
        //}

        //public virtual CaseDefinitionEntity FindDeployedCaseDefinitionByDeploymentAndKey(string deploymentId, string caseDefinitionKey)
        //{
        //         
        //         return caseDefinitionCache.FindDeployedDefinitionByDeploymentAndKey(deploymentId, caseDefinitionKey);
        //}

        //public virtual CaseDefinitionEntity GetCaseDefinitionById(string caseDefinitionId)
        //{
        //         
        //         return caseDefinitionCache.getCaseDefinitionById(caseDefinitionId);
        //}

        //public virtual CaseDefinitionEntity ResolveCaseDefinition(CaseDefinitionEntity caseDefinition)
        //{
        //         
        //         return caseDefinitionCache.resolveDefinition(caseDefinition);
        //}

        //public virtual ICmmnModelInstance FindCmmnModelInstanceForCaseDefinition(string caseDefinitionId)
        //{

        //    return CmmnModelInstanceCache.FindBpmnModelInstanceForDefinition(caseDefinitionId);
        //}

        //public virtual void AddCaseDefinition(CaseDefinitionEntity caseDefinition)
        //{
        //    CaseDefinitionCache.AddDefinition(caseDefinition);
        //}

        public virtual void RemoveCaseDefinition(string caseDefinitionId)
        {
            //caseDefinitionCache.removeDefinitionFromCache(caseDefinitionId);
            //cmmnModelInstanceCache.remove(caseDefinitionId);
        }

        public virtual void DiscardCaseDefinitionCache()
        {
            //caseDefinitionCache.Clear();
            //cmmnModelInstanceCache.Clear();
        }

        // // DECISION DEFINITION ////////////////////////////////////////////////////////////////////////////

        public virtual DecisionDefinitionEntity FindDecisionDefinitionFromCache(string decisionDefinitionId)
        {
            return _decisionDefinitionCache.FindDefinitionFromCache(decisionDefinitionId);
        }

        public virtual DecisionDefinitionEntity FindDeployedDecisionDefinitionById(string decisionDefinitionId)
        {
            return _decisionDefinitionCache.FindDeployedDefinitionById(decisionDefinitionId);
        }

        public virtual IDecisionDefinition FindDeployedLatestDecisionDefinitionByKey(string decisionDefinitionKey)
        {
            return _decisionDefinitionCache.FindDeployedLatestDefinitionByKey(decisionDefinitionKey);
        }

        public virtual IDecisionDefinition FindDeployedLatestDecisionDefinitionByKeyAndTenantId(
            string decisionDefinitionKey, string tenantId)
        {
            return _decisionDefinitionCache.FindDeployedLatestDefinitionByKeyAndTenantId(decisionDefinitionKey, tenantId);
        }

        public virtual IDecisionDefinition FindDeployedDecisionDefinitionByDeploymentAndKey(string deploymentId,
            string decisionDefinitionKey)
        {
            return _decisionDefinitionCache.FindDeployedDefinitionByDeploymentAndKey(deploymentId, decisionDefinitionKey);
        }

        public virtual IDecisionDefinition FindDeployedDecisionDefinitionByKeyAndVersion(string decisionDefinitionKey,
            int? decisionDefinitionVersion)
        {
            return _decisionDefinitionCache.FindDeployedDefinitionByKeyAndVersion(decisionDefinitionKey,
                decisionDefinitionVersion);
        }

        public virtual IDecisionDefinition FindDeployedDecisionDefinitionByKeyVersionAndTenantId(
            string decisionDefinitionKey, int? decisionDefinitionVersion, string tenantId)
        {
            return _decisionDefinitionCache.FindDeployedDefinitionByKeyVersionAndTenantId(decisionDefinitionKey,
                decisionDefinitionVersion, tenantId);
        }

        public virtual DecisionDefinitionEntity ResolveDecisionDefinition(DecisionDefinitionEntity decisionDefinition)
        {
            return _decisionDefinitionCache.ResolveDefinition(decisionDefinition);
        }

        public virtual IDmnModelInstance FindDmnModelInstanceForDecisionDefinition(string decisionDefinitionId)
        {
            return _dmnModelInstanceCache.FindBpmnModelInstanceForDefinition(decisionDefinitionId);
        }

        public virtual void AddDecisionDefinition(DecisionDefinitionEntity decisionDefinition)
        {
            _decisionDefinitionCache.AddDefinition(decisionDefinition);
        }

        public virtual void RemoveDecisionDefinition(string decisionDefinitionId)
        {
            _decisionDefinitionCache.RemoveDefinitionFromCache(decisionDefinitionId);
            _dmnModelInstanceCache.Remove(decisionDefinitionId);
        }

        public virtual void DiscardDecisionDefinitionCache()
        {
            _decisionDefinitionCache.Clear();
            _dmnModelInstanceCache.Clear();
        }

        //DECISION REQUIREMENT DEFINITION ////////////////////////////////////////////////////////////////////////////

        public virtual void AddDecisionRequirementsDefinition(
            DecisionRequirementsDefinitionEntity decisionRequirementsDefinition)
        {
            _decisionRequirementsDefinitionCache.AddDefinition(decisionRequirementsDefinition);
        }

        public virtual DecisionRequirementsDefinitionEntity FindDecisionRequirementsDefinitionFromCache(
            string decisionRequirementsDefinitionId)
        {
            return _decisionRequirementsDefinitionCache.FindDefinitionFromCache(decisionRequirementsDefinitionId);
        }

        public virtual DecisionRequirementsDefinitionEntity FindDeployedDecisionRequirementsDefinitionById(
            string decisionRequirementsDefinitionId)
        {
            return _decisionRequirementsDefinitionCache.FindDeployedDefinitionById(decisionRequirementsDefinitionId);
        }

        public virtual DecisionRequirementsDefinitionEntity ResolveDecisionRequirementsDefinition(
            DecisionRequirementsDefinitionEntity decisionRequirementsDefinition)
        {
            return _decisionRequirementsDefinitionCache.ResolveDefinition(decisionRequirementsDefinition);
        }

        public virtual void DiscardDecisionRequirementsDefinitionCache()
        {
            _decisionDefinitionCache.Clear();
        }

        public virtual void RemoveDecisionRequirementsDefinition(string decisionRequirementsDefinitionId)
        {
            _decisionRequirementsDefinitionCache.RemoveDefinitionFromCache(decisionRequirementsDefinitionId);
        }

        //public virtual ICache<string, CaseDefinitionEntity> CaseDefinitionCache
        //{
        //    get
        //    {

        //        return caseDefinitionCache.Cache;
        //    }
        //}
        public void SetDeployers(IList<IDeployer> value)
        {
            _cacheDeployer.Deployers = value;
        }

        public virtual void RemoveDeployment(string deploymentId)
        {
            _bpmnModelInstanceCache.RemoveAllDefinitionsByDeploymentId(deploymentId);
            //cmmnModelInstanceCache.removeAllDefinitionsByDeploymentId(deploymentId);
            _dmnModelInstanceCache.RemoveAllDefinitionsByDeploymentId(deploymentId);
            RemoveAllDecisionRequirementsDefinitionsByDeploymentId(deploymentId);
        }

        protected internal virtual void RemoveAllDecisionRequirementsDefinitionsByDeploymentId(string deploymentId)
        {
            // remove all decision requirements definitions for a specific deployment

            //IList<IDecisionRequirementsDefinition> allDefinitionsForDeployment = (new DecisionRequirementsDefinitionQueryImpl()).SetDeploymentId(deploymentId).List().GetList();

            //foreach (IDecisionRequirementsDefinition decisionRequirementsDefinition in allDefinitionsForDeployment)
            //{
            //    try
            //    {
            //        RemoveDecisionDefinition(decisionRequirementsDefinition.Id);
            //    }
            //    catch (Exception e)
            //    {
            //        ProcessEngineLogger.PersistenceLogger.RemoveEntryFromDeploymentCacheFailure("decision requirement", decisionRequirementsDefinition.Id, e);
            //    }
            //}
        }

        public virtual CachePurgeReport PurgeCache()
        {
            var result = new CachePurgeReport();
            var processDefinitionCache = ProcessDefinitionCache;
            if (!processDefinitionCache.IsEmpty())
            {
                result.AddPurgeInformation(CachePurgeReport.ProcessDefCache, processDefinitionCache.KeySet());
                processDefinitionCache.Clear();
            }

            var bpmnModelInstanceCache = BpmnModelInstanceCache;
            if (!bpmnModelInstanceCache.IsEmpty())
            {
                result.AddPurgeInformation(CachePurgeReport.BpmnModelInstCache, bpmnModelInstanceCache.KeySet());
                bpmnModelInstanceCache.Clear();
            }

            //ICache<string, ICaseDefinitionEntity> caseDefinitionCache = _caseDefinitionCache;
            //if (!caseDefinitionCache.IsEmpty())
            //{
            //    result.AddPurgeInformation(CachePurgeReport.CASE_DEF_CACHE, caseDefinitionCache.KeySet());
            //    caseDefinitionCache.Clear();
            //}

            //ICache<string, CmmnModelInstance> cmmnModelInstanceCache = CmmnModelInstanceCache;
            //if (!cmmnModelInstanceCache.IsEmpty())
            //{
            //    result.AddPurgeInformation(CachePurgeReport.CASE_MODEL_INST_CACHE, cmmnModelInstanceCache.KeySet());
            //    cmmnModelInstanceCache.Clear();
            //}

            var decisionDefinitionCache = DecisionDefinitionCache;
            if (!decisionDefinitionCache.IsEmpty())
            {
                result.AddPurgeInformation(CachePurgeReport.DmnDefCache, decisionDefinitionCache.KeySet());
                decisionDefinitionCache.Clear();
            }

            var dmnModelInstanceCache = DmnDefinitionCache;
            if (!dmnModelInstanceCache.IsEmpty())
            {
                result.AddPurgeInformation(CachePurgeReport.DmnModelInstCache, dmnModelInstanceCache.KeySet());
                dmnModelInstanceCache.Clear();
            }

            var decisionRequirementsDefinitionCache = DecisionRequirementsDefinitionCache;
            if (!decisionRequirementsDefinitionCache.IsEmpty())
            {
                result.AddPurgeInformation(CachePurgeReport.DmnReqDefCache, decisionRequirementsDefinitionCache.KeySet());
                decisionRequirementsDefinitionCache.Clear();
            }

            return result;
        }
    }
}