using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetPropertiesCmd : ICommand<IDictionary<string, string>>
    {
        private const long SerialVersionUid = 1L;
        
        public virtual IDictionary<string, string> Execute(CommandContext commandContext)
        {
            IList<PropertyEntity> propertyEntities = commandContext.PropertyManager.GetAll().ToList();

            IDictionary<string, string> properties = new Dictionary<string, string>();
            foreach (PropertyEntity propertyEntity in propertyEntities)
            {
                properties[propertyEntity.Name] = propertyEntity.Value;
            }
            return properties;
        }
    }
}