using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class SignalImpl : BaseElementImpl, ISignal
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttributeReference StructureRefAttribute;//IAttributeReference<IItemDefinition>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ISignal>(/*typeof(ISignal),*/ BpmnModelConstants.BpmnElementSignal)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IRootElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            StructureRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeStructureRef)
                .QNameAttributeReference<IItemDefinition>(/*typeof(IItemDefinition)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ISignal>
        {
            public virtual ISignal NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new SignalImpl(instanceContext);
            }
        }

        public SignalImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual IItemDefinition Structure
        {
            get => StructureRefAttribute.GetReferenceTargetElement<IItemDefinition>(this);
            set => StructureRefAttribute.SetReferenceTargetElement(this, value);
        }
    }
}