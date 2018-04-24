using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.Extensions.Logging;
using ESS.FW.Common.ServiceBus;
using ESS.FW.Bpm.Engine.EntityFramework.Maps;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Batch.Impl.History;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.Shared.Entities.Bpm.Config;

namespace ESS.FW.Bpm.Engine.EntityFramework
{
    public class BpmDbContext : DbContext
    {
        protected const string PreFixTableName = "TB_GOS_BPM_";
        //需要排除的类(Entity的子类)
        public static readonly IList<Type> IgnoreType = new List<Type>
        {
            typeof(DeploymentStatisticsEntity),
            typeof(ProcessDefinitionStatisticsEntity),
            typeof(CoreActivity),
            typeof(ActivityImpl),
            typeof(Lane),
            typeof(LaneSet),
            typeof(PvmExecutionImpl),
            typeof(CoreExecution),
            typeof(AbstractVariableScope),
            typeof(CoreModelElement),
            typeof(ProcessDefinitionImpl),
            typeof(DelayedVariableEvent),
            //typeof(HistoricActivityInstanceEntity),
            //typeof(HistoricTaskInstanceEventEntity),
            //typeof(HistoricDetailVariableInstanceUpdateEntity),
            typeof(HistoricFormPropertyEntity),
            //typeof(HistoricFormPropertyEventEntity),
            //typeof(HistoricVariableUpdateEventEntity)
        };

        //public BpmDbContext(DbCompiledModel model, ILoggerFactory loggerFactory, IBus bus)
        //    : base("OracleDbContext", model, loggerFactory, bus)
        //{
        //    Database.SetInitializer<BpmDbContext>(null);

        //    Database.Initialize(force: false);
        //}
        //public static DbModelBuilder OnModelCreating()
        //{
        //    DbModelBuilder modelBuilder = new DbModelBuilder();

        //    //字段映射关系
        //    modelBuilder.Configurations.Add(new DecisionDefinitionMap());
        //    modelBuilder.Configurations.Add(new DecisionRequirementsDefinitionMap());
        //    modelBuilder.Configurations.Add(new PropertyMap());
        //    modelBuilder.Configurations.Add(new ProcessDefinitionMap());
        //    modelBuilder.Configurations.Add(new DeploymentMap());
        //    modelBuilder.Configurations.Add(new ResourceMap());
        //    modelBuilder.Configurations.Add(new ExecutionMap());
        //    modelBuilder.Configurations.Add(new JobMap());
        //    //modelBuilder.Configurations.Add(new MessageMap());
        //    modelBuilder.Configurations.Add(new TaskMap());
        //    modelBuilder.Configurations.Add(new IncidentMap());
        //    modelBuilder.Configurations.Add(new EventSubscriptionMap());
        //    modelBuilder.Configurations.Add(new ExternalTaskMap());
        //    modelBuilder.Configurations.Add(new IdentityLinkMap());
        //    modelBuilder.Configurations.Add(new VariableInstanceMap());
        //    modelBuilder.Configurations.Add(new AttachmentMap());
        //    modelBuilder.Configurations.Add(new AuthorizationMap());
        //    modelBuilder.Configurations.Add(new CommentMap());
        //    modelBuilder.Configurations.Add(new FilterMap());
        //    modelBuilder.Configurations.Add(new GroupMap());
        //    modelBuilder.Configurations.Add(new HistoricActivityInstanceMap());
        //    modelBuilder.Configurations.Add(new HistoricBatchMap());
        //    modelBuilder.Configurations.Add(new HistoricCaseActivityInstanceMap());
        //    modelBuilder.Configurations.Add(new HistoricCaseInstanceMap());
        //    modelBuilder.Configurations.Add(new HistoricDecisionInputInstanceMap());
        //    modelBuilder.Configurations.Add(new HistoricDecisionInstanceMap());
        //    modelBuilder.Configurations.Add(new HistoricDecisionOutputInstanceMap());
        //    modelBuilder.Configurations.Add(new HistoricDetailEventMap());
        //    modelBuilder.Configurations.Add(new HistoricExternalTaskLogMap());
        //    modelBuilder.Configurations.Add(new HistoricIdentityLinkLogMap());
        //    modelBuilder.Configurations.Add(new HistoricIncidentMap());
        //    modelBuilder.Configurations.Add(new HistoricJobLogEventMap());
        //    modelBuilder.Configurations.Add(new HistoricProcessInstanceMap());
        //    modelBuilder.Configurations.Add(new HistoricTaskInstanceMap());
        //    modelBuilder.Configurations.Add(new HistoricVariableInstanceMap());
        //    modelBuilder.Configurations.Add(new IdentityInfoMap());
        //    modelBuilder.Configurations.Add(new JobDefinitionMap());
        //    modelBuilder.Configurations.Add(new MembershipMap());
        //    modelBuilder.Configurations.Add(new MeterLogMap());
        //    modelBuilder.Configurations.Add(new TenantMap());
        //    modelBuilder.Configurations.Add(new TenantMembershipMap());
        //    modelBuilder.Configurations.Add(new UserMap());
        //    modelBuilder.Configurations.Add(new UserOperationLogEntryEventMap());

