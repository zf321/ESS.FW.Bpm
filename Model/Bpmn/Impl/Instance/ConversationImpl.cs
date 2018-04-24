using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ConversationImpl : ConversationNodeImpl, IConversation
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IConversation>(/*typeof(IConversation),*/ BpmnModelConstants.BpmnElementConversation)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IConversationNode))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IConversation>
        {
            public virtual IConversation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ConversationImpl(instanceContext);
            }
        }

        public ConversationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}