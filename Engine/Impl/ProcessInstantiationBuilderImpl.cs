using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl
{

    /// <summary>
    ///     Simply wraps a modification builder because their API is equivalent.
    ///     
    /// </summary>
    public class ProcessInstantiationBuilderImpl : IProcessInstantiationBuilder
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        private readonly ICommandExecutor _commandExecutor;

        private ProcessInstantiationBuilderImpl(ICommandExecutor commandExecutor)
        {
            ModificationBuilder = new ProcessInstanceModificationBuilderImpl();

            _commandExecutor = commandExecutor;
        }

        public virtual string ProcessDefinitionId { get; private set; }

        public virtual string ProcessDefinitionKey { get; private set; }

        public virtual ProcessInstanceModificationBuilderImpl ModificationBuilder { get; }

        public virtual string BusinessKey { get; private set; }

        public virtual string CaseInstanceId { get; private set; }

        public virtual IDictionary<string, object> Variables
        {
            get { return ModificationBuilder.ProcessVariables; }
        }

        public virtual string ProcessDefinitionTenantId { get; private set; }

        public virtual bool IsTenantIdSet { get; private set; }

        public virtual IProcessInstantiationBuilder StartBeforeActivity(string activityId)
        {
            ModificationBuilder.StartBeforeActivity(activityId);
            return this;
        }

        public virtual IProcessInstantiationBuilder StartAfterActivity(string activityId)
        {
            ModificationBuilder.StartAfterActivity(activityId);
            return this;
        }

        public virtual IProcessInstantiationBuilder StartTransition(string transitionId)
        {
            ModificationBuilder.StartTransition(transitionId);
            return this;
        }

        public virtual IProcessInstantiationBuilder SetVariable(string name, object value)
        {
            ModificationBuilder.SetVariable(name, value);
            return this;
        }

        public virtual IProcessInstantiationBuilder SetVariableLocal(string name, object value)
        {
            ModificationBuilder.SetVariableLocal(name, value);
            return this;
        }

        public virtual IProcessInstantiationBuilder SetVariables(IDictionary<string, ITypedValue> variables)
        {
            if (variables != null)
            {
                ModificationBuilder.SetVariablesLocal(variables);
            }
            return this;
        }

        /// <summary>
        ///     If an instruction is submitted before then all local variables are set when
        ///     the instruction is executed. Otherwise, the variables are set on the
        ///     process instance itself.
        /// </summary>
        public virtual IProcessInstantiationBuilder SetVariablesLocal(IDictionary<string, ITypedValue> variables)
        {
            if (variables != null)
            {
                ModificationBuilder.SetVariablesLocal(variables);
            }
            return this;
        }

        public virtual IProcessInstantiationBuilder SetBusinessKey(string businessKey)
        {
            BusinessKey = businessKey;
            return this;
        }

        public virtual IProcessInstantiationBuilder SetCaseInstanceId(string caseInstanceId)
        {
            CaseInstanceId = caseInstanceId;
            return this;
        }

        public virtual IProcessInstantiationBuilder SetProcessDefinitionTenantId(string tenantId)
        {
            ProcessDefinitionTenantId = tenantId;
            IsTenantIdSet = true;
            return this;
        }

        public virtual IProcessInstantiationBuilder ProcessDefinitionWithoutTenantId()
        {
            ProcessDefinitionTenantId = null;
            IsTenantIdSet = true;
            return this;
        }

        public virtual IProcessInstance Execute()
        {
            return Execute(false, false);
        }

        public virtual IProcessInstance Execute(bool skipCustomListeners, bool skipIoMappings)
        {
            return ExecuteWithVariablesInReturn(skipCustomListeners, skipIoMappings);
        }

        public virtual IProcessInstanceWithVariables ExecuteWithVariablesInReturn()
        {
            return ExecuteWithVariablesInReturn(false, false);
        }

        public virtual IProcessInstanceWithVariables ExecuteWithVariablesInReturn(bool skipCustomListeners, bool skipIoMappings)
        {
            EnsureUtil.EnsureOnlyOneNotNull("either process definition id or key must be set", ProcessDefinitionId, ProcessDefinitionKey);

            if (IsTenantIdSet && !ReferenceEquals(ProcessDefinitionId, null))
                throw Log.ExceptionStartProcessInstanceByIdAndTenantId();

            ICommand<IProcessInstanceWithVariables> command;

            if (ModificationBuilder.ModificationOperations.Count == 0)
            {
                if (skipCustomListeners || skipIoMappings)
                    throw Log.ExceptionStartProcessInstanceAtStartActivityAndSkipListenersOrMapping();
                // start at the default start activity
                command = new StartProcessInstanceCmd(this);
            }
            else
            {
                // start at any activity using the instructions
                ModificationBuilder.SkipCustomListeners = skipCustomListeners;
                ModificationBuilder.SkipIoMappings = skipIoMappings;

                command = new StartProcessInstanceAtActivitiesCmd(this);
            }

            return _commandExecutor.Execute(command);
        }

        public static IProcessInstantiationBuilder CreateProcessInstanceById(ICommandExecutor commandExecutor,
            string processDefinitionId)
        {
            var builder = new ProcessInstantiationBuilderImpl(commandExecutor);
            builder.ProcessDefinitionId = processDefinitionId;
            return builder;
        }

        public static IProcessInstantiationBuilder CreateProcessInstanceByKey(ICommandExecutor commandExecutor,
            string processDefinitionKey)
        {
            var builder = new ProcessInstantiationBuilderImpl(commandExecutor);
            builder.ProcessDefinitionKey = processDefinitionKey;
            return builder;
        }
    }
}