        //    //modelBuilder.Configurations.Add(new DyFormMap());
        //    //modelBuilder.Configurations.Add(new DyFormGroupMap());
        //    //modelBuilder.Configurations.Add(new DyFormClMap());
        //    //modelBuilder.Configurations.Add(new DyFormElMap());
        //    //modelBuilder.Configurations.Add(new DyFormGridMap());
        //    //modelBuilder.Configurations.Add(new DyFormErrorMessageMap());
        //    //modelBuilder.Configurations.Add(new DyFormValidatorMap());
        //    modelBuilder.Configurations.Add(new DynamicFormMap());

        //    modelBuilder.Configurations.Add(new ComponentMethodInfoMap());
        //    modelBuilder.Configurations.Add(new ComponentParameterInfoMap());
        //    modelBuilder.Configurations.Add(new ProcessDefinitionBranchMap());
        //    modelBuilder.Configurations.Add(new ProcessDefinitionAuthMap());

        //    //modelBuilder.Configurations.Add(new DynamicFormClMap());
        //    //modelBuilder.Configurations.Add(new HistoryEventMap());


        //    //外键映射
        //    //modelBuilder.Entity<ResourceEntity>()
        //    //   .HasMany(e => e.ExternalTasks)
        //    //   .WithOptional(e => e.ErrorDetailsByteArray)
        //    //   .HasForeignKey(e => e.ErrorDetailsByteArrayId);

        //    //modelBuilder.Entity<ResourceEntity>()
        //    //    .HasMany(e => e.Jobs)
        //    //    .WithOptional(e => e.ExceptionByteArray)
        //    //    .HasForeignKey(e => e.ExceptionByteArrayId);

        //    //modelBuilder.Entity<ResourceEntity>()
        //    //    .HasMany(e => e.Variables)
        //    //    .WithOptional(e => e.ResourceEntity)
        //    //    .HasForeignKey(e => e.ByteArrayValueId);

        //    //modelBuilder.Entity<HistoricActivityInstanceEntity>()
        //    //    .Property(e => e.DurationInMillis);//.Duration)
        //    //                                       //.HasPrecision(19, 0);

        //    //modelBuilder.Entity<HistoricActivityInstanceEntity>()
        //    //    .Property(e => e.SequenceCounter);
        //    //// .HasPrecision(19, 0);

        //    //modelBuilder.Entity<HistoricDecisionInputInstanceEntity>()
        //    //    .Property(e => e.LongValue);//.long)
        //    //   // .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<HiDecisionOutDb>()
        //    ////    .Property(e => e.long)
        //    ////    .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<HiDetailDb>()
        //    ////    .Property(e => e.long)
        //    ////    .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<HiDetailDb>()
        //    ////    .Property(e => e.SequenceCounter)
        //    ////    .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<HiExternalTaskLogDb>()
        //    ////    .Property(e => e.Priority)
        //    ////    .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<HiJobLogDb>()
        //    ////    .Property(e => e.JobPriority)
        //    ////    .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<HiJobLogDb>()
        //    ////    .Property(e => e.SequenceCounter)
        //    ////    .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<HiProcessInstanceDb>()
        //    ////    .Property(e => e.Duration)
        //    ////    .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<HiTaskInstanceDb>()
        //    ////    .Property(e => e.Duration)
        //    ////    .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<HiVariableInstanceDb>()
        //    ////    .Property(e => e.Long)
        //    ////    .HasPrecision(19, 0);

        //    //modelBuilder.Entity<GroupEntity>()
        //    //    .HasMany(e => e.TenantMembers)
        //    //    .WithOptional(e => e.Group)
        //    //    .HasForeignKey(e => e.GroupId);

