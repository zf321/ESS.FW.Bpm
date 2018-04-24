

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class CancelEventDefinitionImpl : EventDefinitionImpl, ICancelEventDefinition
	{

	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICancelEventDefinition>(/*typeof(ICancelEventDefinition),*/ BpmnModelConstants.BpmnElementCancelEventDefinition)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IEventDefinition))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICancelEventDefinition>
	  {
		  public virtual ICancelEventDefinition NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CancelEventDefinitionImpl(instanceContext);
		  }
	  }

	  public CancelEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	}

}