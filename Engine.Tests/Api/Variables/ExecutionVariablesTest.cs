using System.Linq;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Variables
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ExecutionVariablesTest : PluggableProcessEngineTestCase
    {

        [Test]
        public virtual void testTreeCompactionWithLocalVariableOnConcurrentExecution()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            IExecution innerTaskExecution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "innerTask").First();

            IExecution subProcessConcurrentExecution = runtimeService.CreateExecutionQuery(c => c.Id == ((ExecutionEntity)innerTaskExecution).ParentId).First();

            ITask task = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task").First();

            // when
            runtimeService.SetVariableLocal(subProcessConcurrentExecution.Id, "foo", "bar");
            // and completing the concurrent task, thereby pruning the sub process concurrent execution
            taskService.Complete(task.Id);

            // then the variable still exists
            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery().First();
            Assert.NotNull(variable);
            Assert.AreEqual("foo", variable.Name);
            Assert.AreEqual(processInstance.Id, variable.ExecutionId);
        }

        [Test]
        [Deployment("org/camunda/bpm/engine/test/api/variables/ExecutionVariablesTest.TestTreeCompactionWithLocalVariableOnConcurrentExecution.bpmn20.xml")]
        public virtual void testStableVariableInstanceIdsOnCompaction()
        {
            runtimeService.StartProcessInstanceByKey("process");

            IExecution innerTaskExecution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "innerTask").First();

            IExecution subProcessConcurrentExecution = runtimeService.CreateExecutionQuery(c => c.Id == ((ExecutionEntity)innerTaskExecution).ParentId).First();

            ITask task = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task").First();

            // when
            runtimeService.SetVariableLocal(subProcessConcurrentExecution.Id, "foo", "bar");
            IVariableInstance variableBeforeCompaction = runtimeService.CreateVariableInstanceQuery().First();

            // and completing the concurrent task, thereby pruning the sub process concurrent execution
            taskService.Complete(task.Id);

            // then the variable still exists
            IVariableInstance variableAfterCompaction = runtimeService.CreateVariableInstanceQuery().First();
            Assert.AreEqual(variableBeforeCompaction.Id, variableAfterCompaction.Id);
        }

        [Test]
        [Deployment("org/camunda/bpm/engine/test/api/variables/ExecutionVariablesTest.TestTreeCompactionForkParallelGateway.bpmn20.xml")]

        public virtual void testStableVariableInstanceIdsOnCompactionAndExpansion()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            IExecution task1Execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task1").First();

            ITask task2 = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task2").First();

            // when
            runtimeService.SetVariableLocal(task1Execution.Id, "foo", "bar");
            IVariableInstance variableBeforeCompaction = runtimeService.CreateVariableInstanceQuery().First();

            // compacting the tree
            taskService.Complete(task2.Id);

            // expanding the tree
            runtimeService.CreateProcessInstanceModification(processInstance.Id).StartBeforeActivity("task2").Execute();

            // then the variable still exists
            IVariableInstance variableAfterCompaction = runtimeService.CreateVariableInstanceQuery().First();
            Assert.AreEqual(variableBeforeCompaction.Id, variableAfterCompaction.Id);
        }
        [Test]

        public virtual void testTreeCompactionForkParallelGateway()
        {
            // given
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            ITask task1 = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task1").First();

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task2").First();

            // when
            runtimeService.SetVariableLocal(task2Execution.Id, "foo", "bar");
            // and completing the other task, thereby pruning the concurrent execution
            taskService.Complete(task1.Id);

            // then the variable still exists
            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery().First();
            Assert.NotNull(variable);
            Assert.AreEqual("foo", variable.Name);
            Assert.AreEqual(processInstance.Id, variable.ExecutionId);
        }

        [Test]
        public virtual void testTreeCompactionNestedForkParallelGateway()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            ITask task1 = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task1").First();

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task2").First();
            string subProcessScopeExecutionId = ((ExecutionEntity)task2Execution).ParentId;

            // when
            runtimeService.SetVariableLocal(task2Execution.Id, "foo", "bar");
            // and completing the other task, thereby pruning the concurrent execution
            taskService.Complete(task1.Id);

            // then the variable still exists on the subprocess scope execution
            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery().First();
            Assert.NotNull(variable);
            Assert.AreEqual("foo", variable.Name);
            Assert.AreEqual(subProcessScopeExecutionId, variable.ExecutionId);
        }
        [Test]
        [Deployment("org/camunda/bpm/engine/test/api/variables/ExecutionVariablesTest.TestTreeCompactionForkParallelGateway.bpmn20.xml")]

        public virtual void testTreeCompactionWithVariablesOnScopeAndConcurrentExecution()
        {
            // given
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            ITask task1 = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task1").First();

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task2").First();

            // when
            runtimeService.SetVariable(processInstance.Id, "foo", "baz");
            runtimeService.SetVariableLocal(task2Execution.Id, "foo", "bar");
            // and completing the other task, thereby pruning the concurrent execution
            taskService.Complete(task1.Id);

            // then something happens
            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery().First();
            Assert.NotNull(variable);
            Assert.AreEqual("foo", variable.Name);
            Assert.AreEqual(processInstance.Id, variable.ExecutionId);
        }

        [Test]
        public virtual void testForkWithThreeBranchesAndJoinOfTwoBranchesParallelGateway()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task2").First();

            // when
            runtimeService.SetVariableLocal(task2Execution.Id, "foo", "bar");
            taskService.Complete(taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task1").First().Id);
            taskService.Complete(taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task2").First().Id);

            // then
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery().Count());
        }

        [Test]
        public virtual void testForkWithThreeBranchesAndJoinOfTwoBranchesInclusiveGateway()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task2").First();

            // when
            runtimeService.SetVariableLocal(task2Execution.Id, "foo", "bar");
            taskService.Complete(taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task1").First().Id);
            taskService.Complete(taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task2").First().Id);

            // then
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery().Count());
        }

        [Test]
        [Deployment("org/camunda/bpm/engine/test/api/variables/ExecutionVariablesTest.TestTreeCompactionForkParallelGateway.bpmn20.xml")]
        public virtual void testTreeCompactionAndExpansionWithConcurrentLocalVariables()
        {

            // given
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            IExecution task1Execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task1").First();
            ITask task2 = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task2").First();

            runtimeService.SetVariableLocal(task1Execution.Id, "var", "value");

            // when compacting the tree
            taskService.Complete(task2.Id);

            // and expanding again
            runtimeService.CreateProcessInstanceModification(processInstance.Id).StartBeforeActivity("task2").Execute();

            // then the variable is again assigned to task1's concurrent execution
            ITask task1 = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task1").First();
            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery().First();

            Assert.AreEqual(task1.ExecutionId, variable.ExecutionId);
        }


        [Test]
        [Deployment("org/camunda/bpm/engine/test/api/variables/ExecutionVariablesTest.TestTreeCompactionForkParallelGateway.bpmn20.xml")]
        public virtual void testTreeCompactionAndExpansionWithScopeExecutionVariables()
        {

            // given
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            ITask task2 = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task2").First();

            runtimeService.SetVariableLocal(processInstance.Id, "var", "value");

            // when compacting the tree
            taskService.Complete(task2.Id);

            // and expanding again
            runtimeService.CreateProcessInstanceModification(processInstance.Id).StartBeforeActivity("task2").Execute();

            // then the variable is still assigned to the scope execution execution
            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery().First();

            Assert.AreEqual(processInstance.Id, variable.ExecutionId);
        }

    }

}