using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class InputSetRefs : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<InputSetRefs>(/*typeof(InputSetRefs),*/ BpmnModelConstants.BpmnElementInputSetRefs)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<InputSetRefs>
        {
            public virtual InputSetRefs NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InputSetRefs(instanceContext);
            }
        }

        public InputSetRefs(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}