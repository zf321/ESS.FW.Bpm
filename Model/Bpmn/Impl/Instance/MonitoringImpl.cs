using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class MonitoringImpl : BaseElementImpl, IMonitoring
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {

            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IMonitoring>(/*typeof(IMonitoring),*/ BpmnModelConstants.BpmnElementMonitoring)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IMonitoring>
        {
            public virtual IMonitoring NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new MonitoringImpl(instanceContext);
            }
        }

        public MonitoringImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}