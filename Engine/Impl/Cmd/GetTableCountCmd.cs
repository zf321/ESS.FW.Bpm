using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetTableCountCmd : ICommand<IDictionary<string, long>>
    {
        private const long SerialVersionUid = 1L;

        public virtual IDictionary<string, long> Execute(CommandContext commandContext)
        {
            commandContext.AuthorizationManager.CheckCamundaAdmin();
            throw new NotImplementedException();
            //return commandContext.TableDataManager.TableCount;
        }
    }
}