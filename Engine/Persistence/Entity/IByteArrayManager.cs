using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IByteArrayManager : IRepository<ResourceEntity,string>
    {
        /// <summary>
        /// Deletes the <seealso cref="ResourceEntity"/> with the given id from the database.
        /// Important: this operation will NOT do any optimistic locking, to avoid loading the
        /// bytes in memory. So use this method only in conjunction with an entity that has
        /// optimistic locking!.
        /// </summary>
        void DeleteByteArrayById(string byteArrayEntityId);
    }
}