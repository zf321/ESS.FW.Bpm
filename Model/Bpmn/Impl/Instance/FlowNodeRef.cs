

using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class FlowNodeRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<FlowNodeRef>(/*typeof(FlowNodeRef),*/ BpmnModelConstants.BpmnElementFlowNodeRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<FlowNodeRef>
        {
            public virtual FlowNodeRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new FlowNodeRef(instanceContext);
            }
        }

        public FlowNodeRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}