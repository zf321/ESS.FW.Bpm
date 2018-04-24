using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Form.Impl.Handler;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Command for retrieving start or ITask form keys.
    ///      (camunda)
    /// </summary>
    public class GetFormKeyCmd : ICommand<string>
    {
        protected internal string processDefinitionId;

        protected internal string TaskDefinitionKey;

        /// <summary>
        ///     Retrieves a start form key.
        /// </summary>
        public GetFormKeyCmd(string processDefinitionId)
        {
            ProcessDefinitionId = processDefinitionId;
        }

        /// <summary>
        ///     Retrieves a ITask form key.
        /// </summary>
        public GetFormKeyCmd(string processDefinitionId, string taskDefinitionKey)
        {
            ProcessDefinitionId = processDefinitionId;
            if (ReferenceEquals(taskDefinitionKey, null) || (taskDefinitionKey.Length < 1))
                throw new ProcessEngineException("The ITask definition key is mandatory, but '" + taskDefinitionKey +
                                                 "' has been provided.");
            this.TaskDefinitionKey = taskDefinitionKey;
        }

        protected internal virtual string ProcessDefinitionId
        {
            set
            {
                if (ReferenceEquals(value, null) || (value.Length < 1))
                    throw new ProcessEngineException("The process definition id is mandatory, but '" + value +
                                                     "' has been provided.");
                processDefinitionId = value;
            }
        }

        public virtual string Execute(CommandContext commandContext)
        {
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
            ProcessDefinitionEntity processDefinition =    deploymentCache.FindDeployedProcessDefinitionById(processDefinitionId);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadProcessDefinition(processDefinition);
            }

            IExpression formKeyExpression = null;

            if (ReferenceEquals(TaskDefinitionKey, null))
            {
                // TODO: Maybe add getFormKey() to FormHandler interface to avoid the following cast
                IFormHandler formHandler = processDefinition.StartFormHandler;

                if (formHandler is DelegateStartFormHandler)
                {
                    var delegateFormHandler = (DelegateStartFormHandler)formHandler;
                    formHandler = delegateFormHandler.FormHandler;
                }

                // Sorry!!! In case of a custom start form handler (which does not extend
                // the DefaultFormHandler) a formKey would never be returned. So a custom
                // form handler (for a startForm) has always to extend the DefaultStartFormHandler!
                if (formHandler is DefaultStartFormHandler)
                {
                    var startFormHandler = (DefaultStartFormHandler)formHandler;
                    formKeyExpression = startFormHandler.FormKey;
                }
            }

            string formKey = null;
            if (formKeyExpression != null)
                formKey = formKeyExpression.ExpressionText;
            return formKey;
        }
    }
}