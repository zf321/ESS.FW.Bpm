//using System;
//using JZTERP.Common.Shared.Entities.Bpm.Entities;

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

//namespace org.camunda.bpm.engine.impl.history.producer
//{

//	using Context = org.camunda.bpm.engine.impl.context.Context;
//	using HistoricCaseActivityInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricCaseActivityInstanceEventEntity;
//	using HistoricCaseInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricCaseInstanceEventEntity;
//	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;

//	/// <summary>
//	/// 
//	/// </summary>
//	public class CacheAwareCmmnHistoryEventProducer : DefaultCmmnHistoryEventProducer
//	{

//	  protected internal override HistoricCaseInstanceEventEntity loadCaseInstanceEventEntity(CaseExecutionEntity caseExecutionEntity)
//	  {
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final String caseInstanceId = caseExecutionEntity.getCaseInstanceId();
//		string caseInstanceId = caseExecutionEntity.CaseInstanceId;

//		HistoricCaseInstanceEventEntity cachedEntity = findInCache< HistoricCaseInstanceEventEntity>(typeof(HistoricCaseInstanceEventEntity), caseInstanceId);

//		if (cachedEntity != null)
//		{
//		  return cachedEntity;
//		}
//		else
//		{
//		  return newCaseInstanceEventEntity(caseExecutionEntity);
//		}

//	  }

//	  protected internal override HistoricCaseActivityInstanceEventEntity loadCaseActivityInstanceEventEntity(CaseExecutionEntity caseExecutionEntity)
//	  {
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final String caseActivityInstanceId = caseExecutionEntity.getId();
//		string caseActivityInstanceId = caseExecutionEntity.Id;

//		HistoricCaseActivityInstanceEventEntity cachedEntity = findInCache(typeof(HistoricCaseActivityInstanceEventEntity), caseActivityInstanceId);

//		if (cachedEntity != null)
//		{
//		  return cachedEntity;
//		}
//		else
//		{
//		  return newCaseActivityInstanceEventEntity(caseExecutionEntity);
//		}

//	  }

//	  /// <summary>
//	  /// find a cached entity by primary key </summary>
//	  protected internal virtual T findInCache<T>(Type  type, string id) where T : org.camunda.bpm.engine.impl.history.@event.HistoryEvent
//	  {
//		return Context.CommandContext.DbEntityManager.getCachedEntity(type, id);
//	  }

//	}

//}

