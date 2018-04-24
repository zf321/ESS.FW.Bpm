using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.di
{
    public abstract class EdgeImpl : DiagramElementImpl, IEdge
    {

        protected internal static IChildElementCollection/*<IWaypoint>*/ WaypointCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IEdge>(/*typeof(IEdge),*/ BpmnModelConstants.DiElementEdge)
                .NamespaceUri(BpmnModelConstants.DiNs)
                .ExtendsType(typeof(IDiagramElement))
                .AbstractType();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            WaypointCollection = sequenceBuilder.ElementCollection<IWaypoint>(/*typeof(IWaypoint)*/).MinOccurs(2).Build/*<IWaypoint>*/();

            typeBuilder.Build();
        }

        public EdgeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IWaypoint> Waypoints => WaypointCollection.Get<IWaypoint>(this);
    }

}