using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class DeletePropertyCmd : ICommand<object>
    {
        protected internal string Name;

        /// <param name="name"> </param>
        public DeletePropertyCmd(string name)
        {
            this.Name = name;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            IPropertyManager propertyManager = commandContext.PropertyManager;

            PropertyEntity propertyEntity = propertyManager.FindPropertyById(Name);

            if (propertyEntity != null)
            {
                propertyManager.Delete(propertyEntity);
            }

            return null;
        }
    }
}