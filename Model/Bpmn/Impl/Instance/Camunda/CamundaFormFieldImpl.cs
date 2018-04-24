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
    public class CamundaFormFieldImpl : BpmnModelElementInstanceImpl, ICamundaFormField
    {

        protected internal static IAttribute/*<string>*/ CamundaIdAttribute;
        protected internal static IAttribute/*<string>*/ CamundaLabelAttribute;
        protected internal static IAttribute/*<string>*/ CamundaTypeAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDatePatternAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDefaultValueAttribute;
        protected internal static IChildElement/*<ICamundaProperties>*/ CamundaPropertiesChild;
        protected internal static IChildElement/*<ICamundaValidation>*/ CamundaValidationChild;
        protected internal static IChildElementCollection/*<ICamundaValue>*/ CamundaValueCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaFormField>(/*typeof(ICamundaFormField),*/ BpmnModelConstants.CamundaElementFormField)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CamundaIdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeId).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaLabelAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeLabel).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaTypeAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeType).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaDatePatternAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeDatePattern).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaDefaultValueAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeDefaultValue).Namespace(BpmnModelConstants.CamundaNs).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            CamundaPropertiesChild = sequenceBuilder.Element<ICamundaProperties>(/*typeof(ICamundaProperties)*/).Build/*<ICamundaProperties>*/();

            CamundaValidationChild = sequenceBuilder.Element<ICamundaValidation>(/*typeof(ICamundaValidation)*/).Build/*<ICamundaValidation>*/();

            CamundaValueCollection = sequenceBuilder.ElementCollection<ICamundaValue>(/*typeof(ICamundaValue)*/).Build/*<ICamundaValue>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaFormField>
        {
            public virtual ICamundaFormField NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaFormFieldImpl(instanceContext);
            }
        }

        public CamundaFormFieldImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string CamundaId
        {
            get => CamundaIdAttribute.GetValue<String>(this);
            set => CamundaIdAttribute.SetValue(this, value);
        }


        public virtual string CamundaLabel
        {
            get => CamundaLabelAttribute.GetValue<String>(this);
            set => CamundaLabelAttribute.SetValue(this, value);
        }


        public virtual string CamundaType
        {
            get => CamundaTypeAttribute.GetValue<String>(this);
            set => CamundaTypeAttribute.SetValue(this, value);
        }


        public virtual string CamundaDatePattern
        {
            get => CamundaDatePatternAttribute.GetValue<String>(this);
            set => CamundaDatePatternAttribute.SetValue(this, value);
        }


        public virtual string CamundaDefaultValue
        {
            get => CamundaDefaultValueAttribute.GetValue<String>(this);
            set => CamundaDefaultValueAttribute.SetValue(this, value);
        }


        public virtual ICamundaProperties CamundaProperties
        {
            get => (ICamundaProperties)CamundaPropertiesChild.GetChild(this);
            set => CamundaPropertiesChild.SetChild(this, value);
        }


        public virtual ICamundaValidation CamundaValidation
        {
            get => (ICamundaValidation)CamundaValidationChild.GetChild(this);
            set => CamundaValidationChild.SetChild(this, value);
        }


        public virtual ICollection<ICamundaValue> CamundaValues => CamundaValueCollection.Get<ICamundaValue>(this);
    }

}