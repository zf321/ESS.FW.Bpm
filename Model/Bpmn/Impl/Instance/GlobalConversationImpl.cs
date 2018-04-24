

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class GlobalConversationImpl : CollaborationImpl, IGlobalConversation
    {
        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IGlobalConversation>(/*typeof(IGlobalConversation),*/ BpmnModelConstants.BpmnElementGlobalConversation)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(ICollaboration))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IGlobalConversation>
        {
            public virtual IGlobalConversation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new GlobalConversationImpl(instanceContext);
            }
        }

        public GlobalConversationImpl(ModelTypeInstanceContext context) : base(context)
        {
        }
    }

}