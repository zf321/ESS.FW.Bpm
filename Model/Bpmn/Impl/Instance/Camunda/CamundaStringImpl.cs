using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaStringImpl : BpmnModelElementInstanceImpl, ICamundaString
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaString>(/*typeof(ICamundaString),*/ BpmnModelConstants.CamundaElementString)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaString>
        {
            public virtual ICamundaString NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaStringImpl(instanceContext);
            }
        }

        public CamundaStringImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}