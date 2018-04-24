

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{

    public class BindingImpl : DmnModelElementInstanceImpl, IBinding
	{

	  protected internal static IElementReference ParameterRef;//IElementReference<INformationItem, IParameterReference>
        protected internal static IChildElement/*<IExpression>*/ ExpressionChild;

	  public BindingImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual INformationItem Parameter
	  {
		  get => (INformationItem)ParameterRef.GetReferenceTargetElement(this);
	      set => ParameterRef.SetReferenceTargetElement(this, value);
	  }


	  public virtual IExpression Expression
	  {
		  get => (IExpression)ExpressionChild.GetChild(this);
	      set => ExpressionChild.SetChild(this, value);
	  }


	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBinding>(/*typeof(IBinding),*/ DmnModelConstants.DmnElementBinding).NamespaceUri(DmnModelConstants.Dmn11Ns).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

		ParameterRef = sequenceBuilder.Element<IParameterReference>(/*typeof(IParameterReference)*/).Required().UriElementReference<INformationItem>(/*typeof(INformationItem)*/).Build();

		ExpressionChild = sequenceBuilder.Element<IExpression>(/*typeof(IExpression)*/).Build/*<IExpression>*/();

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IBinding>
	  {
		  public ModelTypeInstanceProviderAnonymousInnerClass()
		  {
		  }

		  public virtual IBinding NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new BindingImpl(instanceContext);
		  }
	  }

	}

}