using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IPropertyManager : IRepository<PropertyEntity,string>
    {
        void AcquireExclusiveLock();
        PropertyEntity FindPropertyById(string propertyId);
    }
}