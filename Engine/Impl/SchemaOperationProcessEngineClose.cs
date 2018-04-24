using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///      
    /// </summary>
    internal sealed class SchemaOperationProcessEngineClose : ICommand<object>
    {
        public object Execute(CommandContext commandContext)
        {
            var databaseSchemaUpdate = Context.ProcessEngineConfiguration.DatabaseSchemaUpdate;
            if (ProcessEngineConfiguration.DbSchemaUpdateCreateDrop.Equals(databaseSchemaUpdate))
            {
                //commandContext.GetSession<IPersistenceSession>(typeof(IPersistenceSession)).DbSchemaDrop();
            }
            return null;
        }
    }
}