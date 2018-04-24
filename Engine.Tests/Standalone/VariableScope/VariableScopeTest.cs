using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Standalone.VariableScope
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class VariableScopeTest : PluggableProcessEngineTestCase
    {
        /// <summary>
        ///     A testcase to produce and fix issue ACT-862.
        /// </summary>
        [Test]
        [Deployment]
        public virtual void TestVariableNamesScope()
        {
            // After starting the process, the task in the subprocess should be active
            IDictionary<string, ITypedValue> varMap = new Dictionary<string, ITypedValue>();
            varMap["test"] = new StringValueImpl("test");
            varMap["helloWorld"] = new StringValueImpl("helloWorld");
            var pi = runtimeService.StartProcessInstanceByKey("simpleSubProcess", varMap);
            var subProcessTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)
                .First();//.Where(c=>c.ProcessInstanceId==pi.Id).First();
            runtimeService.SetVariableLocal(pi.ProcessInstanceId, "mainProcessLocalVariable", "Hello World");

            Assert.AreEqual("Task in subprocess", subProcessTask.Name);

            runtimeService.SetVariableLocal(subProcessTask.ExecutionId, "subProcessLocalVariable", "Hello SubProcess");

            // Returns a set of local variablenames of pi
            var result =
                processEngineConfiguration.CommandExecutorTxRequired.Execute(new GetVariableNamesCommand(this,
                    pi.ProcessInstanceId, true));

            // pi contains local the variablenames "test", "helloWorld" and "mainProcessLocalVariable" but not "subProcessLocalVariable"
            Assert.True(result.Contains("test"));
            Assert.True(result.Contains("helloWorld"));
            Assert.True(result.Contains("mainProcessLocalVariable"));
            Assert.IsFalse(result.Contains("subProcessLocalVariable"));

            // Returns a set of global variablenames of pi
            result =
                processEngineConfiguration.CommandExecutorTxRequired.Execute(new GetVariableNamesCommand(this,
                    pi.ProcessInstanceId, false));

            // pi contains global the variablenames "test", "helloWorld" and "mainProcessLocalVariable" but not "subProcessLocalVariable"
            Assert.True(result.Contains("test"));
            Assert.True(result.Contains("mainProcessLocalVariable"));
            Assert.True(result.Contains("helloWorld"));
            Assert.IsFalse(result.Contains("subProcessLocalVariable"));

            // Returns a set of local variablenames of subProcessTask execution
            result =
                processEngineConfiguration.CommandExecutorTxRequired.Execute(new GetVariableNamesCommand(this,
                    subProcessTask.ExecutionId, true));

            // subProcessTask execution contains local the variablenames "test", "subProcessLocalVariable" but not "helloWorld" and "mainProcessLocalVariable"
            Assert.True(result.Contains("test")); // the variable "test" was set locally by SetLocalVariableTask
            Assert.True(result.Contains("subProcessLocalVariable"));
            Assert.IsFalse(result.Contains("helloWorld"));
            Assert.IsFalse(result.Contains("mainProcessLocalVariable"));

            // Returns a set of global variablenames of subProcessTask execution
            result =
                processEngineConfiguration.CommandExecutorTxRequired.Execute(new GetVariableNamesCommand(this,
                    subProcessTask.ExecutionId, false));

            // subProcessTask execution contains global all defined variablenames
            Assert.True(result.Contains("test")); // the variable "test" was set locally by SetLocalVariableTask
            Assert.True(result.Contains("subProcessLocalVariable"));
            Assert.True(result.Contains("helloWorld"));
            Assert.True(result.Contains("mainProcessLocalVariable"));

            taskService.Complete(subProcessTask.Id);
        }
        //TODO 产生了多余的resource
        [Test]
        [Deployment()]
        public virtual void TestVariableByteDelete()
        {
            IDictionary<string, ITypedValue> varMap = new Dictionary<string, ITypedValue>();
            varMap["variableObject"] = new ObjectValueImpl(new VaraibleObject() { Id="test"});
            var pi = runtimeService.StartProcessInstanceByKey("simpleSubProcess", varMap);

            var subProcessTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)
                .First();//.Where(c=>c.ProcessInstanceId==pi.Id).First();

            Assert.AreEqual("Task in subprocess", subProcessTask.Name);
            taskService.Complete(subProcessTask.Id);
            AssertProcessEnded(pi.Id);
        }

        /// <summary>
        ///     A command to get the names of the variables
        /// </summary>
        private class GetVariableNamesCommand : ICommand<IList<string>>
        {
            private readonly VariableScopeTest _outerInstance;


            internal readonly string ExecutionId;
            internal readonly bool IsLocal;


            public GetVariableNamesCommand(VariableScopeTest outerInstance, string executionId, bool isLocal)
            {
                this._outerInstance = outerInstance;
                this.ExecutionId = executionId;
                this.IsLocal = isLocal;
            }

            public virtual IList<string> Execute(CommandContext commandContext)
            {
                EnsureUtil.EnsureNotNull("executionId", ExecutionId);

                var execution = commandContext.ExecutionManager.FindExecutionById(ExecutionId);

                EnsureUtil.EnsureNotNull("execution " + ExecutionId + " doesn't exist", "execution", execution);

                IList<string> executionVariables;
                if (IsLocal)
                    executionVariables = new List<string>(execution.VariableNamesLocal);
                else
                    executionVariables = new List<string>(execution.VariableNames);

                return executionVariables;
            }
        }


    }

    [Serializable]
    public class VaraibleObject
    {
        public string Id { get; set; }
    }
}