using System;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class BusinessRuleTaskImpl : TaskImpl, IBusinessRuleTask
    {

        protected internal static IAttribute/*<string>*/ ImplementationAttribute;
        protected internal static IChildElementCollection/*<IRendering>*/ RenderingCollection;

        /// <summary>
        /// camunda extensions </summary>

        protected internal static IAttribute/*<string>*/ CamundaClassAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDelegateExpressionAttribute;
        protected internal static IAttribute/*<string>*/ CamundaExpressionAttribute;
        protected internal static IAttribute/*<string>*/ CamundaResultVariableAttribute;
        protected internal static IAttribute/*<string>*/ CamundaTopicAttribute;
        protected internal static IAttribute/*<string>*/ CamundaTypeAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDecisionRefAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDecisionRefBindingAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDecisionRefVersionAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDecisionRefTenantIdAttribute;
        protected internal static IAttribute/*<string>*/ CamundaMapDecisionResultAttribute;
        protected internal static IAttribute/*<string>*/ CamundaTaskPriorityAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBusinessRuleTask>(/*typeof(IBusinessRuleTask),*/ BpmnModelConstants.BpmnElementBusinessRuleTask)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(ITask))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ImplementationAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeImplementation)
                .DefaultValue("##unspecified")
                .Build();

            CamundaClassAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeClass)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaDelegateExpressionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeDelegateExpression)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaExpressionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeExpression)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaResultVariableAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeResultVariable)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaTopicAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeTopic)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaTypeAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeType)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaDecisionRefAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeDecisionRef)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaDecisionRefBindingAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeDecisionRefBinding)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaDecisionRefVersionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeDecisionRefVersion)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaDecisionRefTenantIdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeDecisionRefTenantId)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaMapDecisionResultAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeMapDecisionResult)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaTaskPriorityAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeTaskPriority)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            typeBuilder.Build();
        }

        public BusinessRuleTaskImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IBusinessRuleTask>
        {
            public virtual IBusinessRuleTask NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new BusinessRuleTaskImpl(instanceContext);
            }
        }


        public new BusinessRuleTaskBuilder Builder()
        {
            return new BusinessRuleTaskBuilder((IBpmnModelInstance)modelInstance, this);
        }

        public virtual string Implementation
        {
            get => ImplementationAttribute.GetValue<String>(this);
            set => ImplementationAttribute.SetValue(this, value);
        }


        /// <summary>
        /// camunda extensions </summary>

        public virtual string CamundaClass
        {
            get => CamundaClassAttribute.GetValue<String>(this);
            set => CamundaClassAttribute.SetValue(this, value);
        }


        public virtual string CamundaDelegateExpression
        {
            get => CamundaDelegateExpressionAttribute.GetValue<String>(this);
            set => CamundaDelegateExpressionAttribute.SetValue(this, value);
        }


        public virtual string CamundaExpression
        {
            get => CamundaExpressionAttribute.GetValue<String>(this);
            set => CamundaExpressionAttribute.SetValue(this, value);
        }


        public virtual string CamundaResultVariable
        {
            get => CamundaResultVariableAttribute.GetValue<String>(this);
            set => CamundaResultVariableAttribute.SetValue(this, value);
        }


        public virtual string CamundaTopic
        {
            get => CamundaTopicAttribute.GetValue<String>(this);
            set => CamundaTopicAttribute.SetValue(this, value);
        }


        public virtual string CamundaType
        {
            get => CamundaTypeAttribute.GetValue<String>(this);
            set => CamundaTypeAttribute.SetValue(this, value);
        }


        public virtual string CamundaDecisionRef
        {
            get => CamundaDecisionRefAttribute.GetValue<String>(this);
            set => CamundaDecisionRefAttribute.SetValue(this, value);
        }


        public virtual string CamundaDecisionRefBinding
        {
            get => CamundaDecisionRefBindingAttribute.GetValue<String>(this);
            set => CamundaDecisionRefBindingAttribute.SetValue(this, value);
        }


        public virtual string CamundaDecisionRefVersion
        {
            get => CamundaDecisionRefVersionAttribute.GetValue<String>(this);
            set => CamundaDecisionRefVersionAttribute.SetValue(this, value);
        }


        public virtual string CamundaMapDecisionResult
        {
            get => CamundaMapDecisionResultAttribute.GetValue<String>(this);
            set => CamundaMapDecisionResultAttribute.SetValue(this, value);
        }


        public virtual string CamundaDecisionRefTenantId
        {
            get => CamundaDecisionRefTenantIdAttribute.GetValue<String>(this);
            set => CamundaDecisionRefTenantIdAttribute.SetValue(this, value);
        }


        public virtual string CamundaTaskPriority
        {
            get => CamundaTaskPriorityAttribute.GetValue<String>(this);
            set => CamundaTaskPriorityAttribute.SetValue(this, value);
        }

    }

}