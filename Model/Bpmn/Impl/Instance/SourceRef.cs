using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    
    public class SourceRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<SourceRef>(/*typeof(SourceRef),*/ BpmnModelConstants.BpmnElementSourceRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<SourceRef>
        {
            public virtual SourceRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new SourceRef(instanceContext);
            }
        }

        public SourceRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}