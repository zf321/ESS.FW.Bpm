 
namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	/// <summary>
	/// Identifier node interface.
	/// 
	/// 
	/// </summary>
	public interface IIdentifierNode : INode
	{
		/// <summary>
		/// Get the identifier name
		/// </summary>
		string Name {get;}

		/// <summary>
		/// Get the unique index of this identifier in the expression (e.g. preorder index)
		/// </summary>
		int Index {get;}
	}

}