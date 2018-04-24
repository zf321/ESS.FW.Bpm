using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Form.Impl.Engine
{
    /// <summary>
    ///      
    /// </summary>
    public class JuelFormEngine : IFormEngine
    {
        public virtual string Name
        {
            get { return "juel"; }
        }

        public virtual object RenderStartForm(IStartFormData startForm)
        {
            if (ReferenceEquals(startForm.FormKey, null))
                return null;
            var formTemplateString = GetFormTemplateString(startForm, startForm.FormKey);
            return ExecuteScript(formTemplateString, null);
        }


        public virtual object RenderTaskForm(ITaskFormData taskForm)
        {
            if (ReferenceEquals(taskForm.FormKey, null))
                return null;
            var formTemplateString = GetFormTemplateString(taskForm, taskForm.FormKey);
            var task = (TaskEntity) taskForm.Task;
            //return executeScript(formTemplateString, task.getExecution());
            return null;
        }

        protected internal virtual object ExecuteScript(string scriptSrc, IVariableScope scope)
        {
            var processEngineConfiguration = Context.ProcessEngineConfiguration;
            //ScriptFactory scriptFactory = processEngineConfiguration.ScriptFactory;
            //ExecutableScript script = scriptFactory.createScriptFromSource(ScriptingEngines.DEFAULT_SCRIPTING_LANGUAGE,
            //    scriptSrc);

            //var invocation = new ScriptInvocation(script, scope);
            try
            {
                //processEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
            }
            //catch (Exception e)
            //{
            //    throw e;
            //}
            catch (System.Exception e)
            {
                throw new ProcessEngineException(e);
            }

            //return invocation.InvocationResult;
            return null;
        }

        protected internal virtual string GetFormTemplateString(IFormData formInstance, string formKey)
        {
            var deploymentId = formInstance.DeploymentId;

            //ResourceEntity resourceStream =
            //    Context.CommandContext.ResourceManager.findResourceByDeploymentIdAndResourceName(deploymentId, formKey);

            //EnsureUtil.EnsureNotNull("Form with formKey '" + formKey + "' does not exist", "resourceStream",
            //    resourceStream);

            //byte[] resourceBytes = resourceStream.Bytes;
            var encoding = "UTF-8";
            var formTemplateString = "";
            try
            {
                //formTemplateString = StringHelperClass.NewString(resourceBytes, encoding);
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException("Unsupported encoding of :" + encoding, e);
            }
            return formTemplateString;
        }
    }
}