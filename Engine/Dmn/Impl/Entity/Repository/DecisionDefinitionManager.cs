using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using ESS.FW.Bpm.Engine.Persistence;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Common.Extensions;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository
{
    [Component]
    public class DecisionDefinitionManager
        : AbstractManagerNet<DecisionDefinitionEntity>, IAbstractResourceDefinitionManager<DecisionDefinitionEntity>, IDecisionDefinitionManager
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        public DecisionDefinitionManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual void InsertDecisionDefinition(DecisionDefinitionEntity decisionDefinition)
        {
            Add(decisionDefinition);
            CreateDefaultAuthorizations(decisionDefinition);
        }

        public virtual void DeleteDecisionDefinitionsByDeploymentId(string deploymentId)
        {
            Delete(m => m.DeploymentId == deploymentId);
            //DbEntityManager.Delete(typeof(DecisionDefinitionEntity), "deleteDecisionDefinitionsByDeploymentId", deploymentId);
        }

        public virtual DecisionDefinitionEntity FindDecisionDefinitionById(string decisionDefinitionId)
        {
            return Get(decisionDefinitionId);
            //return Get<DecisionDefinitionEntity>(typeof(DecisionDefinitionEntity), decisionDefinitionId);
        }

        /// <returns>
        ///     the latest version of the decision definition with the given key(from any tenant)
        /// </returns>
        /// <exception cref = "ProcessEngineException" >
        ///     if more than one tenant has a decision definition with the given key
        /// </exception>
        /// <seealso cref = # findLatestDecisionDefinitionByKeyAndTenantId( String, String
        /// )
        /// </seealso>
        /// <returns>
        ///     the latest version of the decision definition with the given key and tenant id
        /// </returns>
        /// <seealso cref = # findLatestDecisionDefinitionByKey( String
        /// )
        /// </seealso>
        public virtual DecisionDefinitionEntity FindLatestDecisionDefinitionByKeyAndTenantId(string decisionDefinitionKey, string tenantId)
        {
            //throw new NotImplementedException();
            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters["decisionDefinitionKey"] = decisionDefinitionKey;
            //parameters["tenantId"] = tenantId;

            //if (string.ReferenceEquals(tenantId, null))
            //{
            //    return (DecisionDefinitionEntity)DbEntityManager.SelectOne("selectLatestDecisionDefinitionByKeyWithoutTenantId", parameters);
            //}
            //else
            //{
            //    return (DecisionDefinitionEntity)DbEntityManager.SelectOne("selectLatestDecisionDefinitionByKeyAndTenantId", parameters);
            //}
            #region sql
            //              < select id = "selectLatestDecisionDefinitionByKeyAndTenantId" parameterType = "map" resultMap = "decisionDefinitionResultMap" >
            //         select *
            //         from ${ prefix}
            //            ACT_RE_DECISION_DEF RES
            //    where KEY_ = #{decisionDefinitionKey}
            //          and TENANT_ID_ = #{tenantId}
            //          and VERSION_ = (
            //              select max(VERSION_)
            //              from ${ prefix}
            //            ACT_RE_DECISION_DEF
            //where KEY_ = #{decisionDefinitionKey} and TENANT_ID_ = #{tenantId})
            //  </ select >
            #endregion
            var versions = Find(m => m.Key == decisionDefinitionKey && m.TenantId == tenantId);
            if (versions.Count() == 0)
            {
                return null;
            }
            int version = versions.Max(m => m.Version);
            return Find(m => m.Key == decisionDefinitionKey && m.TenantId == tenantId && m.Version == version).FirstOrDefault();
        }

        public virtual DecisionDefinitionEntity FindDecisionDefinitionByKeyAndVersion(string decisionDefinitionKey, int? decisionDefinitionVersion)
        {
            throw new NotImplementedException();
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["decisionDefinitionVersion"] = decisionDefinitionVersion;
            //parameters["decisionDefinitionKey"] = decisionDefinitionKey;
            //return (DecisionDefinitionEntity)DbEntityManager.SelectOne("selectDecisionDefinitionByKeyAndVersion", ConfigureParameterizedQuery(parameters));
        }

        public virtual DecisionDefinitionEntity FindDecisionDefinitionByKeyVersionAndTenantId(string decisionDefinitionKey, int? decisionDefinitionVersion, string tenantId)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["decisionDefinitionVersion"] = decisionDefinitionVersion;
            //parameters["decisionDefinitionKey"] = decisionDefinitionKey;
            //parameters["tenantId"] = tenantId;
            //if (string.ReferenceEquals(tenantId, null))
            //{
            //    return (DecisionDefinitionEntity)DbEntityManager.SelectOne("selectDecisionDefinitionByKeyVersionWithoutTenantId", parameters);
            //}
            //else
            //{
            //    return (DecisionDefinitionEntity)DbEntityManager.SelectOne("selectDecisionDefinitionByKeyVersionAndTenantId", parameters);
            //}
            return Find(m => m.Key == decisionDefinitionKey && m.Version == decisionDefinitionVersion && m.TenantId == tenantId).FirstOrDefault();
        }

        public virtual DecisionDefinitionEntity FindDecisionDefinitionByDeploymentAndKey(string deploymentId, string decisionDefinitionKey)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["deploymentId"] = deploymentId;
            //parameters["decisionDefinitionKey"] = decisionDefinitionKey;
            //return (DecisionDefinitionEntity)DbEntityManager.SelectOne("selectDecisionDefinitionByDeploymentAndKey", parameters);
            return Find(m => m.DeploymentId == deploymentId && m.Key == decisionDefinitionKey).FirstOrDefault();
        }

        //JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.repository.DecisionDefinition> findDecisionDefinitionsByQueryCriteria(DecisionDefinitionQueryImpl decisionDefinitionQuery, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IDecisionDefinition> FindDecisionDefinitionsByQueryCriteria(DecisionDefinitionQueryImpl decisionDefinitionQuery, Page page)
        //{
        //    ConfigureDecisionDefinitionQuery(decisionDefinitionQuery);
        //    return ListExt.ConvertToListT<IDecisionDefinition>(DbEntityManager.SelectList("selectDecisionDefinitionsByQueryCriteria", decisionDefinitionQuery, page)) ;
        //}

        //public virtual long FindDecisionDefinitionCountByQueryCriteria(DecisionDefinitionQueryImpl decisionDefinitionQuery)
        //{
        //    ConfigureDecisionDefinitionQuery(decisionDefinitionQuery);
        //    return (long)DbEntityManager.SelectOne("selectDecisionDefinitionCountByQueryCriteria", decisionDefinitionQuery);//.Value;
        //}

        public virtual string FindPreviousDecisionDefinitionId(string decisionDefinitionKey, int? version, string tenantId)
        {
            throw new NotImplementedException();
            //IDictionary<string, object> @params = new Dictionary<string, object>();
            //@params["key"] = decisionDefinitionKey;
            //@params["version"] = version;
            //@params["tenantId"] = tenantId;
            //return (string)DbEntityManager.SelectOne("selectPreviousDecisionDefinitionId", @params);
        }

        //JAVA To C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
        //Original LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.repository.DecisionDefinition> FindDecisionDefinitionByDeploymentId(String deploymentId)
        public virtual IList<IDecisionDefinition> FindDecisionDefinitionByDeploymentId(string deploymentId)
        {
            //return ListExt.ConvertToListT<IDecisionDefinition>(DbEntityManager.SelectList("selectDecisionDefinitionByDeploymentId", deploymentId)) ;
            return Find(m => m.DeploymentId == deploymentId).ToList().Cast<IDecisionDefinition>().ToList();
        }
        protected internal virtual void CreateDefaultAuthorizations(IDecisionDefinition decisionDefinition)
        {
            if (AuthorizationEnabled)
            {
                throw new NotImplementedException();
                //ResourceAuthorizationProvider provider = ResourceAuthorizationProvider;
                //AuthorizationEntity[] authorizations = provider.newDecisionDefinition(decisionDefinition);
                //saveDefaultAuthorizations(authorizations);
            }
        }

        //protected internal virtual void ConfigureDecisionDefinitionQuery(DecisionDefinitionQueryImpl query)
        //{
        //    AuthorizationManager.ConfigureDecisionDefinitionQuery(query);
        //    TenantManager.ConfigureQuery(query);
        //}

        //protected internal virtual ListQueryParameterObject ConfigureParameterizedQuery(object parameter)
        //{
        //    return TenantManager.ConfigureQuery(parameter);
        //}

        public virtual DecisionDefinitionEntity FindLatestDefinitionById(string id)
        {
            return FindDecisionDefinitionById(id);
        }

        public virtual DecisionDefinitionEntity FindLatestDefinitionByKey(string key)
        {
            return FindLatestDecisionDefinitionByKey(key);
        }
        /**
   * @return the latest version of the decision definition with the given key (from any tenant)
   *
   * @throws ProcessEngineException if more than one tenant has a decision definition with the given key
   *
   * @see #findLatestDecisionDefinitionByKeyAndTenantId(String, String)
   */
        public DecisionDefinitionEntity FindLatestDecisionDefinitionByKey(string key)
        {
            var keyList = Find(p => p.Key == key).ToList();
            var temp = keyList
                .GroupBy(x => new {x.TenantId, x.Key})
                .Select(group => new
                {
                    list = group,
                    MaxVersion = group.Max(o => o.Version)
                });

            var templist= new List<TbGosBpmReDecisionDefDto>();
            foreach (var v in temp)
            {
                foreach (var j in v.list)
                {
                    templist.Add(new TbGosBpmReDecisionDefDto()
                    {
                        TenantId = j.TenantId,
                        Key = j.Key,
                        MaxVersion = v.MaxVersion,
                    });
                }
            }

            var query = from a in keyList
                        join b in templist
                on a.Key equals b.Key
                where
                a.Version == b.MaxVersion &&
                (a.TenantId == b.TenantId || (string.IsNullOrEmpty(a.TenantId) && string.IsNullOrEmpty(a.TenantId)))
                select a;


                          //List<DecisionDefinitionEntity> decisionDefinitions = getDbEntityManager().selectList("selectLatestDecisionDefinitionByKey", configureParameterizedQuery(decisionDefinitionKey));

                          //if (decisionDefinitions.isEmpty())
                          //{
                          //    return null;

                          //}
                          //else if (decisionDefinitions.size() == 1)
                          //{
                          //    return decisionDefinitions.iterator().next();

                          //}
                          //else
                          //{
                          //    throw LOG.multipleTenantsForDecisionDefinitionKeyException(decisionDefinitionKey);
                          //}
                          //TODO selectLatestDecisionDefinitionByKey 查询逻辑不匹配
                          var decisionDefinitions = Find(m => m.Key == key).OrderByDescending(m => m.Version).ToList();
            if (decisionDefinitions==null|| decisionDefinitions.Count==0)
            {
                return null;

            }
            else if (decisionDefinitions.Count == 1)
            {
                return decisionDefinitions[0];

            }
            else
            {
                throw Log.MultipleTenantsForDecisionDefinitionKeyException(key);
            }
        }

        private class TbGosBpmReDecisionDefDto
        {
            public string TenantId { get; set; }
            public string Key { get; set; }
            public int MaxVersion { get; set; }
        }

        public virtual DecisionDefinitionEntity GetCachedResourceDefinitionEntity(string definitionId)
        {
            throw new NotImplementedException();
            //return DbEntityManager.GetCachedEntity< DecisionDefinitionEntity>(typeof(DecisionDefinitionEntity), definitionId);

        }

        public virtual DecisionDefinitionEntity FindLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
        {
            return FindLatestDecisionDefinitionByKeyAndTenantId(definitionKey, tenantId);
        }

        public virtual DecisionDefinitionEntity FindDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId)
        {
            return FindDecisionDefinitionByKeyVersionAndTenantId(definitionKey, definitionVersion, tenantId);
        }

        public virtual DecisionDefinitionEntity FindDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
        {
            return FindDecisionDefinitionByDeploymentAndKey(deploymentId, definitionKey);
        }
    }
}