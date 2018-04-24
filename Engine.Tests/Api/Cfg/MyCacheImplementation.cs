
using ESS.FW.Bpm.Engine.Common.Cache;

namespace Engine.Tests.Api.Cfg
{
	public class MyCacheImplementation<K, V> : ConcurrentLruCache<K, V>
	{

	  public MyCacheImplementation(int capacity) : base(capacity)
	  {
	  }
	}

}