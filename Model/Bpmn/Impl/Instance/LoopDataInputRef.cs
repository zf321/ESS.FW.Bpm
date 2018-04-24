

using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class LoopDataInputRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<LoopDataInputRef>(/*typeof(LoopDataInputRef),*/ BpmnModelConstants.BpmnElementLoopDataInputRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<LoopDataInputRef>
        {
            public virtual LoopDataInputRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new LoopDataInputRef(instanceContext);
            }
        }

        public LoopDataInputRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}