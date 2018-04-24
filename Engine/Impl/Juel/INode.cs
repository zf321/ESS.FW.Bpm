 
namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	/// <summary>
	/// Basic node interface.
	/// 
	/// 
	/// </summary>
	public interface INode
	{
		/// <summary>
		/// Get the node's number of children.
		/// </summary>
		int Cardinality {get;}

		/// <summary>
		/// Get i'th child
		/// </summary>
		INode GetChild(int i);
	}

}