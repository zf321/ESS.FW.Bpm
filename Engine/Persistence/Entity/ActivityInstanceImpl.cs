using ESS.FW.Bpm.Engine.Runtime;
using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// 
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ActivityInstanceImpl : ProcessElementInstanceImpl, IActivityInstance
	{

	  protected internal string businessKey;
	  protected internal string activityId;
	  protected internal string activityName;
	  protected internal string activityType;

	  protected internal IActivityInstance[] childActivityInstances = new IActivityInstance[0];
	  protected internal ITransitionInstance[] childTransitionInstances = new ITransitionInstance[0];

	  protected internal string[] executionIds = new string[0];

	  public virtual IActivityInstance[] ChildActivityInstances
	  {
		  get
		  {
			return childActivityInstances;
		  }
		  set
		  {
			this.childActivityInstances = value;
		  }
	  }


	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
		  set
		  {
			this.businessKey = value;
		  }
	  }


	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }


	  public virtual string[] ExecutionIds
	  {
		  get
		  {
			return executionIds;
		  }
		  set
		  {
			this.executionIds = value;
		  }
	  }


	  public virtual ITransitionInstance[] ChildTransitionInstances
	  {
		  get
		  {
			return childTransitionInstances;
		  }
		  set
		  {
			this.childTransitionInstances = value;
		  }
	  }


	  public virtual string ActivityType
	  {
		  get
		  {
			return activityType;
		  }
		  set
		  {
			this.activityType = value;
		  }
	  }


	  public virtual string ActivityName
	  {
		  get
		  {
			return activityName;
		  }
		  set
		  {
			this.activityName = value;
		  }
	  }


	  protected internal virtual void WriteTree(StringBuilder writer, string prefix, bool isTail)
	  {
		writer.Append(prefix);
		if (isTail)
		{
		  writer.Append("������ ");
		}
		else
		{
		  writer.Append("������ ");
		}

		writer.Append(ActivityId + "=>" + Id + "\n");

		for (int i = 0; i < childTransitionInstances.Length; i++)
		{
		  ITransitionInstance transitionInstance = childTransitionInstances[i];
		  bool transitionIsTail = (i == (childTransitionInstances.Length - 1)) && (childActivityInstances.Length == 0);
		  WriteTransition(transitionInstance, writer, prefix + (isTail ? "    " : "�?  "), transitionIsTail);
		}

		for (int i = 0; i < childActivityInstances.Length; i++)
		{
		  ActivityInstanceImpl child = (ActivityInstanceImpl) childActivityInstances[i];
		  child.WriteTree(writer, prefix + (isTail ? "    " : "�?  "), (i == (childActivityInstances.Length - 1)));
		}
	  }

	  protected internal virtual void WriteTransition(ITransitionInstance transition, StringBuilder writer, string prefix, bool isTail)
	  {
		writer.Append(prefix);
		if (isTail)
		{
		  writer.Append("������ ");
		}
		else
		{
		  writer.Append("������ ");
		}

		writer.Append("transition to/from " + transition.ActivityId + ":" + transition.Id + "\n");
	  }

	  public override string ToString()
	  {
		StringBuilder writer = new StringBuilder();
		WriteTree(writer, "", true);
		return writer.ToString();
	  }

	  public virtual IActivityInstance[] GetActivityInstances(string activityId)
	  {
		EnsureUtil.EnsureNotNull("activityId", activityId);

		List<IActivityInstance> instances = new List<IActivityInstance>();
		CollectActivityInstances(activityId, instances);

		return instances.ToArray();
	  }

	  protected internal virtual void CollectActivityInstances(string activityId, IList<IActivityInstance> instances)
	  {
		if (this.activityId.Equals(activityId))
		{
		  instances.Add(this);
		}
		else
		{
		  foreach (IActivityInstance childInstance in childActivityInstances)
		  {
			((ActivityInstanceImpl) childInstance).CollectActivityInstances(activityId, instances);
		  }
		}
	  }

	  public virtual ITransitionInstance[] GetTransitionInstances(string activityId)
	  {
		EnsureUtil.EnsureNotNull("activityId", activityId);

		List<ITransitionInstance> instances = new List<ITransitionInstance>();
		CollectTransitionInstances(activityId, instances);

		return instances.ToArray();
	  }

	  protected internal virtual void CollectTransitionInstances(string activityId, IList<ITransitionInstance> instances)
	  {
		bool instanceFound = false;

		foreach (ITransitionInstance childTransitionInstance in childTransitionInstances)
		{
		  if (activityId.Equals(childTransitionInstance.ActivityId))
		  {
			instances.Add(childTransitionInstance);
			instanceFound = true;
		  }
		}

		if (!instanceFound)
		{
		  foreach (IActivityInstance childActivityInstance in childActivityInstances)
		  {
			((ActivityInstanceImpl) childActivityInstance).CollectTransitionInstances(activityId, instances);
		  }
		}
	  }

	}

}