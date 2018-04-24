using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Variable.Value;


namespace ESS.FW.Bpm.Engine.Variable
{
    /// <summary>
	/// A Map of variables.
	/// 
	/// 
	/// </summary>
	public interface IVariableMap :IDictionary<string,object>
	{

	  // fluent api for collecting variables ////////////////////////

	  IVariableMap PutValue(string name, object value);

	  IVariableMap PutValueTyped(string name, ITypedValue value);

	  // retrieving variables ///////////////////////////////////////
      
	  object GetValue(String name, System.Type type);
        
	  T GetValueTyped<T>(String name) where T: ITypedValue;

	  /// <summary>
	  /// Interprets the variable map as variable context
	  /// </summary>
	  /// <returns> A VariableContext which is capable of resolving all variables in the map </returns>
	  IVariableContext AsVariableContext();

	    void PutAll(IDictionary<string, ITypedValue> m);

	}

}