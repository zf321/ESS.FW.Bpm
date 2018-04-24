using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaTaskListenerImpl : BpmnModelElementInstanceImpl, ICamundaTaskListener
    {

        protected internal static IAttribute/*<string>*/ CamundaEventAttribute;
        protected internal static IAttribute/*<string>*/ CamundaClassAttribute;
        protected internal static IAttribute/*<string>*/ CamundaExpressionAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDelegateExpressionAttribute;
        protected internal static IChildElementCollection/*<ICamundaField>*/ CamundaFieldCollection;
        protected internal static IChildElement/*<ICamundaScript>*/ CamundaScriptChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaTaskListener>(/*typeof(ICamundaTaskListener),*/ BpmnModelConstants.CamundaElementTaskListener)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CamundaEventAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeEvent).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaClassAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeClass).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaExpressionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeExpression).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaDelegateExpressionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeDelegateExpression).Namespace(BpmnModelConstants.CamundaNs).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            CamundaFieldCollection = sequenceBuilder.ElementCollection<ICamundaField>(/*typeof(ICamundaField)*/).Build/*<ICamundaField>*/();

            CamundaScriptChild = sequenceBuilder.Element<ICamundaScript>(/*typeof(ICamundaScript)*/).Build/*<ICamundaScript>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaTaskListener>
        {
            public virtual ICamundaTaskListener NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaTaskListenerImpl(instanceContext);
            }
        }

        public CamundaTaskListenerImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string CamundaEvent
        {
            get => CamundaEventAttribute.GetValue<String>(this);
            set => CamundaEventAttribute.SetValue(this, value);
        }


        public virtual string CamundaClass
        {
            get => CamundaClassAttribute.GetValue<String>(this);
            set => CamundaClassAttribute.SetValue(this, value);
        }


        public virtual string CamundaExpression
        {
            get => CamundaExpressionAttribute.GetValue<String>(this);
            set => CamundaExpressionAttribute.SetValue(this, value);
        }


        public virtual string CamundaDelegateExpression
        {
            get => CamundaDelegateExpressionAttribute.GetValue<String>(this);
            set => CamundaDelegateExpressionAttribute.SetValue(this, value);
        }


        public virtual ICollection<ICamundaField> CamundaFields => CamundaFieldCollection.Get<ICamundaField>(this);

        public virtual ICamundaScript CamundaScript
        {
            get => (ICamundaScript)CamundaScriptChild.GetChild(this);
            set => CamundaScriptChild.SetChild(this, value);
        }


    }

}