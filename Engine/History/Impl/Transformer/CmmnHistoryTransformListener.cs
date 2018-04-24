//using System.Collections.Generic;

//
//
// 
//
// 
//
// 
// 
// 
// 
// 
// 

//namespace org.camunda.bpm.engine.impl.history.transformer
//{

//	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
//	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
//	using CmmnHistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.CmmnHistoryEventProducer;

//	/// <summary>
//	/// 
//	/// </summary>
//	public class CmmnHistoryTransformListener : CmmnTransformListener
//	{

//	  // Cached listeners
//	  // listeners can be reused for a given process engine instance but cannot be cached in static fields since
//	  // different process engine instances on the same Classloader may have different HistoryEventProducer
//	  // configurations wired
//	  protected internal CaseExecutionListener CASE_INSTANCE_CREATE_LISTENER;
//	  protected internal CaseExecutionListener CASE_INSTANCE_UPDATE_LISTENER;
//	  protected internal CaseExecutionListener CASE_INSTANCE_CLOSE_LISTENER;

//	  protected internal CaseExecutionListener CASE_ACTIVITY_INSTANCE_CREATE_LISTENER;
//	  protected internal CaseExecutionListener CASE_ACTIVITY_INSTANCE_UPDATE_LISTENER;
//	  protected internal CaseExecutionListener CASE_ACTIVITY_INSTANCE_END_LISTENER;

//	  // The history level set in the process engine configuration
//	  protected internal IHistoryLevel historyLevel;

//	  public CmmnHistoryTransformListener(IHistoryLevel historyLevel, CmmnHistoryEventProducer historyEventProducer)
//	  {
//		this.historyLevel = historyLevel;
//		initCaseExecutionListeners(historyEventProducer, historyLevel);
//	  }

//	  protected internal virtual void initCaseExecutionListeners(CmmnHistoryEventProducer historyEventProducer, IHistoryLevel historyLevel)
//	  {
//		CASE_INSTANCE_CREATE_LISTENER = new CaseInstanceCreateListener(historyEventProducer, historyLevel);
//		CASE_INSTANCE_UPDATE_LISTENER = new CaseInstanceUpdateListener(historyEventProducer, historyLevel);
//		CASE_INSTANCE_CLOSE_LISTENER = new CaseInstanceCloseListener(historyEventProducer, historyLevel);

//		CASE_ACTIVITY_INSTANCE_CREATE_LISTENER = new CaseActivityInstanceCreateListener(historyEventProducer, historyLevel);
//		CASE_ACTIVITY_INSTANCE_UPDATE_LISTENER = new CaseActivityInstanceUpdateListener(historyEventProducer, historyLevel);
//		CASE_ACTIVITY_INSTANCE_END_LISTENER = new CaseActivityInstanceEndListener(historyEventProducer, historyLevel);
//	  }

//	  public virtual void transformRootElement<T1>(Definitions definitions, IList<T1> caseDefinitions) where T1 : org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition
//	  {
//	  }

//	  public virtual void transformCase(Case element, CmmnCaseDefinition caseDefinition)
//	  {
//	  }

//	  public virtual void transformCasePlanModel(org.camunda.bpm.model.cmmn.impl.instance.CasePlanModel casePlanModel, CmmnActivity caseActivity)
//	  {
//		transformCasePlanModel((CasePlanModel) casePlanModel, caseActivity);
//	  }

//	  public virtual void transformCasePlanModel(CasePlanModel casePlanModel, CmmnActivity caseActivity)
//	  {
//		addCasePlanModelHandlers(caseActivity);
//	  }

//	  public virtual void transformHumanTask(PlanItem planItem, HumanTask humanTask, CmmnActivity caseActivity)
//	  {
//		addTaskOrStageHandlers(caseActivity);
//	  }

//	  public virtual void transformProcessTask(PlanItem planItem, ProcessTask processTask, CmmnActivity caseActivity)
//	  {
//		addTaskOrStageHandlers(caseActivity);
//	  }

//	  public virtual void transformCaseTask(PlanItem planItem, CaseTask caseTask, CmmnActivity caseActivity)
//	  {
//		addTaskOrStageHandlers(caseActivity);
//	  }

