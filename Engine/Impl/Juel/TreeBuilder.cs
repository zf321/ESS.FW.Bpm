 
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    
	/// <summary>
	/// Tree builder interface.
	/// A tree builder can be used to create arbitrary many trees. Furthermore, a tree builder
	/// implementation must be thread-safe.
	/// 
	/// 
	/// </summary>
	public interface TreeBuilder
	{
		/// <summary>
		/// Parse the given expression and create an abstract syntax tree for it. </summary>
		/// <param name="expression"> expression string </param>
		/// <returns> tree corresponding to the given expression </returns>
		/// <exception cref="ELException"> on parse error </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Tree build(String expression) throws TreeBuilderException;
		Tree Build(string expression);
	}

}