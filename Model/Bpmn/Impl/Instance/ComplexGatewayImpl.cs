using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ComplexGatewayImpl : GatewayImpl, IComplexGateway
    {

        protected internal static IAttributeReference DefaultAttribute;//IAttributeReference<ISequenceFlow>
        protected internal static IChildElement/*<IActivationCondition>*/ ActivationConditionChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IComplexGateway>(/*typeof(IComplexGateway),*/ BpmnModelConstants.BpmnElementComplexGateway)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IGateway))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            DefaultAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeDefault)
                .IdAttributeReference<ISequenceFlow>(/*typeof(ISequenceFlow)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ActivationConditionChild = sequenceBuilder.Element<IActivationCondition>(/*typeof(IActivationCondition)*/).Build/*<IActivationCondition>*/();

            typeBuilder.Build();
        }


        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IComplexGateway>
        {
            public virtual IComplexGateway NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ComplexGatewayImpl(instanceContext);
            }
        }

        public ComplexGatewayImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new ComplexGatewayBuilder Builder()
        {
            return new ComplexGatewayBuilder((IBpmnModelInstance)modelInstance, this);
        }


        public virtual ISequenceFlow Default
        {
            get => DefaultAttribute.GetReferenceTargetElement<ISequenceFlow>(this);
            set => DefaultAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IActivationCondition ActivationCondition
        {
            get => (IActivationCondition)ActivationConditionChild.GetChild(this);
            set => ActivationConditionChild.SetChild(this, value);
        }
    }
}