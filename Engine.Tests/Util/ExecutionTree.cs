using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace Engine.Tests.Util
{

    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class ExecutionTree : IExecution
	{

	  protected internal ExecutionTree parent;
	  protected internal IList<ExecutionTree> Children;
	  protected internal IExecution WrappedExecution;

	  protected internal ExecutionTree(IExecution execution, IList<ExecutionTree> children)
	  {
		this.WrappedExecution = execution;
		this.Children = children;
		foreach (ExecutionTree child in children)
		{
		  child.parent = this;
		}
	  }

	  public static ExecutionTree ForExecution(string executionId, IProcessEngine processEngine)
	  {
		ProcessEngineConfigurationImpl configuration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;

            ICommandExecutor commandExecutor = configuration.CommandExecutorTxRequired;

		ExecutionTree executionTree = commandExecutor.Execute(new CommandAnonymousInnerClass(executionId));

		return executionTree;
	  }

	  private class CommandAnonymousInnerClass : ICommand<ExecutionTree>
	  {
		  private string _executionId;

		  public CommandAnonymousInnerClass(string executionId)
		  {
			  this._executionId = executionId;
		  }

		  public virtual ExecutionTree Execute(CommandContext commandContext)
		  {
			ExecutionEntity execution = commandContext.ExecutionManager.FindExecutionById(_executionId);
			return ExecutionTree.ForExecution(execution);
		  }
	  }

	  protected internal static ExecutionTree ForExecution(ExecutionEntity execution)
	  {
		IList<ExecutionTree> children = new List<ExecutionTree>();

		foreach (ExecutionEntity child in execution.Executions)
		{
		  children.Add(ExecutionTree.ForExecution(child));
		}

		return new ExecutionTree(execution, children);

	  }

	  public virtual IList<ExecutionTree> Executions
	  {
		  get
		  {
			return Children;
		  }
	  }

	  public virtual IList<ExecutionTree> GetLeafExecutions(string activityId)
	  {
		IList<ExecutionTree> executions = new List<ExecutionTree>();

		foreach (ExecutionTree child in Children)
		{
		  if (!child.EventScope.Value)
		  {
			if (!string.ReferenceEquals(child.ActivityId, null))
			{
			  if (activityId.Equals(child.ActivityId))
			  {
				executions.Add(child);
			  }
			}
			else
			{
			  ((List<ExecutionTree>)executions).AddRange(child.GetLeafExecutions(activityId));
			}
		  }
		}

		return executions;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return WrappedExecution.Id;
		  }
	  }

	  public virtual bool IsSuspended
	  {
		  get
		  {
			return WrappedExecution.IsSuspended;
		  }
	  }

	  public virtual bool IsEnded
        {
		  get
		  {
			return WrappedExecution.IsEnded;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return WrappedExecution.ProcessInstanceId;
		  }
	  }

	  public virtual ExecutionTree Parent
	  {
		  get
		  {
			return parent;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return ((PvmExecutionImpl) WrappedExecution).ActivityId;
		  }
	  }

	  public virtual bool? Scope
	  {
		  get
		  {
			return ((PvmExecutionImpl) WrappedExecution).IsScope;
		  }
	  }

	  public virtual bool? Concurrent
	  {
		  get
		  {
			return ((PvmExecutionImpl) WrappedExecution).IsConcurrent;
		  }
	  }

	  public virtual bool? EventScope
	  {
		  get
		  {
			return ((PvmExecutionImpl) WrappedExecution).IsEventScope;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return WrappedExecution.TenantId;
		  }
	  }

	  public virtual IExecution Execution
	  {
		  get
		  {
			return WrappedExecution;
		  }
	  }

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		AppendString("", sb);
		return sb.ToString();
	  }

	  public virtual void AppendString(string prefix, StringBuilder sb)
	  {
		sb.Append(prefix);
		sb.Append(ExecutionTreeToString(this));
		sb.Append("\n");
		foreach (ExecutionTree child in Executions)
		{
		  child.AppendString(prefix + "   ", sb);
		}
	  }

	  protected internal static string ExecutionTreeToString(ExecutionTree executionTree)
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append(executionTree.Execution);

		sb.Append("[activityId=");
		sb.Append(executionTree.ActivityId);

		sb.Append(", isScope=");
		sb.Append(executionTree.Scope);

		sb.Append(", isConcurrent=");
		sb.Append(executionTree.Concurrent);

		sb.Append(", isEventScope=");
		sb.Append(executionTree.EventScope);

		sb.Append("]");

		return sb.ToString();
	  }
        
	}

}