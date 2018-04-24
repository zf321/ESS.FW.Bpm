using ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy;

namespace Engine.Tests.Api.MultiTenancy
{
    
	
	public class StaticTenantIdTestProvider : ITenantIdProvider
	{

	  protected internal string TenantId;

	  public StaticTenantIdTestProvider(string tenantId)
	  {
		this.TenantId = tenantId;
	  }

	  public virtual string TenantIdProvider
	  {
		  set
		  {
			this.TenantId = value;
		  }
	  }

	  public  string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
	  {
		return TenantId;
	  }

	  public  string ProvideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
	  {
		return TenantId;
	  }

	  public  string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
	  {
		return TenantId;
	  }
        
    }
}