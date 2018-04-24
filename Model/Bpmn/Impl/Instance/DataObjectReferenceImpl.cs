using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DataObjectReferenceImpl : FlowElementImpl, IDataObjectReference
    {

        protected internal static IAttributeReference ItemSubjectRefAttribute;//IAttributeReference<IItemDefinition>
        protected internal static IAttributeReference DataObjectRefAttribute;//IAttributeReference<IDataObject>
        protected internal static IChildElement/*<IDataState>*/ DataStateChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDataObjectReference>(/*typeof(IDataObjectReference),*/ BpmnModelConstants.BpmnElementDataObjectReference)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IFlowElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ItemSubjectRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeItemSubjectRef)
                .QNameAttributeReference<IItemDefinition>(/*typeof(IItemDefinition)*/)
                .Build();

            DataObjectRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeDataObjectRef)
                .IdAttributeReference<IDataObject>(/*typeof(IDataObject)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DataStateChild = sequenceBuilder.Element<IDataState>(/*typeof(IDataState)*/).Build/*<IDataState>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDataObjectReference>
        {
            public virtual IDataObjectReference NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DataObjectReferenceImpl(instanceContext);
            }
        }

        public DataObjectReferenceImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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

        public virtual IDataObject DataObject
        {
            get => DataObjectRefAttribute.GetReferenceTargetElement<IDataObject>(this);
            set => DataObjectRefAttribute.SetReferenceTargetElement(this, value);
        }
    }
}