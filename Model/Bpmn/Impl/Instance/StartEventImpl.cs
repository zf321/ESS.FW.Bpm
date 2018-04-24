using System;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;


namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class StartEventImpl : CatchEventImpl, IStartEvent
    {

        protected internal static IAttribute/*<bool>*/ IsInterruptingAttribute;
        
        protected internal static IAttribute/*<bool>*/ CamundaAsyncAttribute;
        protected internal static IAttribute/*<string>*/ CamundaFormHandlerClassAttribute;
        protected internal static IAttribute/*<string>*/ CamundaFormKeyAttribute;
        protected internal static IAttribute/*<string>*/ CamundaInitiatorAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {

            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IStartEvent>(/*typeof(IStartEvent),*/ BpmnModelConstants.BpmnElementStartEvent)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(ICatchEvent))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            IsInterruptingAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeIsInterrupting).DefaultValue(true).Build();
            
            CamundaAsyncAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.CamundaAttributeAsync).Namespace(BpmnModelConstants.CamundaNs).DefaultValue(false).Build();

            CamundaFormHandlerClassAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeFormHandlerClass).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaFormKeyAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeFormKey).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaInitiatorAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeInitiator).Namespace(BpmnModelConstants.CamundaNs).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IStartEvent>
        {
            public virtual IStartEvent NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new StartEventImpl(instanceContext);
            }
        }

        public StartEventImpl(ModelTypeInstanceContext context) : base(context)
        {
        }


        public override TB Builder<TB, TE>()
        {
            return (TB)(IBpmnModelElementBuilder<TE>)new StartEventBuilder((IBpmnModelInstance)this.modelInstance, this);
        }

        public virtual bool Interrupting
        {
            get { return IsInterruptingAttribute.GetValue<Boolean>(this); }
            set { IsInterruptingAttribute.SetValue(this, value); }
        }


        /// <summary>
        /// camunda extensions </summary>

        /// @deprecated use isCamundaAsyncBefore() instead. 
        [Obsolete("use isCamundaAsyncBefore() instead.")]
        public virtual bool CamundaAsync
        {
            get { return CamundaAsyncAttribute.GetValue<Boolean>(this); }
            set { CamundaAsyncAttribute.SetValue(this, value); }
        }


        public virtual string CamundaFormHandlerClass
        {
            get { return CamundaFormHandlerClassAttribute.GetValue<String>(this); }
            set { CamundaFormHandlerClassAttribute.SetValue(this, value); }
        }


        public virtual string CamundaFormKey
        {
            get { return CamundaFormKeyAttribute.GetValue<String>(this); }
            set { CamundaFormKeyAttribute.SetValue(this, value); }
        }


        public virtual string CamundaInitiator
        {
            get { return CamundaInitiatorAttribute.GetValue<String>(this); }
            set { CamundaInitiatorAttribute.SetValue(this, value); }
        }
    }
}