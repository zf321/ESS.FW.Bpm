using ESS.FW.Bpm.Engine.Impl.DB;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface ITenantManager
    {
        bool TenantCheckEnabled { get; }

        //ListQueryParameterObject ConfigureQuery(ListQueryParameterObject query);
        //ListQueryParameterObject ConfigureQuery(object parameters);
        void ConfigureTenantCheck(TenantCheck tenantCheck);
        bool IsAuthenticatedTenant(string tenantId);
    }
}