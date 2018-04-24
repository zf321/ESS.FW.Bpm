using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    /// <summary>
    /// The BPMN ioBinding element
    /// 
    /// 
    /// </summary>
    public class IoBindingImpl : BaseElementImpl, IOBinding
    {

        protected internal static IAttributeReference OperationRefAttribute;//IAttributeReference<IOperation>
        protected internal static IAttributeReference InputDataRefAttribute;//IAttributeReference<IDataInput>
        protected internal static IAttributeReference OutputDataRefAttribute;//IAttributeReference<IDataOutput>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IOBinding>(/*typeof(IOBinding),*/ BpmnModelConstants.BpmnElementIoBinding)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            OperationRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeOperationRef)
                .Required()
                .QNameAttributeReference<IOperation>(/*typeof(IOperation)*/)
                .Build();

            InputDataRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeInputDataRef)
                .Required()
                .IdAttributeReference<IDataInput>(/*typeof(IDataInput)*/)
                .Build();

            OutputDataRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeOutputDataRef)
                .Required()
                .IdAttributeReference<IDataOutput>(/*typeof(IDataOutput)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IOBinding>
        {
            public virtual IOBinding NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new IoBindingImpl(instanceContext);
            }
        }

        public IoBindingImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IOperation Operation
        {
            get => OperationRefAttribute.GetReferenceTargetElement<IOperation>(this);
            set => OperationRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IDataInput InputData
        {
            get => InputDataRefAttribute.GetReferenceTargetElement<IDataInput>(this);
            set => InputDataRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IDataOutput OutputData
        {
            get => OutputDataRefAttribute.GetReferenceTargetElement<IDataOutput>(this);
            set => OutputDataRefAttribute.SetReferenceTargetElement(this, value);
        }
    }
}