

using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaFailedJobRetryTimeCycleImpl : BpmnModelElementInstanceImpl, ICamundaFailedJobRetryTimeCycle
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaFailedJobRetryTimeCycle>(/*typeof(ICamundaFailedJobRetryTimeCycle),*/ BpmnModelConstants.CamundaElementFailedJobRetryTimeCycle)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaFailedJobRetryTimeCycle>
        {
            public virtual ICamundaFailedJobRetryTimeCycle NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaFailedJobRetryTimeCycleImpl(instanceContext);
            }
        }

        public CamundaFailedJobRetryTimeCycleImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}