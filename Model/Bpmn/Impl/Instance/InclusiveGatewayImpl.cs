

using System;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    /// <summary>
    /// The BPMN inclusiveGateway element
    /// 
    /// 
    /// </summary>
    public class InclusiveGatewayImpl : GatewayImpl, INclusiveGateway
    {

        protected internal static IAttributeReference DefaultAttribute;//IAttributeReference<ISequenceFlow>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INclusiveGateway>(/*typeof(INclusiveGateway),*/ BpmnModelConstants.BpmnElementInclusiveGateway)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IGateway))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            DefaultAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeDefault)
                .IdAttributeReference<ISequenceFlow>(/*typeof(ISequenceFlow)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<INclusiveGateway>
        {
            public virtual INclusiveGateway NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InclusiveGatewayImpl(instanceContext);
            }
        }

        public InclusiveGatewayImpl(ModelTypeInstanceContext context) : base(context)
        {
        }
        
        public new virtual InclusiveGatewayBuilder Builder()
        {
            return new InclusiveGatewayBuilder((IBpmnModelInstance) modelInstance, this);
        }
        

        public virtual ISequenceFlow Default
        {
            get => DefaultAttribute.GetReferenceTargetElement<ISequenceFlow>(this);
            set => DefaultAttribute.SetReferenceTargetElement(this, value);
        }

    }

}