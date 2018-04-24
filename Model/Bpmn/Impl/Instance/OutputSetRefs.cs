using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class OutputSetRefs : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<OutputSetRefs>(/*typeof(OutputSetRefs),*/ BpmnModelConstants.BpmnElementOutputSetRefs)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<OutputSetRefs>
        {
            public virtual OutputSetRefs NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new OutputSetRefs(instanceContext);
            }
        }

        public OutputSetRefs(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}