using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.reference;
using LabelImpl = ESS.FW.Bpm.Model.Bpmn.impl.instance.di.LabelImpl;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.bpmndi
{

    /// <summary>
    /// The BPMNDI BPMNLabel element
    /// 
    /// 
    /// </summary>
    public class BpmnLabelImpl : LabelImpl, IBpmnLabel
    {

        protected internal static IAttributeReference LabelStyleAttribute;//IAttributeReference<IBpmnLabelStyle>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBpmnLabel>(/*typeof(IBpmnLabel),*/ BpmnModelConstants.BpmndiElementBpmnLabel)
                .NamespaceUri(BpmnModelConstants.BpmndiNs)
                .ExtendsType(typeof(ILabel))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            LabelStyleAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmndiAttributeLabelStyle)
                .QNameAttributeReference<IBpmnLabelStyle>(/*typeof(IBpmnLabelStyle)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IBpmnLabel>
        {
            public virtual IBpmnLabel NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new BpmnLabelImpl(instanceContext);
            }
        }

        public BpmnLabelImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IBpmnLabelStyle LabelStyle
        {
            get => LabelStyleAttribute.GetReferenceTargetElement<IBpmnLabelStyle>(this);
            set => LabelStyleAttribute.SetReferenceTargetElement(this, value);
        }

        //public override IExtension Extension { get; set; }
    }
}