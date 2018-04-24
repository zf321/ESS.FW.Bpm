

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class HumanPerformerImpl : PerformerImpl, IHumanPerformer
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IHumanPerformer>(/*typeof(IHumanPerformer),*/ BpmnModelConstants.BpmnElementHumanPerformer)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IPerformer))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IHumanPerformer>
        {
            public virtual IHumanPerformer NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new HumanPerformerImpl(instanceContext);
            }
        }

        public HumanPerformerImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

    }
}