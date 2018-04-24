

using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaConnectorIdImpl : BpmnModelElementInstanceImpl, ICamundaConnectorId
    {
        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaConnectorId>(/*typeof(ICamundaConnectorId),*/ BpmnModelConstants.CamundaElementConnectorId)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaConnectorId>
        {
            public virtual ICamundaConnectorId NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaConnectorIdImpl(instanceContext);
            }
        }

        public CamundaConnectorIdImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}