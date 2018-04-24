using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class IntermediateCatchEventImpl : CatchEventImpl, INtermediateCatchEvent
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INtermediateCatchEvent>(/*typeof(INtermediateCatchEvent),*/ BpmnModelConstants.BpmnElementIntermediateCatchEvent)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(ICatchEvent))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<INtermediateCatchEvent>
        {
            public virtual INtermediateCatchEvent NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new IntermediateCatchEventImpl(instanceContext);
            }
        }

        public IntermediateCatchEventImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new IntermediateCatchEventBuilder Builder()
        {
            return new IntermediateCatchEventBuilder((IBpmnModelInstance)modelInstance, this);
        }
    }

}