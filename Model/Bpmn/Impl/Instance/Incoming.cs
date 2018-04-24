using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class Incoming : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<Incoming>(/*typeof(Incoming),*/ BpmnModelConstants.BpmnElementIncoming)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<Incoming>
        {
            public virtual Incoming NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new Incoming(instanceContext);
            }
        }

        public Incoming(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}