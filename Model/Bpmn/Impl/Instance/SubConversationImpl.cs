using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class SubConversationImpl : ConversationNodeImpl, ISubConversation
    {

        protected internal static IChildElementCollection/*<IConversationNode>*/ ConversationNodeCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ISubConversation>(/*typeof(ISubConversation),*/ BpmnModelConstants.BpmnElementSubConversation)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IConversationNode))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ConversationNodeCollection = sequenceBuilder.ElementCollection<IConversationNode>(/*typeof(IConversationNode)*/).Build/*<IConversationNode>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ISubConversation>
        {
            public virtual ISubConversation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new SubConversationImpl(instanceContext);
            }
        }

        public SubConversationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IConversationNode> ConversationNodes => ConversationNodeCollection.Get<IConversationNode>(this);
    }
}