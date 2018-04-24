

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class CorrelationPropertyRetrievalExpressionImpl : BaseElementImpl, ICorrelationPropertyRetrievalExpression
    {

        protected internal static IAttributeReference MessageRefAttribute;//IAttributeReference<IMessage>
        protected internal static IChildElement/*<MessagePath>*/ MessagePathChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICorrelationPropertyRetrievalExpression>(/*typeof(ICorrelationPropertyRetrievalExpression),*/ BpmnModelConstants.BpmnElementCorrelationPropertyRetrievalExpression)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            MessageRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeMessageRef)
                .Required()
                .QNameAttributeReference<IMessage>(/*typeof(IMessage)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            MessagePathChild = sequenceBuilder.Element<MessagePath>(/*typeof(MessagePath)*/)
                .Required()
                .Build/*<MessagePath>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICorrelationPropertyRetrievalExpression>
        {
            public virtual ICorrelationPropertyRetrievalExpression NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CorrelationPropertyRetrievalExpressionImpl(instanceContext);
            }
        }

        public CorrelationPropertyRetrievalExpressionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IMessage Message
        {
            get => MessageRefAttribute.GetReferenceTargetElement<IMessage>(this);
            set => MessageRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual MessagePath MessagePath
        {
            get => (MessagePath)MessagePathChild.GetChild(this);
            set => MessagePathChild.SetChild(this, value);
        }

    }

}