


using ESS.FW.Bpm.Engine.Common.Cache;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{


	/// <summary>
	/// <para>Builds the caches for the <seealso cref="DeploymentCache"/>.</para>
	/// </summary>
	public interface ICacheFactory
	{

	  /// <summary>
	  /// Creates a cache that does not exceed a specified number of elements.
	  /// </summary>
	  /// <param name="maxNumberOfElementsInCache">
	  ///        The maximum number of elements that is allowed within the cache at the same time.
	  /// @return
	  ///        The cache to be created. </param>
	  ICache<string, T> CreateCache<T>(int maxNumberOfElementsInCache);
	}

}