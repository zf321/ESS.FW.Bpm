using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Variable.Serializer;
using ESS.FW.Bpm.Engine.Persistence;
using ESS.FW.Bpm.Engine.Variable.Value;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace org.camunda.bpm.engine.impl.history.@event
{
    /// <summary>
    ///     Data base operations for <seealso cref="HistoricDecisionInstanceEntity" />.
    ///     
    /// </summary>
    public class HistoricDecisionInstanceManager : AbstractHistoricManagerNet<HistoricDecisionInstanceEntity>, IHistoricDecisionInstanceManager
    {
        public HistoricDecisionInstanceManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }
        public virtual void DeleteHistoricDecisionInstancesByDecisionDefinitionId(string decisionDefinitionId)
        {
            if (HistoryEnabled)
            {
                var decisionInstances = FindHistoricDecisionInstancesByDecisionDefinitionId(decisionDefinitionId);

                ISet<string> decisionInstanceIds = new HashSet<string>();
                foreach (var decisionInstance in decisionInstances)
                {
                    decisionInstanceIds.Add(decisionInstance.Id);
                    // delete decision instance
                    decisionInstance.Delete();
                }

                if (decisionInstanceIds.Count > 0)
                {
                    DeleteHistoricDecisionInputInstancesByDecisionInstanceIds(decisionInstanceIds);

                    DeleteHistoricDecisionOutputInstancesByDecisionInstanceIds(decisionInstanceIds);
                }
            }
        }

        //JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") protected List<HistoricDecisionInstanceEntity> findHistoricDecisionInstancesByDecisionDefinitionId(String decisionDefinitionId)
        protected internal virtual IList<HistoricDecisionInstanceEntity>
            FindHistoricDecisionInstancesByDecisionDefinitionId(string decisionDefinitionId)
        {
            //TODO  TenantManager.configureQuery(query)
            //return DbEntityManager.selectList("selectHistoricDecisionInstancesByDecisionDefinitionId",configureParameterizedQuery(decisionDefinitionId));
            return Find(m => m.DecisionDefinitionId == decisionDefinitionId).ToList();
        }

        protected internal virtual void DeleteHistoricDecisionInputInstancesByDecisionInstanceIds(
            ISet<string> decisionInstanceIds)
        {
            var decisionInputInstances = FindHistoricDecisionInputInstancesByDecisionInstanceIds(decisionInstanceIds);
            foreach (var decisionInputInstance in decisionInputInstances)
            {
                // delete input instance and byte array value if exists
                decisionInputInstance.Delete();
            }
        }

        protected internal virtual void DeleteHistoricDecisionOutputInstancesByDecisionInstanceIds(
            ISet<string> decisionInstanceIds)
        {
            var decisionOutputInstances = FindHistoricDecisionOutputInstancesByDecisionInstanceIds(decisionInstanceIds);
            foreach (var decisionOutputInstance in decisionOutputInstances)
            {
                // delete output instance and byte array value if exists
                decisionOutputInstance.Delete();
            }
        }

        public virtual void DeleteHistoricHistoricInstanceByInstanceId(string historicDecisionInstanceId)
        {
            var decisionInstance = FindHistoricDecisionInstance(historicDecisionInstanceId);
            var foundHistoricDecisionInstance = decisionInstance != null;
            if (foundHistoricDecisionInstance)
            {
                decisionInstance.Delete();
                DeleteHistoricDecisionInputAndOutputInstances(historicDecisionInstanceId);
            }
        }

        protected internal virtual void DeleteHistoricDecisionInputAndOutputInstances(string historicDecisionInstanceId)
        {
            ISet<string> decisionInstanceIds = new HashSet<string>();
            decisionInstanceIds.Add(historicDecisionInstanceId);
            DeleteHistoricDecisionInputInstancesByDecisionInstanceIds(decisionInstanceIds);
            DeleteHistoricDecisionOutputInstancesByDecisionInstanceIds(decisionInstanceIds);
        }

        public virtual void InsertHistoricDecisionInstances(HistoricDecisionEvaluationEvent @event)
        {
            if (HistoryEnabled)
            {
                var rootHistoricDecisionInstance = @event.RootHistoricDecisionInstance;
                InsertHistoricDecisionInstance(rootHistoricDecisionInstance);

                foreach (var requiredHistoricDecisionInstances in @event.RequiredHistoricDecisionInstances)
                {
                    requiredHistoricDecisionInstances.RootDecisionInstanceId = rootHistoricDecisionInstance.Id;

                    InsertHistoricDecisionInstance(requiredHistoricDecisionInstances);
                }
            }
        }

        protected internal virtual void InsertHistoricDecisionInstance(
            HistoricDecisionInstanceEntity historicDecisionInstance)
        {
            //DbEntityManager.insert(historicDecisionInstance);
            Add(historicDecisionInstance);

            InsertHistoricDecisionInputInstances(historicDecisionInstance.Inputs, historicDecisionInstance.Id);
            InsertHistoricDecisionOutputInstances(historicDecisionInstance.Outputs, historicDecisionInstance.Id);
        }

        protected internal virtual void InsertHistoricDecisionInputInstances(
            IList<IHistoricDecisionInputInstance> inputs, string decisionInstanceId)
        {
            foreach (var input in inputs)
            {
                var inputEntity = (HistoricDecisionInputInstanceEntity)input;
                inputEntity.DecisionInstanceId = decisionInstanceId;

                //DbEntityManager.insert(inputEntity);
                GetDbEntityManager<HistoricDecisionInputInstanceEntity>().Add(inputEntity);
            }
        }

        protected internal virtual void InsertHistoricDecisionOutputInstances(
            IList<IHistoricDecisionOutputInstance> outputs, string decisionInstanceId)
        {
            foreach (var output in outputs)
            {
                var outputEntity = (HistoricDecisionOutputInstanceEntity)output;
                outputEntity.DecisionInstanceId = decisionInstanceId;

                //DbEntityManager.insert(outputEntity);
                GetDbEntityManager<HistoricDecisionOutputInstanceEntity>().Add(outputEntity);
            }
        }

        //public virtual IList<IHistoricDecisionInstance> FindHistoricDecisionInstancesByQueryCriteria(
        //    HistoricDecisionInstanceQueryImpl query, Page page)
        //{
        //    if (HistoryEnabled)
        //    {
        //        ConfigureQuery(query);

        //        //JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
        //        //ORIGINAL LINE: @SuppressWarnings("unchecked") List<org.camunda.bpm.engine.history.HistoricDecisionInstance> decisionInstances = getDbEntityManager().selectList("selectHistoricDecisionInstancesByQueryCriteria", query, page);
        //        IList<IHistoricDecisionInstance> decisionInstances =
        //            DbEntityManager.selectList("selectHistoricDecisionInstancesByQueryCriteria", query, page);

        //        IDictionary<string, HistoricDecisionInstanceEntity> decisionInstancesById =
        //            new Dictionary<string, HistoricDecisionInstanceEntity>();
        //        foreach (var decisionInstance in decisionInstances)
        //        {
        //            decisionInstancesById[decisionInstance.Id] = (HistoricDecisionInstanceEntity)decisionInstance;
        //        }

        //        if (decisionInstances.Count > 0 && query.IncludeInput)
        //        {
        //            AppendHistoricDecisionInputInstances(decisionInstancesById, query);
        //        }

        //        if (decisionInstances.Count > 0 && query.IncludeOutputs)
        //        {
        //            AppendHistoricDecisionOutputInstances(decisionInstancesById, query);
        //        }

        //        return decisionInstances;
        //    }
        //    return null;
        //}

        //protected internal virtual void AppendHistoricDecisionInputInstances(
        //    IDictionary<string, HistoricDecisionInstanceEntity> decisionInstancesById,
        //    HistoricDecisionInstanceQueryImpl query)
        //{
        //    var decisionInputInstances =
        //        FindHistoricDecisionInputInstancesByDecisionInstanceIds(decisionInstancesById.Keys);
        //    InitializeInputInstances(decisionInstancesById.Values);

        //    foreach (var decisionInputInstance in decisionInputInstances)
        //    {
        //        var historicDecisionInstance = decisionInstancesById[decisionInputInstance.DecisionInstanceId];
        //        historicDecisionInstance.addInput(decisionInputInstance);

        //        // do not fetch values for byte arrays eagerly (unless requested by the user)
        //        if (!IsBinaryValue(decisionInputInstance) || query.ByteArrayFetchingEnabled)
        //        {
        //            fetchVariableValue(decisionInputInstance, query.CustomObjectDeserializationEnabled);
        //        }
        //    }
        //}

        protected internal virtual void InitializeInputInstances(
            ICollection<HistoricDecisionInstanceEntity> decisionInstances)
        {
            foreach (var decisionInstance in decisionInstances)
            {
                decisionInstance.Inputs = new List<IHistoricDecisionInputInstance>();
            }
        }

        protected internal virtual IQueryable<HistoricDecisionInputInstanceEntity>
            FindHistoricDecisionInputInstancesByDecisionInstanceIds(ISet<string> historicDecisionInstanceKeys)
        {
            //return DbEntityManager.selectList("selectHistoricDecisionInputInstancesByDecisionInstanceIds",historicDecisionInstanceKeys);
            //<select id="selectHistoricDecisionInputInstancesByDecisionInstanceIds" parameterType="org.camunda.bpm.engine.impl.db.ListQueryParameterObject" resultMap="historicDecisionInputInstanceResultMap">
            //            select* from ${ prefix}
            //            ACT_HI_DEC_IN
            //where DEC_INST_ID_ in
            //    <foreach collection = "parameter" index = "index" item = "decisionInstanceId" open = "(" separator = "," close = ")" >
            //                #{decisionInstanceId}
            //    </foreach>
            //    order by CLAUSE_ID_
            //  </ select >
            if (historicDecisionInstanceKeys == null || historicDecisionInstanceKeys.Count == 0)
            {
                return null;
            }
            var query = GetDbEntityManager<HistoricDecisionInputInstanceEntity>().Find(m => historicDecisionInstanceKeys.Contains(m.DecisionInstanceId));
            return query;
        }

        protected internal virtual bool IsBinaryValue(IHistoricDecisionInputInstance decisionInputInstance)
        {
            return AbstractTypedValueSerializer<ITypedValue>.BINARY_VALUE_TYPES.Contains(decisionInputInstance.TypeName);
        }

        protected internal virtual void FetchVariableValue(HistoricDecisionInputInstanceEntity decisionInputInstance,
            bool isCustomObjectDeserializationEnabled)
        {
            try
            {
                decisionInputInstance.GetTypedValue(isCustomObjectDeserializationEnabled);
            }
            catch (Exception t)
            {
                // do not fail if one of the variables fails to load
                Log.FailedTofetchVariableValue(t);
            }
        }


        //protected internal virtual void AppendHistoricDecisionOutputInstances(
        //    IDictionary<string, HistoricDecisionInstanceEntity> decisionInstancesById,
        //    HistoricDecisionInstanceQueryImpl query)
        //{
        //    var decisionOutputInstances =
        //        findHistoricDecisionOutputInstancesByDecisionInstanceIds(decisionInstancesById.Keys);
        //    InitializeOutputInstances(decisionInstancesById.Values);

        //    foreach (var decisionOutputInstance in decisionOutputInstances)
        //    {
        //        var historicDecisionInstance = decisionInstancesById[decisionOutputInstance.DecisionInstanceId];
        //        historicDecisionInstance.addOutput(decisionOutputInstance);

        //        // do not fetch values for byte arrays eagerly (unless requested by the user)
        //        if (!isBinaryValue(decisionOutputInstance) || query.ByteArrayFetchingEnabled)
        //        {
        //            fetchVariableValue(decisionOutputInstance, query.CustomObjectDeserializationEnabled);
        //        }
        //    }
        //}

        protected internal virtual void InitializeOutputInstances(
            ICollection<HistoricDecisionInstanceEntity> decisionInstances)
        {
            foreach (var decisionInstance in decisionInstances)
            {
                decisionInstance.Outputs = new List<IHistoricDecisionOutputInstance>();
            }
        }

        //JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") protected List<HistoricDecisionOutputInstanceEntity> findHistoricDecisionOutputInstancesByDecisionInstanceIds(ISet<string> decisionInstanceKeys)
        protected internal virtual IList<HistoricDecisionOutputInstanceEntity>
            FindHistoricDecisionOutputInstancesByDecisionInstanceIds(ISet<string> decisionInstanceKeys)
        {
            throw new NotImplementedException();
            //return DbEntityManager.selectList("selectHistoricDecisionOutputInstancesByDecisionInstanceIds",
              //  decisionInstanceKeys);
        }

        protected internal virtual bool IsBinaryValue(IHistoricDecisionOutputInstance decisionOutputInstance)
        {
            return AbstractTypedValueSerializer<ITypedValue>.BINARY_VALUE_TYPES.Contains(decisionOutputInstance.TypeName);
        }

        protected internal virtual void fetchVariableValue(HistoricDecisionOutputInstanceEntity decisionOutputInstance,
            bool isCustomObjectDeserializationEnabled)
        {
            try
            {
                decisionOutputInstance.getTypedValue(isCustomObjectDeserializationEnabled);
            }
            catch (Exception t)
            {
                // do not fail if one of the variables fails to load
                Log.FailedTofetchVariableValue(t);
            }
        }

        public virtual HistoricDecisionInstanceEntity FindHistoricDecisionInstance(string historicDecisionInstanceId)
        {
            if (HistoryEnabled)
            {
                throw new NotImplementedException();
                //return
                //    (HistoricDecisionInstanceEntity)
                //        DbEntityManager.selectOne("selectHistoricDecisionInstanceByDecisionInstanceId",
                //            ConfigureParameterizedQuery(historicDecisionInstanceId));
            }
            return null;
        }

        //public virtual long FindHistoricDecisionInstanceCountByQueryCriteria(HistoricDecisionInstanceQueryImpl query)
        //{
        //    if (HistoryEnabled)
        //    {
        //        ConfigureQuery(query);
        //        return
        //            (long?)DbEntityManager.selectOne("selectHistoricDecisionInstanceCountByQueryCriteria", query).Value;
        //    }
        //    return 0;
        //}

        //JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public List<org.camunda.bpm.engine.history.HistoricDecisionInstance> findHistoricDecisionInstancesByNativeQuery(Map<String, Object> parameterMap, int firstResult, int maxResults)
        public virtual IList<IHistoricDecisionInstance> FindHistoricDecisionInstancesByNativeQuery(
            IDictionary<string, object> parameterMap, int firstResult, int maxResults)
        {
            throw new NotImplementedException();
            //return DbEntityManager.selectListWithRawParameter("selectHistoricDecisionInstancesByNativeQuery",
            //    parameterMap, firstResult, maxResults);
        }

        public virtual long FindHistoricDecisionInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
        {
            throw new NotImplementedException();
            //return
            //    (long?)
            //        DbEntityManager.selectOne("selectHistoricDecisionInstanceCountByNativeQuery", parameterMap).Value;
        }

        //protected internal virtual void ConfigureQuery(HistoricDecisionInstanceQueryImpl query)
        //{
        //    AuthorizationManager.ConfigureHistoricDecisionInstanceQuery(query);
        //    TenantManager.configureQuery(query);
        //}


        //protected internal virtual ListQueryParameterObject ConfigureParameterizedQuery(object parameter)
        //{
        //    return TenantManager.configureQuery(parameter);
        //}
    }
}

