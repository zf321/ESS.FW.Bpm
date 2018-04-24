using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ConditionExpressionImpl : FormalExpressionImpl, IConditionExpression
    {

        protected internal static IAttribute/*<string>*/ TypeAttribute;
        protected internal static IAttribute/*<string>*/ CamundaResourceAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IConditionExpression>(/*typeof(IConditionExpression),*/ BpmnModelConstants.BpmnElementConditionExpression)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IFormalExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            TypeAttribute = typeBuilder.StringAttribute(BpmnModelConstants.XsiAttributeType)
                .Namespace(BpmnModelConstants.XsiNs)
                .DefaultValue("tFormalExpression")
                .Build();

            CamundaResourceAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeResource)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IConditionExpression>
        {
            public virtual IConditionExpression NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ConditionExpressionImpl(instanceContext);
            }
        }

        public ConditionExpressionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Type
        {
            get
            {
                return TypeAttribute.GetValue<String>(this);
            }

            set
            {
                TypeAttribute.SetValue(this, value);
            }
        }


        public virtual string CamundaResource
        {
            get
            {
                return CamundaResourceAttribute.GetValue<String>(this);
            }

            set
            {
                CamundaResourceAttribute.SetValue(this, value);
            }
        }
    }
}