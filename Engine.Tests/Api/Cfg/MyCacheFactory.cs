using ESS.FW.Bpm.Engine.Common.Cache;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;

namespace Engine.Tests.Api.Cfg
{


    /// <summary>
    /// Uses the concurrent least recently used cache from camunda commons.
    /// 
    /// @author Johannes Heinemann
    /// </summary>
    public class MyCacheFactory : DefaultCacheFactory
    {
        public override ICache<string, T> CreateCache<T>(int maxNumberOfElementsInCache)
        {
            return new MyCacheImplementation<string, T>(maxNumberOfElementsInCache);
        }

    }

}