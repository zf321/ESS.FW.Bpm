using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using System;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaConstraintImpl : BpmnModelElementInstanceImpl, ICamundaConstraint
    {

        protected internal static IAttribute/*<string>*/ CamundaNameAttribute;
        protected internal static IAttribute/*<string>*/ CamundaConfigAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaConstraint>(/*typeof(ICamundaConstraint),*/ BpmnModelConstants.CamundaElementConstraint)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CamundaNameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeName).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaConfigAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeConfig).Namespace(BpmnModelConstants.CamundaNs).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaConstraint>
        {
            public virtual ICamundaConstraint NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaConstraintImpl(instanceContext);
            }
        }

        public CamundaConstraintImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string CamundaName
        {
            get { return CamundaNameAttribute.GetValue<String>(this); }
            set { CamundaNameAttribute.SetValue(this, value); }
        }


        public virtual string CamundaConfig
        {
            get { return CamundaConfigAttribute.GetValue<String>(this); }
            set { CamundaConfigAttribute.SetValue(this, value); }
        }
    }
}