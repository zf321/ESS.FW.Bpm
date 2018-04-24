using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.dc;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractBaseElementBuilder<TE> : AbstractBpmnModelElementBuilder<TE>, IBaseElementBuilder<TE>  where TE : IBaseElement
    {

        public const double Space = 50;

        protected internal AbstractBaseElementBuilder(IBpmnModelInstance modelInstance, TE element) : base(modelInstance, element)
        {
        }

        protected internal virtual T CreateInstance<T>(Type typeClass) where T : IBpmnModelElementInstance
        {
            return modelInstance.NewInstance<T>(typeClass);
        }

        protected internal virtual T CreateInstance<T>(Type typeClass, string identifier) where T : IBaseElement
        {
            T instance = CreateInstance<T>(typeClass);
            if (identifier!= null)
            {
                instance.SetAttributeValue("id", identifier, true);//.Id = identifier;
            }
            return instance;
        }

        protected internal virtual T CreateChild<T>(Type typeClass) where T : IBpmnModelElementInstance
        {
            return CreateChild<T>(element, typeClass);
        }

        protected internal virtual T CreateChild<T>(Type typeClass, string identifier) where T : IBaseElement
        {
            return CreateChild<T>(element, typeClass, identifier);
        }

        protected internal virtual T CreateChild<T>(IBpmnModelElementInstance parent, Type typeClass) where T : IBpmnModelElementInstance
        {
            T instance = CreateInstance<T>(typeClass);
            parent.AddChildElement(instance);
            return instance;
        }

        protected internal virtual T CreateChild<T>(IBpmnModelElementInstance parent, Type typeClass, string identifier) where T : IBaseElement
        {
            T instance = CreateInstance<T>(typeClass, identifier);
            parent.AddChildElement(instance);
            return instance;
        }

        protected internal virtual T CreateSibling<T>(Type typeClass) where T : IBpmnModelElementInstance
        {
            T instance = CreateInstance<T>(typeClass);
            element.ParentElement.AddChildElement(instance);
            return instance;
        }

        protected internal virtual T CreateSibling<T>(Type typeClass, string identifier) where T : IBaseElement
        {
            T instance = CreateInstance<T>(typeClass, identifier);
            element.ParentElement.AddChildElement(instance);
            return instance;
        }

        protected internal virtual T GetCreateSingleChild<T>(Type typeClass) where T : IBpmnModelElementInstance
        {
            return GetCreateSingleChild<T>(element, typeClass);
        }

        protected internal virtual T GetCreateSingleChild<T>(IBpmnModelElementInstance parent, Type typeClass) where T : IBpmnModelElementInstance
        {
            ICollection<T> childrenOfType = (ICollection<T>)parent.GetChildElementsByType(typeClass);
            if (childrenOfType.Count == 0)
            {
                return CreateChild<T>(parent, typeClass);
            }
            else
            {
                if (childrenOfType.Count > 1)
                {
                    throw new BpmnModelException("XmlElement " + parent + " of type " + parent.ElementType.TypeName + " has more than one child element of type " + typeClass.FullName);
                }
                else
                {
                    return childrenOfType.GetEnumerator().Current;
                }
            }
        }

        protected internal virtual T GetCreateSingleExtensionElement<T>(Type typeClass) where T : IBpmnModelElementInstance
        {
            IExtensionElements extensionElements = GetCreateSingleChild<IExtensionElements>(typeof(IExtensionElements));
            return GetCreateSingleChild<T>(extensionElements, typeClass);
        }

        protected internal virtual IMessage FindMessageForName(string messageName)
        {
            IEnumerable<IMessage> messages = modelInstance.GetModelElementsByType<IMessage>(typeof(IMessage));
            foreach (IMessage m in messages)
            {
                if (messageName.Equals(m.Name))
                {
                    // return already existing message for message name
                    return m;
                }
            }

            // create new message for non existing message name
            IDefinitions definitions = modelInstance.Definitions;
            IMessage message = CreateChild<IMessage>(definitions, typeof(IMessage));
            message.Name = messageName;

            return message;
        }

        protected internal virtual IMessageEventDefinition CreateMessageEventDefinition(string messageName)
        {
            IMessage message = FindMessageForName(messageName);
            IMessageEventDefinition messageEventDefinition = CreateInstance<IMessageEventDefinition>(typeof(IMessageEventDefinition));
            messageEventDefinition.Message = message;
            return messageEventDefinition;
        }

        protected internal virtual IMessageEventDefinition CreateEmptyMessageEventDefinition()
        {
            return CreateInstance<IMessageEventDefinition>(typeof(IMessageEventDefinition));
        }

        protected internal virtual ISignal FindSignalForName(string signalName)
        {
            IEnumerable<ISignal> signals = modelInstance.GetModelElementsByType<ISignal>(typeof(ISignal));
            foreach (ISignal s in signals)
            {
                if (signalName.Equals(s.Name))
                {
                    // return already existing signal for signal name
                    return s;
                }
            }

            // create new signal for non existing signal name
            IDefinitions definitions = modelInstance.Definitions;
            ISignal signal = CreateChild<ISignal>(definitions, typeof(ISignal));
            signal.Name = signalName;

            return signal;
        }

        protected internal virtual ISignalEventDefinition CreateSignalEventDefinition(string signalName)
        {
            ISignal signal = FindSignalForName(signalName);
            ISignalEventDefinition signalEventDefinition = CreateInstance<ISignalEventDefinition>(typeof(ISignalEventDefinition));
            signalEventDefinition.Signal = signal;
            return signalEventDefinition;
        }

        protected internal virtual IErrorEventDefinition FindErrorDefinitionForCode(string errorCode)
        {
            IEnumerable<IErrorEventDefinition> definitions = modelInstance.GetModelElementsByType<IErrorEventDefinition>(typeof(IErrorEventDefinition));
            foreach (IErrorEventDefinition definition in definitions)
            {
                IError error = definition.Error;
                if (error != null && error.ErrorCode.Equals(errorCode))
                {
                    return definition;
                }
            }
            return null;
        }

        protected internal virtual IError FindErrorForNameAndCode(string errorCode)
        {
            IEnumerable<IError> errors = modelInstance.GetModelElementsByType< IError>(typeof(IError));
            foreach (IError e in errors)
            {
                if (errorCode.Equals(e.ErrorCode))
                {
                    // return already existing error
                    return e;
                }
            }

            // create new error
            IDefinitions definitions = modelInstance.Definitions;
            IError error = CreateChild<IError>(definitions, typeof(IError));
            error.ErrorCode = errorCode;

            return error;
        }

        protected internal virtual IErrorEventDefinition CreateEmptyErrorEventDefinition()
        {
            IErrorEventDefinition errorEventDefinition = CreateInstance<IErrorEventDefinition>(typeof(IErrorEventDefinition));
            return errorEventDefinition;
        }

        protected internal virtual IErrorEventDefinition CreateErrorEventDefinition(string errorCode)
        {
            IError error = FindErrorForNameAndCode(errorCode);
            IErrorEventDefinition errorEventDefinition = CreateInstance<IErrorEventDefinition>(typeof(IErrorEventDefinition));
            errorEventDefinition.Error = error;
            return errorEventDefinition;
        }

        protected internal virtual IEscalation FindEscalationForCode(string escalationCode)
        {
            IEnumerable<IEscalation> escalations = modelInstance.GetModelElementsByType< IEscalation>(typeof(IEscalation));
            foreach (IEscalation e in escalations)
            {
                if (escalationCode.Equals(e.EscalationCode))
                {
                    // return already existing escalation
                    return e;
                }
            }

            IDefinitions definitions = modelInstance.Definitions;
            IEscalation escalation = CreateChild<IEscalation>(definitions, typeof(IEscalation));
            escalation.EscalationCode = escalationCode;
            return escalation;
        }

        protected internal virtual IEscalationEventDefinition CreateEscalationEventDefinition(string escalationCode)
        {
            IEscalation escalation = FindEscalationForCode(escalationCode);
            IEscalationEventDefinition escalationEventDefinition = CreateInstance<IEscalationEventDefinition>(typeof(IEscalationEventDefinition));
            escalationEventDefinition.Escalation = escalation;
            return escalationEventDefinition;
        }

        protected internal virtual ICompensateEventDefinition CreateCompensateEventDefinition()
        {
            ICompensateEventDefinition compensateEventDefinition = CreateInstance<ICompensateEventDefinition>(typeof(ICompensateEventDefinition));
            return compensateEventDefinition;
        }


        /// <summary>
        /// Sets the identifier of the element.
        /// </summary>
        /// <param name="identifier">  the identifier to set </param>
        /// <returns> the builder object </returns>
        public virtual TOut Id<TOut>(string identifier) where TOut: IBaseElementBuilder<TE>
        {
            element.Id = identifier;            
            element.SetAttributeValue("id", identifier, true);
            return (TOut)(IBaseElementBuilder<TE>)this;
        }

        /// <summary>
        /// Add an extension element to the element.
        /// </summary>
        /// <param name="extensionElement">  the extension element to add </param>
        /// <returns> the builder object </returns>
        public virtual IBaseElementBuilder<TE> AddExtensionElement(IBpmnModelElementInstance extensionElement)
        {
            IExtensionElements extensionElements = GetCreateSingleChild<IExtensionElements>(typeof(IExtensionElements));
            extensionElements.AddChildElement(extensionElement);
            return this;
        }

        public virtual IBpmnShape CreateBpmnShape(IFlowNode node)
        {
            IBpmnPlane bpmnPlane = FindBpmnPlane();
            if (bpmnPlane != null)
            {
                IBpmnShape bpmnShape = CreateInstance<IBpmnShape>(typeof(IBpmnShape));
                bpmnShape.BpmnElement = node;
                IBounds nodeBounds = CreateInstance<IBounds>(typeof(IBounds));

                if (node is ISubProcess)
                {
                    bpmnShape.Expanded = true;
                    nodeBounds.SetWidth(350);
                    nodeBounds.SetHeight(200);
                }
                else if (node is IActivity)
                {
                    nodeBounds.SetWidth(100);
                    nodeBounds.SetHeight(80);
                }
                else if (node is IEvent)
                {
                    nodeBounds.SetWidth(36);
                    nodeBounds.SetHeight(36);
                }
                else if (node is IGateway)
                {
                    nodeBounds.SetWidth(50);
                    nodeBounds.SetHeight(50);
                }

                nodeBounds.SetX(0);
                nodeBounds.SetY(0);

                bpmnShape.AddChildElement(nodeBounds);
                bpmnPlane.AddChildElement(bpmnShape);

                return bpmnShape;
            }
            return null;
        }

        protected internal virtual IBpmnShape Coordinates
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
                    x = sourceX + sourceWidth + Space;

                    if (element is IFlowNode)
                    {

                        IFlowNode flowNode = (IFlowNode)element;
                        ICollection<ISequenceFlow> outgoing = flowNode.Outgoing;

                        if (outgoing.Count == 0)
                        {
                            double sourceY = sourceBounds.GetY().Value;
                            double sourceHeight = sourceBounds.GetHeight().Value;
                            double targetHeight = shapeBounds.GetHeight().Value;
                            y = sourceY + sourceHeight / 2 - targetHeight / 2;
                        }
                        else
                        {
                            ISequenceFlow[] sequenceFlows = outgoing.ToArray();
                            ISequenceFlow last = sequenceFlows[outgoing.Count - 1];

                            IBpmnShape targetShape = FindBpmnShape(last.Target);
                            if (targetShape != null)
                            {
                                IBounds targetBounds = targetShape.Bounds;
                                double lastY = targetBounds.GetY().Value;
                                double lastHeight = targetBounds.GetHeight().Value;
                                y = lastY + lastHeight + Space;
                            }

                        }
                    }
                }

                shapeBounds.SetX(x);
                shapeBounds.SetY(y);
            }
        }

        public virtual IBpmnEdge CreateBpmnEdge(ISequenceFlow sequenceFlow)
        {
            IBpmnPlane bpmnPlane = FindBpmnPlane();
            if (bpmnPlane != null)
            {


                IBpmnEdge edge = CreateInstance<IBpmnEdge>(typeof(IBpmnEdge));
                edge.BpmnElement = sequenceFlow;
                Waypoints = edge;

                bpmnPlane.AddChildElement(edge);
                return edge;
            }
            return null;

        }

        protected internal virtual IBpmnEdge Waypoints
        {
            set
            {
                ISequenceFlow sequenceFlow = (ISequenceFlow)value.BpmnElement;

                IFlowNode sequenceFlowSource = sequenceFlow.Source;
                IFlowNode sequenceFlowTarget = sequenceFlow.Target;

                IBpmnShape source = FindBpmnShape(sequenceFlowSource);
                IBpmnShape target = FindBpmnShape(sequenceFlowTarget);

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

                    if (sequenceFlowSource.Outgoing.Count == 1)
                    {
                        w1.X = sourceX + sourceWidth;
                        w1.Y = sourceY + sourceHeight / 2;

                        value.AddChildElement(w1);
                    }
                    else
                    {
                        w1.X = sourceX + sourceWidth / 2;
                        w1.Y = sourceY + sourceHeight;

                        value.AddChildElement(w1);

                        IWaypoint w2 = CreateInstance<IWaypoint>(typeof(IWaypoint));
                        w2.X = sourceX + sourceWidth / 2;
                        w2.Y = targetY + targetHeight / 2;

                        value.AddChildElement(w2);
                    }

                    IWaypoint w3 = CreateInstance<IWaypoint>(typeof(IWaypoint));
                    w3.X = targetX;
                    w3.Y = targetY + targetHeight / 2;

                    value.AddChildElement(w3);
                }
            }
        }

        protected internal virtual IBpmnPlane FindBpmnPlane()
        {
            IEnumerable<IBpmnPlane> planes = modelInstance.GetModelElementsByType< IBpmnPlane>(typeof(IBpmnPlane));
            return planes.FirstOrDefault();
        }

        protected internal virtual IBpmnShape FindBpmnShape(IBaseElement node)
        {
            IEnumerable<IBpmnShape> allShapes = modelInstance.GetModelElementsByType< IBpmnShape>(typeof(IBpmnShape));

            IEnumerator<IBpmnShape> iterator = allShapes.GetEnumerator();
            while (iterator.MoveNext())
            {
                IBpmnShape shape = iterator.Current;
                if (shape.BpmnElement.Equals(node))
                {
                    return shape;
                }
            }
            return null;
        }

        protected internal virtual IBpmnEdge FindBpmnEdge(IBaseElement sequenceFlow)
        {
            IEnumerable<IBpmnEdge> allEdges = modelInstance.GetModelElementsByType<IBpmnEdge>(typeof(IBpmnEdge));
            IEnumerator<IBpmnEdge> iterator = allEdges.GetEnumerator();

            while (iterator.MoveNext())
            {
                IBpmnEdge edge = iterator.Current;
                if (edge.BpmnElement.Equals(sequenceFlow))
                {
                    return edge;
                }
            }
            return null;
        }

        protected internal virtual void ResizeSubProcess(IBpmnShape innerShape)
        {

            IBaseElement innerElement = innerShape.BpmnElement;
            IBounds innerShapeBounds = innerShape.Bounds;

            IModelElementInstance parent = innerElement.ParentElement;

            while (parent is ISubProcess)
            {

                IBpmnShape subProcessShape = FindBpmnShape((ISubProcess)parent);

                if (subProcessShape != null)
                {

                    IBounds subProcessBounds = subProcessShape.Bounds;
                    double innerX = innerShapeBounds.GetX().Value;
                    double innerWidth = innerShapeBounds.GetWidth().Value;
                    double innerY = innerShapeBounds.GetY().Value;
                    double innerHeight = innerShapeBounds.GetHeight().Value;

                    double subProcessY = subProcessBounds.GetY().Value;
                    double subProcessHeight = subProcessBounds.GetHeight().Value;
                    double subProcessX = subProcessBounds.GetX().Value;
                    double subProcessWidth = subProcessBounds.GetWidth().Value;

                    double tmpWidth = innerX + innerWidth + Space;
                    double tmpHeight = innerY + innerHeight + Space;

                    if (innerY == subProcessY)
                    {
                        subProcessBounds.SetY(subProcessY - Space);
                        subProcessBounds.SetHeight(subProcessHeight + Space);
                    }

                    if (tmpWidth >= subProcessX + subProcessWidth)
                    {
                        double newWidth = tmpWidth - subProcessX;
                        subProcessBounds.SetWidth(newWidth);
                    }

                    if (tmpHeight >= subProcessY + subProcessHeight)
                    {
                        double newHeight = tmpHeight - subProcessY;
                        subProcessBounds.SetHeight(newHeight);
                    }

                    innerElement = (ISubProcess)parent;
                    innerShapeBounds = subProcessBounds;
                    parent = innerElement.ParentElement;
                }
                else
                {
                    break;
                }
            }
        }
    }

}