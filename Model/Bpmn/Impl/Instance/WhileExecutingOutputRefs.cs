using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class WhileExecutingOutputRefs : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<WhileExecutingOutputRefs>(/*typeof(WhileExecutingOutputRefs),*/ BpmnModelConstants.BpmnElementWhileExecutingOutputRefs)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<WhileExecutingOutputRefs>
        {
            public virtual WhileExecutingOutputRefs NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new WhileExecutingOutputRefs(instanceContext);
            }
        }

        public WhileExecutingOutputRefs(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}