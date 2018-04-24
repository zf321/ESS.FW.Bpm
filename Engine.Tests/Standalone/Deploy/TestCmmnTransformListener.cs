//using System;
//using System.Collections.Generic;


//namespace ESS.FW.Bpm.Engine.Tests.Standalone.Deploy
//{
    
//	/// <summary>
//	/// 
//	/// </summary>
//	public class TestCmmnTransformListener : CmmnTransformListener
//	{

//	  public static ISet<CmmnModelElementInstance> modelElementInstances = new HashSet<CmmnModelElementInstance>();
//	  public static ISet<CmmnActivity> cmmnActivities = new HashSet<CmmnActivity>();
//	  public static ISet<CmmnSentryDeclaration> sentryDeclarations = new HashSet<CmmnSentryDeclaration>();

//	  public virtual void transformRootElement<T1>(Definitions definitions, IList<T1> caseDefinitions) where T1 : impl.cmmn.Model.CmmnCaseDefinition
//	  {
//		modelElementInstances.Add(definitions);
//		foreach (CmmnCaseDefinition caseDefinition in caseDefinitions)
//		{
//		  CaseDefinitionEntity entity = (CaseDefinitionEntity) caseDefinition;
//		  entity.Key = entity.Key.concat("-modified");
//		}
//	  }

//	  public virtual void transformCase(Case element, CmmnCaseDefinition caseDefinition)
//	  {
//		modelElementInstances.Add(element);
//		cmmnActivities.Add(caseDefinition);
//	  }

//	  public virtual void transformCasePlanModel(org.Camunda.bpm.Model.cmmn.impl.Instance.CasePlanModel casePlanModel, CmmnActivity caseActivity)
//	  {
//		transformCasePlanModel((CasePlanModel) casePlanModel, caseActivity);
//	  }

//	  public virtual void transformCasePlanModel(CasePlanModel casePlanModel, CmmnActivity activity)
//	  {
//		modelElementInstances.Add(casePlanModel);
//		cmmnActivities.Add(activity);
//	  }

//	  public virtual void transformHumanTask(PlanItem planItem, HumanTask humanTask, CmmnActivity activity)
//	  {
//		modelElementInstances.Add(planItem);
//		modelElementInstances.Add(humanTask);
//		cmmnActivities.Add(activity);
//	  }

//	  public virtual void transformProcessTask(PlanItem planItem, ProcessTask processTask, CmmnActivity activity)
//	  {
//		modelElementInstances.Add(planItem);
//		modelElementInstances.Add(processTask);
//		cmmnActivities.Add(activity);
//	  }

//	  public virtual void transformCaseTask(PlanItem planItem, CaseTask caseTask, CmmnActivity activity)
//	  {
//		modelElementInstances.Add(planItem);
//		modelElementInstances.Add(caseTask);
//		cmmnActivities.Add(activity);
//	  }

//	  public virtual void transformDecisionTask(PlanItem planItem, DecisionTask decisionTask, CmmnActivity activity)
//	  {
//		modelElementInstances.Add(planItem);
//		modelElementInstances.Add(decisionTask);
//		cmmnActivities.Add(activity);
//	  }

//	  public virtual void transformTask(PlanItem planItem, Task task, CmmnActivity activity)
//	  {
//		modelElementInstances.Add(planItem);
//		modelElementInstances.Add(task);
//		cmmnActivities.Add(activity);
//	  }

//	  public virtual void transformStage(PlanItem planItem, Stage stage, CmmnActivity activity)
//	  {
//		modelElementInstances.Add(planItem);
//		modelElementInstances.Add(stage);
//		cmmnActivities.Add(activity);
//	  }

//	  public virtual void transformMilestone(PlanItem planItem, Milestone milestone, CmmnActivity activity)
//	  {
//		modelElementInstances.Add(planItem);
//		modelElementInstances.Add(milestone);
//		cmmnActivities.Add(activity);
//	  }

//	  public virtual void transformEventListener(PlanItem planItem, EventListener eventListener, CmmnActivity activity)
//	  {
//		modelElementInstances.Add(planItem);
//		modelElementInstances.Add(eventListener);
//		cmmnActivities.Add(activity);
//	  }

//	  public virtual void transformSentry(Sentry sentry, CmmnSentryDeclaration sentryDeclaration)
//	  {
//		modelElementInstances.Add(sentry);
//		sentryDeclarations.Add(sentryDeclaration);
//	  }

//	  protected internal virtual string getNewName(string name)
//	  {
//		if (name.EndsWith("-modified", StringComparison.Ordinal))
//		{
//		  return name + "-again";
//		}
//		else
//		{
//		  return name + "-modified";
//		}
//	  }

//	  public static void reset()
//	  {
//		modelElementInstances = new HashSet<CmmnModelElementInstance>();
//		cmmnActivities = new HashSet<CmmnActivity>();
//		sentryDeclarations = new HashSet<CmmnSentryDeclaration>();
//	  }

//	  public static int numberOfRegistered(Type modelElementInstanceClass)
//	  {
//		int Count = 0;
//		foreach (CmmnModelElementInstance element in modelElementInstances)
//		{
//		  if (modelElementInstanceClass.IsInstanceOfType(element))
//		  {
//			Count++;
//		  }
//		}
//		return Count;
//	  }

//	}

//}