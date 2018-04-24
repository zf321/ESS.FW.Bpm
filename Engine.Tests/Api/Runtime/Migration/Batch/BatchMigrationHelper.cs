using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Repository;

namespace Engine.Tests.Api.Runtime.Migration.Batch
{
    public class BatchMigrationHelper : BatchHelper
    {
        protected internal MigrationTestRule MigrationRule;

        public IProcessDefinition sourceProcessDefinition;
        public IProcessDefinition targetProcessDefinition;

        public BatchMigrationHelper(ProcessEngineRule engineRule, MigrationTestRule migrationRule) : base(engineRule)
        {
            MigrationRule = migrationRule;
        }

        public BatchMigrationHelper(ProcessEngineRule engineRule) : this(engineRule, null)
        {
        }

        public virtual IProcessDefinition SourceProcessDefinition
        {
            get { return sourceProcessDefinition; }
        }

        public virtual IProcessDefinition TargetProcessDefinition
        {
            get { return targetProcessDefinition; }
        }

        public virtual IBatch CreateMigrationBatchWithSize(int batchSize)
        {
            var invocationsPerBatchJob =
                ((ProcessEngineConfigurationImpl) EngineRule.ProcessEngine.ProcessEngineConfiguration)
                .InvocationsPerBatchJob;
            return MigrateProcessInstancesAsync(invocationsPerBatchJob * batchSize);
        }

        public virtual IBatch MigrateProcessInstancesAsync(int numberOfProcessInstances)
        {
            sourceProcessDefinition = MigrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            targetProcessDefinition = MigrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            return MigrateProcessInstancesAsync(numberOfProcessInstances, sourceProcessDefinition,
                targetProcessDefinition);
        }

        public virtual IBatch MigrateProcessInstancesAsyncForTenant(int numberOfProcessInstances, string tenantId)
        {
            sourceProcessDefinition = MigrationRule.DeployForTenantAndGetDefinition(tenantId, ProcessModels.OneTaskProcess);
            targetProcessDefinition = MigrationRule.DeployForTenantAndGetDefinition(tenantId, ProcessModels.OneTaskProcess);
            return MigrateProcessInstancesAsync(numberOfProcessInstances, sourceProcessDefinition,
                targetProcessDefinition);
        }

        public virtual IBatch MigrateProcessInstanceAsync(IProcessDefinition sourceProcessDefinition,
            IProcessDefinition targetProcessDefinition)
        {
            return MigrateProcessInstancesAsync(1, sourceProcessDefinition, targetProcessDefinition);
        }

        public virtual IBatch MigrateProcessInstancesAsync(int numberOfProcessInstances,
            IProcessDefinition sourceProcessDefinition, IProcessDefinition targetProcessDefinition)
        {
            var runtimeService = EngineRule.RuntimeService;

            IList<string> processInstanceIds = new List<string>(numberOfProcessInstances);
            for (var i = 0; i < numberOfProcessInstances; i++)
                processInstanceIds.Add(runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id)
                    .Id);

            var migrationPlan =
                EngineRule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            return runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstanceIds)
                .ExecuteAsync();
        }

        public override IJobDefinition GetExecutionJobDefinition(IBatch batch)
        {
            return EngineRule.ManagementService.CreateJobDefinitionQuery(c=>c.Id == batch.BatchJobDefinitionId&&c.JobType==BatchFields.TypeProcessInstanceMigration)
                .First();
        }

        public virtual long CountSourceProcessInstances()
        {
            return EngineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId ==sourceProcessDefinition.Id)
                .Count();
        }

        public virtual long CountTargetProcessInstances()
        {
            return EngineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId ==targetProcessDefinition.Id)
                .Count();
        }
    }
}