using System;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class EventBasedGatewayImpl : GatewayImpl, IEventBasedGateway
    {

        protected internal static IAttribute/*<bool>*/ InstantiateAttribute;
        protected internal static IAttribute/*<EventBasedGatewayType>*/ EventGatewayTypeAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IEventBasedGateway>(/*typeof(IEventBasedGateway),*/ BpmnModelConstants.BpmnElementEventBasedGateway)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IGateway))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            InstantiateAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeInstantiate).DefaultValue(false).Build();

            EventGatewayTypeAttribute = typeBuilder.EnumAttribute<EventBasedGatewayType>(BpmnModelConstants.BpmnAttributeEventGatewayType/*, typeof(EventBasedGatewayType)*/).DefaultValue(EventBasedGatewayType.Exclusive).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IEventBasedGateway>
        {
            public virtual IEventBasedGateway NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new EventBasedGatewayImpl(instanceContext);
            }
        }

        public EventBasedGatewayImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new EventBasedGatewayBuilder Builder()
        {
            return new EventBasedGatewayBuilder((IBpmnModelInstance)modelInstance, this);
        }

        public virtual bool Instantiate
        {
            get
            {
                return InstantiateAttribute.GetValue<Boolean>(this);
            }
            set
            {
                InstantiateAttribute.SetValue(this, value);
            }
        }


        public virtual EventBasedGatewayType EventGatewayType
        {
            get
            {
                return EventGatewayTypeAttribute.GetValue<EventBasedGatewayType>(this);
            }
            set
            {
                EventGatewayTypeAttribute.SetValue(this, value);
            }
        }


        public override bool CamundaAsyncAfter
        {
            get
            {
                throw new System.NotSupportedException("'asyncAfter' is not supported for 'Event Based Gateway'");
            }
            set
            {
                throw new System.NotSupportedException("'asyncAfter' is not supported for 'Event Based Gateway'");
            }
        }


    }

}