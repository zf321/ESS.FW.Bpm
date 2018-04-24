

using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class Target : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<Target>(/*typeof(Target),*/ BpmnModelConstants.BpmnElementTarget)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<Target>
        {
            public virtual Target NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new Target(instanceContext);
            }
        }

        public Target(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}