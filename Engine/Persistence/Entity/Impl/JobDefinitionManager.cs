using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Management;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Common.Extensions;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{

    /// <summary>
    /// <para>Manager implementation for <seealso cref="JobDefinitionEntity"/></para>
    /// 
    /// 
    /// 
    /// </summary>
    [Component]
    public class JobDefinitionManager : AbstractManagerNet<JobDefinitionEntity>, IJobDefinitionManager
    {
        protected ITenantManager tenantManager;
        public JobDefinitionManager(DbContext dbContex, ILoggerFactory loggerFactory, ITenantManager _tenantManager, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
            tenantManager = _tenantManager;
        }

        public virtual JobDefinitionEntity FindById(string jobDefinitionId)
        {
            return Get(jobDefinitionId);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<JobDefinitionEntity> findByProcessDefinitionId(String processDefinitionId)
        public virtual IList<JobDefinitionEntity> FindByProcessDefinitionId(string processDefinitionId)
        {
            return Find(m => m.ProcessDefinitionId == processDefinitionId).ToList();
        }

        public virtual void DeleteJobDefinitionsByProcessDefinitionId(string id)
        {
            Delete(m => m.ProcessDefinitionId == id);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.management.JobDefinition> findJobDefnitionByQueryCriteria(org.camunda.bpm.engine.impl.JobDefinitionQueryImpl jobDefinitionQuery, org.camunda.bpm.engine.impl.Page page)
        // public virtual IList<IJobDefinition> FindJobDefnitionByQueryCriteria(JobDefinitionQueryImpl jobDefinitionQuery, Page page)
        // {
        //ConfigureQuery(jobDefinitionQuery);
        //return ListExt.ConvertToListT<IJobDefinition>(DbEntityManager.SelectList("selectJobDefinitionByQueryCriteria", jobDefinitionQuery, page));
        // }

        // public virtual long FindJobDefinitionCountByQueryCriteria(JobDefinitionQueryImpl jobDefinitionQuery)
        // {
        //ConfigureQuery(jobDefinitionQuery);
        //return  (long)DbEntityManager.SelectOne("selectJobDefinitionCountByQueryCriteria", jobDefinitionQuery);
        // }

        public virtual void UpdateJobDefinitionSuspensionStateById(string jobDefinitionId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["jobDefinitionId"] = jobDefinitionId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(JobDefinitionEntity), "updateJobDefinitionSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));

            //TODO 更新待优化 Update JobDef.SuspensionState
            Find(m => m.Id == jobDefinitionId).ForEach((t) =>
            {
                t.Revision = t.Revision + 1;
                t.SuspensionState = t.SuspensionState = suspensionState.StateCode;
            });

        }

        public virtual void UpdateJobDefinitionSuspensionStateByProcessDefinitionId(string processDefinitionId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionId"] = processDefinitionId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(JobDefinitionEntity), "updateJobDefinitionSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));

            //TODO 更新待优化 Update JobDef.SuspensionState
            Find(m => m.ProcessDefinitionId == processDefinitionId).ForEach((t) =>
            {
                t.Revision = t.Revision + 1;
                t.SuspensionState = t.SuspensionState = suspensionState.StateCode;
            });
        }

        public virtual void UpdateJobDefinitionSuspensionStateByProcessDefinitionKey(string processDefinitionKey, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["isProcessDefinitionTenantIdSet"] = false;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(JobDefinitionEntity), "updateJobDefinitionSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));

            //TODO 更新待优化 Update JobDef.SuspensionState
            Find(m => m.ProcessDefinitionKey == processDefinitionKey).ForEach((t) =>
            {
                t.Revision = t.Revision + 1;
                t.SuspensionState = t.SuspensionState = suspensionState.StateCode;
            });
        }

        public virtual void UpdateJobDefinitionSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["isProcessDefinitionTenantIdSet"] = true;
            //parameters["processDefinitionTenantId"] = processDefinitionTenantId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(JobDefinitionEntity), "updateJobDefinitionSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));

            //TODO 更新待优化 Update JobDef.SuspensionState
            throw new NotImplementedException("需要关联ACT_RE_PROCDEF");
            //Find(m => m.ProcessDefinitionKey == processDefinitionKey&&m.ProcessDefinitionTenantId).ForEach((t) =>
            //{
            //    t.Revision = t.Revision + 1;
            //    t.SuspensionState = t.SuspensionState = suspensionState.StateCode;
            //});
        }

        // protected internal virtual void ConfigureQuery(JobDefinitionQueryImpl query)
        // {
        //AuthorizationManager.ConfigureJobDefinitionQuery(query);
        //TenantManager.ConfigureQuery(query);
        // }

        //protected internal virtual ListQueryParameterObject ConfigureParameterizedQuery(object parameter)
        //{
        //    return tenantManager.ConfigureQuery(parameter);
        //}

    }

}