using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class EventDefinitionRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<EventDefinitionRef>(/*typeof(EventDefinitionRef),*/ BpmnModelConstants.BpmnElementEventDefinitionRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<EventDefinitionRef>
        {
            public virtual EventDefinitionRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new EventDefinitionRef(instanceContext);
            }
        }

        public EventDefinitionRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}