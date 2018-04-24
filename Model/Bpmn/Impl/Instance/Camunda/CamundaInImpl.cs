using System;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaInImpl : BpmnModelElementInstanceImpl, ICamundaIn
    {

        protected internal static IAttribute/*<string>*/ CamundaSourceAttribute;
        protected internal static IAttribute/*<string>*/ CamundaSourceExpressionAttribute;
        protected internal static IAttribute/*<string>*/ CamundaVariablesAttribute;
        protected internal static IAttribute/*<string>*/ CamundaTargetAttribute;
        protected internal static IAttribute/*<string>*/ CamundaBusinessKeyAttribute;
        protected internal static IAttribute/*<bool>*/ CamundaLocalAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaIn>(/*typeof(ICamundaIn),*/ BpmnModelConstants.CamundaElementIn)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CamundaSourceAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeSource).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaSourceExpressionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeSourceExpression).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaVariablesAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeVariables).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaTargetAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeTarget).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaBusinessKeyAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeBusinessKey).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaLocalAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.CamundaAttributeLocal).Namespace(BpmnModelConstants.CamundaNs).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaIn>
        {
            public virtual ICamundaIn NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaInImpl(instanceContext);
            }
        }

        public CamundaInImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string CamundaSource
        {
            get { return CamundaSourceAttribute.GetValue<String>(this); }
            set { CamundaSourceAttribute.SetValue(this, value); }
        }


        public virtual string CamundaSourceExpression
        {
            get { return CamundaSourceExpressionAttribute.GetValue<String>(this); }
            set { CamundaSourceExpressionAttribute.SetValue(this, value); }
        }


        public virtual string CamundaVariables
        {
            get { return CamundaVariablesAttribute.GetValue<String>(this); }
            set { CamundaVariablesAttribute.SetValue(this, value); }
        }


        public virtual string CamundaTarget
        {
            get { return CamundaTargetAttribute.GetValue<String>(this); }
            set { CamundaTargetAttribute.SetValue(this, value); }
        }


        public virtual string CamundaBusinessKey
        {
            get { return CamundaBusinessKeyAttribute.GetValue<String>(this); }
            set { CamundaBusinessKeyAttribute.SetValue(this, value); }
        }


        public virtual bool CamundaLocal
        {
            get { return CamundaLocalAttribute.GetValue<Boolean>(this); }
            set { CamundaLocalAttribute.SetValue(this, value); }
        }


    }

}