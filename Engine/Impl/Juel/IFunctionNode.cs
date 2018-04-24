 
namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	/// <summary>
	/// Function node interface.
	/// 
	/// 
	/// </summary>
	public interface IFunctionNode : INode
	{
		/// <summary>
		/// Get the full function name
		/// </summary>
		string Name {get;}

		/// <summary>
		/// Get the unique index of this identifier in the expression (e.g. preorder index)
		/// </summary>
		int Index {get;}

		/// <summary>
		/// Get the number of parameters for this function
		/// </summary>
		int ParamCount {get;}

		/// <returns> <code>true</code> if this node supports varargs. </returns>
		bool VarArgs {get;}
	}

}