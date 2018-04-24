using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ChildLaneSet : LaneSetImpl
	{

	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ChildLaneSet>(/*typeof(ChildLaneSet),*/ BpmnModelConstants.BpmnElementChildLaneSet)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(ILaneSet))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ChildLaneSet>
	  {
		  public virtual ChildLaneSet NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ChildLaneSet(instanceContext);
		  }
	  }

	  public ChildLaneSet(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }
	}

}