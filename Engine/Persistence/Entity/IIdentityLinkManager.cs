using System.Collections.Generic;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IIdentityLinkManager:IRepository<IdentityLinkEntity,string>
    {
        void DeleteIdentityLinksByProcDef(string processDefId);
        IList<IdentityLinkEntity> FindIdentityLinkByProcessDefinitionUserAndGroup(string processDefinitionId, string userId, string groupId);
        IList<IdentityLinkEntity> FindIdentityLinkByTaskUserGroupAndType(string taskId, string userId, string groupId, string type);
        IList<IdentityLinkEntity> FindIdentityLinksByProcessDefinitionId(string processDefinitionId);
        IList<IdentityLinkEntity> FindIdentityLinksByTaskId(string taskId);
    }
}