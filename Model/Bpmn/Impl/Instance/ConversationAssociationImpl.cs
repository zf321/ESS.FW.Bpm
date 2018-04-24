using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ConversationAssociationImpl : BaseElementImpl, IConversationAssociation
    {

        protected internal static IAttributeReference InnerConversationNodeRefAttribute;//IAttributeReference<IConversationNode>
        protected internal static IAttributeReference OuterConversationNodeRefAttribute;//IAttributeReference<IConversationNode>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IConversationAssociation>(/*typeof(IConversationAssociation),*/ BpmnModelConstants.BpmnElementConversationAssociation)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IBaseElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            InnerConversationNodeRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeInnerConversationNodeRef)
                .Required()
                .QNameAttributeReference<IConversationNode>(/*typeof(IConversationNode)*/)
                .Build();

            OuterConversationNodeRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeOuterConversationNodeRef)
                .Required().QNameAttributeReference<IConversationNode>(/*typeof(IConversationNode)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IConversationAssociation>
        {
            public virtual IConversationAssociation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ConversationAssociationImpl(instanceContext);
            }
        }

        public ConversationAssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IConversationNode InnerConversationNode
        {
            get => InnerConversationNodeRefAttribute.GetReferenceTargetElement<IConversationNode>(this);
            set => InnerConversationNodeRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IConversationNode OuterConversationNode
        {
            get => OuterConversationNodeRefAttribute.GetReferenceTargetElement<IConversationNode>(this);
            set => OuterConversationNodeRefAttribute.SetReferenceTargetElement(this, value);
        }

    }

}