using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.builder;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using MultiInstanceLoopCharacteristicsBuilder = MultiInstanceLoopCharacteristicsBuilder;

	/// <summary>
	/// The BPMN 2.0 multiInstanceLoopCharacteristics element type
	/// 
	/// @author Filip Hrisafov
	/// 
	/// </summary>
	public interface IMultiInstanceLoopCharacteristics : ILoopCharacteristics
	{

	  ILoopCardinality LoopCardinality {get;set;}


	  IDataInput LoopDataInputRef {get;set;}


	  IDataOutput LoopDataOutputRef {get;set;}


	  INputDataItem InputDataItem {get;set;}


	  IOutputDataItem OutputDataItem {get;set;}


	  ICollection<IComplexBehaviorDefinition> ComplexBehaviorDefinitions {get;}

	  ICompletionCondition CompletionCondition {get;set;}


	  bool Sequential {get;set;}


	  MultiInstanceFlowCondition Behavior {get;set;}


	  IEventDefinition OneBehaviorEventRef {get;set;}


	  IEventDefinition NoneBehaviorEventRef {get;set;}


	  string CamundaCollection {get;set;}


	  string CamundaElementVariable {get;set;}


	  bool CamundaAsyncBefore {get;set;}


	  bool CamundaAsyncAfter {get;set;}


	  bool CamundaExclusive {get;set;}


	  MultiInstanceLoopCharacteristicsBuilder Builder();

	}

}