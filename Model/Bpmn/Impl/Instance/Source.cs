using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class Source : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<Source>(/*typeof(Source),*/ BpmnModelConstants.BpmnElementSource)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<Source>
        {
            public virtual Source NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new Source(instanceContext);
            }
        }

        public Source(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}