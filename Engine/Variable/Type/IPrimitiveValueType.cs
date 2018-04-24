using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Variable.Type
{


	/// <summary>
	/// </summary>
	public interface IPrimitiveValueType : IValueType
	{

	  System.Type NetType {get;}

	}

}