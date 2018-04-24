using System;

namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation.Comparator
{
    /// <summary>
    ///     
    /// </summary>
    public class EntityTypeComparatorForInserts : EntityTypeComparatorForModifications
    {
        public override int Compare(Type firstEntityType, Type secondEntityType)
        {
            return base.Compare(firstEntityType, secondEntityType)*-1;
        }
    }
}