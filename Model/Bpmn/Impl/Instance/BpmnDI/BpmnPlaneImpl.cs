using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.reference;
using PlaneImpl = ESS.FW.Bpm.Model.Bpmn.impl.instance.di.PlaneImpl;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.bpmndi
{


    /// <summary>
    /// The BPMNDI BPMNPlane element
    /// 
    /// 
    /// </summary>
    public class BpmnPlaneImpl : PlaneImpl, IBpmnPlane
    {

        protected internal static IAttributeReference BpmnElementAttribute;//IAttributeReference<IBaseElement>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBpmnPlane>(/*typeof(IBpmnPlane), */BpmnModelConstants.BpmndiElementBpmnPlane)
                .NamespaceUri(BpmnModelConstants.BpmndiNs)
                .ExtendsType(typeof(IPlane))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            BpmnElementAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmndiAttributeBpmnElement)
                .QNameAttributeReference<IBaseElement>(/*typeof(IBaseElement)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IBpmnPlane>
        {
            public virtual IBpmnPlane NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new BpmnPlaneImpl(instanceContext);
            }
        }

        public BpmnPlaneImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IBaseElement BpmnElement
        {
            get =>  BpmnElementAttribute.GetReferenceTargetElement<IBaseElement>(this);
            set => BpmnElementAttribute.SetReferenceTargetElement(this, value);
        }

        //public override IExtension Extension { get; set; }
    }
}