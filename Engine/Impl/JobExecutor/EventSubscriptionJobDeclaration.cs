using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    //using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
    //using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
    //using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
    //using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;

    /// <summary>
    ///     <para>
    ///         Describes and creates jobs for handling an event asynchronously.
    ///         These jobs are created in the context of an <seealso cref="EventSubscriptionEntity" /> and are of type
    ///         <seealso cref="MessageEntity" />.
    ///     </para>
    ///     
    /// </summary>
    [Serializable]
    public class EventSubscriptionJobDeclaration : JobDeclaration<MessageEntity>
    {
        private const long SerialVersionUid = 1L;

        protected internal EventSubscriptionDeclaration EventSubscriptionDeclaration;

        public EventSubscriptionJobDeclaration(EventSubscriptionDeclaration eventSubscriptionDeclaration)
            : base(ProcessEventJobHandler.TYPE)
        {
            EnsureUtil.EnsureNotNull("eventSubscriptionDeclaration", eventSubscriptionDeclaration);
            this.EventSubscriptionDeclaration = eventSubscriptionDeclaration;
        }

        public virtual string EventType
        {
            get { return EventSubscriptionDeclaration.EventType; }
        }

        public virtual string EventName
        {
            get { return EventSubscriptionDeclaration.UnresolvedEventName; }
        }

        public override string ActivityId
        {
            get { return EventSubscriptionDeclaration.ActivityId; }
        }

        protected internal override MessageEntity NewJobInstance(object eventSubscription)
        {
            return NewJobInstanceA(eventSubscription as EventSubscriptionEntity);
        }
        protected internal  MessageEntity NewJobInstanceA(EventSubscriptionEntity eventSubscription)
        {
            var message = new MessageEntity();

            // initialize job
            message.ActivityId = eventSubscription.ActivityId;
            message.ExecutionId = eventSubscription.ExecutionId;
            message.ProcessInstanceId = eventSubscription.ProcessInstanceId;

            ProcessDefinitionEntity processDefinition = eventSubscription.ProcessDefinition;

            if (processDefinition != null)
            {
                message.ProcessDefinitionId = processDefinition.Id;
                message.ProcessDefinitionKey = processDefinition.Key;
            }

            // TODO: support payload
            // if(payload != null) {
            //   message.setEventPayload(payload);
            // }

            return message;
        }

        protected internal override ExecutionEntity ResolveExecution(object context)
        {
            return (context as EventSubscriptionEntity).Execution;
        }

        protected internal override IJobHandlerConfiguration ResolveJobHandlerConfiguration(
            object context)
        {
            return new ProcessEventJobHandler.EventSubscriptionJobConfiguration((context as EventSubscriptionEntity).Id);
        }

//JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static java.Util.List<EventSubscriptionJobDeclaration> getDeclarationsForActivity(org.camunda.bpm.engine.impl.Pvm.PvmActivity activity)
        public static IList<EventSubscriptionJobDeclaration> GetDeclarationsForActivity(IPvmActivity activity)
        {
            var result = activity.GetProperty(BpmnParse.PropertynameEventSubscriptionJobDeclaration);
            if (result != null)
                return (IList<EventSubscriptionJobDeclaration>) result;
            return new List<EventSubscriptionJobDeclaration>();
        }

        /// <summary>
        ///     Assumes that an activity has at most one declaration of a certain eventType.
        /// </summary>
        public static EventSubscriptionJobDeclaration FindDeclarationForSubscription(
            EventSubscriptionEntity eventSubscription)
        {
            IList<EventSubscriptionJobDeclaration> declarations = GetDeclarationsForActivity(eventSubscription.Activity);

            foreach (EventSubscriptionJobDeclaration declaration in declarations)
            {
                if (declaration.EventType.Equals(eventSubscription.EventType))
                {
                    return declaration;
                }
            }

            return null;
        }
    }
}