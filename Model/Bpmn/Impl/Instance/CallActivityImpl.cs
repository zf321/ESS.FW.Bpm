using System;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class CallActivityImpl : ActivityImpl, ICallActivity
    {

        protected internal static IAttribute/*<string>*/ CalledElementAttribute;

        protected internal static IAttribute/*<bool>*/ CamundaAsyncAttribute;
        protected internal static IAttribute/*<string>*/ CamundaCalledElementBindingAttribute;
        protected internal static IAttribute/*<string>*/ CamundaCalledElementVersionAttribute;
        protected internal static IAttribute/*<string>*/ CamundaCalledElementTenantIdAttribute;

        protected internal static IAttribute/*<string>*/ CamundaCaseRefAttribute;
        protected internal static IAttribute/*<string>*/ CamundaCaseBindingAttribute;
        protected internal static IAttribute/*<string>*/ CamundaCaseVersionAttribute;
        protected internal static IAttribute/*<string>*/ CamundaCaseTenantIdAttribute;
        protected internal static IAttribute/*<string>*/ CamundaVariableMappingClassAttribute;
        protected internal static IAttribute/*<string>*/ CamundaVariableMappingDelegateExpressionAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICallActivity>(/*typeof(ICallActivity),*/ BpmnModelConstants.BpmnElementCallActivity)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IActivity))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CalledElementAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeCalledElement).Build();

            CamundaAsyncAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.CamundaAttributeAsync).Namespace(BpmnModelConstants.CamundaNs).DefaultValue(false).Build();

            CamundaCalledElementBindingAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeCalledElementBinding).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaCalledElementVersionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeCalledElementVersion).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaCaseRefAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeCaseRef).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaCaseBindingAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeCaseBinding).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaCaseVersionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeCaseVersion).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaCalledElementTenantIdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeCalledElementTenantId).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaCaseTenantIdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeCaseTenantId).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaVariableMappingClassAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeVariableMappingClass).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaVariableMappingDelegateExpressionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeVariableMappingDelegateExpression).Namespace(BpmnModelConstants.CamundaNs).Build();


            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICallActivity>
        {
            public virtual ICallActivity NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CallActivityImpl(instanceContext);
            }
        }

        public CallActivityImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new CallActivityBuilder Builder()
        {
            return new CallActivityBuilder((IBpmnModelInstance)modelInstance, this);
        }

        public virtual string CalledElement
        {
            get
            {
                return CalledElementAttribute.GetValue<String>(this);
            }
            set
            {
                CalledElementAttribute.SetValue(this, value);
            }
        }


        /// @deprecated use isCamundaAsyncBefore() instead. 
        [Obsolete("use isCamundaAsyncBefore() instead.")]
        public virtual bool CamundaAsync
        {
            get
            {
                return CamundaAsyncAttribute.GetValue<Boolean>(this);
            }
            set
            {
                CamundaAsyncAttribute.SetValue(this, value);
            }
        }


        public virtual string CamundaCalledElementBinding
        {
            get
            {
                return CamundaCalledElementBindingAttribute.GetValue<String>(this);
            }
            set
            {
                CamundaCalledElementBindingAttribute.SetValue(this, value);
            }
        }


        public virtual string CamundaCalledElementVersion
        {
            get
            {
                return CamundaCalledElementVersionAttribute.GetValue<String>(this);
            }
            set
            {
                CamundaCalledElementVersionAttribute.SetValue(this, value);
            }
        }


        public virtual string CamundaCaseRef
        {
            get
            {
                return CamundaCaseRefAttribute.GetValue<String>(this);
            }
            set
            {
                CamundaCaseRefAttribute.SetValue(this, value);
            }
        }


        public virtual string CamundaCaseBinding
        {
            get
            {
                return CamundaCaseBindingAttribute.GetValue<String>(this);
            }
            set
            {
                CamundaCaseBindingAttribute.SetValue(this, value);
            }
        }


        public virtual string CamundaCaseVersion
        {
            get
            {
                return CamundaCaseVersionAttribute.GetValue<String>(this);
            }
            set
            {
                CamundaCaseVersionAttribute.SetValue(this, value);
            }
        }


        public virtual string CamundaCalledElementTenantId
        {
            get
            {
                return CamundaCalledElementTenantIdAttribute.GetValue<String>(this);
            }
            set
            {
                CamundaCalledElementTenantIdAttribute.SetValue(this, value);
            }
        }


        public virtual string CamundaCaseTenantId
        {
            get
            {
                return CamundaCaseTenantIdAttribute.GetValue<String>(this);
            }
            set
            {
                CamundaCaseTenantIdAttribute.SetValue(this, value);
            }
        }


        public virtual string CamundaVariableMappingClass
        {
            get
            {
                return CamundaVariableMappingClassAttribute.GetValue<String>(this);
            }
            set
            {
                CamundaVariableMappingClassAttribute.SetValue(this, value);
            }
        }


        public virtual string CamundaVariableMappingDelegateExpression
        {
            get
            {
                return CamundaVariableMappingDelegateExpressionAttribute.GetValue<String>(this);
            }
            set
            {
                CamundaVariableMappingDelegateExpressionAttribute.SetValue(this, value);
            }
        }

    }

}