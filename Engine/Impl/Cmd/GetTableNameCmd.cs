using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{



    [Serializable]
    public class GetTableNameCmd : ICommand<string>
    {
        private const long SerialVersionUid = 1L;

        private readonly Type _entityClass;

        public GetTableNameCmd(Type entityClass)
        {
            this._entityClass = entityClass;
        }

        public virtual string Execute(CommandContext commandContext)
        {
            throw new NotImplementedException();
            //EnsureUtil.EnsureNotNull("entityClass", _entityClass);

            //commandContext.AuthorizationManager.CheckCamundaAdmin();

            //return commandContext.TableDataManager.GetTableName(_entityClass, true);
        }
    }
}