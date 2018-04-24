using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Bpmn.ScriptTask
{
	/// 
	/// <summary>
	///  (Javascript)
	///  (Python)
	/// @author Nico Rehwaldt (Ruby)
	/// @author Christian Lipphardt (Groovy)
	/// 
	/// </summary>
	public class ScriptTaskTest : PluggableProcessEngineTestCase
	{

	  private const string JAVASCRIPT = "javascript";
	  private const string PYTHON = "python";
	  private const string RUBY = "ruby";
	  private const string GROOVY = "groovy";
	  private const string JUEL = "juel";

	  private IList<string> deploymentIds = new List<string>();

        [TearDown]
        protected internal virtual void tearDown()
	  {
		foreach (string deploymentId in deploymentIds)
		{
		  repositoryService.DeleteDeployment(deploymentId, true);
		}
	  }
        [Test]
	  public virtual void testJavascriptProcessVarVisibility()
	  {

		deployProcess(JAVASCRIPT, "execution.SetVariable('foo', 'a');" + "if (typeof foo !== 'undefined') { " + "  throw 'Variable foo should be defined as script variable.';" + "}" + "var foo = 'b';" + "if(execution.GetVariable('foo') != 'a') {" + "  throw 'IExecution should contain variable foo';" + "}" + "if(foo != 'b') {" + "  throw 'Script variable must override the visibiltity of the execution variable.';" + "}");
			// GIVEN
			// an execution variable 'foo'
			// THEN
			// there should be a script variable defined
			// GIVEN
			// a script variable with the same name
			// THEN
			// it should not change the value of the execution variable
			// AND
			// it should override the visibility of the execution variable

		// GIVEN
		// that we start an instance of this process
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		// THEN
		// the script task can be executed without exceptions
		// the execution variable is stored and has the correct value
		object variableValue = runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual("a", variableValue);

	  }

        [Test]
        public virtual void testPythonProcessVarAssignment()
	  {

		deployProcess(PYTHON, "execution.SetVariable('foo', 'a')\n" + "if not foo:\n" + "    raise Exception('Variable foo should be defined as script variable.')\n" + "foo = 'b'\n" + "if execution.GetVariable('foo') != 'a':\n" + "    raise Exception('IExecution should contain variable foo')\n" + "if foo != 'b':\n" + "    raise Exception('Script variable must override the visibiltity of the execution variable.')\n");
			// GIVEN
			// an execution variable 'foo'
			// THEN
			// there should be a script variable defined
			// GIVEN
			// a script variable with the same name
			// THEN
			// it should not change the value of the execution variable
			// AND
			// it should override the visibility of the execution variable

		// GIVEN
		// that we start an instance of this process
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		// THEN
		// the script task can be executed without exceptions
		// the execution variable is stored and has the correct value
		object variableValue = runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual("a", variableValue);

	  }

        [Test]
        public virtual void testRubyProcessVarVisibility()
	  {

		deployProcess(RUBY, "$execution.SetVariable('foo', 'a')\n" + "raise 'Variable foo should be defined as script variable.' if !$foo.Nil?\n" + "$foo = 'b'\n" + "if $execution.GetVariable('foo') != 'a'\n" + "  raise 'IExecution should contain variable foo'\n" + "end\n" + "if $foo != 'b'\n" + "  raise 'Script variable must override the visibiltity of the execution variable.'\n" + "end");
			// GIVEN
			// an execution variable 'foo'
			// THEN
			// there should NOT be a script variable defined (this is unsupported in Ruby binding)
			// GIVEN
			// a script variable with the same name
			// THEN
			// it should not change the value of the execution variable
			// AND
			// it should override the visibility of the execution variable

		// GIVEN
		// that we start an instance of this process
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		// THEN
		// the script task can be executed without exceptions
		// the execution variable is stored and has the correct value
		object variableValue = runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual("a", variableValue);

	  }

        [Test]
        public virtual void testGroovyProcessVarVisibility()
	  {

		deployProcess(GROOVY, "execution.SetVariable('foo', 'a')\n" + "if ( !foo ) {\n" + "  throw new Exception('Variable foo should be defined as script variable.')\n" + "}\n" + "foo = 'b'\n" + "if (execution.GetVariable('foo') != 'a') {\n" + "  throw new Exception('IExecution should contain variable foo')\n" + "}\n" + "if (foo != 'b') {\n" + "  throw new Exception('Script variable must override the visibiltity of the execution variable.')\n" + "}");
			// GIVEN
			// an execution variable 'foo'
			// THEN
			// there should be a script variable defined
			// GIVEN
			// a script variable with the same name
			// THEN
			// it should not change the value of the execution variable
			// AND
			// it should override the visibility of the execution variable

		// GIVEN
		// that we start an instance of this process
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		// THEN
		// the script task can be executed without exceptions
		// the execution variable is stored and has the correct value
		object variableValue = runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual("a", variableValue);

	  }

        [Test]
        public virtual void testJavascriptFunctionInvocation()
	  {

		deployProcess(JAVASCRIPT, "function sum(a,b){" + "  return a+b;" + "};" + "var result = sum(1,2);" + "execution.SetVariable('foo', result);");
			// GIVEN
			// a function named sum
			// THEN
			// i can call the function

		// GIVEN
		// that we start an instance of this process
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		// THEN
		// the variable is defined
		object variable = runtimeService.GetVariable(pi.Id, "foo");

		if (variable is double?)
		{
		  // jdk 6/7 - rhino returns Double 3.0 for 1+2
		  Assert.AreEqual(3.0, variable);
		}
		else if (variable is int?)
		{
		  // jdk8 - nashorn returns Integer 3 for 1+2
		  Assert.AreEqual(3, variable);
		}

	  }

        [Test]
        public virtual void testPythonFunctionInvocation()
	  {

		deployProcess(PYTHON, "def sum(a, b):\n" + "    return a + b\n" + "result = sum(1,2)\n" + "execution.SetVariable('foo', result)");
			// GIVEN
			// a function named sum
			// THEN
			// i can call the function

		// GIVEN
		// that we start an instance of this process
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		// THEN
		// the variable is defined
		object variable = runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual(3, variable);

	  }

        [Test]
        public virtual void testRubyFunctionInvocation()
	  {

		deployProcess(RUBY, "def sum(a, b)\n" + "    return a + b\n" + "end\n" + "result = sum(1,2)\n" + "$execution.SetVariable('foo', result)\n");
			// GIVEN
			// a function named sum
			// THEN
			// i can call the function

		// GIVEN
		// that we start an instance of this process
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		// THEN
		// the variable is defined
		object variable = runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual(3l, variable);

	  }

        [Test]
        public virtual void testGroovyFunctionInvocation()
	  {

		deployProcess(GROOVY, "def sum(a, b) {\n" + "    return a + b\n" + "}\n" + "result = sum(1,2)\n" + "execution.SetVariable('foo', result)\n");
			// GIVEN
			// a function named sum
			// THEN
			// i can call the function

		// GIVEN
		// that we start an instance of this process
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		// THEN
		// the variable is defined
		object variable = runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual(3, variable);

	  }

        [Test]
        public virtual void testJsVariable()
	  {

		string scriptText = "var foo = 1;";

		deployProcess(JAVASCRIPT, scriptText);

		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");
		object variableValue = runtimeService.GetVariable(pi.Id, "foo");
		Assert.IsNull(variableValue);

	  }

        [Test]
        public virtual void testPythonVariable()
	  {

		string scriptText = "foo = 1";

		deployProcess(PYTHON, scriptText);

		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");
		object variableValue = runtimeService.GetVariable(pi.Id, "foo");
		Assert.IsNull(variableValue);

	  }

        [Test]
        public virtual void testRubyVariable()
	  {

		string scriptText = "foo = 1";

		deployProcess(RUBY, scriptText);

		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");
		object variableValue = runtimeService.GetVariable(pi.Id, "foo");
		Assert.IsNull(variableValue);

	  }

        [Test]
        public virtual void testGroovyVariable()
	  {

		string scriptText = "def foo = 1";

		deployProcess(GROOVY, scriptText);

		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");
		object variableValue = runtimeService.GetVariable(pi.Id, "foo");
		Assert.IsNull(variableValue);

	  }

	  public virtual void testJuelExpression()
	  {
		deployProcess(JUEL, "${execution.SetVariable('foo', 'bar')}");

		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		string variableValue = (string) runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual("bar", variableValue);
	  }

	  public virtual void testJuelCapitalizedExpression()
	  {
		deployProcess(JUEL.ToUpper(), "${execution.SetVariable('foo', 'bar')}");

		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		string variableValue = (string) runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual("bar", variableValue);
	  }

	  public virtual void testSourceAsExpressionAsVariable()
	  {
		deployProcess(PYTHON, "${scriptSource}");

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptSource"] = "execution.SetVariable('foo', 'bar')";
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", variables);

		string variableValue = (string) runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual("bar", variableValue);
	  }

	  public virtual void testSourceAsExpressionAsNonExistingVariable()
	  {
		deployProcess(PYTHON, "${scriptSource}");

		try
		{
		  runtimeService.StartProcessInstanceByKey("testProcess");
		  Assert.Fail("Process variable 'scriptSource' not defined");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresentIgnoreCase("Cannot resolve identifier 'scriptSource'", e.Message);
		}
	  }

	  public virtual void testSourceAsExpressionAsBean()
	  {
		deployProcess(PYTHON, "#{scriptResourceBean.GetSource()}");

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptResourceBean"] = new ScriptResourceBean();
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", variables);

		string variableValue = (string) runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual("bar", variableValue);
	  }

	  public virtual void testSourceAsExpressionWithWhitespace()
	  {
		deployProcess(PYTHON, "\t\n  \t \n  ${scriptSource}");

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptSource"] = "execution.SetVariable('foo', 'bar')";
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess", variables);

		string variableValue = (string) runtimeService.GetVariable(pi.Id, "foo");
		Assert.AreEqual("bar", variableValue);
	  }

	  public virtual void testJavascriptVariableSerialization()
	  {
		deployProcess(JAVASCRIPT, "execution.SetVariable('date', new java.util.Date(0));");

		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		DateTime date = (DateTime) runtimeService.GetVariable(pi.Id, "date");
		Assert.AreEqual(0, date.Ticks);

		deployProcess(JAVASCRIPT, "execution.SetVariable('myVar', new bpmn.scripttask.MySerializable('test'));");

		pi = runtimeService.StartProcessInstanceByKey("testProcess");

		MySerializable myVar = (MySerializable) runtimeService.GetVariable(pi.Id, "myVar");
		Assert.AreEqual("test", myVar.Name);
	  }

	  public virtual void testPythonVariableSerialization()
	  {
		deployProcess(PYTHON, "import java.util.Date\nexecution.SetVariable('date', java.util.Date(0))");

		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		DateTime date = (DateTime) runtimeService.GetVariable(pi.Id, "date");
		Assert.AreEqual(0, date.Ticks);

		deployProcess(PYTHON, "import bpmn.scripttask.MySerializable\n" + "execution.SetVariable('myVar', bpmn.scripttask.MySerializable('test'));");

		pi = runtimeService.StartProcessInstanceByKey("testProcess");

		MySerializable myVar = (MySerializable) runtimeService.GetVariable(pi.Id, "myVar");
		Assert.AreEqual("test", myVar.Name);
	  }

	  public virtual void testRubyVariableSerialization()
	  {
		deployProcess(RUBY, "require 'java'\n$execution.SetVariable('date', java.util.Date.New(0))");

		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		DateTime date = (DateTime) runtimeService.GetVariable(pi.Id, "date");
		Assert.AreEqual(0, date.Ticks);

		deployProcess(RUBY, "$execution.SetVariable('myVar', bpmn.scripttask.MySerializable.New('test'));");

		pi = runtimeService.StartProcessInstanceByKey("testProcess");

		MySerializable myVar = (MySerializable) runtimeService.GetVariable(pi.Id, "myVar");
		Assert.AreEqual("test", myVar.Name);
	  }

	  public virtual void testGroovyVariableSerialization()
	  {
		deployProcess(GROOVY, "execution.SetVariable('date', new java.util.Date(0))");

		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("testProcess");

		DateTime date = (DateTime) runtimeService.GetVariable(pi.Id, "date");
		Assert.AreEqual(0, date.Ticks);

		deployProcess(GROOVY, "execution.SetVariable('myVar', new bpmn.scripttask.MySerializable('test'));");

		pi = runtimeService.StartProcessInstanceByKey("testProcess");

		MySerializable myVar = (MySerializable) runtimeService.GetVariable(pi.Id, "myVar");
		Assert.AreEqual("test", myVar.Name);
	  }

	  public virtual void testGroovyNotExistingImport()
	  {
		deployProcess(GROOVY, "import unknown");

		try
		{
		  runtimeService.StartProcessInstanceByKey("testProcess");
		  Assert.Fail("Should Assert.Fail during script compilation");
		}
		catch (ScriptCompilationException e)
		{
		  AssertTextPresentIgnoreCase("import unknown", e.Message);
		}
	  }

	  public virtual void testGroovyNotExistingImportWithoutCompilation()
	  {
		// disable script compilation
		processEngineConfiguration.EnableScriptCompilation = false;

		deployProcess(GROOVY, "import unknown");

		try
		{
		  runtimeService.StartProcessInstanceByKey("testProcess");
		  Assert.Fail("Should Assert.Fail during script evaluation");
		}
		catch (ScriptEvaluationException e)
		{
		  AssertTextPresentIgnoreCase("import unknown", e.Message);
		}
		finally
		{
		  // re-enable script compilation
		  processEngineConfiguration.EnableScriptCompilation = true;
		}
	  }

	  public virtual void testShouldNotDeployProcessWithMissingScriptElementAndResource()
	  {
		try
		{
              var task=  ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent()
                    .ScriptTask();
                task.ScriptFormat(RUBY);
                var re= task.UserTask().EndEvent().Done();
                deployProcess(re);
          Assert.Fail("this process should not be deployable");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}
	  }

	  public virtual void testShouldUseJuelAsDefaultScriptLanguage()
	  {
            var task = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().ScriptTask();
            task.ScriptText("${true}");
            var r=task.UserTask().EndEvent().Done();
            deployProcess(r);
        runtimeService.StartProcessInstanceByKey("testProcess");

		ITask task2 = taskService.CreateTaskQuery().First();
		Assert.NotNull(task2);
	  }

	  protected internal virtual void deployProcess(IBpmnModelInstance process)
	  {
		var deployment = repositoryService.CreateDeployment().AddModelInstance("testProcess.bpmn", process).Deploy();
		  deploymentIds.Add(deployment.Id);
	  }

	  protected internal virtual void deployProcess(string scriptFormat, string scriptText)
	  {
		var process = createProcess(scriptFormat, scriptText);
		deployProcess(process);
	  }

	  protected internal virtual IBpmnModelInstance createProcess(string scriptFormat, string scriptText)
	  {

            var start = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent();
            start.ScriptTask()
                .ScriptFormat(scriptFormat).ScriptText(scriptText);
            return start.UserTask().EndEvent().Done();
	  }

	  public virtual void testAutoStoreScriptVarsOff()
	  {
		Assert.IsFalse(processEngineConfiguration.IsAutoStoreScriptVariables);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testPreviousTaskShouldNotHandleException()
	  public virtual void testPreviousTaskShouldNotHandleException()
	  {
		try
		{
		  runtimeService.StartProcessInstanceByKey("process");
		  Assert.Fail();
		}
		// since the NVE extends the ProcessEngineException we have to handle it
		// separately
		catch (NullValueException)
		{
		  Assert.Fail("Shouldn't have received NullValueException");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message.Contains("Invalid format"));
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetScriptResultToProcessVariable()
	  public virtual void testSetScriptResultToProcessVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["echo"] = "hello";
		variables["existingProcessVariableName"] = "one";

		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("setScriptResultToProcessVariable", variables);

		Assert.AreEqual("hello", runtimeService.GetVariable(pi.Id, "existingProcessVariableName"));
		Assert.AreEqual(pi.Id, runtimeService.GetVariable(pi.Id, "newProcessVariableName"));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testGroovyScriptExecution()
	  public virtual void testGroovyScriptExecution()
	  {
		try
		{

		  processEngineConfiguration.IsAutoStoreScriptVariables = true;
		  int[] inputArray = new int[] {1, 2, 3, 4, 5};
		  IProcessInstance pi = runtimeService.StartProcessInstanceByKey("scriptExecution"
              , CollectionUtil.SingletonMap("inputArray", inputArray));

		  int? result = (int?) runtimeService.GetVariable(pi.Id, "sum");
		  Assert.AreEqual(15, result.Value);

		}
		finally
		{
		  processEngineConfiguration.IsAutoStoreScriptVariables = false;
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testGroovySetVariableThroughExecutionInScript()
	  public virtual void testGroovySetVariableThroughExecutionInScript()
	  {
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("setScriptVariableThroughExecution");

		// Since 'def' is used, the 'scriptVar' will be script local
		// and not automatically stored as a process variable.
		Assert.IsNull(runtimeService.GetVariable(pi.Id, "scriptVar"));
		Assert.AreEqual("test123", runtimeService.GetVariable(pi.Id, "myVar"));
	  }
	}

}