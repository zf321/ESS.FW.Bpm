using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class OuterParticipantRef : BpmnModelElementInstanceImpl
	{

	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<OuterParticipantRef>(/*typeof(OuterParticipantRef),*/ BpmnModelConstants.BpmnElementOuterParticipantRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<OuterParticipantRef>
	  {
		  public virtual OuterParticipantRef NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new OuterParticipantRef(instanceContext);
		  }
	  }

	  public OuterParticipantRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }
	}

}