using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class LoopDataOutputRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<LoopDataOutputRef>(/*typeof(LoopDataOutputRef),*/ BpmnModelConstants.BpmnElementLoopDataOutputRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<LoopDataOutputRef>
        {
            public virtual LoopDataOutputRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new LoopDataOutputRef(instanceContext);
            }
        }

        public LoopDataOutputRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}