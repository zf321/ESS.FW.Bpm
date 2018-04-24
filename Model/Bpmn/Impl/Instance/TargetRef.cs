using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class TargetRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<TargetRef>(/*typeof(TargetRef),*/ BpmnModelConstants.BpmnElementTargetRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<TargetRef>
        {
            public virtual TargetRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new TargetRef(instanceContext);
            }
        }

        public TargetRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}