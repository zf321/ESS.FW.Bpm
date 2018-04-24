

using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class EndEventImpl : ThrowEventImpl, IEndEvent
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IEndEvent>(/*typeof(IEndEvent),*/ BpmnModelConstants.BpmnElementEndEvent)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IThrowEvent))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IEndEvent>
        {

            public virtual IEndEvent NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new EndEventImpl(instanceContext);
            }
        }

        public EndEventImpl(ModelTypeInstanceContext context) : base(context)
        {
        }


        public new EndEventBuilder Builder()
        {
            return new EndEventBuilder((IBpmnModelInstance)modelInstance, this);
        }
    }

}