        //    //modelBuilder.Entity<GroupEntity>()
        //    //    .HasMany(e => e.Users)
        //    //    .WithMany(e => e.Groups)
        //    //    .Map(m => m.ToTable(PreFixTableName + "ID_MEMBERSHIP","syerp").MapLeftKey("GROUP_ID").MapRightKey("USER_ID"));

        //    //modelBuilder.Entity<TenantEntity>()
        //    //    .HasMany(e => e.TenantMembers)
        //    //    .WithRequired(e => e.Tenant)
        //    //    .HasForeignKey(e => e.TenantId)
        //    //    .WillCascadeOnDelete(false);

        //    //modelBuilder.Entity<UserEntity>()
        //    //    .HasMany(e => e.TenantMembers)
        //    //    .WithOptional(e => e.User)
        //    //    .HasForeignKey(e => e.UserId);

        //    //modelBuilder.Entity<DecisionRequirementsDefinitionEntity>()
        //    //    .HasMany(e => e.DecisionDefinitions)
        //    //    .WithOptional(e => e.DecisionRequirementsDefinition)
        //    //    .HasForeignKey(e => e.DecisionRequirementsDefinitionId);

        //    ////modelBuilder.Entity<DeploymentEntity>()
        //    ////    .HasMany(e => e.ByteArrays)
        //    ////    .WithOptional(e=>e.Deployment)
        //    ////    .HasForeignKey(e => e.DeploymentId)
        //    ////    .WillCascadeOnDelete(false);

        //    //modelBuilder.Entity<ProcessDefinitionEntity>()
        //    //    .HasMany(e => e.IdentityLinks)
        //    //    .WithOptional(e => e.ProcessDef)//.ProcessDefinition)
        //    //    .HasForeignKey(e => e.ProcessDefId);

        //    //modelBuilder.Entity<ProcessDefinitionEntity>()
        //    //    .HasMany(e => e.Executions)
        //    //    .WithOptional(e => e.ProcessDefinitionEf)
        //    //    .HasForeignKey(e => e.ProcessDefinitionId);

        //    //modelBuilder.Entity<ProcessDefinitionEntity>()
        //    //    .HasMany(e => e.Incidents)
        //    //    .WithOptional(e => e.ProcessDefinition)
        //    //    .HasForeignKey(e => e.ProcessDefinitionId);

        //    //modelBuilder.Entity<ProcessDefinitionEntity>()
        //    //    .HasMany(e => e.Tasks)
        //    //    .WithOptional(e => e.ProcessDefinition)
        //    //    .HasForeignKey(e => e.ProcessDefinitionId);

        //    ////modelBuilder.Entity<ExecutionEntity>()
        //    ////    .Property(e => e.SequenceCounter)
        //    ////    .HasPrecision(19, 0);

        //    //modelBuilder.Entity<ExecutionEntity>()
        //    //    .HasMany(e => e.EventSubscriptions)
        //    //    .WithOptional(e => e.Execution)
        //    //    .HasForeignKey(e => e.ExecutionId);

        //    ////modelBuilder.Entity<ExecutionEntity>()
        //    ////   .HasMany(e => e.ChildExecutions)
        //    ////    .WithOptional(e => e.Parent)
        //    ////    .HasForeignKey(e => e.ParentId);

        //    //modelBuilder.Entity<ExecutionEntity>()
        //    //    .HasMany(e => e.ChildExecutions)
        //    //    .WithOptional(e => e.ProcessInstanceExecution)
        //    //    .HasForeignKey(e => e.ProcessInstanceId);

        //    //modelBuilder.Entity<ExecutionEntity>()
        //    //    .HasMany(e => e.SuperExecutions)
        //    //    .WithOptional(e => e.SuperExecution)
        //    //    .HasForeignKey(e => e.SuperExecutionId);

        //    //modelBuilder.Entity<ExecutionEntity>()
        //    //    .HasMany(e => e.ExternalTasks)
        //    //    .WithOptional(e => e.Execution)
        //    //    .HasForeignKey(e => e.ExecutionId);

        //    //modelBuilder.Entity<ExecutionEntity>()
        //    //    .HasMany(e => e.Incidents)
        //    //    .WithOptional(e => e.Execution)
        //    //    .HasForeignKey(e => e.ExecutionId);

