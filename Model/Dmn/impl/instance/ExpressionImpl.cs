

using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public abstract class ExpressionImpl : DmnElementImpl, IExpression
	{

	  protected internal static IAttribute/*<string>*/ TypeRefAttribute;

	  public ExpressionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string TypeRef
	  {
		  get
		  {
			return TypeRefAttribute.GetValue<String>(this);
		  }
		  set
		  {
			TypeRefAttribute.SetValue(this, value);
		  }
	  }


	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IExpression>(/*typeof(IExpression),*/ DmnModelConstants.DmnElementExpression).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IDmnElement)).AbstractType();

		TypeRefAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeTypeRef).Build();

		typeBuilder.Build();
	  }

	}

}