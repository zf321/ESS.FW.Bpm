using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{    
	public class CategoryValueRef : BpmnModelElementInstanceImpl
	{

	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<CategoryValueRef>(/*typeof(CategoryValueRef),*/ BpmnModelConstants.BpmnElementCategoryValueRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<CategoryValueRef>
	  {
		  public virtual CategoryValueRef NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CategoryValueRef(instanceContext);
		  }
	  }

	  public CategoryValueRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }
	}

}