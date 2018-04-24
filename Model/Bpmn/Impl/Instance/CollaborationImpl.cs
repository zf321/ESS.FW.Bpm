using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class CollaborationImpl : RootElementImpl, ICollaboration
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttribute/*<bool>*/ IsClosedAttribute;
        protected internal static IChildElementCollection/*<IParticipant>*/ ParticipantCollection;
        protected internal static IChildElementCollection/*<IMessageFlow>*/ MessageFlowCollection;
        protected internal static IChildElementCollection/*<IArtifact>*/ ArtifactCollection;
        protected internal static IChildElementCollection/*<IConversationNode>*/ ConversationNodeCollection;
        protected internal static IChildElementCollection/*<IConversationAssociation>*/ ConversationAssociationCollection;
        protected internal static IChildElementCollection/*<IParticipantAssociation>*/ ParticipantAssociationCollection;
        protected internal static IChildElementCollection/*<IMessageFlowAssociation>*/ MessageFlowAssociationCollection;
        protected internal static IChildElementCollection/*<ICorrelationKey>*/ CorrelationKeyCollection;
        /// <summary>
        /// TODO: choreographyRef </summary>
        protected internal static IChildElementCollection/*<IConversationLink>*/ ConversationLinkCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICollaboration>(/*typeof(ICollaboration),*/ BpmnModelConstants.BpmnElementCollaboration)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IRootElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            IsClosedAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeIsClosed).DefaultValue(false).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ParticipantCollection = sequenceBuilder.ElementCollection<IParticipant>(/*typeof(IParticipant)*/).Build/*<IParticipant>*/();

            MessageFlowCollection = sequenceBuilder.ElementCollection<IMessageFlow>(/*typeof(IMessageFlow)*/).Build/*<IMessageFlow>*/();

            ArtifactCollection = sequenceBuilder.ElementCollection<IArtifact>(/*typeof(IArtifact)*/).Build/*<IArtifact>*/();

            ConversationNodeCollection = sequenceBuilder.ElementCollection<IConversationNode>(/*typeof(IConversationNode)*/).Build/*<IConversationNode>*/();

            ConversationAssociationCollection = sequenceBuilder.ElementCollection<IConversationAssociation>(/*typeof(IConversationAssociation)*/).Build/*<IConversationAssociation>*/();

            ParticipantAssociationCollection = sequenceBuilder.ElementCollection<IParticipantAssociation>(/*typeof(IParticipantAssociation)*/).Build/*<IParticipantAssociation>*/();

            MessageFlowAssociationCollection = sequenceBuilder.ElementCollection<IMessageFlowAssociation>(/*typeof(IMessageFlowAssociation)*/).Build/*<IMessageFlowAssociation>*/();

            CorrelationKeyCollection = sequenceBuilder.ElementCollection<ICorrelationKey>(/*typeof(ICorrelationKey)*/).Build/*<ICorrelationKey>*/();

            ConversationLinkCollection = sequenceBuilder.ElementCollection<IConversationLink>(/*typeof(IConversationLink)*/).Build/*<IConversationLink>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICollaboration>
        {
            public virtual ICollaboration NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CollaborationImpl(instanceContext);
            }
        }

        public CollaborationImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual bool Closed
        {
            get => IsClosedAttribute.GetValue<Boolean>(this);
            set => IsClosedAttribute.SetValue(this, value);
        }


        public virtual ICollection<IParticipant> Participants => ParticipantCollection.Get<IParticipant>(this);

        public virtual ICollection<IMessageFlow> MessageFlows => MessageFlowCollection.Get<IMessageFlow>(this);

        public virtual ICollection<IArtifact> Artifacts => ArtifactCollection.Get<IArtifact>(this);

        public virtual ICollection<IConversationNode> ConversationNodes => ConversationNodeCollection.Get<IConversationNode>(this);

        public virtual ICollection<IConversationAssociation> ConversationAssociations => ConversationAssociationCollection.Get<IConversationAssociation>(this);

        public virtual ICollection<IParticipantAssociation> ParticipantAssociations => ParticipantAssociationCollection.Get<IParticipantAssociation>(this);

        public virtual ICollection<IMessageFlowAssociation> MessageFlowAssociations => MessageFlowAssociationCollection.Get<IMessageFlowAssociation>(this);

        public virtual ICollection<ICorrelationKey> CorrelationKeys => CorrelationKeyCollection.Get<ICorrelationKey>(this);

        public virtual ICollection<IConversationLink> ConversationLinks => ConversationLinkCollection.Get<IConversationLink>(this);
    }

}