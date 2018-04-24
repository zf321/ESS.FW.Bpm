using System.Collections.Generic;


using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl
{

    /// <summary>
    ///     
    /// </summary>
    public class ProcessInstanceModificationBuilderImpl : IProcessInstanceModificationInstantiationBuilder
    {
        // variables not associated with an activity that are to be set on the instance itself

        public ProcessInstanceModificationBuilderImpl(ICommandExecutor commandExecutor, string processInstanceId)
            : this(processInstanceId)
        {
            CommandExecutor = commandExecutor;
        }

        public ProcessInstanceModificationBuilderImpl(CommandContext commandContext, string processInstanceId)
            : this(processInstanceId)
        {
            CommandContext = commandContext;
        }

        public ProcessInstanceModificationBuilderImpl(string processInstanceId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "processInstanceId", processInstanceId);
            ProcessInstanceId = processInstanceId;
        }

        public ProcessInstanceModificationBuilderImpl()
        {
        }

        protected internal virtual AbstractInstantiationCmd CurrentInstantiation
        {
            get
            {
                if (ModificationOperations.Count == 0)
                    return null;

                // casting should be safe
                var lastInstantiationCmd = ModificationOperations[ModificationOperations.Count - 1];

                if (!(lastInstantiationCmd is AbstractInstantiationCmd))
                    throw new ProcessEngineException("last instruction is not an instantiation");

                return (AbstractInstantiationCmd) lastInstantiationCmd;
            }
        }

        public virtual ICommandExecutor CommandExecutor { get; }

        public virtual CommandContext CommandContext { get; }

        public virtual string ProcessInstanceId { get; }

        public virtual IList<AbstractProcessInstanceModificationCommand> ModificationOperations { get; set; } = new List<AbstractProcessInstanceModificationCommand>();

        public virtual bool SkipCustomListeners { get; set; }

        public virtual bool SkipIoMappings { get; set; }


        public virtual IVariableMap ProcessVariables { get; } = new VariableMapImpl();

        public virtual IProcessInstanceModificationBuilder CancelActivityInstance(string activityInstanceId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityInstanceId", activityInstanceId);
            ModificationOperations.Add(new ActivityInstanceCancellationCmd(ProcessInstanceId, activityInstanceId));
            return this;
        }

        public virtual IProcessInstanceModificationBuilder CancelTransitionInstance(string transitionInstanceId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "transitionInstanceId", transitionInstanceId);
            ModificationOperations.Add(new TransitionInstanceCancellationCmd(ProcessInstanceId, transitionInstanceId));
            return this;
        }

        public virtual IProcessInstanceModificationBuilder CancelAllForActivity(string activityId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityId", activityId);
            ModificationOperations.Add(new ActivityCancellationCmd(ProcessInstanceId, activityId));
            return this;
        }

        public virtual IProcessInstanceModificationInstantiationBuilder StartBeforeActivity(string activityId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityId", activityId);
            AbstractInstantiationCmd currentInstantiation = new ActivityBeforeInstantiationCmd(ProcessInstanceId,
                activityId);
            ModificationOperations.Add(currentInstantiation);
            return this;
        }

        public virtual IProcessInstanceModificationInstantiationBuilder StartBeforeActivity(string activityId,
            string ancestorActivityInstanceId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityId", activityId);
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "ancestorActivityInstanceId",
                ancestorActivityInstanceId);
            AbstractInstantiationCmd currentInstantiation = new ActivityBeforeInstantiationCmd(ProcessInstanceId,
                activityId, ancestorActivityInstanceId);
            ModificationOperations.Add(currentInstantiation);
            return this;
        }

        public virtual IProcessInstanceModificationInstantiationBuilder StartAfterActivity(string activityId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityId", activityId);
            AbstractInstantiationCmd currentInstantiation = new ActivityAfterInstantiationCmd(ProcessInstanceId,
                activityId);
            ModificationOperations.Add(currentInstantiation);
            return this;
        }

        public virtual IProcessInstanceModificationInstantiationBuilder StartAfterActivity(string activityId,
            string ancestorActivityInstanceId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityId", activityId);
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "ancestorActivityInstanceId",
                ancestorActivityInstanceId);
            AbstractInstantiationCmd currentInstantiation = new ActivityAfterInstantiationCmd(ProcessInstanceId,
                activityId, ancestorActivityInstanceId);
            ModificationOperations.Add(currentInstantiation);
            return this;
        }

        public virtual IProcessInstanceModificationInstantiationBuilder StartTransition(string transitionId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "transitionId", transitionId);
            AbstractInstantiationCmd currentInstantiation = new TransitionInstantiationCmd(ProcessInstanceId,
                transitionId);
            ModificationOperations.Add(currentInstantiation);
            return this;
        }

        public virtual IProcessInstanceModificationInstantiationBuilder StartTransition(string transitionId,
            string ancestorActivityInstanceId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "transitionId", transitionId);
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "ancestorActivityInstanceId",
                ancestorActivityInstanceId);
            AbstractInstantiationCmd currentInstantiation = new TransitionInstantiationCmd(ProcessInstanceId,
                transitionId, ancestorActivityInstanceId);
            ModificationOperations.Add(currentInstantiation);
            return this;
        }

        public virtual IProcessInstanceModificationInstantiationBuilder SetVariable(string name, object value)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "Variable name must not be null", "name", name);

            var currentInstantiation = CurrentInstantiation;
            if (currentInstantiation != null)
                currentInstantiation.AddVariable(name, value);
            else
                ProcessVariables.PutValue(name, value);

            return this;
        }

        public virtual IProcessInstanceModificationInstantiationBuilder SetVariableLocal(string name, object value)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "Variable name must not be null", "name", name);

            var currentInstantiation = CurrentInstantiation;
            if (currentInstantiation != null)
                currentInstantiation.AddVariableLocal(name, value);
            else
                ProcessVariables.PutValue(name, value);

            return this;
        }

        public virtual IProcessInstanceModificationInstantiationBuilder SetVariables(
            IDictionary<string, ITypedValue> variables)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "Variable map must not be null", "variables", variables);

            var currentInstantiation = CurrentInstantiation;
            if (currentInstantiation != null)
                currentInstantiation.AddVariables(variables);
            else
                foreach (var t in variables)
                    ProcessVariables.PutValue(t.Key, t.Value);
            return this;
        }

        public virtual IProcessInstanceModificationInstantiationBuilder SetVariablesLocal(
            IDictionary<string, ITypedValue> variables)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "Variable map must not be null", "variablesLocal",
                variables);

            var currentInstantiation = CurrentInstantiation;
            if (currentInstantiation != null)
                currentInstantiation.AddVariablesLocal(variables);
            else
                foreach (var t in variables)
                    ProcessVariables.PutValue(t.Key, t.Value);
            return this;
        }


        public virtual void Execute()
        {
            Execute(false, false);
        }

        public virtual void Execute(bool skipCustomListeners, bool skipIoMappings)
        {
            this.SkipCustomListeners = skipCustomListeners;
            this.SkipIoMappings = skipIoMappings;

            var cmd = new ModifyProcessInstanceCmd(this);
            if (CommandExecutor != null)
                CommandExecutor.Execute(cmd);
            else
                cmd.Execute(CommandContext);
        }
    }
}