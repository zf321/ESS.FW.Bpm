using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ParticipantRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ParticipantRef>(/*typeof(ParticipantRef),*/ BpmnModelConstants.BpmnElementParticipantRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ParticipantRef>
        {
            public virtual ParticipantRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ParticipantRef(instanceContext);
            }
        }

        public ParticipantRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}