using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessEventJobHandler : IJobHandler<ProcessEventJobHandler.EventSubscriptionJobConfiguration>
    {
        public const string TYPE = "event";

        public virtual string Type
        {
            get { return TYPE; }
        }

        public virtual IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            return new EventSubscriptionJobConfiguration(canonicalString);
        }

        public virtual void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            // do nothing
        }

        public virtual void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            // lookup subscription:
            //var eventSubscriptionId = configuration.EventSubscriptionId;
            //EventSubscriptionEntity eventSubscription =
            //    commandContext.EventSubscriptionManager.findEventSubscriptionById(eventSubscriptionId);

            //// if event subscription is null, ignore
            //if (eventSubscription != null)
            //{
            //    eventSubscription.eventReceived(null, false);
            //}
        }

        public class EventSubscriptionJobConfiguration : IJobHandlerConfiguration
        {
            protected internal string eventSubscriptionId;

            public EventSubscriptionJobConfiguration(string eventSubscriptionId)
            {
                this.eventSubscriptionId = eventSubscriptionId;
            }

            public virtual string EventSubscriptionId
            {
                get { return eventSubscriptionId; }
            }

            public virtual string ToCanonicalString()
            {
                return eventSubscriptionId;
            }
        }
    }
}