using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class TerminateEventDefinitionImpl : EventDefinitionImpl, ITerminateEventDefinition
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ITerminateEventDefinition>(/*typeof(ITerminateEventDefinition),*/ BpmnModelConstants.BpmnElementTerminateEventDefinition)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IEventDefinition))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ITerminateEventDefinition>
        {
            public virtual ITerminateEventDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new TerminateEventDefinitionImpl(instanceContext);
            }
        }

        public TerminateEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

    }
}