        //    //modelBuilder.Entity<ExecutionEntity>()
        //    //    .HasMany(e => e.ProcessInstanceIncidents)
        //    //    .WithOptional(e => e.ProcessInstanceExecution)
        //    //    .HasForeignKey(e => e.ProcessInstanceId);

        //    //modelBuilder.Entity<ExecutionEntity>()
        //    //    .HasMany(e => e.Tasks)
        //    //    .WithOptional(e => e.ExecutionEf)
        //    //    .HasForeignKey(e => e.ExecutionId);

        //    //modelBuilder.Entity<ExecutionEntity>()
        //    //    .HasMany(e => e.ProcessInstanceTasks)
        //    //    .WithOptional(e => e.ProcessInstanceExecution)
        //    //    .HasForeignKey(e => e.ProcessInstanceId);

        //    //modelBuilder.Entity<ExecutionEntity>()
        //    //    .HasMany(e => e.VariableEfs)
        //    //    .WithOptional(e => e.Execution)
        //    //    .HasForeignKey(e => e.ExecutionId);

        //    //modelBuilder.Entity<ExecutionEntity>()
        //    //    .HasMany(e => e.ProcessInstanceVariables)
        //    //    .WithOptional(e => e.ProcessInstanceExecution)
        //    //    .HasForeignKey(e => e.ProcessInstanceId);

        //    ////modelBuilder.Entity<ExternalTaskEntity>()
        //    ////    .Property(e => e.Priority)
        //    ////    .HasPrecision(19, 0);

        //    //modelBuilder.Entity<IncidentEntity>()
        //    //    .HasMany(e => e.CauseIncidentIncidents)
        //    //    .WithOptional(e => e.CauseIncidentIncident)
        //    //    .HasForeignKey(e => e.CauseIncidentId);

        //    //modelBuilder.Entity<IncidentEntity>()
        //    //    .HasMany(e => e.RootCauseIncidentIncidents)
        //    //    .WithOptional(e => e.RootCauseIncidentIncident)
        //    //    .HasForeignKey(e => e.RootCauseIncidentId);

        //    ////modelBuilder.Entity<JobDb>()
        //    ////    .Property(e => e.Priority)
        //    ////    .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<JobDb>()
        //    ////    .Property(e => e.SequenceCounter)
        //    ////    .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<JobDefinitionDb>()
        //    ////    .Property(e => e.JobPriority)
        //    ////    .HasPrecision(19, 0);

        //    //modelBuilder.Entity<JobDefinitionEntity>()
        //    //    .HasMany(e => e.JobDefinitionBatchs)
        //    //    .WithOptional(e => e.BatchJobDefinitions)
        //    //    .HasForeignKey(e => e.BatchJobDefinitionId);

        //    //modelBuilder.Entity<JobDefinitionEntity>()
        //    //    .HasMany(e => e.MonitorJobDefBatchs)
        //    //    .WithOptional(e => e.MonitorJobDefinition)
        //    //    .HasForeignKey(e => e.MonitorJobDefinitionId);

        //    //modelBuilder.Entity<JobDefinitionEntity>()
        //    //    .HasMany(e => e.SeedJobDefBatch)
        //    //    .WithOptional(e => e.SeedJobDefinition)
        //    //    .HasForeignKey(e => e.SeedJobDefinitionId);

        //    //modelBuilder.Entity<JobDefinitionEntity>()
        //    //    .HasMany(e => e.Incidents)
        //    //    .WithOptional(e => e.JobDefinition)
        //    //    .HasForeignKey(e => e.JobDefinitionId);

        //    ////modelBuilder.Entity<MeterLogMapDb>()
        //    ////    .Property(e => e.Value)
        //    ////    .HasPrecision(19, 0);

        //    ////modelBuilder.Entity<MeterLogMapDb>()
        //    ////    .Property(e => e.Milliseconds)
        //    ////    .HasPrecision(19, 0);

        //    //modelBuilder.Entity<TaskEntity>()
        //    //    .HasMany(e => e.IdentityLinks)
        //    //    .WithOptional(e => e.Task)
        //    //    .HasForeignKey(e => e.TaskId);

        //    //modelBuilder.Entity<VariableDb>()
        //    //    .Property(e => e.LongValue)
        //    //    .HasPrecision(19, 0);

        //    //modelBuilder.Entity<VariableDb>()
        //    //    .Property(e => e.SequenceCounter)
        //    //    .HasPrecision(19, 0);

