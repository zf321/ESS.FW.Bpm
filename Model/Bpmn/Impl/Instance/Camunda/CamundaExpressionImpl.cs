using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaExpressionImpl : BpmnModelElementInstanceImpl, ICamundaExpression
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaExpression>(/*typeof(ICamundaExpression),*/ BpmnModelConstants.CamundaElementExpression)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaExpression>
        {
            public virtual ICamundaExpression NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaExpressionImpl(instanceContext);
            }
        }

        public CamundaExpressionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}