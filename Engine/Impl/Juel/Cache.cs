using System.Collections.Concurrent;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{




	/// <summary>
	/// Simple (thread-safe) LRU cache.
	/// After the cache size reached a certain limit, the least recently used entry is removed,
	/// when adding a new entry.
	/// 
	/// 
	/// </summary>
	public sealed class Cache : TreeCache
	{
	  private readonly ConcurrentDictionary<string, Tree> _primary;
	  private readonly ConcurrentDictionary<string, Tree> _secondary;

	  /// <summary>
	  /// Constructor.
	  /// Use a <seealso cref="WeakHashMap"/> as secondary map. </summary>
	  /// <param name="size"> maximum primary cache size </param>
		public Cache(int size) : this(size, new ConcurrentDictionary<string, Tree>())
		{
		}

		/// <summary>
		/// Constructor.
		/// If the least recently used entry is removed from the primary cache, it is added to
		/// the secondary map. </summary>
		/// <param name="size"> maximum primary cache size </param>
		/// <param name="secondary"> the secondary map (may be <code>null</code>) </param>
		public Cache(int size, ConcurrentDictionary<string, Tree> secondary)
		{
			this._primary = new ConcurrentDictionary<string, Tree>(secondary);
			this._secondary = secondary == null ? null : secondary;
		}

		//private class LinkedHashMapAnonymousInnerClass : ConcurrentDictionary<string, Tree>
		//{
		//	private readonly Cache outerInstance;

		//	private int size;
		//	private IDictionary<string, Tree> secondary;

		//	public LinkedHashMapAnonymousInnerClass(Cache outerInstance, int size, IDictionary<string, Tree> secondary) : base(16, 0.75f, true)
		//	{
		//		this.outerInstance = outerInstance;
		//		this.size = size;
		//		this.secondary = secondary;
		//	}

		//	protected internal override bool RemoveEldestEntry(KeyValuePair<string, Tree> eldest)
		//	{
		//		if (Size() > size)
		//		{
		//			if (outerInstance.secondary != null)
		//			{ // move to secondary cache
		//				outerInstance.secondary[eldest.Key] = eldest.Value;
		//			}
		//			return true;
		//		}
		//		return false;
		//	}
		//}

		public Tree Get(string expression)
		{
			if (_secondary == null)
			{
				return _primary[expression];
			}
			else
			{
				Tree tree;
                    _primary.TryGetValue(expression,out tree);
				if (tree == null)
				{
                    _secondary.TryGetValue(expression, out tree);
				}
				return tree;
			}
		}

		public void Put(string expression, Tree tree)
		{
			_primary[expression] = tree;
		}
	}

}