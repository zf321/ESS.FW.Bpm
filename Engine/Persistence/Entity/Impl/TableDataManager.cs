//using System;
//using System.Collections;
//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Batch.Impl;
//using ESS.FW.Bpm.Engine.Batch.Impl.History;
//using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
//using ESS.FW.Bpm.Engine.Filter;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.History.Impl.Event;
//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Impl.DB;
//using ESS.FW.Bpm.Engine.Impl.DB.Sql;
//using ESS.FW.Bpm.Engine.Management;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Task;



//namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
//{


//    /// <summary>
//	///  
//	/// </summary>
//    [Obsolete("É¾³ý",true)]
//	public class TableDataManager : AbstractManager
//	{

//	  protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

//	  public static IDictionary<Type, string> ApiTypeToTableNameMap = new Dictionary<Type, string>();
//	  public static IDictionary<Type, string> PersistentObjectToTableNameMap = new Dictionary<Type, string>();

//	  static TableDataManager()
//	  {
//		// runtime
//		PersistentObjectToTableNameMap[typeof(TaskEntity)] = "ACT_RU_TASK";
//		PersistentObjectToTableNameMap[typeof(ExternalTaskEntity)] = "ACT_RU_EXT_TASK";
//		PersistentObjectToTableNameMap[typeof(ExecutionEntity)] = "ACT_RU_EXECUTION";
//		PersistentObjectToTableNameMap[typeof(IdentityLinkEntity)] = "ACT_RU_IDENTITYLINK";
//		PersistentObjectToTableNameMap[typeof(VariableInstanceEntity)] = "ACT_RU_VARIABLE";

//		PersistentObjectToTableNameMap[typeof(JobEntity)] = "ACT_RU_JOB";
//		PersistentObjectToTableNameMap[typeof(MessageEntity)] = "ACT_RU_JOB";
//		PersistentObjectToTableNameMap[typeof(TimerEntity)] = "ACT_RU_JOB";
//		PersistentObjectToTableNameMap[typeof(JobDefinitionEntity)] = "ACT_RU_JOBDEF";
//		PersistentObjectToTableNameMap[typeof(BatchEntity)] = "ACT_RU_BATCH";

//		PersistentObjectToTableNameMap[typeof(IncidentEntity)] = "ACT_RU_INCIDENT";

//		PersistentObjectToTableNameMap[typeof(EventSubscriptionEntity)] = "ACT_RU_EVENT_SUBSCR";

//		PersistentObjectToTableNameMap[typeof(FilterEntity)] = "ACT_RU_FILTER";

//		PersistentObjectToTableNameMap[typeof(MeterLogEntity)] = "ACT_RU_METER_LOG";
//		// repository
//		PersistentObjectToTableNameMap[typeof(DeploymentEntity)] = "ACT_RE_DEPLOYMENT";
//		PersistentObjectToTableNameMap[typeof(ProcessDefinitionEntity)] = "ACT_RE_PROCDEF";

//		//// CMMN
//		//PersistentObjectToTableNameMap[typeof(CaseDefinitionEntity)] = "ACT_RE_CASE_DEF";
//		//PersistentObjectToTableNameMap[typeof(CaseExecutionEntity)] = "ACT_RU_CASE_EXECUTION";
//		//PersistentObjectToTableNameMap[typeof(CaseSentryPartEntity)] = "ACT_RU_CASE_SENTRY_PART";

//		// DMN
//		PersistentObjectToTableNameMap[typeof(DecisionRequirementsDefinitionEntity)] = "ACT_RE_DECISION_REQ_DEF";
//		PersistentObjectToTableNameMap[typeof(DecisionDefinitionEntity)] = "ACT_RE_DECISION_DEF";
//		PersistentObjectToTableNameMap[typeof(HistoricDecisionInputInstanceEntity)] = "ACT_HI_DEC_IN";
//		PersistentObjectToTableNameMap[typeof(HistoricDecisionOutputInstanceEntity)] = "ACT_HI_DEC_OUT";

//		// history
//		PersistentObjectToTableNameMap[typeof(CommentEntity)] = "ACT_HI_COMMENT";

//		PersistentObjectToTableNameMap[typeof(HistoricActivityInstanceEntity)] = "ACT_HI_ACTINST";
//		PersistentObjectToTableNameMap[typeof(AttachmentEntity)] = "ACT_HI_ATTACHMENT";
//		PersistentObjectToTableNameMap[typeof(HistoricProcessInstanceEntity)] = "ACT_HI_PROCINST";
//		PersistentObjectToTableNameMap[typeof(HistoricTaskInstanceEntity)] = "ACT_HI_TASKINST";
//		PersistentObjectToTableNameMap[typeof(HistoricJobLogEventEntity)] = "ACT_HI_JOB_LOG";
//		PersistentObjectToTableNameMap[typeof(HistoricIncidentEventEntity)] = "ACT_HI_INCIDENT";
//		PersistentObjectToTableNameMap[typeof(HistoricBatchEntity)] = "ACT_HI_BATCH";
//		PersistentObjectToTableNameMap[typeof(HistoricExternalTaskLogEntity)] = "ACT_HI_EXT_TASK_LOG";

