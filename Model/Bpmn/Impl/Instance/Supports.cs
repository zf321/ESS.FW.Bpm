

using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class Supports : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<Supports>(/*typeof(Supports),*/ BpmnModelConstants.BpmnElementSupports)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<Supports>
        {
            public virtual Supports NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new Supports(instanceContext);
            }
        }

        public Supports(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}