        //    //忽略类型
        //    modelBuilder.Ignore(IgnoreType);
            
        //    return modelBuilder;
        //}
        public virtual DbSet<ProcessDefinitionEntity> ProcessDefinitions { get; set; }
        public virtual DbSet<PropertyEntity> Properties { get; set; }
        public virtual DbSet<DeploymentEntity> Deployments { get; set; }
        public virtual DbSet<ResourceEntity> Resources { get; set; }
        public virtual DbSet<ExecutionEntity> Executions { get; set; }
        public virtual DbSet<JobEntity> Jobs { get; set; }
        //public virtual DbSet<TimerEntity> Timers { get; set; }
        //public virtual DbSet<MessageEntity> Messages { get; set; }
        public virtual DbSet<TaskEntity> Tasks { get; set; }
        public virtual DbSet<IncidentEntity> Incidents { get; set; }
        public virtual DbSet<EventSubscriptionEntity> EventSubscriptions { get; set; }
        public virtual DbSet<ExternalTaskEntity> ExternalTasks { get; set; }
        public virtual DbSet<IdentityLinkEntity> IdentityLinks { get; set; }
        public virtual DbSet<VariableInstanceEntity> VariableInstances { get; set; }
        public virtual DbSet<AttachmentEntity> Attachments { get; set; }
        public virtual DbSet<AuthorizationEntity> Authorizations { get; set; }
        public virtual DbSet<CommentEntity> Comments { get; set; }
        public virtual DbSet<FilterEntity> Filters { get; set; }
        public virtual DbSet<GroupEntity> Groups { get; set; }
        public virtual DbSet<HistoricActivityInstanceEventEntity> HistoricActivityInstances { get; set; }
        public virtual DbSet<HistoricBatchEntity> HistoricBatchs { get; set; }
        public virtual DbSet<HistoricCaseActivityInstanceEventEntity> HistoricCaseActivityInstances { get; set; }
        public virtual DbSet<HistoricCaseInstanceEventEntity> HistoricCaseInstances { get; set; }
        public virtual DbSet<HistoricDecisionInputInstanceEntity> HistoricDecisionInputInstances { get; set; }
        public virtual DbSet<HistoricDecisionOutputInstanceEntity> HistoricDecisionOutputInstances { get; set; }
        public virtual DbSet<HistoricDetailEventEntity> HistoricDetailEvents { get; set; }
        public virtual DbSet<HistoricExternalTaskLogEntity> HistoricExternalTaskLogs { get; set; }
        public virtual DbSet<HistoricIdentityLinkLogEventEntity> HistoricIdentityLinkLogs { get; set; }
        public virtual DbSet<HistoricIncidentEntity> HistoricIncidents { get; set; }
        public virtual DbSet<HistoricJobLogEventEntity> HistoricJobLogEvents { get; set; }
        public virtual DbSet<HistoricProcessInstanceEventEntity> HistoricProcessInstances { get; set; }
        public virtual DbSet<HistoricTaskInstanceEventEntity> HistoricTaskInstances { get; set; }
        public virtual DbSet<HistoricVariableInstanceEntity> HistoricVariableInstances { get; set; }
        public virtual DbSet<IdentityInfoEntity> IdentityInfos { get; set; }
        public virtual DbSet<JobDefinitionEntity> JobDefinitions { get; set; }
        public virtual DbSet<MembershipEntity> Memberships { get; set; }
        public virtual DbSet<MeterLogEntity> MeterLogs { get; set; }
        public virtual DbSet<TenantEntity> Tenants { get; set; }
        public virtual DbSet<TenantMembershipEntity> TenantMemberships { get; set; }
        public virtual DbSet<UserEntity> Users { get; set; }
        public virtual DbSet<UserOperationLogEntryEventEntity> UserOperationLogEntryEvents { get; set; }
        public virtual DbSet<DecisionDefinitionEntity> DecisionDefinition { get; set; }
        public virtual DbSet<DecisionRequirementsDefinitionEntity> DecisionRequirementsDefinition { get; set; }
        

        public virtual DbSet<ComponentMethodInfo> ComponentMethodInfos { get; set; }
        public virtual DbSet<ComponentParameterInfo> ComponentParameterInfos { get; set; }

        public virtual DbSet<ProcessDefinitionBranch> ProcessDefinitionBranchs { get; set; }

        public virtual DbSet<ProcessDefinitionAuth> ProcessDefinitionAuths { get; set; }

    }
}