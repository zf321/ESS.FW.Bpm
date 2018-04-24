using ESS.FW.Bpm.Engine.Common.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{
    public class DefaultCacheFactory : ICacheFactory
    {
        public virtual ICache<string, T> CreateCache<T>(int maxNumberOfElementsInCache)
        {
            return new ConcurrentLruCache<string, T>(maxNumberOfElementsInCache);
        }
    }
}
