using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ProcessInstanceSuspensionTest : PluggableProcessEngineTestCase
    {
        [Test][Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testProcessInstanceActiveByDefault()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceByKey(processDefinition.Key);

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.IsFalse(processInstance.IsSuspended);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testSuspendActivateProcessInstance()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceByKey(processDefinition.Key);

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.IsFalse(processInstance.IsSuspended);

            //suspend
            runtimeService.SuspendProcessInstanceById(processInstance.Id);
            processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.True(processInstance.IsSuspended);

            //activate
            runtimeService.ActivateProcessInstanceById(processInstance.Id);
            processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.IsFalse(processInstance.IsSuspended);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testSuspendActivateProcessInstanceByProcessDefinitionId()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceByKey(processDefinition.Key);

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.IsFalse(processInstance.IsSuspended);

            //suspend
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);
            processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.True(processInstance.IsSuspended);

            //activate
            runtimeService.ActivateProcessInstanceByProcessDefinitionId(processDefinition.Id);
            processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.IsFalse(processInstance.IsSuspended);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testSuspendActivateProcessInstanceByProcessDefinitionKey()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceByKey(processDefinition.Key);

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.IsFalse(processInstance.IsSuspended);

            //suspend
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.True(processInstance.IsSuspended);

            //activate
            runtimeService.ActivateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.IsFalse(processInstance.IsSuspended);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testActivateAlreadyActiveProcessInstance()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceByKey(processDefinition.Key);

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.IsFalse(processInstance.IsSuspended);

            try
            {
                //activate
                runtimeService.ActivateProcessInstanceById(processInstance.Id);
                processInstance = runtimeService.CreateProcessInstanceQuery()
                    .First();
                Assert.IsFalse(processInstance.IsSuspended);
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("Should not Assert.Fail");
            }

            try
            {
                //activate
                runtimeService.ActivateProcessInstanceByProcessDefinitionId(processDefinition.Id);
                processInstance = runtimeService.CreateProcessInstanceQuery()
                    .First();
                Assert.IsFalse(processInstance.IsSuspended);
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("Should not Assert.Fail");
            }

            try
            {
                //activate
                runtimeService.ActivateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
                processInstance = runtimeService.CreateProcessInstanceQuery()
                    .First();
                Assert.IsFalse(processInstance.IsSuspended);
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("Should not Assert.Fail");
            }
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testSuspendAlreadySuspendedProcessInstance()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceByKey(processDefinition.Key);

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.IsFalse(processInstance.IsSuspended);

            runtimeService.SuspendProcessInstanceById(processInstance.Id);

            try
            {
                runtimeService.SuspendProcessInstanceById(processInstance.Id);
                processInstance = runtimeService.CreateProcessInstanceQuery()
                    .First();
                Assert.True(processInstance.IsSuspended);
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("Should not Assert.Fail");
            }

            try
            {
                runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);
                processInstance = runtimeService.CreateProcessInstanceQuery()
                    .First();
                Assert.True(processInstance.IsSuspended);
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("Should not Assert.Fail");
            }

            try
            {
                runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
                processInstance = runtimeService.CreateProcessInstanceQuery()
                    .First();
                Assert.True(processInstance.IsSuspended);
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("Should not Assert.Fail");
            }
        }

        [Test][Deployment(new string[]{ "resources/api/runtime/superProcessWithMultipleNestedSubProcess.bpmn20.xml", "resources/api/runtime/nestedSubProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml" })]
        public virtual void testQueryForActiveAndSuspendedProcessInstances()
        {
            runtimeService.StartProcessInstanceByKey("nestedSubProcessQueryTest");

            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
                .Count());
            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());

            var piToSuspend = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("nestedSubProcessQueryTest")
                .First();
            runtimeService.SuspendProcessInstanceById(piToSuspend.Id);

            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
                .Count());
            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());

            Assert.AreEqual(piToSuspend.Id, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .First()
                .Id);
        }
        [Test]
        [Deployment(new string[]{ "resources/api/runtime/superProcessWithMultipleNestedSubProcess.bpmn20.xml", "resources/api/runtime/nestedSubProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml" }) ]
        public virtual void testQueryForActiveAndSuspendedProcessInstancesByProcessDefinitionId()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="nestedSubProcessQueryTest")
                .First();

            runtimeService.StartProcessInstanceByKey("nestedSubProcessQueryTest");

            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
                .Count());
            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());

            var piToSuspend = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("nestedSubProcessQueryTest")
                .First();
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
                .Count());
            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());

            Assert.AreEqual(piToSuspend.Id, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .First()
                .Id);
        }

        [Test][Deployment(new string[]{ "resources/api/runtime/superProcessWithMultipleNestedSubProcess.bpmn20.xml", "resources/api/runtime/nestedSubProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml" })]
        public virtual void testQueryForActiveAndSuspendedProcessInstancesByProcessDefinitionKey()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="nestedSubProcessQueryTest")
                .First();

            runtimeService.StartProcessInstanceByKey("nestedSubProcessQueryTest");

            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
                .Count());
            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());

            var piToSuspend = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("nestedSubProcessQueryTest")
                .First();
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

            Assert.AreEqual(5, runtimeService.CreateProcessInstanceQuery()
                .Count());
            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());

            Assert.AreEqual(piToSuspend.Id, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .First()
                .Id);
        }

        [Test][Deployment(new string[]{"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) ]
        public virtual void testTaskSuspendedAfterProcessInstanceSuspension()
        {
            // Start Process Instance
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceByKey(processDefinition.Key);
            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();

            // Suspense process instance
            runtimeService.SuspendProcessInstanceById(processInstance.Id);

            // Assert that the task is now also suspended
            var tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                
                .ToList();
            foreach (var task in tasks)
                Assert.True(task.Suspended);

            // Activate process instance again
            runtimeService.ActivateProcessInstanceById(processInstance.Id);
            tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                
                .ToList();
            foreach (var task in tasks)
                Assert.IsFalse(task.Suspended);
        }

        [Test]
        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
        public virtual void testTaskSuspendedAfterProcessInstanceSuspensionByProcessDefinitionId()
        {
            // Start Process Instance
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceByKey(processDefinition.Key);
            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();

            // Suspense process instance
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

            // Assert that the task is now also suspended
            var tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                
                .ToList();
            foreach (var task in tasks)
                Assert.True(task.Suspended);

            // Activate process instance again
            runtimeService.ActivateProcessInstanceByProcessDefinitionId(processDefinition.Id);
            tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                
                .ToList();
            foreach (var task in tasks)
                Assert.IsFalse(task.Suspended);
        }

        [Test]
        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
        public virtual void testTaskSuspendedAfterProcessInstanceSuspensionByProcessDefinitionKey()
        {
            // Start Process Instance
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceByKey(processDefinition.Key);
            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();

            // Suspense process instance
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

            // Assert that the task is now also suspended
            var tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                
                .ToList();
            foreach (var task in tasks)
                Assert.True(task.Suspended);

            // Activate process instance again
            runtimeService.ActivateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                
                .ToList();
            foreach (var task in tasks)
                Assert.IsFalse(task.Suspended);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testTaskQueryAfterProcessInstanceSuspend()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            task = taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.NotNull(task);

            // Suspend
            runtimeService.SuspendProcessInstanceById(processInstance.Id);
            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());

            // Activate
            runtimeService.ActivateProcessInstanceById(processInstance.Id);
            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());

            // Completing should end the process instance
            task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testTaskQueryAfterProcessInstanceSuspendByProcessDefinitionId()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceById(processDefinition.Id);

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            task = taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.NotNull(task);

            // Suspend
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);
            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());

            // Activate
            runtimeService.ActivateProcessInstanceByProcessDefinitionId(processDefinition.Id);
            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());

            // Completing should end the process instance
            task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testTaskQueryAfterProcessInstanceSuspendByProcessDefinitionKey()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceById(processDefinition.Id);

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            task = taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.NotNull(task);

            // Suspend
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());

            // Activate
            runtimeService.ActivateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());

            // Completing should end the process instance
            task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment]
        public virtual void testChildExecutionsSuspendedAfterProcessInstanceSuspend()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testChildExecutionsSuspended");
            runtimeService.SuspendProcessInstanceById(processInstance.Id);

            var executions = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstance.Id)
                
                .ToList();
            foreach (var execution in executions)
                Assert.True(execution.IsSuspended);

            // Activate again
            runtimeService.ActivateProcessInstanceById(processInstance.Id);
            executions = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstance.Id)
                
                .ToList();
            foreach (var execution in executions)
                Assert.IsFalse(execution.IsSuspended);

            // Finish process
            while (taskService.CreateTaskQuery()
                       .Count() > 0)
                foreach (var task in taskService.CreateTaskQuery()
                    
                    .ToList())
                    taskService.Complete(task.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test][ Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.TestChildExecutionsSuspendedAfterProcessInstanceSuspend.bpmn20.xml"}) ]
        public virtual void testChildExecutionsSuspendedAfterProcessInstanceSuspendByProcessDefinitionId()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testChildExecutionsSuspended");
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processInstance.ProcessDefinitionId);

            var executions = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstance.Id)
                
                .ToList();
            foreach (var execution in executions)
                Assert.True(execution.IsSuspended);

            // Activate again
            runtimeService.ActivateProcessInstanceByProcessDefinitionId(processInstance.ProcessDefinitionId);
            executions = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstance.Id)
                
                .ToList();
            foreach (var execution in executions)
                Assert.IsFalse(execution.IsSuspended);

            // Finish process
            while (taskService.CreateTaskQuery()
                       .Count() > 0)
                foreach (var task in taskService.CreateTaskQuery()
                    
                    .ToList())
                    taskService.Complete(task.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test][Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.TestChildExecutionsSuspendedAfterProcessInstanceSuspend.bpmn20.xml"})]
        public virtual void testChildExecutionsSuspendedAfterProcessInstanceSuspendByProcessDefinitionKey()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testChildExecutionsSuspended");
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey("testChildExecutionsSuspended");

            var executions = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstance.Id)
                
                .ToList();
            foreach (var execution in executions)
                Assert.True(execution.IsSuspended);

            // Activate again
            runtimeService.ActivateProcessInstanceByProcessDefinitionKey("testChildExecutionsSuspended");
            executions = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstance.Id)
                
                .ToList();
            foreach (var execution in executions)
                Assert.IsFalse(execution.IsSuspended);

            // Finish process
            while (taskService.CreateTaskQuery()
                       .Count() > 0)
                foreach (var task in taskService.CreateTaskQuery()
                    
                    .ToList())
                    taskService.Complete(task.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testChangeVariablesAfterProcessInstanceSuspend()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceById(processInstance.Id);

            try
            {
                runtimeService.RemoveVariable(processInstance.Id, "someVariable");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.RemoveVariableLocal(processInstance.Id, "someVariable");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.RemoveVariables(processInstance.Id, new[] {"one", "two", "three"});
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }


            try
            {
                runtimeService.RemoveVariablesLocal(processInstance.Id, new[] {"one", "two", "three"});
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariable(processInstance.Id, "someVariable", "someValue");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariableLocal(processInstance.Id, "someVariable", "someValue");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariables(processInstance.Id, new Dictionary<string, object>());
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariablesLocal(processInstance.Id, new Dictionary<string, object>());
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testChangeVariablesAfterProcessInstanceSuspendByProcessDefinitionId()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processInstance.ProcessDefinitionId);

            try
            {
                runtimeService.RemoveVariable(processInstance.Id, "someVariable");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.RemoveVariableLocal(processInstance.Id, "someVariable");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.RemoveVariables(processInstance.Id, new[] {"one", "two", "three"});
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }


            try
            {
                runtimeService.RemoveVariablesLocal(processInstance.Id, new[] {"one", "two", "three"});
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariable(processInstance.Id, "someVariable", "someValue");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariableLocal(processInstance.Id, "someVariable", "someValue");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariables(processInstance.Id, new Dictionary<string, object>());
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariablesLocal(processInstance.Id, new Dictionary<string, object>());
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testChangeVariablesAfterProcessInstanceSuspendByProcessDefinitionKey()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

            try
            {
                runtimeService.RemoveVariable(processInstance.Id, "someVariable");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.RemoveVariableLocal(processInstance.Id, "someVariable");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.RemoveVariables(processInstance.Id, new[] {"one", "two", "three"});
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.RemoveVariablesLocal(processInstance.Id, new[] {"one", "two", "three"});
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariable(processInstance.Id, "someVariable", "someValue");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariableLocal(processInstance.Id, "someVariable", "someValue");
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariables(processInstance.Id, new Dictionary<string, object>());
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }

            try
            {
                runtimeService.SetVariablesLocal(processInstance.Id, new Dictionary<string, object>());
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("This should be possible");
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSubmitTaskFormFailAfterProcessInstanceSuspend()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceById(processInstance.Id);

            try
            {
                formService.SubmitTaskFormData(taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id)
                    .First()
                    .Id, new Dictionary<string, string>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is expected
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSubmitTaskFormFailAfterProcessInstanceSuspendByProcessDefinitionId()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

            try
            {
                formService.SubmitTaskFormData(taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id)
                    .First()
                    .Id, new Dictionary<string, string>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is expected
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSubmitTaskFormFailAfterProcessInstanceSuspendByProcessDefinitionKey()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

            try
            {
                formService.SubmitTaskFormData(taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id)
                    .First()
                    .Id, new Dictionary<string, string>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is expected
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testProcessInstanceSignalFailAfterSuspend()
        {
            // Suspend process instance
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceById(processInstance.Id);

            try
            {
                runtimeService.Signal(processInstance.Id);
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
                Assert.True(e is BadUserRequestException);
            }

            try
            {
                runtimeService.Signal(processInstance.Id, new Dictionary<string, object>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
                Assert.True(e is BadUserRequestException);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testProcessInstanceSignalFailAfterSuspendByProcessDefinitionId()
        {
            // Suspend process instance
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

            try
            {
                runtimeService.Signal(processInstance.Id);
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
                Assert.True(e is BadUserRequestException);
            }

            try
            {
                runtimeService.Signal(processInstance.Id, new Dictionary<string, object>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
                Assert.True(e is BadUserRequestException);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testProcessInstanceSignalFailAfterSuspendByProcessDefinitionKey()
        {
            // Suspend process instance
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

            try
            {
                runtimeService.Signal(processInstance.Id);
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
                Assert.True(e is BadUserRequestException);
            }

            try
            {
                runtimeService.Signal(processInstance.Id, new Dictionary<string, object>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
                Assert.True(e is BadUserRequestException);
            }
        }

        [Test]
        [Deployment]
        public virtual void testMessageEventReceiveFailAfterSuspend()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceById(processInstance.Id);
            var subscription = runtimeService.CreateEventSubscriptionQuery()
                .First();

            try
            {
                runtimeService.MessageEventReceived("someMessage", subscription.ExecutionId);
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }

            try
            {
                runtimeService.MessageEventReceived("someMessage", subscription.ExecutionId,
                    new Dictionary<string, object>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }
        }

        [Test][Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.TestMessageEventReceiveFailAfterSuspend.bpmn20.xml"})]
        public virtual void testMessageEventReceiveFailAfterSuspendByProcessDefinitionId()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);
            var subscription = runtimeService.CreateEventSubscriptionQuery()
                .First();

            try
            {
                runtimeService.MessageEventReceived("someMessage", subscription.ExecutionId);
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }

            try
            {
                runtimeService.MessageEventReceived("someMessage", subscription.ExecutionId,
                    new Dictionary<string, object>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }
        }

        [Test][Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.TestMessageEventReceiveFailAfterSuspend.bpmn20.xml"})]
        public virtual void testMessageEventReceiveFailAfterSuspendByProcessDefinitionKey()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            var subscription = runtimeService.CreateEventSubscriptionQuery()
                .First();

            try
            {
                runtimeService.MessageEventReceived("someMessage", subscription.ExecutionId);
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }

            try
            {
                runtimeService.MessageEventReceived("someMessage", subscription.ExecutionId,
                    new Dictionary<string, object>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testSignalEventReceivedAfterProcessInstanceSuspended()
        {
            const string signal = "Some Signal";

            // Test if process instance can be completed using the signal
            var processInstance = runtimeService.StartProcessInstanceByKey("signalSuspendedProcessInstance");
            runtimeService.SignalEventReceived(signal);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());

            // Now test when suspending the process instance: the process instance shouldn't be continued
            processInstance = runtimeService.StartProcessInstanceByKey("signalSuspendedProcessInstance");
            runtimeService.SuspendProcessInstanceById(processInstance.Id);
            runtimeService.SignalEventReceived(signal);
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());

            runtimeService.SignalEventReceived(signal, new Dictionary<string, object>());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());

            var subscription = runtimeService.CreateEventSubscriptionQuery()
                .First();
            try
            {
                runtimeService.SignalEventReceived(signal, subscription.ExecutionId);
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }

            try
            {
                runtimeService.SignalEventReceived(signal, subscription.ExecutionId, new Dictionary<string, object>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }

            // Activate and try again
            runtimeService.ActivateProcessInstanceById(processInstance.Id);
            runtimeService.SignalEventReceived(signal);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test][Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.TestSignalEventReceivedAfterProcessInstanceSuspended.bpmn20.xml"}) ]
        public virtual void testSignalEventReceivedAfterProcessInstanceSuspendedByProcessDefinitionId()
        {
            const string signal = "Some Signal";

            // Test if process instance can be completed using the signal
            var processInstance = runtimeService.StartProcessInstanceByKey("signalSuspendedProcessInstance");
            runtimeService.SignalEventReceived(signal);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());

            // Now test when suspending the process instance: the process instance shouldn't be continued
            processInstance = runtimeService.StartProcessInstanceByKey("signalSuspendedProcessInstance");
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processInstance.ProcessDefinitionId);
            runtimeService.SignalEventReceived(signal);
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());

            runtimeService.SignalEventReceived(signal, new Dictionary<string, object>());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());

            var subscription = runtimeService.CreateEventSubscriptionQuery()
                .First();
            try
            {
                runtimeService.SignalEventReceived(signal, subscription.ExecutionId);
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }

            try
            {
                runtimeService.SignalEventReceived(signal, subscription.ExecutionId, new Dictionary<string, object>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }

            // Activate and try again
            runtimeService.ActivateProcessInstanceById(processInstance.Id);
            runtimeService.SignalEventReceived(signal);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test][Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.TestSignalEventReceivedAfterProcessInstanceSuspended.bpmn20.xml"}) ]
        public virtual void testSignalEventReceivedAfterProcessInstanceSuspendedByProcessDefinitionKey()
        {
            const string signal = "Some Signal";

            // Test if process instance can be completed using the signal
            var processInstance = runtimeService.StartProcessInstanceByKey("signalSuspendedProcessInstance");
            runtimeService.SignalEventReceived(signal);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());

            // Now test when suspending the process instance: the process instance shouldn't be continued
            var processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="signalSuspendedProcessInstance")
                .First();

            processInstance = runtimeService.StartProcessInstanceByKey("signalSuspendedProcessInstance");
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            runtimeService.SignalEventReceived(signal);
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());

            runtimeService.SignalEventReceived(signal, new Dictionary<string, object>());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());

            var subscription = runtimeService.CreateEventSubscriptionQuery()
                .First();
            try
            {
                runtimeService.SignalEventReceived(signal, subscription.ExecutionId);
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }

            try
            {
                runtimeService.SignalEventReceived(signal, subscription.ExecutionId, new Dictionary<string, object>());
                Assert.Fail();
            }
            catch (SuspendedEntityInteractionException e)
            {
                // This is expected
                AssertTextPresent("is suspended", e.Message);
            }

            // Activate and try again
            runtimeService.ActivateProcessInstanceById(processInstance.Id);
            runtimeService.SignalEventReceived(signal);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testTaskLifecycleOperationsFailAfterProcessInstanceSuspend()
        {
            // Start a new process instance with one task
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Engine.Task.ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.GetId()).First();
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.NotNull(task);

            // Suspend the process instance
            runtimeService.SuspendProcessInstanceById(processInstance.Id);

            // Completing the task should Assert.Fail
            try
            {
                taskService.Complete(task.Id);
                Assert.Fail("It is not allowed to complete a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Claiming the task should Assert.Fail
            try
            {
                taskService.Claim(task.Id, "jos");
                Assert.Fail("It is not allowed to claim a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }


            // Adding candidate groups on the task should Assert.Fail
            try
            {
                taskService.AddCandidateGroup(task.Id, "blahGroup");
                Assert.Fail("It is not allowed to add a candidate group on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Adding candidate users on the task should Assert.Fail
            try
            {
                taskService.AddCandidateUser(task.Id, "blahUser");
                Assert.Fail("It is not allowed to add a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Adding group identity links on the task should Assert.Fail
            try
            {
                taskService.AddGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.Candidate);
                Assert.Fail("It is not allowed to add a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Adding an identity link on the task should Assert.Fail
            try
            {
                taskService.AddUserIdentityLink(task.Id, "blahUser", IdentityLinkType.Owner);
                Assert.Fail("It is not allowed to add an identityLink on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }


            // Set an assignee on the task should Assert.Fail
            try
            {
                taskService.SetAssignee(task.Id, "mispiggy");
                Assert.Fail("It is not allowed to set an assignee on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Set an owner on the task should Assert.Fail
            try
            {
                taskService.SetOwner(task.Id, "kermit");
                Assert.Fail("It is not allowed to set an owner on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing candidate groups on the task should Assert.Fail
            try
            {
                taskService.DeleteCandidateGroup(task.Id, "blahGroup");
                Assert.Fail("It is not allowed to remove a candidate group on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing candidate users on the task should Assert.Fail
            try
            {
                taskService.DeleteCandidateUser(task.Id, "blahUser");
                Assert.Fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing group identity links on the task should Assert.Fail
            try
            {
                taskService.DeleteGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.Candidate);
                Assert.Fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing an identity link on the task should Assert.Fail
            try
            {
                taskService.DeleteUserIdentityLink(task.Id, "blahUser", IdentityLinkType.Owner);
                Assert.Fail("It is not allowed to remove an identityLink on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testTaskLifecycleOperationsFailAfterProcessInstanceSuspendByProcessDefinitionId()
        {
            // Start a new process instance with one task
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Engine.Task.ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.GetId()).First();
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.NotNull(task);

            // Suspend the process instance
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

            // Completing the task should Assert.Fail
            try
            {
                taskService.Complete(task.Id);
                Assert.Fail("It is not allowed to complete a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Claiming the task should Assert.Fail
            try
            {
                taskService.Claim(task.Id, "jos");
                Assert.Fail("It is not allowed to claim a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }


            // Adding candidate groups on the task should Assert.Fail
            try
            {
                taskService.AddCandidateGroup(task.Id, "blahGroup");
                Assert.Fail("It is not allowed to add a candidate group on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Adding candidate users on the task should Assert.Fail
            try
            {
                taskService.AddCandidateUser(task.Id, "blahUser");
                Assert.Fail("It is not allowed to add a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Adding group identity links on the task should Assert.Fail
            try
            {
                taskService.AddGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.Candidate);
                Assert.Fail("It is not allowed to add a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Adding an identity link on the task should Assert.Fail
            try
            {
                taskService.AddUserIdentityLink(task.Id, "blahUser", IdentityLinkType.Owner);
                Assert.Fail("It is not allowed to add an identityLink on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }


            // Set an assignee on the task should Assert.Fail
            try
            {
                taskService.SetAssignee(task.Id, "mispiggy");
                Assert.Fail("It is not allowed to set an assignee on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Set an owner on the task should Assert.Fail
            try
            {
                taskService.SetOwner(task.Id, "kermit");
                Assert.Fail("It is not allowed to set an owner on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing candidate groups on the task should Assert.Fail
            try
            {
                taskService.DeleteCandidateGroup(task.Id, "blahGroup");
                Assert.Fail("It is not allowed to remove a candidate group on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing candidate users on the task should Assert.Fail
            try
            {
                taskService.DeleteCandidateUser(task.Id, "blahUser");
                Assert.Fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing group identity links on the task should Assert.Fail
            try
            {
                taskService.DeleteGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.Candidate);
                Assert.Fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing an identity link on the task should Assert.Fail
            try
            {
                taskService.DeleteUserIdentityLink(task.Id, "blahUser", IdentityLinkType.Owner);
                Assert.Fail("It is not allowed to remove an identityLink on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testTaskLifecycleOperationsFailAfterProcessInstanceSuspendByProcessDefinitionKey()
        {
            // Start a new process instance with one task
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Engine.Task.ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.GetId()).First();
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.NotNull(task);

            // Suspend the process instance
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

            // Completing the task should Assert.Fail
            try
            {
                taskService.Complete(task.Id);
                Assert.Fail("It is not allowed to complete a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Claiming the task should Assert.Fail
            try
            {
                taskService.Claim(task.Id, "jos");
                Assert.Fail("It is not allowed to claim a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }


            // Adding candidate groups on the task should Assert.Fail
            try
            {
                taskService.AddCandidateGroup(task.Id, "blahGroup");
                Assert.Fail("It is not allowed to add a candidate group on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Adding candidate users on the task should Assert.Fail
            try
            {
                taskService.AddCandidateUser(task.Id, "blahUser");
                Assert.Fail("It is not allowed to add a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Adding group identity links on the task should Assert.Fail
            try
            {
                taskService.AddGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.Candidate);
                Assert.Fail("It is not allowed to add a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Adding an identity link on the task should Assert.Fail
            try
            {
                taskService.AddUserIdentityLink(task.Id, "blahUser", IdentityLinkType.Owner);
                Assert.Fail("It is not allowed to add an identityLink on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }


            // Set an assignee on the task should Assert.Fail
            try
            {
                taskService.SetAssignee(task.Id, "mispiggy");
                Assert.Fail("It is not allowed to set an assignee on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Set an owner on the task should Assert.Fail
            try
            {
                taskService.SetOwner(task.Id, "kermit");
                Assert.Fail("It is not allowed to set an owner on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing candidate groups on the task should Assert.Fail
            try
            {
                taskService.DeleteCandidateGroup(task.Id, "blahGroup");
                Assert.Fail("It is not allowed to remove a candidate group on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing candidate users on the task should Assert.Fail
            try
            {
                taskService.DeleteCandidateUser(task.Id, "blahUser");
                Assert.Fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing group identity links on the task should Assert.Fail
            try
            {
                taskService.DeleteGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.Candidate);
                Assert.Fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }

            // Removing an identity link on the task should Assert.Fail
            try
            {
                taskService.DeleteUserIdentityLink(task.Id, "blahUser", IdentityLinkType.Owner);
                Assert.Fail("It is not allowed to remove an identityLink on a task of a suspended process instance");
            }
            catch (SuspendedEntityInteractionException)
            {
                // This is good
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSubTaskCreationFailAfterProcessInstanceSuspend()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Engine.Task.ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.GetId()).First();
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            runtimeService.SuspendProcessInstanceById(processInstance.Id);

            var subTask = taskService.NewTask("someTaskId");
            subTask.ParentTaskId = task.Id;

            try
            {
                taskService.SaveTask(subTask);
                Assert.Fail("Creating sub tasks for suspended task should not be possible");
            }
            catch (SuspendedEntityInteractionException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSubTaskCreationFailAfterProcessInstanceSuspendByProcessDefinitionId()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Engine.Task.ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.GetId()).First();
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

            var subTask = taskService.NewTask("someTaskId");
            subTask.ParentTaskId = task.Id;

            try
            {
                taskService.SaveTask(subTask);
                Assert.Fail("Creating sub tasks for suspended task should not be possible");
            }
            catch (SuspendedEntityInteractionException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSubTaskCreationFailAfterProcessInstanceSuspendByProcessDefinitionKey()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Engine.Task.ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.GetId()).First();
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

            var subTask = taskService.NewTask("someTaskId");
            subTask.ParentTaskId = task.Id;

            try
            {
                taskService.SaveTask(subTask);
                Assert.Fail("Creating sub tasks for suspended task should not be possible");
            }
            catch (SuspendedEntityInteractionException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testTaskNonLifecycleOperationsSucceedAfterProcessInstanceSuspend()
        {
            // Start a new process instance with one task
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Engine.Task.ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.GetId()).First();
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            runtimeService.SuspendProcessInstanceById(processInstance.Id);
            Assert.NotNull(task);

            try
            {
                taskService.SetVariable(task.Id, "someVar", "someValue");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.SetVariableLocal(task.Id, "someVar", "someValue");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                var variables = new Dictionary<string, object>();
                variables["varOne"] = "one";
                variables["varTwo"] = "two";
                taskService.SetVariables(task.Id, variables);
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                var variables = new Dictionary<string, object>();
                variables["varOne"] = "one";
                variables["varTwo"] = "two";
                taskService.SetVariablesLocal(task.Id, variables);
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariable(task.Id, "someVar");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariableLocal(task.Id, "someVar");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariables(task.Id, new[] {"one", "two"});
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariablesLocal(task.Id, new[] {"one", "two"});
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            if (processEngineConfiguration.HistoryLevel.Id > HistoryLevelFields.HistoryLevelActivity.Id)
            {
                try
                {
                    taskService.CreateComment(task.Id, processInstance.Id, "test comment");
                }
                catch (SuspendedEntityInteractionException)
                {
                    Assert.Fail("should be allowed");
                }

                try
                {
                    taskService.CreateAttachment("text", task.Id, processInstance.Id, "tesTastName", "testDescription",
                        "http://test.com");
                }
                catch (SuspendedEntityInteractionException)
                {
                    Assert.Fail("should be allowed");
                }
            }


            try
            {
                taskService.SetPriority(task.Id, 99);
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testTaskNonLifecycleOperationsSucceedAfterProcessInstanceSuspendByProcessDefinitionId()
        {
            // Start a new process instance with one task
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Engine.Task.ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.GetId()).First();
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processInstance.ProcessDefinitionId);
            Assert.NotNull(task);

            try
            {
                taskService.SetVariable(task.Id, "someVar", "someValue");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.SetVariableLocal(task.Id, "someVar", "someValue");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                var variables = new Dictionary<string, object>();
                variables["varOne"] = "one";
                variables["varTwo"] = "two";
                taskService.SetVariables(task.Id, variables);
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                var variables = new Dictionary<string, object>();
                variables["varOne"] = "one";
                variables["varTwo"] = "two";
                taskService.SetVariablesLocal(task.Id, variables);
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariable(task.Id, "someVar");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariableLocal(task.Id, "someVar");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariables(task.Id, new[] {"one", "two"});
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariablesLocal(task.Id, new[] {"one", "two"});
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            if (processEngineConfiguration.HistoryLevel.Id > HistoryLevelFields.HistoryLevelActivity.Id)
            {
                try
                {
                    taskService.CreateComment(task.Id, processInstance.Id, "test comment");
                }
                catch (SuspendedEntityInteractionException)
                {
                    Assert.Fail("should be allowed");
                }

                try
                {
                    taskService.CreateAttachment("text", task.Id, processInstance.Id, "tesTastName", "testDescription",
                        "http://test.com");
                }
                catch (SuspendedEntityInteractionException)
                {
                    Assert.Fail("should be allowed");
                }
            }


            try
            {
                taskService.SetPriority(task.Id, 99);
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testTaskNonLifecycleOperationsSucceedAfterProcessInstanceSuspendByProcessDefinitionKey()
        {
            // Start a new process instance with one task
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Engine.Task.ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.GetId()).First();
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            Assert.NotNull(task);

            try
            {
                taskService.SetVariable(task.Id, "someVar", "someValue");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.SetVariableLocal(task.Id, "someVar", "someValue");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                var variables = new Dictionary<string, object>();
                variables["varOne"] = "one";
                variables["varTwo"] = "two";
                taskService.SetVariables(task.Id, variables);
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                var variables = new Dictionary<string, object>();
                variables["varOne"] = "one";
                variables["varTwo"] = "two";
                taskService.SetVariablesLocal(task.Id, variables);
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariable(task.Id, "someVar");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariableLocal(task.Id, "someVar");
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariables(task.Id, new[] {"one", "two"});
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            try
            {
                taskService.RemoveVariablesLocal(task.Id, new[] {"one", "two"});
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }

            if (processEngineConfiguration.HistoryLevel.Id > HistoryLevelFields.HistoryLevelActivity.Id)
            {
                try
                {
                    taskService.CreateComment(task.Id, processInstance.Id, "test comment");
                }
                catch (SuspendedEntityInteractionException)
                {
                    Assert.Fail("should be allowed");
                }

                try
                {
                    taskService.CreateAttachment("text", task.Id, processInstance.Id, "tesTastName", "testDescription",
                        "http://test.com");
                }
                catch (SuspendedEntityInteractionException)
                {
                    Assert.Fail("should be allowed");
                }
            }


            try
            {
                taskService.SetPriority(task.Id, 99);
            }
            catch (SuspendedEntityInteractionException)
            {
                Assert.Fail("should be allowed");
            }
        }

        [Test]
        [Deployment]
        public virtual void testJobNotExecutedAfterProcessInstanceSuspend()
        {
            var now = DateTime.Now;
            ClockUtil.CurrentTime = now;

            // Suspending the process instance should also stop the execution of jobs for that process instance
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            Assert.AreEqual(1, managementService.CreateJobQuery()
                .Count());
            runtimeService.SuspendProcessInstanceById(processInstance.Id);
            Assert.AreEqual(1, managementService.CreateJobQuery()
                .Count());

            // The jobs should not be executed now
            ClockUtil.CurrentTime = new DateTime(now.Ticks + 60 * 60 * 1000); // Timer is set to fire on 5 minutes
            Assert.AreEqual(0, managementService.CreateJobQuery()
                /*.Executable()*/
                .Count());

            // Activation of the process instance should now allow for job execution
            runtimeService.ActivateProcessInstanceById(processInstance.Id);
            Assert.AreEqual(1, managementService.CreateJobQuery()
                /*.Executable()*/
                .Count());
            managementService.ExecuteJob(managementService.CreateJobQuery()
                .First()
                .Id);
            Assert.AreEqual(0, managementService.CreateJobQuery()
                .Count());
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test][Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.TestJobNotExecutedAfterProcessInstanceSuspend.bpmn20.xml"})]
        public virtual void testJobNotExecutedAfterProcessInstanceSuspendByProcessDefinitionId()
        {
            var now = DateTime.Now;
            ClockUtil.CurrentTime = now;

            // Suspending the process instance should also stop the execution of jobs for that process instance
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceById(processDefinition.Id);
            Assert.AreEqual(1, managementService.CreateJobQuery()
                .Count());
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processDefinition.Id);
            Assert.AreEqual(1, managementService.CreateJobQuery()
                .Count());

            // The jobs should not be executed now
            ClockUtil.CurrentTime = new DateTime(now.Ticks + 60 * 60 * 1000); // Timer is set to fire on 5 minutes
            Assert.AreEqual(0, managementService.CreateJobQuery()
                /*.Executable()*/
                .Count());

            // Activation of the process instance should now allow for job execution
            runtimeService.ActivateProcessInstanceByProcessDefinitionId(processDefinition.Id);
            Assert.AreEqual(1, managementService.CreateJobQuery()
                /*.Executable()*/
                .Count());
            managementService.ExecuteJob(managementService.CreateJobQuery()
                .First()
                .Id);
            Assert.AreEqual(0, managementService.CreateJobQuery()
                .Count());
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test][Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.TestJobNotExecutedAfterProcessInstanceSuspend.bpmn20.xml"})]
        public virtual void testJobNotExecutedAfterProcessInstanceSuspendByProcessDefinitionKey()
        {
            var now = DateTime.Now;
            ClockUtil.CurrentTime = now;

            // Suspending the process instance should also stop the execution of jobs for that process instance
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceById(processDefinition.Id);
            Assert.AreEqual(1, managementService.CreateJobQuery()
                .Count());
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            Assert.AreEqual(1, managementService.CreateJobQuery()
                .Count());

            // The jobs should not be executed now
            ClockUtil.CurrentTime = new DateTime(now.Ticks + 60 * 60 * 1000); // Timer is set to fire on 5 minutes
            Assert.AreEqual(0, managementService.CreateJobQuery()
                /*.Executable()*/
                .Count());

            // Activation of the process instance should now allow for job execution
            runtimeService.ActivateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            Assert.AreEqual(1, managementService.CreateJobQuery()
                /*.Executable()*/
                .Count());
            managementService.ExecuteJob(managementService.CreateJobQuery()
                .First()
                .Id);
            Assert.AreEqual(0, managementService.CreateJobQuery()
                .Count());
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test][Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.CallSimpleProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml"})]
        public virtual void testCallActivityReturnAfterProcessInstanceSuspend()
        {
            var instance = runtimeService.StartProcessInstanceByKey("callSimpleProcess");
            runtimeService.SuspendProcessInstanceById(instance.Id);

            var task = taskService.CreateTaskQuery()
                .First();

            try
            {
                taskService.Complete(task.Id);
                Assert.Fail("this should not be successful, as the execution of a suspended instance is resumed");
            }
            catch (SuspendedEntityInteractionException)
            {
                // this is expected to Assert.Fail
            }

            // should be successful after reactivation
            runtimeService.ActivateProcessInstanceById(instance.Id);
            taskService.Complete(task.Id);

            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment(new string[] { "resources/api/runtime/ProcessInstanceSuspensionTest.CallSimpleProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml" })]
        public virtual void testCallActivityReturnAfterProcessInstanceSuspendByProcessDefinitionId()
        {
            var instance = runtimeService.StartProcessInstanceByKey("callSimpleProcess");
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(instance.ProcessDefinitionId);

            var task = taskService.CreateTaskQuery()
                .First();

            try
            {
                taskService.Complete(task.Id);
                Assert.Fail("this should not be successful, as the execution of a suspended instance is resumed");
            }
            catch (SuspendedEntityInteractionException)
            {
                // this is expected to Assert.Fail
            }

            // should be successful after reactivation
            runtimeService.ActivateProcessInstanceByProcessDefinitionId(instance.ProcessDefinitionId);
            taskService.Complete(task.Id);

            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment(new string[] { "resources/api/runtime/ProcessInstanceSuspensionTest.CallSimpleProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml" })]
        public virtual void testCallActivityReturnAfterProcessInstanceSuspendByProcessDefinitionKey()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="callSimpleProcess")
                .First();

            runtimeService.StartProcessInstanceByKey("callSimpleProcess");
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

            var task = taskService.CreateTaskQuery()
                .First();

            try
            {
                taskService.Complete(task.Id);
                Assert.Fail("this should not be successful, as the execution of a suspended instance is resumed");
            }
            catch (SuspendedEntityInteractionException)
            {
                // this is expected to Assert.Fail
            }

            // should be successful after reactivation
            runtimeService.ActivateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            taskService.Complete(task.Id);

            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test][Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.CallMISimpleProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml"})]
        public virtual void testMICallActivityReturnAfterProcessInstanceSuspend()
        {
            var instance = runtimeService.StartProcessInstanceByKey("callMISimpleProcess");
            runtimeService.SuspendProcessInstanceById(instance.Id);

            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            var task1 = tasks[0];
            var task2 = tasks[1];

            try
            {
                taskService.Complete(task1.Id);
                Assert.Fail("this should not be successful, as the execution of a suspended instance is resumed");
            }
            catch (SuspendedEntityInteractionException)
            {
                // this is expected to Assert.Fail
            }

            try
            {
                taskService.Complete(task2.Id);
                Assert.Fail("this should not be successful, as the execution of a suspended instance is resumed");
            }
            catch (SuspendedEntityInteractionException)
            {
                // this is expected to Assert.Fail
            }

            // should be successful after reactivation
            runtimeService.ActivateProcessInstanceById(instance.Id);
            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test][Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.CallMISimpleProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml"}) ]
        public virtual void testMICallActivityReturnAfterProcessInstanceSuspendByProcessDefinitionId()
        {
            var instance = runtimeService.StartProcessInstanceByKey("callMISimpleProcess");
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(instance.ProcessDefinitionId);

            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            var task1 = tasks[0];
            var task2 = tasks[1];

            try
            {
                taskService.Complete(task1.Id);
                Assert.Fail("this should not be successful, as the execution of a suspended instance is resumed");
            }
            catch (SuspendedEntityInteractionException)
            {
                // this is expected to Assert.Fail
            }

            try
            {
                taskService.Complete(task2.Id);
                Assert.Fail("this should not be successful, as the execution of a suspended instance is resumed");
            }
            catch (SuspendedEntityInteractionException)
            {
                // this is expected to Assert.Fail
            }

            // should be successful after reactivation
            runtimeService.ActivateProcessInstanceByProcessDefinitionId(instance.ProcessDefinitionId);
            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test][Deployment(new string[] {"resources/api/runtime/ProcessInstanceSuspensionTest.CallMISimpleProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml"})]
        public virtual void testMICallActivityReturnAfterProcessInstanceSuspendByProcessDefinitionKey()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="callMISimpleProcess")
                .First();
            runtimeService.StartProcessInstanceByKey("callMISimpleProcess");
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            var task1 = tasks[0];
            var task2 = tasks[1];

            try
            {
                taskService.Complete(task1.Id);
                Assert.Fail("this should not be successful, as the execution of a suspended instance is resumed");
            }
            catch (SuspendedEntityInteractionException)
            {
                // this is expected to Assert.Fail
            }

            try
            {
                taskService.Complete(task2.Id);
                Assert.Fail("this should not be successful, as the execution of a suspended instance is resumed");
            }
            catch (SuspendedEntityInteractionException)
            {
                // this is expected to Assert.Fail
            }

            // should be successful after reactivation
            runtimeService.ActivateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }

        [Test][Deployment(new string[]{"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) ]
        public virtual void testStartBeforeActivityForSuspendProcessInstance()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            //start process instance
            runtimeService.StartProcessInstanceById(processDefinition.Id);
            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();

            // Suspend process instance
            runtimeService.SuspendProcessInstanceById(processInstance.Id);

            // try to start before activity for suspended processDefinition
            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartBeforeActivity("theTask")
                    .Execute();
                Assert.Fail("Exception is expected but not thrown");
            }
            catch (SuspendedEntityInteractionException e)
            {
                AssertTextPresentIgnoreCase("is suspended", e.Message);
            }
        }

        [Test]
        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
        public virtual void testStartAfterActivityForSuspendProcessInstance()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            //start process instance
            runtimeService.StartProcessInstanceById(processDefinition.Id);
            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();

            // Suspend process instance
            runtimeService.SuspendProcessInstanceById(processInstance.Id);

            // try to start after activity for suspended processDefinition
            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartAfterActivity("theTask")
                    .Execute();
                Assert.Fail("Exception is expected but not thrown");
            }
            catch (SuspendedEntityInteractionException e)
            {
                AssertTextPresentIgnoreCase("is suspended", e.Message);
            }
        }

        [Test][Deployment(new string[] {"resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) ]
        public virtual void testSuspensionByIdCascadesToExternalTasks()
        {
            // given
            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("twoExternalTaskProcess");

            var task1 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance1.Id)
                .First();
            Assert.IsFalse(task1.Suspended);

            // when the process instance is suspended
            runtimeService.SuspendProcessInstanceById(processInstance1.Id);

            // then the task is suspended
            task1 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance1.Id)
                .First();
            Assert.True(task1.Suspended);

            // the other task is not
            var task2 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance2.Id)
                .First();
            Assert.IsFalse(task2.Suspended);

            // when it is activated again
            runtimeService.ActivateProcessInstanceById(processInstance1.Id);

            // then the task is activated too
            task1 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance1.Id)
                .First();
            Assert.IsFalse(task1.Suspended);
        }

        [Test][Deployment(new string[] {"resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) ]
        public virtual void testSuspensionByProcessDefinitionIdCascadesToExternalTasks()
        {
            // given
            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("twoExternalTaskProcess");

            var task1 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance1.Id)
                .First();
            Assert.IsFalse(task1.Suspended);

            // when the process instance is suspended
            runtimeService.SuspendProcessInstanceByProcessDefinitionId(processInstance1.ProcessDefinitionId);

            // then the task is suspended
            task1 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance1.Id)
                .First();
            Assert.True(task1.Suspended);

            // the other task is not
            var task2 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance2.Id)
                .First();
            Assert.IsFalse(task2.Suspended);

            // when it is activated again
            runtimeService.ActivateProcessInstanceByProcessDefinitionId(processInstance1.ProcessDefinitionId);

            // then the task is activated too
            task1 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance1.Id)
                .First();
            Assert.IsFalse(task1.Suspended);
        }
        [Deployment(new string[] {"resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "resources/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) ]
        public virtual void testSuspensionByProcessDefinitionKeyCascadesToExternalTasks()
        {
            // given
            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("twoExternalTaskProcess");

            var task1 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance1.Id)
                .First();
            Assert.IsFalse(task1.Suspended);

            // when the process instance is suspended
            runtimeService.SuspendProcessInstanceByProcessDefinitionKey("oneExternalTaskProcess");

            // then the task is suspended
            task1 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance1.Id)
                .First();
            Assert.True(task1.Suspended);

            // the other task is not
            var task2 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance2.Id)
                .First();
            Assert.IsFalse(task2.Suspended);

            // when it is activated again
            runtimeService.ActivateProcessInstanceByProcessDefinitionKey("oneExternalTaskProcess");

            // then the task is activated too
            task1 = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstance1.Id)
                .First();
            Assert.IsFalse(task1.Suspended);
        }
        [Test]
        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
        public virtual void testSuspendAndActivateProcessInstanceByIdUsingBuilder()
        {
            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.IsFalse(processInstance.IsSuspended);

            //suspend
            runtimeService.UpdateProcessInstanceSuspensionState()
                .ByProcessInstanceId(processInstance.Id)
                .Suspend();

            var query = runtimeService.CreateProcessInstanceQuery();
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            //activate
            runtimeService.UpdateProcessInstanceSuspensionState()
                .ByProcessInstanceId(processInstance.Id)
                .Activate();

            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
        public virtual void testSuspendAndActivateProcessInstanceByProcessDefinitionIdUsingBuilder()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var query = runtimeService.CreateProcessInstanceQuery();
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            //suspend
            runtimeService.UpdateProcessInstanceSuspensionState()
                .ByProcessDefinitionId(processDefinition.Id)
                .Suspend();

            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            //activate
            runtimeService.UpdateProcessInstanceSuspensionState()
                .ByProcessDefinitionId(processDefinition.Id)
                .Activate();

            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
        public virtual void testSuspendAndActivateProcessInstanceByProcessDefinitionKeyUsingBuilder()
        {
            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var query = runtimeService.CreateProcessInstanceQuery();
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            //suspend
            runtimeService.UpdateProcessInstanceSuspensionState()
                .ByProcessDefinitionKey("oneTaskProcess")
                .Suspend();

            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            //activate
            runtimeService.UpdateProcessInstanceSuspensionState()
                .ByProcessDefinitionKey("oneTaskProcess")
                .Activate();

            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment]
        public virtual void testJobSuspensionStateUpdate()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("process");
            var id = instance.ProcessInstanceId;

            //when
            runtimeService.SuspendProcessInstanceById(id);
            var job = managementService.CreateJobQuery(c=>c.ProcessInstanceId== id)
                .First();

            // then
            Assert.True(job.Suspended);
        }
    }
}