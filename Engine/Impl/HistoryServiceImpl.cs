using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl.History;
using ESS.FW.Bpm.Engine.Dmn.Impl.Cmd;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Cmd.Batch;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Query;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Batch.History;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class HistoryServiceImpl : ServiceImpl, IHistoryService
    {
        public IQueryable<IHistoricProcessInstance> CreateHistoricProcessInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<HistoricProcessInstanceEventEntity>(expression));
        }
        public IQueryable<IHistoricProcessInstance> CreateHistoricProcessInstanceQuerySuperProcessInstanceId (string processInstanceId,Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression=null)
        {
            //09:09:39.660 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==>  Preparing: select distinct RES.* from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE SELF.SUPER_PROCESS_INSTANCE_ID_ = ? ) RES order by RES.ID_ asc LIMIT ? OFFSET ?
            var db = CommandExecutor.Execute(new GetDbContextCmd());
            var query = from a in db.Set<HistoricProcessInstanceEventEntity>()
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        into temp
                        where a.SuperProcessInstanceId == processInstanceId
                        from r in temp.DefaultIfEmpty()
                        select a;
            if (expression == null)
                return query;
            else
                return query.Where(expression);
            //return CommandExecutor.Execute(new CreateQueryCmd<HistoricProcessInstanceEventEntity>(expression));
        }

        public IQueryable<IHistoricActivityInstance> CreateHistoricActivityInstanceQuery(Expression<Func<HistoricActivityInstanceEventEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<HistoricActivityInstanceEventEntity>(expression));
            //return new HistoricActivityInstanceQueryImpl(CommandExecutor);
        }

        //public  IQueryable<IHistoricActivityStatistics> CreateHistoricActivityStatisticsQuery(string processDefinitionId)
        //{
        //    Expression<Func<HistoricActivityStatisticsImpl, bool>> expression = c => c.Id == processDefinitionId;

        //    return CommandExecutor.Execute(new CreateQueryCmd<HistoricActivityStatisticsImpl>(expression));

        //    //  return new HistoricActivityStatisticsQueryImpl(processDefinitionId, CommandExecutor);
        //}

        public IQueryable<IHistoricActivityStatistics> CreateHistoricActivityStatisticsQuery(Expression<Func<HistoricActivityStatisticsImpl, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<HistoricActivityStatisticsImpl>(expression));
            //  return new HistoricActivityStatisticsQueryImpl(processDefinitionId, CommandExecutor);
        }

        public IQueryable<IHistoricCaseActivityStatistics> CreateHistoricCaseActivityStatisticsQuery(Expression<Func<HistoricCaseActivityStatisticsImpl, bool>> expression)
        {
            // return new HistoricCaseActivityStatisticsQueryImpl(caseDefinitionId, CommandExecutor);
            return CommandExecutor.Execute(new CreateQueryCmd<HistoricCaseActivityStatisticsImpl>(expression));
        }

        public IQueryable<IHistoricTaskInstance> CreateHistoricTaskInstanceQuery(Expression<Func<History.Impl.Event.HistoricTaskInstanceEventEntity, bool>> expression)
        {
            //  return new HistoricTaskInstanceQueryImpl(CommandExecutor);
            return base.CommandExecutor.Execute(new CreateQueryCmd<History.Impl.Event.HistoricTaskInstanceEventEntity>(expression));
        }
        [Obsolete("子类查询需使用泛型方法CreateHistoricDetailQuery<T>")]
        public IQueryable<IHistoricDetail> CreateHistoricDetailQuery(Expression<Func<HistoricDetailEventEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<HistoricDetailEventEntity>(expression));
        }
        public IQueryable<IHistoricDetail> CreateHistoricDetailQuery<T>(Expression<Func<T, bool>> expression) where T: HistoricDetailEventEntity, new()
        {
            return CommandExecutor.Execute(new CreateQueryCmd<T>(expression));
        }

        public IQueryable<IUserOperationLogEntry> CreateUserOperationLogQuery(Expression<Func<UserOperationLogEntryEventEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<UserOperationLogEntryEventEntity>(expression));
        }

        public IQueryable<IHistoricVariableInstance> CreateHistoricVariableInstanceQuery(Expression<Func<HistoricVariableInstanceEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<HistoricVariableInstanceEntity>(expression));
            // return new HistoricVariableInstanceQueryImpl(CommandExecutor);
        }

        public IQueryable<IHistoricIncident> CreateHistoricIncidentQuery(Expression<Func<HistoricIncidentEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<HistoricIncidentEntity>(expression));
            //  return new HistoricIncidentQueryImpl(CommandExecutor);
        }

        public IQueryable<IHistoricCaseInstance> CreateHistoricCaseInstanceQuery(Expression<Func<HistoricCaseInstanceEventEntity, bool>> expression)
        {
            return base.CommandExecutor.Execute(new CreateQueryCmd<HistoricCaseInstanceEventEntity>(expression));

            //  return new HistoricCaseInstanceQueryImpl(CommandExecutor);
        }

        public IQueryable<IHistoricCaseActivityInstance> createHistoricCaseActivityInstanceQuery(Expression<Func<HistoricCaseActivityInstanceEventEntity, bool>> expression)
        {
            return base.CommandExecutor.Execute(new CreateQueryCmd<HistoricCaseActivityInstanceEventEntity>(expression));

            // return new HistoricCaseActivityInstanceQueryImpl(commandExecutor);
        }

        public IQueryable<IHistoricDecisionInstance> CreateHistoricDecisionInstanceQuery(Expression<Func<HistoricDecisionInstanceEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<HistoricDecisionInstanceEntity>(expression));

            // return new HistoricDecisionInstanceQueryImpl(CommandExecutor);
        }

        public void DeleteHistoricTaskInstance(string taskId)
        {
            CommandExecutor.Execute(new DeleteHistoricTaskInstanceCmd(taskId));
        }

        public void DeleteHistoricProcessInstance(string processInstanceId)
        {
            CommandExecutor.Execute(new DeleteHistoricProcessInstanceCmd(processInstanceId));
        }

        public void DeleteHistoricProcessInstances(IList<string> processInstanceIds)
        {
            CommandExecutor.Execute(new DeleteHistoricProcessInstancesCmd(processInstanceIds));
        }

        public IBatch DeleteHistoricProcessInstancesAsync(IList<string> processInstanceIds, string deleteReason)
        {
            return DeleteHistoricProcessInstancesAsync(processInstanceIds, null, deleteReason);
        }

        public IBatch DeleteHistoricProcessInstancesAsync(IQueryable<IHistoricProcessInstance> query, string deleteReason)
        {
            return DeleteHistoricProcessInstancesAsync(null, query, deleteReason);
        }

        public IBatch DeleteHistoricProcessInstancesAsync(IList<string> processInstanceIds,
            IQueryable<IHistoricProcessInstance> query, string deleteReason)
        {
            return
                CommandExecutor.Execute(new DeleteHistoricProcessInstancesBatchCmd(processInstanceIds, query,
                    deleteReason));
        }

        public void DeleteUserOperationLogEntry(string entryId)
        {
            CommandExecutor.Execute(new DeleteUserOperationLogEntryCmd(entryId));
        }

        public void DeleteHistoricCaseInstance(string caseInstanceId)
        {
            CommandExecutor.Execute(new DeleteHistoricCaseInstanceCmd(caseInstanceId));
        }

        public void DeleteHistoricDecisionInstance(string decisionDefinitionId)
        {
            DeleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);
        }

        public void DeleteHistoricDecisionInstanceByDefinitionId(string decisionDefinitionId)
        {
            CommandExecutor.Execute(new DeleteHistoricDecisionInstanceByDefinitionIdCmd(decisionDefinitionId));
        }

        public void DeleteHistoricDecisionInstanceByInstanceId(string historicDecisionInstanceId)
        {
            CommandExecutor.Execute(new DeleteHistoricDecisionInstanceByInstanceIdCmd(historicDecisionInstanceId));
        }

        //public  IQueryable<IHistoricProcessInstance> CreateNativeHistoricProcessInstanceQuery()
        //{
        //    return new NativeHistoricProcessInstanceQueryImpl(CommandExecutor);
        //}

        //public  IQueryable<IHistoricTaskInstance> CreateNativeHistoricTaskInstanceQuery()
        //{
        //    return new NativeHistoricTaskInstanceQueryImpl(CommandExecutor);
        //}

        //public  IQueryable<IHistoricActivityInstance> CreateNativeHistoricActivityInstanceQuery()
        //{
        //    return new NativeHistoricActivityInstanceQueryImpl(CommandExecutor);
        //}

        //public  IQueryable<IHistoricCaseInstance> createNativeHistoricCaseInstanceQuery()
        //{
        //    return new NativeHistoricCaseInstanceQueryImpl(commandExecutor);
        //}

        //public  IQueryable<IHistoricCaseActivityInstance> CreateNativeHistoricCaseActivityInstanceQuery()
        //{
        //    return new NativeHistoricCaseActivityInstanceQueryImpl(CommandExecutor);
        //}

        //public  IQueryable<IHistoricDecisionInstance> CreateNativeHistoricDecisionInstanceQuery()
        //{
        //    return new NativeHistoryDecisionInstanceQueryImpl(CommandExecutor);
        //}

        //public  IQueryable<IHistoricJobLog> CreateHistoricJobLogQuery()
        //{
        //    return new HistoricJobLogQueryImpl(CommandExecutor);
        //}

        public string GetHistoricJobLogExceptionStacktrace(string historicJobLogId)
        {
            return CommandExecutor.Execute(new GetHistoricJobLogExceptionStacktraceCmd(historicJobLogId));
        }

        public IHistoricProcessInstanceReport CreateHistoricProcessInstanceReport()
        {
            return new HistoricProcessInstanceReportImpl(CommandExecutor);
        }

        public IHistoricTaskInstanceReport CreateHistoricTaskInstanceReport()
        {
            return new HistoricTaskInstanceReportImpl(CommandExecutor);
        }

        //public  IHistoricBatchQuery CreateHistoricBatchQuery()
        //{
        //    return new HistoricBatchQueryImpl(CommandExecutor);
        //}

        public void DeleteHistoricBatch(string batchId)
        {
            CommandExecutor.Execute(new DeleteHistoricBatchCmd(batchId));
        }

        public IQueryable<IHistoricDecisionInstanceStatistics> CreateHistoricDecisionInstanceStatisticsQuery(
           Expression<Func<DecisionInstanceStatisticsImpl, bool>> expression)// string decisionRequirementsDefinitionId
        {
            //TODO
            //Expression<Func<DecisionInstanceStatisticsImpl, bool>> expression = c => true;
            //throw new NotImplementedException();
            return CommandExecutor.Execute(new CreateQueryCmd<DecisionInstanceStatisticsImpl>(expression));
        }

        public IQueryable<IHistoricExternalTaskLog> CreateHistoricExternalTaskLogQuery(Expression<Func<HistoricExternalTaskLogEntity, bool>> expression = null)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<HistoricExternalTaskLogEntity>(expression));
        }

        public string GetHistoricExternalTaskLogErrorDetails(string historicExternalTaskLogId)
        {
            return CommandExecutor.Execute(new GetHistoricExternalTaskLogErrorDetailsCmd(historicExternalTaskLogId));
        }

        public IJob CleanUpHistoryAsync()
        {
            return CleanUpHistoryAsync(false);
        }

        public IJob CleanUpHistoryAsync(bool immediatelyDue)
        {
            throw new NotImplementedException();
            //return CommandExecutor.Execute(new HistoryCleanupCmd(immediatelyDue));
        }

        public IJob FindHistoryCleanupJob()
        {
            return CommandExecutor.Execute(new FindHistoryCleanupJobCmd());
        }

        public void DeleteHistoricProcessInstance(Expression<Func<HistoricTaskInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public void DeleteHistoricDecisionInstance(Expression<Func<HistoricTaskInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public void DeleteHistoricDecisionInstanceByDefinitionId(Expression<Func<HistoricDecisionInstanceEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public void DeleteHistoricDecisionInstanceByInstanceId(Expression<Func<HistoricTaskInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricProcessInstance> CreateNativeHistoricProcessInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<HistoricProcessInstanceEventEntity>(expression));
        }

        public IQueryable<IHistoricTaskInstance> CreateNativeHistoricTaskInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricActivityInstance> CreateNativeHistoricActivityInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
            //return CommandExecutor.Execute(new CreateQueryCmd<HistoricProcessInstanceEventEntity>(expression));
        }

        public IQueryable<IHistoricCaseInstance> CreateNativeHistoricCaseInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricCaseActivityInstance> CreateNativeHistoricCaseActivityInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricDecisionInstance> CreateNativeHistoricDecisionInstanceQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricJobLog> CreateHistoricJobLogQuery(Expression<Func<HistoricJobLogEventEntity, bool>> expression = null)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<HistoricJobLogEventEntity>(expression));
        }

        public IHistoricProcessInstanceReport CreateHistoricProcessInstanceReport(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public IHistoricTaskInstanceReport CreateHistoricTaskInstanceReport(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricBatch> CreateHistoricBatchQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricDecisionInstanceStatistics> CreateHistoricDecisionInstanceStatisticsQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricExternalTaskLog> CreateHistoricExternalTaskLogQuery(Expression<Func<HistoricProcessInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public void DeleteHistoricDecisionInstancesBulk(IList<string> decisionInstanceIds)
        {
            throw new NotImplementedException();
        }

        public void DeleteHistoricProcessInstancesBulk(IList<string> processInstanceIds)
        {
            throw new NotImplementedException();
        }

        public void DeleteHistoricCaseInstancesBulk(IList<string> caseInstanceIds)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricIdentityLinkLog> CreateHistoricIdentityLinkLogQuery(Expression<Func<HistoricIdentityLinkLogEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricCaseActivityInstance> CreateHistoricCaseActivityInstanceQuery(Expression<Func<HistoricCaseActivityInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public void DeleteHistoricTaskInstance(Expression<Func<HistoricTaskInstanceEventEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricActivityStatistics> CreateHistoricActivityStatisticsQuery(string processDefinitionId)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IHistoricDecisionInstanceStatistics> CreateHistoricDecisionInstanceStatisticsQuery(string decisionRequirementsDefinitionId)
        {
            throw new NotImplementedException();
        }
    }
}