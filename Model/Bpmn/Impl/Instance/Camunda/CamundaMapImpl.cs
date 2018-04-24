using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    
    public class CamundaMapImpl : BpmnModelElementInstanceImpl, ICamundaMap
	{

	  protected internal static IChildElementCollection/*<ICamundaEntry>*/ CamundaEntryCollection;

	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaMap>(/*typeof(ICamundaMap),*/ BpmnModelConstants.CamundaElementMap)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

		CamundaEntryCollection = sequenceBuilder.ElementCollection<ICamundaEntry>(/*typeof(ICamundaEntry)*/).Build/*<ICamundaEntry>*/();

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaMap>
	  {
		  public virtual ICamundaMap NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaMapImpl(instanceContext);
		  }
	  }

	  public CamundaMapImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual ICollection<ICamundaEntry> CamundaEntries => CamundaEntryCollection.Get<ICamundaEntry>(this);
	}

}