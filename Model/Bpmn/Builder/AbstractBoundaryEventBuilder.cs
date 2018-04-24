using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.dc;
using ESS.FW.Bpm.Model.Bpmn.instance.di;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractBoundaryEventBuilder : AbstractCatchEventBuilder<IBoundaryEvent>
	{

	  protected internal AbstractBoundaryEventBuilder(IBpmnModelInstance modelInstance, IBoundaryEvent element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Set if the boundary event cancels the attached activity.
	  /// </summary>
	  /// <param name="cancelActivity"> true if the boundary event cancels the activiy, false otherwise </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBoundaryEventBuilder CancelActivity(bool? cancelActivity)
	  {
		element.CancelActivity = (bool) cancelActivity;

		return this;
	  }

	  /// <summary>
	  /// Sets a catch all error definition.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBoundaryEventBuilder Error()
	  {
		IErrorEventDefinition errorEventDefinition = CreateInstance<IErrorEventDefinition>(typeof(IErrorEventDefinition));
		element.EventDefinitions.Add(errorEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Sets an error definition for the given error code. If already an error
	  /// with this code exists it will be used, otherwise a new error is created.
	  /// </summary>
	  /// <param name="errorCode"> the code of the error </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBoundaryEventBuilder Error(string errorCode)
	  {
		IErrorEventDefinition errorEventDefinition = CreateErrorEventDefinition(errorCode);
		element.EventDefinitions.Add(errorEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Creates an error event definition with an unique id
	  /// and returns a builder for the error event definition.
	  /// </summary>
	  /// <returns> the error event definition builder object </returns>
	  public virtual ErrorEventDefinitionBuilder errorEventDefinition(string id)
	  {
		IErrorEventDefinition errorEventDefinition = CreateEmptyErrorEventDefinition();
		if (!string.ReferenceEquals(id, null))
		{
		  errorEventDefinition.Id = id;
		}

		element.EventDefinitions.Add(errorEventDefinition);
		return new ErrorEventDefinitionBuilder(modelInstance, errorEventDefinition);
	  }

	  /// <summary>
	  /// Creates an error event definition
	  /// and returns a builder for the error event definition.
	  /// </summary>
	  /// <returns> the error event definition builder object </returns>
	  public virtual ErrorEventDefinitionBuilder errorEventDefinition()
	  {
		IErrorEventDefinition errorEventDefinition = CreateEmptyErrorEventDefinition();
		element.EventDefinitions.Add(errorEventDefinition);
		return new ErrorEventDefinitionBuilder(modelInstance, errorEventDefinition);
	  }

	  /// <summary>
	  /// Sets a catch all escalation definition.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBoundaryEventBuilder Escalation()
	  {
		IEscalationEventDefinition escalationEventDefinition = CreateInstance<IEscalationEventDefinition>(typeof(IEscalationEventDefinition));
		element.EventDefinitions.Add(escalationEventDefinition);

		return this;
	  }

	  /// <summary>
	  /// Sets an escalation definition for the given escalation code. If already an escalation
	  /// with this code exists it will be used, otherwise a new escalation is created.
	  /// </summary>
	  /// <param name="escalationCode"> the code of the escalation </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractBoundaryEventBuilder Escalation(string escalationCode)
	  {
		IEscalationEventDefinition escalationEventDefinition = CreateEscalationEventDefinition(escalationCode);
		element.EventDefinitions.Add(escalationEventDefinition);

		return this;
	  }


	  protected internal override IBpmnShape Coordinates
	  {
		  set
		  {
			IBpmnShape source = FindBpmnShape(element);
			IBounds shapeBounds = value.Bounds;
    
			double x = 0;
			double y = 0;
    
			if (source != null)
			{
			  IBounds sourceBounds = source.Bounds;
    
			  double sourceX = sourceBounds.GetX().Value;
			  double sourceWidth = sourceBounds.GetWidth().Value;
			  double sourceY = sourceBounds.GetY().Value;
			  double sourceHeight = sourceBounds.GetHeight().Value;
			  double targetHeight = shapeBounds.GetHeight().Value;
    
			  x = sourceX + sourceWidth + Space;
			  y = sourceY + sourceHeight / 2 - targetHeight / 2 + Space;
			}
    
			shapeBounds.SetX(x);
			shapeBounds.SetY(y);
		  }
	  }

	  protected internal override IBpmnEdge Waypoints
	  {
		  set
		  {
			ISequenceFlow sequenceFlow = (ISequenceFlow) value.BpmnElement;
    
			IFlowNode sourceFlowNode = sequenceFlow.Source;
			IFlowNode targetFlowNode = sequenceFlow.Target;
    
			IBpmnShape source = FindBpmnShape(sourceFlowNode);
			IBpmnShape target = FindBpmnShape(targetFlowNode);
    
			if (source != null && target != null)
			{
			  IBounds sourceBounds = source.Bounds;
			  IBounds targetBounds = target.Bounds;
    
			  double sourceX = sourceBounds.GetX().Value;
			  double sourceY = sourceBounds.GetY().Value;
			  double sourceWidth = sourceBounds.GetWidth().Value;
			  double sourceHeight = sourceBounds.GetHeight().Value;
    
			  double targetX = targetBounds.GetX().Value;
			  double targetY = targetBounds.GetY().Value;
			  double targetHeight = targetBounds.GetHeight().Value;
    
			  IWaypoint w1 = CreateInstance<IWaypoint>(typeof(IWaypoint));
			  w1.X = sourceX + sourceWidth / 2;
			  w1.Y = sourceY + sourceHeight;
    
			  IWaypoint w2 = CreateInstance<IWaypoint>(typeof(IWaypoint));
			  w2.X = sourceX + sourceWidth / 2;
			  w2.Y = sourceY + sourceHeight / 2 + Space;
    
			  IWaypoint w3 = CreateInstance<IWaypoint>(typeof(IWaypoint));
			  w3.X = targetX;
			  w3.Y = targetY + targetHeight / 2;
    
			  value.AddChildElement(w1);
			  value.AddChildElement(w2);
			  value.AddChildElement(w3);
			}
		  }
	  }
	}

}