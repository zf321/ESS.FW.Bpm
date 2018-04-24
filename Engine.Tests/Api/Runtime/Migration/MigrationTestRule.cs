using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationTestRule : ProcessEngineTestRule
    {
        public ProcessInstanceSnapshot SnapshotAfterMigration;

        public ProcessInstanceSnapshot SnapshotBeforeMigration;

        public MigrationTestRule(ProcessEngineRule processEngineRule) : base(processEngineRule)
        {
        }

        public virtual string GetSingleExecutionIdForActivity(IActivityInstance activityInstance, string activityId)
        {
            var singleInstance = GetSingleActivityInstance(activityInstance, activityId);

            var executionIds = singleInstance.ExecutionIds;
            if (executionIds.Length == 1)
                return executionIds[0];
            throw new System.Exception("There is more than one execution assigned to activity instance " + singleInstance.Id);
        }

        public virtual string GetSingleExecutionIdForActivityBeforeMigration(string activityId)
        {
            return GetSingleExecutionIdForActivity(SnapshotBeforeMigration.ActivityTree, activityId);
        }

        public virtual string GetSingleExecutionIdForActivityAfterMigration(string activityId)
        {
            return GetSingleExecutionIdForActivity(SnapshotAfterMigration.ActivityTree, activityId);
        }

        public virtual IActivityInstance GetSingleActivityInstance(
            IActivityInstance tree, string activityId)
        {
            var activityInstances = tree.GetActivityInstances(activityId);
            if (activityInstances.Length == 1)
                return activityInstances[0];
            throw new System.Exception("There is not exactly one activity instance for activity " + activityId);
        }

        public virtual IActivityInstance GetSingleActivityInstanceBeforeMigration(
            string activityId)
        {
            return GetSingleActivityInstance(SnapshotBeforeMigration.ActivityTree, activityId);
        }

        public virtual IActivityInstance GetSingleActivityInstanceAfterMigration(
            string activityId)
        {
            return GetSingleActivityInstance(SnapshotAfterMigration.ActivityTree, activityId);
        }

        public virtual ProcessInstanceSnapshot TakeFullProcessInstanceSnapshot(IProcessInstance processInstance)
        {
            return TakeProcessInstanceSnapshot(processInstance)
                .Full();
        }

        public virtual ProcessInstanceSnapshotBuilder TakeProcessInstanceSnapshot(IProcessInstance processInstance)
        {
            return new ProcessInstanceSnapshotBuilder(processInstance, ProcessEngine);
        }

        public virtual IProcessInstance CreateProcessInstanceAndMigrate(IMigrationPlan migrationPlan)
        {
            var processInstance =
                ProcessEngine.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

            MigrateProcessInstance(migrationPlan, processInstance);
            return processInstance;
        }

        public virtual IProcessInstance CreateProcessInstanceAndMigrate(IMigrationPlan migrationPlan,
            IDictionary<string, object> variables)
        {
            var processInstance =
                ProcessEngine.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId
                    /*, variables*/);

            MigrateProcessInstance(migrationPlan, processInstance);
            return processInstance;
        }

        public virtual void MigrateProcessInstance(IMigrationPlan migrationPlan, IProcessInstance processInstance)
        {
            SnapshotBeforeMigration = TakeFullProcessInstanceSnapshot(processInstance);

            var runtimeService = ProcessEngine.RuntimeService;

            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(new List<string> {SnapshotBeforeMigration.ProcessInstanceId})
                .Execute();

            // fetch updated process instance
            processInstance =
                runtimeService.CreateProcessInstanceQuery(c => c.ProcessInstanceId == processInstance.Id)
                    .First();

            SnapshotAfterMigration = TakeFullProcessInstanceSnapshot(processInstance);
        }

        public virtual void TriggerTimer()
        {
            var job = AssertTimerJobExists(SnapshotAfterMigration);
            ProcessEngine.ManagementService.ExecuteJob(job.Id);
        }

        public virtual ExecutionAssert AssertExecutionTreeAfterMigration()
        {
            //return Assert.That(SnapshotAfterMigration.ExecutionTree);
            return new ExecutionAssert();
        }

        public virtual ActivityInstanceAssert.ActivityInstanceAssertThatClause AssertActivityTreeAfterMigration()
        {
            //return Assert.That(SnapshotAfterMigration.ActivityTree);
            return null;
        }

        public virtual void AssertEventSubscriptionsMigrated(string activityIdBefore, string activityIdAfter,
            string eventName)
        {
            var eventSubscriptionsBefore =
                SnapshotBeforeMigration.GetEventSubscriptionsForActivityIdAndEventName(activityIdAfter, eventName);

            foreach (var eventSubscription in eventSubscriptionsBefore)
                AssertEventSubscriptionMigrated(eventSubscription, activityIdAfter, eventName);
        }

        protected internal virtual void AssertEventSubscriptionMigrated(IEventSubscription eventSubscriptionBefore,
            string activityIdAfter, string eventName)
        {
            var eventSubscriptionAfter = SnapshotAfterMigration.GetEventSubscriptionById(eventSubscriptionBefore.Id);
            Assert.NotNull(eventSubscriptionAfter,
                "Expected that an event subscription with id '" + eventSubscriptionBefore.Id + "' " +
                "exists after migration");

            Assert.AreEqual(eventSubscriptionBefore.EventType, eventSubscriptionAfter.EventType);
            Assert.AreEqual(activityIdAfter, eventSubscriptionAfter.ActivityId);
            Assert.AreEqual(eventName, eventSubscriptionAfter.EventName);
        }


        public virtual void AssertEventSubscriptionMigrated(string activityIdBefore, string activityIdAfter,
            string eventName)
        {
            var eventSubscriptionBefore =
                SnapshotBeforeMigration.GetEventSubscriptionForActivityIdAndEventName(activityIdBefore, eventName);
            Assert.NotNull(eventSubscriptionBefore,
                "Expected that an event subscription for activity '" + activityIdBefore + "' exists before migration"
            );

            AssertEventSubscriptionMigrated(eventSubscriptionBefore, activityIdAfter, eventName);
        }

        public virtual void AssertEventSubscriptionMigrated(string activityIdBefore, string eventNameBefore,
            string activityIdAfter, string eventNameAfter)
        {
            var eventSubscriptionBefore =
                SnapshotBeforeMigration.GetEventSubscriptionForActivityIdAndEventName(activityIdBefore, eventNameBefore);
            Assert.NotNull(eventSubscriptionBefore,
                "Expected that an event subscription for activity '" + activityIdBefore + "' exists before migration"
            );

            AssertEventSubscriptionMigrated(eventSubscriptionBefore, activityIdAfter, eventNameAfter);
        }

        public virtual void AssertEventSubscriptionRemoved(string activityId, string eventName)
        {
            var eventSubscriptionBefore =
                SnapshotBeforeMigration.GetEventSubscriptionForActivityIdAndEventName(activityId, eventName);
            Assert.NotNull(eventSubscriptionBefore,
                "Expected an event subscription for activity '" + activityId + "' before the migration"
            );

            foreach (var eventSubscription in SnapshotAfterMigration.EventSubscriptions)
                if (eventSubscriptionBefore.Id.Equals(eventSubscription.Id))
                    Assert.Fail("Expected event subscription '" + eventSubscriptionBefore.Id +
                                "' to be removed after migration");
        }

        public virtual void AssertEventSubscriptionCreated(string activityId, string eventName)
        {
            var eventSubscriptionAfter = SnapshotAfterMigration.GetEventSubscriptionForActivityIdAndEventName(
                activityId, eventName);
            Assert.NotNull(eventSubscriptionAfter,
                "Expected an event subscription for activity '" + activityId + "' after the migration"
            );

            foreach (var eventSubscription in SnapshotBeforeMigration.EventSubscriptions)
                if (eventSubscriptionAfter.Id.Equals(eventSubscription.Id))
                    Assert.Fail("Expected event subscription '" + eventSubscriptionAfter.Id +
                                "' to be created after migration");
        }

        public virtual void AssertTimerJob(IJob job)
        {
            Assert.True( job is TimerEntity, "Expected job to be a timer job");
        }

        public virtual IJob AssertTimerJobExists(ProcessInstanceSnapshot snapshot)
        {
            var jobs = snapshot.Jobs;
            Assert.AreEqual(1, jobs.Count);
            var job = jobs[0];
            AssertTimerJob(job);
            return job;
        }

        public virtual void AssertJobCreated(string activityId, string handlerType)
        {
            var jobDefinitionAfter = SnapshotAfterMigration.GetJobDefinitionForActivityIdAndType(activityId, handlerType);
            Assert.NotNull(jobDefinitionAfter,
                "Expected that a job definition for activity '" + activityId + "' exists after migration"
            );

            var jobAfter = SnapshotAfterMigration.GetJobForDefinitionId(jobDefinitionAfter.Id);
            Assert.NotNull(jobAfter, "Expected that a job for activity '" + activityId + "' exists after migration");
            AssertTimerJob(jobAfter);
            Assert.AreEqual(jobDefinitionAfter.ProcessDefinitionId, jobAfter.ProcessDefinitionId);
            Assert.AreEqual(jobDefinitionAfter.ProcessDefinitionKey, jobAfter.ProcessDefinitionKey);

            foreach (var job in SnapshotBeforeMigration.Jobs)
                if (jobAfter.Id.Equals(job.Id))
                    Assert.Fail("Expected job '" + jobAfter.Id + "' to be created first after migration");
        }

        public virtual void AssertJobRemoved(string activityId, string handlerType)
        {
            var jobDefinitionBefore = SnapshotBeforeMigration.GetJobDefinitionForActivityIdAndType(activityId,
                handlerType);
            Assert.NotNull(jobDefinitionBefore,
                "Expected that a job definition for activity '" + activityId + "' exists before migration"
            );

            var jobBefore = SnapshotBeforeMigration.GetJobForDefinitionId(jobDefinitionBefore.Id);
            Assert.NotNull(jobBefore, "Expected that a job for activity '" + activityId + "' exists before migration");
            AssertTimerJob(jobBefore);

            foreach (var job in SnapshotAfterMigration.Jobs)
                if (jobBefore.Id.Equals(job.Id))
                    Assert.Fail("Expected job '" + jobBefore.Id + "' to be removed after migration");
        }

        public virtual void AssertJobMigrated(string activityIdBefore, string activityIdAfter, string handlerType)
        {
            var jobDefinitionBefore = SnapshotBeforeMigration.GetJobDefinitionForActivityIdAndType(activityIdBefore,
                handlerType);
            Assert.NotNull(jobDefinitionBefore,
                "Expected that a job definition for activity '" + activityIdBefore + "' exists before migration"
            );

            var jobBefore = SnapshotBeforeMigration.GetJobForDefinitionId(jobDefinitionBefore.Id);
            Assert.NotNull(jobBefore,
                "Expected that a timer job for activity '" + activityIdBefore + "' exists before migration"
            );

            AssertJobMigrated(jobBefore, activityIdAfter, (DateTime)jobBefore.Duedate);
        }

        public virtual void AssertJobMigrated(IJob jobBefore, string activityIdAfter)
        {
            AssertJobMigrated(jobBefore, activityIdAfter, (DateTime)jobBefore.Duedate);
        }

        public virtual void AssertJobMigrated(IJob jobBefore, string activityIdAfter, DateTime dueDateAfter)
        {
            var jobAfter = SnapshotAfterMigration.GetJobById(jobBefore.Id);
            Assert.NotNull(jobAfter, "Expected that a job with id '" + jobBefore.Id + "' exists after migration");

            var jobDefinitionAfter = SnapshotAfterMigration.GetJobDefinitionForActivityIdAndType(activityIdAfter,
                ((JobEntity) jobBefore).JobHandlerType);
            Assert.NotNull(jobDefinitionAfter,
                "Expected that a job definition for activity '" + activityIdAfter + "' exists after migration"
            );

            Assert.AreEqual(jobBefore.Id, jobAfter.Id);
            Assert.AreEqual(
                "Expected that job is assigned to job definition '" + jobDefinitionAfter.Id + "' after migration",
                jobDefinitionAfter.Id, jobAfter.JobDefinitionId);
            Assert.AreEqual(
                "Expected that job is assigned to deployment '" + SnapshotAfterMigration.DeploymentId +
                "' after migration", SnapshotAfterMigration.DeploymentId, jobAfter.DeploymentId);
            Assert.AreEqual(dueDateAfter, jobAfter.Duedate);
            //Assert.AreEqual(((JobEntity) jobBefore).Type, ((JobEntity) jobAfter).Type);
            Assert.AreEqual(jobBefore.Priority, jobAfter.Priority);
            Assert.AreEqual(jobDefinitionAfter.ProcessDefinitionId, jobAfter.ProcessDefinitionId);
            Assert.AreEqual(jobDefinitionAfter.ProcessDefinitionKey, jobAfter.ProcessDefinitionKey);
        }

        public virtual void AssertBoundaryTimerJobCreated(string activityId)
        {
            AssertJobCreated(activityId, TimerExecuteNestedActivityJobHandler.TYPE);
        }

        public virtual void AssertBoundaryTimerJobRemoved(string activityId)
        {
            AssertJobRemoved(activityId, TimerExecuteNestedActivityJobHandler.TYPE);
        }

        public virtual void AssertBoundaryTimerJobMigrated(string activityIdBefore, string activityIdAfter)
        {
            AssertJobMigrated(activityIdBefore, activityIdAfter, TimerExecuteNestedActivityJobHandler.TYPE);
        }

        public virtual void AssertIntermediateTimerJobCreated(string activityId)
        {
            AssertJobCreated(activityId, TimerCatchIntermediateEventJobHandler.TYPE);
        }

        public virtual void AssertIntermediateTimerJobRemoved(string activityId)
        {
            AssertJobRemoved(activityId, TimerCatchIntermediateEventJobHandler.TYPE);
        }

        public virtual void AssertIntermediateTimerJobMigrated(string activityIdBefore, string activityIdAfter)
        {
            AssertJobMigrated(activityIdBefore, activityIdAfter, TimerCatchIntermediateEventJobHandler.TYPE);
        }

        public virtual void AssertEventSubProcessTimerJobCreated(string activityId)
        {
            AssertJobCreated(activityId, TimerStartEventSubprocessJobHandler.TYPE);
        }

        public virtual void AssertEventSubProcessTimerJobRemoved(string activityId)
        {
            AssertJobRemoved(activityId, TimerStartEventSubprocessJobHandler.TYPE);
        }

        public virtual void AssertVariableMigratedToExecution(IVariableInstance variableBefore, string executionId)
        {
            AssertVariableMigratedToExecution(variableBefore, executionId, variableBefore.ActivityInstanceId);
        }

        public virtual void AssertVariableMigratedToExecution(IVariableInstance variableBefore, string executionId,
            string activityInstanceId)
        {
            var variableAfter = SnapshotAfterMigration.GetVariable(variableBefore.Id);

            Assert.NotNull(variableAfter, "Variable with id " + variableBefore.Id + " does not exist");

            Assert.AreEqual(activityInstanceId, variableAfter.ActivityInstanceId);
            Assert.AreEqual(variableBefore.CaseExecutionId, variableAfter.CaseExecutionId);
            Assert.AreEqual(variableBefore.CaseInstanceId, variableAfter.CaseInstanceId);
            Assert.AreEqual(variableBefore.ErrorMessage, variableAfter.ErrorMessage);
            Assert.AreEqual(executionId, variableAfter.ExecutionId);
            Assert.AreEqual(variableBefore.Id, variableAfter.Id);
            Assert.AreEqual(variableBefore.Name, variableAfter.Name);
            Assert.AreEqual(variableBefore.ProcessInstanceId, variableAfter.ProcessInstanceId);
            Assert.AreEqual(variableBefore.TaskId, variableAfter.TaskId);
            Assert.AreEqual(variableBefore.TenantId, variableAfter.TenantId);
            Assert.AreEqual(variableBefore.TypeName, variableAfter.TypeName);
            Assert.AreEqual(variableBefore.Value, variableAfter.Value);
        }

        public virtual void AssertSuperExecutionOfCaseInstance(string caseInstanceId, string expectedSuperExecutionId)
        {
            //CaseExecutionEntity calledInstance = (CaseExecutionEntity) processEngine.CaseService.CreateCaseInstanceQuery(c=>c.CaseInstanceId ==caseInstanceId).First();

            //Assert.AreEqual(expectedSuperExecutionId, calledInstance.SuperExecutionId);
        }

        public virtual void AssertSuperExecutionOfProcessInstance(string processInstance,
            string expectedSuperExecutionId)
        {
            //ExecutionEntity calledInstance = (ExecutionEntity) processEngine.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance).First();

            //Assert.AreEqual(expectedSuperExecutionId, calledInstance.SuperExecutionId);
        }
    }
}