using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using System.Collections;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    

    /// <summary>
    ///     
    /// </summary>
    public class StartProcessInstanceAtActivitiesCmd : ICommand<IProcessInstanceWithVariables>
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        protected internal ProcessInstantiationBuilderImpl InstantiationBuilder;

        public StartProcessInstanceAtActivitiesCmd(ProcessInstantiationBuilderImpl instantiationBuilder)
        {
            this.InstantiationBuilder = instantiationBuilder;
        }

        public virtual IProcessInstanceWithVariables Execute(CommandContext commandContext)
        {
            var processDefinition =
                new GetDeployedProcessDefinitionCmd(InstantiationBuilder, false).Execute(commandContext);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckCreateProcessInstance(processDefinition);

            var modificationBuilder = InstantiationBuilder.ModificationBuilder;
            EnsureUtil.EnsureNotEmpty(
                @"At least one instantiation instruction required (e.g. by invoking startBefore(..), startAfter(..) or startTransition(..))","instructions",ListExt.ConvertToIlist(modificationBuilder.ModificationOperations));

            // instantiate the process
            var initialActivity = DetermineFirstActivity(processDefinition, modificationBuilder);

            ExecutionEntity processInstance =(ExecutionEntity) processDefinition.CreateProcessInstance(InstantiationBuilder.BusinessKey,
                InstantiationBuilder.CaseInstanceId, initialActivity);
            processInstance.SkipCustomListeners = modificationBuilder.SkipCustomListeners;
            var variables = modificationBuilder.ProcessVariables;
            
            ExecutionVariableSnapshotObserver variablesListener = new ExecutionVariableSnapshotObserver(processInstance);

            processInstance.StartWithoutExecuting(variables);

            // prevent ending of the process instance between instructions
            processInstance.PreserveScope = true;

            // apply modifications
            var instructions = modificationBuilder.ModificationOperations;

            for (var i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];
                Log.DebugStartingInstruction(processInstance.Id, i, instruction.Describe());

                instruction.ProcessInstanceId = processInstance.Id;
                instruction.SkipCustomListeners = modificationBuilder.SkipCustomListeners;
                instruction.SkipIoMappings = modificationBuilder.SkipIoMappings;
                instruction.Execute(commandContext);
            }

            if (!processInstance.HasChildren() && processInstance.IsEnded)
            {
                // process instance has ended regularly but this has not been propagated yet
                // due to preserveScope setting
                processInstance.PropagateEnd();
            }

            return new ProcessInstanceWithVariablesImpl(processInstance, variablesListener.Variables);
        }


        /// <summary>
        ///     get the activity that is started by the first instruction, if exists;
        ///     return null if the first instruction is a start-transition instruction
        /// </summary>
        protected internal virtual ActivityImpl DetermineFirstActivity(ProcessDefinitionImpl processDefinition,
            ProcessInstanceModificationBuilderImpl modificationBuilder)
        {
            var firstInstruction = modificationBuilder.ModificationOperations[0];

            if (firstInstruction is AbstractInstantiationCmd)
            {
                var instantiationInstruction = (AbstractInstantiationCmd) firstInstruction;
                var targetElement = instantiationInstruction.GetTargetElement(processDefinition);

                EnsureUtil.EnsureNotNull(typeof(NotValidException),
                    "Element '" + instantiationInstruction.TargetElementId + "' does not exist in process " +
                    processDefinition.Id, "targetElement", targetElement);

                if (targetElement is ActivityImpl)
                    return (ActivityImpl) targetElement;
                if (targetElement is TransitionImpl)
                    return (ActivityImpl) ((TransitionImpl) targetElement).Destination;
            }

            return null;
        }
    }
}