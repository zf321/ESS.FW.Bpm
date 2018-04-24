

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ParticipantAssociationImpl : BaseElementImpl, IParticipantAssociation
    {

        protected internal static IElementReference InnerParticipantRefChild;//IElementReference<IParticipant, InnerParticipantRef>
        protected internal static IElementReference OuterParticipantRefChild;//IElementReference<IParticipant, OuterParticipantRef>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IParticipantAssociation>(/*typeof(IParticipantAssociation),*/ BpmnModelConstants.BpmnElementParticipantAssociation)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            InnerParticipantRefChild = sequenceBuilder
                .Element<InnerParticipantRef>(/*typeof(InnerParticipantRef)*/)
                .Required()
                .QNameElementReference<IParticipant>(/*typeof(IParticipant)*/)
                .Build();

            OuterParticipantRefChild = sequenceBuilder
                .Element<OuterParticipantRef>(/*typeof(OuterParticipantRef)*/)
                .Required()
                .QNameElementReference<IParticipant>(/*typeof(IParticipant)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IParticipantAssociation>
        {
            public virtual IParticipantAssociation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ParticipantAssociationImpl(instanceContext);
            }
        }

        public ParticipantAssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IParticipant InnerParticipant
        {
            get => (IParticipant)InnerParticipantRefChild.GetReferenceTargetElement(this);
            set => InnerParticipantRefChild.SetReferenceTargetElement(this, value);
        }


        public virtual IParticipant OuterParticipant
        {
            get => (IParticipant)OuterParticipantRefChild.GetReferenceTargetElement(this);
            set => OuterParticipantRefChild.SetReferenceTargetElement(this, value);
        }

    }
}