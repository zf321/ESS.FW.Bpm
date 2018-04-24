using System;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public abstract class AbstractWritableIdentityServiceCmd<T> : ICommand<T>
    {

        public T Execute(CommandContext commandContext)
        {
            // check identity service implementation
            //if (!commandContext.SessionFactories.ContainsKey(typeof(IWritableIdentityProvider)))
            //    throw new NotSupportedException("This identity service implementation is read-only.");

            var result = ExecuteCmd(commandContext);
            return result;
        }

        protected internal abstract T ExecuteCmd(CommandContext commandContext);
    }
}