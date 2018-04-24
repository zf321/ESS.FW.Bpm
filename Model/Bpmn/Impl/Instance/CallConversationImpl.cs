using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class CallConversationImpl : ConversationNodeImpl, ICallConversation
    {

        protected internal static IAttributeReference CalledCollaborationRefAttribute;//IAttributeReference<IGlobalConversation>
        protected internal static IChildElementCollection/*<IParticipantAssociation>*/ ParticipantAssociationCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICallConversation>(/*typeof(ICallConversation),*/ BpmnModelConstants.BpmnElementCallConversation)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IConversationNode))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CalledCollaborationRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeCalledCollaborationRef)
                .QNameAttributeReference<IGlobalConversation>(/*typeof(IGlobalConversation)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ParticipantAssociationCollection = sequenceBuilder.ElementCollection<IParticipantAssociation>(/*typeof(IParticipantAssociation)*/).Build/*<IParticipantAssociation>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICallConversation>
        {
            public virtual ICallConversation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CallConversationImpl(instanceContext);
            }
        }

        public CallConversationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IGlobalConversation CalledCollaboration
        {
            get => CalledCollaborationRefAttribute.GetReferenceTargetElement<IGlobalConversation>(this);
            set => CalledCollaborationRefAttribute.SetReferenceTargetElement(this, value);
        }

        public virtual ICollection<IParticipantAssociation> ParticipantAssociations => ParticipantAssociationCollection.Get<IParticipantAssociation>(this);
    }

}