using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public abstract class ConversationNodeImpl : BaseElementImpl, IConversationNode
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IElementReferenceCollection ParticipantRefCollection;//IElementReferenceCollection<IParticipant, ParticipantRef>
        protected internal static IElementReferenceCollection MessageFlowRefCollection;//IElementReferenceCollection<IMessageFlow, MessageFlowRef>
        protected internal static IChildElementCollection/*<ICorrelationKey>*/ CorrelationKeyCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IConversationNode>(/*typeof(IConversationNode),*/ BpmnModelConstants.BpmnElementConversationNode)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IBaseElement))
                    .AbstractType();

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ParticipantRefCollection = sequenceBuilder
                .ElementCollection<ParticipantRef>(/*typeof(ParticipantRef)*/)
                .QNameElementReferenceCollection<IParticipant>(/*typeof(IParticipant)*/)
                .Build();

            MessageFlowRefCollection = sequenceBuilder
                .ElementCollection<MessageFlowRef>(/*typeof(MessageFlowRef)*/)
                .QNameElementReferenceCollection<IMessageFlow>(/*typeof(IMessageFlow)*/)
                .Build();

            CorrelationKeyCollection = sequenceBuilder.ElementCollection<ICorrelationKey>(/*typeof(ICorrelationKey)*/).Build/*<ICorrelationKey>*/();

            typeBuilder.Build();
        }

        public ConversationNodeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual ICollection<IParticipant> Participants => ParticipantRefCollection.GetReferenceTargetElements<IParticipant>(this);

        public virtual ICollection<IMessageFlow> MessageFlows => MessageFlowRefCollection.GetReferenceTargetElements<IMessageFlow>(this);

        public virtual ICollection<ICorrelationKey> CorrelationKeys => CorrelationKeyCollection.Get<ICorrelationKey>(this);
    }

}