using System;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class SetProcessDefinitionVersionCmdTest : PluggableProcessEngineTestCase
    {
        private const string TEST_PROCESS_WITH_PARALLEL_GATEWAY =
            "resources/bpmn/gateway/ParallelGatewayTest.TestForkJoin.bpmn20.xml";

        private const string TEST_PROCESS =
            "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.TestSetProcessDefinitionVersion.bpmn20.xml";

        private const string TEST_PROCESS_ACTIVITY_MISSING =
            "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.TestSetProcessDefinitionVersionActivityMissing.bpmn20.xml";

        private const string TEST_PROCESS_CALL_ACTIVITY =
            "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.WithCallActivity.bpmn20.xml";

        private const string TEST_PROCESS_USER_TASK_V1 =
            "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.TestSetProcessDefinitionVersionWithTask.bpmn20.xml";

        private const string TEST_PROCESS_USER_TASK_V2 =
            "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.TestSetProcessDefinitionVersionWithTaskV2.bpmn20.xml";

        private const string TEST_PROCESS_SERVICE_TASK_V1 =
            "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.TestSetProcessDefinitionVersionWithServiceTask.bpmn20.xml";

        private const string TEST_PROCESS_SERVICE_TASK_V2 =
            "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.TestSetProcessDefinitionVersionWithServiceTaskV2.bpmn20.xml";

        private const string TEST_PROCESS_WITH_MULTIPLE_PARENTS =
            "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.TestSetProcessDefinitionVersionWithMultipleParents.bpmn";

        private const string TEST_PROCESS_ONE_JOB =
            "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.oneJobProcess.bpmn20.xml";

        private const string TEST_PROCESS_TWO_JOBS =
            "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.TwoJobsProcess.bpmn20.xml";

        private const string TEST_PROCESS_ATTACHED_TIMER =
            "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.TestAttachedTimer.bpmn20.xml";

        [Test]
        public virtual void testSetProcessDefinitionVersionEmptyArguments()
        {
            try
            {
                new SetProcessDefinitionVersionCmd(null, 23);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("The process instance id is mandatory: ProcessInstanceId is null", ae.Message);
            }

            try
            {
                new SetProcessDefinitionVersionCmd("", 23);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("The process instance id is mandatory: ProcessInstanceId is empty", ae.Message);
            }

            try
            {
                new SetProcessDefinitionVersionCmd("42", null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("The process definition version is mandatory: processDefinitionVersion is null",
                    ae.Message);
            }

            try
            {
                new SetProcessDefinitionVersionCmd("42", -1);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent(
                    "The process definition version must be positive: processDefinitionVersion is not greater than 0",
                    ae.Message);
            }
        }

        [Test]
        public virtual void testSetProcessDefinitionVersionNonExistingPI()
        {
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            try
            {
                commandExecutor.Execute(new SetProcessDefinitionVersionCmd("42", 23));
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("No process instance found for id = '42'.", ae.Message);
            }
        }

        [Test][Deployment(new string[] {TEST_PROCESS_WITH_PARALLEL_GATEWAY}) ]
        public virtual void testSetProcessDefinitionVersionPIIsSubExecution()
        {
            // start process instance
            var pi = runtimeService.StartProcessInstanceByKey("forkJoin");

            var execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "receivePayment")
                .First();
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            var command = new SetProcessDefinitionVersionCmd(execution.Id, 1);
            try
            {
                commandExecutor.Execute(command);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent(
                    "A process instance id is required, but the provided id '" + execution.Id +
                    "' points to a child execution of process instance '" + pi.Id + "'. Please invoke the " +
                    command.GetType()
                        .Name + " with a root execution id.", ae.Message);
            }
        }

        [Test][Deployment(new string[] {TEST_PROCESS})]
        public virtual void testSetProcessDefinitionVersionNonExistingPD()
        {
            // start process instance
            var pi = runtimeService.StartProcessInstanceByKey("receiveTask");

            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            try
            {
                commandExecutor.Execute(new SetProcessDefinitionVersionCmd(pi.Id, 23));
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("no processes deployed with key = 'receiveTask', version = '23'", ae.Message);
            }
        }

        [Test]
        [Deployment(new string[] { TEST_PROCESS })]
        public virtual void testSetProcessDefinitionVersionActivityMissing()
        {
            // start process instance
            var pi = runtimeService.StartProcessInstanceByKey("receiveTask");

            // check that receive task has been reached
            var execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "waitState1")
                .First();
            Assert.NotNull(execution);

            // deploy new version of the process definition
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource(TEST_PROCESS_ACTIVITY_MISSING)
                .Deploy();
            Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery()
                .Count());

            // migrate process instance to new process definition version
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            var setProcessDefinitionVersionCmd = new SetProcessDefinitionVersionCmd(pi.Id, 2);
            try
            {
                commandExecutor.Execute(setProcessDefinitionVersionCmd);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent(
                    "The new process definition (key = 'receiveTask') does not contain the current activity (id = 'waitState1') of the process instance (id = '",
                    ae.Message);
            }

            // undeploy "manually" deployed process definition
            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        [Deployment]
        public virtual void testSetProcessDefinitionVersion()
        {
            // start process instance
            var pi = runtimeService.StartProcessInstanceByKey("receiveTask");

            // check that receive task has been reached
            var execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == pi.Id&& c.ActivityId =="waitState1")
                .First();
            Assert.NotNull(execution);

            // deploy new version of the process definition
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource(TEST_PROCESS)
                .Deploy();
            Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery()
                .Count());

            // migrate process instance to new process definition version
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new SetProcessDefinitionVersionCmd(pi.Id, 2));

            // signal process instance
            runtimeService.Signal(execution.Id);

            // check that the instance now uses the new process definition version
            var newProcessDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Version == 2)
                .First();
            pi = runtimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId == pi.Id)
                .First();
            Assert.AreEqual(newProcessDefinition.Id, pi.ProcessDefinitionId);

            // check history
            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                var historicPI = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId ==pi.Id)
                    .First();

                //      Assert.AreEqual(newProcessDefinition.GetId(), historicPI.GetProcessDefinitionId());
            }

            // undeploy "manually" deployed process definition
            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        [Deployment(new string[] { TEST_PROCESS_WITH_PARALLEL_GATEWAY })]
        public virtual void testSetProcessDefinitionVersionSubExecutions()
        {
            // start process instance
            var pi = runtimeService.StartProcessInstanceByKey("forkJoin");

            // check that the user tasks have been reached
            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());

            // deploy new version of the process definition
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource(TEST_PROCESS_WITH_PARALLEL_GATEWAY)
                .Deploy();
            Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery()
                .Count());

            // migrate process instance to new process definition version
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new SetProcessDefinitionVersionCmd(pi.Id, 2));

            // check that all executions of the instance now use the new process definition version
            var newProcessDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Version == 2)
                .First();
            var executions = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == pi.Id)
                
                .ToList();
            foreach (var execution in executions)
                Assert.AreEqual(newProcessDefinition.Id, ((ExecutionEntity) execution).ProcessDefinitionId);

            // undeploy "manually" deployed process definition
            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test][Deployment(new string[] {TEST_PROCESS_CALL_ACTIVITY}) ]
        public virtual void testSetProcessDefinitionVersionWithCallActivity()
        {
            // start process instance
            var pi = runtimeService.StartProcessInstanceByKey("parentProcess");

            // check that receive task has been reached
            var execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "waitState1")
                ////.ProcessDefinitionKey("childProcess")
                .First();
            Assert.NotNull(execution);

            // deploy new version of the process definition
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource(TEST_PROCESS_CALL_ACTIVITY)
                .Deploy();
            Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="parentProcess")
                .Count());

            // migrate process instance to new process definition version
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new SetProcessDefinitionVersionCmd(pi.Id, 2));

            // signal process instance
            runtimeService.Signal(execution.Id);

            // should be finished now
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId == pi.Id)
                .Count());

            // undeploy "manually" deployed process definition
            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test][Deployment(new string[] {TEST_PROCESS_USER_TASK_V1}) ]
        public virtual void testSetProcessDefinitionVersionWithWithTask()
        {
            try
            {
                // start process instance
                var pi = runtimeService.StartProcessInstanceByKey("userTask");

                // check that user task has been reached
                Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id)
                    .Count());

                // deploy new version of the process definition
                var deployment = repositoryService.CreateDeployment()
                    .AddClasspathResource(TEST_PROCESS_USER_TASK_V2)
                    .Deploy();
                Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="userTask")
                    .Count());

                var newProcessDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="userTask")
                    //.ProcessDefinitionVersion(2)
                    .First();

                // migrate process instance to new process definition version
                processEngineConfiguration.CommandExecutorTxRequired.Execute(new SetProcessDefinitionVersionCmd(pi.Id, 2));

                // check UserTask
                var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id)
                    .First();
                Assert.AreEqual(newProcessDefinition.Id, task.ProcessDefinitionId);
                Assert.AreEqual("testFormKey", formService.GetTaskFormData(task.Id)
                    .FormKey);

                // continue
                taskService.Complete(task.Id);

                AssertProcessEnded(pi.Id);

                // undeploy "manually" deployed process definition
                repositoryService.DeleteDeployment(deployment.Id, true);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.Write(ex.StackTrace);
            }
        }

        [Test][Deployment( TEST_PROCESS_SERVICE_TASK_V1) ]
        public virtual void testSetProcessDefinitionVersionWithFollowUpTask()
        {
            var processDefinitionId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;

            var secondDeploymentId = repositoryService.CreateDeployment()
                .AddClasspathResource(TEST_PROCESS_SERVICE_TASK_V2)
                .Deploy()
                .Id;

            runtimeService.StartProcessInstanceById(processDefinitionId);

            // execute job that triggers the migrating service task
            var migrationJob = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(migrationJob);

            managementService.ExecuteJob(migrationJob.Id);

            var followUpTask = taskService.CreateTaskQuery()
                .First();

            Assert.NotNull(followUpTask,
                "Should have migrated to the new version and immediately executed the correct follow-up activity"
            );

            repositoryService.DeleteDeployment(secondDeploymentId, true);
        }
        [Deployment(new string[] {TEST_PROCESS_WITH_MULTIPLE_PARENTS}) ]
        public virtual void testSetProcessDefinitionVersionWithMultipleParents()
        {
            // start process instance
            var pi = runtimeService.StartProcessInstanceByKey("multipleJoins");

            // check that the user tasks have been reached
            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());

            //finish task1
            var task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task1")
                .First();
            taskService.Complete(task.Id);

            //we have reached task4
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task4")
                .First();
            Assert.NotNull(task);

            //The timer job has been created
            var job = managementService.CreateJobQuery(c=>c.ExecutionId ==task.ExecutionId)
                .First();
            Assert.NotNull(job);

            // check there are 2 user tasks task4 and task2
            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());

            // deploy new version of the process definition
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource(TEST_PROCESS_WITH_MULTIPLE_PARENTS)
                .Deploy();
            Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery()
                .Count());

            // migrate process instance to new process definition version
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new SetProcessDefinitionVersionCmd(pi.Id, 2));

            // check that all executions of the instance now use the new process definition version
            var newProcessDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Version == 2)
                .First();
            var executions = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == pi.Id)
                
                .ToList();
            foreach (var execution in executions)
                Assert.AreEqual(newProcessDefinition.Id, ((ExecutionEntity) execution).ProcessDefinitionId);

            // undeploy "manually" deployed process definition
            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test][Deployment(TEST_PROCESS_ONE_JOB) ]
        public virtual void testSetProcessDefinitionVersionMigrateJob()
        {
            // given a process instance
            var instance = runtimeService.StartProcessInstanceByKey("oneJobProcess");

            // with a job
            var job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);

            // and a second deployment of the process
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource(TEST_PROCESS_ONE_JOB)
                .Deploy();

            var newDefinition = repositoryService.CreateProcessDefinitionQuery()
                //.DeploymentId(deployment.Id)
                .First();
            Assert.NotNull(newDefinition);

            // when the process instance is migrated
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new SetProcessDefinitionVersionCmd(instance.Id, 2));

            // then the the job should also be migrated
            var migratedJob = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(migratedJob);
            Assert.AreEqual(job.Id, migratedJob.Id);
            Assert.AreEqual(newDefinition.Id, migratedJob.ProcessDefinitionId);
            Assert.AreEqual(deployment.Id, migratedJob.DeploymentId);

            var newJobDefinition = managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionId==newDefinition.Id)
                .First();
            Assert.NotNull(newJobDefinition);
            Assert.AreEqual(newJobDefinition.Id, migratedJob.JobDefinitionId);

            repositoryService.DeleteDeployment(deployment.Id, true);
        }
        [Deployment( TEST_PROCESS_TWO_JOBS)]
        public virtual void testMigrateJobWithMultipleDefinitionsOnActivity()
        {
            // given a process instance
            var asyncAfterInstance = runtimeService.StartProcessInstanceByKey("twoJobsProcess");

            // with an async after job
            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;
            managementService.ExecuteJob(jobId);
            var asyncAfterJob = managementService.CreateJobQuery()
                .First();

            // and a process instance with an before after job
            var asyncBeforeInstance = runtimeService.StartProcessInstanceByKey("twoJobsProcess");
            var asyncBeforeJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== asyncBeforeInstance.Id)
                .First();

            // and a second deployment of the process
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource(TEST_PROCESS_TWO_JOBS)
                .Deploy();

            var newDefinition = repositoryService.CreateProcessDefinitionQuery()
                //.DeploymentId(deployment.Id)
                .First();
            Assert.NotNull(newDefinition);

            var asnycBeforeJobDefinition = managementService.CreateJobDefinitionQuery()
                //.JobConfiguration(MessageJobDeclaration.AsyncBefore)
               // .ProcessDefinitionId(newDefinition.Id)
                .First();
            var asnycAfterJobDefinition = managementService.CreateJobDefinitionQuery()
                //.JobConfiguration(MessageJobDeclaration.AsyncAfter)
                //.ProcessDefinitionId(newDefinition.Id)
                .First();

            Assert.NotNull(asnycBeforeJobDefinition);
            Assert.NotNull(asnycAfterJobDefinition);

            // when the process instances are migrated
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new SetProcessDefinitionVersionCmd(asyncBeforeInstance.Id, 2));
            commandExecutor.Execute(new SetProcessDefinitionVersionCmd(asyncAfterInstance.Id, 2));

            // then the the job's definition reference should also be migrated
            var migratedAsyncBeforeJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== asyncBeforeInstance.Id)
                .First();
            Assert.AreEqual(asyncBeforeJob.Id, migratedAsyncBeforeJob.Id);
            Assert.NotNull(migratedAsyncBeforeJob);
            Assert.AreEqual(asnycBeforeJobDefinition.Id, migratedAsyncBeforeJob.JobDefinitionId);

            var migratedAsyncAfterJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== asyncAfterInstance.Id)
                .First();
            Assert.AreEqual(asyncAfterJob.Id, migratedAsyncAfterJob.Id);
            Assert.NotNull(migratedAsyncAfterJob);
            Assert.AreEqual(asnycAfterJobDefinition.Id, migratedAsyncAfterJob.JobDefinitionId);

            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test][ Deployment(TEST_PROCESS_ONE_JOB)]
        public virtual void testSetProcessDefinitionVersionMigrateIncident()
        {
            // given a process instance
            var instance = runtimeService.StartProcessInstanceByKey("oneJobProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("shouldFail", true));

            // with a failed job
            ExecuteAvailableJobs();

            // and an incident
            var incident = runtimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(incident);

            // and a second deployment of the process
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource(TEST_PROCESS_ONE_JOB)
                .Deploy();

            var newDefinition = repositoryService.CreateProcessDefinitionQuery()
                //.DeploymentId(deployment.Id)
                .First();
            Assert.NotNull(newDefinition);

            // when the process instance is migrated
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new SetProcessDefinitionVersionCmd(instance.Id, 2));

            // then the the incident should also be migrated
            var migratedIncident = runtimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(migratedIncident);
            Assert.AreEqual(newDefinition.Id, migratedIncident.ProcessDefinitionId);
            Assert.AreEqual(instance.Id, migratedIncident.ProcessInstanceId);
            Assert.AreEqual(instance.Id, migratedIncident.ExecutionId);

            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test][Deployment( TEST_PROCESS_ATTACHED_TIMER)]
        public virtual void testSetProcessDefinitionVersionAttachedTimer()
        {
            // given a process instance
            var instance = runtimeService.StartProcessInstanceByKey("attachedTimer");

            // and a second deployment of the process
            var deployment = repositoryService.CreateDeployment()
                .AddClasspathResource(TEST_PROCESS_ATTACHED_TIMER)
                .Deploy();

            var newDefinition = repositoryService.CreateProcessDefinitionQuery()
                //.DeploymentId(deployment.Id)
                .First();
            Assert.NotNull(newDefinition);

            // when the process instance is migrated
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new SetProcessDefinitionVersionCmd(instance.Id, 2));

            var job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);
            Assert.AreEqual(newDefinition.Id, job.ProcessDefinitionId);

            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        public virtual void testHistoryOfSetProcessDefinitionVersionCmd()
        {
            // given
            var resource = "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.bpmn";

            // Deployments
            var firstDeployment = repositoryService.CreateDeployment()
                .AddClasspathResource(resource)
                .Deploy();

            var secondDeployment = repositoryService.CreateDeployment()
                .AddClasspathResource(resource)
                .Deploy();

            // Process definitions
            var processDefinitionV1 = repositoryService.CreateProcessDefinitionQuery()
                //.DeploymentId(firstDeployment.Id)
                .First();

            var processDefinitionV2 = repositoryService.CreateProcessDefinitionQuery()
                //.DeploymentId(secondDeployment.Id)
                .First();

            // start process instance
            var processInstance = runtimeService.StartProcessInstanceById(processDefinitionV1.Id);

            // when
            setProcessDefinitionVersion(processInstance.Id, 2);

            // then
            var processInstanceAfterMigration = runtimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId == processInstance.Id)
                .First();
            Assert.AreEqual(processDefinitionV2.Id, processInstanceAfterMigration.ProcessDefinitionId);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                var historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId ==processInstance.Id)
                    .First();
                Assert.AreEqual(processDefinitionV2.Id, historicProcessInstance.ProcessDefinitionId);
            }

            // Clean up the test
            repositoryService.DeleteDeployment(firstDeployment.Id, true);
            repositoryService.DeleteDeployment(secondDeployment.Id, true);
        }

        [Test]
        public virtual void testOpLogSetProcessDefinitionVersionCmd()
        {
            // given
            try
            {
                identityService.AuthenticatedUserId = "demo";
                var resource = "resources/api/runtime/migration/SetProcessDefinitionVersionCmdTest.bpmn";

                // Deployments
                var firstDeployment = repositoryService.CreateDeployment()
                    .AddClasspathResource(resource)
                    .Deploy();

                var secondDeployment = repositoryService.CreateDeployment()
                    .AddClasspathResource(resource)
                    .Deploy();

                // Process definitions
                var processDefinitionV1 = repositoryService.CreateProcessDefinitionQuery()
                    //.DeploymentId(firstDeployment.Id)
                    .First();

                var processDefinitionV2 = repositoryService.CreateProcessDefinitionQuery()
                    //.DeploymentId(secondDeployment.Id)
                    .First();

                // start process instance
                var processInstance = runtimeService.StartProcessInstanceById(processDefinitionV1.Id);

                // when
                setProcessDefinitionVersion(processInstance.Id, 2);

                // then
                var processInstanceAfterMigration = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId==processInstance.Id)
                    .First();
                Assert.AreEqual(processDefinitionV2.Id, processInstanceAfterMigration.ProcessDefinitionId);

                if (processEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelFull))
                {
                    var userOperations = historyService.CreateUserOperationLogQuery()
                        .Where(c=>c.ProcessInstanceId==processInstance.Id)
                        
                        .ToList();

                    Assert.AreEqual(1, userOperations.Count);

                    var userOperationLogEntry = userOperations[0];
                    Assert.AreEqual(UserOperationLogEntryFields.OperationTypeModifyProcessInstance,
                        userOperationLogEntry.OperationType);
                    Assert.AreEqual("processDefinitionVersion", userOperationLogEntry.Property);
                    Assert.AreEqual("1", userOperationLogEntry.OrgValue);
                    Assert.AreEqual("2", userOperationLogEntry.NewValue);
                }

                // Clean up the test
                repositoryService.DeleteDeployment(firstDeployment.Id, true);
                repositoryService.DeleteDeployment(secondDeployment.Id, true);
            }
            finally
            {
                identityService.ClearAuthentication();
            }
        }

        protected internal virtual void setProcessDefinitionVersion(string ProcessInstanceId,
            int newProcessDefinitionVersion)
        {
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
            commandExecutor.Execute(new SetProcessDefinitionVersionCmd(ProcessInstanceId, newProcessDefinitionVersion));
        }
    }
}