using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Engine.Tests.Util
{
    /// <summary>
	/// 
	/// 
	/// </summary>
	public class ExecutionTreeStructureAssertion : IExecutionTreeAssertion
	{

	  protected internal string expectedActivityId;
	  protected internal bool? expectedIsScope = true;
	  protected internal bool? expectedIsConcurrent = false;
	  protected internal bool? expectedIsEventScope = false;
	  protected internal string expectedId;

	  protected internal IList<ExecutionTreeStructureAssertion> ChildAssertions = new List<ExecutionTreeStructureAssertion>();

	  public virtual void AddChildAssertion(ExecutionTreeStructureAssertion childAssertion)
	  {
		this.ChildAssertions.Add(childAssertion);
	  }

	  public virtual string ExpectedActivityId
	  {
		  set
		  {
			this.ExpectedActivityId = value;
		  }
	  }

	  /// <summary>
	  /// This assumes that all children have been fetched
	  /// </summary>
	  protected internal virtual bool Matches(ExecutionTree tree)
	  {
		// match activity id
		string actualActivityId = tree.ActivityId;
		if (string.ReferenceEquals(expectedActivityId, null) && !string.ReferenceEquals(actualActivityId, null))
		{
		  return false;
		}
		else if (!string.ReferenceEquals(expectedActivityId, null) && !expectedActivityId.Equals(tree.ActivityId))
		{
		  return false;
		}

		if (!string.ReferenceEquals(expectedId, null) && !expectedId.Equals(tree.Id))
		{
		  return false;
		}


		// match is scope
		if (expectedIsScope != null && !expectedIsScope.Equals(tree.Scope))
		{
		  return false;
		}

		if (expectedIsConcurrent != null && !expectedIsConcurrent.Equals(tree.Concurrent))
		{
		  return false;
		}

		if (expectedIsEventScope != null && !expectedIsEventScope.Equals(tree.EventScope))
		{
		  return false;
		}

		// match children
		if (tree.Executions.Count != ChildAssertions.Count)
		{
		  return false;
		}

		IList<ExecutionTreeStructureAssertion> unmatchedChildAssertions = new List<ExecutionTreeStructureAssertion>(ChildAssertions);
		foreach (ExecutionTree child in tree.Executions)
		{
		  foreach (ExecutionTreeStructureAssertion childAssertion in unmatchedChildAssertions)
		  {
			if (childAssertion.Matches(child))
			{
			  unmatchedChildAssertions.Remove(childAssertion);
			  break;
			}
		  }
		}

		if (unmatchedChildAssertions.Count > 0)
		{
		  return false;
		}

		return true;
	  }

	  public virtual void AssertExecution(ExecutionTree tree)
	  {
            bool matches = Matches(tree);
            if (!matches)
            {
                StringBuilder errorBuilder = new StringBuilder();
                errorBuilder.Append("Expected tree: \n");
                Describe(this, "", errorBuilder);
                errorBuilder.Append("Actual tree: \n");
                errorBuilder.Append(tree);
                Assert.Fail(errorBuilder.ToString());
            }
        }

	  public static void Describe(ExecutionTreeStructureAssertion Assertion, string prefix, StringBuilder errorBuilder)
	  {
		errorBuilder.Append(prefix);
		errorBuilder.Append(Assertion);
		errorBuilder.Append("\n");
		foreach (ExecutionTreeStructureAssertion child in Assertion.ChildAssertions)
		{
		  Describe(child, prefix + "   ", errorBuilder);
		}
	  }

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("[activityId=");
		sb.Append(expectedActivityId);

		if (!string.ReferenceEquals(expectedId, null))
		{
		  sb.Append(", id=");
		  sb.Append(expectedId);
		}

		if (expectedIsScope != null)
		{
		  sb.Append(", isScope=");
		  sb.Append(expectedIsScope);
		}

		if (expectedIsConcurrent != null)
		{
		  sb.Append(", isConcurrent=");
		  sb.Append(expectedIsConcurrent);
		}

		if (expectedIsEventScope != null)
		{
		  sb.Append(", isEventScope=");
		  sb.Append(expectedIsEventScope);
		}

		sb.Append("]");

		return sb.ToString();
	  }

	  public virtual bool? ExpectedIsScope
	  {
		  set
		  {
			this.ExpectedIsScope = value;
    
		  }
	  }

	  public virtual bool? ExpectedIsConcurrent
	  {
		  set
		  {
			this.ExpectedIsConcurrent = value;
		  }
	  }

	  public virtual bool? ExpectedIsEventScope
	  {
		  set
		  {
			this.ExpectedIsEventScope = value;
		  }
	  }

	  public virtual string ExpectedId
	  {
		  set
		  {
			this.ExpectedId = value;
		  }
	  }

	}

}