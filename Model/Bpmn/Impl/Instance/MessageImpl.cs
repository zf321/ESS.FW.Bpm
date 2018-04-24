using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class MessageImpl : RootElementImpl, IMessage
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttributeReference ItemRefAttribute;//IAttributeReference<IItemDefinition>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IMessage>(/*typeof(IMessage), */BpmnModelConstants.BpmnElementMessage)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IRootElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            ItemRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeItemRef)
                .QNameAttributeReference<IItemDefinition>(/*typeof(IItemDefinition)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IMessage>
        {
            public virtual IMessage NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new MessageImpl(instanceContext);
            }
        }

        public MessageImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual IItemDefinition Item
        {
            get => ItemRefAttribute.GetReferenceTargetElement<IItemDefinition>(this);
            set => ItemRefAttribute.SetReferenceTargetElement(this, value);
        }
    }
}