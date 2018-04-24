using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Runtime;
using System.Linq;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using System;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    /// <summary>
	/// 
	/// </summary>
    [Component]
    public class IncidentManager : AbstractManagerNet<IncidentEntity>, IIncidentManager
    {
        public IncidentManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }
        
        public virtual IList<IncidentEntity> FindIncidentsByExecution(string id)
        {
            //return ListExt.ConvertToListT<IncidentEntity>(DbEntityManager.SelectList("selectIncidentsByExecutionId", id)) ;
            return Find(m => m.ExecutionId == id).ToList();
        }
        
        public virtual IList<IncidentEntity> FindIncidentsByProcessInstance(string id)
        {
            //return ListExt.ConvertToListT<IncidentEntity>(DbEntityManager.SelectList("selectIncidentsByProcessInstanceId", id));
            return Find(m => m.ProcessInstanceId == id).ToList();
        }

        public IncidentEntity FindIncidentById(string incidentId)
        {
            return Get(incidentId);
        }

        // public virtual long FindIncidentCountByQueryCriteria(IncidentQueryImpl incidentQuery)
        // {
        //          throw new System.NotImplementedException();
        //ConfigureQuery(incidentQuery);
        //          //return (long) DbEntityManager.SelectOne("selectIncidentCountByQueryCriteria", incidentQuery);

        // }

        public virtual IList<IIncident> FindIncidentByConfiguration(string configuration)
        {
            return FindIncidentByConfigurationAndIncidentType(configuration, null);
        }
        
        public virtual IList<IIncident> FindIncidentByConfigurationAndIncidentType(string configuration, string incidentType)
        {
            //IDictionary<string, object> @params = new Dictionary<string, object>();
            //@params["configuration"] = configuration;
            //@params["incidentType"] = incidentType;
            //throw new System.NotImplementedException();
            //return ListExt.ConvertToListT<IIncident>(DbEntityManager.SelectList("selectIncidentsByConfiguration", @params));
            var query = Find(m => m.Configuration == configuration);
            if (incidentType != null)
            {
                return query.Where(m => m.IncidentType == incidentType).ToList().Cast<IIncident>().ToList();
            }
            else
            {
                return query.ToList().Cast<IIncident>().ToList();
            }
            //return DbEntityManager.SelectList(m => m.Configuration == configuration && m.IncidentType == incidentType) as IList<IIncident>;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.runtime.Incident> findIncidentByQueryCriteria(org.camunda.bpm.engine.impl.IncidentQueryImpl incidentQuery, org.camunda.bpm.engine.impl.Page page)
        // public virtual IList<IIncident> FindIncidentByQueryCriteria(IncidentQueryImpl incidentQuery, Page page)
        // {
        //ConfigureQuery(incidentQuery);
        //          throw new System.NotImplementedException();
        //          //return ListExt.ConvertToListT<IIncident>(DbEntityManager.SelectList("selectIncidentByQueryCriteria", incidentQuery, page));
        // }

        // protected internal virtual void ConfigureQuery(IncidentQueryImpl query)
        // {
        //AuthorizationManager.ConfigureIncidentQuery(query);
        //TenantManager.ConfigureQuery(query);
        // }

    }

}