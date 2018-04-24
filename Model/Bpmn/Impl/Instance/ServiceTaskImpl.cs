using System;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    
    public class ServiceTaskImpl : TaskImpl, IServiceTask
    {

        protected internal static IAttribute/*<string>*/ ImplementationAttribute;
        protected internal static IAttributeReference OperationRefAttribute;//IAttributeReference<IOperation>

        protected internal static IAttribute/*<string>*/ CamundaClassAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDelegateExpressionAttribute;
        protected internal static IAttribute/*<string>*/ CamundaExpressionAttribute;
        protected internal static IAttribute/*<string>*/ CamundaResultVariableAttribute;
        protected internal static IAttribute/*<string>*/ CamundaTopicAttribute;
        protected internal static IAttribute/*<string>*/ CamundaTypeAttribute;
        protected internal static IAttribute/*<string>*/ CamundaTaskPriorityAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IServiceTask>(/*typeof(IServiceTask),*/ BpmnModelConstants.BpmnElementServiceTask)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(ITask))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ImplementationAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeImplementation).DefaultValue("##WebService").Build();

            OperationRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeOperationRef)
                .QNameAttributeReference<IOperation>(/*typeof(IOperation)*/)
                .Build();            

            CamundaClassAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeClass)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaDelegateExpressionAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeDelegateExpression)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaExpressionAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeExpression)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaResultVariableAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeResultVariable)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaTopicAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeTopic)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaTypeAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeType)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaTaskPriorityAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeTaskPriority)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IServiceTask>
        {
            public virtual IServiceTask NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ServiceTaskImpl(instanceContext);
            }
        }

        public ServiceTaskImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new ServiceTaskBuilder Builder()
        {
            return new ServiceTaskBuilder((IBpmnModelInstance)modelInstance, this);
        }
        

        public virtual string Implementation
        {
            get => ImplementationAttribute.GetValue<String>(this);
            set => ImplementationAttribute.SetValue(this, value);
        }


        public virtual IOperation Operation
        {
            get => OperationRefAttribute.GetReferenceTargetElement<IOperation>(this);
            set => OperationRefAttribute.SetReferenceTargetElement(this, value);
        }
        

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


        public virtual string CamundaTaskPriority
        {
            get => CamundaTaskPriorityAttribute.GetValue<String>(this);
            set => CamundaTaskPriorityAttribute.SetValue(this, value);
        }
    }
}