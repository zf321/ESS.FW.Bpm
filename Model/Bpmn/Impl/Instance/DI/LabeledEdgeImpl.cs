using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.di
{
    public abstract class LabeledEdgeImpl : EdgeImpl, ILabeledEdge
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ILabeledEdge>(/*typeof(ILabeledEdge),*/ BpmnModelConstants.DiElementLabeledEdge)
                .NamespaceUri(BpmnModelConstants.DiNs)
                .ExtendsType(typeof(IEdge))
                .AbstractType();

            typeBuilder.Build();
        }

        public LabeledEdgeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}