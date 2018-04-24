////using System;


////
////
//// 
////
//// 
////
//// 
//// 
//// 
//// 
//// 
//// 

////namespace org.camunda.bpm.engine.impl.form.type
////{

////	using Variables = org.camunda.bpm.engine.Variable.Variables;
////	//using BooleanValue = BooleanValue;

////    /// <summary>
////	/// 
////	/// </summary>
////	public class BooleanFormType : SimpleFormFieldType
////	{

////	  public const string TYPE_NAME = "boolean";

////	  public override string Name
////	  {
////		  get
////		  {
////			return TYPE_NAME;
////		  }
////	  }

////	  public override ITypedValue convertValue(ITypedValue propertyValue)
////	  {
////		if (propertyValue is BooleanValue)
////		{
////		  return propertyValue;
////		}
////		else
////		{
////		  object value = propertyValue.Value;
////		  if (value == null)
////		  {
////			return Variables.booleanValue(null);
////		  }
////		  else if ((value is bool?) || (value is string))
////		  {
////			return Variables.booleanValue(new bool?(value.ToString()));
////		  }
////		  else
////		  {
////			throw new ProcessEngineException("Value '" + value + "' is not of type Boolean.");
////		  }
////		}
////	  }
////	  // deprecated /////////////////////////////////////////////////

////	  public override object convertFormValueToModelValue(object propertyValue)
////	  {
////		if (propertyValue == null || "".Equals(propertyValue))
////		{
////		  return null;
////		}
////		return Convert.ToBoolean(propertyValue.ToString());
////	  }

////	  public override string convertModelValueToFormValue(object modelValue)
////	  {

////		if (modelValue == null)
////		{
////		  return null;
////		}

////		if (modelValue.GetType().IsSubclassOf(typeof(Boolean)) || modelValue.GetType().IsSubclassOf(typeof(bool)))
////		{
////		  return modelValue.ToString();
////		}
//////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
////		throw new ProcessEngineException("Model value is not of type boolean, but of type " + modelValue.GetType().FullName);
////	  }

////	}

////}

