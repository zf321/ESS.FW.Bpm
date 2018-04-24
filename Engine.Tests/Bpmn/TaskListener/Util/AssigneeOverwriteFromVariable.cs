using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.TaskListener.Util
{
    

	/// <summary>
	///  <falko.Menge@camunda.com>
	/// </summary>
	public class AssigneeOverwriteFromVariable : ITaskListener
	{
        
      public virtual void Notify(IDelegateTask delegateTask)
	  {
		// get mapping table from variable
		IDelegateExecution execution = delegateTask.Execution;
		IDictionary<string, string> assigneeMappingTable = (IDictionary<string, string>) execution.GetVariable("assigneeMappingTable");

		// get assignee from process
		string assigneeFromProcessDefinition = delegateTask.Assignee;

		// overwrite assignee if there is an entry in the mapping table
		if (assigneeMappingTable.ContainsKey(assigneeFromProcessDefinition))
		{
		  string assigneeFromMappingTable = assigneeMappingTable[assigneeFromProcessDefinition];
		  delegateTask.Assignee = assigneeFromMappingTable;
		}
	  }

	}

}