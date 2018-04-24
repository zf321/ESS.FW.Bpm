using System;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.migration.instance.parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Implementation to create a new <seealso cref="ICaseInstance" /> using the BPMN 2.0 call activity
    ///     
    /// </summary>
    public class CaseCallActivityBehavior : CallableElementActivityBehavior, IMigrationObserverBehavior
    {
        public virtual void MigrateScope(IActivityExecution scopeExecution)
        {
        }

        public virtual void OnParseMigratingInstance(MigratingInstanceParseContext parseContext,
            MigratingActivityInstance migratingInstance)
        {
            var callActivity = (ActivityImpl)migratingInstance.SourceScope;

            // A call activity is typically scope and since we guarantee stability of scope executions during migration,
            // the superExecution link does not have to be maintained during migration.
            // There are some exceptions, though: A multi-instance call activity is not scope and therefore
            // does not have a dedicated scope execution. In this case, the link to the super execution
            // must be maintained throughout migration
            if (!callActivity.IsScope)
            {
                var callActivityExecution = migratingInstance.ResolveRepresentativeExecution();
                //CaseExecutionEntity calledCaseInstance = callActivityExecution.getSubCaseInstance();
                //migratingInstance.addMigratingDependentInstance(new MigratingCalledCaseInstance(calledCaseInstance));
            }
        }

        protected internal override void StartInstance(IActivityExecution execution, IVariableMap variables,
            string businessKey)
        {
            throw new NotImplementedException();
            //CmmnCaseDefinition definition = getCaseDefinitionToCall(execution, CallableElement);
            //CmmnCaseInstance caseInstance = execution.createSubCaseInstance(definition, businessKey);
            //caseInstance.create(variables);
        }
    }
}