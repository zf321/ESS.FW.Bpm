using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Util
{

    

    /// <summary>
	/// 
	/// 
	/// </summary>
	public class ExecutionTreeProcessDefinitionIdAssertion : IExecutionTreeAssertion
	{

	  protected internal string ExpectedProcessDefinitionId;

	  public virtual void AssertExecution(ExecutionTree tree)
	  {
		IList<IExecution> nonMatchingExecutions = Matches(tree);

		if (nonMatchingExecutions.Count > 0)
		{
		  StringBuilder sb = new StringBuilder();
		  sb.Append("Expected all executions to have process definition id " + ExpectedProcessDefinitionId + "\n");
		  sb.Append("Actual Tree: \n");
		  sb.Append(tree);
		  sb.Append("\nExecutions with unexpected process definition id:\n");
		  sb.Append("[\n");
		  foreach (IExecution execution in nonMatchingExecutions)
		  {
			sb.Append(execution);
			sb.Append("\n");
		  }
		  sb.Append("]\n");
		  Assert.Fail(sb.ToString());
		}
	  }

	  /// <summary>
	  /// returns umatched executions in the tree
	  /// </summary>
	  protected internal virtual IList<IExecution> Matches(ExecutionTree tree)
	  {
		ExecutionEntity executionEntity = (ExecutionEntity) tree.Execution;
		IList<IExecution> unmatchedExecutions = new List<IExecution>();

		if (!ExpectedProcessDefinitionId.Equals(executionEntity.ProcessDefinitionId))
		{
		  unmatchedExecutions.Add(tree.Execution);
		}
		foreach (ExecutionTree child in tree.Executions)
		{
		  ((List<IExecution>)unmatchedExecutions).AddRange(Matches(child));
		}

		return unmatchedExecutions;
	  }

	  public static ExecutionTreeProcessDefinitionIdAssertion ProcessDefinitionId(string expectedProcessDefinitionId)
	  {
		ExecutionTreeProcessDefinitionIdAssertion Assertion = new ExecutionTreeProcessDefinitionIdAssertion();
		Assertion.ExpectedProcessDefinitionId = expectedProcessDefinitionId;

		return Assertion;
	  }

	}

}