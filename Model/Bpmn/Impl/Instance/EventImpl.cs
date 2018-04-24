using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;


namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public abstract class EventImpl : FlowNodeImpl, IEvent
    {

        protected internal static IChildElementCollection/*<IProperty>*/ PropertyCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IEvent>(/*typeof(IEvent),*/ BpmnModelConstants.BpmnElementEvent)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IFlowNode))
                .AbstractType();

            ISequenceBuilder sequence = typeBuilder.Sequence();

            PropertyCollection = sequence.ElementCollection<IProperty>(/*typeof(IProperty)*/).Build/*<IProperty>*/();

            typeBuilder.Build();
        }

        public EventImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual ICollection<IProperty> Properties => PropertyCollection.Get<IProperty>(this);

        public new IBpmnShape DiagramElement => (IBpmnShape)base.DiagramElement;
    }
}