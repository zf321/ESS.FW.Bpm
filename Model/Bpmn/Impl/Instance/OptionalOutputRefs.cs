

using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class OptionalOutputRefs : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<OptionalOutputRefs>(/*typeof(OptionalOutputRefs),*/ BpmnModelConstants.BpmnElementOptionalOutputRefs)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<OptionalOutputRefs>
        {
            public virtual OptionalOutputRefs NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new OptionalOutputRefs(instanceContext);
            }
        }

        public OptionalOutputRefs(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}