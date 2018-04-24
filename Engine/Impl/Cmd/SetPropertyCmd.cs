using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     <para>Command which can be used for setting the value of a property</para>
    ///     
    /// </summary>
    public class SetPropertyCmd : ICommand<object>
    {
        protected internal string Name;
        protected internal string Value;

        public SetPropertyCmd(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            IPropertyManager propertyManager = commandContext.PropertyManager;

            PropertyEntity property = propertyManager.FindPropertyById(Name);
            if (property != null)
            {
                // update
                property.Value = Value;

            }
            else
            {
                // create
                property = new PropertyEntity(Name, Value);
                propertyManager.Add(property);

            }

            return null;
        }
    }
}