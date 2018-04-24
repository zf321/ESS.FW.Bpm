 
namespace ESS.FW.Bpm.Engine.Impl.Juel
{


	/// <summary>
	/// Tree store class.
	/// A tree store holds a <seealso cref="TreeBuilder"/> and a
	/// <seealso cref="TreeCache"/>, provided at construction time.
	/// The <code>get(String)</code> method is then used to serve expression trees.
	/// 
	/// 
	/// </summary>
	public class TreeStore
	{
		private readonly TreeCache _cache;
		private readonly TreeBuilder _builder;

		/// <summary>
		/// Constructor. </summary>
		/// <param name="builder"> the tree builder </param>
		/// <param name="cache"> the tree cache (may be <code>null</code>) </param>
		public TreeStore(TreeBuilder builder, TreeCache cache) : base()
		{

			this._builder = builder;
			this._cache = cache;
		}

		public virtual TreeBuilder Builder
		{
			get
			{
				return _builder;
			}
		}

		/// <summary>
		/// Get a <seealso cref="Tree"/>.
		/// If a tree for the given expression is present in the cache, it is
		/// taken from there; otherwise, the expression string is parsed and
		/// the resulting tree is added to the cache. </summary>
		/// <param name="expression"> expression string </param>
		/// <returns> expression tree </returns>
		public virtual Tree Get(string expression)
		{
			if (_cache == null)
			{
				return _builder.Build(expression);
			}
			Tree tree = _cache.Get(expression);
			if (tree == null)
			{
				_cache.Put(expression, tree = _builder.Build(expression));
			}
			return tree;
		}
	}

}