//		PersistentObjectToTableNameMap[typeof(HistoricCaseInstanceEntity)] = "ACT_HI_CASEINST";
//		PersistentObjectToTableNameMap[typeof(HistoricCaseActivityInstanceEntity)] = "ACT_HI_CASEACTINST";
//		PersistentObjectToTableNameMap[typeof(HistoricIdentityLinkLogEntity)] = "ACT_HI_IDENTITYLINK";
//		// a couple of stuff goes to the same table
//		PersistentObjectToTableNameMap[typeof(HistoricFormPropertyEntity)] = "ACT_HI_DETAIL";
//		PersistentObjectToTableNameMap[typeof(HistoricVariableInstanceEntity)] = "ACT_HI_DETAIL";
//		PersistentObjectToTableNameMap[typeof(HistoricVariableInstanceEntity)] = "ACT_HI_VARINST";
//		PersistentObjectToTableNameMap[typeof(HistoricDetailEventEntity)] = "ACT_HI_DETAIL";

//		PersistentObjectToTableNameMap[typeof(HistoricDecisionInstanceEntity)] = "ACT_HI_DECINST";
//		PersistentObjectToTableNameMap[typeof(UserOperationLogEntryEventEntity)] = "ACT_HI_OP_LOG";


//		// Identity module
//		PersistentObjectToTableNameMap[typeof(GroupEntity)] = "ACT_ID_GROUP";
//		PersistentObjectToTableNameMap[typeof(MembershipEntity)] = "ACT_ID_MEMBERSHIP";
//		PersistentObjectToTableNameMap[typeof(TenantEntity)] = "ACT_ID_TENANT";
//		PersistentObjectToTableNameMap[typeof(TenantMembershipEntity)] = "ACT_ID_TENANT_MEMBER";
//		PersistentObjectToTableNameMap[typeof(UserEntity)] = "ACT_ID_USER";
//		PersistentObjectToTableNameMap[typeof(IdentityInfoEntity)] = "ACT_ID_INFO";
//		PersistentObjectToTableNameMap[typeof(AuthorizationEntity)] = "ACT_RU_AUTHORIZATION";


//		// general
//		PersistentObjectToTableNameMap[typeof(PropertyEntity)] = "ACT_GE_PROPERTY";
//		PersistentObjectToTableNameMap[typeof(ResourceEntity)] = "ACT_GE_BYTEARRAY";
//		PersistentObjectToTableNameMap[typeof(ResourceEntity)] = "ACT_GE_BYTEARRAY";
//		PersistentObjectToTableNameMap[typeof(FilterEntity)] = "ACT_RU_FILTER";

//		// and now the map for the API types (does not cover all cases)
//		ApiTypeToTableNameMap[typeof(ITask)] = "ACT_RU_TASK";
//		ApiTypeToTableNameMap[typeof(IExecution)] = "ACT_RU_EXECUTION";
//		ApiTypeToTableNameMap[typeof(IProcessInstance)] = "ACT_RU_EXECUTION";
//		ApiTypeToTableNameMap[typeof(IProcessDefinition)] = "ACT_RE_PROCDEF";
//		ApiTypeToTableNameMap[typeof(IDeployment)] = "ACT_RE_DEPLOYMENT";
//		ApiTypeToTableNameMap[typeof(IJob)] = "ACT_RU_JOB";
//		ApiTypeToTableNameMap[typeof(IIncident)] = "ACT_RU_INCIDENT";
//		ApiTypeToTableNameMap[typeof(IFilter)] = "ACT_RU_FILTER";


//		// history
//		ApiTypeToTableNameMap[typeof(IHistoricProcessInstance)] = "ACT_HI_PROCINST";
//		ApiTypeToTableNameMap[typeof(IHistoricActivityInstance)] = "ACT_HI_ACTINST";
//		ApiTypeToTableNameMap[typeof(IHistoricDetail)] = "ACT_HI_DETAIL";
//		ApiTypeToTableNameMap[typeof(IHistoricVariableUpdate)] = "ACT_HI_DETAIL";
//		ApiTypeToTableNameMap[typeof(IHistoricFormProperty)] = "ACT_HI_DETAIL";
//		ApiTypeToTableNameMap[typeof(IHistoricTaskInstance)] = "ACT_HI_TASKINST";
//		ApiTypeToTableNameMap[typeof(IHistoricVariableInstance)] = "ACT_HI_VARINST";


//		ApiTypeToTableNameMap[typeof(IHistoricCaseInstance)] = "ACT_HI_CASEINST";
//		ApiTypeToTableNameMap[typeof(IHistoricCaseActivityInstance)] = "ACT_HI_CASEACTINST";

//		ApiTypeToTableNameMap[typeof(IHistoricDecisionInstance)] = "ACT_HI_DECINST";

//		// TODO: Identity skipped for the moment as no SQL injection is provided here
//	  }

