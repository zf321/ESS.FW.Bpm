using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Management;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{


    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetTableMetaDataCmd : ICommand<TableMetaData>
    {
        private const long SerialVersionUid = 1L;
        protected internal string TableName;

        public GetTableMetaDataCmd(string tableName)
        {
            this.TableName = tableName;
        }

        public virtual TableMetaData Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("tableName", TableName);
            commandContext.AuthorizationManager.CheckCamundaAdmin();
            throw new NotImplementedException();
            //return commandContext.TableDataManager.GetTableMetaData(TableName);
        }
    }
}