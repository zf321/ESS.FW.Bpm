using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    
    public class MessageFlowImpl : BaseElementImpl, IMessageFlow
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttributeReference SourceRefAttribute;//IAttributeReference<IInteractionNode>
        protected internal static IAttributeReference TargetRefAttribute;//IAttributeReference<IInteractionNode>
        protected internal static IAttributeReference MessageRefAttribute;//IAttributeReference<IMessage>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IMessageFlow>(/*typeof(IMessageFlow),*/ BpmnModelConstants.BpmnElementMessageFlow)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            SourceRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeSourceRef)
                .Required()
                .QNameAttributeReference<IInteractionNode>(/*typeof(IInteractionNode)*/)
                .Build();

            TargetRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeTargetRef)
                .Required()
                .QNameAttributeReference<IInteractionNode>(/*typeof(IInteractionNode)*/)
                .Build();

            MessageRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeMessageRef)
                .QNameAttributeReference<IMessage>(/*typeof(IMessage)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IMessageFlow>
        {
            public virtual IMessageFlow NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new MessageFlowImpl(instanceContext);
            }
        }

        public MessageFlowImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual IInteractionNode Source
        {
            get => SourceRefAttribute.GetReferenceTargetElement<IInteractionNode>(this);
            set => SourceRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IInteractionNode Target
        {
            get => TargetRefAttribute.GetReferenceTargetElement<IInteractionNode>(this);
            set => TargetRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IMessage Message
        {
            get => MessageRefAttribute.GetReferenceTargetElement<IMessage>(this);
            set => MessageRefAttribute.SetReferenceTargetElement(this, value);
        }

        public new IBpmnEdge DiagramElement => (IBpmnEdge) base.DiagramElement;
    }
}