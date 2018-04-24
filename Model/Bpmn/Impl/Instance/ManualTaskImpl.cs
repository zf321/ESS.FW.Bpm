

using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ManualTaskImpl : TaskImpl, IManualTask
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IManualTask>(/*typeof(IManualTask),*/ BpmnModelConstants.BpmnElementManualTask)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(ITask))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IManualTask>
        {
            public virtual IManualTask NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ManualTaskImpl(instanceContext);
            }
        }

        public ManualTaskImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new ManualTaskBuilder Builder()
        {
            return new ManualTaskBuilder((IBpmnModelInstance)modelInstance, this);
        }
    }

}