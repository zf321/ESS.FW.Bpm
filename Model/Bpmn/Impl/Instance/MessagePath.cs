

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class MessagePath : FormalExpressionImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<MessagePath>(/*typeof(MessagePath), */BpmnModelConstants.BpmnElementMessagePath)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IFormalExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<MessagePath>
        {
            public virtual MessagePath NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new MessagePath(instanceContext);
            }
        }

        public MessagePath(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}