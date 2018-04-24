using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    
    public class WhileExecutingInputRefs : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<WhileExecutingInputRefs>(/*typeof(WhileExecutingInputRefs),*/ BpmnModelConstants.BpmnElementWhileExecutingInputRefs)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<WhileExecutingInputRefs>
        {
            public virtual WhileExecutingInputRefs NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new WhileExecutingInputRefs(instanceContext);
            }
        }

        public WhileExecutingInputRefs(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}