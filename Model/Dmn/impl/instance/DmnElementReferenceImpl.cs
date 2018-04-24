

using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class DmnElementReferenceImpl : DmnModelElementInstanceImpl, IDmnElementReference
	{

	  protected internal static IAttribute/*<string>*/ HrefAttribute;

	  public DmnElementReferenceImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Href
	  {
		  get
		  {
			return HrefAttribute.GetValue<String>(this);
		  }
		  set
		  {
			HrefAttribute.SetValue(this, value);
		  }
	  }


	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDmnElementReference>(/*typeof(IDmnElementReference),*/ DmnModelConstants.DmnElementReference).NamespaceUri(DmnModelConstants.Dmn11Ns).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		HrefAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeHref).Required().Build();

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDmnElementReference>
	  {
		  public ModelTypeInstanceProviderAnonymousInnerClass()
		  {
		  }

		  public virtual IDmnElementReference NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new DmnElementReferenceImpl(instanceContext);
		  }
	  }

	}

}