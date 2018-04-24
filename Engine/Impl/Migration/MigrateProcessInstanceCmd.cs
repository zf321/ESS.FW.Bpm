using System;
using System.Collections.Generic;
using System.Threading;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.migration.instance.parser;
using ESS.FW.Bpm.Engine.Impl.migration.validation.instance;
using ESS.FW.Bpm.Engine.Impl.tree;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.Util.EnsureUtil.ensureNotContainsNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.Util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.Util.EnsureUtil.ensureNotNull;

    /// <summary>
    ///     How migration works:
    ///     <ol>
    ///         <li>
    ///             Validate migration instructions.
    ///             <li>
    ///                 Delete activity instances that are not going to be migrated, invoking execution listeners
    ///                 and io mappings. This is performed in a bottom-up fashion in the activity instance tree and ensures
    ///                 that the "upstream" tree is always consistent with respect to the old process definition.
    ///                 <li>
    ///                     Migrate and create activity instances. Creation invokes execution listeners
    ///                     and io mappings. This is performed in a top-down fashion in the activity instance tree and
    ///                     ensures that the "upstream" tree is always consistent with respect to the new process definition.
    ///     </ol>
    ///     
    /// </summary>
    public class MigrateProcessInstanceCmd : AbstractMigrationCmd<object>
    {
        protected internal static readonly MigrationLogger Logger = ProcessEngineLogger.MigrationLogger;

        protected internal bool WriteOperationLog;

        public MigrateProcessInstanceCmd(MigrationPlanExecutionBuilderImpl migrationPlanExecutionBuilder,
            bool writeOperationLog) : base(migrationPlanExecutionBuilder)
        {
            this.WriteOperationLog = writeOperationLog;
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
        public override object Execute(CommandContext commandContext)
        {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.migration.MigrationPlan migrationPlan = executionBuilder.getMigrationPlan();
            var migrationPlan = ExecutionBuilder.MigrationPlan;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.Util.Collection<String> processInstanceIds = collectProcessInstanceIds(commandContext);
            var processInstanceIds = CollectProcessInstanceIds(commandContext);

            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Migration plan cannot be null", "migration plan",
                migrationPlan);
            //EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "Process instance ids cannot empty", "process instance ids", processInstanceIds);
            EnsureUtil.EnsureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null",
                "process instance ids", processInstanceIds);

            var sourceDefinition = ResolveSourceProcessDefinition(commandContext);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity targetDefinition = resolveTargetProcessDefinition(commandContext);
            var targetDefinition = ResolveTargetProcessDefinition(commandContext);

            CheckAuthorizations(commandContext, sourceDefinition, targetDefinition, processInstanceIds);
            if (WriteOperationLog)
                WriteUserOperationLog(commandContext, sourceDefinition, targetDefinition, processInstanceIds.Count,
                    false);

            //commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext, migrationPlan, processInstanceIds, targetDefinition));

            return null;
        }

        public virtual void MigrateProcessInstance(CommandContext commandContext, string processInstanceId,
            IMigrationPlan migrationPlan, ProcessDefinitionEntity targetProcessDefinition)
        {
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Process instance id cannot be null",
                "process instance id", processInstanceId);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity processInstance = commandContext.getExecutionManager().findExecutionById(processInstanceId);
            //ExecutionEntity processInstance = commandContext.ExecutionManager.findExecutionById(processInstanceId);

            //ensureProcessInstanceExist(processInstanceId, processInstance);
            //ensureOperationAllowed(commandContext, processInstance, targetProcessDefinition);
            //ensureSameProcessDefinition(processInstance, migrationPlan.SourceProcessDefinitionId);

            var processInstanceReport = new MigratingProcessInstanceValidationReportImpl();

            // Initialize migration: match migration instructions to activity instances and collect required entities
            var migratingInstanceParser = new MigratingInstanceParser(Context.ProcessEngineConfiguration.ProcessEngine);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.migration.instance.MigratingProcessInstance migratingProcessInstance = migratingInstanceParser.parse(processInstance.getId(), migrationPlan, processInstanceReport);
            //MigratingProcessInstance migratingProcessInstance = migratingInstanceParser.parse(processInstance.Id, migrationPlan, processInstanceReport);

            //validateInstructions(commandContext, migratingProcessInstance, processInstanceReport);

            if (processInstanceReport.HasFailures())
                throw Logger.FailingMigratingProcessInstanceValidation(processInstanceReport);

            //executeInContext(() => { deleteUnmappedActivityInstances(migratingProcessInstance); },
            //    migratingProcessInstance.SourceDefinition);

            //executeInContext(() => { migrateProcessInstance(migratingProcessInstance); },
            //    migratingProcessInstance.TargetDefinition);

            //return null;
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected <T> void executeInContext(final Runnable runnable, org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity contextDefinition)
        protected internal virtual void ExecuteInContext<T>(ThreadStart runnable,
            ProcessDefinitionEntity contextDefinition)
        {
            //ProcessApplicationReference processApplication =
            //    ProcessApplicationContextUtil.getTargetProcessApplication(contextDefinition);
            //if (ProcessApplicationContextUtil.requiresContextSwitch(processApplication))
            //{
            //    Context.executeWithinProcessApplication(new CallableAnonymousInnerClass2(this, runnable),
            //        processApplication);
            //}
            //else
            //{
            //    runnable.run();
            //}
        }

        /// <summary>
        ///     delete unmapped instances in a bottom-up fashion (similar to deleteCascade and regular BPMN execution)
        /// </summary>
        protected internal virtual void DeleteUnmappedActivityInstances(
            MigratingProcessInstance migratingProcessInstance)
        {
            throw new NotImplementedException();
//            var leafInstances = CollectLeafInstances(migratingProcessInstance);
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final org.camunda.bpm.engine.impl.migration.instance.DeleteUnmappedInstanceVisitor visitor = new org.camunda.bpm.engine.impl.migration.instance.DeleteUnmappedInstanceVisitor(executionBuilder.isSkipCustomListeners(), executionBuilder.isSkipIoMappings());
//            var visitor = new DeleteUnmappedInstanceVisitor(ExecutionBuilder.SkipCustomListeners,
//                ExecutionBuilder.SkipIoMappings);

//            foreach (var leafInstance in leafInstances)
//            {
//                var walker = new MigratingScopeInstanceBottomUpWalker(leafInstance);

//                //walker.addPreVisitor(visitor);

//                //walker.walkUntil(new WalkConditionAnonymousInnerClass(this, visitor));
//            }
        }

        protected internal virtual ISet<MigratingScopeInstance> CollectLeafInstances(
            MigratingProcessInstance migratingProcessInstance)
        {
            ISet<MigratingScopeInstance> leafInstances = new HashSet<MigratingScopeInstance>();

            foreach (var migratingScopeInstance in migratingProcessInstance.MigratingScopeInstances)
                if (migratingScopeInstance.ChildScopeInstances.Count == 0)
                    leafInstances.Add(migratingScopeInstance);

            return leafInstances;
        }

        protected internal virtual void ValidateInstructions(CommandContext commandContext,
            MigratingProcessInstance migratingProcessInstance,
            MigratingProcessInstanceValidationReportImpl processInstanceReport)
        {
            var migratingActivityInstanceValidators =
                commandContext.ProcessEngineConfiguration.MigratingActivityInstanceValidators;
            var migratingTransitionInstanceValidators =
                commandContext.ProcessEngineConfiguration.MigratingTransitionInstanceValidators;
            var migratingCompensationInstanceValidators =
                commandContext.ProcessEngineConfiguration.MigratingCompensationInstanceValidators;

            IDictionary<MigratingActivityInstance, MigratingActivityInstanceValidationReportImpl> instanceReports =
                new Dictionary<MigratingActivityInstance, MigratingActivityInstanceValidationReportImpl>();

            foreach (var migratingActivityInstance in migratingProcessInstance.MigratingActivityInstances)
            {
                var instanceReport = ValidateActivityInstance(migratingActivityInstance, migratingProcessInstance,
                    migratingActivityInstanceValidators);
                instanceReports[migratingActivityInstance] = instanceReport;
            }

            foreach (var migratingEventScopeInstance in migratingProcessInstance.MigratingEventScopeInstances)
            {
                var ancestorInstance = migratingEventScopeInstance.ClosestAncestorActivityInstance;

                ValidateEventScopeInstance(migratingEventScopeInstance, migratingProcessInstance,
                    migratingCompensationInstanceValidators, instanceReports[ancestorInstance]);
            }

            foreach (
                var migratingEventSubscriptionInstance in
                migratingProcessInstance.MigratingCompensationSubscriptionInstances)
            {
                var ancestorInstance = migratingEventSubscriptionInstance.ClosestAncestorActivityInstance;

                ValidateCompensateSubscriptionInstance(migratingEventSubscriptionInstance, migratingProcessInstance,
                    migratingCompensationInstanceValidators, instanceReports[ancestorInstance]);
            }

            foreach (var instanceReport in instanceReports.Values)
                if (instanceReport.HasFailures())
                    processInstanceReport.AddActivityInstanceReport(instanceReport);

            foreach (var migratingTransitionInstance in migratingProcessInstance.MigratingTransitionInstances)
            {
                var instanceReport = ValidateTransitionInstance(migratingTransitionInstance, migratingProcessInstance,
                    migratingTransitionInstanceValidators);
                if (instanceReport.HasFailures())
                    processInstanceReport.AddTransitionInstanceReport(instanceReport);
            }
        }

        protected internal virtual MigratingActivityInstanceValidationReportImpl ValidateActivityInstance(
            MigratingActivityInstance migratingActivityInstance, MigratingProcessInstance migratingProcessInstance,
            IList<IMigratingActivityInstanceValidator> migratingActivityInstanceValidators)
        {
            var instanceReport = new MigratingActivityInstanceValidationReportImpl(migratingActivityInstance);
            foreach (var migratingActivityInstanceValidator in migratingActivityInstanceValidators)
                migratingActivityInstanceValidator.Validate(migratingActivityInstance, migratingProcessInstance,
                    instanceReport);
            return instanceReport;
        }

        protected internal virtual MigratingTransitionInstanceValidationReportImpl ValidateTransitionInstance(
            MigratingTransitionInstance migratingTransitionInstance, MigratingProcessInstance migratingProcessInstance,
            IList<IMigratingTransitionInstanceValidator> migratingTransitionInstanceValidators)
        {
            var instanceReport = new MigratingTransitionInstanceValidationReportImpl(migratingTransitionInstance);
            foreach (var migratingTransitionInstanceValidator in migratingTransitionInstanceValidators)
                migratingTransitionInstanceValidator.Validate(migratingTransitionInstance, migratingProcessInstance,
                    instanceReport);
            return instanceReport;
        }

        protected internal virtual void ValidateEventScopeInstance(MigratingEventScopeInstance eventScopeInstance,
            MigratingProcessInstance migratingProcessInstance,
            IList<IMigratingCompensationInstanceValidator> migratingTransitionInstanceValidators,
            MigratingActivityInstanceValidationReportImpl instanceReport)
        {
            foreach (var validator in migratingTransitionInstanceValidators)
                validator.Validate(eventScopeInstance, migratingProcessInstance, instanceReport);
        }

        protected internal virtual void ValidateCompensateSubscriptionInstance(
            MigratingCompensationEventSubscriptionInstance eventSubscriptionInstance,
            MigratingProcessInstance migratingProcessInstance,
            IList<IMigratingCompensationInstanceValidator> migratingTransitionInstanceValidators,
            MigratingActivityInstanceValidationReportImpl instanceReport)
        {
            foreach (var validator in migratingTransitionInstanceValidators)
                validator.Validate(eventSubscriptionInstance, migratingProcessInstance, instanceReport);
        }

        /// <summary>
        ///     Migrate activity instances to their new activities and process definition. Creates new
        ///     scope instances as necessary.
        /// </summary>
        protected internal virtual void MigrateProcessInstance(MigratingProcessInstance migratingProcessInstance)
        {
            var rootActivityInstance = migratingProcessInstance.RootInstance;

            var walker = new MigratingProcessElementInstanceTopDownWalker(rootActivityInstance);

            //walker.addPreVisitor(new MigratingActivityInstanceVisitor(executionBuilder.SkipCustomListeners,
            //    executionBuilder.SkipIoMappings));
            //walker.addPreVisitor(new MigrationCompensationInstanceVisitor());

            //walker.walkUntil();
        }

        protected internal virtual void EnsureProcessInstanceExist(string processInstanceId,
            ExecutionEntity processInstance)
        {
            if (processInstance == null)
                throw Logger.ProcessInstanceDoesNotExist(processInstanceId);
        }

        protected internal virtual void EnsureSameProcessDefinition(ExecutionEntity processInstance,
            string processDefinitionId)
        {
            if (!processDefinitionId.Equals(processInstance.ProcessDefinitionId))
            {
                //throw LOGGER.processDefinitionOfInstanceDoesNotMatchMigrationPlan(processInstance, processDefinitionId);
            }
        }

        protected internal virtual void EnsureOperationAllowed(CommandContext commandContext,
            ExecutionEntity processInstance, ProcessDefinitionEntity targetProcessDefinition)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                //checker.checkMigrateProcessInstance(processInstance, targetProcessDefinition);
            }
        }

        private class CallableAnonymousInnerClass /*: Callable<object>*/
        {
            private readonly CommandContext _commandContext;
            private readonly IMigrationPlan _migrationPlan;
            private readonly MigrateProcessInstanceCmd _outerInstance;
            private readonly ICollection<string> _processInstanceIds;
            private readonly ProcessDefinitionEntity _targetDefinition;

            public CallableAnonymousInnerClass(MigrateProcessInstanceCmd outerInstance, CommandContext commandContext,
                IMigrationPlan migrationPlan, ICollection<string> processInstanceIds,
                ProcessDefinitionEntity targetDefinition)
            {
                this._outerInstance = outerInstance;
                this._commandContext = commandContext;
                this._migrationPlan = migrationPlan;
                this._processInstanceIds = processInstanceIds;
                this._targetDefinition = targetDefinition;
            }


//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void call() throws Exception
            public void Call()
            {
                foreach (var processInstanceId in _processInstanceIds)
                    _outerInstance.MigrateProcessInstance(_commandContext, processInstanceId, _migrationPlan,
                        _targetDefinition);
                //return null;
            }
        }

        private class CallableAnonymousInnerClass2 /*: Callable<object>*/
        {
            private readonly MigrateProcessInstanceCmd _outerInstance;

            private readonly ThreadStart _runnable;

            public CallableAnonymousInnerClass2(MigrateProcessInstanceCmd outerInstance, ThreadStart runnable)
            {
                this._outerInstance = outerInstance;
                this._runnable = runnable;
            }


//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void call() throws Exception
            public void Call()
            {
                //runnable.run();
                //return null;
            }
        }

        //private class WalkConditionAnonymousInnerClass :IWalkCondition<MigratingScopeInstance>
        //{
        //    private readonly MigrateProcessInstanceCmd _outerInstance;

        //    private readonly DeleteUnmappedInstanceVisitor _visitor;

        //    public WalkConditionAnonymousInnerClass(MigrateProcessInstanceCmd outerInstance,
        //        DeleteUnmappedInstanceVisitor visitor)
        //    {
        //        this._outerInstance = outerInstance;
        //        this._visitor = visitor;
        //    }


        //    public virtual bool IsFulfilled(MigratingScopeInstance element)
        //    {
        //        // walk until top of instance tree is reached or until
        //        // a node is reached for which we have not yet visited every child
        //        return (element == null) || !_visitor.HasVisitedAll(element.ChildScopeInstances);
        //    }
        //}
    }
}