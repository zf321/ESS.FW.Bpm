
using ESS.FW.Bpm.Engine.Variable.Type;
using System;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Util
{
    /// <summary>
	/// </summary>
	public interface ITypedValueUpdateListener
	{

	  /// <summary>
	  /// Called when an implicit update to a typed value is detected
	  /// </summary>
	  /// <param name="updatedValue"> </param>
	  void OnImplicitValueUpdate(ITypedValue updatedValue);
	}

}