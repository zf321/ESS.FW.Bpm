using System;
using ESS.FW.Bpm.Model.Xml.type.attribute;


namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class EnumAttributeBuilder<T> : AttributeBuilderImpl<T>  where T: struct
	{

	  public EnumAttributeBuilder(string attributeName, ModelElementTypeImpl modelType) 
            : base(attributeName, modelType, new EnumAttribute<T>(modelType))
	  {
	  }

	  public override IAttributeBuilder<T> Namespace(string namespaceUri)
	  {
		return (EnumAttributeBuilder<T>) base.Namespace(namespaceUri);
	  }

	  public override IAttributeBuilder<T> DefaultValue(T defaultValue)
	  {
		return (EnumAttributeBuilder<T>) base.DefaultValue(defaultValue);
	  }

	  public override IAttributeBuilder<T> Required()
	  {
		return (EnumAttributeBuilder<T>) base.Required();
	  }

	  public override IAttributeBuilder<T> IdAttribute()
	  {
		return (EnumAttributeBuilder<T>) base.IdAttribute();
	  }
	}

}