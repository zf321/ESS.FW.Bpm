

namespace ESS.Shared.Entities.Bpm
{
    public interface IDbEntity
    {
        string Id { get; set; }

        /// <summary>
        ///     Returns a representation of the object,
        ///     as would be stored in the database.
        ///     Used when deciding if updates have
        ///     occurred to the object or not since
        ///     it was last loaded.
        /// </summary>
        object GetPersistentState();
    }
}
