using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.EntityFramework;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class DeleteHistoricProcessInstancesCmd : ICommand<object>
    {
        protected internal readonly IList<string> ProcessInstanceIds;

        public DeleteHistoricProcessInstancesCmd(IList<string> processInstanceIds)
        {
            this.ProcessInstanceIds = processInstanceIds;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            //Query转换
            EnsureUtil.EnsureNotEmpty("processInstanceIds", ProcessInstanceIds);
            // Check if process instance is still running
            //var instances = commandContext.HistoricProcessInstanceManager.Find
            //  new HistoricProcessInstanceQueryImpl().ProcessInstanceIds(new HashSet<string>(ProcessInstanceIds))
            //    .list();
            DbContext db = commandContext.Scope.Resolve<DbContext>();
            var query = from a in db.Set<HistoricProcessInstanceEventEntity>()
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        where ProcessInstanceIds.Contains(a.ProcessInstanceId)
                        select a;
            var instances = query.ToList();

            EnsureUtil.EnsureNotEmpty("No historic process instances found ", instances);
            EnsureUtil.EnsureNumberOfElements("historic process instances", instances, ProcessInstanceIds.Count);

            foreach (var historicProcessInstance in instances)
            {
                foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                    checker.CheckDeleteHistoricProcessInstance(historicProcessInstance);

                EnsureUtil.EnsureNotNull(
                    "Process instance is still running, cannot delete historic process instance: " +
                    historicProcessInstance, "instance.getEndTime()", historicProcessInstance.EndTime);

                var toDelete = historicProcessInstance.Id;
                commandContext.HistoricProcessInstanceManager.DeleteHistoricProcessInstanceById(toDelete);
            }
            return null;
        }
    }
}