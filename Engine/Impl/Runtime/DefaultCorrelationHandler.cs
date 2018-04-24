using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System.Linq;

namespace ESS.FW.Bpm.Engine.Impl.runtime
{
    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public class DefaultCorrelationHandler : ICorrelationHandler
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        public virtual CorrelationHandlerResult CorrelateMessage(CommandContext commandContext, string messageName,
            CorrelationSet correlationSet)
        {
            // first try to correlate to execution
            var correlations = CorrelateMessageToExecutions(commandContext, messageName, correlationSet);

            if (correlations.Count > 1)
                throw Log.ExceptionCorrelateMessageToSingleExecution(messageName, correlations.Count, correlationSet);
            if (correlations.Count == 1)
                return correlations[0];

            // then try to correlate to process definition
            correlations = CorrelateStartMessages(commandContext, messageName, correlationSet);

            if (correlations.Count > 1)
                throw Log.ExceptionCorrelateMessageToSingleProcessDefinition(messageName, correlations.Count,
                    correlationSet);
            if (correlations.Count == 1)
                return correlations[0];
            return null;
        }

        public virtual IList<CorrelationHandlerResult> CorrelateMessages(CommandContext commandContext,
            string messageName, CorrelationSet correlationSet)
        {
            List<CorrelationHandlerResult> results = new List<CorrelationHandlerResult>();

            // first collect correlations to executions
            results.AddRange(CorrelateMessageToExecutions(commandContext, messageName,
                correlationSet));
            // now collect correlations to process definition
            results.AddRange(CorrelateStartMessages(commandContext, messageName,
                correlationSet));

            return results;
        }

        public virtual IList<CorrelationHandlerResult> CorrelateStartMessages(CommandContext commandContext,
            string messageName, CorrelationSet correlationSet)
        {
            if (messageName== null)
                return null;

            if (correlationSet.ProcessDefinitionId==null)
                return CorrelateStartMessageByEventSubscription(commandContext, messageName, correlationSet);
            var correlationResult = CorrelateStartMessageByProcessDefinitionId(commandContext, messageName,
                correlationSet.ProcessDefinitionId);
            if (correlationResult != null)
                return new List<CorrelationHandlerResult> {correlationResult};
            return null;
        }
        //Correlate联系; 使互相关联; 相关物; 相关联的人; 相关的; 相应特点的
        protected internal virtual IList<CorrelationHandlerResult> CorrelateMessageToExecutions(
            CommandContext commandContext, string messageName, CorrelationSet correlationSet)
        {
            //TODO 复杂查询可能有误
            //var query = new ExecutionQueryImpl();
            var exp = commandContext.ExecutionManager.Find((t)=>true);
            var correlationKeys = correlationSet.CorrelationKeys;
            if (correlationKeys != null)
                foreach (var correlationKey in correlationKeys)
                {
                    //query.ProcessVariableValueEquals(correlationKey.Key, correlationKey.Value);
                    ProcessVariableValueEquals(correlationKey.Key, correlationKey.Value);
                }


            var localCorrelationKeys = correlationSet.LocalCorrelationKeys;
            if (localCorrelationKeys != null)
                foreach (var correlationKey in localCorrelationKeys)
                {
                    //query.VariableValueEquals(correlationKey.Key, correlationKey.Value);
                    throw new NotImplementedException();
                }


            var businessKey = correlationSet.BusinessKey;
            if (!ReferenceEquals(businessKey, null))
            {
                //query.ProcessInstanceBusinessKey(businessKey);
                exp.Where(m => m.BusinessKey == businessKey);
            }


            var processInstanceId = correlationSet.ProcessInstanceId;
            if (!ReferenceEquals(processInstanceId, null))
            {
                //query.processInstanceId(processInstanceId);
                exp.Where(m => m.ProcessInstanceId == processInstanceId);
            }


            if (!ReferenceEquals(messageName, null))
            {
                //query.MessageEventSubscriptionName(messageName);
                //exp= exp.Where(m => m.EventName == messageName);
                exp = from a in exp
                      join b in commandContext.GetDbEntityManager<EventSubscriptionEntity>().Find(m => m.EventType == "message" && m.EventName == messageName) on a.Id equals b.ExecutionId
                      select a;
            }
            else
            {
                //query.MessageEventSubscription();
                throw new NotImplementedException();
            }

            if (correlationSet.IsTenantIdSet)
            {
                var tenantId = correlationSet.TenantId;
                if (!ReferenceEquals(tenantId, null))
                {
                    //query.TenantIdIn(tenantId);
                    exp = exp.Where(m => m.TenantId == tenantId);
                }

                else
                {
                    //query.WithoutTenantId();
                    exp = exp.Where(m => m.TenantId == null);
                }

            }

            //// restrict to active executions
            //query.Active();
            exp.Where(m => m.SuspensionState == SuspensionStateFields.Active.StateCode);
            //var matchingExecutions = query.EvaluateExpressionsAndExecuteList(commandContext, null);
            var matchingExecutions = exp.ToList();
            IList<CorrelationHandlerResult> result = new List<CorrelationHandlerResult>(/*matchingExecutions.Count*/);

            foreach (var matchingExecution in matchingExecutions)
            {
                var correlationResult = CorrelationHandlerResult.MatchedExecution(matchingExecution);
                result.Add(correlationResult);
            }
            return result;
        }

