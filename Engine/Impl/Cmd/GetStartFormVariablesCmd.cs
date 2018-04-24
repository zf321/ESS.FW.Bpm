using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;


using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetStartFormVariablesCmd : AbstractGetFormVariablesCmd
    {
        private const long SerialVersionUid = 1L;

        public GetStartFormVariablesCmd(string resourceId, ICollection<string> formVariableNames,
            bool deserializeObjectValues) : base(resourceId, formVariableNames, deserializeObjectValues)
        {
        }
        
        public override IVariableMap Execute(CommandContext commandContext)
        {
            IStartFormData startFormData = commandContext.RunWithoutAuthorization(() => new GetStartFormCmd(ResourceId).Execute(commandContext));

            IProcessDefinition definition = startFormData.ProcessDefinition;
            CheckGetStartFormVariables((ProcessDefinitionEntity)definition, commandContext);

            IVariableMap result = new VariableMapImpl();

            foreach (IFormField formField in startFormData.FormFields)
            {
                if (FormVariableNames == null || FormVariableNames.Contains(formField.Id))
                {
                    result.PutValue(formField.Id, CreateVariable(formField, null));
                }
            }

            return result;
        }

        protected internal virtual void CheckGetStartFormVariables(ProcessDefinitionEntity definition,
            CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadProcessDefinition(definition);
            }
        }
        
    }
}