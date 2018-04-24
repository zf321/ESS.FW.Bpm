using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.Query.History
{

	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT) public class MultiTenancyHistoricProcessInstanceStateTest
	public class MultiTenancyHistoricProcessInstanceStateTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricProcessInstanceStateTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			processEngineTestRule = new ProcessEngineTestRule(processEngineRule);
			//ruleChain = RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
		}

	  public const string PROCESS_ID = "process1";
	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule processEngineTestRule;
        
	   [Test]   public virtual void testSuspensionWithTenancy()
	  {
		IBpmnModelInstance instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID).StartEvent().UserTask().EndEvent().Done();
		IProcessDefinition processDefinition = processEngineTestRule.DeployAndGetDefinition(instance);
		IProcessDefinition processDefinition1 = processEngineTestRule.DeployForTenantAndGetDefinition(TENANT_ONE, instance);
		IProcessDefinition processDefinition2 = processEngineTestRule.DeployForTenantAndGetDefinition(TENANT_TWO, instance);

		IProcessInstance processInstance = processEngineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id);
		IProcessInstance processInstance1 = processEngineRule.RuntimeService.StartProcessInstanceById(processDefinition1.Id);
		IProcessInstance processInstance2 = processEngineRule.RuntimeService.StartProcessInstanceById(processDefinition2.Id);

		//suspend ITenant one
		processEngineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(processDefinition1.Key).ProcessDefinitionTenantId(processDefinition1.TenantId).Suspend();

		string[] processInstances = new string[] {processInstance1.Id, processInstance2.Id, processInstance.Id};

		verifyStates(processInstances, new string[]{ HistoricProcessInstanceFields.StateSuspended, HistoricProcessInstanceFields.StateActive, HistoricProcessInstanceFields.StateActive });


		//suspend without tenant
		processEngineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(processDefinition.Key).ProcessDefinitionWithoutTenantId().Suspend();

		verifyStates(processInstances, new string[]{ HistoricProcessInstanceFields.StateSuspended, HistoricProcessInstanceFields.StateActive, HistoricProcessInstanceFields.StateSuspended});

		//reactivate without tenant
		processEngineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(processDefinition.Key).ProcessDefinitionWithoutTenantId().Activate();


		verifyStates(processInstances, new string[]{ HistoricProcessInstanceFields.StateSuspended, HistoricProcessInstanceFields.StateActive, HistoricProcessInstanceFields.StateActive });
	  }

	  protected internal virtual void verifyStates(string[] processInstances, string[] states)
	  {
		for (int i = 0; i < processInstances.Length; i++)
		{
		  Assert.That(processEngineRule.HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstances[i]).First().State, Is.EqualTo(states[i]));
		}
	  }
	}

}