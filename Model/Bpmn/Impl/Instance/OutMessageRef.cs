

using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class OutMessageRef : BpmnModelElementInstanceImpl
    {
        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<OutMessageRef>(/*typeof(OutMessageRef),*/ BpmnModelConstants.BpmnElementOutMessageRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<OutMessageRef>
        {
            public virtual OutMessageRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new OutMessageRef(instanceContext);
            }
        }

        public OutMessageRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}