using ESS.FW.Bpm.Engine.Filter;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IFilterManager
    {
        IFilter CreateNewFilter(string resourceType);
        void DeleteFilter(string filterId);
        FilterEntity FindFilterById(string filterId);
        IFilter InsertOrUpdateFilter(IFilter filter);
    }
}