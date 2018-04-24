using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IIncidentManager:IRepository<IncidentEntity,string>
    {
        IList<IIncident> FindIncidentByConfiguration(string configuration);
        IList<IIncident> FindIncidentByConfigurationAndIncidentType(string configuration, string incidentType);
        IList<IncidentEntity> FindIncidentsByExecution(string id);
        IList<IncidentEntity> FindIncidentsByProcessInstance(string id);
        IncidentEntity FindIncidentById(string incidentId);
    }
}