using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.di
{
    
    public class ExtensionImpl : BpmnModelElementInstanceImpl, IExtension
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IExtension>(/*typeof(IExtension),*/ BpmnModelConstants.DiElementExtension)
                .NamespaceUri(BpmnModelConstants.DiNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IExtension>
        {
            public virtual IExtension NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ExtensionImpl(instanceContext);
            }
        }

        public ExtensionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}