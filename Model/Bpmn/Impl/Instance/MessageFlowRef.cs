using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class MessageFlowRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<MessageFlowRef>(/*typeof(MessageFlowRef),*/ BpmnModelConstants.BpmnElementMessageFlowRef)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<MessageFlowRef>
        {
            public virtual MessageFlowRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new MessageFlowRef(instanceContext);
            }
        }

        public MessageFlowRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}