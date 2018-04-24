using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class Outgoing : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<Outgoing>(/*typeof(Outgoing),*/ BpmnModelConstants.BpmnElementOutgoing)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<Outgoing>
        {
            public virtual Outgoing NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new Outgoing(instanceContext);
            }
        }

        public Outgoing(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}