using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class ItemAwareElementImpl : BaseElementImpl, IItemAwareElement
    {

        protected internal static IAttributeReference ItemSubjectRefAttribute;//IAttributeReference<IItemDefinition>
        protected internal static IChildElement/*<IDataState>*/ DataStateChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IItemAwareElement>(/*typeof(IItemAwareElement),*/ BpmnModelConstants.BpmnElementItemAwareElement)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .AbstractType();

            ItemSubjectRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeItemSubjectRef)
                .QNameAttributeReference<IItemDefinition>(/*typeof(IItemDefinition)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DataStateChild = sequenceBuilder
                .Element<IDataState>(/*typeof(IDataState)*/)
                .Build/*<IDataState>*/();

            typeBuilder.Build();
        }

        public ItemAwareElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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
    }
}