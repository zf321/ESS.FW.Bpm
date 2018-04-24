using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.TaskListener.Util
{
   
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class RecorderTaskListener : ITaskListener
	{

	  private const long serialVersionUID = 1L;

	  private static IList<RecorderTaskListener.RecordedTaskEvent> recordedEvents = new List<RecorderTaskListener.RecordedTaskEvent>();

	  public class RecordedTaskEvent
	  {

		protected internal string taskId;
		protected internal string executionId;
		protected internal string @event;
		protected internal string activityInstanceId;

		public RecordedTaskEvent(string taskId, string executionId, string @event, string activityInstanceId)
		{
		  this.ExecutionId = executionId;
		  this.taskId = taskId;
		  this.@event = @event;
		  this.ActivityInstanceId = activityInstanceId;
		}

		public virtual string ExecutionId
		{
			get
			{
			  return executionId;
			}
		    set { executionId = value; }
		}

		public virtual string TaskId
		{
			get
			{
			  return taskId;
			}
		}

		public virtual string Event
		{
			get
			{
			  return @event;
			}
		}

		public virtual string ActivityInstanceId
		{
			get
			{
			  return activityInstanceId;
			}
		    set { activityInstanceId = value; }
		}

	  }

	  public virtual void Notify(IDelegateTask task)
	  {
		IDelegateExecution execution = task.Execution;
		recordedEvents.Add(new RecordedTaskEvent(task.Id, task.ExecutionId, task.EventName, execution.ActivityInstanceId));
	  }

      
	  public static void clear()
	  {
		recordedEvents.Clear();
	  }

	  public static IList<RecordedTaskEvent> RecordedEvents
	  {
		  get
		  {
			return recordedEvents;
		  }
	  }


	}

}