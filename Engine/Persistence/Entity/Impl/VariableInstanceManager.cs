//using JZTERP.Common.Shared.Entities.Bpm.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using Microsoft.Extensions.Logging;
using ESS.FW.Common.Components;
using ESS.FW.Bpm.Engine.Impl.Core.Variable;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    [Component]
    public class VariableInstanceManager : AbstractManagerNet<VariableInstanceEntity>, IVariableInstanceManager 
    {

        public VariableInstanceManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual IList<VariableInstanceEntity> FindVariableInstancesByTaskId(string taskId)
        {
            //return ListExt.ConvertToListT<VariableInstanceEntity>(DbEntityManager.SelectList("selectVariablesByTaskId", taskId));
            return Find(m => m.TaskId == taskId).ToList();
        }

        public virtual IList<VariableInstanceEntity> FindVariableInstancesByExecutionId(string executionId)
        {
            //return ListExt.ConvertToListT<VariableInstanceEntity>(DbEntityManager.SelectList("selectVariablesByExecutionId", executionId));

            //TODO <sql id="actInstIdColumn">
            var query = from v in DbContext.Set<VariableInstanceEntity>()
                        join e in DbContext.Set<ExecutionEntity>() on v.ExecutionId equals e.Id into ejoin
                        from ee in ejoin.DefaultIfEmpty()
                        join p in DbContext.Set<ExecutionEntity>() on ee.ParentId equals p.Id into pjoin
                        from pp in pjoin.DefaultIfEmpty()
                        where v.ExecutionId == executionId && v.TaskId == null
                        select v;
            return query.ToList();
        }

        public virtual IList<VariableInstanceEntity> FindVariableInstancesByProcessInstanceId(string processInstanceId)
        {
            //return ListExt.ConvertToListT<VariableInstanceEntity>(DbEntityManager.SelectList("selectVariablesByProcessInstanceId", processInstanceId));
            return Find(m => m.ProcessInstanceId == processInstanceId).ToList();
        }
        public virtual VariableInstanceEntity FindVariableInstancesById(string id)
        {
            return DbSet.Find(id);
        }
        public virtual IList<VariableInstanceEntity> FindVariableInstancesByCaseExecutionId(string caseExecutionId)
        {
            //return ListExt.ConvertToListT<VariableInstanceEntity>(DbEntityManager.SelectList("selectVariablesByCaseExecutionId", caseExecutionId));
            return Find(m => m.CaseExecutionId == caseExecutionId).ToList();
        }

        public virtual void DeleteVariableInstanceByTask(TaskEntity task)
        {

            IList<ICoreVariableInstance> variableInstances = task.VariableStore.Variables;
            foreach (VariableInstanceEntity variableInstance in variableInstances)
            {
                variableInstance.Delete();
            }
        }

        //public virtual long FindVariableInstanceCountByQueryCriteria(VariableInstanceQueryImpl variableInstanceQuery)
        //{
        //    ConfigureQuery(variableInstanceQuery);
        //    return (long)DbEntityManager.SelectOne("selectVariableInstanceCountByQueryCriteria", variableInstanceQuery);
        //}

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        // ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.runtime.VariableInstance> findVariableInstanceByQueryCriteria(org.camunda.bpm.engine.impl.VariableInstanceQueryImpl variableInstanceQuery, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<org.camunda.bpm.engine.runtime.IVariableInstance> FindVariableInstanceByQueryCriteria(VariableInstanceQueryImpl variableInstanceQuery, Page page)
        //{
        //    ConfigureQuery(variableInstanceQuery);
        //    return ListExt.ConvertToListT<IVariableInstance>(DbEntityManager.SelectList("selectVariableInstanceByQueryCriteria", variableInstanceQuery, page));
        //}

        //protected internal virtual void ConfigureQuery(VariableInstanceQueryImpl query)
        //{

        //    AuthorizationManager.ConfigureVariableInstanceQuery(query);
        //    TenantManager.ConfigureQuery(query);
        //}

    }
}
