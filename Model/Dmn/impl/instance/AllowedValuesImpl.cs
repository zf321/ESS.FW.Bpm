

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_NS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN_ELEMENT_ALLOWED_VALUE;

    using ModelBuilder = ModelBuilder;
    using ModelTypeInstanceContext = ModelTypeInstanceContext;

    public class AllowedValuesImpl : UnaryTestsImpl, IAllowedValues
	{

	  public AllowedValuesImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IAllowedValues>(/*typeof(IAllowedValues),*/ DmnModelConstants.DmnElementAllowedValue).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(ILiteralExpression)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IAllowedValues>
	  {
		  public ModelTypeInstanceProviderAnonymousInnerClass()
		  {
		  }

		  public virtual IAllowedValues NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new AllowedValuesImpl(instanceContext);
		  }
	  }
	}

}