using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class DataObjectImpl : FlowElementImpl, IDataObject
    {

        protected internal static IAttributeReference ItemSubjectRefAttribute;//IAttributeReference<IItemDefinition>
        protected internal static IAttribute/*<bool>*/ IsCollectionAttribute;
        protected internal static IChildElement/*<IDataState>*/ DataStateChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDataObject>(/*typeof(IDataObject),*/ BpmnModelConstants.BpmnElementDataObject)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IFlowElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ItemSubjectRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeItemSubjectRef)
                .QNameAttributeReference<IItemDefinition>(/*typeof(IItemDefinition)*/)
                .Build();

            IsCollectionAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeIsCollection).DefaultValue(false).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DataStateChild = sequenceBuilder.Element<IDataState>(/*typeof(IDataState)*/).Build/*<IDataState>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDataObject>
        {
            public virtual IDataObject NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DataObjectImpl(instanceContext);
            }
        }

        public DataObjectImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IItemDefinition ItemSubject
        {
            get => ItemSubjectRefAttribute.GetReferenceTargetElement<IItemDefinition>(this);
            set => ItemSubjectRefAttribute.SetReferenceTargetElement(this, value);
        }

        public virtual IDataState DataState
        {
            get => (IDataState)DataStateChild.GetChild(this);
            set => DataStateChild.SetChild(this, value);
        }

        public virtual bool Collection
        {
            get => IsCollectionAttribute.GetValue<Boolean>(this);
            set => IsCollectionAttribute.SetValue(this, value);
        }

    }

}