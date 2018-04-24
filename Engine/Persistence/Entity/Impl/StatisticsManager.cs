
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Repository;





namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    [System.Obsolete("É¾³ý",true)]
	public class StatisticsManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.management.ProcessDefinitionStatistics> getStatisticsGroupedByProcessDefinitionVersion(org.camunda.bpm.engine.impl.ProcessDefinitionStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
//	  public virtual IList<IProcessDefinitionStatistics> GetStatisticsGroupedByProcessDefinitionVersion(ProcessDefinitionStatisticsQueryImpl query, Page page)
//	  {
//		ConfigureQuery(query);
//		return ListExt.ConvertToListT <IProcessDefinitionStatistics> (DbEntityManager.SelectList("selectProcessDefinitionStatistics", query, page));
//	  }

//	  public virtual long GetStatisticsCountGroupedByProcessDefinitionVersion(ProcessDefinitionStatisticsQueryImpl query)
//	  {
//		ConfigureQuery(query);
//		return (long) DbEntityManager.SelectOne("selectProcessDefinitionStatisticsCount", query);
//	  }

////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.management.ActivityStatistics> getStatisticsGroupedByActivity(org.camunda.bpm.engine.impl.ActivityStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
//	  public virtual IList<IActivityStatistics> GetStatisticsGroupedByActivity(ActivityStatisticsQueryImpl query, Page page)
//	  {
//		ConfigureQuery(query);
//		return ListExt.ConvertToListT<IActivityStatistics>(DbEntityManager.SelectList("selectActivityStatistics", query, page));
//	  }

//	  public virtual long GetStatisticsCountGroupedByActivity(ActivityStatisticsQueryImpl query)
//	  {
//		ConfigureQuery(query);
//		return (long) DbEntityManager.SelectOne("selectActivityStatisticsCount", query);
//	  }

////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.management.DeploymentStatistics> getStatisticsGroupedByDeployment(org.camunda.bpm.engine.impl.DeploymentStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
//	  public virtual IList<IDeploymentStatistics> GetStatisticsGroupedByDeployment(DeploymentStatisticsQueryImpl query, Page page)
//	  {
//		ConfigureQuery(query);
//		return ListExt.ConvertToListT<IDeploymentStatistics>(DbEntityManager.SelectList("selectDeploymentStatistics", query, page)) ;
//	  }

//	  public virtual long GetStatisticsCountGroupedByDeployment(DeploymentStatisticsQueryImpl query)
//	  {
//		ConfigureQuery(query);
//		return (long) DbEntityManager.SelectOne("selectDeploymentStatisticsCount", query);
//	  }

////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.batch.BatchStatistics> getStatisticsGroupedByBatch(org.camunda.bpm.engine.impl.batch.BatchStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
//	  public virtual IList<IBatchStatistics> GetStatisticsGroupedByBatch(BatchStatisticsQueryImpl query, Page page)
//	  {
//		ConfigureQuery(query);
//		return ListExt.ConvertToListT<IBatchStatistics>(DbEntityManager.SelectList("selectBatchStatistics", query, page)) ;
//	  }

//	  public virtual long GetStatisticsCountGroupedByBatch(BatchStatisticsQueryImpl query)
//	  {
//		ConfigureQuery(query);
//		return (long) DbEntityManager.SelectOne("selectBatchStatisticsCount", query);
//	  }

//	  protected internal virtual void ConfigureQuery(DeploymentStatisticsQueryImpl query)
//	  {
//		AuthorizationManager.ConfigureDeploymentStatisticsQuery(query);
//		TenantManager.ConfigureQuery(query);
//	  }

//	  protected internal virtual void ConfigureQuery(ProcessDefinitionStatisticsQueryImpl query)
//	  {
//		AuthorizationManager.ConfigureProcessDefinitionStatisticsQuery(query);
//		TenantManager.ConfigureQuery(query);
//	  }

//	  protected internal virtual void ConfigureQuery(ActivityStatisticsQueryImpl query)
//	  {
//		CheckReadProcessDefinition(query);
//		AuthorizationManager.ConfigureActivityStatisticsQuery(query);
//		TenantManager.ConfigureQuery(query);
//	  }

//	  protected internal virtual void ConfigureQuery(BatchStatisticsQueryImpl batchQuery)
//	  {
//		AuthorizationManager.ConfigureBatchStatisticsQuery(batchQuery);
//		TenantManager.ConfigureQuery(batchQuery);
//	  }

//	  protected internal virtual void CheckReadProcessDefinition(ActivityStatisticsQueryImpl query)
//	  {
//		CommandContext commandContext = CommandContext;
//		if (AuthorizationEnabled && CurrentAuthentication != null && commandContext.AuthorizationCheckEnabled)
//		{
//		  string processDefinitionId = query.ProcessDefinitionId;
//		  ProcessDefinitionEntity definition = ProcessDefinitionManager.FindLatestProcessDefinitionById(processDefinitionId);
//		  //EnsureNotNull("no deployed process definition found with id '" + processDefinitionId + "'", "processDefinition", definition);
//		  AuthorizationManager.CheckAuthorization(Permissions.Read,Resources.ProcessDefinition, definition.Key);
//		}
//	  }

//	  public virtual long GetStatisticsCountGroupedByDecisionRequirementsDefinition(HistoricDecisionInstanceStatisticsQueryImpl decisionRequirementsDefinitionStatisticsQuery)
//	  {
//		ConfigureQuery(decisionRequirementsDefinitionStatisticsQuery);
//		return (long) DbEntityManager.SelectOne("selectDecisionDefinitionStatisticsCount", decisionRequirementsDefinitionStatisticsQuery);
//	  }

//	  protected internal virtual void ConfigureQuery(HistoricDecisionInstanceStatisticsQueryImpl decisionRequirementsDefinitionStatisticsQuery)
//	  {
//		CheckReadDecisionRequirementsDefinition(decisionRequirementsDefinitionStatisticsQuery);
//		TenantManager.ConfigureQuery(decisionRequirementsDefinitionStatisticsQuery);
//	  }

//	  protected internal virtual void CheckReadDecisionRequirementsDefinition(HistoricDecisionInstanceStatisticsQueryImpl query)
//	  {
//		CommandContext commandContext = CommandContext;
//		if (AuthorizationEnabled && CurrentAuthentication != null && commandContext.AuthorizationCheckEnabled)
//		{
//		  string decisionRequirementsDefinitionId = query.DecisionRequirementsDefinitionId;
//		  IDecisionRequirementsDefinition definition = DecisionRequirementsDefinitionManager.FindDecisionRequirementsDefinitionById(decisionRequirementsDefinitionId);
//		  //EnsureNotNull("no deployed decision requirements definition found with id '" + decisionRequirementsDefinitionId + "'", "decisionRequirementsDefinition", definition);
//		  AuthorizationManager.CheckAuthorization(Permissions.Read,Resources.DecisionRequirementsDefinition , definition.Key);
//		}
//	  }

//	  public virtual IList<IHistoricDecisionInstanceStatistics> GetStatisticsGroupedByDecisionRequirementsDefinition(HistoricDecisionInstanceStatisticsQueryImpl query, Page page)
//	  {
//		ConfigureQuery(query);
//		return ListExt.ConvertToListT<IHistoricDecisionInstanceStatistics>(DbEntityManager.SelectList("selectDecisionDefinitionStatistics", query, page)) ;
//	  }
	}

}