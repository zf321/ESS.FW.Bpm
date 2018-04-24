using System;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaFieldImpl : BpmnModelElementInstanceImpl, ICamundaField
    {

        protected internal static IAttribute/*<string>*/ CamundaNameAttribute;
        protected internal static IAttribute/*<string>*/ CamundaExpressionAttribute;
        protected internal static IAttribute/*<string>*/ CamundaStringValueAttribute;
        protected internal static IChildElement/*<ICamundaExpression>*/ camundaExpressionChild;
        protected internal static IChildElement/*<ICamundaString>*/ CamundaStringChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaField>(/*typeof(ICamundaField),*/ BpmnModelConstants.CamundaElementField)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CamundaNameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeName).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaExpressionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeExpression).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaStringValueAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeStringValue).Namespace(BpmnModelConstants.CamundaNs).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            camundaExpressionChild = sequenceBuilder.Element<ICamundaExpression>(/*typeof(ICamundaExpression)*/).Build/*<ICamundaExpression>*/();

            CamundaStringChild = sequenceBuilder.Element<ICamundaString>(/*typeof(ICamundaString)*/).Build/*<ICamundaString>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaField>
        {
            public virtual ICamundaField NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaFieldImpl(instanceContext);
            }
        }

        public CamundaFieldImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string CamundaName
        {
            get { return CamundaNameAttribute.GetValue<String>(this); }
            set { CamundaNameAttribute.SetValue(this, value); }
        }


        public virtual string CamundaExpression
        {
            get { return CamundaExpressionAttribute.GetValue<String>(this); }
            set { CamundaExpressionAttribute.SetValue(this, value); }
        }


        public virtual string CamundaStringValue
        {
            get { return CamundaStringValueAttribute.GetValue<String>(this); }
            set { CamundaStringValueAttribute.SetValue(this, value); }
        }


        public virtual ICamundaString CamundaString
        {
            get => (ICamundaString)CamundaStringChild.GetChild(this);
            set => CamundaStringChild.SetChild(this, value);
        }


        public virtual ICamundaExpression CamundaExpressionChild
        {
            get => (ICamundaExpression)camundaExpressionChild.GetChild(this);
            set => camundaExpressionChild.SetChild(this, value);
        }

    }

}