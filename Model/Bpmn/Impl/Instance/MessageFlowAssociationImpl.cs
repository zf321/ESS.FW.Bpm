

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class MessageFlowAssociationImpl : BaseElementImpl, IMessageFlowAssociation
    {

        protected internal static IAttributeReference InnerMessageFlowRefAttribute;//IAttributeReference<IMessageFlow>
        protected internal static IAttributeReference OuterMessageFlowRefAttribute;//IAttributeReference<IMessageFlow>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IMessageFlowAssociation>(/*typeof(IMessageFlowAssociation),*/ BpmnModelConstants.BpmnElementMessageFlowAssociation)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            InnerMessageFlowRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeInnerMessageFlowRef)
                .Required()
                .QNameAttributeReference<IMessageFlow>(/*typeof(IMessageFlow)*/)
                .Build();

            OuterMessageFlowRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeOuterMessageFlowRef)
                .Required()
                .QNameAttributeReference<IMessageFlow>(/*typeof(IMessageFlow)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IMessageFlowAssociation>
        {
            public virtual IMessageFlowAssociation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new MessageFlowAssociationImpl(instanceContext);
            }
        }

        public MessageFlowAssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IMessageFlow InnerMessageFlow
        {
            get => InnerMessageFlowRefAttribute.GetReferenceTargetElement<IMessageFlow>(this);
            set => InnerMessageFlowRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IMessageFlow OuterMessageFlow
        {
            get => OuterMessageFlowRefAttribute.GetReferenceTargetElement<IMessageFlow>(this);
            set => OuterMessageFlowRefAttribute.SetReferenceTargetElement(this, value);
        }
    }
}