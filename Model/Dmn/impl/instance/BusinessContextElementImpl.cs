

using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public abstract class BusinessContextElementImpl : NamedElementImpl, IBusinessContextElement
	{

	  protected internal static IAttribute/*<string>*/ UriAttribute;

	  public BusinessContextElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Uri
	  {
		  get
		  {
			return UriAttribute.GetValue<String>(this);
		  }
		  set
		  {
			UriAttribute.SetValue(this, value);
		  }
	  }


	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBusinessContextElement>(/*typeof(IBusinessContextElement),*/ DmnModelConstants.DmnElementBusinessContextElement).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(INamedElement)).AbstractType();

		UriAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeUri).Build();

		typeBuilder.Build();
	  }

	}

}