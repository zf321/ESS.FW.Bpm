using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.IoMapping
{



    /// <summary>
    /// Testcase for camunda input / output in BPMN
    /// </summary>
    [TestFixture]
    public class InputOutputTest : PluggableProcessEngineTestCase
    {

        // Input parameters /////////////////////////////////////////

        [Test]
        [Deployment]
        public virtual void TestInputNullValue()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            var execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(null, variable.TypeName);
            Assert.AreEqual(execution.First().Id, variable.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void TestInputStringConstantValue()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual("stringValue", variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }


        [Test]
        [Deployment]
        public virtual void TestInputElValue()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            var execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2L, variable.Value);
            Assert.AreEqual(execution.First().Id, variable.ExecutionId);
        }

        [Test]//Groovy
        [Deployment]
        public virtual void TestInputScriptValue()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void TestInputScriptValueAsVariable()
        {
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["scriptSource"] = new StringValueImpl("int Todo(){ return 1 + 1;}");
            runtimeService.StartProcessInstanceByKey("testProcess", variables);
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }
        [Test]
        [Deployment]
        public virtual void TestInputScriptValueAsVariablePython()
        {
            string pythonTxt= 
@"def say_hello():
    print 'hello!'

def get_text():
    return 'text from hello.py'

def foo(arg1, arg2):
    return arg1 + arg2

def add():
    return 1+1"
               ;
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["scriptSource"] = new StringValueImpl(pythonTxt);
            runtimeService.StartProcessInstanceByKey("testProcess", variables);
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }

        //[Test]//Groovy
        [Deployment]
        public virtual void TestInputScriptValueAsBean()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["onePlusOneBean"] = new OnePlusOneBean();
            runtimeService.StartProcessInstanceByKey("testProcess", variables);
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }

        //[Test]//Groovy
        [Deployment]
        public virtual void TestInputExternalScriptValue()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }

        //[Test]//Groovy
        [Deployment]
        public virtual void TestInputExternalScriptValueAsVariable()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptPath"] = "resources/bpmn/iomapping/oneplusone.groovy";
            runtimeService.StartProcessInstanceByKey("testProcess", variables);
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }

        //[Test]//Groovy
        [Deployment]
        public virtual void TestInputExternalScriptValueAsBean()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["onePlusOneBean"] = new OnePlusOneBean();
            runtimeService.StartProcessInstanceByKey("testProcess", variables);
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }

        //[Test]//Groovy
        [Deployment]
        public virtual void TestInputExternalClasspathScriptValue()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }

        //[Test]//Groovy
        [Deployment]
        public virtual void TestInputExternalClasspathScriptValueAsVariable()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptPath"] = "classpath://resources/bpmn/iomapping/oneplusone.groovy";
            runtimeService.StartProcessInstanceByKey("testProcess", variables);
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }
        //[Test]//Groovy
        [Deployment]
        public virtual void TestInputExternalClasspathScriptValueAsBean()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["onePlusOneBean"] = new OnePlusOneBean();
            runtimeService.StartProcessInstanceByKey("testProcess", variables);
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }

        //[Test]//groovy
        [Deployment(Resources = new string[] { "resources/bpmn/iomapping/InputOutputTest.TestInputExternalDeploymentScriptValue.bpmn", "resources/bpmn/iomapping/oneplusone.groovy" })]
        public virtual void TestInputExternalDeploymentScriptValue()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }

        //[Test]//groovy
        [Deployment(Resources = new string[] { "resources/bpmn/iomapping/InputOutputTest.TestInputExternalDeploymentScriptValueAsVariable.bpmn", "resources/bpmn/iomapping/oneplusone.groovy" })]
        public virtual void TestInputExternalDeploymentScriptValueAsVariable()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptPath"] = "deployment://resources/bpmn/iomapping/oneplusone.groovy";
            runtimeService.StartProcessInstanceByKey("testProcess", variables);
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }

        //[Test]groovy
        [Deployment(Resources = new string[] { "resources/bpmn/iomapping/InputOutputTest.TestInputExternalDeploymentScriptValueAsBean.bpmn", "resources/bpmn/iomapping/oneplusone.groovy" })]
        public virtual void TestInputExternalDeploymentScriptValueAsBean()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["onePlusOneBean"] = new OnePlusOneBean();
            runtimeService.StartProcessInstanceByKey("testProcess", variables);
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(execution.Id, variable.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void TestInputListElValues()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            IList<object> value = (IList<object>)variable.Value;
            Assert.AreEqual(2l, value[0]);
            Assert.AreEqual(3l, value[1]);
            Assert.AreEqual(4l, value[2]);
        }

        //[Test]
        [Deployment]//Groovy
        public virtual void TestInputListMixedValues()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            IList<object> value = (IList<object>)variable.Value;
            Assert.AreEqual("constantStringValue", value[0]);
            Assert.AreEqual("elValue", value[1]);
            Assert.AreEqual("scriptValue", value[2]);
        }

        [Test]
        [Deployment]
        public virtual void TestInputMapElValues()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            SortedDictionary<string, object> value = (SortedDictionary<string, object>)variable.Value;
            Assert.AreEqual(2l, value["a"]);
            Assert.AreEqual(3l, value["b"]);
            Assert.AreEqual(4l, value["c"]);

        }
        [Test]
        [Deployment]
        public virtual void TestInputMultipleElValue()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance var1 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(var1);
            Assert.AreEqual(2l, var1.Value);
            Assert.AreEqual(execution.Id, var1.ExecutionId);

            IVariableInstance var2 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var2")/*.VariableName("var2")*/.First();
            Assert.NotNull(var2);
            Assert.AreEqual(3l, var2.Value);
            Assert.AreEqual(execution.Id, var2.ExecutionId);
        }
        [Test]
        [Deployment]
        public virtual void TestInputMultipleMixedValue()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance var1 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(var1);
            Assert.AreEqual(2l, var1.Value);
            Assert.AreEqual(execution.Id, var1.ExecutionId);

            IVariableInstance var2 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var2")/*.VariableName("var2")*/.First();
            Assert.NotNull(var2);
            Assert.AreEqual("stringConstantValue", var2.Value);
            Assert.AreEqual(execution.Id, var2.ExecutionId);
        }
        [Test]
        [Deployment]
        public virtual void TestInputNested()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "wait").First();

            IVariableInstance var1 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            SortedDictionary<string, object> value = (SortedDictionary<string, object>)var1.Value;
            IList<object> nestedList = (IList<object>)value["a"];
            Assert.AreEqual("stringInListNestedInMap", nestedList[0]);
            Assert.AreEqual("b", nestedList[1]);

            IVariableInstance var2 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var2")/*.VariableName("var2")*/.First();
            Assert.NotNull(var2);
            Assert.AreEqual("stringConstantValue", var2.Value);
            Assert.AreEqual(execution.Id, var2.ExecutionId);
        }
        //[Test]//Groovy
        //[Deployment]
        protected virtual void TestInputNestedListValues()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            IList<object> value = (IList<object>)variable.Value;
            Assert.AreEqual("constantStringValue", value[0]);
            Assert.AreEqual("elValue", value[1]);
            Assert.AreEqual("scriptValue", value[2]);

            IList<object> nestedList = (IList<object>)value[3];
            IList<object> nestedNestedList = (IList<object>)nestedList[0];
            Assert.AreEqual("a", nestedNestedList[0]);
            Assert.AreEqual("b", nestedNestedList[1]);
            Assert.AreEqual("c", nestedNestedList[2]);
            Assert.AreEqual("d", nestedList[1]);

            SortedDictionary<string, object> nestedMap = (SortedDictionary<string, object>)value[4];
            Assert.AreEqual("bar", nestedMap["foo"]);
            Assert.AreEqual("world", nestedMap["hello"]);
        }

        // output parameter ///////////////////////////////////////////////////////
        [Test]
        [Deployment]
        public virtual void TestOutputNullValue()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.FirstOrDefault();
            Assert.NotNull(variable);
            Assert.AreEqual(null, variable.TypeName);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }
        [Test]
        [Deployment]
        public virtual void TestOutputStringConstantValue()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual("stringValue", variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void TestOutputElValue()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2l, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }
        //[Test]//Groovy
        [Deployment]
        public virtual void TestOutputScriptValue()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }
        //[Test]//Groovy
        [Deployment]
        public virtual void TestOutputScriptValueAsVariable()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptSource"] = "return 1 + 1";
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", variables);

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }
        //[Test]//Groovy
        [Deployment]
        public virtual void TestOutputScriptValueAsBean()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["onePlusOneBean"] = new OnePlusOneBean();
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", variables);

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }
        //[Test]//Groovy
        [Deployment]
        public virtual void TestOutputExternalScriptValue()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.FirstOrDefault();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }
        //[Test]//Groovy
        [Deployment]
        public virtual void TestOutputExternalScriptValueAsVariable()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptPath"] = "resources/bpmn/iomapping/oneplusone.groovy";
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", variables);

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }
        //[Test]//Groovy
        [Deployment]
        public virtual void TestOutputExternalScriptValueAsBean()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["onePlusOneBean"] = new OnePlusOneBean();
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", variables);

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }
        //[Test]//Groovy
        [Deployment]
        public virtual void TestOutputExternalClasspathScriptValue()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }
        //[Test]//Groovy
        [Deployment]
        public virtual void TestOutputExternalClasspathScriptValueAsVariable()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptPath"] = "classpath://resources/bpmn/iomapping/oneplusone.groovy";
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", variables);

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }
        //[Test]//Groovy
        [Deployment]
        public virtual void TestOutputExternalClasspathScriptValueAsBean()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["onePlusOneBean"] = new OnePlusOneBean();
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", variables);

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }

        //[Test]//groovy
        [Deployment(Resources = new string[] { "resources/bpmn/iomapping/InputOutputTest.TestOutputExternalDeploymentScriptValue.bpmn", "resources/bpmn/iomapping/oneplusone.groovy" })]
        public virtual void TestOutputExternalDeploymentScriptValue()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }

        //[Test]//groovy
        [Deployment(Resources = new string[] { "resources/bpmn/iomapping/InputOutputTest.TestOutputExternalDeploymentScriptValueAsVariable.bpmn", "resources/bpmn/iomapping/oneplusone.groovy" })]
        public virtual void TestOutputExternalDeploymentScriptValueAsVariable()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptPath"] = "deployment://resources/bpmn/iomapping/oneplusone.groovy";
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", variables);

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }

        //[Test]//groovy
        [Deployment(Resources = new string[] { "resources/bpmn/iomapping/InputOutputTest.TestOutputExternalDeploymentScriptValueAsBean.bpmn", "resources/bpmn/iomapping/oneplusone.groovy" })]
        public virtual void TestOutputExternalDeploymentScriptValueAsBean()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["onePlusOneBean"] = new OnePlusOneBean();
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", variables);

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            Assert.AreEqual(2, variable.Value);
            Assert.AreEqual(pi.Id, variable.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void TestOutputListElValues()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.FirstOrDefault();
            Assert.NotNull(variable);
            IList<object> value = (IList<object>)variable.Value;
            Assert.AreEqual(2l, value[0]);
            Assert.AreEqual(3l, value[1]);
            Assert.AreEqual(4l, value[2]);
        }
        //TODO Groovy
        //[Test]//Groovy
        [Deployment]
        public virtual void TestOutputListMixedValues()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            IList<object> value = (IList<object>)variable.Value;
            Assert.AreEqual("constantStringValue", value[0]);
            Assert.AreEqual("elValue", value[1]);
            Assert.AreEqual("scriptValue", value[2]);
        }

        [Test]
        [Deployment]
        public virtual void TestOutputMapElValues()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            SortedDictionary<string, object> value = (SortedDictionary<string, object>)variable.Value;
            Assert.AreEqual(2l, value["a"]);
            Assert.AreEqual(3l, value["b"]);
            Assert.AreEqual(4l, value["c"]);

        }
        [Test]
        [Deployment]
        public virtual void TestOutputMultipleElValue()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance var1 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(var1);
            Assert.AreEqual(2l, var1.Value);
            Assert.AreEqual(pi.Id, var1.ExecutionId);

            IVariableInstance var2 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var2")/*.VariableName("var2")*/.First();
            Assert.NotNull(var2);
            Assert.AreEqual(3l, var2.Value);
            Assert.AreEqual(pi.Id, var2.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void TestOutputMultipleMixedValue()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance var1 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(var1);
            Assert.AreEqual(2l, var1.Value);
            Assert.AreEqual(pi.Id, var1.ExecutionId);

            IVariableInstance var2 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var2")/*.VariableName("var2")*/.First();
            Assert.NotNull(var2);
            Assert.AreEqual("stringConstantValue", var2.Value);
            Assert.AreEqual(pi.Id, var2.ExecutionId);
        }
        [Test]
        [Deployment]
        public virtual void TestOutputNested()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance var1 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.FirstOrDefault();
            SortedDictionary<string, object> value = (SortedDictionary<string, object>)var1.Value;
            IList<object> nestedList = (IList<object>)value["a"];
            Assert.AreEqual("stringInListNestedInMap", nestedList[0]);
            Assert.AreEqual("b", nestedList[1]);
            Assert.AreEqual(pi.Id, var1.ExecutionId);

            IVariableInstance var2 = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var2")/*.VariableName("var2")*/.FirstOrDefault();
            Assert.NotNull(var2);
            Assert.AreEqual("stringConstantValue", var2.Value);
            Assert.AreEqual(pi.Id, var2.ExecutionId);
        }
        //[Test]//Groovy
        [Deployment]
        public virtual void testOutputListNestedValues()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            IVariableInstance variable = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var1")/*.VariableName("var1")*/.First();
            Assert.NotNull(variable);
            IList<object> value = (IList<object>)variable.Value;
            Assert.AreEqual("constantStringValue", value[0]);
            Assert.AreEqual("elValue", value[1]);
            Assert.AreEqual("scriptValue", value[2]);

            IList<object> nestedList = (IList<object>)value[3];
            IList<object> nestedNestedList = (IList<object>)nestedList[0];
            Assert.AreEqual("a", nestedNestedList[0]);
            Assert.AreEqual("b", nestedNestedList[1]);
            Assert.AreEqual("c", nestedNestedList[2]);
            Assert.AreEqual("d", nestedList[1]);

            SortedDictionary<string, object> nestedMap = (SortedDictionary<string, object>)value[4];
            Assert.AreEqual("bar", nestedMap["foo"]);
            Assert.AreEqual("world", nestedMap["hello"]);
        }

        // ensure Io supported on event subprocess /////////////////////////////////
        [Test]
        public virtual void TestInterruptingEventSubprocessIoSupport()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/iomapping/InputOutputTest.TestInterruptingEventSubprocessIoSupport.bpmn").Deploy();
                Assert.Fail("exception expected");
            }
            catch (/*ProcessEngine*/System.Exception e)
            {
                // happy path
                AssertTextPresent("camunda:inputOutput mapping unsupported for element type 'subProcess' with attribute 'triggeredByEvent = true'", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void TestSubprocessIoSupport()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["processVar"] = "value";
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess", variables);

            IExecution subprocessExecution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "subprocessTask").First();
            IDictionary<string, object> variablesLocal = runtimeService.GetVariablesLocal(subprocessExecution.Id);
            Assert.AreEqual(1, variablesLocal.Count);
            Assert.AreEqual("value", variablesLocal["innerVar"]);

            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            string outerVariable = (string)runtimeService.GetVariableLocal(processInstance.Id, "outerVar");
            Assert.NotNull(outerVariable);
            Assert.AreEqual("value", outerVariable);


        }
        [Test]
        [Deployment]
        public virtual void TestSequentialMIActivityIoSupport()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["counter"] = new IntegerValueImpl(0);// new AtomicInteger();
            variables["nrOfLoops"] = 2;
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("miSequentialActivity", variables);

            // first sequential mi execution
            IExecution miExecution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "miTask").FirstOrDefault();
            Assert.NotNull(miExecution);
            Assert.IsFalse(instance.Id.Equals(miExecution.Id));
            Assert.AreEqual(0, runtimeService.GetVariable(miExecution.Id, "loopCounter"));

            // input mapping
            Assert.AreEqual(1, runtimeService.CreateVariableInstanceQuery(m => m.Name == "miCounterValue")/*.VariableName("miCounterValue")*/.Count());
            Assert.AreEqual(1, runtimeService.GetVariableLocal(miExecution.Id, "miCounterValue"));

            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            // second sequential mi execution
            miExecution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "miTask").First();
            Assert.NotNull(miExecution);
            Assert.IsFalse(instance.Id.Equals(miExecution.Id));
            Assert.AreEqual(1, runtimeService.GetVariable(miExecution.Id, "loopCounter"));

            // input mapping
            Assert.AreEqual(1, runtimeService.CreateVariableInstanceQuery(m => m.Name == "miCounterValue")/*.VariableName("miCounterValue")*/.Count());
            Assert.AreEqual(2, runtimeService.GetVariableLocal(miExecution.Id, "miCounterValue"));

            task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            // variable does not exist outside of scope
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(m => m.Name == "miCounterValue")/*.VariableName("miCounterValue")*/.Count());
        }
        [Test]
        [Deployment]
        public virtual void TestSequentialMISubprocessIoSupport()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["counter"] = new IntegerValueImpl(0);// AtomicInteger();
            variables["nrOfLoops"] = 2;
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess", variables);

            // first sequential mi execution
            IExecution miScopeExecution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task").First();
            Assert.NotNull(miScopeExecution);
            Assert.AreEqual(0, runtimeService.GetVariable(miScopeExecution.Id, "loopCounter"));

            // input mapping
            Assert.AreEqual(1, runtimeService.CreateVariableInstanceQuery(m => m.Name == "miCounterValue")/*.VariableName("miCounterValue")*/.Count());
            Assert.AreEqual(1, runtimeService.GetVariableLocal(miScopeExecution.Id, "miCounterValue"));

            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            //second sequential mi execution
            miScopeExecution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task").First();
            Assert.NotNull(miScopeExecution);
            Assert.IsFalse(instance.Id.Equals(miScopeExecution.Id));
            Assert.AreEqual(1, runtimeService.GetVariable(miScopeExecution.Id, "loopCounter"));

            // input mapping
            Assert.AreEqual(1, runtimeService.CreateVariableInstanceQuery(m => m.Name == "miCounterValue")/*.VariableName("miCounterValue")*/.Count());
            Assert.AreEqual(2, runtimeService.GetVariableLocal(miScopeExecution.Id, "miCounterValue"));

            task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            // variable does not exist outside of scope
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(m => m.Name == "miCounterValue")/*.VariableName("miCounterValue")*/.Count());
        }
        //[Test]//IntegerValueImpl.IncrementAndGet() 方法 反射找不到
        [Deployment]
        public virtual void TestParallelMIActivityIoSupport()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["counter"] = new IntegerValueImpl(0);//AtomicInteger();
            variables["nrOfLoops"] = 2;
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("miParallelActivity", variables);

            ISet<int?> counters = new HashSet<int?>();
            // first mi execution
            IExecution miExecution1 = runtimeService.CreateExecutionQuery(c => c.ActivityId == "miTask")/*//.VariableValueEquals("loopCounter", 0)*/.First();
            Assert.NotNull(miExecution1);
            Assert.IsFalse(instance.Id.Equals(miExecution1.Id));
            counters.Add((int?)runtimeService.GetVariableLocal(miExecution1.Id, "miCounterValue"));

            // second mi execution
            IExecution miExecution2 = runtimeService.CreateExecutionQuery(c => c.ActivityId == "miTask")/*//.VariableValueEquals("loopCounter", 1)*/.First();
            Assert.NotNull(miExecution2);
            Assert.IsFalse(instance.Id.Equals(miExecution2.Id));
            counters.Add((int?)runtimeService.GetVariableLocal(miExecution2.Id, "miCounterValue"));

            Assert.True(counters.Contains(1));
            Assert.True(counters.Contains(2));

            Assert.AreEqual(2, runtimeService.CreateVariableInstanceQuery(m => m.Name == "miCounterValue")/*.VariableName("miCounterValue")*/.Count());

            foreach (ITask task in taskService.CreateTaskQuery().ToList())
            {
                taskService.Complete(task.Id);
            }

            // variable does not exist outside of scope
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(m => m.Name == "miCounterValue")/*.VariableName("miCounterValue")*/.Count());
        }
        //[Test]//incrementAndGet
        [Deployment]
        public virtual void TestParallelMISubprocessIoSupport()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["counter"] = new IntegerValueImpl(0);// AtomicInteger();
            variables["nrOfLoops"] = 2;
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("miParallelSubprocess", variables);

            ISet<int?> counters = new HashSet<int?>();

            // first parallel mi execution
            IExecution miScopeExecution1 = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task")/*//.VariableValueEquals("loopCounter", 0)*/.First();
            Assert.NotNull(miScopeExecution1);
            counters.Add((int?)runtimeService.GetVariableLocal(miScopeExecution1.Id, "miCounterValue"));

            // second parallel mi execution
            IExecution miScopeExecution2 = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task")/*//.VariableValueEquals("loopCounter", 1)*/.First();
            Assert.NotNull(miScopeExecution2);
            Assert.IsFalse(instance.Id.Equals(miScopeExecution2.Id));
            counters.Add((int?)runtimeService.GetVariableLocal(miScopeExecution2.Id, "miCounterValue"));

            Assert.True(counters.Contains(1));
            Assert.True(counters.Contains(2));

            Assert.AreEqual(2, runtimeService.CreateVariableInstanceQuery(m => m.Name == "miCounterValue")/*.VariableName("miCounterValue")*/.Count());

            foreach (ITask task in taskService.CreateTaskQuery().ToList())
            {
                taskService.Complete(task.Id);
            }

            // variable does not exist outside of scope
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(m => m.Name == "miCounterValue")/*.VariableName("miCounterValue")*/.Count());
        }
        [Test]
        public virtual void TestMIOutputMappingDisallowed()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/iomapping/InputOutputTest.TestMIOutputMappingDisallowed.bpmn20.xml").Deploy();
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("camunda:outputParameter not allowed for multi-instance constructs", e.Message);
            }

        }

        //[Test]
        //[Deployment(Resources =new string[] { "resources/bpmn/iomapping/InputOutputTest.TestThrowErrorInScriptInputOutputMapping.bpmn" })]
        public virtual void FAILING_testBpmnErrorInScriptInputMapping()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["throwInMapping"] = "in";
            variables["exception"] = new BpmnError("error");
            runtimeService.StartProcessInstanceByKey("testProcess", variables);
            //we will only reach the IUser task if the BPMNError from the script was handled by the boundary event
            ITask task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("User Task"));
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/iomapping/InputOutputTest.TestThrowErrorInScriptInputOutputMapping.bpmn" })]
        public virtual void TestExceptionInScriptInputMapping()
        {
            string exceptionMessage = "myException";
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["throwInMapping"] = "in";
            variables["exception"] = new System.Exception(exceptionMessage);
            try
            {
                runtimeService.StartProcessInstanceByKey("testProcess", variables);
            }
            catch (System.Exception re)
            {
                Assert.That(re.Message, Contains.Substring(exceptionMessage));
            }
        }

        //[Test]
        //[Deployment(Resources = new string[] { "resources/bpmn/iomapping/InputOutputTest.TestThrowErrorInScriptInputOutputMapping.bpmn" })]
        public virtual void FAILING_testBpmnErrorInScriptOutputMapping()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["throwInMapping"] = "out";
            variables["exception"] = new BpmnError("error");
            runtimeService.StartProcessInstanceByKey("testProcess", variables);
            //we will only reach the IUser task if the BPMNError from the script was handled by the boundary event
            ITask task = taskService.CreateTaskQuery().First();
            Assert.That(task.Name, Is.EqualTo("IUser Task"));
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/iomapping/InputOutputTest.TestThrowErrorInScriptInputOutputMapping.bpmn" })]
        public virtual void TestExceptionInScriptOutputMapping()
        {
            string exceptionMessage = "myException";
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["throwInMapping"] = "out";
            variables["exception"] = new System.Exception(exceptionMessage);
            try
            {
                runtimeService.StartProcessInstanceByKey("testProcess", variables);
            }
            catch (System.Exception re)
            {
                Assert.That(re.Message, Contains.Substring(exceptionMessage));
            }
        }

        //[Test]
        [Deployment]
        public virtual void FAILING_testOutputMappingOnErrorBoundaryEvent()
        {

            // case 1: no error occurs
            runtimeService.StartProcessInstanceByKey("testProcess");

            ITask task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual("taskOk", task.TaskDefinitionKey);

            // then: variable mapped exists
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(m => m.Name == "localNotMapped")/*.VariableName("localNotMapped")*/.Count());
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(m => m.Name == "localMapped")/*.VariableName("localMapped")*/.Count());
            Assert.AreEqual(1, runtimeService.CreateVariableInstanceQuery(m => m.Name == "mapped")/*.VariableName("mapped")*/.Count());

            taskService.Complete(task.Id);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

            // case 2: error occurs
            runtimeService.StartProcessInstanceByKey("testProcess", Collections.SingletonMap<string, object>("throwError", true));

            task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual("taskError", task.TaskDefinitionKey);

            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(m => m.Name == "localNotMapped")/*.VariableName("localNotMapped")*/.Count());
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(m => m.Name == "localMapped")/*.VariableName("localMapped")*/.Count());
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(m => m.Name == "mapped")/*.VariableName("mapped")*/.Count());

            taskService.Complete(task.Id);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
        }

        //[Test]
        //[Deployment]
        public virtual void FAILING_testOutputMappingOnMessageBoundaryEvent()
        {

            // case 1: no error occurs
            runtimeService.StartProcessInstanceByKey("testProcess");

            ITask task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual("wait", task.TaskDefinitionKey);

            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual("taskOk", task.TaskDefinitionKey);

            // then: variable mapped exists
            Assert.AreEqual(1, runtimeService.CreateVariableInstanceQuery(m => m.Name == "mapped")/*.VariableName("mapped")*/.Count());

            taskService.Complete(task.Id);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

            // case 2: error occurs
            runtimeService.StartProcessInstanceByKey("testProcess", Collections.SingletonMap<string, object>("throwError", true));

            task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual("wait", task.TaskDefinitionKey);

            runtimeService.CorrelateMessage("message");

            task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual("taskError", task.TaskDefinitionKey);

            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(m => m.Name == "mapped")/*.VariableName("mapped")*/.Count());

            taskService.Complete(task.Id);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
        }

        //[Test]
        //[Deployment]
        public virtual void FAILING_testOutputMappingOnTimerBoundaryEvent()
        {

            // case 1: no error occurs
            runtimeService.StartProcessInstanceByKey("testProcess");

            ITask task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual("wait", task.TaskDefinitionKey);

            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual("taskOk", task.TaskDefinitionKey);

            // then: variable mapped exists
            Assert.AreEqual(1, runtimeService.CreateVariableInstanceQuery(m => m.Name == "mapped")/*.VariableName("mapped")*/.Count());

            taskService.Complete(task.Id);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

            // case 2: error occurs
            runtimeService.StartProcessInstanceByKey("testProcess", Collections.SingletonMap<string, object>("throwError", true));

            task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual("wait", task.TaskDefinitionKey);

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            managementService.ExecuteJob(job.Id);

            task = taskService.CreateTaskQuery().First();

            Assert.NotNull(task);
            Assert.AreEqual("taskError", task.TaskDefinitionKey);

            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery(m => m.Name == "mapped")/*.VariableName("mapped")*/.Count());

            taskService.Complete(task.Id);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void TestScopeActivityInstanceId()
        {
            // given
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            IActivityInstance tree = runtimeService.GetActivityInstance(ProcessInstanceId);
            IActivityInstance theTaskInstance = tree.GetActivityInstances("theTask")[0];

            // when
            IVariableInstance variableInstance = runtimeService.CreateVariableInstanceQuery().First();
            //13:58:30.380 [main] DEBUG o.c.b.e.i.p.e.V.selectVariableInstanceByQueryCriteria - ==>  Preparing: select distinct RES.* from ( select RES.*, ( case when RES.TASK_ID_ is not null and RES.EXECUTION_ID_ is not null then EXECUTION.ACT_INST_ID_ when RES.CASE_EXECUTION_ID_ is not null then RES.CASE_EXECUTION_ID_ when EXECUTION.PARENT_ID_ is null and RES.IS_CONCURRENT_LOCAL_ = 0 then EXECUTION.ID_ when EXECUTION.IS_SCOPE_ = 1 and EXECUTION.PARENT_ID_ is not null and RES.IS_CONCURRENT_LOCAL_ = 0 then PARENT_EXECUTION.ACT_INST_ID_ else EXECUTION.ACT_INST_ID_ end ) ACT_INST_ID_ from ACT_RU_VARIABLE RES left join ACT_RU_EXECUTION EXECUTION on RES.EXECUTION_ID_ = EXECUTION.ID_ left join ACT_RU_EXECUTION PARENT_EXECUTION on EXECUTION.PARENT_ID_ = PARENT_EXECUTION.ID_ ) RES order by RES.ID_ asc LIMIT ? OFFSET ? 
            //var test = runtimeService.GetDbContext().Database.SqlQuery(typeof(string), "select  RES.ACT_INST_ID from ( select RES.*, ( case when RES.TASK_ID is not null and RES.EXECUTION_ID is not null then EXECUTION.ACT_INST_ID when RES.CASE_EXECUTION_ID is not null then RES.CASE_EXECUTION_ID when EXECUTION.PARENT_ID is null and RES.IS_CONCURRENT_LOCAL = 0 then EXECUTION.ID when EXECUTION.IS_SCOPE = 1 and EXECUTION.PARENT_ID is not null and RES.IS_CONCURRENT_LOCAL = 0 then PARENT_EXECUTION.ACT_INST_ID else EXECUTION.ACT_INST_ID end ) ACT_INST_ID from TB_GOS_BPM_RU_VARIABLE RES left join TB_GOS_BPM_RU_EXECUTION EXECUTION on RES.EXECUTION_ID = EXECUTION.ID left join TB_GOS_BPM_RU_EXECUTION PARENT_EXECUTION on EXECUTION.PARENT_ID = PARENT_EXECUTION.ID ) RES order by RES.ID ");
            //var r = test.ToListAsync().Result.Cast<string>().First();

            // then
            //Assert.AreEqual(theTaskInstance.Id, variableInstance.ActivityInstanceId);
            //Assert.AreEqual(theTaskInstance.Id, r);
        }
        [Test]//Model.Bpmn.Bpmn
        public virtual void TestCompositeExpressionForInputValue()
        {

            // given
            IBpmnModelInstance instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process").StartEvent().ReceiveTask().CamundaInputParameter("var", "Hello World${'!'}").EndEvent("end").Done();

            Deployment(instance);
            runtimeService.StartProcessInstanceByKey("Process");

            // when
            IVariableInstance variableInstance = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var")/*.VariableName("var")*/.First();

            // then
            Assert.AreEqual("Hello World!", variableInstance.Value);
        }
        [Test]//Model.Bpmn.Bpmn
        public virtual void TestCompositeExpressionForOutputValue()
        {

            // given
            IBpmnModelInstance instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process").StartEvent().ServiceTask().CamundaExpression("${true}").CamundaInputParameter("var1", "World!").CamundaOutputParameter("var2", "Hello ${var1}").UserTask().EndEvent("end").Done();

            Deployment(instance);
            runtimeService.StartProcessInstanceByKey("Process");

            // when
            IVariableInstance variableInstance = runtimeService.CreateVariableInstanceQuery(m => m.Name == "var2")/*.VariableName("var2")*/.First();

            // then
            Assert.AreEqual("Hello World!", variableInstance.Value);
        }

    }
}