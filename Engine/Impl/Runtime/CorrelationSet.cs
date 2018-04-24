using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.runtime
{
    public class CorrelationSet
    {
        protected internal readonly string businessKey;
        protected internal readonly IDictionary<string, object> correlationKeys;
        protected internal readonly bool IsTenantIdSet;
        protected internal readonly IDictionary<string, object> localCorrelationKeys;
        protected internal readonly string processDefinitionId;
        protected internal readonly string processInstanceId;
        protected internal readonly string tenantId;

        public CorrelationSet(MessageCorrelationBuilderImpl builder)
        {
            businessKey = builder.BusinessKey;
            processInstanceId = builder.processInstanceId;
            correlationKeys = builder.CorrelationProcessInstanceVariables;
            localCorrelationKeys = builder.CorrelationLocalVariables;
            processDefinitionId = builder.processDefinitionId;
            tenantId = builder.TenantId;
            IsTenantIdSet = builder.TenantIdSet;
        }

        public virtual string BusinessKey
        {
            get { return businessKey; }
        }

        public virtual IDictionary<string, object> CorrelationKeys
        {
            get { return correlationKeys; }
        }

        public virtual IDictionary<string, object> LocalCorrelationKeys
        {
            get { return localCorrelationKeys; }
        }

        public virtual string ProcessInstanceId
        {
            get { return processInstanceId; }
        }

        public virtual string ProcessDefinitionId
        {
            get { return processDefinitionId; }
        }

        public virtual string TenantId
        {
            get { return tenantId; }
        }

        public virtual bool TenantIdSet
        {
            get { return IsTenantIdSet; }
        }

        public override string ToString()
        {
            return "CorrelationSet [businessKey=" + businessKey + ", processInstanceId=" + processInstanceId +
                   ", processDefinitionId=" + processDefinitionId + ", correlationKeys=" + correlationKeys +
                   ", localCorrelationKeys=" + localCorrelationKeys + ", tenantId=" + tenantId + ", isTenantIdSet=" +
                   IsTenantIdSet + "]";
        }
    }
}