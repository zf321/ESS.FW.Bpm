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
    public abstract class CatchEventImpl : EventImpl, ICatchEvent
    {
        protected internal static IAttribute/*<bool>*/ ParallelMultipleAttribute;
        protected internal static IChildElementCollection/*<IDataOutput>*/ DataOutputCollection;
        protected internal static IChildElementCollection/*<IDataOutputAssociation>*/ DataOutputAssociationCollection;
        protected internal static IChildElement/*<IOutputSet>*/ OutputSetChild;
        protected internal static IChildElementCollection/*<IEventDefinition>*/ EventDefinitionCollection;
        protected internal static IElementReferenceCollection EventDefinitionRefCollection;//IElementReferenceCollection<IEventDefinition, EventDefinitionRef>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICatchEvent>(/*typeof(ICatchEvent),*/ BpmnModelConstants.BpmnElementCatchEvent)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IEvent))
                .AbstractType();

            ParallelMultipleAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeParallelMultiple).DefaultValue(false).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DataOutputCollection = sequenceBuilder.ElementCollection<IDataOutput>(/*typeof(IDataOutput)*/).Build/*<IDataOutput>*/();

            DataOutputAssociationCollection = sequenceBuilder.ElementCollection<IDataOutputAssociation>(/*typeof(IDataOutputAssociation)*/).Build/*<IDataOutputAssociation>*/();

            OutputSetChild = sequenceBuilder.Element<IOutputSet>(/*typeof(IOutputSet)*/).Build/*<IOutputSet>*/();

            EventDefinitionCollection = sequenceBuilder.ElementCollection<IEventDefinition>(/*typeof(IEventDefinition)*/).Build/*<IEventDefinition>*/();

            EventDefinitionRefCollection = sequenceBuilder
                .ElementCollection<EventDefinitionRef>(/*typeof(EventDefinitionRef)*/)
                .QNameElementReferenceCollection<IEventDefinition>(/*typeof(IEventDefinition)*/)
                .Build();

            typeBuilder.Build();
        }


        public CatchEventImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual bool ParallelMultiple
        {
            get => ParallelMultipleAttribute.GetValue<Boolean>(this);
            set => ParallelMultipleAttribute.SetValue(this, value);
        }


        public virtual ICollection<IDataOutput> DataOutputs => DataOutputCollection.Get<IDataOutput>(this);

        public virtual ICollection<IDataOutputAssociation> DataOutputAssociations => DataOutputAssociationCollection.Get<IDataOutputAssociation>(this);

        public virtual IOutputSet OutputSet
        {
            get => (IOutputSet)OutputSetChild.GetChild(this);
            set => OutputSetChild.SetChild(this, value);
        }


        public virtual ICollection<IEventDefinition> EventDefinitions => EventDefinitionCollection.Get<IEventDefinition>(this);

        public virtual ICollection<IEventDefinition> EventDefinitionRefs => EventDefinitionRefCollection.GetReferenceTargetElements<IEventDefinition>(this);
    }

}