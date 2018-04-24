

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public abstract class InteractionNodeImpl : BpmnModelElementInstanceImpl, IInteractionNode
    {
        //public abstract string Id { set; get; }

        protected internal static IAttribute/*<string>*/ IdAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IInteractionNode>(/*typeof(IInteractionNode),*/ "")
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .AbstractType();

            IdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeId)
                .IdAttribute()
                .Build();

            typeBuilder.Build();
        }

        public InteractionNodeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}