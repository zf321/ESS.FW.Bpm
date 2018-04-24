using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Task;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.EntityFramework.Maps;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    /// <summary>
	///  Stefan Hentschel
	/// 
	/// </summary>
    [Component]
    public class TaskReportManager : AbstractManagerNet<TaskEntity>, ITaskReportManager
    {
        public TaskReportManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual IList<ITaskCountByCandidateGroupResult> CreateTaskCountByCandidateGroupReport(
            TaskReportImpl query)
        {
            //throw new NotImplementedException();
            //ConfigureQuery(query);

            var queryTask = Find(o => o.Assignee == null);
            var resule = from o in queryTask
                         join n in DbContext.Set<IdentityLinkEntity>() on o.Id equals n.TaskId
                         group n by n.GroupId
                into g
                orderby  g.Key
                select new TaskCountByCandidateGroupResultEntity()
                {
                    TaskCount = g.Count(),
                    GroupName = g.Key
                };

            return resule.ToList<ITaskCountByCandidateGroupResult>();
            // return ListExt.ConvertToListT<ITaskCountByCandidateGroupResult>(DbEntityManager.SelectListWithRawParameter("selectTaskCountByCandidateGroupReportQuery", query, 0, int.MaxValue));
        }

        public virtual IList<IHistoricTaskInstanceReportResult> SelectHistoricTaskInstanceCountByTaskNameReport(HistoricTaskInstanceReportImpl query)
        {
            var pdQuery = from a in DbContext.Set<ProcessDefinitionEntity>()
                          group a by new { a.Key, a.TenantId }
                into g  select
                      new
                      {
                          Key = g.Key.Key,
                          TenantId = g.Key.TenantId,
                          Version = g.Max(o => o.Version)
                      };

            var tempQuery = from a in DbContext.Set<HistoricTaskInstanceEventEntity>()
                             join b in pdQuery
                             on a.ProcessDefinitionKey equals b.Key
                             where
                             a.TenantId == b.TenantId || (string.IsNullOrEmpty(a.TenantId) && string.IsNullOrEmpty(b.TenantId))
                             select new
                             {
                                 a.Id,
                                 a.Name,
                                 b.TenantId,
                                 b.Version,
                                 a.ProcessDefinitionKey
                             };
            var innerQuery = from q in tempQuery
                             group q by new {q.Name, q.ProcessDefinitionKey, q.Version, q.TenantId}
                into g
                select new
                {
                  Name=  g.Key.Name,
                  Key=  g.Key.ProcessDefinitionKey,
                  Version=  g.Key.Version,
                 TenantId=   g.Key.TenantId,
                  Count=  g.Count(),
                };

            return (from a in innerQuery
                join b in DbContext.Set<ProcessDefinitionEntity>() on
                new
                {
                    a.Key,
                    a.Version,
                    a.TenantId
                }
                equals
                new
                {
                    b.Key,
                    b.Version,
                    b.TenantId
                }
                where !string.IsNullOrEmpty(a.Name)
                orderby a.Name, a.Count
                select new TaskReportResultEntity()
                {
                   Count=a.Count,
                   ProcessDefinitionKey=b.Key,
                   ProcessDefinitionId=b.Id,
                   ProcessDefinitionName=b.Name,
                   TaskName= a.Name,
                   TenantId=a.TenantId,
                 }).ToList<IHistoricTaskInstanceReportResult>();

            //return result;
            //ConfigureQuery(query);
            //return ListExt.ConvertToListT<IHistoricTaskInstanceReportResult>(DbEntityManager.SelectListWithRawParameter("selectHistoricTaskInstanceCountByTaskNameReport", query, 0, int.MaxValue));
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult> selectHistoricTaskInstanceCountByProcDefKeyReport(org.camunda.bpm.engine.impl.HistoricTaskInstanceReportImpl query)
        public virtual IList<IHistoricTaskInstanceReportResult> SelectHistoricTaskInstanceCountByProcDefKeyReport(HistoricTaskInstanceReportImpl query)
        {
            throw new NotImplementedException();
            //ConfigureQuery(query);
            //return ListExt.ConvertToListT<IHistoricTaskInstanceReportResult>(DbEntityManager.SelectListWithRawParameter("selectHistoricTaskInstanceCountByProcDefKeyReport", query, 0, int.MaxValue));
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.DurationReportResult> createHistoricTaskDurationReport(org.camunda.bpm.engine.impl.HistoricTaskInstanceReportImpl query)
        public virtual IList<IDurationReportResult> CreateHistoricTaskDurationReport(HistoricTaskInstanceReportImpl query)
        {
            throw new NotImplementedException();
            //ConfigureQuery(query);
           // return ListExt.ConvertToListT<IDurationReportResult>(DbEntityManager.SelectListWithRawParameter("selectHistoricTaskInstanceDurationReport", query, 0, int.MaxValue));
        }

        //protected internal virtual void ConfigureQuery(HistoricTaskInstanceReportImpl parameter)
        //{
        //    AuthorizationManager.CheckAuthorization(Permissions.ReadHistory, Resources.Task, AuthorizationFields.Any);
        //    TenantManager.ConfigureTenantCheck(parameter.TenantCheck);
        //}

        //protected internal virtual void ConfigureQuery(TaskReportImpl parameter)
        //{
        //    AuthorizationManager.CheckAuthorization(Permissions.Read, Resources.Task, AuthorizationFields.Any);
        //    TenantManager.ConfigureTenantCheck(parameter.TenantCheck);
        //}

    }

}