using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    public interface IEntityLoadListener
    {
        void OnEntityLoaded(IDbEntity entity);
        
    }
}