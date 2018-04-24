using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit) ]
    [TestFixture]
    public class HistoricVariableInstanceScopeTest : PluggableProcessEngineTestCase
    {
        [Test][Deployment("resources/api/oneTaskProcess.bpmn20.xml") ]
        public virtual void testSetVariableOnProcessInstanceStart()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["testVar"] = "testValue";
            var pi = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

            var query = historyService.CreateHistoricVariableInstanceQuery(c=>c.ProcessInstanceId == pi.Id);
            Assert.AreEqual(1, query.Count());

            var variable = query.First();
            Assert.NotNull(variable);

            // the variable is in the process instance scope
            Assert.AreEqual(pi.Id, variable.ActivityInstanceId);

            taskService.Complete(taskService.CreateTaskQuery()
                .First()
                .Id);
            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSetVariableLocalOnUserTask()
        {
            var pi = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            taskService.SetVariableLocal(task.Id, "testVar", "testValue");
            var taskExecution = (ExecutionEntity) runtimeService.CreateExecutionQuery(c=>c.Id==task.ExecutionId)
                .First();
            Assert.NotNull(taskExecution);

            var query = historyService.CreateHistoricVariableInstanceQuery();
            Assert.AreEqual(1, query.Count());

            var variable = query.First();
            Assert.NotNull(variable);

            // the variable is in the task scope
            Assert.AreEqual(taskExecution.ActivityInstanceId, variable.ActivityInstanceId);

            taskService.Complete(task.Id);
            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSetVariableOnProcessIntanceStartAndSetVariableLocalOnUserTask()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["testVar"] = "testValue";
            var pi = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            taskService.SetVariableLocal(task.Id, "testVar", "anotherTestValue");
            var taskExecution = (ExecutionEntity) runtimeService.CreateExecutionQuery()
                .First();
            Assert.NotNull(taskExecution);

            var query = historyService.CreateHistoricVariableInstanceQuery();
            Assert.AreEqual(2, query.Count());

            var result = query
                .ToList();

            var firstVar = result[0];
            Assert.AreEqual("testVar", firstVar.VariableName);
            Assert.AreEqual("testValue", firstVar.Value);
            // the variable is in the process instance scope
            Assert.AreEqual(pi.Id, firstVar.ActivityInstanceId);

            var secondVar = result[1];
            Assert.AreEqual("testVar", secondVar.VariableName);
            Assert.AreEqual("anotherTestValue", secondVar.Value);
            // the variable is in the task scope
            Assert.AreEqual(taskExecution.ActivityInstanceId, secondVar.ActivityInstanceId);

            taskService.Complete(task.Id);
            AssertProcessEnded(pi.Id);
        }

        [Test][Deployment("resources/api/oneSubProcess.bpmn20.xml") ]
        public virtual void testSetVariableOnUserTaskInsideSubProcess()
        {
            var pi = runtimeService.StartProcessInstanceByKey("startSimpleSubProcess");

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            taskService.SetVariable(task.Id, "testVar", "testValue");

            var query = historyService.CreateHistoricVariableInstanceQuery();
            Assert.AreEqual(1, query.Count());

            var variable = query.First();
            // the variable is in the process instance scope
            Assert.AreEqual(pi.Id, variable.ActivityInstanceId);

            taskService.Complete(task.Id);
            AssertProcessEnded(pi.Id);
        }

        [Test][Deployment("resources/history/HistoricVariableInstanceScopeTest.TestSetVariableLocalOnTaskInsideParallelBranch.bpmn") ]
        public virtual void testSetVariableOnTaskInsideParallelBranch()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            taskService.SetVariable(task.Id, "testVar", "testValue");

            var query = historyService.CreateHistoricVariableInstanceQuery();
            Assert.AreEqual(1, query.Count());

            var variable = query.First();
            // the variable is in the process instance scope
            Assert.AreEqual(pi.Id, variable.ActivityInstanceId);

            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCmmnActivityInstanceIdOnCaseExecution()
        {
            // given
            caseService.CreateCaseInstanceByKey("oneTaskCase");

            var taskExecutionId = caseService.CreateCaseExecutionQuery(c=> c.ActivityId =="PI_HumanTask_1")
                .First()
                .Id;

            // when
            caseService.WithCaseExecution(taskExecutionId)
                .SetVariableLocal("foo", "bar")
                .Execute();

            // then
            var variable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("foo")
                .First();

            Assert.NotNull(variable);
            Assert.AreEqual(taskExecutionId, variable.ActivityInstanceId);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var variableDetail = historyService.CreateHistoricDetailQuery()
                    /*.VariableUpdates()*/
                    ////.VariableInstanceId(variable.Id)
                    .First();
                Assert.AreEqual(taskExecutionId, variableDetail.ActivityInstanceId);
            }
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCmmnActivityInstanceIdOnCaseInstance()
        {
            // given
            var caseInstance = caseService.CreateCaseInstanceByKey("oneTaskCase");

            var taskExecutionId = caseService.CreateCaseExecutionQuery(c=> c.ActivityId =="PI_HumanTask_1")
                .First()
                .Id;

            // when
            caseService.WithCaseExecution(taskExecutionId)
                .SetVariable("foo", "bar")
                .Execute();

            // then
            var variable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("foo")
                .First();

            Assert.NotNull(variable);
            Assert.AreEqual(caseInstance.Id, variable.ActivityInstanceId);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var variableDetail = historyService.CreateHistoricDetailQuery()
                    /*.VariableUpdates()*/
                    ////.VariableInstanceId(variable.Id)
                    .First();
                Assert.AreEqual(taskExecutionId, variableDetail.ActivityInstanceId);
            }
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCmmnActivityInstanceIdOnTask()
        {
            // given
            var caseInstance = caseService.CreateCaseInstanceByKey("oneTaskCase");

            var taskExecutionId = caseService.CreateCaseExecutionQuery(c=> c.ActivityId =="PI_HumanTask_1")
                .First()
                .Id;

            var task = taskService.CreateTaskQuery()
                .First();

            // when
            taskService.SetVariable(task.Id, "foo", "bar");

            // then
            var variable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("foo")
                .First();

            Assert.NotNull(variable);
            Assert.AreEqual(caseInstance.Id, variable.ActivityInstanceId);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var variableDetail = historyService.CreateHistoricDetailQuery()
                    /*.VariableUpdates()*/
                    ////.VariableInstanceId(variable.Id)
                    .First();
                Assert.AreEqual(taskExecutionId, variableDetail.ActivityInstanceId);
            }
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testHistoricCaseVariableInstanceQuery()
        {
            // start case instance with variables
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["foo"] = "bar";
            var caseInstanceId = caseService.CreateCaseInstanceByKey("oneTaskCase", variables)
                .Id;

            var caseExecutionId = caseService.CreateCaseExecutionQuery(c=> c.ActivityId =="CasePlanModel_1")
                .First()
                .Id;
            var taskExecutionId = caseService.CreateCaseExecutionQuery(c=> c.ActivityId =="PI_HumanTask_1")
                .First()
                .Id;

            // set variable on both executions
            caseService.SetVariableLocal(caseExecutionId, "case", "execution");
            caseService.SetVariableLocal(taskExecutionId, "task", "execution");

            // update variable on both executions
            caseService.SetVariableLocal(caseExecutionId, "case", "update");
            caseService.SetVariableLocal(taskExecutionId, "task", "update");

            Assert.AreEqual(3, historyService.CreateHistoricVariableInstanceQuery()
                .Count());
            Assert.AreEqual(3, historyService.CreateHistoricVariableInstanceQuery(c=>c.CaseInstanceId== caseInstanceId)
                .Count());
            Assert.AreEqual(3, historyService.CreateHistoricVariableInstanceQuery()
                //.CaseExecutionIdIn(caseExecutionId, taskExecutionId)
                .Count());
            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery()
                //.CaseExecutionIdIn(caseExecutionId)
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.CaseExecutionIdIn(taskExecutionId)
                .Count());

            var historyLevel = processEngineConfiguration.HistoryLevel;
            if (historyLevel.Equals(HistoryLevelFields.HistoryLevelFull))
            {
                Assert.AreEqual(5, historyService.CreateHistoricDetailQuery()
                    .Count());
                Assert.AreEqual(5, historyService.CreateHistoricDetailQuery()
                    //.CaseInstanceId(caseInstanceId)
                    .Count());
                Assert.AreEqual(3, historyService.CreateHistoricDetailQuery()
                    //.CaseExecutionId(caseExecutionId)
                    .Count());
                Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                    //.CaseExecutionId(taskExecutionId)
                    .Count());
            }
        }

        [Test]
        [Deployment]
        public virtual void testInputMappings()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            var activityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId== ProcessInstanceId);

            var theService1Id = activityInstanceQuery.Where(c=>c.ActivityId == "theService1")
                .First()
                .Id;
            var theService2Id = activityInstanceQuery.Where(c=>c.ActivityId == "theService2")
                .First()
                .Id;
            var theTaskId = activityInstanceQuery.Where(c=>c.ActivityId == "theTask")
                .First()
                .Id;

            // when (1)
            var firstVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("firstInputVariable")
                .First();

            // then (1)
            Assert.AreEqual(theService1Id, firstVariable.ActivityInstanceId);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var firstVariableDetail = historyService.CreateHistoricDetailQuery()
                    /*.VariableUpdates()*/
                    //////.VariableInstanceId(firstVariable.Id)
                    .First();
                Assert.AreEqual(theService1Id, firstVariableDetail.ActivityInstanceId);
            }

            // when (2)
            var secondVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("secondInputVariable")
                .First();

            // then (2)
            Assert.AreEqual(theService2Id, secondVariable.ActivityInstanceId);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var secondVariableDetail = historyService.CreateHistoricDetailQuery()
                    /*.VariableUpdates()*/
                    ////.VariableInstanceId(secondVariable.Id)
                    .First();
                Assert.AreEqual(theService2Id, secondVariableDetail.ActivityInstanceId);
            }

            // when (3)
            var thirdVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("thirdInputVariable")
                .First();

            // then (3)
            Assert.AreEqual(theTaskId, thirdVariable.ActivityInstanceId);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var thirdVariableDetail = historyService.CreateHistoricDetailQuery()
                    /*.VariableUpdates()*/
                    //////.VariableInstanceId(thirdVariable.Id)
                    .First();
                Assert.AreEqual(theTaskId, thirdVariableDetail.ActivityInstanceId);
            }
        }

        [Test]
        [Deployment]
        public virtual void testSetVariableLocalOnServiceTaskInsideParallelBranch()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var serviceTask = historyService.CreateHistoricActivityInstanceQuery(c=> c.ActivityId =="serviceTask1")
                .First();
            Assert.NotNull(serviceTask);

            var query = historyService.CreateHistoricVariableInstanceQuery();
            Assert.AreEqual(1, query.Count());

            var variable = query.First();
            // the variable is in the service task scope
            Assert.AreEqual(serviceTask.Id, variable.ActivityInstanceId);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testSetVariableLocalOnServiceTaskInsideSubProcess()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var query = historyService.CreateHistoricVariableInstanceQuery();
            Assert.AreEqual(1, query.Count());

            var activityInstanceId = historyService.CreateHistoricActivityInstanceQuery(c=> c.ActivityId =="SubProcess_1")
                .First()
                .Id;

            var variable = query.First();
            // the variable is in the sub process scope
            Assert.AreEqual(activityInstanceId, variable.ActivityInstanceId);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testSetVariableLocalOnTaskInsideParallelBranch()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            taskService.SetVariableLocal(task.Id, "testVar", "testValue");
            var taskExecution = (ExecutionEntity) runtimeService.CreateExecutionQuery(c=>c.Id==task.ExecutionId)
                .First();
            Assert.NotNull(taskExecution);

            var query = historyService.CreateHistoricVariableInstanceQuery();
            Assert.AreEqual(1, query.Count());

            var variable = query.First();
            // the variable is in the IUser task scope
            Assert.AreEqual(taskExecution.ActivityInstanceId, variable.ActivityInstanceId);

            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testSetVariableOnServiceTaskInsideParallelBranch()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var query = historyService.CreateHistoricVariableInstanceQuery();
            Assert.AreEqual(1, query.Count());

            var variable = query.First();
            // the variable is in the process instance scope
            Assert.AreEqual(pi.Id, variable.ActivityInstanceId);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testSetVariableOnServiceTaskInsideSubProcess()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var query = historyService.CreateHistoricVariableInstanceQuery();
            Assert.AreEqual(1, query.Count());

            var variable = query.First();
            // the variable is in the process instance scope
            Assert.AreEqual(pi.Id, variable.ActivityInstanceId);

            AssertProcessEnded(pi.Id);
        }
    }
}