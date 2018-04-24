using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;

namespace Engine.Tests.Api.Variables.Scope
{
    
	/// <summary>
	/// 
	/// </summary>
	public class TargetVariableScopeTest
	{
		private bool InstanceFieldsInitialized = false;

		public TargetVariableScopeTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}
       // public readonly IExpectedException thrown = IExpectedException();

        private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public ProcessEngineRule engineRule = new ProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProcessEngineRule();
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public util.ProcessEngineTestRule testHelper = new util.ProcessEngineTestRule(engineRule);
	  public ProcessEngineTestRule testHelper;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
	  //public ExpectedException thrown = ExpectedException.None();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/variables/scope/TargetVariableScopeTest.TestExecutionWithDelegateProcess.bpmn","org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"}) public void testExecutionWithDelegateProcess()
	  public virtual void testExecutionWithDelegateProcess()
	  {
		// Given we create a new process instance
		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("orderIds", (new int[]{1, 2, 3}));
		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("Process_MultiInstanceCallAcitivity",variables);

		// it runs without any problems
		Assert.That(processInstance.IsEnded,Is.EqualTo(true));
		Assert.That(((ProcessInstanceWithVariablesImpl) processInstance).Variables.ContainsKey("targetOrderId"),Is.EqualTo(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/variables/scope/TargetVariableScopeTest.TestExecutionWithScriptTargetScope.bpmn","org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"}) public void testExecutionWithScriptTargetScope()
	  public virtual void testExecutionWithScriptTargetScope()
	  {
		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("orderIds", (new int[]{1, 2, 3}));
		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("Process_MultiInstanceCallAcitivity",variables);

		// it runs without any problems
		Assert.That(processInstance.IsEnded,Is.EqualTo(true));
		Assert.That(((ProcessInstanceWithVariablesImpl) processInstance).Variables.ContainsKey("targetOrderId"),Is.EqualTo(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/variables/scope/TargetVariableScopeTest.TestExecutionWithoutProperTargetScope.bpmn","org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"}) public void testExecutionWithoutProperTargetScope()
	  public virtual void testExecutionWithoutProperTargetScope()
	  {
		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("orderIds", (new int[]{1, 2, 3}));
            //fails due to inappropriate variable scope target
       // thrown.Expect(typeof(ScriptEvaluationException));
		//thrown.ExpectMessage.StartsWith("Unable to evaluate script: org.camunda.bpm.Engine.ProcessEngineException: ENGINE-20011 Scope with specified activity Id NOT_EXISTING and execution"));
		engineRule.RuntimeService.StartProcessInstanceByKey("Process_MultiInstanceCallAcitivity",variables);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"}) public void testWithDelegateVariableMapping()
	  public virtual void testWithDelegateVariableMapping()
	  {
		//IBpmnModelInstance instance = Model.Bpmn.Bpmn.CreateExecutableProcess("process1").StartEvent().SubProcess("SubProcess_1").EmbeddedSubProcess().StartEvent().callActivity().calledElement("Process_StuffDoer").camundaVariableMappingClass("api.Variables.scope.SetVariableMappingDelegate").serviceTask().camundaClass("api.Variables.scope.AssertVariableScopeDelegate").EndEvent().subProcessDone().EndEvent().Done();
		//instance = Modify(instance).activityBuilder("SubProcess_1").MultiInstance().parallel().camundaCollection("orderIds").camundaElementVariable("orderId").Done();

		//IProcessDefinition processDefinition = testHelper.DeployAndGetDefinition(instance);
		//IVariableMap variables = Variable.Variables.CreateVariables().PutValue("orderIds", (new int[]{1, 2, 3}));
		//engineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id, variables as IDictionary<string, ITypedValue>);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/variables/scope/doer.bpmn"}) public void testWithDelegateVariableMappingAndChildScope()
	  public virtual void testWithDelegateVariableMappingAndChildScope()
	  {
		//IBpmnModelInstance instance = Model.Bpmn.Bpmn.CreateExecutableProcess("process1").StartEvent().ParallelGateway().SubProcess("SubProcess_1").EmbeddedSubProcess().StartEvent().callActivity().calledElement("Process_StuffDoer").camundaVariableMappingClass("api.Variables.scope.SetVariableToChildMappingDelegate").serviceTask().camundaClass("api.Variables.scope.AssertVariableScopeDelegate").EndEvent().subProcessDone().MoveToLastGateway().SubProcess("SubProcess_2").EmbeddedSubProcess().StartEvent().UserTask("ut").EndEvent().subProcessDone().EndEvent().Done();
		//instance = modify(instance).activityBuilder("SubProcess_1").MultiInstance().parallel().camundaCollection("orderIds").camundaElementVariable("orderId").Done();

		//IProcessDefinition processDefinition = testHelper.DeployAndGetDefinition(instance);
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage.StartsWith("org.camunda.bpm.Engine.ProcessEngineException: ENGINE-20011 Scope with specified activity Id SubProcess_2 and execution"));
		//IVariableMap variables = Variable.Variables.CreateVariables().PutValue("orderIds", (new int[]{1, 2, 3}));
		//engineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id,variables as IDictionary<string, ITypedValue>);

        }


	}

}