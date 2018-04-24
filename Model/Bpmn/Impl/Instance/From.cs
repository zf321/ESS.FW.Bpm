using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class From : ExpressionImpl
	{

	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<From>(/*typeof(From),*/ BpmnModelConstants.BpmnElementFrom)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IExpression))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<From>
	  {
		  public virtual From NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new From(instanceContext);
		  }
	  }

	  public From(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }
	}

}