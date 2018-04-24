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
    public class SignalEventDefinitionImpl : EventDefinitionImpl, ISignalEventDefinition
    {

        protected internal static IAttributeReference SignalRefAttribute;//IAttributeReference<ISignal>
        protected internal static IAttribute/*<bool>*/ CamundaAsyncAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ISignalEventDefinition>(/*typeof(ISignalEventDefinition),*/ BpmnModelConstants.BpmnElementSignalEventDefinition)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IEventDefinition))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            SignalRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeSignalRef)
                .QNameAttributeReference<ISignal>(/*typeof(ISignal)*/)
                .Build();
            
            CamundaAsyncAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.CamundaAttributeAsync)
                .Namespace(BpmnModelConstants.CamundaNs)
                .DefaultValue(false)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ISignalEventDefinition>
        {
            public virtual ISignalEventDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new SignalEventDefinitionImpl(instanceContext);
            }
        }

        public SignalEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual ISignal Signal
        {
            get => SignalRefAttribute.GetReferenceTargetElement<ISignal>(this);
            set => SignalRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual bool CamundaAsync
        {
            get => CamundaAsyncAttribute.GetValue<Boolean>(this);
            set => CamundaAsyncAttribute.SetValue(this, value);
        }
    }
}