//	  public virtual IDictionary<string, long> TableCount
//	  {
//		  get
//		  {
//			IDictionary<string, long> tableCount = new Dictionary<string, long>();
//			try
//			{
//			 // foreach (string tableName in DbEntityManager.TableNamesPresentInDatabase)
//			 // {
//				//tableCount[tableName] = GetTableCount(tableName);
//			 // }
//			  Log.CountRowsPerProcessEngineTable(tableCount);
//			}
//			catch (System.Exception e)
//			{
//			  throw Log.CountTableRowsException(e);
//			}
//			return tableCount;
//		  }
//	  }

//	  protected internal virtual long GetTableCount(string tableName)
//	  {
//	      throw new NotImplementedException();
//	      //Log.SelectTableCountForTable(tableName);
//	      //long count = (long) DbEntityManager.SelectOne("selectTableCount", Collections.singletonMap("tableName", tableName));
//	      //return count;
//	  }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @SuppressWarnings("unchecked") public org.camunda.bpm.engine.management.TablePage getTablePage(org.camunda.bpm.engine.impl.TablePageQueryImpl tablePageQuery, int firstResult, int maxResults)
//        public virtual TablePage GetTablePage(TablePageQueryImpl tablePageQuery, int firstResult, int maxResults)
//        {
//            throw new NotImplementedException();

//            //		TablePage tablePage = new TablePage();

//            ////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//            ////ORIGINAL LINE: @SuppressWarnings("rawtypes") List tableData = getDbSqlSession().getSqlSession().selectList("selectTableData", tablePageQuery, new org.apache.ibatis.session.RowBounds(firstResult, maxResults));
//            //		IList tableData = DbSqlSession.SqlSession.SelectList("selectTableData", tablePageQuery, new RowBounds(firstResult, maxResults));

//            //		tablePage.TableName = tablePageQuery.TableName;
//            //		tablePage.Total = GetTableCount(tablePageQuery.TableName);
//            //		tablePage.Rows = tableData;
//            //		tablePage.FirstResult = firstResult;

//            //		return tablePage;
//        }

//        public virtual IList<Type> GetEntities(string tableName)
//	  {
//            throw new NotImplementedException();
//		//string databaseTablePrefix = DbSqlSession.DbSqlSessionFactory.DatabaseTablePrefix;
//		//IList<Type> entities = new List<Type>();

//		//IDictionary<Type, string>.KeyCollection entityClasses = PersistentObjectToTableNameMap.Keys;
//		//foreach (Type entityClass in entityClasses)
//		//{
//		//  string entityTableName = PersistentObjectToTableNameMap[entityClass];
//		//  if ((databaseTablePrefix + entityTableName).Equals(tableName))
//		//  {
//		//	entities.Add(entityClass);
//		//  }
//		//}
//		//return entities;
//	  }

//        [Obsolete("ÆúÓÃ",true)]
//	  public virtual string GetTableName(Type entityClass, bool withPrefix)
//	  {
//            throw new NotImplementedException();
//		//string databaseTablePrefix = DbSqlSession.DbSqlSessionFactory.DatabaseTablePrefix;
//		//string tableName = null;

//		//if (entityClass.IsSubclassOf(typeof(IDbEntity)))
//		//{
//		//  tableName = PersistentObjectToTableNameMap[entityClass];
//		//}
//		//else
//		//{
//		//  tableName = ApiTypeToTableNameMap[entityClass];
//		//}
//		//if (withPrefix)
//		//{
//		//  return databaseTablePrefix + tableName;
//		//}
//		//else
//		//{
//		//  return tableName;
//		//}
//	  }

//	  public virtual TableMetaData GetTableMetaData(string tableName)
//	  {
//            throw new NotImplementedException();
//		//TableMetaData result = new TableMetaData();
//		//ResultSet resultSet = null;

//		//try
//		//{
//		//  try
//		//  {
//		//	result.TableName = tableName;
//		//	DatabaseMetaData metaData = DbSqlSession.SqlSession.Connection.MetaData;

//		//	if (DbSqlSessionFactory.Postgres.Equals(DbSqlSession.DbSqlSessionFactory.DatabaseType))
//		//	{
//		//	  tableName = tableName.ToLower();
//		//	}

//		//	resultSet = metaData.getColumns(null, null, tableName, null);
//		//	while (resultSet.next())
//		//	{
//		//	  string name = resultSet.getString("COLUMN_NAME").ToUpper();
//		//	  string type = resultSet.getString("TYPE_NAME").ToUpper();
//		//	  result.AddColumnMetaData(name, type);
//		//	}

//		//  }
//		//  catch (SQLException se)
//		//  {
//		//	throw se;
//		//  }
//		//  finally
//		//  {
//		//	if (resultSet != null)
//		//	{
//		//	  resultSet.close();
//		//	}
//		//  }
//		//}
//		//catch (System.Exception e)
//		//{
//		//  throw Log.RetrieveMetadataException(e);
//		//}

//		//if (result.ColumnNames.Count == 0)
//		//{
//		//  // According to API, when a table doesn't exist, null should be returned
//		//  result = null;
//		//}
//		//return result;
//	  }

//	}

//}