using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class InterfaceRef : BpmnModelElementInstanceImpl
    {
        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<InterfaceRef>(/*typeof(InterfaceRef), */BpmnModelConstants.BpmnElementInterfaceRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<InterfaceRef>
        {
            public virtual InterfaceRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InterfaceRef(instanceContext);
            }
        }

        public InterfaceRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}