

using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class InMessageRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<InMessageRef>(/*typeof(InMessageRef),*/ BpmnModelConstants.BpmnElementInMessageRef)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<InMessageRef>
        {
            public virtual InMessageRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InMessageRef(instanceContext);
            }
        }

        public InMessageRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}