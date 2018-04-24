

using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class OptionalInputRefs : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<OptionalInputRefs>(/*typeof(OptionalInputRefs),*/ BpmnModelConstants.BpmnElementOptionalInputRefs)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<OptionalInputRefs>
        {
            public virtual OptionalInputRefs NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new OptionalInputRefs(instanceContext);
            }
        }

        public OptionalInputRefs(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}