

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class PerformerImpl : ResourceRoleImpl, IPerformer
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IPerformer>(/*typeof(IPerformer),*/ BpmnModelConstants.BpmnElementPerformer)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IResourceRole))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IPerformer>
        {
            public virtual IPerformer NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new PerformerImpl(instanceContext);
            }
        }

        public PerformerImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}