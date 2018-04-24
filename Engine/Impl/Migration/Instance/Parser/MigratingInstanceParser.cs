using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.migration.validation.instance;
using ESS.FW.Bpm.Engine.Impl.tree;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     Builds a <seealso cref="MigratingProcessInstance" />, a data structure that contains meta-data for the activity
    ///     instances that are migrated.
    ///     
    /// </summary>
    public class MigratingInstanceParser
    {
        protected internal IMigratingInstanceParseHandler<IActivityInstance> activityInstanceHandler =
            new ActivityInstanceHandler();

        protected internal IMigratingInstanceParseHandler<EventSubscriptionEntity> CompensationInstanceHandler =
            new CompensationInstanceHandler();

        protected internal IMigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<JobEntity>>
            dependentActivityInstanceJobHandler = new ActivityInstanceJobHandler();

        protected internal
            IMigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<EventSubscriptionEntity>>
            dependentEventSubscriptionHandler = new EventSubscriptionInstanceHandler();

        protected internal IMigratingDependentInstanceParseHandler<MigratingTransitionInstance, IList<JobEntity>>
            dependentTransitionInstanceJobHandler = new TransitionInstanceJobHandler();

        protected internal
            IMigratingDependentInstanceParseHandler<MigratingProcessElementInstance, IList<VariableInstanceEntity>>
            DependentVariableHandler = new VariableInstanceHandler();

        protected internal IProcessEngine Engine;
        protected internal IMigratingInstanceParseHandler<IncidentEntity> incidentHandler = new IncidentInstanceHandler();

        protected internal IMigratingInstanceParseHandler<ITransitionInstance> transitionInstanceHandler =
            new TransitionInstanceHandler();

        public MigratingInstanceParser(IProcessEngine engine)
        {
            this.Engine = engine;
        }

        public virtual IMigratingInstanceParseHandler<IActivityInstance> ActivityInstanceHandler
        {
            get { return activityInstanceHandler; }
        }

        public virtual IMigratingInstanceParseHandler<ITransitionInstance> TransitionInstanceHandler
        {
            get { return transitionInstanceHandler; }
        }

        public virtual IMigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<EventSubscriptionEntity>>
            DependentEventSubscriptionHandler
        {
            get { return dependentEventSubscriptionHandler; }
        }

        public virtual IMigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<JobEntity>>
            DependentActivityInstanceJobHandler
        {
            get { return dependentActivityInstanceJobHandler; }
        }

        public virtual IMigratingDependentInstanceParseHandler<MigratingTransitionInstance, IList<JobEntity>>
            DependentTransitionInstanceJobHandler
        {
            get { return dependentTransitionInstanceJobHandler; }
        }

        public virtual IMigratingInstanceParseHandler<IncidentEntity> IncidentHandler
        {
            get { return incidentHandler; }
        }

        public virtual
            IMigratingDependentInstanceParseHandler<MigratingProcessElementInstance, IList<VariableInstanceEntity>>
            DependentVariablesHandler
        {
            get { return DependentVariableHandler; }
        }

        public virtual MigratingProcessInstance Parse(string processInstanceId, IMigrationPlan migrationPlan,
            MigratingProcessInstanceValidationReportImpl processInstanceReport)
        {
            var commandContext = Context.CommandContext;
            var eventSubscriptions = FetchEventSubscriptions(commandContext, processInstanceId);
            var executions = FetchExecutions(commandContext, processInstanceId);
            var externalTasks = FetchExternalTasks(commandContext, processInstanceId);
            var incidents = FetchIncidents(commandContext, processInstanceId);
            var jobs = FetchJobs(commandContext, processInstanceId);
            var tasks = FetchTasks(commandContext, processInstanceId);
            var variables = FetchVariables(commandContext, processInstanceId);

            //ExecutionEntity processInstance = commandContext.ExecutionManager.findExecutionById(processInstanceId);
            //processInstance.restoreProcessInstance(executions, eventSubscriptions, variables, tasks, jobs, incidents, externalTasks);

            //ProcessDefinitionEntity targetProcessDefinition = Context.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(migrationPlan.TargetProcessDefinitionId);
            //IList<JobDefinitionEntity> targetJobDefinitions = fetchJobDefinitions(commandContext, targetProcessDefinition.Id);

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final MigratingInstanceParseContext parseContext = new MigratingInstanceParseContext(this, migrationPlan, processInstance, targetProcessDefinition).eventSubscriptions(eventSubscriptions).externalTasks(externalTasks).incidents(incidents).jobs(jobs).tasks(tasks).targetJobDefinitions(targetJobDefinitions).variables(variables);
            //MigratingInstanceParseContext parseContext = (new MigratingInstanceParseContext(this, migrationPlan, processInstance, targetProcessDefinition)).eventSubscriptions(eventSubscriptions).externalTasks(externalTasks).incidents(incidents).jobs(jobs).tasks(tasks).targetJobDefinitions(targetJobDefinitions).variables(variables);

            var activityInstance = Engine.RuntimeService.GetActivityInstance(processInstanceId);

            var activityInstanceWalker = new ActivityInstanceWalker(activityInstance);

            //activityInstanceWalker.addPreVisitor(new TreeVisitorAnonymousInnerClass(this, parseContext));

            activityInstanceWalker.WalkWhile();

            //CompensationEventSubscriptionWalker compensateSubscriptionsWalker = new CompensationEventSubscriptionWalker(parseContext.MigratingActivityInstances);

            //compensateSubscriptionsWalker.addPreVisitor(new TreeVisitorAnonymousInnerClass2(this, parseContext));

            //compensateSubscriptionsWalker.walkWhile();

            //foreach (IncidentEntity incidentEntity in incidents)
            //{
            //    incidentHandler.handle(parseContext, incidentEntity);
            //}

            //parseContext.validateNoEntitiesLeft(processInstanceReport);

            //return parseContext.MigratingProcessInstance;
            return null;
        }

        protected internal virtual IList<ExecutionEntity> FetchExecutions(CommandContext commandContext,
            string processInstanceId)
        {
            //return commandContext.ExecutionManager.findExecutionsByProcessInstanceId(processInstanceId);
            return null;
        }

        protected internal virtual IList<EventSubscriptionEntity> FetchEventSubscriptions(CommandContext commandContext,
            string processInstanceId)
        {
            //return commandContext.EventSubscriptionManager.findEventSubscriptionsByProcessInstanceId(processInstanceId);
            return null;
        }

        protected internal virtual IList<ExternalTaskEntity> FetchExternalTasks(CommandContext commandContext,
            string processInstanceId)
        {
            //return commandContext.ExternalTaskManager.findExternalTasksByProcessInstanceId(processInstanceId);
            return null;
        }

        protected internal virtual IList<JobEntity> FetchJobs(CommandContext commandContext, string processInstanceId)
        {
            //return commandContext.JobManager.findJobsByProcessInstanceId(processInstanceId);
            return null;
        }

        protected internal virtual IList<IncidentEntity> FetchIncidents(CommandContext commandContext,
            string processInstanceId)
        {
            //return commandContext.IncidentManager.findIncidentsByProcessInstance(processInstanceId);
            return null;
        }

        protected internal virtual IList<TaskEntity> FetchTasks(CommandContext commandContext, string processInstanceId)
        {
            //return commandContext.TaskManager.findTasksByProcessInstanceId(processInstanceId);
            return null;
        }

        protected internal virtual IList<JobDefinitionEntity> FetchJobDefinitions(CommandContext commandContext,
            string processDefinitionId)
        {
            //return commandContext.JobDefinitionManager.findByProcessDefinitionId(processDefinitionId);
            return null;
        }

        protected internal virtual IList<VariableInstanceEntity> FetchVariables(CommandContext commandContext,
            string processInstanceId)
        {
            //return commandContext.VariableInstanceManager.findVariableInstancesByProcessInstanceId(processInstanceId);
            return null;
        }

        private class TreeVisitorAnonymousInnerClass : ITreeVisitor<IActivityInstance>
        {
            private readonly MigratingInstanceParser _outerInstance;

            private readonly MigratingInstanceParseContext _parseContext;

            public TreeVisitorAnonymousInnerClass(MigratingInstanceParser outerInstance,
                MigratingInstanceParseContext parseContext)
            {
                this._outerInstance = outerInstance;
                this._parseContext = parseContext;
            }

            public virtual void Visit(IActivityInstance obj)
            {
                _outerInstance.activityInstanceHandler.Handle(_parseContext, obj);
            }
        }

        private class TreeVisitorAnonymousInnerClass2 : ITreeVisitor<EventSubscriptionEntity>
        {
            private readonly MigratingInstanceParser _outerInstance;

            private readonly MigratingInstanceParseContext _parseContext;

            public TreeVisitorAnonymousInnerClass2(MigratingInstanceParser outerInstance,
                MigratingInstanceParseContext parseContext)
            {
                this._outerInstance = outerInstance;
                this._parseContext = parseContext;
            }

            public virtual void Visit(EventSubscriptionEntity obj)
            {
                _outerInstance.CompensationInstanceHandler.Handle(_parseContext, obj);
            }
        }
    }
}