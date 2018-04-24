using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.Form.Impl;
using ESS.FW.Bpm.Engine.Form.Impl.Handler;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;

namespace Engine.Tests.Api.Authorization.Service
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class MyTaskFormHandler : MyDelegationService, ITaskFormHandler
    {

        //public virtual void parseConfiguration(Element activityElement, DeploymentEntity deployment, ProcessDefinitionEntity processDefinition, BpmnParse bpmnParse)
        //{
        //    // do nothing
        //}

        //public virtual void submitFormVariables(IVariableMap properties, IVariableScope variableScope)
        //{
        //    ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
        //    IIdentityService identityService = processEngineConfiguration.IdentityService;
        //    IRuntimeService runtimeService = processEngineConfiguration.RuntimeService;

        //    logAuthentication(identityService);
        //    logInstancesCount(runtimeService);
        //}

        //public virtual ITaskFormData createTaskForm(TaskEntity task)
        //{
        //    ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
        //    IIdentityService identityService = processEngineConfiguration.IdentityService;
        //    IRuntimeService runtimeService = processEngineConfiguration.RuntimeService;

        //    logAuthentication(identityService);
        //    logInstancesCount(runtimeService);

        //    TaskFormDataImpl result = new TaskFormDataImpl();
        //    result.Task = task;
        //    return result;
        //}

        public ITaskFormData CreateTaskForm(TaskEntity task)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = ESS.FW.Bpm.Engine.context.Impl.Context.ProcessEngineConfiguration;
            IIdentityService identityService = processEngineConfiguration.IdentityService;
            IRuntimeService runtimeService = processEngineConfiguration.RuntimeService;

            logAuthentication(identityService);
            logInstancesCount(runtimeService);

            TaskFormDataImpl result = new TaskFormDataImpl();
            result.Task = task;
            return result;
        }

        public void ParseConfiguration(Element activityElement, DeploymentEntity deployment, ProcessDefinitionEntity processDefinition, BpmnParse bpmnParse)
        {
            // do nothing
        }

        public void SubmitFormVariables(IVariableMap properties, IVariableScope variableScope)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = ESS.FW.Bpm.Engine.context.Impl.Context.ProcessEngineConfiguration;
            IIdentityService identityService = processEngineConfiguration.IdentityService;
            IRuntimeService runtimeService = processEngineConfiguration.RuntimeService;

            logAuthentication(identityService);
            logInstancesCount(runtimeService);
        }
    }

}