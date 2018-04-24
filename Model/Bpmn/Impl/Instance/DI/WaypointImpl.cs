

using ESS.FW.Bpm.Model.Bpmn.instance.dc;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using PointImpl = ESS.FW.Bpm.Model.Bpmn.impl.instance.dc.PointImpl;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.di
{
    
    public class WaypointImpl : PointImpl, IWaypoint
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IWaypoint>(/*typeof(IWaypoint),*/ BpmnModelConstants.DiElementWaypoint)
                .NamespaceUri(BpmnModelConstants.DiNs)
                .ExtendsType(typeof(IPoint))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IWaypoint>
        {
            public virtual IWaypoint NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new WaypointImpl(instanceContext);
            }
        }

        public WaypointImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        //public double X { get; set; }
        //public double Y { get; set; }
    }
}