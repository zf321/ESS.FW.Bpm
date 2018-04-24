


using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using ESS.FW.Common.Components;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{


    /// <summary>
    /// 
    /// </summary>
    [System.Obsolete("DB全局配置表没有用？")]
    [Component]
    public class PropertyManager : AbstractManagerNet<PropertyEntity>, IPropertyManager
    {
        //private IEntityLoadListener __listener;
        public PropertyManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
            //__listener = listener;
        }

        public virtual PropertyEntity FindPropertyById(string propertyId)
        {
            return Single(m => m.Name == propertyId);// DbEntityManager.SelectOne(m=>m.Name== propertyId);
        }
        //TODO 互斥锁Property
        public virtual void AcquireExclusiveLock()
        {
            throw new System.NotImplementedException();
            // We lock a special deployment lock property
         //  DbEntityManager.Lock("lockDeploymentLockProperty");

        }

    }

}