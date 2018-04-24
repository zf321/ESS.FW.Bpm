using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ScriptImpl : BpmnModelElementInstanceImpl, IScript
    {
        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IScript>(/*typeof(IScript),*/ BpmnModelConstants.BpmnElementScript)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IScript>
        {
            public virtual IScript NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ScriptImpl(instanceContext);
            }
        }

        public ScriptImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}