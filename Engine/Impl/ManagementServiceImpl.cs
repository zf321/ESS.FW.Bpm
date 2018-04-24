using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Metrics;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///      
    ///     
    ///     
    ///     
    ///     
    /// </summary>
    public class ManagementServiceImpl : ServiceImpl, IManagementService
    {
        public virtual IProcessApplicationRegistration RegisterProcessApplication(string deploymentId,
            IProcessApplicationReference reference)
        {
            return CommandExecutor.Execute(new RegisterProcessApplicationCmd(deploymentId, reference));
        }
        /// <summary>
        /// 重写部署入口
        /// </summary>
        /// <param name="deployment"></param>
        /// <returns></returns>
        public IProcessApplicationRegistration RegisterProcessApplication(DeploymentEntity deployment)
        {
            return CommandExecutor.Execute(new RegisterProcessApplicationNetCmd(deployment));
        }

        public virtual void UnregisterProcessApplication(string deploymentId, bool removeProcessesFromCache)
        {
            CommandExecutor.Execute(new UnregisterProcessApplicationCmd(deploymentId, removeProcessesFromCache));
        }

        public virtual void UnregisterProcessApplication(IList<string> deploymentIds, bool removeProcessesFromCache)
        {
            CommandExecutor.Execute(new UnregisterProcessApplicationCmd(deploymentIds, removeProcessesFromCache));
        }

        public virtual string GetProcessApplicationForDeployment(string deploymentId)
        {
            return CommandExecutor.Execute(new GetProcessApplicationForDeploymentCmd(deploymentId));
        }

        public virtual IDictionary<string, long> TableCount
        {
            get { return CommandExecutor.Execute(new GetTableCountCmd()); }
        }

        public virtual string GetTableName(Type activitiEntityClass)
        {
            return CommandExecutor.Execute(new GetTableNameCmd(activitiEntityClass));
        }

        public virtual TableMetaData GetTableMetaData(string tableName)
        {
            return CommandExecutor.Execute(new GetTableMetaDataCmd(tableName));
        }

        public virtual void ExecuteJob(string jobId)
        {
            ExecuteJobHelper.ExecuteJob(jobId, CommandExecutor);
        }

        public virtual void DeleteJob(string jobId)
        {
            CommandExecutor.Execute(new DeleteJobCmd(jobId));
        }

        public virtual void SetJobRetries(string jobId, int retries)
        {
            CommandExecutor.Execute(new SetJobRetriesCmd(jobId, null, retries));
        }

        public virtual void SetJobRetries(IList<string> jobIds, int retries)
        {
            CommandExecutor.Execute(new SetJobsRetriesCmd(jobIds, retries));
        }

        public virtual  IBatch  SetJobRetriesAsync(IList<string> jobIds, int retries)
        {
            return SetJobRetriesAsync(jobIds, (Expression<Func<IJob,bool>>) null, retries);
        }

        public virtual  IBatch  SetJobRetriesAsync(Expression<Func<IJob,bool>> jobQuery, int retries)
        {
            return SetJobRetriesAsync(null, jobQuery, retries);
        }

        public virtual  IBatch  SetJobRetriesAsync(IList<string> jobIds, Expression<Func<IJob,bool>> jobQuery, int retries)
        {
            return CommandExecutor.Execute(new SetJobsRetriesBatchCmd(jobIds, jobQuery, retries));
        }

        public virtual  IBatch  SetJobRetriesAsync(IList<string> processInstanceIds, IQueryable<IProcessInstance> query,
            int retries)
        {
            return CommandExecutor.Execute(new SetJobsRetriesByProcessBatchCmd(processInstanceIds, query, retries));
        }

        public virtual void SetJobRetriesByJobDefinitionId(string jobDefinitionId, int retries)
        {
            CommandExecutor.Execute(new SetJobRetriesCmd(null, jobDefinitionId, retries));
        }

        public virtual void SetJobDuedate(string jobId, DateTime newDuedate)
        {
            CommandExecutor.Execute(new SetJobDuedateCmd(jobId, newDuedate));
        }

        public virtual void SetJobPriority(string jobId, long priority)
        {
            CommandExecutor.Execute(new SetJobPriorityCmd(jobId, priority));
        }

        //public virtual ITablePageQuery CreateTablePageQuery()
        //{
        //    //return CommandExecutor.Execute(new CreateQueryCmd<TablePage>(expression));
        //}

        public virtual IQueryable<IJob> CreateJobQuery(Expression<Func<JobEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<JobEntity>(expression));
        }

        public virtual IQueryable<IJobDefinition> CreateJobDefinitionQuery(Expression<Func<JobDefinitionEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<JobDefinitionEntity>(expression));
        }

        public virtual string GetJobExceptionStacktrace(string jobId)
        {
            return CommandExecutor.Execute(new GetJobExceptionStacktraceCmd(jobId));
        }

        public virtual IDictionary<string, string> Properties
        {
            get { return CommandExecutor.Execute(new GetPropertiesCmd()); }
        }

        public virtual void SetProperty(string name, string value)
        {
            CommandExecutor.Execute(new SetPropertyCmd(name, value));
        }

        public virtual void DeleteProperty(string name)
        {
            CommandExecutor.Execute(new DeletePropertyCmd(name));
        }
        
        //public virtual string databaseSchemaUpgrade(Connection connection, string catalog, string schema)
        //{
        //    return commandExecutor.execute(new CommandAnonymousInnerClass(this, connection, catalog, schema));
        //}


        public virtual IQueryable<IProcessDefinitionStatistics> CreateProcessDefinitionStatisticsQuery(Expression<Func<ProcessDefinitionStatisticsEntity,bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<ProcessDefinitionStatisticsEntity>(expression));
        }

        public virtual IQueryable<IActivityStatistics> CreateActivityStatisticsQuery(string processDefinitionId)
        {
            //Expression<Func<IActivityStatistics, bool>> expression =
            //    entity => entity.Id == processDefinitionId; 
            //return CommandExecutor.Execute(new CreateQueryCmd<IActivityStatistics>(expression));
            //return new ActivityStatisticsQueryImpl(processDefinitionId, CommandExecutor);
            return null;
        }

        public virtual IQueryable<IDeploymentStatistics> CreateDeploymentStatisticsQuery(Expression<Func<DeploymentStatisticsEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<DeploymentStatisticsEntity>(expression));
        }

        public virtual IList<string> RegisteredDeployments
        {
            get { return CommandExecutor.Execute(new CommandAnonymousInnerClass2()); }
        }
        
        public virtual void RegisterDeploymentForJobExecutor(string deploymentId)
        {
            CommandExecutor.Execute(new RegisterDeploymentCmd(deploymentId));
        }
        
        public virtual void UnregisterDeploymentForJobExecutor(string deploymentId)
        {
            CommandExecutor.Execute(new UnregisterDeploymentCmd(deploymentId));
        }


        public virtual void ActivateJobDefinitionById(string jobDefinitionId)
        {
            UpdateJobDefinitionSuspensionState().ByJobDefinitionId(jobDefinitionId).Activate();
        }

        public virtual void ActivateJobDefinitionById(string jobDefinitionId, bool activateJobs)
        {
            UpdateJobDefinitionSuspensionState().ByJobDefinitionId(jobDefinitionId).SetIncludeJobs(activateJobs).Activate();
        }

        public virtual void ActivateJobDefinitionById(string jobDefinitionId, bool activateJobs, DateTime activationDate)
        {
            UpdateJobDefinitionSuspensionState()
                .ByJobDefinitionId(jobDefinitionId)
                .SetIncludeJobs(activateJobs)
                .ExecutionDate(activationDate)
                .Activate();
        }

        public virtual void SuspendJobDefinitionById(string jobDefinitionId)
        {
            UpdateJobDefinitionSuspensionState().ByJobDefinitionId(jobDefinitionId).Suspend();
        }

        public virtual void SuspendJobDefinitionById(string jobDefinitionId, bool suspendJobs)
        {
            UpdateJobDefinitionSuspensionState().ByJobDefinitionId(jobDefinitionId).SetIncludeJobs(suspendJobs).Suspend();
        }

        public virtual void SuspendJobDefinitionById(string jobDefinitionId, bool suspendJobs, DateTime? suspensionDate)
        {
            UpdateJobDefinitionSuspensionState()
                .ByJobDefinitionId(jobDefinitionId)
                .SetIncludeJobs(suspendJobs)
                .ExecutionDate(suspensionDate)
                .Suspend();
        }

        public virtual void ActivateJobDefinitionByProcessDefinitionId(string processDefinitionId)
        {
            UpdateJobDefinitionSuspensionState().ByProcessDefinitionId(processDefinitionId).Activate();
        }

        public virtual void ActivateJobDefinitionByProcessDefinitionId(string processDefinitionId, bool activateJobs)
        {
            UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionId(processDefinitionId)
                .SetIncludeJobs(activateJobs)
                .Activate();
        }

        public virtual void ActivateJobDefinitionByProcessDefinitionId(string processDefinitionId, bool activateJobs,
            DateTime activationDate)
        {
            UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionId(processDefinitionId)
                .SetIncludeJobs(activateJobs)
                .ExecutionDate(activationDate)
                .Activate();
        }

        public virtual void SuspendJobDefinitionByProcessDefinitionId(string processDefinitionId)
        {
            UpdateJobDefinitionSuspensionState().ByProcessDefinitionId(processDefinitionId).Suspend();
        }

        public virtual void SuspendJobDefinitionByProcessDefinitionId(string processDefinitionId, bool suspendJobs)
        {
            UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionId(processDefinitionId)
                .SetIncludeJobs(suspendJobs)
                .Suspend();
        }

        public virtual void SuspendJobDefinitionByProcessDefinitionId(string processDefinitionId, bool suspendJobs,
            DateTime? suspensionDate)
        {
            UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionId(processDefinitionId)
                .SetIncludeJobs(suspendJobs)
                .ExecutionDate(suspensionDate)
                .Suspend();
        }

        public virtual void ActivateJobDefinitionByProcessDefinitionKey(string processDefinitionKey)
        {
            UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(processDefinitionKey).Activate();
        }

        public virtual void ActivateJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool activateJobs)
        {
            UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionKey(processDefinitionKey)
                .SetIncludeJobs(activateJobs)
                .Activate();
        }

        public virtual void ActivateJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool activateJobs,
            DateTime activationDate)
        {
            UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionKey(processDefinitionKey)
                .SetIncludeJobs(activateJobs)
                .ExecutionDate(activationDate)
                .Activate();
        }

        public virtual void SuspendJobDefinitionByProcessDefinitionKey(string processDefinitionKey)
        {
            UpdateJobDefinitionSuspensionState().ByProcessDefinitionKey(processDefinitionKey).Suspend();
        }

        public virtual void SuspendJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool suspendJobs)
        {
            UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionKey(processDefinitionKey)
                .SetIncludeJobs(suspendJobs)
                .Suspend();
        }

        public virtual void SuspendJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool suspendJobs,
            DateTime? suspensionDate)
        {
            UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionKey(processDefinitionKey)
                .SetIncludeJobs(suspendJobs)
                .ExecutionDate(suspensionDate)
                .Suspend();
        }

        public virtual IUpdateJobDefinitionSuspensionStateSelectBuilder UpdateJobDefinitionSuspensionState()
        {
            return new UpdateJobDefinitionSuspensionStateBuilderImpl(CommandExecutor);
        }

        public virtual void ActivateJobById(string jobId)
        {
            UpdateJobSuspensionState().ByJobId(jobId).Activate();
        }

        public virtual void ActivateJobByProcessInstanceId(string processInstanceId)
        {
            UpdateJobSuspensionState().ByProcessInstanceId(processInstanceId).Activate();
        }

        public virtual void ActivateJobByJobDefinitionId(string jobDefinitionId)
        {
            UpdateJobSuspensionState().ByJobDefinitionId(jobDefinitionId).Activate();
        }

        public virtual void ActivateJobByProcessDefinitionId(string processDefinitionId)
        {
            UpdateJobSuspensionState().ByProcessDefinitionId(processDefinitionId).Activate();
        }

        public virtual void ActivateJobByProcessDefinitionKey(string processDefinitionKey)
        {
            UpdateJobSuspensionState().ByProcessDefinitionKey(processDefinitionKey).Activate();
        }

        public virtual void SuspendJobById(string jobId)
        {
            UpdateJobSuspensionState().ByJobId(jobId).Suspend();
        }

        public virtual void SuspendJobByJobDefinitionId(string jobDefinitionId)
        {
            UpdateJobSuspensionState().ByJobDefinitionId(jobDefinitionId).Suspend();
        }

        public virtual void SuspendJobByProcessInstanceId(string processInstanceId)
        {
            UpdateJobSuspensionState().ByProcessInstanceId(processInstanceId).Suspend();
        }

        public virtual void SuspendJobByProcessDefinitionId(string processDefinitionId)
        {
            UpdateJobSuspensionState().ByProcessDefinitionId(processDefinitionId).Suspend();
        }

        public virtual void SuspendJobByProcessDefinitionKey(string processDefinitionKey)
        {
            UpdateJobSuspensionState().ByProcessDefinitionKey(processDefinitionKey).Suspend();
        }

        public virtual IUpdateJobSuspensionStateSelectBuilder UpdateJobSuspensionState()
        {
            return new UpdateJobSuspensionStateBuilderImpl(CommandExecutor);
        }

        public virtual int HistoryLevel
        {
            get { return CommandExecutor.Execute(new GetHistoryLevelCmd()); }
        }

        public virtual IMetricsQuery CreateMetricsQuery()
        {
            return new MetricsQueryImpl(CommandExecutor);
        }

        public virtual void DeleteMetrics(DateTime? timestamp)
        {
            CommandExecutor.Execute(new DeleteMetricsCmd(timestamp, null));
        }

        public virtual void DeleteMetrics(DateTime? timestamp, string reporter)
        {
            CommandExecutor.Execute(new DeleteMetricsCmd(timestamp, reporter));
        }

        public virtual void ReportDbMetricsNow()
        {
            CommandExecutor.Execute(new ReportDbMetricsCmd());
        }

        public virtual void SetOverridingJobPriorityForJobDefinition(string jobDefinitionId, long priority)
        {
            CommandExecutor.Execute(new SetJobDefinitionPriorityCmd(jobDefinitionId, priority, false));
        }

        public virtual void SetOverridingJobPriorityForJobDefinition(string jobDefinitionId, long priority, bool cascade)
        {
            CommandExecutor.Execute(new SetJobDefinitionPriorityCmd(jobDefinitionId, priority, true));
        }

        public virtual void ClearOverridingJobPriorityForJobDefinition(string jobDefinitionId)
        {
            CommandExecutor.Execute(new SetJobDefinitionPriorityCmd(jobDefinitionId, null, false));
        }

        public virtual IQueryable<IBatch> CreateBatchQuery(Expression<Func<BatchEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<BatchEntity>(expression));

        }

        public virtual void DeleteBatch(string batchId, bool cascade)
        {
            CommandExecutor.Execute(new DeleteBatchCmd(batchId, cascade));
        }

        public virtual void SuspendBatchById(string batchId)
        {
            CommandExecutor.Execute(new SuspendBatchCmd(batchId));
        }

        public virtual void ActivateBatchById(string batchId)
        {
            CommandExecutor.Execute(new ActivateBatchCmd(batchId));
        }

        public virtual IQueryable<IBatchStatistics> CreateBatchStatisticsQuery(Expression<Func<BatchStatisticsEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<BatchStatisticsEntity>(expression));
        }

        /// <summary>
        ///     Purges the database and the deployment cache.
        /// </summary>
        public virtual PurgeReport Purge()
        {
            return CommandExecutor.Execute(new PurgeDatabaseAndCacheCmd());
        }

        public void DeleteMetrics(DateTime timestamp, string reporter)
        {
            throw new NotImplementedException();
        }
        

        //public string databaseSchemaUpgrade(Connection connection, string catalog, string schema)
        //{
        //    throw new NotImplementedException();
        //}

        private class CommandAnonymousInnerClass : ICommand<string>
        {
            private readonly string _catalog;
            private readonly ManagementServiceImpl _outerInstance;

            //private readonly Connection connection;
            private readonly string _schema;

            //public CommandAnonymousInnerClass(ManagementServiceImpl outerInstance, Connection connection, string catalog,
            //    string schema)
            //{
            //    this.outerInstance = outerInstance;
            //    //this.connection = connection;
            //    this.catalog = catalog;
            //    this.schema = schema;
            //}

            public virtual string Execute(CommandContext commandContext)
            {
                //commandContext.AuthorizationManager.checkCamundaAdmin();
                //var dbSqlSessionFactory = (DbSqlSessionFactory)commandContext.SessionFactories[typeof(DbSqlSession)];
                //DbSqlSession dbSqlSession = new DbSqlSession(dbSqlSessionFactory, connection, catalog, schema);
                //commandContext.Sessions[typeof(DbSqlSession)] = dbSqlSession;
                //dbSqlSession.dbSchemaUpdate();

                return "";
            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<IList<string>>
        {
            //private readonly ManagementServiceImpl _outerInstance;

            //public CommandAnonymousInnerClass2(ManagementServiceImpl outerInstance)
            //{
            //    this._outerInstance = outerInstance;
            //}

            public virtual IList<string> Execute(CommandContext commandContext)
            {
                commandContext.AuthorizationManager.CheckCamundaAdmin();
                var registeredDeployments = Context.ProcessEngineConfiguration.RegisteredDeployments;
                return new List<string>(registeredDeployments);
            }
        }
    }
}