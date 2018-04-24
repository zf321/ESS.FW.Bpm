using System;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaInputParameterImpl : CamundaGenericValueElementImpl, ICamundaInputParameter
    {

        protected internal static IAttribute/*<string>*/ CamundaNameAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaInputParameter>(/*typeof(ICamundaInputParameter),*/ BpmnModelConstants.CamundaElementInputParameter)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CamundaNameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeName).Namespace(BpmnModelConstants.CamundaNs).Required().Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaInputParameter>
        {
            public virtual ICamundaInputParameter NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaInputParameterImpl(instanceContext);
            }
        }

        public CamundaInputParameterImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string CamundaName
        {
            get { return CamundaNameAttribute.GetValue<String>(this); }
            set { CamundaNameAttribute.SetValue(this, value); }
        }


    }

}