using System;


namespace ESS.FW.Bpm.Model.Bpmn.impl
{

    /// <summary>
    /// Constants used in the BPMN 2.0 Language (DI + Semantic)
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    public sealed class BpmnModelConstants
    {

        /// <summary>
        /// The XSI namespace </summary>
        public const string XsiNs = "http://www.w3.org/2001/XMLSchema-instance";

        /// <summary>
        /// The BPMN 2.0 namespace </summary>
        public const string Bpmn20Ns = "http://www.omg.org/spec/BPMN/20100524/MODEL";

        /// <summary>
        /// The BPMNDI namespace </summary>
        public const string BpmndiNs = "http://www.omg.org/spec/BPMN/20100524/DI";

        /// <summary>
        /// The DC namespace </summary>
        public const string DcNs = "http://www.omg.org/spec/DD/20100524/DC";

        /// <summary>
        /// The DI namespace </summary>
        public const string DiNs = "http://www.omg.org/spec/DD/20100524/DI";

        /// <summary>
        /// The location of the BPMN 2.0 XML schema. </summary>
        public const string Bpmn20SchemaLocation = "BPMN20.xsd";

        /// <summary>
        /// Xml Schema is the default type language </summary>
        public const string XmlSchemaNs = "http://www.w3.org/2001/XMLSchema";

        public const string XpathNs = "http://www.w3.org/1999/XPath";

        /// @deprecated use <seealso cref="#CAMUNDA_NS"/> 
        [Obsolete("use <seealso cref=\"#CAMUNDA_NS\"/>")]
        public const string ActivitiNs = "http://activiti.org/bpmn";

        /// <summary>
        /// CAMUNDA_NS namespace </summary>
        public const string CamundaNs = "http://camunda.org/schema/1.0/bpmn";

        // elements ////////////////////////////////////////

        public const string BpmnElementBaseElement = "baseElement";
        public const string BpmnElementDefinitions = "definitions";
        public const string BpmnElementDocumentation = "documentation";
        public const string BpmnElementExtension = "extension";
        public const string BpmnElementExtensionElements = "extensionElements";
        public const string BpmnElementImport = "import";
        public const string BpmnElementRelationship = "relationship";
        public const string BpmnElementSource = "source";
        public const string BpmnElementTarget = "target";
        public const string BpmnElementRootElement = "rootElement";
        public const string BpmnElementAuditing = "auditing";
        public const string BpmnElementMonitoring = "monitoring";
        public const string BpmnElementCategoryValue = "categoryValue";
        public const string BpmnElementFlowElement = "flowElement";
        public const string BpmnElementFlowNode = "flowNode";
        public const string BpmnElementCategoryValueRef = "categoryValueRef";
        public const string BpmnElementExpression = "expression";
        public const string BpmnElementConditionExpression = "conditionExpression";
        public const string BpmnElementSequenceFlow = "sequenceFlow";
        public const string BpmnElementIncoming = "incoming";
        public const string BpmnElementOutgoing = "outgoing";
        public const string BpmnElementDataState = "dataState";
        public const string BpmnElementItemDefinition = "itemDefinition";
        public const string BpmnElementError = "error";
        public const string BpmnElementInMessageRef = "inMessageRef";
        public const string BpmnElementOutMessageRef = "outMessageRef";
        public const string BpmnElementErrorRef = "errorRef";
        public const string BpmnElementOperation = "operation";
        public const string BpmnElementImplementationRef = "implementationRef";
        public const string BpmnElementOperationRef = "operationRef";
        public const string BpmnElementDataOutput = "dataOutput";
        public const string BpmnElementFrom = "from";
        public const string BpmnElementTo = "to";
        public const string BpmnElementAssignment = "assignment";
        public const string BpmnElementItemAwareElement = "itemAwareElement";
        public const string BpmnElementDataObject = "dataObject";
        public const string BpmnElementDataObjectReference = "dataObjectReference";
        public const string BpmnElementDataStore = "dataStore";
        public const string BpmnElementDataInput = "dataInput";
        public const string BpmnElementFormalExpression = "formalExpression";
        public const string BpmnElementDataAssociation = "dataAssociation";
        public const string BpmnElementSourceRef = "sourceRef";
        public const string BpmnElementTargetRef = "targetRef";
        public const string BpmnElementTransformation = "transformation";
        public const string BpmnElementDataInputAssociation = "dataInputAssociation";
        public const string BpmnElementDataOutputAssociation = "dataOutputAssociation";
        public const string BpmnElementInputSet = "inputSet";
        public const string BpmnElementOutputSet = "outputSet";
        public const string BpmnElementDataInputRefs = "dataInputRefs";
        public const string BpmnElementOptionalInputRefs = "optionalInputRefs";
        public const string BpmnElementWhileExecutingInputRefs = "whileExecutingInputRefs";
        public const string BpmnElementOutputSetRefs = "outputSetRefs";
        public const string BpmnElementDataOutputRefs = "dataOutputRefs";
        public const string BpmnElementOptionalOutputRefs = "optionalOutputRefs";
        public const string BpmnElementWhileExecutingOutputRefs = "whileExecutingOutputRefs";
        public const string BpmnElementInputSetRefs = "inputSetRefs";
        public const string BpmnElementCatchEvent = "catchEvent";
        public const string BpmnElementThrowEvent = "throwEvent";
        public const string BpmnElementEndEvent = "endEvent";
        public const string BpmnElementIoSpecification = "ioSpecification";
        public const string BpmnElementLoopCharacteristics = "loopCharacteristics";
        public const string BpmnElementResourceParameter = "resourceParameter";
        public const string BpmnElementResource = "resource";
        public const string BpmnElementResourceParameterBinding = "resourceParameterBinding";
        public const string BpmnElementResourceAssignmentExpression = "resourceAssignmentExpression";
        public const string BpmnElementResourceRole = "resourceRole";
        public const string BpmnElementResourceRef = "resourceRef";
        public const string BpmnElementPerformer = "performer";
        public const string BpmnElementHumanPerformer = "humanPerformer";
        public const string BpmnElementPotentialOwner = "potentialOwner";
        public const string BpmnElementActivity = "activity";
        public const string BpmnElementIoBinding = "ioBinding";
        public const string BpmnElementInterface = "interface";
        public const string BpmnElementEvent = "event";
        public const string BpmnElementMessage = "message";
        public const string BpmnElementStartEvent = "startEvent";
        public const string BpmnElementProperty = "property";
        public const string BpmnElementEventDefinition = "eventDefinition";
        public const string BpmnElementEventDefinitionRef = "eventDefinitionRef";
        public const string BpmnElementMessageEventDefinition = "messageEventDefinition";
        public const string BpmnElementCancelEventDefinition = "cancelEventDefinition";
        public const string BpmnElementCompensateEventDefinition = "compensateEventDefinition";
        public const string BpmnElementConditionalEventDefinition = "conditionalEventDefinition";
        public const string BpmnElementCondition = "condition";
        public const string BpmnElementErrorEventDefinition = "errorEventDefinition";
        public const string BpmnElementLinkEventDefinition = "linkEventDefinition";
        public const string BpmnElementSignalEventDefinition = "signalEventDefinition";
        public const string BpmnElementTerminateEventDefinition = "terminateEventDefinition";
        public const string BpmnElementTimerEventDefinition = "timerEventDefinition";
        public const string BpmnElementSupportedInterfaceRef = "supportedInterfaceRef";
        public const string BpmnElementCallableElement = "callableElement";
        public const string BpmnElementPartitionElement = "partitionElement";
        public const string BpmnElementFlowNodeRef = "flowNodeRef";
        public const string BpmnElementChildLaneSet = "childLaneSet";
        public const string BpmnElementLaneSet = "laneSet";
        public const string BpmnElementLane = "lane";
        public const string BpmnElementArtifact = "artifact";
        public const string BpmnElementCorrelationPropertyRetrievalExpression = "correlationPropertyRetrievalExpression";
        public const string BpmnElementMessagePath = "messagePath";
        public const string BpmnElementDataPath = "dataPath";
        public const string BpmnElementCallActivity = "callActivity";
        public const string BpmnElementCorrelationPropertyBinding = "correlationPropertyBinding";
        public const string BpmnElementCorrelationProperty = "correlationProperty";
        public const string BpmnElementCorrelationPropertyRef = "correlationPropertyRef";
        public const string BpmnElementCorrelationKey = "correlationKey";
        public const string BpmnElementCorrelationSubscription = "correlationSubscription";
        public const string BpmnElementSupports = "supports";
        public const string BpmnElementProcess = "process";
        public const string BpmnElementTask = "task";
        public const string BpmnElementSendTask = "sendTask";
        public const string BpmnElementServiceTask = "serviceTask";
        public const string BpmnElementScriptTask = "scriptTask";
        public const string BpmnElementUserTask = "userTask";
        public const string BpmnElementReceiveTask = "receiveTask";
        public const string BpmnElementBusinessRuleTask = "businessRuleTask";
        public const string BpmnElementManualTask = "manualTask";
        public const string BpmnElementScript = "script";
        public const string BpmnElementRendering = "rendering";
        public const string BpmnElementBoundaryEvent = "boundaryEvent";
        public const string BpmnElementSubProcess = "subProcess";
        public const string BpmnElementTransaction = "transaction";
        public const string BpmnElementGateway = "gateway";
        public const string BpmnElementParallelGateway = "parallelGateway";
        public const string BpmnElementExclusiveGateway = "exclusiveGateway";
        public const string BpmnElementIntermediateCatchEvent = "intermediateCatchEvent";
        public const string BpmnElementIntermediateThrowEvent = "intermediateThrowEvent";
        public const string BpmnElementEndPoint = "endPoint";
        public const string BpmnElementParticipantMultiplicity = "participantMultiplicity";
        public const string BpmnElementParticipant = "participant";
        public const string BpmnElementParticipantRef = "participantRef";
        public const string BpmnElementInterfaceRef = "interfaceRef";
        public const string BpmnElementEndPointRef = "endPointRef";
        public const string BpmnElementMessageFlow = "messageFlow";
        public const string BpmnElementMessageFlowRef = "messageFlowRef";
        public const string BpmnElementConversationNode = "conversationNode";
        public const string BpmnElementConversation = "conversation";
        public const string BpmnElementSubConversation = "subConversation";
        public const string BpmnElementGlobalConversation = "globalConversation";
        public const string BpmnElementCallConversation = "callConversation";
        public const string BpmnElementParticipantAssociation = "participantAssociation";
        public const string BpmnElementInnerParticipantRef = "innerParticipantRef";
        public const string BpmnElementOuterParticipantRef = "outerParticipantRef";
        public const string BpmnElementConversationAssociation = "conversationAssociation";
        public const string BpmnElementMessageFlowAssociation = "messageFlowAssociation";
        public const string BpmnElementConversationLink = "conversationLink";
        public const string BpmnElementCollaboration = "collaboration";
        public const string BpmnElementAssociation = "association";
        public const string BpmnElementSignal = "signal";
        public const string BpmnElementTimeDate = "timeDate";
        public const string BpmnElementTimeDuration = "timeDuration";
        public const string BpmnElementTimeCycle = "timeCycle";
        public const string BpmnElementEscalation = "escalation";
        public const string BpmnElementEscalationEventDefinition = "escalationEventDefinition";
        public const string BpmnElementActivationCondition = "activationCondition";
        public const string BpmnElementComplexGateway = "complexGateway";
        public const string BpmnElementEventBasedGateway = "eventBasedGateway";
        public const string BpmnElementInclusiveGateway = "inclusiveGateway";
        public const string BpmnElementTextAnnotation = "textAnnotation";
        public const string BpmnElementText = "text";
        public const string BpmnElementComplexBehaviorDefinition = "complexBehaviorDefinition";
        public const string BpmnElementMultiInstanceLoopCharacteristics = "multiInstanceLoopCharacteristics";
        public const string BpmnElementLoopCardinality = "loopCardinality";
        public const string BpmnElementCompletionCondition = "completionCondition";
        public const string BpmnElementOutputDataItem = "outputDataItem";
        public const string BpmnElementInputDataItem = "inputDataItem";
        public const string BpmnElementLoopDataOutputRef = "loopDataOutputRef";
        public const string BpmnElementLoopDataInputRef = "loopDataInputRef";
        public const string BpmnElementIsSequential = "isSequential";
        public const string BpmnElementBehavior = "behavior";
        public const string BpmnElementOneBehaviorEventRef = "oneBehaviorEventRef";
        public const string BpmnElementNoneBehaviorEventRef = "noneBehaviorEventRef";

        /// <summary>
        /// DC </summary>

        public const string DcElementFont = "Font";
        public const string DcElementPoint = "Point";
        public const string DcElementBounds = "Bounds";

        /// <summary>
        /// DI </summary>

        public const string DiElementDiagramElement = "DiagramElement";
        public const string DiElementDiagram = "Diagram";
        public const string DiElementEdge = "Edge";
        public const string DiElementExtension = "extension";
        public const string DiElementLabeledEdge = "LabeledEdge";
        public const string DiElementLabel = "Label";
        public const string DiElementLabeledShape = "LabeledShape";
        public const string DiElementNode = "Node";
        public const string DiElementPlane = "Plane";
        public const string DiElementShape = "Shape";
        public const string DiElementStyle = "Style";
        public const string DiElementWaypoint = "waypoint";

        /// <summary>
        /// BPMNDI </summary>

        public const string BpmndiElementBpmnDiagram = "BPMNDiagram";
        public const string BpmndiElementBpmnPlane = "BPMNPlane";
        public const string BpmndiElementBpmnLabelStyle = "BPMNLabelStyle";
        public const string BpmndiElementBpmnShape = "BPMNShape";
        public const string BpmndiElementBpmnLabel = "BPMNLabel";
        public const string BpmndiElementBpmnEdge = "BPMNEdge";

        /// <summary>
        /// camunda extensions </summary>

        public const string CamundaElementConnector = "connector";
        public const string CamundaElementConnectorId = "connectorId";
        public const string CamundaElementConstraint = "constraint";
        public const string CamundaElementEntry = "entry";
        public const string CamundaElementExecutionListener = "executionListener";
        public const string CamundaElementExpression = "expression";
        public const string CamundaElementFailedJobRetryTimeCycle = "failedJobRetryTimeCycle";
        public const string CamundaElementField = "field";
        public const string CamundaElementFormData = "formData";
        public const string CamundaElementFormField = "formField";
        public const string CamundaElementFormProperty = "formProperty";
        public const string CamundaElementIn = "in";
        public const string CamundaElementInputOutput = "inputOutput";
        public const string CamundaElementInputParameter = "inputParameter";
        public const string CamundaElementList = "list";
        public const string CamundaElementMap = "map";
        public const string CamundaElementOutputParameter = "outputParameter";
        public const string CamundaElementOut = "out";
        public const string CamundaElementPotentialStarter = "potentialStarter";
        public const string CamundaElementProperties = "properties";
        public const string CamundaElementProperty = "property";
        public const string CamundaElementScript = "script";
        public const string CamundaElementString = "string";
        public const string CamundaElementTaskListener = "taskListener";
        public const string CamundaElementValidation = "validation";
        public const string CamundaElementValue = "value";

        // attributes //////////////////////////////////////

        /// <summary>
        /// XSI attributes * </summary>

        public const string XsiAttributeType = "type";

        /// <summary>
        /// BPMN attributes * </summary>

        public const string BpmnAttributeExporter = "exporter";
        public const string BpmnAttributeExporterVersion = "exporterVersion";
        public const string BpmnAttributeExpressionLanguage = "expressionLanguage";
        public const string BpmnAttributeId = "id";
        public const string BpmnAttributeName = "name";
        public const string BpmnAttributeTargetNamespace = "targetNamespace";
        public const string BpmnAttributeTypeLanguage = "typeLanguage";
        public const string BpmnAttributeNamespace = "namespace";
        public const string BpmnAttributeLocation = "location";
        public const string BpmnAttributeImportType = "importType";
        public const string BpmnAttributeTextFormat = "textFormat";
        public const string BpmnAttributeProcessType = "processType";
        public const string BpmnAttributeIsClosed = "isClosed";
        public const string BpmnAttributeIsExecutable = "isExecutable";
        public const string BpmnAttributeMessageRef = "messageRef";
        public const string BpmnAttributeDefinition = "definition";
        public const string BpmnAttributeMustUnderstand = "mustUnderstand";
        public const string BpmnAttributeType = "type";
        public const string BpmnAttributeDirection = "direction";
        public const string BpmnAttributeSourceRef = "sourceRef";
        public const string BpmnAttributeTargetRef = "targetRef";
        public const string BpmnAttributeIsImmediate = "isImmediate";
        public const string BpmnAttributeValue = "value";
        public const string BpmnAttributeStructureRef = "structureRef";
        public const string BpmnAttributeIsCollection = "isCollection";
        public const string BpmnAttributeItemKind = "itemKind";
        public const string BpmnAttributeItemRef = "itemRef";
        public const string BpmnAttributeItemSubjectRef = "itemSubjectRef";
        public const string BpmnAttributeErrorCode = "errorCode";
        public const string BpmnAttributeLanguage = "language";
        public const string BpmnAttributeEvaluatesToTypeRef = "evaluatesToTypeRef";
        public const string BpmnAttributeParallelMultiple = "parallelMultiple";
        public const string BpmnAttributeIsInterrupting = "isInterrupting";
        public const string BpmnAttributeIsRequired = "isRequired";
        public const string BpmnAttributeParameterRef = "parameterRef";
        public const string BpmnAttributeIsForCompensation = "isForCompensation";
        public const string BpmnAttributeStartQuantity = "startQuantity";
        public const string BpmnAttributeCompletionQuantity = "completionQuantity";
        public const string BpmnAttributeDefault = "default";
        public const string BpmnAttributeOperationRef = "operationRef";
        public const string BpmnAttributeInputDataRef = "inputDataRef";
        public const string BpmnAttributeOutputDataRef = "outputDataRef";
        public const string BpmnAttributeImplementationRef = "implementationRef";
        public const string BpmnAttributePartitionElementRef = "partitionElementRef";
        public const string BpmnAttributeCorrelationPropertyRef = "correlationPropertyRef";
        public const string BpmnAttributeCorrelationKeyRef = "correlationKeyRef";
        public const string BpmnAttributeImplementation = "implementation";
        public const string BpmnAttributeScriptFormat = "scriptFormat";
        public const string BpmnAttributeInstantiate = "instantiate";
        public const string BpmnAttributeCancelActivity = "cancelActivity";
        public const string BpmnAttributeAttachedToRef = "attachedToRef";
        public const string BpmnAttributeTriggeredByEvent = "triggeredByEvent";
        public const string BpmnAttributeGatewayDirection = "gatewayDirection";
        public const string BpmnAttributeCalledElement = "calledElement";
        public const string BpmnAttributeMinimum = "minimum";
        public const string BpmnAttributeMaximum = "maximum";
        public const string BpmnAttributeProcessRef = "processRef";
        public const string BpmnAttributeCalledCollaborationRef = "calledCollaborationRef";
        public const string BpmnAttributeInnerConversationNodeRef = "innerConversationNodeRef";
        public const string BpmnAttributeOuterConversationNodeRef = "outerConversationNodeRef";
        public const string BpmnAttributeInnerMessageFlowRef = "innerMessageFlowRef";
        public const string BpmnAttributeOuterMessageFlowRef = "outerMessageFlowRef";
        public const string BpmnAttributeAssociationDirection = "associationDirection";
        public const string BpmnAttributeWaitForCompletion = "waitForCompletion";
        public const string BpmnAttributeActivityRef = "activityRef";
        public const string BpmnAttributeErrorRef = "errorRef";
        public const string BpmnAttributeSignalRef = "signalRef";
        public const string BpmnAttributeEscalationCode = "escalationCode";
        public const string BpmnAttributeEscalationRef = "escalationRef";
        public const string BpmnAttributeEventGatewayType = "eventGatewayType";
        public const string BpmnAttributeDataObjectRef = "dataObjectRef";
        public const string BpmnAttributeMethod = "method";
        public const string BpmnAttributeCapacity = "capacity";
        public const string BpmnAttributeIsUnlimited = "isUnlimited";

        /// <summary>
        /// DC </summary>

        public const string DcAttributeName = "name";
        public const string DcAttributeSize = "size";
        public const string DcAttributeIsBold = "isBold";
        public const string DcAttributeIsItalic = "isItalic";
        public const string DcAttributeIsUnderline = "isUnderline";
        public const string DcAttributeIsStrikeThrough = "isStrikeThrough";
        public const string DcAttributeX = "x";
        public const string DcAttributeY = "y";
        public const string DcAttributeWidth = "width";
        public const string DcAttributeHeight = "height";

        /// <summary>
        /// DI </summary>

        public const string DiAttributeId = "id";
        public const string DiAttributeName = "name";
        public const string DiAttributeDocumentation = "documentation";
        public const string DiAttributeResolution = "resolution";

        /// <summary>
        /// BPMNDI </summary>

        public const string BpmndiAttributeBpmnElement = "bpmnElement";
        public const string BpmndiAttributeSourceElement = "sourceElement";
        public const string BpmndiAttributeTargetElement = "targetElement";
        public const string BpmndiAttributeMessageVisibleKind = "messageVisibleKind";
        public const string BpmndiAttributeIsHorizontal = "isHorizontal";
        public const string BpmndiAttributeIsExpanded = "isExpanded";
        public const string BpmndiAttributeIsMarkerVisible = "isMarkerVisible";
        public const string BpmndiAttributeIsMessageVisible = "isMessageVisible";
        public const string BpmndiAttributeParticipantBandKind = "participantBandKind";
        public const string BpmndiAttributeChoreographyActivityShape = "choreographyActivityShape";
        public const string BpmndiAttributeLabelStyle = "labelStyle";

        /// <summary>
        /// camunda extensions </summary>

        public const string CamundaAttributeAssignee = "assignee";
        public const string CamundaAttributeAsync = "async";
        public const string CamundaAttributeAsyncBefore = "asyncBefore";
        public const string CamundaAttributeAsyncAfter = "asyncAfter";
        public const string CamundaAttributeBusinessKey = "businessKey";
        public const string CamundaAttributeCalledElementBinding = "calledElementBinding";
        public const string CamundaAttributeCalledElementVersion = "calledElementVersion";
        public const string CamundaAttributeCalledElementTenantId = "calledElementTenantId";
        public const string CamundaAttributeCandidateGroups = "candidateGroups";
        public const string CamundaAttributeCandidateStarterGroups = "candidateStarterGroups";
        public const string CamundaAttributeCandidateStarterUsers = "candidateStarterUsers";
        public const string CamundaAttributeCandidateUsers = "candidateUsers";
        public const string CamundaAttributeClass = "class";
        public const string CamundaAttributeCollection = "collection";
        public const string CamundaAttributeConfig = "config";
        public const string CamundaAttributeDatePattern = "datePattern";
        public const string CamundaAttributeDecisionRef = "decisionRef";
        public const string CamundaAttributeDecisionRefBinding = "decisionRefBinding";
        public const string CamundaAttributeDecisionRefVersion = "decisionRefVersion";
        public const string CamundaAttributeDecisionRefTenantId = "decisionRefTenantId";
        public const string CamundaAttributeDefault = "default";
        public const string CamundaAttributeDefaultValue = "defaultValue";
        public const string CamundaAttributeDelegateExpression = "delegateExpression";
        public const string CamundaAttributeDueDate = "dueDate";
        public const string CamundaAttributeFollowUpDate = "followUpDate";
        public const string CamundaAttributeElementVariable = "elementVariable";
        public const string CamundaAttributeEvent = "event";
        public const string CamundaAttributeErrorCodeVariable = "errorCodeVariable";
        public const string CamundaAttributeErrorMessageVariable = "errorMessageVariable";
        public const string CamundaAttributeExclusive = "exclusive";
        public const string CamundaAttributeExpression = "expression";
        public const string CamundaAttributeFormHandlerClass = "formHandlerClass";
        public const string CamundaAttributeFormKey = "formKey";
        public const string CamundaAttributeId = "id";
        public const string CamundaAttributeInitiator = "initiator";
        public const string CamundaAttributeJobPriority = "jobPriority";
        public const string CamundaAttributeTaskPriority = "taskPriority";


        public const string CamundaAttributeKey = "key";
        public const string CamundaAttributeLabel = "label";
        public const string CamundaAttributeLocal = "local";
        public const string CamundaAttributeMapDecisionResult = "mapDecisionResult";
        public const string CamundaAttributeName = "name";
        public const string CamundaAttributePriority = "priority";
        public const string CamundaAttributeReadable = "readable";
        public const string CamundaAttributeRequired = "required";
        public const string CamundaAttributeResource = "resource";
        public const string CamundaAttributeResultVariable = "resultVariable";
        public const string CamundaAttributeScriptFormat = "scriptFormat";
        public const string CamundaAttributeSource = "source";
        public const string CamundaAttributeSourceExpression = "sourceExpression";
        public const string CamundaAttributeStringValue = "stringValue";
        public const string CamundaAttributeTarget = "target";
        public const string CamundaAttributeTopic = "topic";
        public const string CamundaAttributeType = "type";
        public const string CamundaAttributeValue = "value";
        public const string CamundaAttributeVariable = "variable";
        public const string CamundaAttributeVariableMappingClass = "variableMappingClass";
        public const string CamundaAttributeVariableMappingDelegateExpression = "variableMappingDelegateExpression";
        public const string CamundaAttributeVariables = "variables";
        public const string CamundaAttributeWriteable = "writeable";
        public const string CamundaAttributeCaseRef = "caseRef";
        public const string CamundaAttributeCaseBinding = "caseBinding";
        public const string CamundaAttributeCaseVersion = "caseVersion";
        public const string CamundaAttributeCaseTenantId = "caseTenantId";
        public const string CamundaAttributeVariableName = "variableName";
        public const string CamundaAttributeVariableEvents = "variableEvents";
        public const string CamundaAttributeHistoryTimeToLive = "historyTimeToLive";
    }

}