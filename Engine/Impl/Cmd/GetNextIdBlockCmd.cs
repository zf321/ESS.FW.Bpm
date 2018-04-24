using System;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Obsolete("user newid instead")]
    public class GetNextIdBlockCmd : ICommand<IdBlock>
    {
        private const long SerialVersionUid = 1L;
        protected internal int IdBlockSize;

        public GetNextIdBlockCmd(int idBlockSize)
        {
            this.IdBlockSize = idBlockSize;
        }

        public virtual IdBlock Execute(CommandContext commandContext)
        {
            PropertyEntity property = commandContext.PropertyManager.FindPropertyById("next.dbid");
            long oldValue = long.Parse(property.Value);
            long newValue = oldValue + IdBlockSize;
            property.Value = Convert.ToString(newValue);
            return new IdBlock(oldValue, newValue - 1);
        }
    }
}