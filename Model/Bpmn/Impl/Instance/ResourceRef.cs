

using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    
    public class ResourceRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ResourceRef>(/*typeof(ResourceRef),*/ BpmnModelConstants.BpmnElementResourceRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ResourceRef>
        {
            public virtual ResourceRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ResourceRef(instanceContext);
            }
        }

        public ResourceRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}