using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Cmd.Batch;
using ESS.FW.Bpm.Engine.Impl.migration;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace ESS.FW.Bpm.Engine.Impl
{

    /// <summary>
    ///     
    /// </summary>
    public class RuntimeServiceImpl : ServiceImpl, IRuntimeService
    {
        public virtual IProcessInstance StartProcessInstanceByKey(string processDefinitionKey)
        {
            return CreateProcessInstanceByKey(processDefinitionKey).ExecuteWithVariablesInReturn();//.Execute();
        }

        public virtual IProcessInstance StartProcessInstanceByKey(string processDefinitionKey, string businessKey)
        {
            return CreateProcessInstanceByKey(processDefinitionKey).SetBusinessKey(businessKey).ExecuteWithVariablesInReturn();//.Execute();
        }

        public virtual IProcessInstance StartProcessInstanceByKey(string processDefinitionKey, string businessKey,
            string caseInstanceId)
        {
            return
                CreateProcessInstanceByKey(processDefinitionKey)
                    .SetBusinessKey(businessKey)
                    .SetCaseInstanceId(caseInstanceId)
                    .ExecuteWithVariablesInReturn();
        }

        public virtual IProcessInstance StartProcessInstanceByKey(string processDefinitionKey,
            IDictionary<string, ITypedValue> variables)
        {
            return CreateProcessInstanceByKey(processDefinitionKey).SetVariables(variables).ExecuteWithVariablesInReturn();
        }
        public virtual IProcessInstance StartProcessInstanceByKey(string processDefinitionKey,
            IDictionary<string, object> variables)
        {
            return CreateProcessInstanceByKey(processDefinitionKey).SetVariables(VariableHelper.GetTypedValue(variables)).ExecuteWithVariablesInReturn();
        }

        public virtual IProcessInstance StartProcessInstanceByKey(string processDefinitionKey, string businessKey,
            IDictionary<string, ITypedValue> variables)
        {
            return
                CreateProcessInstanceByKey(processDefinitionKey)
                    .SetBusinessKey(businessKey)
                    .SetVariables(variables)
                    .ExecuteWithVariablesInReturn();
        }

        public virtual IProcessInstance StartProcessInstanceByKey(string processDefinitionKey, string businessKey,
            string caseInstanceId, IDictionary<string, ITypedValue> variables)
        {
            return
                CreateProcessInstanceByKey(processDefinitionKey)
                    .SetBusinessKey(businessKey)
                    .SetCaseInstanceId(caseInstanceId)
                    .SetVariables(variables)
                    .ExecuteWithVariablesInReturn();
        }

        public virtual IProcessInstance StartProcessInstanceById(string processDefinitionId)
        {
            return CreateProcessInstanceById(processDefinitionId).ExecuteWithVariablesInReturn();
        }

        public virtual IProcessInstance StartProcessInstanceById(string processDefinitionId, string businessKey)
        {
            return CreateProcessInstanceById(processDefinitionId).SetBusinessKey(businessKey).ExecuteWithVariablesInReturn();
        }

        public virtual IProcessInstance StartProcessInstanceById(string processDefinitionId, string businessKey,
            string caseInstanceId)
        {
            return
                CreateProcessInstanceById(processDefinitionId)
                    .SetBusinessKey(businessKey)
                    .SetCaseInstanceId(caseInstanceId)
                    .ExecuteWithVariablesInReturn();
        }

        public virtual IProcessInstance StartProcessInstanceById(string processDefinitionId,
            IDictionary<string, ITypedValue> variables)
        {
            return CreateProcessInstanceById(processDefinitionId).SetVariables(variables).ExecuteWithVariablesInReturn();
        }

        public virtual IProcessInstance StartProcessInstanceById(string processDefinitionId, string businessKey,
            IDictionary<string, ITypedValue> variables)
        {
            return
                CreateProcessInstanceById(processDefinitionId)
                    .SetBusinessKey(businessKey)
                    .SetVariables(variables)
                    .ExecuteWithVariablesInReturn();
        }

        public virtual IProcessInstance StartProcessInstanceById(string processDefinitionId, string businessKey,
            string caseInstanceId, IDictionary<string, ITypedValue> variables)
        {
            return
                CreateProcessInstanceById(processDefinitionId)
                    .SetBusinessKey(businessKey)
                    .SetCaseInstanceId(caseInstanceId)
                    .SetVariables(variables)
                    .ExecuteWithVariablesInReturn();
        }

        public virtual void DeleteProcessInstance(string processInstanceId, string deleteReason)
        {
            DeleteProcessInstance(processInstanceId, deleteReason, false);
        }

        public virtual IBatch DeleteProcessInstancesAsync(IList<string> processInstanceIds,
            IQueryable<IProcessInstance> processInstanceQuery, string deleteReason)
        {
            return
                CommandExecutor.Execute(new DeleteProcessInstanceBatchCmd(processInstanceIds, processInstanceQuery,
                    deleteReason));
        }

        public virtual IBatch DeleteProcessInstancesAsync(IList<string> processInstanceIds, string deleteReason)
        {
            return CommandExecutor.Execute(new DeleteProcessInstanceBatchCmd(processInstanceIds, null, deleteReason));
        }

        public virtual IBatch DeleteProcessInstancesAsync(IQueryable<IProcessInstance> processInstanceQuery, string deleteReason)
        {
            return CommandExecutor.Execute(new DeleteProcessInstanceBatchCmd(null, processInstanceQuery, deleteReason));
        }

        public virtual void DeleteProcessInstance(string processInstanceId, string deleteReason,
            bool skipCustomListeners)
        {
            DeleteProcessInstance(processInstanceId, deleteReason, skipCustomListeners, false);
        }

        public virtual void DeleteProcessInstance(string processInstanceId, string deleteReason,
            bool skipCustomListeners, bool externallyTerminated)
        {
            CommandExecutor.Execute(new DeleteProcessInstanceCmd(processInstanceId, deleteReason, skipCustomListeners,
                externallyTerminated));
        }

        public virtual void DeleteProcessInstances(IList<string> processInstanceIds, string deleteReason,
            bool skipCustomListeners, bool externallyTerminated)
        {
            CommandExecutor.Execute(new DeleteProcessInstancesCmd(processInstanceIds, deleteReason, skipCustomListeners,
                externallyTerminated));
        }
        /// <summary>
        /// 已修改成源码查询
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual IQueryable<IExecution> CreateExecutionQuery(Expression<Func<ExecutionEntity, bool>> expression)
        {
            var db = CommandExecutor.Execute<DbContext>(new GetDbContextCmd());
            var query = from a in db.Set<ExecutionEntity>()
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        select a;
            if (expression == null)
            {
                return query;
            }
            else
            {
                return query.Where(expression);
            }
            //return CommandExecutor.Execute(new CreateQueryCmd<ExecutionEntity>(expression));
        }

        public virtual IQueryable<IIncident> CreateIncidentQuery(Expression<Func<IncidentEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<IncidentEntity>(expression));
        }


        public virtual IQueryable<IEventSubscription> CreateEventSubscriptionQuery(Expression<Func<EventSubscriptionEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<EventSubscriptionEntity>(expression));
        }

        public virtual IQueryable<IVariableInstance> CreateVariableInstanceQuery(Expression<Func<VariableInstanceEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<VariableInstanceEntity>(expression));
        }

        public virtual IVariableMap GetVariablesTyped(string executionId)
        {
            return GetVariablesTyped(executionId, true);
        }

        public virtual IVariableMap GetVariablesTyped(string executionId, bool deserializeObjectValues)
        {
            return
                CommandExecutor.Execute(new GetExecutionVariablesCmd(executionId, null, false, deserializeObjectValues));
        }

        public virtual IVariableMap GetVariablesLocalTyped(string executionId)
        {
            return GetVariablesLocalTyped(executionId, true);
        }

        public virtual IVariableMap GetVariablesLocalTyped(string executionId, bool deserializeObjectValues)
        {
            return
                CommandExecutor.Execute(new GetExecutionVariablesCmd(executionId, null, true, deserializeObjectValues));
        }

        public virtual IVariableMap GetVariablesTyped(string executionId, ICollection<string> variableNames,
            bool deserializeObjectValues)
        {
            return
                CommandExecutor.Execute(new GetExecutionVariablesCmd(executionId, variableNames, false,
                    deserializeObjectValues));
        }

        public virtual IVariableMap GetVariablesLocalTyped(string executionId, ICollection<string> variableNames,
            bool deserializeObjectValues)
        {
            return
                CommandExecutor.Execute(new GetExecutionVariablesCmd(executionId, variableNames, true,
                    deserializeObjectValues));
        }

        public virtual object GetVariable(string executionId, string variableName)
        {
            return CommandExecutor.Execute(new GetExecutionVariableCmd(executionId, variableName, false));
        }

        public virtual T GetVariableTyped<T>(string executionId, string variableName)
        {
            return GetVariableTyped<T>(executionId, variableName, true);
        }

        public virtual T GetVariableTyped<T>(string executionId, string variableName, bool deserializeObjectValue)
        {
            return
                CommandExecutor.Execute(new GetExecutionVariableTypedCmd<T>(executionId, variableName, false,
                    deserializeObjectValue));
        }

        public virtual T GetVariableLocalTyped<T>(string executionId, string variableName)
        {
            return GetVariableLocalTyped<T>(executionId, variableName, true);
        }

        public virtual T GetVariableLocalTyped<T>(string executionId, string variableName, bool deserializeObjectValue)
        {
            return
                CommandExecutor.Execute(new GetExecutionVariableTypedCmd<T>(executionId, variableName, true,
                    deserializeObjectValue));
        }

        public virtual object GetVariableLocal(string executionId, string variableName)
        {
            return CommandExecutor.Execute(new GetExecutionVariableCmd(executionId, variableName, true));
        }

        public virtual void SetVariable(string executionId, string variableName, object value)
        {
            EnsureUtil.EnsureNotNull("variableName", variableName);
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables[variableName] = value;
            CommandExecutor.Execute(new SetExecutionVariablesCmd(executionId, variables, false));
        }

        public virtual void SetVariableLocal(string executionId, string variableName, object value)
        {
            EnsureUtil.EnsureNotNull("variableName", variableName);
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables[variableName] = value;
            CommandExecutor.Execute(new SetExecutionVariablesCmd(executionId, variables, true));
        }

        public virtual void RemoveVariable(string executionId, string variableName)
        {
            ICollection<string> variableNames = new List<string>();
            variableNames.Add(variableName);
            CommandExecutor.Execute(new RemoveExecutionVariablesCmd(executionId, variableNames, false));
        }

        public virtual void RemoveVariableLocal(string executionId, string variableName)
        {
            ICollection<string> variableNames = new List<string>();
            variableNames.Add(variableName);
            CommandExecutor.Execute(new RemoveExecutionVariablesCmd(executionId, variableNames, true));
        }

        public virtual void RemoveVariables(string executionId, ICollection<string> variableNames)
        {
            CommandExecutor.Execute(new RemoveExecutionVariablesCmd(executionId, variableNames, false));
        }

        public virtual void RemoveVariablesLocal(string executionId, ICollection<string> variableNames)
        {
            CommandExecutor.Execute(new RemoveExecutionVariablesCmd(executionId, variableNames, true));
        }

        public virtual void Signal(string executionId)
        {
            CommandExecutor.Execute(new SignalCmd(executionId, "compensationDone", null, null));
        }

        public virtual void Signal(string executionId, string signalName, object signalData,
            IDictionary<string, object> processVariables)
        {
            CommandExecutor.Execute(new SignalCmd(executionId, signalName, signalData, processVariables));
        }

        public virtual void Signal(string executionId, IDictionary<string, object> processVariables)
        {
            CommandExecutor.Execute(new SignalCmd(executionId, null, null, processVariables));
        }

        public virtual IQueryable<IProcessInstance> CreateProcessInstanceQuery(Expression<Func<ExecutionEntity, bool>> expression)
        {
            //15:33:07.225 [main] DEBUG o.c.b.e.i.p.e.E.selectProcessInstanceByQueryCriteria - ==>  Preparing: select distinct RES.* from ACT_RU_EXECUTION RES inner join ACT_RE_PROCDEF P on RES.PROC_DEF_ID_ = P.ID_ WHERE RES.PARENT_ID_ is null order by RES.ID_ asc LIMIT ? OFFSET ? 
            var db = GetDbContext();
            var query = from a in db.Set<ExecutionEntity>()
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        where a.ParentId == null
                        select a;
            if (expression == null)
                return query;
            else
                return query.Where(expression);
            //return CommandExecutor.Execute(new CreateQueryCmd<ExecutionEntity>(expression));
        }
        public virtual IQueryable<IProcessInstance> CreateProcessInstanceQuerySubProcessInstanceId(string subProcessInstanceId, Expression<Func<ExecutionEntity, bool>> expression = null)
        {
            //16:20:14.141 [main] DEBUG o.c.b.e.i.p.e.E.selectProcessInstanceByQueryCriteria - ==>  Preparing: select distinct RES.* from ACT_RU_EXECUTION RES inner join ACT_RE_PROCDEF P on RES.PROC_DEF_ID_ = P.ID_ WHERE RES.PARENT_ID_ is null and RES.SUPER_EXEC_ IN (select ID_ from ACT_RU_EXECUTION where PROC_INST_ID_ = ?) order by RES.ID_ asc LIMIT ? OFFSET ? 
            var db = GetDbContext();
            var ids = from a in db.Set<ExecutionEntity>()
                      //where a.ProcessInstanceId == subProcessInstanceId
                      select a.Id;
            var query =
                from a in db.Set<ExecutionEntity>().Where(m=>ids.Contains(m.SuperExecutionId))
                join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                where a.ParentId == null //&& ids.Contains(a.SuperExecutionId)
                select a;
            if (expression == null)
                return query;
            else
                return query.Where(expression);
        }
        public virtual IQueryable<IProcessInstance> CreateProcessInstanceQueryProcessDefinitionKey(string processDefinitionKey, Expression<Func<ExecutionEntity, bool>> expression = null)
        {
            //15:10:10.620 [main] DEBUG o.c.b.e.i.p.e.E.selectProcessInstanceByQueryCriteria - ==>  Preparing: select distinct RES.* from ACT_RU_EXECUTION RES inner join ACT_RE_PROCDEF P on RES.PROC_DEF_ID_ = P.ID_ WHERE RES.PARENT_ID_ is null and P.KEY_ = ? order by RES.ID_ asc LIMIT ? OFFSET ? 
            var db = GetDbContext();
            var query = from a in db.Set<ExecutionEntity>()
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        where a.ParentId == null && b.Key == processDefinitionKey
                        select a;
            if (expression == null)
                return query;
            else
                return query.Where(expression);
            //return CommandExecutor.Execute(new CreateQueryCmd<ExecutionEntity>(expression));
        }
        public virtual IQueryable<IProcessInstance> CreateProcessInstanceQuerySuperProcessInstanceId(string superProcessInstanceId, Expression<Func<ExecutionEntity, bool>> expression = null)
        {
            //15:59:30.758 [main] DEBUG o.c.b.e.i.p.e.E.selectProcessInstanceByQueryCriteria - ==>  Preparing: select distinct RES.* from ACT_RU_EXECUTION RES inner join ACT_RE_PROCDEF P on RES.PROC_DEF_ID_ = P.ID_ WHERE RES.PARENT_ID_ is null and RES.SUPER_EXEC_ IN (select ID_ from ACT_RU_EXECUTION where PROC_INST_ID_ = ?) order by RES.ID_ asc LIMIT ? OFFSET ? 
            //15:59:30.764[main] DEBUG o.c.b.e.i.p.e.E.selectProcessInstanceByQueryCriteria - ==> Parameters: 6(String), 2147483647(Integer), 0(Integer)
            var db = GetDbContext();
            var query =
                from a in db.Set<ExecutionEntity>()
                join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                where a.ParentId == null && (from c in db.Set<ExecutionEntity>()
                                             where c.ProcessInstanceId == superProcessInstanceId
                                             select c.Id).Contains(a.SuperExecutionId)
                select a;
            if (expression == null)
                return query;
            else
                return query.Where(expression);
            //return CommandExecutor.Execute(new CreateQueryCmd<ExecutionEntity>(expression));
        }

        public virtual IList<string> GetActiveActivityIds(string executionId)
        {
            return CommandExecutor.Execute(new FindActiveActivityIdsCmd(executionId));
        }

        public virtual IActivityInstance GetActivityInstance(string processInstanceId)
        {
            return CommandExecutor.Execute(new GetActivityInstanceCmd(processInstanceId));
        }

        public virtual void SuspendProcessInstanceById(string processInstanceId)
        {
            UpdateProcessInstanceSuspensionState().ByProcessInstanceId(processInstanceId).Suspend();
        }

        public virtual void SuspendProcessInstanceByProcessDefinitionId(string processDefinitionId)
        {
            UpdateProcessInstanceSuspensionState().ByProcessDefinitionId(processDefinitionId).Suspend();
        }

        public virtual void SuspendProcessInstanceByProcessDefinitionKey(string processDefinitionKey)
        {
            UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(processDefinitionKey).Suspend();
        }

        public virtual void ActivateProcessInstanceById(string processInstanceId)
        {
            UpdateProcessInstanceSuspensionState().ByProcessInstanceId(processInstanceId).Activate();
        }

        public virtual void ActivateProcessInstanceByProcessDefinitionId(string processDefinitionId)
        {
            UpdateProcessInstanceSuspensionState().ByProcessDefinitionId(processDefinitionId).Activate();
        }

        public virtual void ActivateProcessInstanceByProcessDefinitionKey(string processDefinitionKey)
        {
            UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(processDefinitionKey).Activate();
        }

        public virtual IUpdateProcessInstanceSuspensionStateSelectBuilder UpdateProcessInstanceSuspensionState()
        {
            return new UpdateProcessInstanceSuspensionStateBuilderImpl(CommandExecutor);
        }

        public virtual IProcessInstance StartProcessInstanceByMessage(string messageName)
        {
            return CreateMessageCorrelation(messageName).CorrelateStartMessage();
        }

        public virtual IProcessInstance StartProcessInstanceByMessage(string messageName, string businessKey)
        {
            return CreateMessageCorrelation(messageName).ProcessInstanceBusinessKey(businessKey).CorrelateStartMessage();
        }

        public virtual IProcessInstance StartProcessInstanceByMessage(string messageName,
            IDictionary<string, object> processVariables)
        {
            return CreateMessageCorrelation(messageName).SetVariables(processVariables).CorrelateStartMessage();
        }

        public virtual IProcessInstance StartProcessInstanceByMessage(string messageName, string businessKey,
            IDictionary<string, object> processVariables)
        {
            return
                CreateMessageCorrelation(messageName)
                    .ProcessInstanceBusinessKey(businessKey)
                    .SetVariables(processVariables)
                    .CorrelateStartMessage();
        }

        public virtual IProcessInstance StartProcessInstanceByMessageAndProcessDefinitionId(string messageName,
            string processDefinitionId)
        {
            return
                CreateMessageCorrelation(messageName).ProcessDefinitionId(processDefinitionId).CorrelateStartMessage();
        }

        public virtual IProcessInstance StartProcessInstanceByMessageAndProcessDefinitionId(string messageName,
            string processDefinitionId, string businessKey)
        {
            return
                CreateMessageCorrelation(messageName)
                    .ProcessDefinitionId(processDefinitionId)
                    .ProcessInstanceBusinessKey(businessKey)
                    .CorrelateStartMessage();
        }

        public virtual IProcessInstance StartProcessInstanceByMessageAndProcessDefinitionId(string messageName,
            string processDefinitionId, IDictionary<string, object> processVariables)
        {
            return
                CreateMessageCorrelation(messageName)
                    .ProcessDefinitionId(processDefinitionId)
                    .SetVariables(processVariables)
                    .CorrelateStartMessage();
        }

        public virtual IProcessInstance StartProcessInstanceByMessageAndProcessDefinitionId(string messageName,
            string processDefinitionId, string businessKey, IDictionary<string, object> processVariables)
        {
            return
                CreateMessageCorrelation(messageName)
                    .ProcessDefinitionId(processDefinitionId)
                    .ProcessInstanceBusinessKey(businessKey)
                    .SetVariables(processVariables)
                    .CorrelateStartMessage();
        }

        public virtual void SignalEventReceived(string signalName)
        {
            CreateSignalEvent(signalName).Send();
        }

        public virtual void SignalEventReceived(string signalName, IDictionary<string, object> processVariables)
        {
            CreateSignalEvent(signalName).SetVariables(processVariables).Send();
        }

        public virtual void SignalEventReceived(string signalName, string executionId)
        {
            CreateSignalEvent(signalName).SetExecutionId(executionId).Send();
        }

        public virtual void SignalEventReceived(string signalName, string executionId,
            IDictionary<string, object> processVariables)
        {
            CreateSignalEvent(signalName).SetExecutionId(executionId).SetVariables(processVariables).Send();
        }

        public virtual ISignalEventReceivedBuilder CreateSignalEvent(string signalName)
        {
            return new SignalEventReceivedBuilderImpl(CommandExecutor, signalName);
        }

        public virtual void MessageEventReceived(string messageName, string executionId)
        {
            EnsureUtil.EnsureNotNull("messageName", messageName);
            CommandExecutor.Execute(new MessageEventReceivedCmd(messageName, executionId, null));
        }

        public virtual void MessageEventReceived(string messageName, string executionId,
            IDictionary<string, object> processVariables)
        {
            EnsureUtil.EnsureNotNull("messageName", messageName);
            CommandExecutor.Execute(new MessageEventReceivedCmd(messageName, executionId, processVariables));
        }

        public virtual IMessageCorrelationBuilder CreateMessageCorrelation(string messageName)
        {
            return new MessageCorrelationBuilderImpl(CommandExecutor, messageName);
        }

        public virtual void CorrelateMessage(string messageName, IDictionary<string, object> correlationKeys,
            IDictionary<string, object> processVariables)
        {
            CreateMessageCorrelation(messageName)
                .ProcessInstanceVariablesEqual(correlationKeys)
                .SetVariables(processVariables)
                .Correlate();
        }

        public virtual void CorrelateMessage(string messageName, string businessKey,
            IDictionary<string, object> correlationKeys, IDictionary<string, object> processVariables)
        {
            CreateMessageCorrelation(messageName)
                .ProcessInstanceVariablesEqual(correlationKeys)
                .ProcessInstanceBusinessKey(businessKey)
                .SetVariables(processVariables)
                .Correlate();
        }

        public virtual void CorrelateMessage(string messageName)
        {
            CreateMessageCorrelation(messageName).Correlate();
        }

        public virtual void CorrelateMessage(string messageName, string businessKey)
        {
            CreateMessageCorrelation(messageName).ProcessInstanceBusinessKey(businessKey).Correlate();
        }

        public virtual void CorrelateMessage(string messageName, IDictionary<string, object> correlationKeys)
        {
            CreateMessageCorrelation(messageName).ProcessInstanceVariablesEqual(correlationKeys).Correlate();
        }

        public virtual void CorrelateMessage(string messageName, string businessKey,
            IDictionary<string, object> processVariables)
        {
            CreateMessageCorrelation(messageName)
                .ProcessInstanceBusinessKey(businessKey)
                .SetVariables(processVariables)
                .Correlate();
        }

        public virtual IProcessInstanceModificationBuilder CreateProcessInstanceModification(string processInstanceId)
        {
            return new ProcessInstanceModificationBuilderImpl(CommandExecutor, processInstanceId);
        }

        public virtual IProcessInstantiationBuilder CreateProcessInstanceById(string processDefinitionId)
        {
            return ProcessInstantiationBuilderImpl.CreateProcessInstanceById(CommandExecutor, processDefinitionId);
        }

        public virtual IProcessInstantiationBuilder CreateProcessInstanceByKey(string processDefinitionKey)
        {
            return ProcessInstantiationBuilderImpl.CreateProcessInstanceByKey(CommandExecutor, processDefinitionKey);
        }

        public virtual IMigrationPlanBuilder CreateMigrationPlan(string sourceProcessDefinitionId,
            string targetProcessDefinitionId)
        {
            return new MigrationPlanBuilderImpl(CommandExecutor, sourceProcessDefinitionId, targetProcessDefinitionId);
        }

        public virtual IMigrationPlanExecutionBuilder NewMigration(IMigrationPlan migrationPlan)
        {
            return new MigrationPlanExecutionBuilderImpl(CommandExecutor, migrationPlan);
        }

        IDictionary<string, object> IRuntimeService.GetVariables(string executionId)
        {
            return GetVariablesTyped(executionId);
        }

        IDictionary<string, object> IRuntimeService.GetVariablesLocal(string executionId)
        {
            return GetVariablesLocalTyped(executionId);
        }

        IDictionary<string, object> IRuntimeService.GetVariables(string executionId, ICollection<string> variableNames)
        {
            return GetVariablesTyped(executionId, variableNames, true);
        }

        IDictionary<string, object> IRuntimeService.GetVariablesLocal(string executionId,
            ICollection<string> variableNames)
        {
            throw new NotImplementedException();
        }

        public void SetVariables<T1>(string executionId, IDictionary<string, T1> variables)
        {
            throw new NotImplementedException();
        }

        public void SetVariablesLocal<T1>(string executionId, IDictionary<string, T1> variables)
        {
            throw new NotImplementedException();
        }

        public virtual IVariableMap GetVariables(string executionId)
        {
            return GetVariablesTyped(executionId);
        }

        public virtual IVariableMap GetVariablesLocal(string executionId)
        {
            return GetVariablesLocalTyped(executionId);
        }

        public virtual IVariableMap GetVariables(string executionId, ICollection<string> variableNames)
        {
            return GetVariablesTyped(executionId, variableNames, true);
        }

        public virtual IVariableMap GetVariablesLocal(string executionId, ICollection<string> variableNames)
        {
            return GetVariablesLocalTyped(executionId, variableNames, true);
        }

        public virtual void SetVariables(string executionId, IDictionary<string, object> variables)
        {
            CommandExecutor.Execute(new SetExecutionVariablesCmd(executionId, variables, false));
        }

        public virtual void SetVariablesLocal(string executionId, IDictionary<string, object> variables)
        {
            CommandExecutor.Execute(new SetExecutionVariablesCmd(executionId, variables, true));
        }

        public virtual void UpdateVariables(string executionId, IDictionary<string, object> modifications,
            ICollection<string> deletions)
        {
            CommandExecutor.Execute(new PatchExecutionVariablesCmd(executionId, modifications, deletions, false));
        }

        public virtual void UpdateVariablesLocal(string executionId, IDictionary<string, object> modifications,
            ICollection<string> deletions)
        {
            CommandExecutor.Execute(new PatchExecutionVariablesCmd(executionId, modifications, deletions, true));
        }

        public virtual IFormData GetFormInstanceById(string processDefinitionId)
        {
            return CommandExecutor.Execute(new GetStartFormCmd(processDefinitionId));
        }

        public IModificationBuilder CreateModification(string processDefinitionId)
        {
            return new ModificationBuilderImpl(CommandExecutor, processDefinitionId);
        }

        public IRestartProcessInstanceBuilder RestartProcessInstances(string processDefinitionId)
        {
            return new RestartProcessInstanceBuilderImpl(CommandExecutor, processDefinitionId);
        }
        public DbContext GetDbContext()
        {
            return CommandExecutor.Execute(new GetDbContextCmd());
        }
        public TManager GetManager<TManager>() where TManager : class
        {
            return CommandExecutor.Execute(new GetManagerCmd<TManager>());
        }
        public virtual IIncident CreateIncident(string incidentType, string executionId, string configuration)
        {
            return CreateIncident(incidentType, executionId, configuration, null);
        }

        public virtual IIncident CreateIncident(string incidentType, string executionId, string configuration, string message)
        {
            return CommandExecutor.Execute(new CreateIncidentCmd(incidentType, executionId, configuration, message));
        }

        public virtual void ResolveIncident(string incidentId)
        {
            CommandExecutor.Execute(new ResolveIncidentCmd(incidentId));
        }

    }
}