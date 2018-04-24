using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public abstract class ThrowEventImpl : EventImpl, IThrowEvent
    {

        protected internal static IChildElementCollection/*<IDataInput>*/ DataInputCollection;
        protected internal static IChildElementCollection/*<IDataInputAssociation>*/ DataInputAssociationCollection;
        protected internal static IChildElement/*<INputSet>*/ InputSetChild;
        protected internal static IChildElementCollection/*<IEventDefinition>*/ EventDefinitionCollection;
        protected internal static IElementReferenceCollection EventDefinitionRefCollection;//IElementReferenceCollection<IEventDefinition, EventDefinitionRef>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IThrowEvent>(/*typeof(IThrowEvent),*/ BpmnModelConstants.BpmnElementThrowEvent)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IEvent))
                .AbstractType();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DataInputCollection = sequenceBuilder
                .ElementCollection<IDataInput>(/*typeof(IDataInput)*/)
                .Build/*<IDataInput>*/();

            DataInputAssociationCollection = sequenceBuilder
                .ElementCollection<IDataInputAssociation>(/*typeof(IDataInputAssociation)*/)
                .Build/*<IDataInputAssociation>*/();

            InputSetChild = sequenceBuilder
                .Element<INputSet>(/*typeof(INputSet)*/)
                .Build/*<INputSet>*/();

            EventDefinitionCollection = sequenceBuilder
                .ElementCollection<IEventDefinition>(/*typeof(IEventDefinition)*/)
                .Build/*<IEventDefinition>*/();

            EventDefinitionRefCollection = sequenceBuilder
                .ElementCollection<EventDefinitionRef>(/*typeof(EventDefinitionRef)*/)
                .QNameElementReferenceCollection<IEventDefinition>(/*typeof(IEventDefinition)*/)
                .Build();

            typeBuilder.Build();
        }

        public ThrowEventImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual ICollection<IDataInput> DataInputs => DataInputCollection.Get<IDataInput>(this);

        public virtual ICollection<IDataInputAssociation> DataInputAssociations => DataInputAssociationCollection.Get<IDataInputAssociation>(this);

        public virtual INputSet InputSet
        {
            get => (INputSet)InputSetChild.GetChild(this);
            set => InputSetChild.SetChild(this, value);
        }

        public virtual ICollection<IEventDefinition> EventDefinitions => EventDefinitionCollection.Get<IEventDefinition>(this);

        public virtual ICollection<IEventDefinition> EventDefinitionRefs => EventDefinitionRefCollection.GetReferenceTargetElements<IEventDefinition>(this);
    }
}