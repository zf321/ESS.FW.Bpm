 
namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	/// <summary>
	/// Tree cache interface.
	/// A tree cache holds expression trees by expression strings. A tree cache implementation
	/// must be thread-safe.
	/// 
	/// 
	/// </summary>
	public interface TreeCache
	{
		/// <summary>
		/// Lookup tree
		/// </summary>
		Tree Get(string expression);

		/// <summary>
		/// Cache tree
		/// </summary>
		void Put(string expression, Tree tree);
	}

}