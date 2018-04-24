

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class EscalationEventDefinitionImpl : EventDefinitionImpl, IEscalationEventDefinition
    {

        protected internal static IAttributeReference EscalationRefAttribute;//IAttributeReference<IEscalation>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IEscalationEventDefinition>(/*typeof(IEscalationEventDefinition),*/ BpmnModelConstants.BpmnElementEscalationEventDefinition)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IEventDefinition))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            EscalationRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeEscalationRef)
                .QNameAttributeReference<IEscalation>(/*typeof(IEscalation)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IEscalationEventDefinition>
        {
            public virtual IEscalationEventDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new EscalationEventDefinitionImpl(instanceContext);
            }
        }

        public EscalationEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual IEscalation Escalation
        {
            get => EscalationRefAttribute.GetReferenceTargetElement<IEscalation>(this);
            set => EscalationRefAttribute.SetReferenceTargetElement(this, value);
        }


    }

}