using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class CorrelationPropertyBindingImpl : BaseElementImpl, ICorrelationPropertyBinding
    {

        protected internal static IAttributeReference CorrelationPropertyRefAttribute;//IAttributeReference<ICorrelationProperty>
        protected internal static IChildElement/*<DataPath>*/ DataPathChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICorrelationPropertyBinding>(/*typeof(ICorrelationPropertyBinding),*/ BpmnModelConstants.BpmnElementCorrelationPropertyBinding)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IBaseElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CorrelationPropertyRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeCorrelationPropertyRef)
                .Required()
                .QNameAttributeReference<ICorrelationProperty>(/*typeof(ICorrelationProperty)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DataPathChild = sequenceBuilder.Element<DataPath>(/*typeof(DataPath)*/).Required().Build/*<DataPath>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICorrelationPropertyBinding>
        {
            public virtual ICorrelationPropertyBinding NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CorrelationPropertyBindingImpl(instanceContext);
            }
        }

        public CorrelationPropertyBindingImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICorrelationProperty CorrelationProperty
        {
            get => CorrelationPropertyRefAttribute.GetReferenceTargetElement<ICorrelationProperty>(this);
            set => CorrelationPropertyRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual DataPath DataPath
        {
            get => (DataPath)DataPathChild.GetChild(this);
            set => DataPathChild.SetChild(this, value);
        }

    }

}