
using System;

namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{
    
	public class BooleanAttributeBuilder : AttributeBuilderImpl<Boolean>
	{

	  public BooleanAttributeBuilder(string attributeName, ModelElementTypeImpl modelType) 
            : base(attributeName, modelType, new BooleanAttribute(modelType))
	  {
	  }

	}

}