using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    


    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetIdentityLinksForProcessDefinitionCmd : ICommand<IList<IIdentityLink>>
    {
        private const long SerialVersionUid = 1L;
        protected internal string ProcessDefinitionId;

        public GetIdentityLinksForProcessDefinitionCmd(string processDefinitionId)
        {
            this.ProcessDefinitionId = processDefinitionId;
        }
        
        public virtual IList<IIdentityLink> Execute(CommandContext commandContext)
        {
            ProcessDefinitionEntity processDefinition = Context.CommandContext.ProcessDefinitionManager.FindLatestProcessDefinitionById(ProcessDefinitionId);

            EnsureUtil.EnsureNotNull("Cannot find process definition with id " + ProcessDefinitionId, "processDefinition", processDefinition);

            IList<IIdentityLink> identityLinks = processDefinition.IdentityLinks.Cast<IIdentityLink>().ToList();
            return identityLinks;
        }
    }
}