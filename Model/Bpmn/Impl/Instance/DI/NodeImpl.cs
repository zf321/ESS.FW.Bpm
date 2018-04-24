

using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.di
{

    /// <summary>
    /// The DI Node element
    /// 
    /// 
    /// </summary>
    public abstract class NodeImpl : DiagramElementImpl, INode
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INode>(/*typeof(INode),*/ BpmnModelConstants.DiElementNode)
                .NamespaceUri(BpmnModelConstants.DiNs)
                .ExtendsType(typeof(IDiagramElement))
                .AbstractType();

            typeBuilder.Build();
        }

        public NodeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}