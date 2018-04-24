using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;
using LabeledEdgeImpl = ESS.FW.Bpm.Model.Bpmn.impl.instance.di.LabeledEdgeImpl;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.bpmndi
{
    
    /// <summary>
    /// The BPMNDI BPMNEdge element
    /// 
    /// 
    /// </summary>
    public class BpmnEdgeImpl : LabeledEdgeImpl, IBpmnEdge
    {

        protected internal static IAttributeReference BpmnElementAttribute;//IAttributeReference<IBaseElement>
        protected internal static IAttributeReference SourceElementAttribute;//IAttributeReference<IDiagramElement>
        protected internal static IAttributeReference TargetElementAttribute;//IAttributeReference<IDiagramElement>
        protected internal static IAttribute/*<MessageVisibleKind>*/ MessageVisibleKindAttribute;
        protected internal static IChildElement/*<IBpmnLabel>*/ BpmnLabelChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBpmnEdge>(/*typeof(IBpmnEdge),*/ BpmnModelConstants.BpmndiElementBpmnEdge)
                .NamespaceUri(BpmnModelConstants.BpmndiNs)
                .ExtendsType(typeof(ILabeledEdge))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            BpmnElementAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmndiAttributeBpmnElement)
                .QNameAttributeReference<IBaseElement>(/*typeof(IBaseElement)*/)
                .Build();

            SourceElementAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmndiAttributeSourceElement)
                .QNameAttributeReference<IDiagramElement>(/*typeof(IDiagramElement)*/)
                .Build();

            TargetElementAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmndiAttributeTargetElement)
                .QNameAttributeReference<IDiagramElement>(/*typeof(IDiagramElement)*/)
                .Build();

            MessageVisibleKindAttribute = typeBuilder.EnumAttribute<MessageVisibleKind>(BpmnModelConstants.BpmndiAttributeMessageVisibleKind/*, typeof(MessageVisibleKind)*/).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            BpmnLabelChild = sequenceBuilder.Element<IBpmnLabel>(/*typeof(IBpmnLabel)*/).Build/*<IBpmnLabel>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IBpmnEdge>
        {
            public virtual IBpmnEdge NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new BpmnEdgeImpl(instanceContext);
            }
        }

        public BpmnEdgeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IBaseElement BpmnElement
        {
            get => BpmnElementAttribute.GetReferenceTargetElement<IBaseElement>(this);
            set => BpmnElementAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IDiagramElement SourceElement
        {
            get => SourceElementAttribute.GetReferenceTargetElement<IDiagramElement>(this);
            set => SourceElementAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IDiagramElement TargetElement
        {
            get => TargetElementAttribute.GetReferenceTargetElement<IDiagramElement>(this);
            set => TargetElementAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual MessageVisibleKind MessageVisibleKind
        {
            get => MessageVisibleKindAttribute.GetValue<MessageVisibleKind>(this);
            set => MessageVisibleKindAttribute.SetValue(this, value);
        }


        public virtual IBpmnLabel BpmnLabel
        {
            get => (IBpmnLabel)BpmnLabelChild.GetChild(this);
            set => BpmnLabelChild.SetChild(this, value);
        }


        //public override IExtension Extension { get; set; }
    }
}