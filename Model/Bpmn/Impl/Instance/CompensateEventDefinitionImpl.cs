using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class CompensateEventDefinitionImpl : EventDefinitionImpl, ICompensateEventDefinition
    {

        protected internal static IAttribute/*<bool>*/ WaitForCompletionAttribute;
        protected internal static IAttributeReference ActivityRefAttribute;//IAttributeReference<IActivity>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICompensateEventDefinition>(/*typeof(ICompensateEventDefinition), */BpmnModelConstants.BpmnElementCompensateEventDefinition)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IEventDefinition))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            WaitForCompletionAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeWaitForCompletion).Build();

            ActivityRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeActivityRef)
                .QNameAttributeReference<IActivity>(/*typeof(IActivity)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICompensateEventDefinition>
        {
            public virtual ICompensateEventDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CompensateEventDefinitionImpl(instanceContext);
            }
        }

        public CompensateEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual bool WaitForCompletion
        {
            get => WaitForCompletionAttribute.GetValue<Boolean>(this);
            set => WaitForCompletionAttribute.SetValue(this, value);
        }


        public virtual IActivity Activity
        {
            get => ActivityRefAttribute.GetReferenceTargetElement<IActivity>(this);
            set => ActivityRefAttribute.SetReferenceTargetElement(this, value);
        }

    }

}