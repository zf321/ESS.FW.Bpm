using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace Engine.Tests.Util
{
    
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class ExecutionAssert
	{

	  protected internal ExecutionTree Tree;
	  protected internal ICommandExecutor commandExecutor;

	  public static ExecutionAssert That(ExecutionTree tree)
	  {

		ExecutionAssert Assertion = new ExecutionAssert();
		Assertion.Tree = tree;
		return Assertion;
	  }

	  public virtual ExecutionAssert Matches(IExecutionTreeAssertion Assertion)
	  {
		Assertion.AssertExecution(Tree);
		return this;
	  }

	  public virtual ExecutionAssert HasProcessDefinitionId(string expectedProcessDefinitionId)
	  {
		IExecutionTreeAssertion Assertion = ExecutionTreeProcessDefinitionIdAssertion.ProcessDefinitionId(expectedProcessDefinitionId);
		Matches(Assertion);
		return this;
	  }

	  public class ExecutionTreeBuilder
	  {

		protected internal ExecutionTreeStructureAssertion RootAssertion = null;
		protected internal Stack<ExecutionTreeStructureAssertion> ActivityInstanceStack = new Stack<ExecutionTreeStructureAssertion>();

		public ExecutionTreeBuilder(string rootActivityInstanceId)
		{
		  RootAssertion = new ExecutionTreeStructureAssertion();
		  RootAssertion.ExpectedActivityId = rootActivityInstanceId;
		  ActivityInstanceStack.Push(RootAssertion);
		}

		public virtual ExecutionTreeBuilder Child(string activityId)
		{
		  ExecutionTreeStructureAssertion newInstance = new ExecutionTreeStructureAssertion();
		  newInstance.ExpectedActivityId = activityId;

		  ExecutionTreeStructureAssertion parentInstance = ActivityInstanceStack.Peek();
		  parentInstance.AddChildAssertion(newInstance);

		  ActivityInstanceStack.Push(newInstance);

		  return this;
		}

		public virtual ExecutionTreeBuilder Scope()
		{
		  ExecutionTreeStructureAssertion currentAssertion = ActivityInstanceStack.Peek();
		  currentAssertion.ExpectedIsScope = true;
		  return this;
		}

		public virtual ExecutionTreeBuilder Concurrent()
		{
		  ExecutionTreeStructureAssertion currentAssertion = ActivityInstanceStack.Peek();
		  currentAssertion.ExpectedIsConcurrent = true;
		  return this;
		}

		public virtual ExecutionTreeBuilder EventScope()
		{
		  ExecutionTreeStructureAssertion currentAssertion = ActivityInstanceStack.Peek();
		  currentAssertion.ExpectedIsEventScope = true;
		  return this;
		}

		public virtual ExecutionTreeBuilder NoScope()
		{
		  ExecutionTreeStructureAssertion currentAssertion = ActivityInstanceStack.Peek();
		  currentAssertion.ExpectedIsScope = false;
		  return this;
		}

		public virtual ExecutionTreeBuilder Id(string id)
		{
		  ExecutionTreeStructureAssertion currentAssertion = ActivityInstanceStack.Peek();
		  currentAssertion.ExpectedId = id;
		  return this;
		}

		public virtual ExecutionTreeBuilder Up()
		{
		  ActivityInstanceStack.Pop();
		  return this;
		}

		public virtual ExecutionTreeStructureAssertion Done()
		{
		  return RootAssertion;
		}
	  }

	  public static ExecutionTreeBuilder DescribeExecutionTree(string activityId)
	  {
		return new ExecutionTreeBuilder(activityId);
	  }

	}

}