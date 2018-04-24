



using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class VariableInstanceHistoryListener : IVariableInstanceLifecycleListener<ICoreVariableInstance>
	{

	  public static readonly IVariableInstanceLifecycleListener<ICoreVariableInstance> Instance = new VariableInstanceHistoryListener();
        
	  public  void OnCreate(ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {
		if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.VariableInstanceCreate, variableInstance))
		{
		  HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper(this, variableInstance, sourceScope));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClassHelper : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly VariableInstanceHistoryListener _outerInstance;

		  private VariableInstanceEntity _variableInstance;
		  private AbstractVariableScope _sourceScope;

		  public HistoryEventCreatorAnonymousInnerClassHelper(VariableInstanceHistoryListener outerInstance, ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
		  {
			  this._outerInstance = outerInstance;
			  this._variableInstance = (VariableInstanceEntity) variableInstance;
			  this._sourceScope = sourceScope;
		  }

		  public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
		  {
			return producer.CreateHistoricVariableCreateEvt(_variableInstance, _sourceScope);
		  }
	  }
        
	  public  void OnDelete(ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {
		if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.VariableInstanceDelete, variableInstance))
		{
		  HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper2(this, variableInstance, sourceScope));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClassHelper2 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly VariableInstanceHistoryListener _outerInstance;

		  private VariableInstanceEntity _variableInstance;
		  private AbstractVariableScope _sourceScope;

		  public HistoryEventCreatorAnonymousInnerClassHelper2(VariableInstanceHistoryListener outerInstance, ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
		  {
			  this._outerInstance = outerInstance;
			  this._variableInstance = (VariableInstanceEntity) variableInstance;
			  this._sourceScope = sourceScope;
		  }

		  public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
		  {
			return producer.CreateHistoricVariableDeleteEvt(_variableInstance, _sourceScope);
		  }
	  }
        
	  public  void OnUpdate(ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {
		if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.VariableInstanceUpdate, variableInstance))
		{
		  HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper3(this, variableInstance, sourceScope));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClassHelper3 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly VariableInstanceHistoryListener _outerInstance;

		  private VariableInstanceEntity _variableInstance;
		  private AbstractVariableScope _sourceScope;

		  public HistoryEventCreatorAnonymousInnerClassHelper3(VariableInstanceHistoryListener outerInstance, ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
		  {
			  this._outerInstance = outerInstance;
			  this._variableInstance = (VariableInstanceEntity) variableInstance;
			  this._sourceScope = sourceScope;
		  }

		  public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
		  {
			return producer.CreateHistoricVariableUpdateEvt(_variableInstance, _sourceScope);
		  }
	  }

	  protected internal virtual IHistoryLevel HistoryLevel
	  {
		  get
		  {
			return context.Impl.Context.ProcessEngineConfiguration.HistoryLevel;
		  }
	  }
	}

}