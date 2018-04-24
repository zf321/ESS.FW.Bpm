using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Common.Components;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{


    /// <summary>
    ///  
    /// </summary>
    [Component]
    public class HistoricProcessInstanceManager : AbstractHistoricManagerNet<HistoricProcessInstanceEventEntity>, IHistoricProcessInstanceManager
    {
        public HistoricProcessInstanceManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual HistoricProcessInstanceEventEntity FindHistoricProcessInstance(string processInstanceId)
        {
            if (HistoryEnabled)
            {
                //return Get< HistoricProcessInstanceEntity>(typeof(HistoricProcessInstanceEntity), processInstanceId);
                return Get(processInstanceId);
            }
            return null;
        }

        public virtual HistoricProcessInstanceEventEntity FindHistoricProcessInstanceEvent(string eventId)
        {
            if (HistoryEnabled)
            {
                //throw new System.NotImplementedException();
                //return Get< HistoricProcessInstanceEventEntity>(typeof(HistoricProcessInstanceEventEntity), eventId);
                return Get(eventId);
            }
            return null;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteHistoricProcessInstanceByProcessDefinitionId(String processDefinitionId)
        public virtual void DeleteHistoricProcessInstanceByProcessDefinitionId(string processDefinitionId)
        {
            if (HistoryEnabled)
            {
                //IList<string> historicProcessInstanceIds =ListExt.ConvertToListT<string>( DbEntityManager.SelectList("selectHistoricProcessInstanceIdsByProcessDefinitionId", processDefinitionId));
                IList<string> historicProcessInstanceIds = Find(m => m.ProcessDefinitionId == processDefinitionId).Select(m => m.ProcessInstanceId).ToList();

                foreach (string historicProcessInstanceId in historicProcessInstanceIds)
                {
                    DeleteHistoricProcessInstanceById(historicProcessInstanceId);
                }
            }
        }

        public virtual void DeleteHistoricProcessInstanceById(string historicProcessInstanceId)
        {
            if (HistoryEnabled)
            {

                CommandContext commandContext = context.Impl.Context.CommandContext;

                commandContext.HistoricDetailManager.DeleteHistoricDetailsByProcessInstanceId(historicProcessInstanceId);

                commandContext.HistoricVariableInstanceManager.DeleteHistoricVariableInstanceByProcessInstanceId(historicProcessInstanceId);

                commandContext.HistoricActivityInstanceManager.DeleteHistoricActivityInstancesByProcessInstanceId(historicProcessInstanceId);

                commandContext.HistoricTaskInstanceManager.DeleteHistoricTaskInstancesByProcessInstanceId(historicProcessInstanceId);

                commandContext.HistoricIncidentManager.DeleteHistoricIncidentsByProcessInstanceId(historicProcessInstanceId);

                commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByProcessInstanceId(historicProcessInstanceId);

                commandContext.HistoricExternalTaskLogManager.DeleteHistoricExternalTaskLogsByProcessInstanceId(historicProcessInstanceId);


                //commandContext.DbEntityManager.Delete(typeof(HistoricProcessInstanceEntity), "deleteHistoricProcessInstance", historicProcessInstanceId);
                Delete(m => m.ProcessInstanceId == historicProcessInstanceId);
            }
        }

        // public virtual long FindHistoricProcessInstanceCountByQueryCriteria(HistoricProcessInstanceQueryImpl historicProcessInstanceQuery)
        // {
        //if (HistoryEnabled)
        //{
        //  ConfigureQuery(historicProcessInstanceQuery);
        //  //return (long) DbEntityManager.SelectOne("selectHistoricProcessInstanceCountByQueryCriteria", historicProcessInstanceQuery);
        //              throw new System.NotImplementedException();
        //}
        //return 0;
        // }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricProcessInstance> findHistoricProcessInstancesByQueryCriteria(org.camunda.bpm.engine.impl.HistoricProcessInstanceQueryImpl historicProcessInstanceQuery, org.camunda.bpm.engine.impl.Page page)
        // public virtual IList<IHistoricProcessInstance> FindHistoricProcessInstancesByQueryCriteria(HistoricProcessInstanceQueryImpl historicProcessInstanceQuery, Page page)
        // {
        //if (HistoryEnabled)
        //{
        //  ConfigureQuery(historicProcessInstanceQuery);
        //              //return ListExt.ConvertToListT<IHistoricProcessInstance>(DbEntityManager.SelectList("selectHistoricProcessInstancesByQueryCriteria", historicProcessInstanceQuery, page)) ;
        //              throw new System.NotImplementedException();
        //}
        //return new List<IHistoricProcessInstance>(); // Collections.EMPTY_LIST;
        // }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricProcessInstance> findHistoricProcessInstancesByNativeQuery(java.Util.Map<String, Object> parameterMap, int firstResult, int maxResults)
        public virtual IList<IHistoricProcessInstance> FindHistoricProcessInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
        {
            //return ListExt.ConvertToListT<IHistoricProcessInstance>(DbEntityManager.SelectListWithRawParameter("selectHistoricProcessInstanceByNativeQuery", parameterMap, firstResult, maxResults));
            throw new System.NotImplementedException();
        }

        public List<string> FindHistoricProcessInstanceIdsForCleanup(int batchSize)
        {
            throw new System.NotImplementedException();
            //ListQueryParameterObject parameterObject = new ListQueryParameterObject();
            //parameterObject.setParameter(ClockUtil.getCurrentTime());
            //parameterObject.setFirstResult(0);
            //parameterObject.setMaxResults(batchSize);
            //return (List<String>)getDbEntityManager().selectList("selectHistoricProcessInstanceIdsForCleanup", parameterObject);
        }
        public long FindHistoricProcessInstanceIdsForCleanupCount()
        {
            throw new System.NotImplementedException();
            //ListQueryParameterObject parameterObject = new ListQueryParameterObject();
            //parameterObject.setParameter(ClockUtil.getCurrentTime());
            //return (Long)getDbEntityManager().selectOne("selectHistoricProcessInstanceIdsForCleanupCount", parameterObject);
        }
        public virtual long FindHistoricProcessInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
        {
            throw new System.NotImplementedException();
            //return (long) DbEntityManager.SelectOne("selectHistoricProcessInstanceCountByNativeQuery", parameterMap);
        }

        // protected internal virtual void ConfigureQuery(HistoricProcessInstanceQueryImpl query)
        // {
        //AuthorizationManager.ConfigureHistoricProcessInstanceQuery(query);
        //TenantManager.ConfigureQuery(query);
        // }

        public IQueryable<HistoricProcessInstanceEventEntity> VariableValueEquals(string key, string value)
        {
            //17:37:38.181 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==>  Preparing: select distinct RES.* from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE EXISTS ( select ID_ from ACT_HI_VARINST WHERE NAME_= ? AND PROC_INST_ID_ = SELF.PROC_INST_ID_ and ( ( VAR_TYPE_ is not null and VAR_TYPE_ = ? and TEXT_ is not null and TEXT_ = ? ) ) ) ) RES order by RES.ID_ asc LIMIT ? OFFSET ? 
            //17:37:38.210[main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==> Parameters: stringVar(String), string(String), abcdef(String), 2147483647(Integer), 0(Integer)
            var db = DbContext;
            var query = from self in db.Set<HistoricProcessInstanceEventEntity>()
                        join def in db.Set<ProcessDefinitionEntity>() on self.ProcessDefinitionId equals def.Id
                        into temp
                        where db.Set<HistoricVariableInstanceEntity>().Any(m=>m.Name==key&&m.ProcessInstanceId==self.ProcessInstanceId && (m.SerializerName == "string" && m.TextValue != null && m.TextValue == value))
                        from r in temp.DefaultIfEmpty()
                        select self;
            return query;
        }
        public IQueryable<HistoricProcessInstanceEventEntity> VariableValueNotEquals(string key, string value)
        {
            //16:09:02.559 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==>  Preparing: select distinct RES.* from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE EXISTS ( select ID_ from ACT_HI_VARINST WHERE NAME_= ? AND PROC_INST_ID_ = SELF.PROC_INST_ID_ and NOT ( ( VAR_TYPE_ is not null and VAR_TYPE_ = ? and TEXT_ is not null and TEXT_ = ? ) ) ) ) RES order by RES.ID_ asc LIMIT ? OFFSET ? 
            //16:09:02.598[main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==> Parameters: stringVar(String), string(String), abcdef(String), 2147483647(Integer), 0(Integer)
            var db = DbContext;
            var query = //from var in db.Set<HistoricVariableInstanceEntity>() select var
                        from self in db.Set<HistoricProcessInstanceEventEntity>()
                        join def in db.Set<ProcessDefinitionEntity>() on self.ProcessDefinitionId equals def.Id
                        into temp
                        where db.Set<HistoricVariableInstanceEntity>().Any(var=>var.Name == key && var.ProcessInstanceId == self.ProcessInstanceId && !(var.SerializerName != null && var.SerializerName == "string" && var.TextValue != null && var.TextValue == value))
                        from r in temp.DefaultIfEmpty()
                        select self;
            return query;
        }
        public IQueryable<HistoricProcessInstanceEventEntity> VariableValueEquals(IDictionary<string, string> dic)
        {
            //09:41:31.432 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==>  Preparing: select distinct RES.* from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE EXISTS ( select ID_ from ACT_HI_VARINST WHERE NAME_= ? AND PROC_INST_ID_ = SELF.PROC_INST_ID_ and ( ( VAR_TYPE_ is not null and VAR_TYPE_ = ? and TEXT_ is not null and TEXT_ = ? ) ) ) and EXISTS ( select ID_ from ACT_HI_VARINST WHERE NAME_= ? AND PROC_INST_ID_ = SELF.PROC_INST_ID_ and ( ( VAR_TYPE_ is not null and VAR_TYPE_ = ? and TEXT_ is not null and TEXT_ = ? ) ) ) ) RES order by RES.ID_ asc LIMIT ? OFFSET ? 
            //09:41:31.501[main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==> Parameters: stringVar(String), string(String), abcdef(String), stringVar2(String), string(String), ghijkl(String), 2147483647(Integer), 0(Integer)
            throw new System.NotImplementedException();
        }
        public IQueryable<HistoricProcessInstanceEventEntity> ProcessInstanceIds(IEnumerable<string> processInstanceIds)
        {
            EnsureUtil.EnsureNotEmpty("Set of process instance ids", processInstanceIds);
            //14:54:37.502 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstanceCountByQueryCriteria - ==>  Preparing: select count(distinct RES.ID_) from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE SELF.PROC_INST_ID_ in ( ? , ? , ? , ? , ? ) ) RES 
            var db = DbContext;
            var query = from self in db.Set<HistoricProcessInstanceEventEntity>()
                        join def in db.Set<ProcessDefinitionEntity>() on self.ProcessDefinitionId equals def.Id
                        into temp
                        where processInstanceIds.Contains(self.ProcessInstanceId)
                        from r in temp.DefaultIfEmpty()
                        select self;
            //select new
            //{
            //    Self = self,
            //    DefName = r == null ? "" : r.Name,
            //    DefVersion = r == null ? 0 : r.Version
            //};
            return query;
        }
        public IQueryable<HistoricProcessInstanceEventEntity> VariableValueLessThan(string key,string value)
        {
            //11:08:48.861 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==>  Preparing: select distinct RES.* from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE EXISTS ( select ID_ from ACT_HI_VARINST WHERE NAME_= ? AND PROC_INST_ID_ = SELF.PROC_INST_ID_ and ( ( VAR_TYPE_ is not null and VAR_TYPE_ = ? and TEXT_ is not null and TEXT_ < ? ) ) ) ) RES order by RES.ID_ asc LIMIT ? OFFSET ? 
            //11:08:48.891[main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==> Parameters: stringVar(String), string(String), abcdeg(String), 2147483647(Integer), 0(Integer)
            var db = DbContext;
            var query =
                        from self in db.Set<HistoricProcessInstanceEventEntity>()
                        join def in db.Set<ProcessDefinitionEntity>() on self.ProcessDefinitionId equals def.Id
                        into temp
                        where db.Set<HistoricVariableInstanceEntity>().Any(var => var.Name == key && var.ProcessInstanceId == self.ProcessInstanceId && (var.SerializerName == "string" && var.TextValue != null && var.TextValue.CompareTo(value) < 0))
                        from r in temp.DefaultIfEmpty()
                        select self;
            return query;
        }
        public IQueryable<HistoricProcessInstanceEventEntity> VariableValueGreaterThan(string key, string value)
        {
            //16:23:42.703 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==>  Preparing: select distinct RES.* from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE EXISTS ( select ID_ from ACT_HI_VARINST WHERE NAME_= ? AND PROC_INST_ID_ = SELF.PROC_INST_ID_ and ( ( VAR_TYPE_ is not null and VAR_TYPE_ = ? and TEXT_ is not null and TEXT_ > ? ) ) ) ) RES order by RES.ID_ asc LIMIT ? OFFSET ? 
            //16:23:42.731[main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==> Parameters: stringVar(String), string(String), abcdef(String), 2147483647(Integer), 0(Integer)
            var db = DbContext;
            var query =
                        from self in db.Set<HistoricProcessInstanceEventEntity>()
                        join def in db.Set<ProcessDefinitionEntity>() on self.ProcessDefinitionId equals def.Id
                        into temp
                        where db.Set<HistoricVariableInstanceEntity>().Any(var=>var.Name == key && var.ProcessInstanceId == self.ProcessInstanceId && (var.SerializerName == "string" && var.TextValue != null &&  var.TextValue.CompareTo(value)>0))
                        from r in temp.DefaultIfEmpty()
                        select self;
            return query;
        }
        public IQueryable<HistoricProcessInstanceEventEntity> VariableValueGreaterThanOrEqual(string key, string value)
        {
            //16:46:37.019 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstanceCountByQueryCriteria - ==>  Preparing: select count(distinct RES.ID_) from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE EXISTS ( select ID_ from ACT_HI_VARINST WHERE NAME_= ? AND PROC_INST_ID_ = SELF.PROC_INST_ID_ and ( ( VAR_TYPE_ is not null and VAR_TYPE_ = ? and TEXT_ is not null and TEXT_ >= ? ) ) ) ) RES 
            //16:46:37.052[main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstanceCountByQueryCriteria - ==> Parameters: stringVar(String), string(String), abcdef(String)
            var db = DbContext;
            var query = from self in db.Set<HistoricProcessInstanceEventEntity>()
                        join def in db.Set<ProcessDefinitionEntity>() on self.ProcessDefinitionId equals def.Id
                        into temp
                        where db.Set<HistoricVariableInstanceEntity>().Any(m => m.Name == key&&self.ProcessInstanceId==m.ProcessInstanceId && (m.SerializerName == "string" && m.TextValue != null && m.TextValue.CompareTo(value) >= 0))
                        from r in temp.DefaultIfEmpty()
                        select self;
            return query;
        }
        public IQueryable<HistoricProcessInstanceEventEntity> VariableValueLessThanOrEqual(string key, string value)
        {
            //16:58:05.556 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstanceCountByQueryCriteria - ==>  Preparing: select count(distinct RES.ID_) from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE EXISTS ( select ID_ from ACT_HI_VARINST WHERE NAME_= ? AND PROC_INST_ID_ = SELF.PROC_INST_ID_ and ( ( VAR_TYPE_ is not null and VAR_TYPE_ = ? and TEXT_ is not null and TEXT_ <= ? ) ) ) ) RES 
            //16:58:05.584[main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstanceCountByQueryCriteria - ==> Parameters: stringVar(String), string(String), z(String)
            var db = DbContext;
            var query = from self in db.Set<HistoricProcessInstanceEventEntity>()
                        join def in db.Set<ProcessDefinitionEntity>() on self.ProcessDefinitionId equals def.Id
                        into temp
                        where db.Set<HistoricVariableInstanceEntity>().Any(m => m.Name == key && self.ProcessInstanceId == m.ProcessInstanceId && (m.SerializerName == "string" && m.TextValue != null && m.TextValue.CompareTo(value) <= 0))
                        from r in temp.DefaultIfEmpty()
                        select self;
            return query;
        }

        public IQueryable<HistoricProcessInstanceEventEntity> VariableValueLike(string key, string value)
        {
            //11:25:54.417 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==>  Preparing: select distinct RES.* from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE EXISTS ( select ID_ from ACT_HI_VARINST WHERE NAME_= ? AND PROC_INST_ID_ = SELF.PROC_INST_ID_ and ( ( VAR_TYPE_ is not null and VAR_TYPE_ = ? and TEXT_ is not null and TEXT_ LIKE ? ESCAPE ? ) ) ) ) RES order by RES.ID_ asc LIMIT ? OFFSET ? 
            //11:25:54.447[main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==> Parameters: stringVar(String), string(String), azert % (String), \(String), 2147483647(Integer), 0(Integer)
            var db = DbContext;
            var query = from self in db.Set<HistoricProcessInstanceEventEntity>()
                        join def in db.Set<ProcessDefinitionEntity>()
                        on self.ProcessDefinitionId equals def.Id
                        into temp
                        where db.Set<HistoricVariableInstanceEntity>().Any(m => m.Name == key && m.ProcessInstanceId == self.ProcessInstanceId && (m.SerializerName == "string" && m.TextValue != null && m.TextValue.Contains(value)))
                        select self;
            return query;
        }
    }

}