using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class CorrelationSubscriptionImpl : BaseElementImpl, ICorrelationSubscription
    {

        protected internal static IAttributeReference CorrelationKeyAttribute;//IAttributeReference<ICorrelationKey>
        protected internal static IChildElementCollection/*<ICorrelationPropertyBinding>*/ CorrelationPropertyBindingCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICorrelationSubscription>(/*typeof(ICorrelationSubscription),*/ BpmnModelConstants.BpmnElementCorrelationSubscription)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IBaseElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CorrelationKeyAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeCorrelationKeyRef)
                .Required()
                .QNameAttributeReference<ICorrelationKey>(/*typeof(ICorrelationKey)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            CorrelationPropertyBindingCollection = sequenceBuilder.ElementCollection<ICorrelationPropertyBinding>(/*typeof(ICorrelationPropertyBinding)*/).Build/*<ICorrelationPropertyBinding>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICorrelationSubscription>
        {
            public virtual ICorrelationSubscription NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CorrelationSubscriptionImpl(instanceContext);
            }
        }

        public CorrelationSubscriptionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICorrelationKey CorrelationKey
        {
            get => CorrelationKeyAttribute.GetReferenceTargetElement<ICorrelationKey>(this);
            set => CorrelationKeyAttribute.SetReferenceTargetElement(this, value);
        }

        public virtual ICollection<ICorrelationPropertyBinding> CorrelationPropertyBindings => CorrelationPropertyBindingCollection.Get<ICorrelationPropertyBinding>(this);
    }

}