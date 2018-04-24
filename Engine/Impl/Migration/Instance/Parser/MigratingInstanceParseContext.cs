using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.migration.validation.instance;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingInstanceParseContext
    {
        protected internal IDictionary<string, MigratingActivityInstance> ActivityInstances =
            new Dictionary<string, MigratingActivityInstance>();

        protected internal IDictionary<string, MigratingEventScopeInstance> CompensationInstances =
            new Dictionary<string, MigratingEventScopeInstance>();

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal ICollection<EventSubscriptionEntity> EventSubscriptionsRenamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal ICollection<ExternalTaskEntity> ExternalTasksRenamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal ICollection<IncidentEntity> IncidentsRenamed;
        protected internal IDictionary<string, IList<IMigrationInstruction>> InstructionsBySourceScope;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal ICollection<JobEntity> JobsRenamed;
        protected internal ActivityExecutionTreeMapping mapping;

        protected internal IDictionary<string, MigratingExternalTaskInstance> MigratingExternalTasks =
            new Dictionary<string, MigratingExternalTaskInstance>();

        protected internal IDictionary<string, MigratingJobInstance> MigratingJobs =
            new Dictionary<string, MigratingJobInstance>();

        protected internal MigratingProcessInstance migratingProcessInstance;

        protected internal MigratingInstanceParser Parser;

        protected internal ProcessDefinitionEntity sourceProcessDefinition;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDictionary<string, IList<JobDefinitionEntity>> TargetJobDefinitionsRenamed;
        protected internal ProcessDefinitionEntity targetProcessDefinition;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal ICollection<TaskEntity> TasksRenamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal ICollection<VariableInstanceEntity> VariablesRenamed;

        public MigratingInstanceParseContext(MigratingInstanceParser parser, IMigrationPlan migrationPlan,
            ExecutionEntity processInstance, ProcessDefinitionEntity targetProcessDefinition)
        {
            this.Parser = parser;
            sourceProcessDefinition = processInstance.GetProcessDefinition();//.ProcessDefinition;
            this.targetProcessDefinition = targetProcessDefinition;
            migratingProcessInstance = new MigratingProcessInstance(processInstance.Id, sourceProcessDefinition,
                targetProcessDefinition);
            mapping = new ActivityExecutionTreeMapping(context.Impl.Context.CommandContext, processInstance.Id);
            InstructionsBySourceScope = OrganizeInstructionsBySourceScope(migrationPlan);
        }

        public virtual MigratingProcessInstance MigratingProcessInstance
        {
            get { return migratingProcessInstance; }
        }

        public virtual ICollection<MigratingActivityInstance> MigratingActivityInstances
        {
            get { return ActivityInstances.Values; }
        }

        public virtual ProcessDefinitionImpl SourceProcessDefinition
        {
            get
            {
                return null;
                //return sourceProcessDefinition;
            }
        }

        public virtual ProcessDefinitionImpl TargetProcessDefinition
        {
            get
            {
                return null;
                //return targetProcessDefinition;
            }
        }

        public virtual ActivityExecutionTreeMapping Mapping
        {
            get { return mapping; }
        }

        public virtual MigratingInstanceParseContext Jobs(ICollection<JobEntity> jobs)
        {
            JobsRenamed = new HashSet<JobEntity>(jobs);
            return this;
        }

        public virtual MigratingInstanceParseContext Incidents(ICollection<IncidentEntity> incidents)
        {
            IncidentsRenamed = new HashSet<IncidentEntity>(incidents);
            return this;
        }

        public virtual MigratingInstanceParseContext Tasks(ICollection<TaskEntity> tasks)
        {
            TasksRenamed = new HashSet<TaskEntity>(tasks);
            return this;
        }

        public virtual MigratingInstanceParseContext ExternalTasks(ICollection<ExternalTaskEntity> externalTasks)
        {
            ExternalTasksRenamed = new HashSet<ExternalTaskEntity>(externalTasks);
            return this;
        }

        public virtual MigratingInstanceParseContext EventSubscriptions(
            ICollection<EventSubscriptionEntity> eventSubscriptions)
        {
            EventSubscriptionsRenamed = new HashSet<EventSubscriptionEntity>(eventSubscriptions);
            return this;
        }

        public virtual MigratingInstanceParseContext TargetJobDefinitions(
            ICollection<JobDefinitionEntity> jobDefinitions)
        {
            TargetJobDefinitionsRenamed = new Dictionary<string, IList<JobDefinitionEntity>>();

            foreach (var jobDefinition in jobDefinitions)
                CollectionUtil.AddToMapOfLists(TargetJobDefinitionsRenamed, jobDefinition.ActivityId, jobDefinition);
            return this;
        }

        public virtual MigratingInstanceParseContext Variables(ICollection<VariableInstanceEntity> variables)
        {
            VariablesRenamed = new HashSet<VariableInstanceEntity>(variables);
            return this;
        }

        public virtual void Submit(MigratingActivityInstance activityInstance)
        {
            ActivityInstances[activityInstance.ActivityInstance.Id] = activityInstance;
        }

        public virtual void Submit(MigratingEventScopeInstance compensationInstance)
        {
            var scopeExecution = compensationInstance.ResolveRepresentativeExecution();
            if (scopeExecution != null)
                CompensationInstances[scopeExecution.Id] = compensationInstance;
        }

        public virtual void Submit(MigratingJobInstance job)
        {
            MigratingJobs[job.JobEntity.Id] = job;
        }

        public virtual void Submit(MigratingExternalTaskInstance externalTask)
        {
            MigratingExternalTasks[externalTask.Id] = externalTask;
        }

        public virtual void Consume(TaskEntity task)
        {
            TasksRenamed.Remove(task);
        }

        public virtual void Consume(ExternalTaskEntity externalTask)
        {
            ExternalTasksRenamed.Remove(externalTask);
        }

        public virtual void Consume(IncidentEntity incident)
        {
            IncidentsRenamed.Remove(incident);
        }

        public virtual void Consume(JobEntity job)
        {
            JobsRenamed.Remove(job);
        }

        public virtual void Consume(EventSubscriptionEntity eventSubscription)
        {
            EventSubscriptionsRenamed.Remove(eventSubscription);
        }

        public virtual void Consume(VariableInstanceEntity variableInstance)
        {
            VariablesRenamed.Remove(variableInstance);
        }

        public virtual ActivityImpl GetTargetActivity(IMigrationInstruction instruction)
        {
            if (instruction != null)
                return null;
            return null;
        }

        public virtual JobDefinitionEntity GetTargetJobDefinition(string activityId, string jobHandlerType)
        {
            var jobDefinitionsForActivity = TargetJobDefinitionsRenamed[activityId];

            if (jobDefinitionsForActivity != null)
                foreach (var jobDefinition in jobDefinitionsForActivity)
                    if (jobHandlerType.Equals(jobDefinition.JobType))
                        return jobDefinition;

            return null;
        }

        // TODO: conditions would go here
        public virtual IMigrationInstruction GetInstructionFor(string scopeId)
        {
            var instructions = InstructionsBySourceScope[scopeId];

            if ((instructions == null) || (instructions.Count == 0))
                return null;
            return instructions[0];
        }

        public virtual MigratingActivityInstance GetMigratingActivityInstanceById(string activityInstanceId)
        {
            return ActivityInstances[activityInstanceId];
        }

        public virtual MigratingScopeInstance GetMigratingCompensationInstanceByExecutionId(string id)
        {
            return CompensationInstances[id];
        }

        public virtual MigratingJobInstance GetMigratingJobInstanceById(string jobId)
        {
            return MigratingJobs[jobId];
        }

        public virtual MigratingExternalTaskInstance GetMigratingExternalTaskInstanceById(string externalTaskId)
        {
            return MigratingExternalTasks[externalTaskId];
        }

        public virtual IMigrationInstruction FindSingleMigrationInstruction(string sourceScopeId)
        {
            var instructions = InstructionsBySourceScope[sourceScopeId];

            if ((instructions != null) && (instructions.Count > 0))
                return instructions[0];
            return null;
        }

        protected internal virtual IDictionary<string, IList<IMigrationInstruction>> OrganizeInstructionsBySourceScope(
            IMigrationPlan migrationPlan)
        {
            IDictionary<string, IList<IMigrationInstruction>> organizedInstructions =
                new Dictionary<string, IList<IMigrationInstruction>>();

            foreach (var instruction in migrationPlan.Instructions)
                CollectionUtil.AddToMapOfLists(organizedInstructions, instruction.SourceActivityId, instruction);

            return organizedInstructions;
        }

        public virtual void HandleDependentActivityInstanceJobs(MigratingActivityInstance migratingInstance,
            IList<JobEntity> jobs)
        {
            Parser.DependentActivityInstanceJobHandler.Handle(this, migratingInstance, jobs);
        }

        public virtual void HandleDependentTransitionInstanceJobs(MigratingTransitionInstance migratingInstance,
            IList<JobEntity> jobs)
        {
            Parser.DependentTransitionInstanceJobHandler.Handle(this, migratingInstance, jobs);
        }

        public virtual void HandleDependentEventSubscriptions(MigratingActivityInstance migratingInstance,
            IList<EventSubscriptionEntity> eventSubscriptions)
        {
            Parser.DependentEventSubscriptionHandler.Handle(this, migratingInstance, eventSubscriptions);
        }

        public virtual void HandleDependentVariables(MigratingProcessElementInstance migratingInstance,
            IList<VariableInstanceEntity> variables)
        {
            Parser.DependentVariablesHandler.Handle(this, migratingInstance, variables);
        }

        public virtual void HandleTransitionInstance(ITransitionInstance transitionInstance)
        {
            Parser.TransitionInstanceHandler.Handle(this, transitionInstance);
        }

        public virtual void ValidateNoEntitiesLeft(MigratingProcessInstanceValidationReportImpl processInstanceReport)
        {
            processInstanceReport.ProcessInstanceId = migratingProcessInstance.ProcessInstanceId;

            EnsureNoEntitiesAreLeft("tasks", TasksRenamed, processInstanceReport);
            EnsureNoEntitiesAreLeft("externalTask", ExternalTasksRenamed, processInstanceReport);
            EnsureNoEntitiesAreLeft("incidents", IncidentsRenamed, processInstanceReport);
            EnsureNoEntitiesAreLeft("jobs", JobsRenamed, processInstanceReport);
            EnsureNoEntitiesAreLeft("event subscriptions", EventSubscriptionsRenamed, processInstanceReport);
            EnsureNoEntitiesAreLeft("variables", VariablesRenamed, processInstanceReport);
        }

        public virtual void EnsureNoEntitiesAreLeft<T1>(string entityName, ICollection<T1> dbEntities,
            MigratingProcessInstanceValidationReportImpl processInstanceReport) where T1 : IDbEntity
        {
            if (dbEntities.Count > 0)
            {
                throw new NotImplementedException();
                //processInstanceReport.AddFailure("Process instance contains not migrated " + entityName + ": [" +StringUtil.JoinDbEntityIds(dbEntities) + "]");
            }
        }
    }
}