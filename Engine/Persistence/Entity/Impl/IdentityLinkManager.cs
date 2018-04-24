using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Common;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{



    /// <summary>
    ///  
    /// 
    /// </summary>
    [Component]
    public class IdentityLinkManager : AbstractManagerNet<IdentityLinkEntity>, IIdentityLinkManager
    {
        public IdentityLinkManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<IdentityLinkEntity> findIdentityLinksByTaskId(String taskId)
        public virtual IList<IdentityLinkEntity> FindIdentityLinksByTaskId(string taskId)
	  {
		//return ListExt.ConvertToListT<IdentityLinkEntity>(DbEntityManager.SelectList("selectIdentityLinksByTask", taskId));
            return Find(m => m.TaskId == taskId).ToList();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<IdentityLinkEntity> findIdentityLinksByProcessDefinitionId(String processDefinitionId)
	  public virtual IList<IdentityLinkEntity> FindIdentityLinksByProcessDefinitionId(string processDefinitionId)
	  {
		//return ListExt.ConvertToListT<IdentityLinkEntity>(DbEntityManager.SelectList("selectIdentityLinksByProcessDefinition", processDefinitionId));
            return Find(m => m.ProcessDefId == processDefinitionId).ToList();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<IdentityLinkEntity> findIdentityLinkByTaskUserGroupAndType(String taskId, String userId, String groupId, String type)
	  public virtual IList<IdentityLinkEntity> FindIdentityLinkByTaskUserGroupAndType(string taskId, string userId, string groupId, string type)
	  {
		//IDictionary<string, string> parameters = new Dictionary<string, string>();
		//parameters["taskId"] = taskId;
		//parameters["userId"] = userId;
		//parameters["groupId"] = groupId;
		//parameters["type"] = type;
		//return ListExt.ConvertToListT<IdentityLinkEntity>(DbEntityManager.SelectList("selectIdentityLinkByTaskUserGroupAndType", parameters));
           return Find(m => m.TaskId == taskId && m.UserId == userId && m.GroupId == groupId&&m.Type==type).ToList();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<IdentityLinkEntity> findIdentityLinkByProcessDefinitionUserAndGroup(String processDefinitionId, String userId, String groupId)
	  public virtual IList<IdentityLinkEntity> FindIdentityLinkByProcessDefinitionUserAndGroup(string processDefinitionId, string userId, string groupId)
	  {
		//IDictionary<string, string> parameters = new Dictionary<string, string>();
		//parameters["processDefinitionId"] = processDefinitionId;
		//parameters["userId"] = userId;
		//parameters["groupId"] = groupId;
		//return ListExt.ConvertToListT<IdentityLinkEntity>(DbEntityManager.SelectList("selectIdentityLinkByProcessDefinitionUserAndGroup", parameters));
           return Find(m => m.ProcessDefId == processDefinitionId && m.UserId == userId && m.GroupId == groupId).ToList();
	  }

	  public virtual void DeleteIdentityLinksByProcDef(string processDefId)
	  {
            //DbEntityManager.Delete(typeof(IdentityLinkEntity), "deleteIdentityLinkByProcDef", processDefId);
            Delete(m=>m.ProcessDefId==processDefId);
	  }

	}

}