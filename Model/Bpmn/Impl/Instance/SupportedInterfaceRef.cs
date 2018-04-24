

using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class SupportedInterfaceRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<SupportedInterfaceRef>(/*typeof(SupportedInterfaceRef),*/ BpmnModelConstants.BpmnElementSupportedInterfaceRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<SupportedInterfaceRef>
        {
            public virtual SupportedInterfaceRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new SupportedInterfaceRef(instanceContext);
            }
        }

        public SupportedInterfaceRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}