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
    public class CamundaFormPropertyImpl : BpmnModelElementInstanceImpl, ICamundaFormProperty
    {

        protected internal static IAttribute/*<string>*/ CamundaIdAttribute;
        protected internal static IAttribute/*<string>*/ CamundaNameAttribute;
        protected internal static IAttribute/*<string>*/ CamundaTypeAttribute;
        protected internal static IAttribute/*<bool>*/ CamundaRequiredAttribute;
        protected internal static IAttribute/*<bool>*/ CamundaReadableAttribute;
        protected internal static IAttribute/*<bool>*/ CamundaWriteableAttribute;
        protected internal static IAttribute/*<string>*/ CamundaVariableAttribute;
        protected internal static IAttribute/*<string>*/ CamundaExpressionAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDatePatternAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDefaultAttribute;
        protected internal static IChildElementCollection/*<ICamundaValue>*/ CamundaValueCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaFormProperty>(/*typeof(ICamundaFormProperty),*/ BpmnModelConstants.CamundaElementFormProperty)
                    .NamespaceUri(BpmnModelConstants.CamundaNs)
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CamundaIdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeId).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaNameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeName).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaTypeAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeType).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaRequiredAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.CamundaAttributeRequired).Namespace(BpmnModelConstants.CamundaNs).DefaultValue(false).Build();

            CamundaReadableAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.CamundaAttributeReadable).Namespace(BpmnModelConstants.CamundaNs).DefaultValue(true).Build();

            CamundaWriteableAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.CamundaAttributeWriteable).Namespace(BpmnModelConstants.CamundaNs).DefaultValue(true).Build();

            CamundaVariableAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeVariable).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaExpressionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeExpression).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaDatePatternAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeDatePattern).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaDefaultAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeDefault).Namespace(BpmnModelConstants.CamundaNs).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            CamundaValueCollection = sequenceBuilder.ElementCollection<ICamundaValue>(/*typeof(ICamundaValue)*/).Build/*<ICamundaValue>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaFormProperty>
        {
            public virtual ICamundaFormProperty NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaFormPropertyImpl(instanceContext);
            }
        }

        public CamundaFormPropertyImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string CamundaId
        {
            get => CamundaIdAttribute.GetValue<String>(this);
            set => CamundaIdAttribute.SetValue(this, value);
        }


        public virtual string CamundaName
        {
            get => CamundaNameAttribute.GetValue<String>(this);
            set => CamundaNameAttribute.SetValue(this, value);
        }


        public virtual string CamundaType
        {
            get => CamundaTypeAttribute.GetValue<String>(this);
            set => CamundaTypeAttribute.SetValue(this, value);
        }


        public virtual bool CamundaRequired
        {
            get => CamundaRequiredAttribute.GetValue<Boolean>(this);
            set => CamundaRequiredAttribute.SetValue(this, value);
        }


        public virtual bool CamundaReadable
        {
            get => CamundaReadableAttribute.GetValue<Boolean>(this);
            set => CamundaReadableAttribute.SetValue(this, value);
        }


        public virtual bool CamundaWriteable
        {
            get => CamundaWriteableAttribute.GetValue<Boolean>(this);
            set => CamundaWriteableAttribute.SetValue(this, value);
        }


        public virtual string CamundaVariable
        {
            get => CamundaVariableAttribute.GetValue<String>(this);
            set => CamundaVariableAttribute.SetValue(this, value);
        }


        public virtual string CamundaExpression
        {
            get => CamundaExpressionAttribute.GetValue<String>(this);
            set => CamundaExpressionAttribute.SetValue(this, value);
        }


        public virtual string CamundaDatePattern
        {
            get => CamundaDatePatternAttribute.GetValue<String>(this);
            set => CamundaDatePatternAttribute.SetValue(this, value);
        }


        public virtual string CamundaDefault
        {
            get => CamundaDefaultAttribute.GetValue<String>(this);
            set => CamundaDefaultAttribute.SetValue(this, value);
        }


        public virtual ICollection<ICamundaValue> CamundaValues => CamundaValueCollection.Get<ICamundaValue>(this);
    }

}