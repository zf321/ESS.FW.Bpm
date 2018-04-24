using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Common.Components;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{

    /// <summary>
    /// 
    /// </summary>
    [Component]
    public class ByteArrayManager : AbstractManagerNet<ResourceEntity>, IByteArrayManager
    {
        public ByteArrayManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        /// <summary>
        /// Deletes the <seealso cref="ResourceEntity"/> with the given id from the database.
        /// Important: this operation will NOT do any optimistic locking, to avoid loading the
        /// bytes in memory. So use this method only in conjunction with an entity that has
        /// optimistic locking!.
        /// </summary>
        public virtual void DeleteByteArrayById(string byteArrayEntityId)
        {
            //DbEntityManager.Delete(typeof(ResourceEntity), "deleteByteArrayNoRevisionCheck", byteArrayEntityId);
            Delete(m => m.Id == byteArrayEntityId);
        }

    }

}