using System.Collections.Generic;




namespace ESS.FW.Bpm.Engine.Variable.Type
{

	/// <summary>
	/// </summary>
	public interface IValueTypeResolver
	{

	  void AddType(IValueType type);

	  IValueType TypeForName(string typeName);

	  /// <summary>
	  /// Returns all (transitive) sub types of the provided type
	  /// given they are not abstract
	  /// 
	  /// @return
	  /// </summary>
	  ICollection<IValueType> GetSubTypes(IValueType type);
	}

}