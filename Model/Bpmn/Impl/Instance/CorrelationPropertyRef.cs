using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class CorrelationPropertyRef : BpmnModelElementInstanceImpl
    {
        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<CorrelationPropertyRef>(/*typeof(CorrelationPropertyRef),*/ BpmnModelConstants.BpmnElementCorrelationPropertyRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<CorrelationPropertyRef>
        {
            public virtual CorrelationPropertyRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CorrelationPropertyRef(instanceContext);
            }
        }

        public CorrelationPropertyRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}