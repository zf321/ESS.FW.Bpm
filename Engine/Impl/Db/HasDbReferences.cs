using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///     
    /// </summary>
    public interface IHasDbReferences
    {
        /// <returns>
        ///     the ids of the entities that this entity references. Should
        ///     only return ids for entities of the same type
        /// </returns>
        ISet<string> ReferencedEntityIds { get; }
    }
}