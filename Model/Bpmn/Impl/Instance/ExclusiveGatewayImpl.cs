

using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ExclusiveGatewayImpl : GatewayImpl, IExclusiveGateway
    {

        protected internal static IAttributeReference DefaultAttribute;//IAttributeReference<ISequenceFlow>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IExclusiveGateway>(/*typeof(IExclusiveGateway),*/ BpmnModelConstants.BpmnElementExclusiveGateway)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IGateway))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            DefaultAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeDefault)
                .IdAttributeReference<ISequenceFlow>(/*typeof(ISequenceFlow)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IExclusiveGateway>
        {
            public virtual IExclusiveGateway NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ExclusiveGatewayImpl(instanceContext);
            }
        }

        public ExclusiveGatewayImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new ExclusiveGatewayBuilder Builder()
        {
            return new ExclusiveGatewayBuilder((IBpmnModelInstance)modelInstance, this);
        }


        public virtual ISequenceFlow Default
        {
            get => DefaultAttribute.GetReferenceTargetElement<ISequenceFlow>(this);
            set => DefaultAttribute.SetReferenceTargetElement(this, value);
        }

    }

}