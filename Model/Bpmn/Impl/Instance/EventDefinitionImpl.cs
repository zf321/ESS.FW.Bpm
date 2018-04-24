using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public abstract class EventDefinitionImpl : RootElementImpl, IEventDefinition
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IEventDefinition>(/*typeof(IEventDefinition),*/ BpmnModelConstants.BpmnElementEventDefinition)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IRootElement))
                .AbstractType();

            typeBuilder.Build();
        }

        public EventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

    }

}