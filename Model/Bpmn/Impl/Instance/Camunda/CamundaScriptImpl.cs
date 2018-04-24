using System;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaScriptImpl : BpmnModelElementInstanceImpl, ICamundaScript
    {

        protected internal static IAttribute/*<string>*/ CamundaScriptFormatAttribute;
        protected internal static IAttribute/*<string>*/ CamundaResourceAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaScript>(/*typeof(ICamundaScript),*/ BpmnModelConstants.CamundaElementScript)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CamundaScriptFormatAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeScriptFormat).Required().Build();

            CamundaResourceAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeResource).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaScript>
        {
            public virtual ICamundaScript NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaScriptImpl(instanceContext);
            }
        }

        public CamundaScriptImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string CamundaScriptFormat
        {
            get { return CamundaScriptFormatAttribute.GetValue<String>(this); }
            set { CamundaScriptFormatAttribute.SetValue(this, value); }
        }


        public virtual string CamundaResource
        {
            get { return CamundaResourceAttribute.GetValue<String>(this); }
            set { CamundaResourceAttribute.SetValue(this, value); }
        }

    }

}