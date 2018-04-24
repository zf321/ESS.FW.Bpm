

using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class IntermediateThrowEventImpl : ThrowEventImpl, INtermediateThrowEvent
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INtermediateThrowEvent>(/*typeof(INtermediateThrowEvent),*/ BpmnModelConstants.BpmnElementIntermediateThrowEvent)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IThrowEvent))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<INtermediateThrowEvent>
        {
            public virtual INtermediateThrowEvent NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new IntermediateThrowEventImpl(instanceContext);
            }
        }

        public IntermediateThrowEventImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new IntermediateThrowEventBuilder Builder()
        {
            return new IntermediateThrowEventBuilder((IBpmnModelInstance)modelInstance, this);
        }
    }

}