        private void ProcessVariableValueEquals(string key, object value)
        {
            throw new NotImplementedException();
        }

        protected internal virtual IList<CorrelationHandlerResult> CorrelateStartMessageByEventSubscription(
            CommandContext commandContext, string messageName, CorrelationSet correlationSet)
        {
            IList<CorrelationHandlerResult> results = new List<CorrelationHandlerResult>();
            DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;

            var messageEventSubscriptions = FindMessageStartEventSubscriptions(commandContext, messageName,
                correlationSet);
            foreach (var messageEventSubscription in messageEventSubscriptions)
                if (messageEventSubscription.Configuration!= null)
                {
                    var processDefinitionId = messageEventSubscription.Configuration;
                    ProcessDefinitionEntity processDefinition = deploymentCache.FindDeployedProcessDefinitionById(processDefinitionId);
                    // only an active process definition will be returned
                    if (processDefinition != null && !processDefinition.Suspended)
                    {
                        CorrelationHandlerResult result = CorrelationHandlerResult.MatchedProcessDefinition(processDefinition, messageEventSubscription.ActivityId);
                        results.Add(result);

                    }
                    else
                    {
                        Log.CouldNotFindProcessDefinitionForEventSubscription(messageEventSubscription, processDefinitionId);
                    }
                }
            return results;
        }

        protected internal virtual IList<EventSubscriptionEntity> FindMessageStartEventSubscriptions(
            CommandContext commandContext, string messageName, CorrelationSet correlationSet)
        {
            IEventSubscriptionManager eventSubscriptionManager = commandContext.EventSubscriptionManager;

            if (correlationSet.IsTenantIdSet)
            {
                EventSubscriptionEntity eventSubscription = eventSubscriptionManager.FindMessageStartEventSubscriptionByNameAndTenantId(messageName, correlationSet.TenantId);
                if (eventSubscription != null)
                {
                    return new List<EventSubscriptionEntity> { eventSubscription };
                }
                else
                {
                    return new List<EventSubscriptionEntity>();
                }
            }
            return eventSubscriptionManager.FindMessageStartEventSubscriptionByName(messageName); ;
        }

        protected internal virtual CorrelationHandlerResult CorrelateStartMessageByProcessDefinitionId(
            CommandContext commandContext, string messageName, string processDefinitionId)
        {
            DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;
            ProcessDefinitionEntity processDefinition = deploymentCache.FindDeployedProcessDefinitionById(processDefinitionId);
            //only an active process definition will be returned
            if (processDefinition != null && !processDefinition.Suspended)
            {
                string startActivityId = FindStartActivityIdByMessage(processDefinition, messageName);
                if (!string.ReferenceEquals(startActivityId, null))
                {
                    return CorrelationHandlerResult.MatchedProcessDefinition(processDefinition, startActivityId);
                }
            }
            return null;
        }

        protected internal virtual string FindStartActivityIdByMessage(ProcessDefinitionEntity processDefinition,
            string messageName)
        {
            foreach (EventSubscriptionDeclaration declaration in EventSubscriptionDeclaration.GetDeclarationsForScope(processDefinition).Values)
            {
                if (IsMessageStartEventWithName(declaration, messageName))
                {
                    return declaration.ActivityId;
                }
            }
            return null;
        }

        protected internal virtual bool IsMessageStartEventWithName(EventSubscriptionDeclaration declaration,
            string messageName)
        {
            return EventType.Message.Name.Equals(declaration.EventType) && declaration.StartEvent &&
                   messageName.Equals(declaration.UnresolvedEventName);
        }
    }
}