//	  public virtual void transformDecisionTask(PlanItem planItem, DecisionTask decisionTask, CmmnActivity caseActivity)
//	  {
//		addTaskOrStageHandlers(caseActivity);
//	  }

//	  public virtual void transformTask(PlanItem planItem, ITask ITask, CmmnActivity caseActivity)
//	  {
//		addTaskOrStageHandlers(caseActivity);
//	  }

//	  public virtual void transformStage(PlanItem planItem, Stage stage, CmmnActivity caseActivity)
//	  {
//		addTaskOrStageHandlers(caseActivity);
//	  }

//	  public virtual void transformMilestone(PlanItem planItem, Milestone milestone, CmmnActivity caseActivity)
//	  {
//		addEventListenerOrMilestoneHandlers(caseActivity);
//	  }

//	  public virtual void transformEventListener(PlanItem planItem, EventListener eventListener, CmmnActivity caseActivity)
//	  {
//		addEventListenerOrMilestoneHandlers(caseActivity);
//	  }

//	  public virtual void transformSentry(Sentry sentry, CmmnSentryDeclaration sentryDeclaration)
//	  {
//	  }

//	  protected internal virtual void addCasePlanModelHandlers(CmmnActivity caseActivity)
//	  {
//		if (caseActivity != null)
//		{
//		  if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.CASE_INSTANCE_CREATE, null))
//		  {
//			foreach (string @event in ItemHandler.CASE_PLAN_MODEL_CREATE_EVENTS)
//			{
//			  caseActivity.addBuiltInListener(@event, CASE_INSTANCE_CREATE_LISTENER);
//			}
//		  }
//		  if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.CASE_INSTANCE_UPDATE, null))
//		  {
//			foreach (string @event in ItemHandler.CASE_PLAN_MODEL_UPDATE_EVENTS)
//			{
//			  caseActivity.addBuiltInListener(@event, CASE_INSTANCE_UPDATE_LISTENER);
//			}
//		  }
//		  if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.CASE_INSTANCE_CLOSE, null))
//		  {
//			foreach (string @event in ItemHandler.CASE_PLAN_MODEL_CLOSE_EVENTS)
//			{
//			  caseActivity.addBuiltInListener(@event, CASE_INSTANCE_CLOSE_LISTENER);
//			}
//		  }
//		}
//	  }

//	  protected internal virtual void addTaskOrStageHandlers(CmmnActivity caseActivity)
//	  {
//		if (caseActivity != null)
//		{
//		  if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_CREATE, null))
//		  {
//			foreach (string @event in ItemHandler.TASK_OR_STAGE_CREATE_EVENTS)
//			{
//			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_CREATE_LISTENER);
//			}
//		  }
//		  if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_UPDATE, null))
//		  {
//			foreach (string @event in ItemHandler.TASK_OR_STAGE_UPDATE_EVENTS)
//			{
//			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_UPDATE_LISTENER);
//			}
//		  }
//		  if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_END, null))
//		  {
//			foreach (string @event in ItemHandler.TASK_OR_STAGE_END_EVENTS)
//			{
//			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_END_LISTENER);
//			}
//		  }
//		}
//	  }

//	  protected internal virtual void addEventListenerOrMilestoneHandlers(CmmnActivity caseActivity)
//	  {
//		if (caseActivity != null)
//		{
//		  if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_CREATE, null))
//		  {
//			foreach (string @event in ItemHandler.EVENT_LISTENER_OR_MILESTONE_CREATE_EVENTS)
//			{
//			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_CREATE_LISTENER);
//			}
//		  }
//		  if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_UPDATE, null))
//		  {
//			foreach (string @event in ItemHandler.EVENT_LISTENER_OR_MILESTONE_UPDATE_EVENTS)
//			{
//			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_UPDATE_LISTENER);
//			}
//		  }
//		  if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_END, null))
//		  {
//			foreach (string @event in ItemHandler.EVENT_LISTENER_OR_MILESTONE_END_EVENTS)
//			{
//			  caseActivity.addBuiltInListener(@event, CASE_ACTIVITY_INSTANCE_END_LISTENER);
//			}
//		  }
//		}
//	  }

//	}

//}

