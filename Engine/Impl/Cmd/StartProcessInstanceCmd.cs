using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    [Serializable]
    public class StartProcessInstanceCmd : ICommand<IProcessInstanceWithVariables>
    {
        private const long SerialVersionUid = 1L;

        protected internal readonly ProcessInstantiationBuilderImpl InstantiationBuilder;

        public StartProcessInstanceCmd(ProcessInstantiationBuilderImpl instantiationBuilder)
        {
            this.InstantiationBuilder = instantiationBuilder;
        }

        public virtual IProcessInstanceWithVariables Execute(CommandContext commandContext)
        {
            var processDefinition =
                new GetDeployedProcessDefinitionCmd(InstantiationBuilder, false).Execute(commandContext);


            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckCreateProcessInstance(processDefinition);
            }


            // Start the process instance
            Pvm.IPvmProcessInstance processInstance =processDefinition.CreateProcessInstance(InstantiationBuilder.BusinessKey,InstantiationBuilder.CaseInstanceId);
            

            ExecutionVariableSnapshotObserver variablesListener = new ExecutionVariableSnapshotObserver(processInstance);

            processInstance.Start(InstantiationBuilder.Variables);
            return new ProcessInstanceWithVariablesImpl(processInstance, variablesListener.Variables);
        }
    }
}