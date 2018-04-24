using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public abstract class RootElementImpl : BaseElementImpl, IRootElement
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IRootElement>(/*typeof(IRootElement),*/ BpmnModelConstants.BpmnElementRootElement)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .AbstractType();

            typeBuilder.Build();
        }

        public RootElementImpl(ModelTypeInstanceContext context) : base(context)
        {
        }
    }
}