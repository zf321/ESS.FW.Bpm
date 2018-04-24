using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using ESS.FW.Bpm.Engine.Common;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///     
    ///     
    /// </summary>
    [Serializable]
    public class MessageEventReceivedCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal readonly string ExecutionId;
        protected internal readonly string MessageName;
        protected internal readonly IDictionary<string, object> ProcessVariables;
        protected internal bool Exclusive;

        public MessageEventReceivedCmd(string messageName, string executionId,
            IDictionary<string, object> processVariables)
        {
            this.ExecutionId = executionId;
            this.MessageName = messageName;
            this.ProcessVariables = processVariables;
        }

        public MessageEventReceivedCmd(string messageName, string executionId,
            IDictionary<string, object> processVariables, bool exclusive)
            : this(messageName, executionId, processVariables)
        {
            this.Exclusive = exclusive;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("executionId", ExecutionId);
            
            var eventSubscriptionManager = commandContext.EventSubscriptionManager;

            IList<EventSubscriptionEntity> eventSubscriptions = null;
            if (!string.ReferenceEquals(MessageName, null))
            {
                eventSubscriptions = eventSubscriptionManager.FindEventSubscriptionsByNameAndExecution(EventType.Message.Name, MessageName, ExecutionId, Exclusive);
            }
            else
            {
                eventSubscriptions = eventSubscriptionManager.FindEventSubscriptionsByExecutionAndType(ExecutionId, EventType.Message.Name, Exclusive);
            }

            EnsureUtil.EnsureNotEmpty("Execution with id '" + ExecutionId + "' does not have a subscription to a message event with name '" + MessageName + "'", "eventSubscriptions",ListExt.ConvertToIlist( eventSubscriptions));
            EnsureUtil.EnsureNumberOfElements("More than one matching message subscription found for execution " + ExecutionId, "eventSubscriptions", ListExt.ConvertToIlist(eventSubscriptions), 1);

            // there can be only one:
            var eventSubscriptionEntity = eventSubscriptions[0];

            // check authorization
            var processInstanceId = eventSubscriptionEntity.ProcessInstanceId;
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckUpdateProcessInstanceById(processInstanceId);

            eventSubscriptionEntity.EventReceived(ProcessVariables, false);
            return null;
        }
    }
}