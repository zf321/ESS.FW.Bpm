using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Persistence;
using ESS.FW.Bpm.Engine.Repository;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository
{
    /// <summary>
    ///     
    /// </summary>
    [Component]
    public class DecisionRequirementsDefinitionManager : AbstractManagerNet<DecisionRequirementsDefinitionEntity>, IAbstractResourceDefinitionManager<DecisionRequirementsDefinitionEntity>, IDecisionRequirementsDefinitionManager
    {
        public DecisionRequirementsDefinitionManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual void InsertDecisionRequirementsDefinition(
            DecisionRequirementsDefinitionEntity decisionRequirementsDefinition)
        {
            Add(decisionRequirementsDefinition);
            CreateDefaultAuthorizations(decisionRequirementsDefinition);
        }

        public virtual void DeleteDecisionRequirementsDefinitionsByDeploymentId(string deploymentId)
        {
            Delete(c => c.DeploymentId == deploymentId);
        }

        public virtual DecisionRequirementsDefinitionEntity FindDecisionRequirementsDefinitionById(
            string decisionRequirementsDefinitionId)
        {
            throw new System.NotImplementedException();
            //return DbEntityManager.selectById(typeof (DecisionRequirementsDefinitionEntity),
            //    decisionRequirementsDefinitionId);
            return null;
        }

        public virtual string FindPreviousDecisionRequirementsDefinitionId(string decisionRequirementsDefinitionKey,
            int? version, string tenantId)
        {
            throw new System.NotImplementedException();
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["key"] = decisionRequirementsDefinitionKey;
            @params["version"] = version;
            @params["tenantId"] = tenantId;
            //return (string) DbEntityManager.selectOne("selectPreviousDecisionRequirementsDefinitionId", @params);
            return string.Empty;
        }

        public virtual IList<IDecisionRequirementsDefinition> FindDecisionRequirementsDefinitionByDeploymentId(
            string deploymentId)
        {
            return Find(c => c.DeploymentId == deploymentId).ToList().Cast<IDecisionRequirementsDefinition>().ToList();
        }

        public virtual DecisionRequirementsDefinitionEntity FindDecisionRequirementsDefinitionByDeploymentAndKey(
            string deploymentId, string decisionRequirementsDefinitionKey)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["deploymentId"] = deploymentId;
            //parameters["decisionRequirementsDefinitionKey"] = decisionRequirementsDefinitionKey;
            //return
            //    (DecisionRequirementsDefinitionEntity)
            //        DbEntityManager.selectOne("selectDecisionRequirementsDefinitionByDeploymentAndKey", parameters);
            // <select id="selectDecisionRequirementsDefinitionByDeploymentAndKey" parameterType="map" resultMap="decisionRequirementsDefinitionsResultMap">
            //            select*
            //            from ${ prefix}
            //            ACT_RE_DECISION_REQ_DEF
            //where DEPLOYMENT_ID_ = #{deploymentId}
            //      and KEY_ = #{decisionRequirementsDefinitionKey}
            //  </ select >
            return Find(m => m.DeploymentId == deploymentId && m.Key == decisionRequirementsDefinitionKey).FirstOrDefault();
        }

        /// <returns> the latest version of the decision requirements definition with the given key and tenant id </returns>
        public virtual DecisionRequirementsDefinitionEntity FindLatestDecisionRequirementsDefinitionByKeyAndTenantId(
            string decisionRequirementsDefinitionKey, string tenantId)
        {
            //throw new System.NotImplementedException();
            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters["decisionRequirementsDefinitionKey"] = decisionRequirementsDefinitionKey;
            //parameters["tenantId"] = tenantId;
            #region sql
            //             < select id = "selectLatestDecisionRequirementsDefinitionByKeyWithoutTenantId" parameterType = "map" resultMap = "decisionRequirementsDefinitionsResultMap" >
            //         select *
            //         from ${ prefix}
            //            ACT_RE_DECISION_REQ_DEF
            //where KEY_ = #{decisionRequirementsDefinitionKey}
            //          and TENANT_ID_ is null
            //          and VERSION_ = (
            //              select max(VERSION_)
            //              from ${ prefix}
            //            ACT_RE_DECISION_REQ_DEF
            //where KEY_ = #{decisionRequirementsDefinitionKey} and TENANT_ID_ is null)
            //  </ select >


            //< select id = "selectLatestDecisionRequirementsDefinitionByKeyAndTenantId" parameterType = "map" resultMap = "decisionRequirementsDefinitionsResultMap" >
            //select *
            //from ${ prefix}
            //            ACT_RE_DECISION_REQ_DEF RES
            //    where KEY_ = #{decisionRequirementsDefinitionKey}
            //          and TENANT_ID_ = #{tenantId}
            //          and VERSION_ = (
            //              select max(VERSION_)
            //              from ${ prefix}
            //            ACT_RE_DECISION_REQ_DEF
            //where KEY_ = #{decisionRequirementsDefinitionKey} and TENANT_ID_ = #{tenantId})
            //  </ select >
            #endregion
            var versions = Find(m => m.Key == decisionRequirementsDefinitionKey && m.TenantId == tenantId);
            if (versions.Count() == 0)
            {
                return null;
            }
            var version = versions.Max(m => m.Version);
            var query = Find(m => m.Key == decisionRequirementsDefinitionKey && m.TenantId == tenantId && m.Version == version);
            return query.FirstOrDefault();
            //if (ReferenceEquals(tenantId, null))
            //{

            //    return
            //        (DecisionRequirementsDefinitionEntity)
            //            DbEntityManager.selectOne("selectLatestDecisionRequirementsDefinitionByKeyWithoutTenantId",
            //                parameters);
            //}
            //return
            //    (DecisionRequirementsDefinitionEntity)
            //        DbEntityManager.selectOne("selectLatestDecisionRequirementsDefinitionByKeyAndTenantId", parameters);
        }

        //JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.repository.DecisionRequirementsDefinition> findDecisionRequirementsDefinitionsByQueryCriteria(DecisionRequirementsDefinitionQueryImpl query, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IDecisionRequirementsDefinition> FindDecisionRequirementsDefinitionsByQueryCriteria(
        //    DecisionRequirementsDefinitionQueryImpl query, Page page)
        //{
        //    ConfigureDecisionRequirementsDefinitionQuery(query);
        //    //return DbEntityManager.selectList("selectDecisionRequirementsDefinitionsByQueryCriteria", query, page);
        //    return null;
        //}

        //public virtual long FindDecisionRequirementsDefinitionCountByQueryCriteria(
        //    DecisionRequirementsDefinitionQueryImpl query)
        //{
        //    ConfigureDecisionRequirementsDefinitionQuery(query);
        //    //return
        //    //    (long?)
        //    //        DbEntityManager.selectOne("selectDecisionRequirementsDefinitionCountByQueryCriteria", query).Value;
        //    return 0;
        //}

        protected internal virtual void CreateDefaultAuthorizations(
            IDecisionRequirementsDefinition decisionRequirementsDefinition)
        {
            //if (AuthorizationEnabled)
            //{
            //    ResourceAuthorizationProvider provider = ResourceAuthorizationProvider;
            //    AuthorizationEntity[] authorizations =
            //        provider.newDecisionRequirementsDefinition(decisionRequirementsDefinition);
            //    saveDefaultAuthorizations(authorizations);
            //}
        }

        //protected internal virtual void ConfigureDecisionRequirementsDefinitionQuery(
        //    DecisionRequirementsDefinitionQueryImpl query)
        //{
        //    //AuthorizationManager.configureDecisionRequirementsDefinitionQuery(query);
        //    //TenantManager.configureQuery(query);
        //}


        public virtual DecisionRequirementsDefinitionEntity FindLatestDefinitionByKey(string key)
        {
            throw new System.NotImplementedException();
            //return null;
        }

        public virtual DecisionRequirementsDefinitionEntity FindLatestDefinitionById(string id)
        {
            throw new System.NotImplementedException();
            //return DbEntityManager.selectById(typeof (DecisionRequirementsDefinitionEntity), id);
            //return null;
        }

        public virtual DecisionRequirementsDefinitionEntity FindLatestDefinitionByKeyAndTenantId(string definitionKey,
            string tenantId)
        {
            throw new System.NotImplementedException();
            return null;
        }

        public virtual DecisionRequirementsDefinitionEntity FindDefinitionByKeyVersionAndTenantId(string definitionKey,
            int? definitionVersion, string tenantId)
        {
            throw new System.NotImplementedException();
            return null;
        }

        public virtual DecisionRequirementsDefinitionEntity FindDefinitionByDeploymentAndKey(string deploymentId,
            string definitionKey)
        {
            throw new System.NotImplementedException();
            return null;
        }

        public virtual DecisionRequirementsDefinitionEntity GetCachedResourceDefinitionEntity(string definitionId)
        {
            throw new System.NotImplementedException();
            //return DbEntityManager.getCachedEntity(typeof (DecisionRequirementsDefinitionEntity), definitionId);
            return null;
        }
    }
}