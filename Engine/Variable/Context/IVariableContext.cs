using ESS.FW.Bpm.Engine.Variable.Type;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Value;



namespace ESS.FW.Bpm.Engine.Variable.Context
{
    /// <summary>
	/// A context for variables. Allows resolving variables.
	/// 
	/// An API may choose to accept a VariableContext instead of a map of concrete values
	/// in situations where passing all available variables would be expensive and
	/// lazy-loading is a desirable optimization.
	/// 
	/// 
	/// </summary>
	public interface IVariableContext
	{

	  /// <summary>
	  /// Resolve a value in this context.
	  /// </summary>
	  /// <param name="variableName"> the name of the variable to resolve. </param>
	  /// <returns> the value of the variable or null in case the variable does not exist. </returns>
	  ITypedValue Resolve(string variableName);

	  /// <summary>
	  /// Checks whether a variable with the given name is resolve through this context.
	  /// </summary>
	  /// <param name="variableName"> the name of the variable to check </param>
	  /// <returns> true if the variable is resolve. </returns>
	  bool ContainsVariable(string variableName);

	  /// <returns> a set of all variable names resolvable through this Context. </returns>
	  ICollection<string> KeySet();

	}

}