using System;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.migration.instance.parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Impl.Pvm;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    public class CallActivityBehavior : CallableElementActivityBehavior, IMigrationObserverBehavior
    {
        public CallActivityBehavior()
        {
        }


        public CallActivityBehavior(string className) : base(className)
        {
        }

        public CallActivityBehavior(IExpression expression) : base(expression)
        {
        }

        public virtual void MigrateScope(IActivityExecution scopeExecution)
        {
        }

        public virtual void OnParseMigratingInstance(MigratingInstanceParseContext parseContext, MigratingActivityInstance migratingInstance)
        {

            ActivityImpl callActivity = (ActivityImpl)migratingInstance.SourceScope;

            // A call activity is typically scope and since we guarantee stability of scope executions during migration,
            // the superExecution link does not have to be maintained during migration.
            // There are some exceptions, though: A multi-instance call activity is not scope and therefore
            // does not have a dedicated scope execution. In this case, the link to the super execution
            // must be maintained throughout migration
            if (!callActivity.IsScope)
            {
                ExecutionEntity callActivityExecution = migratingInstance.ResolveRepresentativeExecution();
                ExecutionEntity calledProcessInstance = callActivityExecution.SubProcessInstance as ExecutionEntity;
                migratingInstance.AddMigratingDependentInstance(new MigratingCalledProcessInstance(calledProcessInstance));
            }
        }

        protected internal override void StartInstance(IActivityExecution execution, IVariableMap variables, string businessKey)
        {
            ProcessDefinitionImpl definition = CallableElementUtil.GetProcessDefinitionToCall(execution, CallableElement);
            IPvmProcessInstance processInstance = execution.CreateSubProcessInstance(definition, businessKey);
            processInstance.Start(variables);
        }
    }
}