using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public abstract class AbstractGetFormVariablesCmd : ICommand<IVariableMap>
    {
        private const long SerialVersionUid = 1L;
        protected internal bool DeserializeObjectValues;
        public ICollection<string> FormVariableNames;

        public string ResourceId;

        public AbstractGetFormVariablesCmd(string resourceId, ICollection<string> formVariableNames,
            bool deserializeObjectValues)
        {
            this.ResourceId = resourceId;
            this.FormVariableNames = formVariableNames;
            this.DeserializeObjectValues = deserializeObjectValues;
        }

        public abstract IVariableMap Execute(CommandContext commandContext);

        protected internal virtual ITypedValue CreateVariable(IFormField formField, IVariableScope variableScope)
        {
            var value = formField.Value;

            if (value != null)
                return value;
            return null;
        }
    }
}