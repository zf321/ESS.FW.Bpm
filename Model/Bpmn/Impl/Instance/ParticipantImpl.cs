using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    /// <summary>
    /// The BPMN participant element
    /// 
    /// 
    /// </summary>
    public class ParticipantImpl : BaseElementImpl, IParticipant
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttributeReference ProcessRefAttribute;//IAttributeReference<IProcess>
        protected internal static IElementReferenceCollection InterfaceRefCollection;//IElementReferenceCollection<INterface, InterfaceRef>
        protected internal static IElementReferenceCollection EndPointRefCollection;//IElementReferenceCollection<IEndPoint, EndPointRef>
        protected internal static IChildElement/*<IParticipantMultiplicity>*/ ParticipantMultiplicityChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IParticipant>(/*typeof(IParticipant),*/ BpmnModelConstants.BpmnElementParticipant)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            ProcessRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeProcessRef)
                .QNameAttributeReference<IProcess>(/*typeof(IProcess)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            InterfaceRefCollection = sequenceBuilder
                .ElementCollection<InterfaceRef>(/*typeof(InterfaceRef)*/)
                .QNameElementReferenceCollection<INterface>(/*typeof(INterface)*/)
                .Build();

            EndPointRefCollection = sequenceBuilder
                .ElementCollection<EndPointRef>(/*typeof(EndPointRef)*/)
                .QNameElementReferenceCollection<IEndPoint>(/*typeof(IEndPoint)*/)
                .Build();

            ParticipantMultiplicityChild = sequenceBuilder.Element<IParticipantMultiplicity>(/*typeof(IParticipantMultiplicity)*/).Build/*<IParticipantMultiplicity>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IParticipant>
        {
            public virtual IParticipant NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ParticipantImpl(instanceContext);
            }
        }

        public ParticipantImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual IProcess Process
        {
            get => ProcessRefAttribute.GetReferenceTargetElement<IProcess>(this);
            set => ProcessRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual ICollection<INterface> Interfaces => InterfaceRefCollection.GetReferenceTargetElements<INterface>(this);

        public virtual ICollection<IEndPoint> EndPoints => EndPointRefCollection.GetReferenceTargetElements<IEndPoint>(this);

        public virtual IParticipantMultiplicity ParticipantMultiplicity
        {
            get => (IParticipantMultiplicity)ParticipantMultiplicityChild.GetChild(this);
            set => ParticipantMultiplicityChild.SetChild(this, value);
        }
    }
}