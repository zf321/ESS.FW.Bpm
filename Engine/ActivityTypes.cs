namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Class contains constants that identifies the activity types, which are used by Camunda.
    ///     Events, gateways and activities are summed together as activities.
    ///     They typically correspond to the XML tags used in the BPMN 2.0 process definition file.
    ///     
    ///      
    /// </summary>
    public static class ActivityTypes
    {
        public const string MultiInstanceBody = "multiinstancebody";//"multiInstanceBody";

        //gateways //////////////////////////////////////////////

        public const string GatewayExclusive = "exclusivegateway";// "exclusiveGateway";
        public const string GatewayInclusive = "inclusivegateway";//"inclusiveGateway";
        public const string GatewayParallel = "parallelgateway";// "parallelGateway";
        public const string GatewayComplex = "complexgateway";// "complexGateway";
        public const string GatewayEventBased = "eventbasedgateway";//"eventBasedGateway";

        //tasks //////////////////////////////////////////////
        public const string Task = "task";
        public const string TaskScript = "scripttask";//"scriptTask";
        public const string TaskService = "servicetask";//"serviceTask";
        public const string TaskBusinessRule = "businessruletask";//"businessRuleTask";
        public const string TaskManualTask = "manualtask";//"manualTask";
        public const string TaskUserTask = "usertask";//"userTask";
        public const string TaskSendTask = "sendtask";//"sendTask";
        public const string TaskReceiveTask = "receivetask";//"receiveTask";

        //other ////////////////////////////////////////////////
        public const string SubProcess = "subprocess";//"subProcess";
        public const string SubProcessAdHoc = "adhocsubprocess";//"adHocSubProcess";
        public const string CallActivity = "callactivity";//"callActivity";
        public const string Transaction = "transaction";

        //boundary events ////////////////////////////////////////
        public const string BoundaryTimer = "boundarytimer";//"boundaryTimer";
        public const string BoundaryMessage = "boundarymessage";//"boundaryMessage";
        public const string BoundarySignal = "boundarysignal";//"boundarySignal";
        public const string BoundaryCompensation = "compensationboundarycatch";//"compensationBoundaryCatch";
        public const string BoundaryError = "boundaryerror";//"boundaryError";
        public const string BoundaryEscalation = "boundaryescalation";//"boundaryEscalation";
        public const string BoundaryCancel = "cancelboundarycatch";//"cancelBoundaryCatch";
        public const string BoundaryConditional = "boundaryconditional";//"boundaryConditional";

        //start events ////////////////////////////////////////
        public const string StartEvent = "startevent";//"startEvent";
        public const string StartEventTimer = "starttimerevent";// "startTimerEvent";
        public const string StartEventMessage = "messagestartevent";//"messageStartEvent";
        public const string StartEventSignal = "signalstartevent";//"signalStartEvent";
        public const string StartEventEscalation = "escalationstartevent";//"escalationStartEvent";
        public const string StartEventCompensation = "compensationstartevent";// "compensationStartEvent";
        public const string StartEventError = "errorstartevent";//"errorStartEvent";
        public const string StartEventConditional = "conditionalstartevent";//"conditionalStartEvent";

        //intermediate catch events ////////////////////////////////////////
        public const string IntermediateEventCatch = "intermediatecatchevent";//"intermediateCatchEvent";
        public const string IntermediateEventMessage = "intermediatemessagecatch";//"intermediateMessageCatch";
        public const string IntermediateEventTimer = "intermediatetimer";//"intermediateTimer";
        public const string IntermediateEventLink = "intermediatelinkcatch";//"intermediateLinkCatch";
        public const string IntermediateEventSignal = "intermediatesignalcatch";//"intermediateSignalCatch";
        public const string IntermediateEventConditional = "intermediateconditional";//"intermediateConditional";

        //intermediate throw events ////////////////////////////////
        public const string IntermediateEventThrow = "intermediatethrowevent";//"intermediateThrowEvent";
        public const string IntermediateEventSignalThrow = "intermediatesignalthrow";//"intermediateSignalThrow";
        public const string IntermediateEventCompensationThrow = "intermediatecompensationthrowevent";//"intermediateCompensationThrowEvent";
        public const string IntermediateEventMessageThrow = "intermediatemessagethrowevent";//"intermediateMessageThrowEvent";
        public const string IntermediateEventNoneThrow = "intermediatenonethrowevent";//"intermediateNoneThrowEvent";
        public const string IntermediateEventEscalationThrow = "intermediateescalationthrowevent";//"intermediateEscalationThrowEvent";


        //end events ////////////////////////////////////////
        public const string EndEventError = "errorendevent";//"errorEndEvent";
        public const string EndEventCancel = "cancelendevent";//"cancelEndEvent";
        public const string EndEventTerminate = "terminateendevent";//"terminateEndEvent";
        public const string EndEventMessage = "messageendevent";//"messageEndEvent";
        public const string EndEventSignal = "signalendevent";//"signalEndEvent";
        public const string EndEventCompensation = "compensationendevent";//"compensationEndEvent";
        public const string EndEventEscalation = "escalationendevent";//"escalationEndEvent";
        public const string EndEventNone = "noneendevent";//"noneEndEvent";
    }
}