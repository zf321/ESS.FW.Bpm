using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Identity;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    [Component]
    public class TenantManager : AbstractManagerNet<TenantEntity>, ITenantManager
    {
        protected IAuthorizationManager authorizationManager;
        public TenantManager(DbContext dbContex, ILoggerFactory loggerFactory, IAuthorizationManager _authorizationManager, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
            authorizationManager = _authorizationManager;
        }

        //public virtual ListQueryParameterObject ConfigureQuery(ListQueryParameterObject query)
        //{
        //    TenantCheck tenantCheck = query.TenantCheck;

        //    ConfigureTenantCheck(tenantCheck);

        //    return query;
        //}

        public virtual void ConfigureTenantCheck(TenantCheck tenantCheck)
        {
            if (TenantCheckEnabled)
            {
                Authentication currentAuthentication = CurrentAuthentication;

                tenantCheck.IsTenantCheckEnabled = true;
                tenantCheck.AuthTenantIds = currentAuthentication.TenantIds;

            }
            else
            {
                tenantCheck.IsTenantCheckEnabled = false;
                tenantCheck.AuthTenantIds = null;
            }
        }

        //public virtual ListQueryParameterObject ConfigureQuery(object parameters)
        //{
        //    ListQueryParameterObject queryObject = new ListQueryParameterObject();
        //    queryObject.Parameter = parameters;

        //    return ConfigureQuery(queryObject);
        //}

        public virtual bool IsAuthenticatedTenant(string tenantId)
        {
            if (tenantId != null && TenantCheckEnabled)
            {

                Authentication currentAuthentication = CurrentAuthentication;
                IList<string> authenticatedTenantIds = currentAuthentication.TenantIds;
                if (authenticatedTenantIds != null)
                {
                    return authenticatedTenantIds.Contains(tenantId);

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public virtual bool TenantCheckEnabled
        {
            get
            {
                return context.Impl.Context.ProcessEngineConfiguration.TenantCheckEnabled && context.Impl.Context.CommandContext.TenantCheckEnabled && CurrentAuthentication != null && !authorizationManager.IsCamundaAdmin(CurrentAuthentication);
            }
        }

    }

}