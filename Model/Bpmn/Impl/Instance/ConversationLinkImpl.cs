using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ConversationLinkImpl : BaseElementImpl, IConversationLink
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttributeReference SourceRefAttribute;//IAttributeReference<IInteractionNode>
        protected internal static IAttributeReference TargetRefAttribute;//IAttributeReference<IInteractionNode>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IConversationLink>(/*typeof(IConversationLink),*/ BpmnModelConstants.BpmnElementConversationLink)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IBaseElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            SourceRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeSourceRef)
                .Required()
                .QNameAttributeReference<IInteractionNode>(/*typeof(IInteractionNode)*/)
                .Build();

            TargetRefAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeTargetRef)
                .Required()
                .QNameAttributeReference<IInteractionNode>(/*typeof(IInteractionNode)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IConversationLink>
        {
            public virtual IConversationLink NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ConversationLinkImpl(instanceContext);
            }
        }

        public ConversationLinkImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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

    }

}