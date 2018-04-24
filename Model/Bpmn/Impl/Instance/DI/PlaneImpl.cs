using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.di
{
    /// <summary>
    /// The DI Plane element
    /// 
    /// 
    /// </summary>
    public abstract class PlaneImpl : NodeImpl, IPlane
    {

        protected internal static IChildElementCollection/*<IDiagramElement>*/ DiagramElementCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IPlane>(/*typeof(IPlane),*/ BpmnModelConstants.DiElementPlane)
                .NamespaceUri(BpmnModelConstants.DiNs)
                .ExtendsType(typeof(INode)).AbstractType();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DiagramElementCollection = sequenceBuilder.ElementCollection<IDiagramElement>(/*typeof(IDiagramElement)*/).Build/*<IDiagramElement>*/();

            typeBuilder.Build();
        }

        public PlaneImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IDiagramElement> DiagramElements => DiagramElementCollection.Get<IDiagramElement>(this);
    }
}