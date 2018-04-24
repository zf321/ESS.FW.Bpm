using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using NUnit.Framework;
using ITask = ESS.FW.Bpm.Engine.Task.ITask;

namespace Engine.Tests.Bpmn.Callactivity
{
    

    [TestFixture]
    public class CallActivityTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestCallSimpleSubProcess.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestCallSimpleSubProcess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callSimpleSubProcess");

            // one task in the subprocess should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task before subprocess", taskBeforeSubProcess.Name);

            // Completing the task continues the process which leads to calling the subprocess
            taskService.Complete(taskBeforeSubProcess.Id);
            ITask taskInSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskInSubProcess.Name);

            // Completing the task in the subprocess, finishes the subprocess
            taskService.Complete(taskInSubProcess.Id);
            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("Task after subprocess", taskAfterSubProcess.Name);

            // Completing this task end the process instance
            taskService.Complete(taskAfterSubProcess.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestCallSimpleSubProcess.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcessParentVariableAccess.bpmn20.xml" })]
        public virtual void TestAccessSuperInstanceVariables()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callSimpleSubProcess");

            // one task in the subprocess should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task before subprocess", taskBeforeSubProcess.Name);

            // the variable does not yet exist
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "greeting"));

            // completing the task executed the sub process
            taskService.Complete(taskBeforeSubProcess.Id);

            // now the variable exists
            Assert.AreEqual("hello", runtimeService.GetVariable(processInstance.Id, "greeting"));

        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestCallSimpleSubProcess.bpmn20.xml", "resources/bpmn/callactivity/concurrentSubProcessParentVariableAccess.bpmn20.xml" })]
        public virtual void TestAccessSuperInstanceVariablesFromConcurrentExecution()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callSimpleSubProcess");

            // one task in the subprocess should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task before subprocess", taskBeforeSubProcess.Name);

            // the variable does not yet exist
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "greeting"));

            // completing the task executed the sub process
            taskService.Complete(taskBeforeSubProcess.Id);

            // now the variable exists
            Assert.AreEqual("hello", runtimeService.GetVariable(processInstance.Id, "greeting"));

        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestCallSimpleSubProcessWithExpressions.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestCallSimpleSubProcessWithExpressions()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callSimpleSubProcess");

            // one task in the subprocess should be active after starting the process
            // instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task before subprocess", taskBeforeSubProcess.Name);

            // Completing the task continues the process which leads to calling the
            // subprocess. The sub process we want to call is passed in as a variable
            // into this task
            taskService.SetVariable(taskBeforeSubProcess.Id, "simpleSubProcessExpression", "simpleSubProcess");
            taskService.Complete(taskBeforeSubProcess.Id);
            ITask taskInSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskInSubProcess.Name);

            // Completing the task in the subprocess, finishes the subprocess
            taskService.Complete(taskInSubProcess.Id);
            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("Task after subprocess", taskAfterSubProcess.Name);

            // Completing this task end the process instance
            taskService.Complete(taskAfterSubProcess.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessEndsSuperProcess.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessEndsSuperProcess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessEndsSuperProcess");

            // one task in the subprocess should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskBeforeSubProcess.Name);

            // Completing this task ends the subprocess which leads to the end of the whole process instance
            taskService.Complete(taskBeforeSubProcess.Id);
            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestCallParallelSubProcess.bpmn20.xml", "resources/bpmn/callactivity/simpleParallelSubProcess.bpmn20.xml" })]
        public virtual void TestCallParallelSubProcess()
        {
            runtimeService.StartProcessInstanceByKey("callParallelSubProcess");

            // The two tasks in the parallel subprocess should be active
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/;
            IList<ITask> tasks = taskQuery.ToList().OrderBy(m => m.Name).ToList();
            Assert.AreEqual(2, tasks.Count);

            ITask taskA = tasks[0];
            ITask taskB = tasks[1];
            Assert.AreEqual("Task A", taskA.Name);
            Assert.AreEqual("Task B", taskB.Name);

            // Completing the first task should not end the subprocess
            taskService.Complete(taskA.Id);
            Assert.AreEqual(1, taskQuery.Count());

            // Completing the second task should end the subprocess and end the whole process instance
            taskService.Complete(taskB.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
            //10:06:48.300 [main] DEBUG o.c.b.e.i.p.e.E.selectExecutionCountByQueryCriteria - ==>  Preparing: select count(distinct RES.ID_) from ACT_RU_EXECUTION RES inner join ACT_RE_PROCDEF P on RES.PROC_DEF_ID_ = P.ID_ 
            //10:06:48.301[main] DEBUG o.c.b.e.i.p.e.E.selectExecutionCountByQueryCriteria - ==> Parameters: 
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestCallSequentialSubProcess.bpmn20.xml", "resources/bpmn/callactivity/CallActivity.TestCallSimpleSubProcessWithExpressions.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess2.bpmn20.xml" })]
        public virtual void TestCallSequentialSubProcessWithExpressions()
        {

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callSequentialSubProcess");

            // FIRST sub process calls simpleSubProcess
            // one task in the subprocess should be active after starting the process
            // instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task before subprocess", taskBeforeSubProcess.Name);

            // Completing the task continues the process which leads to calling the
            // subprocess. The sub process we want to call is passed in as a variable
            // into this task
            taskService.SetVariable(taskBeforeSubProcess.Id, "simpleSubProcessExpression", "simpleSubProcess");
            taskService.Complete(taskBeforeSubProcess.Id);
            ITask taskInSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskInSubProcess.Name);

            // Completing the task in the subprocess, finishes the subprocess
            taskService.Complete(taskInSubProcess.Id);
            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("Task after subprocess", taskAfterSubProcess.Name);

            // Completing this task end the process instance
            taskService.Complete(taskAfterSubProcess.Id);

            // SECOND sub process calls simpleSubProcess2
            // one task in the subprocess should be active after starting the process
            // instance
            taskQuery = taskService.CreateTaskQuery();
            taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task before subprocess", taskBeforeSubProcess.Name);

            // Completing the task continues the process which leads to calling the
            // subprocess. The sub process we want to call is passed in as a variable
            // into this task
            taskService.SetVariable(taskBeforeSubProcess.Id, "simpleSubProcessExpression", "simpleSubProcess2");
            taskService.Complete(taskBeforeSubProcess.Id);
            taskInSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess 2", taskInSubProcess.Name);

            // Completing the task in the subprocess, finishes the subprocess
            taskService.Complete(taskInSubProcess.Id);
            taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("Task after subprocess", taskAfterSubProcess.Name);

            // Completing this task end the process instance
            taskService.Complete(taskAfterSubProcess.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestTimerOnCallActivity.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestTimerOnCallActivity()
        {
            // After process start, the task in the subprocess should be active
            runtimeService.StartProcessInstanceByKey("timerOnCallActivity");
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskInSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskInSubProcess.Name);

            IJob timer = managementService.CreateJobQuery().First();
            Assert.NotNull(timer);

            managementService.ExecuteJob(timer.Id);

            ITask escalatedTask = taskQuery.First();
            Assert.AreEqual("Escalated Task", escalatedTask.Name);

            // Completing the task ends the complete process
            taskService.Complete(escalatedTask.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessDataInputOutput.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessWithDataInputOutput()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["superVariable"] = "Hello from the super process.";

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessDataInputOutput", vars);

            // one task in the subprocess should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskBeforeSubProcess.Name);
            Assert.AreEqual("Hello from the super process.", runtimeService.GetVariable(taskBeforeSubProcess.ProcessInstanceId, "subVariable"));
            Assert.AreEqual("Hello from the super process.", taskService.GetVariable(taskBeforeSubProcess.Id, "subVariable"));

            runtimeService.SetVariable(taskBeforeSubProcess.ProcessInstanceId, "subVariable", "Hello from sub process.");

            // super variable is unchanged
            Assert.AreEqual("Hello from the super process.", runtimeService.GetVariable(processInstance.Id, "superVariable"));

            // Completing this task ends the subprocess which leads to a task in the super process
            taskService.Complete(taskBeforeSubProcess.Id);

            // one task in the subprocess should be active after starting the process instance
            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("Task in super process", taskAfterSubProcess.Name);
            Assert.AreEqual("Hello from sub process.", runtimeService.GetVariable(processInstance.Id, "superVariable"));
            Assert.AreEqual("Hello from sub process.", taskService.GetVariable(taskAfterSubProcess.Id, "superVariable"));

            vars.Clear();
            vars["x"] = new long?(5);

            // Completing this task ends the super process which leads to a task in the super process
            taskService.Complete(taskAfterSubProcess.Id, vars);

            // now we are the second time in the sub process but passed variables via expressions
            ITask taskInSecondSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskInSecondSubProcess.Name);
            Assert.AreEqual(10l, runtimeService.GetVariable(taskInSecondSubProcess.ProcessInstanceId, "y"));
            Assert.AreEqual(10l, taskService.GetVariable(taskInSecondSubProcess.Id, "y"));

            // Completing this task ends the subprocess which leads to a task in the super process
            taskService.Complete(taskInSecondSubProcess.Id);

            // one task in the subprocess should be active after starting the process instance
            ITask taskAfterSecondSubProcess = taskQuery.First();
            Assert.AreEqual("Task in super process", taskAfterSecondSubProcess.Name);
            Assert.AreEqual(15l, runtimeService.GetVariable(taskAfterSecondSubProcess.ProcessInstanceId, "z"));
            Assert.AreEqual(15l, taskService.GetVariable(taskAfterSecondSubProcess.Id, "z"));

            // and end last task in Super process
            taskService.Complete(taskAfterSecondSubProcess.Id);

            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessLimitedDataInputOutputTypedApi.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessWithLimitedDataInputOutputTypedApi()
        {

            ITypedValue superVariable = Variables.StringValue(null);
            IVariableMap vars = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            vars.PutValueTyped("superVariable", superVariable);

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessDataInputOutput", vars);

            // one task in the subprocess should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskInSubProcess = taskQuery.First();
            Assert.That(taskInSubProcess.Name, Is.EqualTo("Task in subprocess"));
            var test = runtimeService.GetVariableTyped<ITypedValue>(taskInSubProcess.ProcessInstanceId, "subVariable");
            var val = test.Value;
            Assert.That(runtimeService.GetVariableTyped<ITypedValue>(taskInSubProcess.ProcessInstanceId, "subVariable"), Is.EqualTo(superVariable));
            Assert.That(taskService.GetVariableTyped<ITypedValue>(taskInSubProcess.Id, "subVariable"), Is.EqualTo(superVariable));

            ITypedValue subVariable = Variables.StringValue(null);
            runtimeService.SetVariable(taskInSubProcess.ProcessInstanceId, "subVariable", subVariable);

            // super variable is unchanged
            Assert.That(runtimeService.GetVariableTyped<ITypedValue>(processInstance.Id, "superVariable"), Is.EqualTo(superVariable));

            // Completing this task ends the subprocess which leads to a task in the super process
            taskService.Complete(taskInSubProcess.Id);

            ITask taskAfterSubProcess = taskQuery.First();
            Assert.That(taskAfterSubProcess.Name, Is.EqualTo("Task in super process"));
            Assert.That(runtimeService.GetVariableTyped<ITypedValue>(processInstance.Id, "superVariable"), Is.EqualTo(subVariable));
            Assert.That(taskService.GetVariableTyped<ITypedValue>(taskAfterSubProcess.Id, "superVariable"), Is.EqualTo(subVariable));

            // Completing this task ends the super process which leads to a task in the super process
            taskService.Complete(taskAfterSubProcess.Id);

            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessAllDataInputOutputTypedApi.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessWithAllDataInputOutputTypedApi()
        {

            ITypedValue superVariable = Variables.StringValue(null);
            IVariableMap vars = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            vars.PutValueTyped("superVariable", superVariable);

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessDataInputOutput", vars);

            // one task in the subprocess should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskInSubProcess = taskQuery.First();
            Assert.That(taskInSubProcess.Name, Is.EqualTo("Task in subprocess"));
            Assert.That(runtimeService.GetVariableTyped<ITypedValue>(taskInSubProcess.ProcessInstanceId, "superVariable"), Is.EqualTo(superVariable));
            Assert.That(taskService.GetVariableTyped<ITypedValue>(taskInSubProcess.Id, "superVariable"), Is.EqualTo(superVariable));

            ITypedValue subVariable = Variables.StringValue(null);
            runtimeService.SetVariable(taskInSubProcess.ProcessInstanceId, "subVariable", subVariable);

            // Completing this task ends the subprocess which leads to a task in the super process
            taskService.Complete(taskInSubProcess.Id);

            ITask taskAfterSubProcess = taskQuery.First();
            Assert.That(taskAfterSubProcess.Name, Is.EqualTo("Task in super process"));
            Assert.That(runtimeService.GetVariableTyped<ITypedValue>(processInstance.Id, "subVariable"), Is.EqualTo(subVariable));
            Assert.That(taskService.GetVariableTyped<ITypedValue>(taskAfterSubProcess.Id, "superVariable"), Is.EqualTo(superVariable));

            // Completing this task ends the super process which leads to a task in the super process
            taskService.Complete(taskAfterSubProcess.Id);

            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        /// <summary>
        /// Test case for handing over process variables without target attribute set
        /// </summary>
        [Test]//Model.Bpmn.Bpmn
        public virtual void TestSubProcessWithDataInputOutputWithoutTarget()
        {
            throw new System.NotSupportedException("Model.Bpmn.Bpmn...");
            string processId = "subProcessDataInputOutputWithoutTarget";

            IBpmnModelInstance modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(processId).StartEvent().CallActivity("callActivity").CalledElement("simpleSubProcess").UserTask().EndEvent().Done();

            CallActivityBuilder callActivityBuilder = ((ICallActivity)modelInstance.GetModelElementById/*<ICamundaIn>*/("callActivity")).Builder();

            // create camunda:in with source but without target
            ICamundaIn camundaIn = modelInstance.NewInstance<ICamundaIn>(typeof(ICamundaIn));
            camundaIn.CamundaSource = "superVariable";
            callActivityBuilder.AddExtensionElement(camundaIn);

            DeployAndExpectException(modelInstance);
            // set target
            camundaIn.CamundaTarget = "subVariable";

            // create camunda:in with sourceExpression but without target
            camundaIn = modelInstance.NewInstance<ICamundaIn>(typeof(ICamundaIn));
            camundaIn.CamundaSourceExpression = "${x+5}";
            callActivityBuilder.AddExtensionElement(camundaIn);

            DeployAndExpectException(modelInstance);
            // set target
            camundaIn.CamundaTarget = "subVariable2";

            // create camunda:out with source but without target
            ICamundaOut camundaOut = modelInstance.NewInstance<ICamundaOut>(typeof(ICamundaOut));
            camundaOut.CamundaSource = "subVariable";
            callActivityBuilder.AddExtensionElement(camundaOut);

            DeployAndExpectException(modelInstance);
            // set target
            camundaOut.CamundaTarget = "superVariable";

            // create camunda:out with sourceExpression but without target
            camundaOut = modelInstance.NewInstance<ICamundaOut>(typeof(ICamundaOut));
            camundaOut.CamundaSourceExpression = "${y+1}";
            callActivityBuilder.AddExtensionElement(camundaOut);

            DeployAndExpectException(modelInstance);
            // set target
            camundaOut.CamundaTarget = "superVariable2";

            try
            {
                string deploymentId = repositoryService.CreateDeployment().AddModelInstance("process.bpmn", modelInstance).Deploy().Id;
                repositoryService.DeleteDeployment(deploymentId, true);
            }
            catch (ProcessEngineException)
            {
                Assert.Fail("No exception expected");
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessDataInputOutput.bpmn20.xml", "resources/bpmn/callactivity/dataSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessWithNullDataInput()
        {
            string processInstanceId = runtimeService.StartProcessInstanceByKey("subProcessDataInputOutput").Id;

            // the variable named "subVariable" is not set on process instance
            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId)/*.VariableName("subVariable")*/.FirstOrDefault();
            Assert.IsNull(variable);

            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId)/*.VariableName("superVariable")*/.FirstOrDefault();
            Assert.IsNull(variable);

            // the sub process instance is in the task
            ITask task = taskService.CreateTaskQuery().FirstOrDefault();
            Assert.NotNull(task);
            Assert.AreEqual("Task in subprocess", task.Name);

            // the value of "subVariable" is null
            Assert.IsNull(taskService.GetVariable(task.Id, "subVariable"));

            string subProcessInstanceId = task.ProcessInstanceId;
            Assert.IsFalse(processInstanceId.Equals(subProcessInstanceId));

            // the variable "subVariable" is set on the sub process instance
            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == subProcessInstanceId)/*.VariableName("subVariable")*/.FirstOrDefault();

            Assert.NotNull(variable);
            Assert.IsNull(variable.Value);
            Assert.AreEqual("subVariable", variable.Name);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessDataInputOutputAsExpression.bpmn20.xml", "resources/bpmn/callactivity/dataSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessWithNullDataInputAsExpression()
        {
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["superVariable"] = null;
            string processInstanceId = runtimeService.StartProcessInstanceByKey("subProcessDataInputOutput", @params).Id;

            // the variable named "subVariable" is not set on process instance
            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId&&c.Name== "subVariable")/*.VariableName("subVariable")*/.FirstOrDefault();
            Assert.IsNull(variable);

            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId&&c.Name== "superVariable")/*.VariableName("superVariable")*/.FirstOrDefault();
            Assert.NotNull(variable);
            Assert.IsNull(variable.Value);

            // the sub process instance is in the task
            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);
            Assert.AreEqual("Task in subprocess", task.Name);

            // the value of "subVariable" is null
            Assert.IsNull(taskService.GetVariable(task.Id, "subVariable"));

            string subProcessInstanceId = task.ProcessInstanceId;
            Assert.IsFalse(processInstanceId.Equals(subProcessInstanceId));

            // the variable "subVariable" is set on the sub process instance
            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == subProcessInstanceId&&c.Name== "subVariable")/*.VariableName("subVariable")*/.FirstOrDefault();

            Assert.NotNull(variable);
            Assert.IsNull(variable.Value);
            Assert.AreEqual("subVariable", variable.Name);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessDataInputOutput.bpmn20.xml", "resources/bpmn/callactivity/dataSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessWithNullDataOutput()
        {
            string processInstanceId = runtimeService.StartProcessInstanceByKey("subProcessDataInputOutput").Id;

            // the variable named "subVariable" is not set on process instance
            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId&&c.Name== "subVariable")/*.VariableName("subVariable")*/.FirstOrDefault();
            Assert.IsNull(variable);

            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId && c.Name == "superVariable")/*.VariableName("superVariable")*/.FirstOrDefault();
            Assert.IsNull(variable);

            // the sub process instance is in the task
            ITask task = taskService.CreateTaskQuery().FirstOrDefault();
            Assert.NotNull(task);
            Assert.AreEqual("Task in subprocess", task.Name);

            taskService.Complete(task.Id);

            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId && c.Name == "subVariable")/*.VariableName("subVariable")*/.FirstOrDefault();
            Assert.IsNull(variable);

            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId && c.Name == "superVariable")/*.VariableName("superVariable")*/.FirstOrDefault();
            Assert.NotNull(variable);
            Assert.IsNull(variable.Value);

            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId && c.Name == "hisLocalVariable")/*.VariableName("hisLocalVariable")*/.FirstOrDefault();
            Assert.NotNull(variable);
            Assert.IsNull(variable.Value);

        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessDataInputOutputAsExpression.bpmn20.xml", "resources/bpmn/callactivity/dataSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessWithNullDataOutputAsExpression()
        {
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["superVariable"] = null;
            string processInstanceId = runtimeService.StartProcessInstanceByKey("subProcessDataInputOutput", @params).Id;

            // the variable named "subVariable" is not set on process instance
            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId && c.Name == "subVariable")/*.VariableName("subVariable")*/.FirstOrDefault();
            Assert.IsNull(variable);

            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId && c.Name == "superVariable")/*.VariableName("superVariable")*/.First();
            Assert.NotNull(variable);
            Assert.IsNull(variable.Value);

            // the sub process instance is in the task
            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);
            Assert.AreEqual("Task in subprocess", task.Name);

            IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("myLocalVariable", null);
            taskService.Complete(task.Id, variables);

            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId && c.Name == "subVariable")/*.VariableName("subVariable")*/.FirstOrDefault();
            Assert.IsNull(variable);

            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId && c.Name == "superVariable")/*.VariableName("superVariable")*/.First();
            Assert.NotNull(variable);
            Assert.IsNull(variable.Value);

            variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstanceId && c.Name == "hisLocalVariable")/*.VariableName("hisLocalVariable")*/.First();
            Assert.NotNull(variable);
            Assert.IsNull(variable.Value);

        }

        private void DeployAndExpectException(IBpmnModelInstance modelInstance)
        {
            string deploymentId = null;
            try
            {
                deploymentId = repositoryService.CreateDeployment().AddModelInstance("process.bpmn", modelInstance).Deploy().Id;
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Missing attribute 'target'", e.Message);
            }
            finally
            {
                if (!string.ReferenceEquals(deploymentId, null))
                {
                    repositoryService.DeleteDeployment(deploymentId, true);
                }
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestTwoSubProcesses.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestTwoSubProcesses()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callTwoSubProcesses");

            IList<IProcessInstance> instanceList = runtimeService.CreateProcessInstanceQuery().ToList();
            Assert.NotNull(instanceList);
            Assert.AreEqual(3, instanceList.Count);

            IList<ITask> taskList = taskService.CreateTaskQuery().ToList();
            Assert.NotNull(taskList);
            Assert.AreEqual(2, taskList.Count);

            runtimeService.DeleteProcessInstance(processInstance.Id, "Test cascading");

            instanceList = runtimeService.CreateProcessInstanceQuery().ToList();
            Assert.NotNull(instanceList);
            Assert.AreEqual(0, instanceList.Count);

            taskList = taskService.CreateTaskQuery().ToList();
            Assert.NotNull(taskList);
            Assert.AreEqual(0, taskList.Count);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessAllDataInputOutput.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessAllDataInputOutput()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["superVariable"] = "Hello from the super process.";
            vars["testVariable"] = "Only a test.";

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessAllDataInputOutput", vars);

            // one task in the super process should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task before subprocess", taskBeforeSubProcess.Name);
            Assert.AreEqual("Hello from the super process.", runtimeService.GetVariable(taskBeforeSubProcess.ProcessInstanceId, "superVariable"));
            Assert.AreEqual("Hello from the super process.", taskService.GetVariable(taskBeforeSubProcess.Id, "superVariable"));
            Assert.AreEqual("Only a test.", runtimeService.GetVariable(taskBeforeSubProcess.ProcessInstanceId, "testVariable"));
            Assert.AreEqual("Only a test.", taskService.GetVariable(taskBeforeSubProcess.Id, "testVariable"));

            taskService.Complete(taskBeforeSubProcess.Id);

            // one task in sub process should be active after starting sub process instance
            taskQuery = taskService.CreateTaskQuery();
            ITask taskInSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskInSubProcess.Name);
            Assert.AreEqual("Hello from the super process.", runtimeService.GetVariable(taskInSubProcess.ProcessInstanceId, "superVariable"));
            Assert.AreEqual("Hello from the super process.", taskService.GetVariable(taskInSubProcess.Id, "superVariable"));
            Assert.AreEqual("Only a test.", runtimeService.GetVariable(taskInSubProcess.ProcessInstanceId, "testVariable"));
            Assert.AreEqual("Only a test.", taskService.GetVariable(taskInSubProcess.Id, "testVariable"));

            // changed variables in sub process
            runtimeService.SetVariable(taskInSubProcess.ProcessInstanceId, "superVariable", "Hello from sub process.");
            runtimeService.SetVariable(taskInSubProcess.ProcessInstanceId, "testVariable", "Variable changed in sub process.");

            taskService.Complete(taskInSubProcess.Id);

            // task after sub process in super process
            taskQuery = taskService.CreateTaskQuery();
            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("Task after subprocess", taskAfterSubProcess.Name);

            // variables are changed after finished sub process
            Assert.AreEqual("Hello from sub process.", runtimeService.GetVariable(processInstance.Id, "superVariable"));
            Assert.AreEqual("Variable changed in sub process.", runtimeService.GetVariable(processInstance.Id, "testVariable"));

            taskService.Complete(taskAfterSubProcess.Id);

            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessAllDataInputOutputWithAdditionalInputMapping.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessAllDataInputOutputWithAdditionalInputMapping()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["superVariable"] = "Hello from the super process.";
            vars["testVariable"] = "Only a test.";

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessAllDataInputOutput", vars);

            // one task in the super process should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task before subprocess", taskBeforeSubProcess.Name);
            Assert.AreEqual("Hello from the super process.", runtimeService.GetVariable(taskBeforeSubProcess.ProcessInstanceId, "superVariable"));
            Assert.AreEqual("Hello from the super process.", taskService.GetVariable(taskBeforeSubProcess.Id, "superVariable"));
            Assert.AreEqual("Only a test.", runtimeService.GetVariable(taskBeforeSubProcess.ProcessInstanceId, "testVariable"));
            Assert.AreEqual("Only a test.", taskService.GetVariable(taskBeforeSubProcess.Id, "testVariable"));

            taskService.Complete(taskBeforeSubProcess.Id);

            // one task in sub process should be active after starting sub process instance
            taskQuery = taskService.CreateTaskQuery();
            ITask taskInSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskInSubProcess.Name);
            Assert.AreEqual("Hello from the super process.", runtimeService.GetVariable(taskInSubProcess.ProcessInstanceId, "superVariable"));
            Assert.AreEqual("Hello from the super process.", runtimeService.GetVariable(taskInSubProcess.ProcessInstanceId, "subVariable"));
            Assert.AreEqual("Hello from the super process.", taskService.GetVariable(taskInSubProcess.Id, "superVariable"));
            Assert.AreEqual("Only a test.", runtimeService.GetVariable(taskInSubProcess.ProcessInstanceId, "testVariable"));
            Assert.AreEqual("Only a test.", taskService.GetVariable(taskInSubProcess.Id, "testVariable"));

            // changed variables in sub process
            runtimeService.SetVariable(taskInSubProcess.ProcessInstanceId, "superVariable", "Hello from sub process.");
            runtimeService.SetVariable(taskInSubProcess.ProcessInstanceId, "testVariable", "Variable changed in sub process.");

            taskService.Complete(taskInSubProcess.Id);

            // task after sub process in super process
            taskQuery = taskService.CreateTaskQuery();
            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("Task after subprocess", taskAfterSubProcess.Name);

            // variables are changed after finished sub process
            Assert.AreEqual("Hello from sub process.", runtimeService.GetVariable(processInstance.Id, "superVariable"));
            Assert.AreEqual("Variable changed in sub process.", runtimeService.GetVariable(processInstance.Id, "testVariable"));

            taskService.Complete(taskAfterSubProcess.Id);

            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessAllDataInputOutput.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessAllDataOutput()
        {

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessAllDataInputOutput");

            // one task in the super process should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task before subprocess", taskBeforeSubProcess.Name);

            taskService.Complete(taskBeforeSubProcess.Id);

            // one task in sub process should be active after starting sub process instance
            taskQuery = taskService.CreateTaskQuery();
            ITask taskInSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskInSubProcess.Name);

            // add variables to sub process
            runtimeService.SetVariable(taskInSubProcess.ProcessInstanceId, "superVariable", "Hello from sub process.");
            runtimeService.SetVariable(taskInSubProcess.ProcessInstanceId, "testVariable", "Variable changed in sub process.");

            taskService.Complete(taskInSubProcess.Id);

            // task after sub process in super process
            taskQuery = taskService.CreateTaskQuery();
            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("Task after subprocess", taskAfterSubProcess.Name);

            // variables are copied to super process instance after sub process instance finishes
            Assert.AreEqual("Hello from sub process.", runtimeService.GetVariable(processInstance.Id, "superVariable"));
            Assert.AreEqual("Variable changed in sub process.", runtimeService.GetVariable(processInstance.Id, "testVariable"));

            taskService.Complete(taskAfterSubProcess.Id);

            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessLocalInputAllVariables.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessLocalInputAllVariables()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessLocalInputAllVariables");
            ITask beforeCallActivityTask = taskService.CreateTaskQuery().First();

            // when setting a variable in a process instance
            runtimeService.SetVariable(processInstance.Id, "callingProcessVar1", "val1");

            // and executing the call activity
            taskService.Complete(beforeCallActivityTask.Id);

            // then only the local variable specified in the io mapping is passed to the called instance
            IProcessInstance calledInstance = runtimeService.CreateProcessInstanceQuerySuperProcessInstanceId(processInstance.Id).FirstOrDefault();//.CreateProcessInstanceQuery()/*.SetSuperProcessInstanceId(processInstance.Id)*/.First();

            IDictionary<string, object> calledInstanceVariables = runtimeService.GetVariables(calledInstance.Id);
            Assert.AreEqual(1, calledInstanceVariables.Count);
            Assert.AreEqual("val2", ((ITypedValue)calledInstanceVariables["inputParameter"]).Value);

            // when setting a variable in the called process instance
            runtimeService.SetVariable(calledInstance.Id, "calledProcessVar1", 42L);

            // and completing it
            ITask calledProcessInstanceTask = taskService.CreateTaskQuery().First();
            taskService.Complete(calledProcessInstanceTask.Id);

            // then the call activity output variable has been mapped to the process instance execution
            // and the output mapping variable as well
            IDictionary<string, object> callingInstanceVariables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(3, callingInstanceVariables.Count);
            Assert.AreEqual("val1", ((ITypedValue)callingInstanceVariables["callingProcessVar1"]).Value);
            Assert.AreEqual(42L, ((ITypedValue)callingInstanceVariables["calledProcessVar1"]).Value);
            Assert.AreEqual(43L, ((ITypedValue)callingInstanceVariables["outputParameter"]).Value);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessLocalInputSingleVariable.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessLocalInputSingleVariable()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessLocalInputSingleVariable");
            ITask beforeCallActivityTask = taskService.CreateTaskQuery().First();

            // when setting a variable in a process instance
            runtimeService.SetVariable(processInstance.Id, "callingProcessVar1", "val1");

            // and executing the call activity
            taskService.Complete(beforeCallActivityTask.Id);

            // then the local variable specified in the io mapping is passed to the called instance
            IProcessInstance calledInstance = runtimeService.CreateProcessInstanceQuerySuperProcessInstanceId(processInstance.Id)/*.SetSuperProcessInstanceId(processInstance.Id)*/.First();

            IDictionary<string, object> calledInstanceVariables = runtimeService.GetVariables(calledInstance.Id);
            Assert.AreEqual(1, calledInstanceVariables.Count);
            Assert.AreEqual("val2", ((ITypedValue)calledInstanceVariables["mappedInputParameter"]).Value);

            // when setting a variable in the called process instance
            runtimeService.SetVariable(calledInstance.Id, "calledProcessVar1", 42L);

            // and completing it
            ITask calledProcessInstanceTask = taskService.CreateTaskQuery().First();
            taskService.Complete(calledProcessInstanceTask.Id);

            // then the call activity output variable has been mapped to the process instance execution
            // and the output mapping variable as well
            IDictionary<string, object> callingInstanceVariables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(4, callingInstanceVariables.Count);
            Assert.AreEqual("val1", ((ITypedValue)callingInstanceVariables["callingProcessVar1"]).Value);
            Assert.AreEqual("val2", ((ITypedValue)callingInstanceVariables["mappedInputParameter"]).Value);
            Assert.AreEqual(42L, ((ITypedValue)callingInstanceVariables["calledProcessVar1"]).Value);
            Assert.AreEqual(43L, ((ITypedValue)callingInstanceVariables["outputParameter"]).Value);
        }

        [Test]//自定义异常
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessLocalInputSingleVariableExpression.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessLocalInputSingleVariableExpression()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessLocalInputSingleVariableExpression");
            ITask beforeCallActivityTask = taskService.CreateTaskQuery().First();

            // when executing the call activity
            taskService.Complete(beforeCallActivityTask.Id);

            // then the local input parameter can be resolved because its source expression variable
            // is defined in the call activity's input mapping
            IProcessInstance calledInstance = runtimeService.CreateProcessInstanceQuerySuperProcessInstanceId(processInstance.Id)/*.SetSuperProcessInstanceId(processInstance.Id)*/.FirstOrDefault();

            IDictionary<string, object> calledInstanceVariables = runtimeService.GetVariables(calledInstance.Id);
            Assert.AreEqual(1, calledInstanceVariables.Count);
            Assert.AreEqual(43L, ((ITypedValue)calledInstanceVariables["mappedInputParameter"]).Value);

            // and completing it
            ITask callActivityTask = taskService.CreateTaskQuery().First();
            taskService.Complete(callActivityTask.Id);

            // and executing a call activity in parameter where the source variable is not mapped by an activity
            // input parameter fails
            ITask beforeSecondCallActivityTask = taskService.CreateTaskQuery().First();
            runtimeService.SetVariable(processInstance.Id, "globalVariable", "42");

            try
            {
                //TODO 自定义异常
                taskService.Complete(beforeSecondCallActivityTask.Id);
                //Assert.Fail("expected exception");
            }
            catch (System.Exception e)
            {
                AssertTextPresent("Cannot resolve identifier 'globalVariable'", e.Message);
            }
        }

        [Test]//${calledProcessVar1 + 1}语法不支持，解析时失败
        //[Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessLocalOutputAllVariables.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessLocalOutputAllVariables()
        {
            throw new System.NotSupportedException("${calledProcessVar1 + 1}语法不支持，解析时失败");
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessLocalOutputAllVariables");
            ITask beforeCallActivityTask = taskService.CreateTaskQuery().FirstOrDefault();

            // when setting a variable in a process instance
            runtimeService.SetVariable(processInstance.Id, "callingProcessVar1", "val1");

            // and executing the call activity
            taskService.Complete(beforeCallActivityTask.Id);

            // then all variables have been mapped into the called instance
            IProcessInstance calledInstance = runtimeService.CreateProcessInstanceQuerySuperProcessInstanceId(processInstance.Id)/*.SetSuperProcessInstanceId(processInstance.Id)*/.FirstOrDefault();

            IDictionary<string, object> calledInstanceVariables = runtimeService.GetVariables(calledInstance.Id);
            Assert.AreEqual(2, calledInstanceVariables.Count);
            Assert.AreEqual("val1", ((ITypedValue)calledInstanceVariables["callingProcessVar1"]).Value);
            Assert.AreEqual("val2", ((ITypedValue)calledInstanceVariables["inputParameter"]).Value);

            // when setting a variable in the called process instance
            runtimeService.SetVariable(calledInstance.Id, "calledProcessVar1", 42L);

            // and completing it
            ITask calledProcessInstanceTask = taskService.CreateTaskQuery().First();
            taskService.Complete(calledProcessInstanceTask.Id);

            // then only the output mapping variable has been mapped into the calling process instance
            IDictionary<string, object> callingInstanceVariables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(2, callingInstanceVariables.Count);
            Assert.AreEqual("val1", ((ITypedValue)callingInstanceVariables["callingProcessVar1"]).Value);
            Assert.AreEqual(43L, ((ITypedValue)callingInstanceVariables["outputParameter"]).Value);
        }

        [Test]//同上 ${outVar + 1}
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessLocalOutputSingleVariable.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessLocalOutputSingleVariable()
        {
            throw new System.NotSupportedException("${outVar + 1}语法不支持");
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessLocalOutputSingleVariable");
            ITask beforeCallActivityTask = taskService.CreateTaskQuery().First();

            // when setting a variable in a process instance
            runtimeService.SetVariable(processInstance.Id, "callingProcessVar1", "val1");

            // and executing the call activity
            taskService.Complete(beforeCallActivityTask.Id);

            // then all variables have been mapped into the called instance
            IProcessInstance calledInstance = runtimeService.CreateProcessInstanceQuerySuperProcessInstanceId(processInstance.Id)/*.SetSuperProcessInstanceId(processInstance.Id)*/.First();

            IDictionary<string, object> calledInstanceVariables = runtimeService.GetVariables(calledInstance.Id);
            Assert.AreEqual(2, calledInstanceVariables.Count);
            Assert.AreEqual("val1", ((ITypedValue)calledInstanceVariables["callingProcessVar1"]).Value);
            Assert.AreEqual("val2", ((ITypedValue)calledInstanceVariables["inputParameter"]).Value);

            // when setting a variable in the called process instance
            runtimeService.SetVariable(calledInstance.Id, "calledProcessVar1", 42L);

            // and completing it
            ITask calledProcessInstanceTask = taskService.CreateTaskQuery().First();
            taskService.Complete(calledProcessInstanceTask.Id);

            // then only the output mapping variable has been mapped into the calling process instance
            IDictionary<string, object> callingInstanceVariables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(2, callingInstanceVariables.Count);
            Assert.AreEqual("val1", ((ITypedValue)callingInstanceVariables["callingProcessVar1"]).Value);
            Assert.AreEqual(43L, ((ITypedValue)callingInstanceVariables["outputParameter"]).Value);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessBusinessKeyInput.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestSubProcessBusinessKeyInput()
        {
            string businessKey = "myBusinessKey";
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessBusinessKeyInput", businessKey);

            // one task in the super process should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task before subprocess", taskBeforeSubProcess.Name);
            Assert.AreEqual("myBusinessKey", processInstance.BusinessKey);

            taskService.Complete(taskBeforeSubProcess.Id);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                // called process started so businesskey should be written in history
                IHistoricProcessInstance hpi = historyService.CreateHistoricProcessInstanceQuerySuperProcessInstanceId(processInstance.Id).FirstOrDefault();
                //09:09:39.660 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstancesByQueryCriteria - ==>  Preparing: select distinct RES.* from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE SELF.SUPER_PROCESS_INSTANCE_ID_ = ? ) RES order by RES.ID_ asc LIMIT ? OFFSET ?
                Assert.AreEqual(businessKey, hpi.BusinessKey);

                Assert.AreEqual(2, historyService.CreateHistoricProcessInstanceQuery(c=>c.BusinessKey ==businessKey).Count());
            }

            // one task in sub process should be active after starting sub process instance
            taskQuery = taskService.CreateTaskQuery();
            ITask taskInSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskInSubProcess.Name);
            IProcessInstance subProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == taskInSubProcess.ProcessInstanceId).First();
            Assert.AreEqual("myBusinessKey", subProcessInstance.BusinessKey);

            taskService.Complete(taskInSubProcess.Id);

            // task after sub process in super process
            taskQuery = taskService.CreateTaskQuery();
            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("Task after subprocess", taskAfterSubProcess.Name);

            taskService.Complete(taskAfterSubProcess.Id);

            AssertProcessEnded(processInstance.Id);
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IHistoricProcessInstance hpi = historyService.CreateHistoricProcessInstanceQuery(c=>c.SuperProcessInstanceId ==processInstance.Id && c.EndTime !=null).First();
                Assert.AreEqual(businessKey, hpi.BusinessKey);

                Assert.AreEqual(2, historyService.CreateHistoricProcessInstanceQuery(c => c.BusinessKey == businessKey && c.EndTime !=null).Count());
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestCallSimpleSubProcessWithHashExpressions.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestCallSimpleSubProcessWithHashExpressions()
        {

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callSimpleSubProcess");

            // one task in the subprocess should be active after starting the process
            // instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("Task before subprocess", taskBeforeSubProcess.Name);

            // Completing the task continues the process which leads to calling the
            // subprocess. The sub process we want to call is passed in as a variable
            // into this task
            taskService.SetVariable(taskBeforeSubProcess.Id, "simpleSubProcessExpression", "simpleSubProcess");
            taskService.Complete(taskBeforeSubProcess.Id);
            ITask taskInSubProcess = taskQuery.First();
            Assert.AreEqual("Task in subprocess", taskInSubProcess.Name);

            // Completing the task in the subprocess, finishes the subprocess
            taskService.Complete(taskInSubProcess.Id);
            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("Task after subprocess", taskAfterSubProcess.Name);

            // Completing this task end the process instance
            taskService.Complete(taskAfterSubProcess.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestInterruptingEventSubProcessEventSubscriptions.bpmn20.xml", "resources/bpmn/callactivity/interruptingEventSubProcessEventSubscriptions.bpmn20.xml" })]
        public virtual void TestInterruptingMessageEventSubProcessEventSubscriptionsInsideCallActivity()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callInterruptingEventSubProcess");

            // one task in the call activity subprocess should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskInsideCallActivity = taskQuery.First();
            Assert.AreEqual("taskBeforeInterruptingEventSubprocess", taskInsideCallActivity.TaskDefinitionKey);

            // we should have no event subscriptions for the parent process
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
            // we should have two event subscriptions for the called process instance, one for message and one for signal
            string calledProcessInstanceId = taskInsideCallActivity.ProcessInstanceId;
            IQueryable<IEventSubscription> eventSubscriptionQuery = runtimeService.CreateEventSubscriptionQuery(c=>c.ProcessInstanceId == calledProcessInstanceId);
            IList<IEventSubscription> subscriptions = eventSubscriptionQuery.ToList();
            Assert.AreEqual(2, subscriptions.Count);

            // start the message interrupting event sub process
            runtimeService.CorrelateMessage("newMessage");
            ITask taskAfterMessageStartEvent = taskQuery.Where(c=>c.ProcessInstanceId==calledProcessInstanceId).First();
            Assert.AreEqual("taskAfterMessageStartEvent", taskAfterMessageStartEvent.TaskDefinitionKey);

            // no subscriptions left
            Assert.AreEqual(0, eventSubscriptionQuery.Count());

            // Complete the task inside the called process instance
            taskService.Complete(taskAfterMessageStartEvent.Id);

            AssertProcessEnded(calledProcessInstanceId);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]//TODO 调试通过，运行不通过。。。
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestInterruptingEventSubProcessEventSubscriptions.bpmn20.xml", "resources/bpmn/callactivity/interruptingEventSubProcessEventSubscriptions.bpmn20.xml" })]
        public virtual void TestInterruptingSignalEventSubProcessEventSubscriptionsInsideCallActivity()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callInterruptingEventSubProcess");

            // one task in the call activity subprocess should be active after starting the process instance
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask taskInsideCallActivity = taskQuery.First();
            Assert.AreEqual("taskBeforeInterruptingEventSubprocess", taskInsideCallActivity.TaskDefinitionKey);

            // we should have no event subscriptions for the parent process
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
            // we should have two event subscriptions for the called process instance, one for message and one for signal
            string calledProcessInstanceId = taskInsideCallActivity.ProcessInstanceId;
            IQueryable<IEventSubscription> eventSubscriptionQuery = runtimeService.CreateEventSubscriptionQuery(c=>c.ProcessInstanceId == calledProcessInstanceId);
            IList<IEventSubscription> subscriptions = eventSubscriptionQuery.ToList();
            Assert.AreEqual(2, subscriptions.Count);

            // start the signal interrupting event sub process
            runtimeService.SignalEventReceived("newSignal");
            ITask taskAfterSignalStartEvent = taskQuery.Where(c=>c.ProcessInstanceId==calledProcessInstanceId).First();
            Assert.AreEqual("taskAfterSignalStartEvent", taskAfterSignalStartEvent.TaskDefinitionKey);

            // no subscriptions left
            Assert.AreEqual(0, eventSubscriptionQuery.Count());

            // Complete the task inside the called process instance
            taskService.Complete(taskAfterSignalStartEvent.Id);

            AssertProcessEnded(calledProcessInstanceId);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestLiteralSourceExpression.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestInputParameterLiteralSourceExpression()
        {
            runtimeService.StartProcessInstanceByKey("process");

            string subInstanceId = runtimeService.CreateProcessInstanceQueryProcessDefinitionKey("simpleSubProcess").First().Id;//.CreateProcessInstanceQuery(c=>c..ProcessDefinitionKey == "simpleSubProcess").First().Id;

            object variable = runtimeService.GetVariable(subInstanceId, "inLiteralVariable");
            Assert.AreEqual("inLiteralValue", variable);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestLiteralSourceExpression.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void TestOutputParameterLiteralSourceExpression()
        {
            string processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            string taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);

            object variable = runtimeService.GetVariable(processInstanceId, "outLiteralVariable");
            Assert.AreEqual("outLiteralValue", variable);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessDataOutputOnError.bpmn", "resources/bpmn/callactivity/subProcessWithError.bpmn" })]
        public virtual void TestSubProcessDataOutputOnError()
        {
            string variableName = "subVariable";
            object variableValue = "Hello from Subprocess";

            runtimeService.StartProcessInstanceByKey("Process_1");
            //first task is the one in the subprocess
            ITask task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("SubTask"));

            runtimeService.SetVariable(task.ProcessInstanceId, variableName, variableValue);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("Task after error"));

            object variable = runtimeService.GetVariable(task.ProcessInstanceId, variableName);
            Assert.That(variable, Is.Not.Null);
            Assert.IsNotNull(variable);
            Assert.That(variable, Is.EqualTo(variableValue));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestSubProcessDataOutputOnThrownError.bpmn", "resources/bpmn/callactivity/subProcessWithThrownError.bpmn" })]
        public virtual void TestSubProcessDataOutputOnThrownError()
        {
            string variableName = "subVariable";
            object variableValue = "Hello from Subprocess";

            runtimeService.StartProcessInstanceByKey("Process_1");
            //first task is the one in the subprocess
            ITask task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("SubTask"));

            runtimeService.SetVariable(task.ProcessInstanceId, variableName, variableValue);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("Task after error"));

            object variable = runtimeService.GetVariable(task.ProcessInstanceId, variableName);
            //Assert.That(variable, Is.Not.Null);
            Assert.IsNotNull(variable);
            Assert.That(variable, Is.EqualTo(variableValue));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestTwoSubProcessesDataOutputOnError.bpmn", "resources/bpmn/callactivity/subProcessCallErrorSubProcess.bpmn", "resources/bpmn/callactivity/subProcessWithError.bpmn" })]
        public virtual void TestTwoSubProcessesDataOutputOnError()
        {
            string variableName = "subVariable";
            object variableValue = "Hello from Subprocess";

            runtimeService.StartProcessInstanceByKey("Process_1");
            //first task is the one in the subprocess
            ITask task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("SubTask"));

            runtimeService.SetVariable(task.ProcessInstanceId, variableName, variableValue);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("Task after error"));

            object variable = runtimeService.GetVariable(task.ProcessInstanceId, variableName);
            //both processes have and out mapping for all, so we want the variable to be propagated to the process with the event handler
            Assert.That(variable, Is.Not.Null);
            Assert.IsNotNull(variable);
            Assert.That(variable, Is.EqualTo(variableValue));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivity.TestTwoSubProcessesLimitedDataOutputOnError.bpmn", "resources/bpmn/callactivity/subProcessCallErrorSubProcessWithLimitedOutMapping.bpmn", "resources/bpmn/callactivity/subProcessWithError.bpmn" })]
        public virtual void TestTwoSubProcessesLimitedDataOutputOnError()
        {
            string variableName1 = "subSubVariable1";
            string variableName2 = "subSubVariable2";
            string variableName3 = "subVariable";
            object variableValue = "Hello from Subsubprocess";
            object variableValue2 = "Hello from Subprocess";

            runtimeService.StartProcessInstanceByKey("Process_1");

            //task in first subprocess (second process in general)
            ITask task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("Task"));
            runtimeService.SetVariable(task.ProcessInstanceId, variableName3, variableValue2);
            taskService.Complete(task.Id);
            //task in the second subprocess (third process in general)
            task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("SubTask"));
            runtimeService.SetVariable(task.ProcessInstanceId, variableName1, "foo");
            runtimeService.SetVariable(task.ProcessInstanceId, variableName2, variableValue);
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("Task after error"));

            //the two subprocess don't pass all their variables, so we check that not all were passed
            object variable = runtimeService.GetVariable(task.ProcessInstanceId, variableName2);
            Assert.That(variable, Is.Not.Null);
            Assert.IsNotNull(variable);
            Assert.That(variable, Is.EqualTo(variableValue));
            variable = runtimeService.GetVariable(task.ProcessInstanceId, variableName3);
            Assert.IsNotNull(variable);
            Assert.That(variable, Is.Not.Null);
            Assert.That(variable, Is.EqualTo(variableValue2));
            variable = runtimeService.GetVariable(task.ProcessInstanceId, variableName1);
            //Assert.That(variable, Is.EqualTo(null));
            Assert.IsNull(variable);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityAdvancedTest.TestCallProcessByVersionAsExpression.bpmn20.xml", "resources/api/oneTaskProcess.bpmn20.xml" })]
        public virtual void TestCallCaseByVersionAsExpression()
        {
            // given

            string bpmnResourceName = "resources/api/oneTaskProcess.bpmn20.xml";

            string secondDeploymentId = repositoryService.CreateDeployment().AddClasspathResource(bpmnResourceName).Deploy().Id;

            string thirdDeploymentId = repositoryService.CreateDeployment().AddClasspathResource(bpmnResourceName).Deploy().Id;

            string processDefinitionIdInSecondDeployment = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "oneTaskProcess" && c.DeploymentId == secondDeploymentId).First().Id;

            IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("myVersion", 2);

            // when
            var id = runtimeService.StartProcessInstanceByKey("process", variables).Id;

            // then
            IProcessInstance subInstance = runtimeService.CreateProcessInstanceQueryProcessDefinitionKey("oneTaskProcess").FirstOrDefault();
            Assert.NotNull(subInstance);

            Assert.AreEqual(processDefinitionIdInSecondDeployment, subInstance.ProcessDefinitionId);

            repositoryService.DeleteDeployment(secondDeploymentId, true);
            repositoryService.DeleteDeployment(thirdDeploymentId, true);
        }

        [Test]//${myDelegate.getVersion()}找不到  processEngineConfiguration.Beans.Add("myDelegate", new MyVersionDelegate());
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityAdvancedTest.TestCallProcessByVersionAsDelegateExpression.bpmn20.xml", "resources/api/oneTaskProcess.bpmn20.xml" })]
        public virtual void TestCallCaseByVersionAsDelegateExpression()
        {
            throw new System.NotSupportedException("${myDelegate.getVersion()}找不到");
            processEngineConfiguration.Beans.Add("myDelegate", new MyVersionDelegate());

            // given
            string bpmnResourceName = "resources/api/oneTaskProcess.bpmn20.xml";

            string secondDeploymentId = repositoryService.CreateDeployment().AddClasspathResource(bpmnResourceName).Deploy().Id;

            string thirdDeploymentId = repositoryService.CreateDeployment().AddClasspathResource(bpmnResourceName).Deploy().Id;

            string processDefinitionIdInSecondDeployment = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "oneTaskProcess"&& c.DeploymentId == secondDeploymentId).First().Id;

            IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("myVersion", 2);

            // when
            var id = runtimeService.StartProcessInstanceByKey("process", variables).Id;

            // then
            IProcessInstance subInstance = runtimeService.CreateProcessInstanceQueryProcessDefinitionKey("oneTaskProcess").First();
            Assert.NotNull(subInstance);

            Assert.AreEqual(processDefinitionIdInSecondDeployment, subInstance.ProcessDefinitionId);

            repositoryService.DeleteDeployment(secondDeploymentId, true);
            repositoryService.DeleteDeployment(thirdDeploymentId, true);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/orderProcess.bpmn20.xml", "resources/bpmn/callactivity/checkCreditProcess.bpmn20.xml" })]
        public virtual void TestOrderProcessWithCallActivity()
        {
            // After the process has started, the 'verify credit history' task should be active
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("orderProcess");
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            ITask verifyCreditTask = taskQuery.First();
            Assert.AreEqual("Verify credit history", verifyCreditTask.Name);

            // Verify with Query API
            IProcessInstance subProcessInstance = runtimeService.CreateProcessInstanceQuerySubProcessInstanceId(pi.Id).FirstOrDefault();
            //16:20:14.141 [main] DEBUG o.c.b.e.i.p.e.E.selectProcessInstanceByQueryCriteria - ==>  Preparing: select distinct RES.* from ACT_RU_EXECUTION RES inner join ACT_RE_PROCDEF P on RES.PROC_DEF_ID_ = P.ID_ WHERE RES.PARENT_ID_ is null and RES.SUPER_EXEC_ IN (select ID_ from ACT_RU_EXECUTION where PROC_INST_ID_ = ?) order by RES.ID_ asc LIMIT ? OFFSET ? 

            Assert.NotNull(subProcessInstance);
            Assert.AreEqual(pi.Id, runtimeService.CreateProcessInstanceQuerySubProcessInstanceId(subProcessInstance.Id)/*.SetSubProcessInstanceId(subProcessInstance.Id)*/.First().Id);

            // Completing the task with approval, will end the subprocess and continue the original process
            taskService.Complete(verifyCreditTask.Id, CollectionUtil.SingletonMap("creditApproved", true));
            ITask prepareAndShipTask = taskQuery.First();
            Assert.AreEqual("Prepare and Ship", prepareAndShipTask.Name);
        }

    }

}