using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Form.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///      
    ///     
    /// </summary>
    [Serializable]
    public class SubmitStartFormCmd : ICommand<IProcessInstance>
    {
        private const long SerialVersionUid = 1L;
        protected internal readonly string BusinessKey;

        protected internal readonly string ProcessDefinitionId;
        protected internal IVariableMap Variables;

        public SubmitStartFormCmd(string processDefinitionId, string businessKey, IDictionary<string, object> properties)
        {
            this.ProcessDefinitionId = processDefinitionId;
            this.BusinessKey = businessKey;
            //Variables = Variables.FromMap(properties);
        }

        public virtual IProcessInstance Execute(CommandContext commandContext)
        {
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
            ProcessDefinitionEntity processDefinition =
                deploymentCache.FindDeployedProcessDefinitionById(ProcessDefinitionId);
            EnsureUtil.EnsureNotNull("No process definition found for id = '" + ProcessDefinitionId + "'",
                "processDefinition", processDefinition);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckCreateProcessInstance(processDefinition);
            }

            ExecutionEntity processInstance = null;
            if (!ReferenceEquals(BusinessKey, null))
            {
                processInstance = (ExecutionEntity)processDefinition.CreateProcessInstance(BusinessKey);
            }
            else
            {
                processInstance = (ExecutionEntity)processDefinition.CreateProcessInstance();
            }

            //if the start event is async, we have to set the variables already here
            //since they are lost after the async continuation otherwise
            if (processDefinition.Initial.AsyncBefore)
            {
            // avoid firing history events
            processInstance.StartContext = new ProcessInstanceStartContext(processInstance.Activity as ActivityImpl);
            FormPropertyHelper.InitFormPropertiesOnScope(Variables, processInstance);
            processInstance.Start();
        }
            else
            {
            processInstance.StartWithFormProperties(Variables);
        }


            return (IProcessInstance) processInstance;
        }
    }
}