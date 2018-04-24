using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class EndPointRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<EndPointRef>(/*typeof(EndPointRef),*/ BpmnModelConstants.BpmnElementEndPointRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<EndPointRef>
        {
            public virtual EndPointRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new EndPointRef(instanceContext);
            }
        }

        public EndPointRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}