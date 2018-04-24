using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///      
    /// </summary>
    public class FormServiceImpl : ServiceImpl, IFormService
    {
        public virtual object GetRenderedStartForm(string processDefinitionId)
        {
            return CommandExecutor.Execute(new GetRenderedStartFormCmd(processDefinitionId, null));
        }

        public virtual object GetRenderedStartForm(string processDefinitionId, string engineName)
        {
            return CommandExecutor.Execute(new GetRenderedStartFormCmd(processDefinitionId, engineName));
        }

        public virtual object GetRenderedTaskForm(string taskId)
        {
            return CommandExecutor.Execute(new GetRenderedTaskFormCmd(taskId, null));
        }

        public virtual object GetRenderedTaskForm(string taskId, string engineName)
        {
            return CommandExecutor.Execute(new GetRenderedTaskFormCmd(taskId, engineName));
        }

        public virtual IStartFormData GetStartFormData(string processDefinitionId)
        {
            return CommandExecutor.Execute(new GetStartFormCmd(processDefinitionId));
        }

        public virtual ITaskFormData GetTaskFormData(string taskId)
        {
            return CommandExecutor.Execute(new GetTaskFormCmd(taskId));
        }

        public virtual IProcessInstance SubmitStartFormData(string processDefinitionId,
            IDictionary<string, string> properties)
        {
            return
                CommandExecutor.Execute(new SubmitStartFormCmd(processDefinitionId, null,
                    (IDictionary<string, object>) properties));
        }

        public virtual IProcessInstance SubmitStartFormData(string processDefinitionId, string businessKey,
            IDictionary<string, string> properties)
        {
            return
                CommandExecutor.Execute(new SubmitStartFormCmd(processDefinitionId, businessKey,
                    (IDictionary<string, object>) properties));
        }

        public virtual IProcessInstance SubmitStartForm(string processDefinitionId,
            IDictionary<string, object> properties)
        {
            return CommandExecutor.Execute(new SubmitStartFormCmd(processDefinitionId, null, properties));
        }

        public virtual IProcessInstance SubmitStartForm(string processDefinitionId, string businessKey,
            IDictionary<string, object> properties)
        {
            return CommandExecutor.Execute(new SubmitStartFormCmd(processDefinitionId, businessKey, properties));
        }

        public virtual void SubmitTaskFormData(string taskId, IDictionary<string, string> properties)
        {
            CommandExecutor.Execute(new SubmitTaskFormCmd(taskId, (IDictionary<string, ITypedValue>) properties));
        }

        public virtual void SubmitTaskForm(string taskId, IDictionary<string, ITypedValue> properties)
        {
            CommandExecutor.Execute(new SubmitTaskFormCmd(taskId, properties));
        }

        public virtual string GetStartFormKey(string processDefinitionId)
        {
            return CommandExecutor.Execute(new GetFormKeyCmd(processDefinitionId));
        }

        public virtual string GetTaskFormKey(string processDefinitionId, string taskDefinitionKey)
        {
            return CommandExecutor.Execute(new GetFormKeyCmd(processDefinitionId, taskDefinitionKey));
        }

        public virtual IVariableMap GetStartFormVariables(string processDefinitionId)
        {
            return GetStartFormVariables(processDefinitionId, null, true);
        }

        public virtual IVariableMap GetStartFormVariables(string processDefinitionId, ICollection<string> formVariables,
            bool deserializeObjectValues)
        {
            return
                CommandExecutor.Execute(new GetStartFormVariablesCmd(processDefinitionId, formVariables,
                    deserializeObjectValues));
        }

        public virtual IVariableMap GetTaskFormVariables(string taskId)
        {
            return GetTaskFormVariables(taskId, null, true);
        }

        public virtual IVariableMap GetTaskFormVariables(string taskId, ICollection<string> formVariables,
            bool deserializeObjectValues)
        {
            return CommandExecutor.Execute(new GetTaskFormVariablesCmd(taskId, formVariables, deserializeObjectValues));
        }
    }
}