using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class PotentialOwnerImpl : HumanPerformerImpl, IPotentialOwner
    {
        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IPotentialOwner>(/*typeof(IPotentialOwner),*/ BpmnModelConstants.BpmnElementPotentialOwner)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IHumanPerformer))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IPotentialOwner>
        {
            public virtual IPotentialOwner NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new PotentialOwnerImpl(instanceContext);
            }
        }

        public PotentialOwnerImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}