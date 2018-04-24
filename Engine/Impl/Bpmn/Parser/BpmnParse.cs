using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Dmn.Impl.Result;
using ESS.FW.Bpm.Engine.Form.Impl.Handler;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Listener;
using ESS.FW.Bpm.Engine.Impl.Core.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.task;
using ESS.FW.Bpm.Engine.Impl.task.listener;
using IExpression = ESS.FW.Bpm.Engine.Impl.EL.IExpression;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common.Utils;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Impl.Variable;
using ESS.FW.Bpm.Engine.Impl.Scripting;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     用于解析bpn2.0 xml文件,由BpmnParser创建,不要重复使用同一个实例
    ///     Specific parsing of one BPMN 2.0 XML file, created by the <seealso cref="BpmnParser" />.
    ///     Instances of this class should not be reused and are also not threadsafe.
    ///     
    /// </summary>
    public class BpmnParse : Parse
    {
        public const string MultiInstanceBodyIdSuffix = "#multiInstanceBody";

        public const string PropertynameDocumentation = "documentation";
        public const string PropertynameInitiatorVariableName = "initiatorVariableName";
        public const string PropertynameHasConditionalEvents = "hasConditionalEvents";
        public const string PropertynameCondition = "condition";
        public const string PropertynameConditionText = "conditionText";
        public const string PropertynameVariableDeclarations = "variableDeclarations";
        public const string PropertynameTimerDeclaration = "timerDeclarations";
        public const string PropertynameMessageJobDeclaration = "messageJobDeclaration";
        public const string PropertynameIsexpanded = "isExpanded";
        public const string PropertynameStartTimer = "timerStart";
        public const string PropertynameCompensationHandlerId = "compensationHandler";
        public const string PropertynameIsForCompensation = "isForCompensation";
        public const string PropertynameEventSubscriptionJobDeclaration = "eventJobDeclarations";
        public const string PropertynameThrowsCompensation = "throwsCompensation";
        public const string PropertynameConsumesCompensation = "consumesCompensation";
        public const string PropertynameJobPriority = "jobPriority";
        public const string PropertynameTaskPriority = "taskPriority";
        public const string PropertynameClass = "class";
        public const string PropertynameMethod = "method";
        public const string PropertynameExpression = "expression";
        public const string PropertynameDelegateExpression = "delegateExpression";
        public const string PropertynameVariableMappingClass = "variableMappingClass";
        public const string PropertynameVariableMappingDelegateExpression = "variableMappingDelegateExpression";
        public const string PropertynameResource = "resource";
        public const string PropertynameLanguage = "language";
        public const string Type = "type";

        public const string True = "true";
        public const string Interrupting = "isInterrupting";

        public const string ConditionalEventDefinition = "conditionalEventDefinition";
        public const string EscalationEventDefinition = "escalationEventDefinition";
        public const string CompensateEventDefinition = "compensateEventDefinition";
        public const string TimerEventDefinition = "timerEventDefinition";
        public const string SignalEventDefinition = "signalEventDefinition";
        public const string MessageEventDefinition = "messageEventDefinition";
        public const string ErrorEventDefinition = "errorEventDefinition";
        public const string CancelEventDefinition = "cancelEventDefinition";
        public const string LinkEventDefinition = "linkEventDefinition";
        public const string ConditionExpression = "conditionExpression";
        public const string Condition = "condition";

        /* process start authorization specific finals */
        protected internal const string PotentialStarter = "potentialStarter";
        protected internal const string CandidateStarterUsersExtension = "candidateStarterUsers";
        protected internal const string CandidateStarterGroupsExtension = "candidateStarterGroups";

        public const string PropertynameIsMultiInstance = "isMultiInstance";
        public const string All = "all";

        /* userTask specific finals */

        protected internal const string HumanPerformer = "humanPerformer";
        protected internal const string PotentialOwner = "potentialOwner";

        protected internal const string ResourceAssignmentExpr = "resourceAssignmentExpression";
        protected internal const string FormalExpression = "formalExpression";

        protected internal const string UserPrefix = "user(";
        protected internal const string GroupPrefix = "group(";

        protected internal const string AssigneeExtension = "assignee";
        protected internal const string CandidateUsersExtension = "candidateUsers";
        protected internal const string CandidateGroupsExtension = "candidateGroups";
        protected internal const string DueDateExtension = "dueDate";
        protected internal const string FollowUpDateExtension = "followUpDate";
        protected internal const string PriorityExtension = "priority";

        protected internal static readonly BpmnParseLogger Log = ProcessEngineLogger.BpmnParseLogger;

        public static readonly IList<string> VariableEvents = new List<string>
        {
            VariableListenerFields.Create,
            VariableListenerFields.Delete,
            VariableListenerFields.Update
        };

        /// @deprecated use
        /// <seealso cref="BpmnProperties#TYPE" />
        [Obsolete("use <seealso cref=\"BpmnProperties#TYPE\"/>")]
        public static readonly string PropertynameType =
            BpmnProperties.Type.Name;

        /// @deprecated use
        /// <seealso cref="BpmnProperties#ERROR_EVENT_DEFINITIONS" />
        [Obsolete("use <seealso cref=\"BpmnProperties#ERROR_EVENT_DEFINITIONS\"/>")]
        public static readonly string
            PropertynameErrorEventDefinitions = BpmnProperties.ErrorEventDefinitions.Name;

        protected internal static readonly string AttributevalueTFormalExpression = BpmnParser.Bpmn20Ns +
                                                                                       ":tFormalExpression";

        public static readonly Namespace CamundaBpmnExtensionsNs =
            new Namespace(BpmnParser.CamundaBpmnExtensionsNs, BpmnParser.ActivitiBpmnExtensionsNs);

        public static readonly Namespace XsiNs = new Namespace(BpmnParser.XsiNs);
        public static readonly Namespace BpmnDiNs = new Namespace(BpmnParser.BpmnDiNs);
        public static readonly Namespace OmgDiNs = new Namespace(BpmnParser.OmgDiNs);
        public static readonly Namespace BpmnDcNs = new Namespace(BpmnParser.BpmnDcNs);

        private readonly IDictionary<string, string> _eventLinkSources = new Dictionary<string, string>();

        private readonly IDictionary<string, string> _eventLinkTargets = new Dictionary<string, string>();
        /// <summary>
        /// error元素
        /// </summary>
        protected IDictionary<string, Error> BpmnErrors = new Dictionary<string, Error>();

        /// <summary>
        ///     The deployment to which the parsed process definitions will be added.
        /// </summary>
        protected internal DeploymentEntity DeploymentRenamed;

        /// <summary>
        ///     A list of all element IDs. This allows us to parse only what we actually
        ///     support but still validate the references among elements we do not support.
        /// </summary>
        protected internal IList<string> ElementIds = new List<string>();

        //基类已有相同名称 已改名
        /// <summary>
        ///     Mapping of found errors in BPMN 2.0 file
        /// </summary>
        protected internal IDictionary<string, Error> Errors = new Dictionary<string, Error>();

        /// <summary>
        ///     Mapping of found escalation elements
        /// </summary>
        protected internal IDictionary<string, Escalation> Escalations = new Dictionary<string, Escalation>();

        // Members
        protected internal ExpressionManager ExpressionManager;
        protected internal IDictionary<string, IXmlImporter> Importers = new Dictionary<string, IXmlImporter>();

        /// <summary>
        ///     Mapping from a process definition key to his containing list of job
        ///     declarations
        /// </summary>
        protected internal IDictionary<string, IList<IJobDeclaration>> jobDeclarations = new Dictionary<string, IList<IJobDeclaration>>();

        /// <summary>
        ///     Mapping containing values stored during the first phase of parsing since
        ///     other elements can reference these messages.
        ///     All the map's elements are defined outside the process definition(s), which
        ///     means that this map doesn't need to be re-initialized for each new process
        ///     definition.
        /// </summary>
        protected internal IDictionary<string, MessageDefinition> Messages = new Dictionary<string, MessageDefinition>();

        protected internal IList<IBpmnParseListener> ParseListeners;

        /// <summary>
        ///     A map for storing the process references of participants
        /// </summary>
        protected internal IDictionary<string, string> ParticipantProcesses = new Dictionary<string, string>();

        protected internal IDictionary<string, string> Prefixs = new Dictionary<string, string>();

        /// <summary>
        ///     The end result of the parsing: a list of process definition.
        /// </summary>
        //protected internal IList<ProcessDefinitionEntity> processDefinitions = new List<ProcessDefinitionEntity>();
        protected internal IList<Persistence.Entity.ProcessDefinitionEntity> processDefinitions = new List<Persistence.Entity.ProcessDefinitionEntity>();

        /// <summary>
        ///     A map for storing sequence flow based on their id during parsing.
        /// </summary>
        protected internal IDictionary<string, TransitionImpl> SequenceFlows;

        protected internal IDictionary<string, SignalDefinition> Signals = new Dictionary<string, SignalDefinition>();
        protected internal string TargetNamespace;
        protected XmlReaderSettings XmlReaderSettings;

        /// <summary>
        ///     用于bpmn20协议校验(内部有事件通知)
        /// </summary>
        protected XmlSchemaSet XmlSchemaSet;

        /// <summary>
        ///     Constructor to be called by the <seealso cref="BpmnParser" />.
        /// </summary>
        public BpmnParse(BpmnParser parser) : base(parser)
        {
            ExpressionManager = parser.ExpressionManager;
            ParseListeners = parser.ParseListeners;

            // Todo : xsd:boolean类型之支持true|false，C# Boolean.ToString() True|False字符串被视为非法值
            //架构信息,用户校验xml文档协议(bpmn2.0)
            //SchemaResource = Properties.Resources.BPMN20;//ReflectUtil.GetResourceUrlAsString(BpmnParser.Bpmn20SchemaLocation); //转换成了物理地址
            ////bpmn2.0协议校验
            //XmlReaderSettings = new XmlReaderSettings();
            //XmlReaderSettings.ValidationType = ValidationType.Schema;

            //XmlReaderSettings.Schemas = parser.XmlSchemaSetBase;
            //XmlReaderSettings.ValidationEventHandler += Schema_ValidationEventHandler;
        }

        /// <summary>
        ///     Sets the delegates for the activity, which will be called
        ///     if the attribute asyncAfter or asyncBefore was changed.
        /// </summary>
        /// <param name="activity"> the activity which gets the delegates </param>
        protected internal virtual ActivityImpl ActivityAsyncDelegates
        {
            set
            {
                value.DelegateAsyncAfterUpdate = new AsyncAfterUpdateAnonymousInnerClass(this, value);

                value.DelegateAsyncBeforeUpdate = new AsyncBeforeUpdateAnonymousInnerClass(this, value);
            }
        }

        // Getters, setters and Parser overridden operations
        // ////////////////////////////////////////

        public virtual IList<ProcessDefinitionEntity> ProcessDefinitions
        {
            get { return processDefinitions; }
        }

        public virtual IDictionary<string, IList<IJobDeclaration>> GetJobDeclarations()
        {
            return jobDeclarations;
        }

        public virtual IList<IJobDeclaration> GetJobDeclarationsByKey(string processDefinitionKey)
        {
            return jobDeclarations[processDefinitionKey];
        }


        //bpmn2.0校验异常事件通知 
        private void Schema_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
                AddError(e.Message, rootElement);
            if (e.Severity == XmlSeverityType.Warning)
                AddWarning(e.Message, rootElement);
        }

        public virtual BpmnParse Deployment(DeploymentEntity deployment)
        {
            DeploymentRenamed = deployment;
            return this;
        }

        /// <summary>
        ///     重写了基类的errors重名变量->bpmnErrors
        ///     理解错误。基类ERROR是解析错误
        ///     此类error是解析的error元素
        /// </summary>
        /// <returns></returns>
        //public override bool HasErrors()
        //{
        //    return (BpmnErrors != null) && (BpmnErrors.Count > 0);
        //}

        //TODO addError已重写 格式需定义
        /// <summary>
        ///     已重写 格式需定义
        /// </summary>
        /// <param name="e"></param>
        public override void AddError(BpmnParseException e)
        {
            //base.AddError(e);
            var key = Guid.NewGuid().ToString();
            BpmnErrors.Add(key, new Error { Id = key, ErrorCode = e.Message });
        }

        //TODO addError已重写 格式需定义
        /// <summary>
        ///     已重写 格式需定义
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="element"></param>
        // public override void AddError(string errorMessage, Element element)
        //{
        //base.addError(errorMessage, element);
        //bpmnErrors.Add(new Problem(errorMessage, name_Renamed, element));
        //var key = Guid.NewGuid().ToString();
        //BpmnErrors.Add(key, new Error { Id = key, ErrorCode = errorMessage });
        //base.AddError(errorMessage, element);
        //}

        public override Parse Execute()
        {
            base.Execute(); // schema validation
            //TODO 取消异常拦截及抛出
            //ParseRootElement();
            try
            {
                ParseRootElement();
            }
            catch (BpmnParseException e)
            {
                AddError(e);
            }
            catch (System.Exception e)
            {
                Log.ParsingFailure(e);

                // ALL unexpected exceptions should bubble up since they are not handled
                // accordingly by underlying parse-methods and the process can't be
                // deployed
                throw Log.ParsingProcessException(e);
            }
            finally
            {
                if (HasWarnings())
                {
                    LogWarnings();
                }
                if (HasErrors())
                {
                    ThrowExceptionForErrors();
                }
            }

            return this;
        }
        //基类有
        //public override void ThrowExceptionForErrors()
        //{
        //    var strb = new StringBuilder();
        //    foreach (var error in base.Errors)
        //    {
        //        strb.Append("\n* ");
        //        strb.Append(error.ToString()/*.Value.ErrorCode*/);
        //    }
        //    throw ProcessEngineLogger.UtilLogger.ExceptionDuringParsing(strb.ToString());
        //}

        /// <summary>
        ///     从xml到Element
        ///     Parses the 'definitions' root element
        /// </summary>
        protected internal virtual void ParseRootElement()
        {
            var parse = this;
            CollectElementIds();
            ParseDefinitionsAttributes();
            ParseImports();
            ParseMessages();
            ParseSignals();
            ParseErrors();
            ParseEscalations();
            ParseProcessDefinitions();
            ParseCollaboration();


            // Diagram interchange parsing must be after parseProcessDefinitions,
            // since it depends and sets values on existing process definition objects
            ParseDiagramInterchangeElements();

            foreach (var parseListener in ParseListeners)
                parseListener.ParseRootElement(rootElement, ProcessDefinitions);
        }

        /// <summary>
        ///     递归获取所有Element包含的key为Id的attribute值 赋给elementIds
        /// </summary>
        private void CollectElementIds()
        {
            rootElement.CollectIds(ElementIds);
        }

        /// <summary>
        ///     解析定义的特性集合
        /// </summary>
        private void ParseDefinitionsAttributes()
        {
            TargetNamespace = rootElement.GetAttributeValue("targetNamespace");

            foreach (var attribute in rootElement.GetAllAttributeKeys())
                if (attribute.StartsWith("http://www.w3.org/2000/xmlns/:", StringComparison.Ordinal))
                {
                    var prefixValue = rootElement.GetAttributeValue(attribute);
                    var prefixName = attribute.Substring("http://www.w3.org/2000/xmlns/:".Length);
                    Prefixs[prefixName] = prefixValue;
                }
        }

        protected internal virtual string ResolveName(string name)
        {
            if (ReferenceEquals(name, null))
                return null;
            var indexOfP = name.IndexOf(':');
            if (indexOfP != -1)
            {
                var prefix = name.Substring(0, indexOfP);
                var resolvedPrefix = Prefixs[prefix];
                return resolvedPrefix + ":" + name.Substring(indexOfP + 1);
            }
            return TargetNamespace + ":" + name;
        }

        /// <summary>
        ///     Parses the rootElement importing structures
        /// </summary>
        private void ParseImports()
        {
            var imports = rootElement.Elements("import");
            foreach (var theImport in imports)
            {
                var importType = theImport.GetAttributeValue("importType");
                var importer = GetImporter(importType, theImport);
                if (importer == null)
                    AddError("Could not import item of type " + importType, theImport);
                else
                    importer.ImportFrom(theImport, this);
            }
        }

        protected internal virtual IXmlImporter GetImporter(string importType, Element theImport)
        {
            if (Importers.ContainsKey(importType))
                return Importers[importType];
            if (importType.Equals("http://schemas.xmlsoap.org/wsdl/"))
            {
                Type wsdlImporterClass;
                try
                {
                    wsdlImporterClass = System.Type.GetType("org.camunda.bpm.engine.impl.webservice.CxfWSDLImporter", true);
                    var newInstance = (IXmlImporter)Activator.CreateInstance(wsdlImporterClass);
                    Importers[importType] = newInstance;
                    return newInstance;
                }
                catch (System.Exception)
                {
                    AddError("Could not find importer for type " + importType, theImport);
                }
            }
            return null;
        }

        /// <summary>
        ///     Parses the messages of the given definitions file. Messages are not
        ///     contained within a process element, but they can be referenced from inner
        ///     process elements.
        /// </summary>
        public virtual void ParseMessages()
        {
            foreach (var messageElement in rootElement.Elements("message"))
            {
                var id = messageElement.GetAttributeValue("id");
                var messageName = messageElement.GetAttributeValue("name");

                IExpression messageExpression = null;
                if (!ReferenceEquals(messageName, null))
                    messageExpression = (IExpression)ExpressionManager.CreateExpression(messageName);

                var messageDefinition = new MessageDefinition(TargetNamespace + ":" + id, (Engine.Delegate.IExpression)messageExpression);
                Messages[messageDefinition.Id] = messageDefinition;
            }
        }

        /// <summary>
        ///     Parses the signals of the given definitions file. Signals are not contained
        ///     within a process element, but they can be referenced from inner process
        ///     elements.
        /// </summary>
        protected internal virtual void ParseSignals()
        {
            foreach (var signalElement in rootElement.Elements("signal"))
            {
                var id = signalElement.GetAttributeValue("id");
                var signalName = signalElement.GetAttributeValue("name");

                foreach (var signalDefinition in Signals.Values)
                    if (signalDefinition.Name.Equals(signalName))
                        AddError("duplicate signal name '" + signalName + "'.", signalElement);

                if (ReferenceEquals(id, null))
                {
                    AddError("signal must have an id", signalElement);
                }
                else if (ReferenceEquals(signalName, null))
                {
                    AddError("signal with id '" + id + "' has no name", signalElement);
                }
                else
                {
                    var signalExpression = ExpressionManager.CreateExpression(signalName);
                    var signal = new SignalDefinition();
                    signal.Id = TargetNamespace + ":" + id;
                    signal.Expression = signalExpression;

                    Signals[signal.Id] = signal;
                }
            }
        }

        public virtual void ParseErrors()
        {
            foreach (var errorElement in rootElement.Elements("error"))
            {
                var error = new Error();

                var id = errorElement.GetAttributeValue("id");
                if (ReferenceEquals(id, null))
                    AddError("'id' is mandatory on error definition", errorElement);
                error.Id = id;

                var errorCode = errorElement.GetAttributeValue("errorCode");
                if (!ReferenceEquals(errorCode, null))
                    error.ErrorCode = errorCode;

                BpmnErrors[id] = error;
            }
        }

        protected internal virtual void ParseEscalations()
        {
            foreach (var element in rootElement.Elements("escalation"))
            {
                var id = element.GetAttributeValue("id");
                if (ReferenceEquals(id, null))
                {
                    AddError("escalation must have an id", element);
                }
                else
                {
                    var escalation = CreateEscalation(id, element);
                    Escalations[id] = escalation;
                }
            }
        }

        protected internal virtual Escalation CreateEscalation(string id, Element element)
        {
            var escalation = new Escalation(id);

            var name = element.GetAttributeValue("name");
            if (!ReferenceEquals(name, null))
                escalation.Name = name;

            var escalationCode = element.GetAttributeValue("escalationCode");
            if (!ReferenceEquals(escalationCode, null) && (escalationCode.Length > 0))
                escalation.EscalationCode = escalationCode;
            return escalation;
        }

        /// <summary>
        ///     Parses all the process definitions defined within the 'definitions' root
        ///     element.
        /// </summary>
        public virtual void ParseProcessDefinitions()
        {
            foreach (var processElement in rootElement.Elements("process"))
            {
                var isExecutable = !DeploymentRenamed.IsNew;
                var isExecutableStr = processElement.GetAttributeValue("isExecutable");
                if (!ReferenceEquals(isExecutableStr, null))
                {
                    isExecutable = bool.Parse(isExecutableStr);
                    if (!isExecutable)
                        Log.IgnoringNonExecutableProcess(processElement.GetAttributeValue("id"));
                }
                else
                {
                    Log.MissingIsExecutableAttribute(processElement.GetAttributeValue("id"));
                }

                // Only process executable processes
                if (isExecutable)
                    processDefinitions.Add(ParseProcess(processElement));
            }
        }

        /// <summary>
        ///     Parses the collaboration(协作) definition defined within the 'definitions' root
        ///     element and get all participants to lookup their process references during
        ///     DI parsing.
        /// </summary>
        public virtual void ParseCollaboration()
        {
            var collaboration = rootElement.element("collaboration");
            if (collaboration != null)
                foreach (var participant in collaboration.Elements("participant"))
                {
                    var processRef = participant.GetAttributeValue("processRef");
                    if (!ReferenceEquals(processRef, null))
                    {
                        var procDef = GetProcessDefinition(processRef);
                        if (procDef != null)
                        {
                            // Set participant process on the procDef, so it can get rendered
                            // later on if needed
                            var participantProcess = new ParticipantProcess();
                            participantProcess.Id = participant.GetAttributeValue("id");
                            participantProcess.Name = participant.GetAttributeValue("name");
                            procDef.ParticipantProcess = participantProcess;

                            ParticipantProcesses[participantProcess.Id] = processRef;
                        }
                    }
                }
        }

        //已重构返回类型ProcessDefinitionEntity,获取entity需要再次转换
        /// <summary>
        ///     解析Process 移除直接调用/返回ProcessDefinitionEntity,新建数据模型 继承自<seealso cref="ProcessDefinitionImpl" />
        ///     Parses one process (ie anything inside a &lt;process&gt; element).
        /// </summary>
        /// <param name="processElement">
        ///     The 'process' element.
        /// </param>
        /// <returns>
        ///     The parsed version of the XML: a <seealso cref="ProcessDefinitionImpl" />
        ///     object.
        /// </returns>
        public virtual ProcessDefinitionEntity ParseProcess(Element processElement)
        {
            // reset all mappings that are related to one process definition
            SequenceFlows = new Dictionary<string, TransitionImpl>();
            var processDefinitionId = processElement.GetAttributeValue("id");
            var processDefinition = new ProcessDefinitionEntity();
            /*
		 * Mapping object model - bpmn xml: processDefinition.id -> generated by
		 * processDefinition.key -> bpmn id (required) processDefinition.name ->
		 * bpmn name (optional)
		 */
            processDefinition.Key = processDefinitionId;
            processDefinition.Name = processElement.GetAttributeValue("name");
            processDefinition.Category = rootElement.GetAttributeValue("targetNamespace");
            processDefinition.SetProperty(PropertynameDocumentation, ParseDocumentation(processElement));
            processDefinition.TaskDefinitions = new Dictionary<string, TaskDefinition>();
            processDefinition.DeploymentId = DeploymentRenamed.Id;
            //TODO 需要传入ExpressionManager才能解析
            processDefinition.SetProperty(PropertynameJobPriority, ParsePriority(processElement, PropertynameJobPriority));
            processDefinition.SetProperty(PropertynameTaskPriority, ParsePriority(processElement, PropertynameTaskPriority));
            processDefinition.VersionTag = processElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                "versionTag");

            Log.ParsingElement("process", processDefinition.Key);
            //解析Scope
            ParseScope(processElement, processDefinition);
            //解析LaneSet
            // Parse any laneSets defined for this process
            ParseLaneSets(processElement, processDefinition);
            //TODO Process在此处由bpmnParser属性的自定义listener解析
            foreach (var parseListener in ParseListeners)
                parseListener.ParseProcess(processElement, processDefinition);

            // now we have parsed anything we can validate some stuff
            ValidateActivities(processDefinition.GetActivities());

            //unregister delegates
            //此处可以添加自定义更新事件
            foreach (var activity in processDefinition.GetActivities())
            {
                activity.DelegateAsyncAfterUpdate = null;
                activity.DelegateAsyncBeforeUpdate = null;
            }
            return processDefinition;
        }

        protected internal virtual void ParseLaneSets(Element parentElement, ProcessDefinitionEntity processDefinition)
        {
            var laneSets = parentElement.Elements("laneSet");

            if ((laneSets != null) && (laneSets.Count > 0))
                foreach (var laneSetElement in laneSets)
                {
                    var newLaneSet = new LaneSet();

                    newLaneSet.Id = laneSetElement.GetAttributeValue("id");
                    newLaneSet.Name = laneSetElement.GetAttributeValue("name");
                    ParseLanes(laneSetElement, newLaneSet);

                    // Finally, add the set
                    processDefinition.AddLaneSet(newLaneSet);
                }
        }

        protected internal virtual void ParseLanes(Element laneSetElement, LaneSet laneSet)
        {
            var lanes = laneSetElement.Elements("lane");
            if ((lanes != null) && (lanes.Count > 0))
                foreach (var laneElement in lanes)
                {
                    // Parse basic attributes
                    var lane = new Lane();
                    lane.Id = laneElement.GetAttributeValue("id");
                    lane.Name = laneElement.GetAttributeValue("name");

                    // Parse ID's of flow-nodes that live inside this lane
                    var flowNodeElements = laneElement.Elements("flowNodeRef");
                    if ((flowNodeElements != null) && (flowNodeElements.Count > 0))
                        foreach (var flowNodeElement in flowNodeElements)
                            lane.FlowNodeIds.Add(flowNodeElement.Text);

                    laneSet.AddLane(lane);
                }
        }

        /// <summary>
        ///     Parses a scope: a process, subprocess, etc.
        ///     Note that a process definition is a scope on itself.
        /// </summary>
        /// <param name="scopeElement">
        ///     The XML element defining the scope
        /// </param>
        /// <param name="parentScope">
        ///     The scope that contains the nested scope.
        /// </param>
        public virtual void ParseScope(Element scopeElement, ScopeImpl parentScope)
        {
            // Not yet supported on process level (PVM additions needed):
            //parseProperties(processElement);

            // filter activities that must be parsed separately
            IList<Element> activityElements = new List<Element>(scopeElement.GetAllElement());
            IDictionary<string, Element> intermediateCatchEvents = FilterIntermediateCatchEvents(activityElements);
            IDictionary<string, Element> compensationHandlers = filterCompensationHandlers(activityElements);
            IList<Element> activityElementsNew = new List<Element>(activityElements);
            //过滤,合并上面2个removeAll
            for (int i = 0; i < activityElements.Count; i++)
            {
                if (intermediateCatchEvents.Values.Contains(activityElements[i]) || compensationHandlers.Values.Contains(activityElements[i]))
                {
                    activityElementsNew.Remove(activityElements[i]);
                }
            }

            //从element到实例
            ParseStartEvents(scopeElement, parentScope);
            ParseActivities(activityElementsNew, scopeElement, parentScope);
            ParseIntermediateCatchEvents(scopeElement, parentScope, intermediateCatchEvents);
            ParseEndEvents(scopeElement, parentScope);
            ParseBoundaryEvents(scopeElement, parentScope);
            ParseSequenceFlow(scopeElement, parentScope, compensationHandlers);
            ParseExecutionListenersOnScope(scopeElement, parentScope);
            ParseAssociations(scopeElement, parentScope, compensationHandlers);
            parseCompensationHandlers(parentScope, compensationHandlers);

            foreach (var callback in parentScope.BacklogErrorCallbacks)
                callback.Callback();

            if (parentScope is IProcessDefinition)
                ParseProcessDefinitionCustomExtensions(scopeElement, (IProcessDefinition)parentScope);
        }

        protected internal virtual Dictionary<string, Element> FilterIntermediateCatchEvents(
            IList<Element> activityElements)
        {
            var intermediateCatchEvents = new Dictionary<string, Element>();
            foreach (var activityElement in activityElements)
                if (activityElement.TagName.ToLower() == ActivityTypes.IntermediateEventCatch)
                    intermediateCatchEvents[activityElement.GetAttributeValue("id")] = activityElement;
            return intermediateCatchEvents;
        }

        protected internal virtual Dictionary<string, Element> filterCompensationHandlers(
            IList<Element> activityElements)
        {
            var compensationHandlers = new Dictionary<string, Element>();
            foreach (var activityElement in activityElements)
                if (isCompensationHandler(activityElement))
                    compensationHandlers[activityElement.GetAttributeValue("id")] = activityElement;
            return compensationHandlers;
        }

        protected internal virtual void ParseIntermediateCatchEvents(Element scopeElement, ScopeImpl parentScope,
            IDictionary<string, Element> intermediateCatchEventElements)
        {
            foreach (var intermediateCatchEventElement in intermediateCatchEventElements.Values)
                if (parentScope.FindActivity(intermediateCatchEventElement.GetAttributeValue("id")) == null)
                {
                    // check whether activity is already parsed
                    var activity = ParseIntermediateCatchEvent(intermediateCatchEventElement, parentScope, null);

                    if (activity != null)
                        ParseActivityInputOutput(intermediateCatchEventElement, activity);
                }
            intermediateCatchEventElements.Clear();
        }

        protected internal virtual void ParseProcessDefinitionCustomExtensions(Element scopeElement,
            IProcessDefinition definition)
        {
            ParseStartAuthorization(scopeElement, definition);
        }

        protected internal virtual void ParseStartAuthorization(Element scopeElement, IProcessDefinition definition)
        {
            var processDefinition = (Persistence.Entity.ProcessDefinitionEntity)definition;

            // parse activiti:potentialStarters
            var extentionsElement = scopeElement.element("extensionElements");
            if (extentionsElement != null)
            {
                var potentialStarterElements = extentionsElement.ElementsNS(CamundaBpmnExtensionsNs,
                    PotentialStarter);

                foreach (var potentialStarterElement in potentialStarterElements)
                    ParsePotentialStarterResourceAssignment(potentialStarterElement, processDefinition);
            }

            // parse activiti:candidateStarterUsers
            var candidateUsersString = scopeElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                CandidateStarterUsersExtension);
            if (!ReferenceEquals(candidateUsersString, null))
            {
                var candidateUsers = ParseCommaSeparatedList(candidateUsersString);
                foreach (var candidateUser in candidateUsers)
                {
                    processDefinition.AddCandidateStarterUserIdExpression(
                        ExpressionManager.CreateExpression(candidateUser.Trim()));
                }
            }

            // Candidate activiti:candidateStarterGroups
            var candidateGroupsString = scopeElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                CandidateStarterGroupsExtension);
            if (!ReferenceEquals(candidateGroupsString, null))
            {
                var candidateGroups = ParseCommaSeparatedList(candidateGroupsString);
                foreach (var candidateGroup in candidateGroups)
                {
                    processDefinition.AddCandidateStarterGroupIdExpression(
                        ExpressionManager.CreateExpression(candidateGroup.Trim()));
                }
            }
        }

        protected internal virtual void ParsePotentialStarterResourceAssignment(Element performerElement,
            Persistence.Entity.ProcessDefinitionEntity processDefinition)
        {
            var raeElement = performerElement.element(ResourceAssignmentExpr);
            if (raeElement != null)
            {
                var feElement = raeElement.element(FormalExpression);
                if (feElement != null)
                {
                    var assignmentExpressions = ParseCommaSeparatedList(feElement.Text);
                    foreach (var assignmentExpression in assignmentExpressions)
                        if (assignmentExpression.StartsWith(UserPrefix, StringComparison.Ordinal))
                        {
                            var userAssignementId = GetAssignmentId(assignmentExpression, UserPrefix);
                            processDefinition.AddCandidateStarterUserIdExpression(ExpressionManager.CreateExpression(userAssignementId));
                        }
                        else if (assignmentExpression.StartsWith(GroupPrefix, StringComparison.Ordinal))
                        {
                            var groupAssignementId = GetAssignmentId(assignmentExpression, GroupPrefix);
                            processDefinition.AddCandidateStarterGroupIdExpression(ExpressionManager.CreateExpression(groupAssignementId));
                        }
                }
            }
        }

        protected internal virtual void ParseAssociations(Element scopeElement, ScopeImpl parentScope,
            IDictionary<string, Element> compensationHandlers)
        {
            foreach (var associationElement in scopeElement.Elements("association"))
            {
                var sourceRef = associationElement.GetAttributeValue("sourceRef");
                if (ReferenceEquals(sourceRef, null))
                    AddError("association element missing attribute 'sourceRef'", associationElement);
                var targetRef = associationElement.GetAttributeValue("targetRef");
                if (ReferenceEquals(targetRef, null))
                    AddError("association element missing attribute 'targetRef'", associationElement);
                var sourceActivity = (ActivityImpl)parentScope.FindActivity(sourceRef);
                var targetActivity = (ActivityImpl)parentScope.FindActivity(targetRef);

                // an association may reference elements that are not parsed as activities
                // (like for instance text annotations so do not throw an exception if sourceActivity or targetActivity are null)
                // However, we make sure they reference 'something':
                if ((sourceActivity == null) && !ElementIds.Contains(sourceRef))
                {
                    AddError("Invalid reference sourceRef '" + sourceRef + "' of association element ",
                        associationElement);
                }
                else if ((targetActivity == null) && !ElementIds.Contains(targetRef))
                {
                    AddError("Invalid reference targetRef '" + targetRef + "' of association element ",
                        associationElement);
                }
                else
                {
                    if ((sourceActivity != null) &&
                        ActivityTypes.BoundaryCompensation.Equals(sourceActivity.GetProperty(BpmnProperties.Type.Name)))
                    {
                        if ((targetActivity == null) && compensationHandlers.ContainsKey(targetRef))
                        {
                            targetActivity = parseCompensationHandlerForCompensationBoundaryEvent(parentScope,
                                sourceActivity, targetRef, compensationHandlers);

                            compensationHandlers.Remove(targetActivity.Id);
                        }

                        if (targetActivity != null)
                            ParseAssociationOfCompensationBoundaryEvent(associationElement, sourceActivity,
                                targetActivity);
                    }
                }
            }
        }

        protected internal virtual ActivityImpl parseCompensationHandlerForCompensationBoundaryEvent(
            ScopeImpl parentScope, ActivityImpl sourceActivity, string targetRef,
            IDictionary<string, Element> compensationHandlers)
        {
            var compensationHandler = compensationHandlers[targetRef];

            var eventScope = (ActivityImpl)sourceActivity.EventScope;
            ActivityImpl compensationHandlerActivity = null;
            if (eventScope.MultiInstance)
            {
                var miBody = eventScope.FlowScope;
                compensationHandlerActivity = ParseActivity(compensationHandler, null, miBody);
            }
            else
            {
                compensationHandlerActivity = ParseActivity(compensationHandler, null, parentScope);
            }

            compensationHandlerActivity.Properties.Set(BpmnProperties.CompensationBoundaryEvent, sourceActivity);
            return compensationHandlerActivity;
        }

        protected internal virtual void ParseAssociationOfCompensationBoundaryEvent(Element associationElement,
            ActivityImpl sourceActivity, ActivityImpl targetActivity)
        {
            if (!targetActivity.CompensationHandler)
            {
                AddError("compensation boundary catch must be connected to element with isForCompensation=true",
                    associationElement);
            }
            else
            {
                var compensatedActivity = (ActivityImpl)sourceActivity.EventScope;

                var compensationHandler = compensatedActivity.findCompensationHandler();
                if ((compensationHandler != null) && compensationHandler.SubProcessScope)
                    AddError(
                        "compensation boundary event and event subprocess with compensation start event are not supported on the same scope",
                        associationElement);
                else
                    compensatedActivity.SetProperty(PropertynameCompensationHandlerId, targetActivity.Id);
            }
        }

        protected internal virtual void parseCompensationHandlers(ScopeImpl parentScope,
            IDictionary<string, Element> compensationHandlers)
        {
            // compensation handlers attached to compensation boundary events should be already parsed
            foreach (var compensationHandler in new HashSet<Element>(compensationHandlers.Values))
                ParseActivity(compensationHandler, null, parentScope);
            compensationHandlers.Clear();
        }

        /// <summary>
        ///     Parses the start events of a certain level in the process (process,
        ///     subprocess or another scope).
        /// </summary>
        /// <param name="parentElement">
        ///     传入的xml根节点为process
        ///     The 'parent' element that contains the start events (process,
        ///     subprocess).
        /// </param>
        /// <param name="scope">
        ///     The <seealso cref="ScopeImpl" /> to which the start events must be added.
        /// </param>
        public virtual void ParseStartEvents(Element parentElement, ScopeImpl scope)
        {
            var startEventElements = parentElement.Elements("startEvent");
            //TODO 缺少Attribute testErrorEventSubprocess()
            IList<ActivityImpl> startEventActivities = new List<ActivityImpl>();
            foreach (var startEventElement in startEventElements)
            {
                var startEventActivity = CreateActivityOnScope(startEventElement, scope);
                ParseAsynchronousContinuationForActivity(startEventElement, startEventActivity);

                if (scope is ProcessDefinitionEntity)
                {
                    ParseProcessDefinitionStartEvent(startEventActivity, startEventElement, parentElement, scope);
                    startEventActivities.Add(startEventActivity);
                }
                else
                {
                    ParseScopeStartEvent(startEventActivity, startEventElement, parentElement, (ActivityImpl)scope);
                }

                EnsureNoIoMappingDefined(startEventElement);

                foreach (var parseListener in ParseListeners)
                    parseListener.ParseStartEvent(startEventElement, scope, startEventActivity);
                ParseExecutionListenersOnScope(startEventElement, startEventActivity);
            }

            if (scope is ProcessDefinitionEntity)
            {
                SelectInitial(startEventActivities, (ProcessDefinitionEntity)scope, parentElement);
                parseStartFormHandlers(startEventElements, (ProcessDefinitionEntity)scope);
            }
        }

        protected internal virtual void SelectInitial(IList<ActivityImpl> startEventActivities,
            ProcessDefinitionEntity processDefinition, Element parentElement)
        {
            ActivityImpl initial = null;
            // validate that there is s single none start event / timer start event:
            IList<string> exclusiveStartEventTypes = new[] { "startEvent", "startTimerEvent" };

            foreach (var activityImpl in startEventActivities)
                if (exclusiveStartEventTypes.Contains(activityImpl.GetProperty(BpmnProperties.Type.Name)))
                    if (initial == null)
                        initial = activityImpl;
                    else
                        AddError(
                            "multiple none start events or timer start events not supported on process definition",
                            parentElement);
            // if there is a single start event, select it as initial, regardless of it's type:
            if ((initial == null) && (startEventActivities.Count == 1))
                initial = startEventActivities[0];
            processDefinition.Initial = initial;
        }

        protected internal virtual void ParseProcessDefinitionStartEvent(ActivityImpl startEventActivity,
            Element startEventElement, Element parentElement, ScopeImpl scope)
        {
            var processDefinition = (ProcessDefinitionEntity)scope;
            //CAMUNDA_BPMN_EXTENSIONS_NS
            var initiatorVariableName = startEventElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                "initiator");
            if (initiatorVariableName != null)
                processDefinition.SetProperty(PropertynameInitiatorVariableName, initiatorVariableName);

            // all start events share the same behavior:
            startEventActivity.SetActivityBehavior(new NoneStartEventActivityBehavior());
            var timerEventDefinition = startEventElement.element(TimerEventDefinition);
            var messageEventDefinition = startEventElement.element(MessageEventDefinition);
            var signalEventDefinition = startEventElement.element(SignalEventDefinition);
            if (timerEventDefinition != null)
            {
                ParseTimerStartEventDefinition(timerEventDefinition, startEventActivity, processDefinition);
            }
            else if (messageEventDefinition != null)
            {
                startEventActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.StartEventMessage);

                var messageStartEventSubscriptionDeclaration = ParseMessageEventDefinition(messageEventDefinition);
                messageStartEventSubscriptionDeclaration.ActivityId = startEventActivity.Id;
                messageStartEventSubscriptionDeclaration.StartEvent = true;

                EnsureNoExpressionInMessageStartEvent(messageEventDefinition, messageStartEventSubscriptionDeclaration);
                AddEventSubscriptionDeclaration(messageStartEventSubscriptionDeclaration, processDefinition,
                    startEventElement);
            }
            else if (signalEventDefinition != null)
            {
                startEventActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.StartEventSignal);
                startEventActivity.EventScope = scope;

                ParseSignalCatchEventDefinition(signalEventDefinition, startEventActivity, true);
            }
        }

        protected internal virtual void parseStartFormHandlers(IList<Element> startEventElements,
            ProcessDefinitionEntity processDefinition)
        {
            if (processDefinition.Initial != null)
                foreach (var startEventElement in startEventElements)
                    if (startEventElement.GetAttributeValue("id").Equals(processDefinition.Initial.Id))
                    {
                        IStartFormHandler startFormHandler;
                        var startFormHandlerClassName = startEventElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                            "formHandlerClass");
                        if (!ReferenceEquals(startFormHandlerClassName, null))
                            startFormHandler = (IStartFormHandler)ReflectUtil.Instantiate(startFormHandlerClassName);
                        else
                            startFormHandler = new DefaultStartFormHandler();
                        //TODO parseConfiguration
                        startFormHandler.ParseConfiguration(startEventElement, DeploymentRenamed, processDefinition, this);

                        processDefinition.StartFormHandler = new DelegateStartFormHandler(startFormHandler, DeploymentRenamed);
                    }
        }

        protected internal virtual void ParseScopeStartEvent(ActivityImpl startEventActivity, Element startEventElement,
            Element parentElement, ActivityImpl scopeActivity)
        {
            var scopeProperties = scopeActivity.Properties;

            // set this as the scope's initial
            if (!scopeProperties.Contains(BpmnProperties.InitialActivity))
                scopeProperties.Set(BpmnProperties.InitialActivity, startEventActivity);
            else
                AddError("multiple start events not supported for subprocess", startEventElement);

            var errorEventDefinition = startEventElement.element(ErrorEventDefinition);
            var messageEventDefinition = startEventElement.element(MessageEventDefinition);
            var signalEventDefinition = startEventElement.element(SignalEventDefinition);
            var timerEventDefinition = startEventElement.element(TimerEventDefinition);
            var compensateEventDefinition = startEventElement.element(CompensateEventDefinition);
            var escalationEventDefinitionElement = startEventElement.element(EscalationEventDefinition);
            var conditionalEventDefinitionElement = startEventElement.element(ConditionalEventDefinition);

            if (scopeActivity.TriggeredByEvent)
            {
                // event subprocess
                var behavior = new EventSubProcessStartEventActivityBehavior();

                // parse isInterrupting
                var isInterruptingAttr = startEventElement.GetAttributeValue(Interrupting);
                var isInterrupting =True== isInterruptingAttr.ToLower();//.Equals(True, StringComparison.CurrentCultureIgnoreCase);

                if (isInterrupting)
                    scopeActivity.ActivityStartBehavior = ActivityStartBehavior.InterruptEventScope;
                else
                    scopeActivity.ActivityStartBehavior = ActivityStartBehavior.ConcurrentInFlowScope;

                // the event scope of the start event is the flow scope of the event subprocess
                startEventActivity.EventScope = scopeActivity.FlowScope;

                if (errorEventDefinition != null)
                {
                    if (!isInterrupting)
                        AddError("error start event of event subprocess must be interrupting", startEventElement);
                    ParseErrorStartEventDefinition(errorEventDefinition, startEventActivity);
                }
                else if (messageEventDefinition != null)
                {
                    startEventActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.StartEventMessage);

                    var messageStartEventSubscriptionDeclaration = ParseMessageEventDefinition(messageEventDefinition);
                    ParseEventDefinitionForSubprocess(messageStartEventSubscriptionDeclaration, startEventActivity,
                        messageEventDefinition);
                }
                else if (signalEventDefinition != null)
                {
                    startEventActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.StartEventSignal);

                    var eventSubscriptionDeclaration = ParseSignalEventDefinition(signalEventDefinition);
                    ParseEventDefinitionForSubprocess(eventSubscriptionDeclaration, startEventActivity,
                        signalEventDefinition);
                }
                else if (timerEventDefinition != null)
                {
                    ParseTimerStartEventDefinitionForEventSubprocess(timerEventDefinition, startEventActivity,
                        isInterrupting);
                }
                else if (compensateEventDefinition != null)
                {
                    ParseCompensationEventSubprocess(startEventActivity, startEventElement, scopeActivity,
                        compensateEventDefinition);
                }
                else if (escalationEventDefinitionElement != null)
                {
                    startEventActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.StartEventEscalation);

                    var escalationEventDefinition =
                        createEscalationEventDefinitionForEscalationHandler(escalationEventDefinitionElement,
                            scopeActivity, isInterrupting);
                    AddEscalationEventDefinition((ScopeImpl)startEventActivity.EventScope, escalationEventDefinition,
                        escalationEventDefinitionElement);
                }
                else if (conditionalEventDefinitionElement != null)
                {
                    var conditionalEventDef =
                        ParseConditionalStartEventForEventSubprocess(conditionalEventDefinitionElement,
                            startEventActivity, isInterrupting);
                    behavior = new EventSubProcessStartConditionalEventActivityBehavior(conditionalEventDef);
                }
                else
                {
                    AddError(
                        "start event of event subprocess must be of type 'error', 'message', 'timer', 'signal', 'compensation' or 'escalation'",
                        startEventElement);
                }

                startEventActivity.SetActivityBehavior(behavior);
            }
            else
            {
                // "regular" subprocess
                var conditionalEventDefinition = startEventElement.element(ConditionalEventDefinition);

                if (conditionalEventDefinition != null)
                    AddError("conditionalEventDefinition is not allowed on start event within a subprocess",
                        conditionalEventDefinition);
                if (timerEventDefinition != null)
                    AddError("timerEventDefinition is not allowed on start event within a subprocess",
                        timerEventDefinition);
                if (escalationEventDefinitionElement != null)
                    AddError("escalationEventDefinition is not allowed on start event within a subprocess",
                        escalationEventDefinitionElement);
                if (compensateEventDefinition != null)
                    AddError("compensateEventDefinition is not allowed on start event within a subprocess",
                        compensateEventDefinition);
                if (errorEventDefinition != null)
                    AddError("errorEventDefinition only allowed on start event if subprocess is an event subprocess",
                        errorEventDefinition);
                if (messageEventDefinition != null)
                    AddError("messageEventDefinition only allowed on start event if subprocess is an event subprocess",
                        messageEventDefinition);
                if (signalEventDefinition != null)
                    AddError("signalEventDefintion only allowed on start event if subprocess is an event subprocess",
                        messageEventDefinition);

                startEventActivity.SetActivityBehavior(new NoneStartEventActivityBehavior());
            }
        }

        protected internal virtual void ParseCompensationEventSubprocess(ActivityImpl startEventActivity,
            Element startEventElement, ActivityImpl scopeActivity, Element compensateEventDefinition)
        {
            startEventActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.StartEventCompensation);
            scopeActivity.SetProperty(PropertynameIsForCompensation, true);

            if (scopeActivity.FlowScope is ProcessDefinitionEntity)
                AddError(
                    "event subprocess with compensation start event is only supported for embedded subprocess " +
                    "(since throwing compensation through a call activity-induced process hierarchy is not supported)",
                    startEventElement);

            var subprocess = scopeActivity.FlowScope;
            var compensationHandler = ((ActivityImpl)subprocess).findCompensationHandler();
            if (compensationHandler == null)
            {
                // add property to subprocess
                subprocess.SetProperty(PropertynameCompensationHandlerId, scopeActivity.ActivityId);
            }
            else
            {
                if (compensationHandler.SubProcessScope)
                    AddError(
                        "multiple event subprocesses with compensation start event are not supported on the same scope",
                        startEventElement);
                else
                    AddError(
                        "compensation boundary event and event subprocess with compensation start event are not supported on the same scope",
                        startEventElement);
            }

            ValidateCatchCompensateEventDefinition(compensateEventDefinition);
        }

        protected internal virtual void ParseErrorStartEventDefinition(Element errorEventDefinition,
            ActivityImpl startEventActivity)
        {
            startEventActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.StartEventError);
            var errorRef = errorEventDefinition.GetAttributeValue("errorRef");
            Error error = null;
            // the error event definition executes the event subprocess activity which
            // hosts the start event
            var eventSubProcessActivity = startEventActivity.FlowScope.Id;
            var definition = new ErrorEventDefinition(eventSubProcessActivity);
            if (!ReferenceEquals(errorRef, null))
            {
                error = BpmnErrors[errorRef];
                var errorCode = error == null ? errorRef : error.ErrorCode;
                definition.ErrorCode = errorCode;
            }
            definition.Precedence = 10;
            SetErrorCodeVariableOnErrorEventDefinition(errorEventDefinition, definition);
            SetErrorMessageVariableOnErrorEventDefinition(errorEventDefinition, definition);
            AddErrorEventDefinition(definition, (ScopeImpl)startEventActivity.EventScope);
        }

        /// <summary>
        ///     Sets the value for "camunda:errorCodeVariable" on the passed definition if
        ///     it's present.
        /// </summary>
        /// <param name="errorEventDefinition">
        ///     the XML errorEventDefinition tag
        /// </param>
        /// <param name="definition">
        ///     the errorEventDefintion that can get the errorCodeVariable value
        /// </param>
        protected internal virtual void SetErrorCodeVariableOnErrorEventDefinition(Element errorEventDefinition,
            ErrorEventDefinition definition)
        {
            var errorCodeVar = errorEventDefinition.GetAttributeNS(CamundaBpmnExtensionsNs,
                "errorCodeVariable");
            if (!ReferenceEquals(errorCodeVar, null))
                definition.ErrorCodeVariable = errorCodeVar;
        }

        /// <summary>
        ///     Sets the value for "camunda:errorMessageVariable" on the passed definition if
        ///     it's present.
        /// </summary>
        /// <param name="errorEventDefinition">
        ///     the XML errorEventDefinition tag
        /// </param>
        /// <param name="definition">
        ///     the errorEventDefintion that can get the errorMessageVariable value
        /// </param>
        protected internal virtual void SetErrorMessageVariableOnErrorEventDefinition(Element errorEventDefinition,
            ErrorEventDefinition definition)
        {
            var errorMessageVariable = errorEventDefinition.GetAttributeNS(CamundaBpmnExtensionsNs,
                "errorMessageVariable");
            if (!ReferenceEquals(errorMessageVariable, null))
                definition.ErrorMessageVariable = errorMessageVariable;
        }

        protected internal virtual EventSubscriptionDeclaration ParseMessageEventDefinition(
            Element messageEventDefinition)
        {
            var messageRef = messageEventDefinition.GetAttributeValue("messageRef");
            if (ReferenceEquals(messageRef, null))
                AddError("attribute 'messageRef' is required", messageEventDefinition);
            var messageDefinition = Messages[ResolveName(messageRef)];
            if (messageDefinition == null)
                AddError("Invalid 'messageRef': no message with id '" + messageRef + "' found.", messageEventDefinition);
            return new EventSubscriptionDeclaration(messageDefinition.Expression, EventType.Message);
        }

        protected internal virtual void AddEventSubscriptionDeclaration(EventSubscriptionDeclaration subscription,
            ScopeImpl scope, Element element)
        {
            if (subscription.EventType.Equals(EventType.Message.Name) && !subscription.HasEventName())
                AddError("Cannot have a message event subscription with an empty or missing name", element);

            var eventDefinitions = scope.Properties.Get(BpmnProperties.EventSubscriptionDeclarations);

            // if this is a message event, validate that it is the only one with the provided name for this scope
            if (HasMultipleMessageEventDefinitionsWithSameName(subscription, eventDefinitions.Values))
                AddError(
                    "Cannot have more than one message event subscription with name '" +
                    subscription.UnresolvedEventName + "' for scope '" + scope.Id + "'", element);

            // if this is a signal event, validate that it is the only one with the provided name for this scope
            if (HasMultipleSignalEventDefinitionsWithSameName(subscription, eventDefinitions.Values))
                AddError(
                    "Cannot have more than one signal event subscription with name '" + subscription.UnresolvedEventName +
                    "' for scope '" + scope.Id + "'", element);

            scope.Properties.PutMapEntry(BpmnProperties.EventSubscriptionDeclarations, subscription.ActivityId,
                subscription);
        }

        protected internal virtual bool HasMultipleMessageEventDefinitionsWithSameName(
            EventSubscriptionDeclaration subscription, ICollection<EventSubscriptionDeclaration> eventDefinitions)
        {
            return HasMultipleEventDefinitionsWithSameName(subscription, eventDefinitions, EventType.Message.Name);
        }

        protected internal virtual bool HasMultipleSignalEventDefinitionsWithSameName(
            EventSubscriptionDeclaration subscription, ICollection<EventSubscriptionDeclaration> eventDefinitions)
        {
            return HasMultipleEventDefinitionsWithSameName(subscription, eventDefinitions, EventType.Signal.Name);
        }

        protected internal virtual bool HasMultipleEventDefinitionsWithSameName(
            EventSubscriptionDeclaration subscription, ICollection<EventSubscriptionDeclaration> eventDefinitions,
            string eventType)
        {
            if (subscription.EventType.Equals(eventType))
                foreach (var eventDefinition in eventDefinitions)
                    if (eventDefinition.EventType.Equals(eventType) &&
                        eventDefinition.UnresolvedEventName.Equals(subscription.UnresolvedEventName) &&
                        (eventDefinition.StartEvent == subscription.StartEvent))
                        return true;
            return false;
        }

        protected internal virtual void AddEventSubscriptionJobDeclaration(
            EventSubscriptionJobDeclaration jobDeclaration, ActivityImpl activity, Element element)
        {
            var jobDeclarationsForActivity =
                (IList<EventSubscriptionJobDeclaration>)
                activity.GetProperty(PropertynameEventSubscriptionJobDeclaration);

            if (jobDeclarationsForActivity == null)
            {
                jobDeclarationsForActivity = new List<EventSubscriptionJobDeclaration>();
                activity.SetProperty(PropertynameEventSubscriptionJobDeclaration, jobDeclarationsForActivity);
            }

            if (ActivityAlreadyContainsJobDeclarationEventType(jobDeclarationsForActivity, jobDeclaration))
                AddError("Activity contains already job declaration with type " + jobDeclaration.EventType, element);

            jobDeclarationsForActivity.Add(jobDeclaration);
        }

        /// <summary>
        ///     Assumes that an activity has at most one declaration of a certain eventType.
        /// </summary>
        protected internal virtual bool ActivityAlreadyContainsJobDeclarationEventType(
            IList<EventSubscriptionJobDeclaration> jobDeclarationsForActivity,
            EventSubscriptionJobDeclaration jobDeclaration)
        {
            foreach (var declaration in jobDeclarationsForActivity)
                if (declaration.EventType.Equals(jobDeclaration.EventType))
                    return true;
            return false;
        }

        /// <summary>
        ///     Parses the activities of a certain level in the process (process,
        ///     subprocess or another scope).
        /// </summary>
        /// <param name="activityElements">
        ///     The list of activities to be parsed. This list may be filtered before.
        /// </param>
        /// <param name="parentElement">
        ///     The 'parent' element that contains the activities (process, subprocess).
        /// </param>
        /// <param name="scopeElement">
        ///     The <seealso cref="ScopeImpl" /> to which the activities must be added.
        /// </param>
        public virtual void ParseActivities(IList<Element> activityElements, Element parentElement,
            ScopeImpl scopeElement)
        {
            foreach (var activityElement in activityElements)
                ParseActivity(activityElement, parentElement, scopeElement);
        }

        protected internal virtual ActivityImpl ParseActivity(Element activityElement, Element parentElement,
            ScopeImpl scopeElement)
        {
            ActivityImpl activity = null;

            var isMultiInstance = false;
            var miBody = ParseMultiInstanceLoopCharacteristics(activityElement, scopeElement);
            if (miBody != null)
            {
                scopeElement = miBody;
                isMultiInstance = true;
            }
            switch (activityElement.TagName.ToLower())
            {
                case ActivityTypes.GatewayExclusive:
                    activity = ParseExclusiveGateway(activityElement, scopeElement);
                    break;
                case ActivityTypes.GatewayInclusive:
                    activity = ParseInclusiveGateway(activityElement, scopeElement);
                    break;
                case ActivityTypes.GatewayParallel:
                    activity = ParseParallelGateway(activityElement, scopeElement);
                    break;
                case ActivityTypes.TaskScript:
                    activity = ParseScriptTask(activityElement, scopeElement);
                    break;
                case ActivityTypes.TaskService:
                    activity = ParseServiceTask(activityElement, scopeElement);
                    break;
                case ActivityTypes.TaskBusinessRule:
                    activity = ParseBusinessRuleTask(activityElement, scopeElement);
                    break;
                case ActivityTypes.Task:
                    activity = ParseTask(activityElement, scopeElement);
                    break;
                case ActivityTypes.TaskManualTask:
                    activity = ParseManualTask(activityElement, scopeElement);
                    break;
                case ActivityTypes.TaskUserTask:
                    activity = ParseUserTask(activityElement, scopeElement);
                    break;
                case ActivityTypes.TaskSendTask:
                    activity = ParseSendTask(activityElement, scopeElement);
                    break;
                case ActivityTypes.TaskReceiveTask:
                    activity = ParseReceiveTask(activityElement, scopeElement);
                    break;
                case ActivityTypes.SubProcess:
                    activity = ParseSubProcess(activityElement, scopeElement);
                    break;
                case ActivityTypes.CallActivity:
                    activity = ParseCallActivity(activityElement, scopeElement, isMultiInstance);
                    break;
                case ActivityTypes.IntermediateEventThrow:
                    activity = ParseIntermediateThrowEvent(activityElement, scopeElement);
                    break;
                case ActivityTypes.GatewayEventBased:
                    activity = ParseEventBasedGateway(activityElement, parentElement, scopeElement);
                    break;
                case ActivityTypes.Transaction:
                    activity = ParseTransaction(activityElement, scopeElement);
                    break;
                case ActivityTypes.SubProcessAdHoc:
                    AddWarning("Ignoring unsupported activity type", activityElement);
                    break;
                case ActivityTypes.GatewayComplex:
                    AddWarning("Ignoring unsupported activity type", activityElement);
                    break;
            }

            if (isMultiInstance)
                activity.SetProperty(PropertynameIsMultiInstance, true);

            if (activity != null)
            {
                activity.Name = activityElement.GetAttributeValue("name");
                ParseActivityInputOutput(activityElement, activity);
            }

            return activity;
        }

        public virtual void ValidateActivities(IList<ActivityImpl> activities)
        {
            foreach (var activity in activities)
            {
                ValidateActivity(activity);
                // check children if it is an own scope / subprocess / ...
                if (activity.GetActivities().Count > 0)
                    ValidateActivities(activity.GetActivities());
            }
        }

        protected internal virtual void ValidateActivity(ActivityImpl activity)
        {
            if (activity.ActivityBehavior is ExclusiveGatewayActivityBehavior)
                ValidateExclusiveGateway(activity);
            ValidateOutgoingFlows(activity);
        }

        protected internal virtual void ValidateOutgoingFlows(ActivityImpl activity)
        {
            if (activity.AsyncAfter)
                foreach (IPvmTransition transition in activity.OutgoingTransitions)
                    if (ReferenceEquals(transition.Id, null))
                        AddError(
                            "Sequence flow with sourceRef='" + activity.Id + "' must have an id, activity with id '" +
                            activity.Id + "' uses 'asyncAfter'.", null);
        }

        public virtual void ValidateExclusiveGateway(ActivityImpl activity)
        {
            if (activity.OutgoingTransitions.Count == 0)
            {
                // TODO: double check if this is valid (I think in Activiti yes, since we
                // need start events we will need an end event as well)
                AddError("Exclusive Gateway '" + activity.Id + "' has no outgoing sequence flows.", null);
            }
            else if (activity.OutgoingTransitions.Count == 1)
            {
                IPvmTransition flow = activity.OutgoingTransitions[0];
                var condition = (ICondition)flow.GetProperty(PropertynameCondition);
                if (condition != null)
                    AddError(
                        "Exclusive Gateway '" + activity.Id + "' has only one outgoing sequence flow ('" + flow.Id +
                        "'). This is not allowed to have a condition.", null);
            }
            else
            {
                var defaultSequenceFlow = (string)activity.GetProperty("default");
                var hasDefaultFlow = !ReferenceEquals(defaultSequenceFlow, null) && (defaultSequenceFlow.Length > 0);

                var flowsWithoutCondition = new List<IPvmTransition>();
                foreach (IPvmTransition flow in activity.OutgoingTransitions)
                {
                    var condition = (ICondition)flow.GetProperty(PropertynameCondition);
                    var isDefaultFlow = !ReferenceEquals(flow.Id, null) && flow.Id.Equals(defaultSequenceFlow);
                    var hasConditon = condition != null;

                    if (!hasConditon && !isDefaultFlow)
                        flowsWithoutCondition.Add(flow);
                    if (hasConditon && isDefaultFlow)
                        AddError(
                            "Exclusive Gateway '" + activity.Id + "' has outgoing sequence flow '" + flow.Id +
                            "' which is the default flow but has a condition too.", null);
                }
                if (hasDefaultFlow || (flowsWithoutCondition.Count > 1))
                {
                    // if we either have a default flow (then no flows without conditions
                    // are valid at all) or if we have more than one flow without condition
                    // this is an error
                    foreach (var flow in flowsWithoutCondition)
                        AddError(
                            "Exclusive Gateway '" + activity.Id + "' has outgoing sequence flow '" + flow.Id +
                            "' without condition which is not the default flow.", null);
                }
                else if (flowsWithoutCondition.Count == 1)
                {
                    // Havinf no default and exactly one flow without condition this is
                    // considered the default one now (to not break backward compatibility)
                    var flow = flowsWithoutCondition[0];
                    AddWarning(
                        "Exclusive Gateway '" + activity.Id + "' has outgoing sequence flow '" + flow.Id +
                        "' without condition which is not the default flow. We assume it to be the default flow, but it is bad modeling practice, better set the default flow in your gateway.",
                        null);
                }
            }
        }

        public virtual ActivityImpl ParseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scopeElement,
            ActivityImpl eventBasedGateway)
        {
            var nestedActivity = CreateActivityOnScope(intermediateEventElement, scopeElement);

            var timerEventDefinition = intermediateEventElement.element(TimerEventDefinition);
            var signalEventDefinition = intermediateEventElement.element(SignalEventDefinition);
            var messageEventDefinition = intermediateEventElement.element(MessageEventDefinition);
            var linkEventDefinitionElement = intermediateEventElement.element(LinkEventDefinition);
            var conditionalEventDefinitionElement = intermediateEventElement.element(ConditionalEventDefinition);

            // shared by all events except for link event
            var defaultCatchBehaviour = new IntermediateCatchEventActivityBehavior(eventBasedGateway != null);

            ParseAsynchronousContinuationForActivity(intermediateEventElement, nestedActivity);
            var isEventBaseGatewayPresent = eventBasedGateway != null;

            if (isEventBaseGatewayPresent)
            {
                nestedActivity.EventScope = eventBasedGateway;
                nestedActivity.ActivityStartBehavior = ActivityStartBehavior.CancelEventScope;
            }
            else
            {
                nestedActivity.EventScope = nestedActivity;
                nestedActivity.IsScope = true;
            }

            nestedActivity.ActivityBehavior = defaultCatchBehaviour;
            if (timerEventDefinition != null)
            {
                ParseIntermediateTimerEventDefinition(timerEventDefinition, nestedActivity);
            }
            else if (signalEventDefinition != null)
            {
                ParseIntermediateSignalEventDefinition(signalEventDefinition, nestedActivity);
            }
            else if (messageEventDefinition != null)
            {
                ParseIntermediateMessageEventDefinition(messageEventDefinition, nestedActivity);
            }
            else if (linkEventDefinitionElement != null)
            {
                if (isEventBaseGatewayPresent)
                    AddError("IntermediateCatchLinkEvent is not allowed after an EventBasedGateway.",
                        intermediateEventElement);
                nestedActivity.ActivityBehavior =
                    new IntermediateCatchLinkEventActivityBehavior();
                ParseIntermediateLinkEventCatchBehavior(intermediateEventElement, nestedActivity,
                    linkEventDefinitionElement);
            }
            else if (conditionalEventDefinitionElement != null)
            {
                var conditionalEvent = ParseIntermediateConditionalEventDefinition(conditionalEventDefinitionElement,
                    nestedActivity);
                nestedActivity.ActivityBehavior =
                    new IntermediateConditionalEventBehavior(conditionalEvent,
                        isEventBaseGatewayPresent);
            }
            else
            {
                AddError("Unsupported intermediate catch event type", intermediateEventElement);
            }

            foreach (var parseListener in ParseListeners)
                parseListener.ParseIntermediateCatchEvent(intermediateEventElement, scopeElement, nestedActivity);

            ParseExecutionListenersOnScope(intermediateEventElement, nestedActivity);

            return nestedActivity;
        }

        protected internal virtual void ParseIntermediateLinkEventCatchBehavior(Element intermediateEventElement,
            ActivityImpl activity, Element linkEventDefinitionElement)
        {
            activity.Properties.Set(BpmnProperties.Type, ActivityTypes.IntermediateEventLink);

            var linkName = linkEventDefinitionElement.GetAttributeValue("name");
            var elementName = intermediateEventElement.GetAttributeValue("name");
            var elementId = intermediateEventElement.GetAttributeValue("id");

            if (_eventLinkTargets.ContainsKey(linkName))
            {
                //base.AddError("Multiple Intermediate Catch Events with the same link event name ('" + linkName +
                //    "') are not allowed.", intermediateEventElement);
                AddError(
                    "Multiple Intermediate Catch Events with the same link event name ('" + linkName +
                    "') are not allowed.", intermediateEventElement);
            }
            else
            {
                if (!linkName.Equals(elementName))
                    AddWarning(
                        "Link Event named '" + elementName + "' containes link event definition with name '" + linkName +
                        "' - it is recommended to use the same name for both.", intermediateEventElement);

                // now we remember the link in order to replace the sequence flow later on
                _eventLinkTargets[linkName] = elementId;
            }
        }

        protected internal virtual void ParseIntermediateMessageEventDefinition(Element messageEventDefinition,
            ActivityImpl nestedActivity)
        {
            nestedActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.IntermediateEventMessage);

            var messageDefinition = ParseMessageEventDefinition(messageEventDefinition);
            messageDefinition.ActivityId = nestedActivity.Id;
            AddEventSubscriptionDeclaration(messageDefinition, (ScopeImpl)nestedActivity.EventScope,
                messageEventDefinition);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseIntermediateMessageCatchEventDefinition(messageEventDefinition, nestedActivity);
        }

        public virtual ActivityImpl ParseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scopeElement)
        {
            var signalEventDefinitionElement = intermediateEventElement.element(SignalEventDefinition);
            var compensateEventDefinitionElement = intermediateEventElement.element(CompensateEventDefinition);
            var linkEventDefinitionElement = intermediateEventElement.element(LinkEventDefinition);
            var messageEventDefinitionElement = intermediateEventElement.element(MessageEventDefinition);
            var escalationEventDefinition = intermediateEventElement.element(EscalationEventDefinition);

            // the link event gets a special treatment as a throwing link event (event
            // source)
            // will not create any activity instance but serves as a "redirection" to
            // the catching link
            // event (event target)
            if (linkEventDefinitionElement != null)
            {
                var linkName = linkEventDefinitionElement.GetAttributeValue("name");
                var elementId = intermediateEventElement.GetAttributeValue("id");

                // now we remember the link in order to replace the sequence flow later on
                _eventLinkSources[elementId] = linkName;
                // and done - no activity created
                return null;
            }

            var nestedActivityImpl = CreateActivityOnScope(intermediateEventElement, scopeElement);
            IActivityBehavior activityBehavior = null;

            ParseAsynchronousContinuationForActivity(intermediateEventElement, nestedActivityImpl);

            if (signalEventDefinitionElement != null)
            {
                nestedActivityImpl.Properties.Set(BpmnProperties.Type, ActivityTypes.IntermediateEventSignalThrow);

                var signalDefinition = ParseSignalEventDefinition(signalEventDefinitionElement);
                activityBehavior = new ThrowSignalEventActivityBehavior(signalDefinition);
            }
            else if (compensateEventDefinitionElement != null)
            {
                nestedActivityImpl.Properties.Set(BpmnProperties.Type,
                    ActivityTypes.IntermediateEventCompensationThrow);
                var compensateEventDefinition = ParseThrowCompensateEventDefinition(compensateEventDefinitionElement,
                    scopeElement);
                activityBehavior = new CompensationEventActivityBehavior(compensateEventDefinition);
                nestedActivityImpl.SetProperty(PropertynameThrowsCompensation, true);
                nestedActivityImpl.IsScope = true;
            }
            else if (messageEventDefinitionElement != null)
            {
                if (IsServiceTaskLike(messageEventDefinitionElement))
                {
                    // CAM-436 same behavior as service ITask
                    nestedActivityImpl.Properties.Set(BpmnProperties.Type,
                        ActivityTypes.IntermediateEventMessageThrow);
                    activityBehavior =
                        ParseServiceTaskLike(ActivityTypes.IntermediateEventMessageThrow, messageEventDefinitionElement, scopeElement).ActivityBehavior;
                }
                else
                {
                    // default to non behavior if no service ITask
                    // properties have been specified
                    nestedActivityImpl.Properties.Set(BpmnProperties.Type, ActivityTypes.IntermediateEventNoneThrow);
                    activityBehavior = new IntermediateThrowNoneEventActivityBehavior();
                }
            }
            else if (escalationEventDefinition != null)
            {
                nestedActivityImpl.Properties.Set(BpmnProperties.Type, ActivityTypes.IntermediateEventEscalationThrow);

                var escalation = FindEscalationForEscalationEventDefinition(escalationEventDefinition);
                if ((escalation != null) && ReferenceEquals(escalation.EscalationCode, null))
                    AddError("throwing escalation event must have an 'escalationCode'", escalationEventDefinition);

                activityBehavior = new ThrowEscalationEventActivityBehavior(escalation);
            }
            else
            {
                // None intermediate event
                nestedActivityImpl.Properties.Set(BpmnProperties.Type, ActivityTypes.IntermediateEventNoneThrow);
                activityBehavior = new IntermediateThrowNoneEventActivityBehavior();
            }

            foreach (var parseListener in ParseListeners)
                parseListener.ParseIntermediateThrowEvent(intermediateEventElement, scopeElement, nestedActivityImpl);

            nestedActivityImpl.ActivityBehavior = activityBehavior;

            ParseExecutionListenersOnScope(intermediateEventElement, nestedActivityImpl);

            return nestedActivityImpl;
        }

        protected internal virtual CompensateEventDefinition ParseThrowCompensateEventDefinition(
            Element compensateEventDefinitionElement, ScopeImpl scopeElement)
        {
            var activityRef = compensateEventDefinitionElement.GetAttributeValue("activityRef");
            var waitForCompletion =
                True.Equals(compensateEventDefinitionElement.GetAttributeValue("waitForCompletion", True));

            if (!ReferenceEquals(activityRef, null))
                if (scopeElement.FindActivityAtLevelOfSubprocess(activityRef) == null)
                {
                    bool? isTriggeredByEvent = scopeElement.Properties.Get(BpmnProperties.TriggeredByEvent);
                    var type = (string)scopeElement.GetProperty(PropertynameType);
                    if ((true == isTriggeredByEvent) && "subProcess".Equals(type))
                        scopeElement = scopeElement.FlowScope;
                    if (scopeElement.FindActivityAtLevelOfSubprocess(activityRef) == null)
                    {
                        var scopeId = scopeElement.Id;
                        scopeElement.AddToBacklog(activityRef,
                            new BacklogErrorCallbackAnonymousInnerClass(this, compensateEventDefinitionElement,
                                activityRef, scopeId));
                    }
                }

            var compensateEventDefinition = new CompensateEventDefinition();
            compensateEventDefinition.ActivityRef = activityRef;

            compensateEventDefinition.WaitForCompletion = waitForCompletion;
            if (!waitForCompletion)
                AddWarning(
                    "Unsupported attribute value for 'waitForCompletion': 'waitForCompletion=false' is not supported. Compensation event will wait for compensation to join.",
                    compensateEventDefinitionElement);

            return compensateEventDefinition;
        }

        protected internal virtual void ValidateCatchCompensateEventDefinition(Element compensateEventDefinitionElement)
        {
            var activityRef = compensateEventDefinitionElement.GetAttributeValue("activityRef");
            if (!ReferenceEquals(activityRef, null))
                AddWarning(
                    "attribute 'activityRef' is not supported on catching compensation event. attribute will be ignored",
                    compensateEventDefinitionElement);

            var waitForCompletion = compensateEventDefinitionElement.GetAttributeValue("waitForCompletion");
            if (!ReferenceEquals(waitForCompletion, null))
                AddWarning(
                    "attribute 'waitForCompletion' is not supported on catching compensation event. attribute will be ignored",
                    compensateEventDefinitionElement);
        }

        protected internal virtual void ParseBoundaryCompensateEventDefinition(Element compensateEventDefinition,
            ActivityImpl activity)
        {
            activity.Properties.Set(BpmnProperties.Type, ActivityTypes.BoundaryCompensation);

            var hostActivity = (ScopeImpl)activity.EventScope;
            foreach (ActivityImpl sibling in activity.FlowScope.Activities)
                if (sibling.GetProperty(BpmnProperties.Type.Name).Equals("compensationBoundaryCatch") &&
                    sibling.EventScope.Equals(hostActivity) && (sibling != activity))
                    AddError("multiple boundary events with compensateEventDefinition not supported on same activity",
                        compensateEventDefinition);

            ValidateCatchCompensateEventDefinition(compensateEventDefinition);
        }

        protected internal virtual IActivityBehavior ParseBoundaryCancelEventDefinition(Element cancelEventDefinition,
            ActivityImpl activity)
        {
            activity.Properties.Set(BpmnProperties.Type, ActivityTypes.BoundaryCancel);

            LegacyBehavior.ParseCancelBoundaryEvent(activity);

            var transaction = (ActivityImpl)activity.EventScope;
            if ((transaction.ActivityBehavior != null) && transaction.ActivityBehavior is MultiInstanceActivityBehavior)
                transaction = (ActivityImpl)transaction.Activities[0];

            if ("transaction" != transaction.GetProperty(BpmnProperties.Type.Name).ToString())
                AddError("boundary event with cancelEventDefinition only supported on transaction subprocesses",
                    cancelEventDefinition);

            // ensure there is only one cancel boundary event
            foreach (ActivityImpl sibling in activity.FlowScope.Activities)
                if ("cancelBoundaryCatch".Equals(sibling.GetProperty(BpmnProperties.Type.Name)) && (sibling != activity) &&
                    (sibling.EventScope == transaction))
                    AddError(
                        "multiple boundary events with cancelEventDefinition not supported on same transaction subprocess",
                        cancelEventDefinition);

            // find all cancel end events
            foreach (ActivityImpl childActivity in transaction.Activities)
            {
                var activityBehavior = childActivity.ActivityBehavior;
                if ((activityBehavior != null) && activityBehavior is CancelEndEventActivityBehavior)
                    ((CancelEndEventActivityBehavior)activityBehavior).CancelBoundaryEvent = activity;
            }

            return new CancelBoundaryEventActivityBehavior();
        }

        /// <summary>
        ///     Parses loopCharacteristics (standardLoop/Multi-instance) of an activity, if
        ///     any is defined.
        /// </summary>
        public virtual ScopeImpl ParseMultiInstanceLoopCharacteristics(Element activityElement, ScopeImpl scope)
        {
            var miLoopCharacteristics = activityElement.element("multiInstanceLoopCharacteristics");
            if (miLoopCharacteristics == null)
                return null;
            var id = activityElement.GetAttributeValue("id");

            Log.ParsingElement("mi body for activity", id);

            id = GetIdForMiBody(id);
            var miBodyScope = (ActivityImpl)scope.CreateActivity(id);
            ActivityAsyncDelegates = miBodyScope;
            miBodyScope.SetProperty(PropertynameType, ActivityTypes.MultiInstanceBody);
            miBodyScope.IsScope = true;

            var isSequential =
                ParseBooleanAttribute(miLoopCharacteristics.GetAttributeValue("isSequential"), false).Value;

            MultiInstanceActivityBehavior behavior = null;
            if (isSequential)
                behavior = new SequentialMultiInstanceActivityBehavior();
            else
                behavior = new ParallelMultiInstanceActivityBehavior();
            miBodyScope.SetActivityBehavior(behavior);//as ICoreActivityBehavior<IBaseDelegateExecution>;

            // loopCardinality
            var loopCardinality = miLoopCharacteristics.element("loopCardinality");
            if (loopCardinality != null)
            {
                var loopCardinalityText = loopCardinality.Text;
                if (string.IsNullOrEmpty(loopCardinalityText))
                    AddError("loopCardinality must be defined for a multiInstanceLoopCharacteristics definition ",
                        miLoopCharacteristics);
                behavior.LoopCardinalityExpression = ExpressionManager.CreateExpression(loopCardinalityText);
            }

            // completionCondition
            var completionCondition = miLoopCharacteristics.element("completionCondition");
            if (completionCondition != null)
            {
                var completionConditionText = completionCondition.Text;
                behavior.CompletionConditionExpression = ExpressionManager.CreateExpression(completionConditionText);
            }

            // activiti:collection
            var collection = miLoopCharacteristics.GetAttributeNS(CamundaBpmnExtensionsNs,
                "collection");
            if (!ReferenceEquals(collection, null))
                if (collection.Contains("{"))
                    behavior.CollectionExpression = ExpressionManager.CreateExpression(collection);
                else
                    behavior.CollectionVariable = collection;

            // loopDataInputRef
            var loopDataInputRef = miLoopCharacteristics.element("loopDataInputRef");
            if (loopDataInputRef != null)
            {
                var loopDataInputRefText = loopDataInputRef.Text;
                if (!ReferenceEquals(loopDataInputRefText, null))
                    if (loopDataInputRefText.Contains("{"))
                        behavior.CollectionExpression = ExpressionManager.CreateExpression(loopDataInputRefText);
                    else
                        behavior.CollectionVariable = loopDataInputRefText;
            }

            // activiti:elementVariable
            var elementVariable = miLoopCharacteristics.GetAttributeNS(CamundaBpmnExtensionsNs,
                "elementVariable");
            if (!ReferenceEquals(elementVariable, null))
                behavior.CollectionElementVariable = elementVariable;

            // dataInputItem
            var inputDataItem = miLoopCharacteristics.element("inputDataItem");
            if (inputDataItem != null)
            {
                var inputDataItemName = inputDataItem.GetAttributeValue("name");
                behavior.CollectionElementVariable = inputDataItemName;
            }

            // Validation
            if ((behavior.LoopCardinalityExpression == null) && (behavior.CollectionExpression == null) &&
                ReferenceEquals(behavior.CollectionVariable, null))
                AddError("Either loopCardinality or loopDataInputRef/activiti:collection must been set",
                    miLoopCharacteristics);

            // Validation
            if ((behavior.CollectionExpression == null) && ReferenceEquals(behavior.CollectionVariable, null) &&
                !ReferenceEquals(behavior.CollectionElementVariable, null))
                AddError(
                    "LoopDataInputRef/activiti:collection must be set when using inputDataItem or activiti:elementVariable",
                    miLoopCharacteristics);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseMultiInstanceLoopCharacteristics(activityElement, miLoopCharacteristics, miBodyScope);

            return miBodyScope;
        }

        public static string GetIdForMiBody(string id)
        {
            return id + MultiInstanceBodyIdSuffix;
        }

        /// <summary>
        ///     Parses the generic information of an activity element (id, name,
        ///     documentation, etc.), and creates a new <seealso cref="ActivityImpl" /> on the given
        ///     scope element.
        /// </summary>
        public virtual ActivityImpl CreateActivityOnScope(Element activityElement, ScopeImpl scopeElement)
        {
            var id = activityElement.GetAttributeValue("id");

            Log.ParsingElement("activity", id);
            var activity = (ActivityImpl)scopeElement.CreateActivity(id);

            activity.SetProperty("name", activityElement.GetAttributeValue("name"));
            activity.SetProperty("documentation", ParseDocumentation(activityElement));
            activity.SetProperty("default", activityElement.GetAttributeValue("default"));
            activity.Properties.Set(BpmnProperties.Type, activityElement.TagName);
            activity.SetProperty("line", activityElement.Line);
            ActivityAsyncDelegates = activity;
            //TODO 表达式解析，后期处理
            activity.SetProperty(PropertynameJobPriority, ParsePriority(activityElement, PropertynameJobPriority));

            if (isCompensationHandler(activityElement))
            {
                activity.SetProperty(PropertynameIsForCompensation, true);
            }

            return activity;
        }

        /// <summary>
        ///     Adds the new message job declaration to existing declarations.
        ///     There will be executed an existing check before the adding is executed.
        /// </summary>
        /// <param name="messageJobDeclaration"> the new message job declaration </param>
        /// <param name="activity"> the corresponding activity </param>
        /// <param name="exclusive"> the flag which indicates if the async should be exclusive </param>
        protected internal virtual void AddMessageJobDeclaration(MessageJobDeclaration messageJobDeclaration,
            ActivityImpl activity, bool exclusive)
        {
            var procDef = (IProcessDefinition)activity.ProcessDefinition;
            if (!Exists(messageJobDeclaration, procDef.Key, activity.ActivityId))
            {
                messageJobDeclaration.Exclusive = exclusive;
                messageJobDeclaration.Activity = activity;
                //TODO JobPriorityProvider
                messageJobDeclaration.JobPriorityProvider =
                    (IParameterValueProvider)activity.GetProperty(PropertynameJobPriority);

                AddMessageJobDeclarationToActivity(messageJobDeclaration, activity);

                AddJobDeclarationToProcessDefinition(messageJobDeclaration, procDef);
            }
        }

        /// <summary>
        ///     Checks whether the message declaration already exists.
        /// </summary>
        /// <param name="msgJobdecl"> the message job declaration which is searched </param>
        /// <param name="procDefKey"> the corresponding process definition key </param>
        /// <param name="activityId"> the corresponding activity id </param>
        /// <returns> true if the message job declaration exists, false otherwise </returns>
        protected internal virtual bool Exists(MessageJobDeclaration msgJobdecl, string procDefKey, string activityId)
        {
            var exist = false;
            var declarations = jobDeclarations.ContainsKey(procDefKey) ? jobDeclarations[procDefKey] : null;
            if (declarations != null)
                for (var i = 0; (i < declarations.Count) && !exist; i++)
                {
                    var decl = declarations[i];
                    if (decl.ActivityId.Equals(activityId) &&
                        decl.JobConfiguration.Equals(msgJobdecl.JobConfiguration, StringComparison.CurrentCultureIgnoreCase))
                        exist = true;
                }
            return exist;
        }

        protected internal virtual void RemoveMessageJobDeclarationWithJobConfiguration(ActivityImpl activity, string jobConfiguration)
        {
            List<MessageJobDeclaration> messageJobDeclarations = (List<MessageJobDeclaration>)activity.GetProperty(PropertynameMessageJobDeclaration);

            messageJobDeclarations?.RemoveAll(
                c => c.jobConfiguration.Equals(jobConfiguration, StringComparison.CurrentCultureIgnoreCase) &&
                     c.ActivityId.Equals(activity.ActivityId, StringComparison.CurrentCultureIgnoreCase));


            var procDef = (IProcessDefinition)activity.ProcessDefinition;
            var declarations = jobDeclarations.ContainsKey(procDef.Key) ? (List<IJobDeclaration>)jobDeclarations[procDef.Key] : null;

            declarations?.RemoveAll(
                c => c.JobConfiguration.Equals(jobConfiguration, StringComparison.CurrentCultureIgnoreCase) &&
                     c.ActivityId.Equals(activity.ActivityId, StringComparison.CurrentCultureIgnoreCase));
        }

        public virtual string ParseDocumentation(Element element)
        {
            var docElements = element.Elements("documentation");
            IList<string> docStrings = new List<string>();
            foreach (var e in docElements)
                docStrings.Add(e.Text);

            return ParseDocumentation(docStrings);
        }

        public static string ParseDocumentation(IList<string> docStrings)
        {
            if (docStrings.Count == 0)
                return null;

            var builder = new StringBuilder();
            foreach (var e in docStrings)
            {
                if (builder.Length != 0)
                    builder.Append("\n\n");

                builder.Append(e.Trim());
            }

            return builder.ToString();
        }

        protected internal virtual bool isCompensationHandler(Element activityElement)
        {
            var isForCompensation = activityElement.GetAttributeValue("isForCompensation");
            return !ReferenceEquals(isForCompensation, null) &&
                   isForCompensation.Equals(True, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        ///     Parses an exclusive gateway declaration.
        /// </summary>
        public virtual ActivityImpl ParseExclusiveGateway(Element exclusiveGwElement, ScopeImpl scope)
        {
            var activity = CreateActivityOnScope(exclusiveGwElement, scope);
            activity.ActivityBehavior =
                new ExclusiveGatewayActivityBehavior();

            ParseAsynchronousContinuationForActivity(exclusiveGwElement, activity);

            ParseExecutionListenersOnScope(exclusiveGwElement, activity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseExclusiveGateway(exclusiveGwElement, scope, activity);
            return activity;
        }

        /// <summary>
        ///     Parses an inclusive gateway declaration.
        /// </summary>
        public virtual ActivityImpl ParseInclusiveGateway(Element inclusiveGwElement, ScopeImpl scope)
        {
            var activity = CreateActivityOnScope(inclusiveGwElement, scope);
            activity.ActivityBehavior =
                new InclusiveGatewayActivityBehavior();

            ParseAsynchronousContinuationForActivity(inclusiveGwElement, activity);

            ParseExecutionListenersOnScope(inclusiveGwElement, activity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseInclusiveGateway(inclusiveGwElement, scope, activity);
            return activity;
        }

        public virtual ActivityImpl ParseEventBasedGateway(Element eventBasedGwElement, Element parentElement,
            ScopeImpl scope)
        {
            var activity = CreateActivityOnScope(eventBasedGwElement, scope);
            activity.ActivityBehavior =
                new EventBasedGatewayActivityBehavior();
            activity.IsScope = true;

            ParseAsynchronousContinuationForActivity(eventBasedGwElement, activity);

            if (activity.AsyncAfter)
                AddError("'asyncAfter' not supported for " + eventBasedGwElement.TagName + " elements.",
                    eventBasedGwElement);

            ParseExecutionListenersOnScope(eventBasedGwElement, activity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseEventBasedGateway(eventBasedGwElement, scope, activity);

            // find all outgoing sequence flows:
            var sequenceFlows = parentElement.Elements("sequenceFlow");

            // collect all siblings in a map
            IDictionary<string, Element> siblingsMap = new Dictionary<string, Element>();
            var siblings = parentElement.GetAllElement();
            foreach (var sibling in siblings)
            {
                if (!string.IsNullOrEmpty(sibling.GetAttributeValue("id")))
                {
                    siblingsMap[sibling.GetAttributeValue("id")] = sibling;
                }
            }


            foreach (var sequenceFlow in sequenceFlows)
            {
                var sourceRef = sequenceFlow.GetAttributeValue("sourceRef");
                var targetRef = sequenceFlow.GetAttributeValue("targetRef");

                if (activity.Id.Equals(sourceRef))
                {
                    var sibling = siblingsMap[targetRef];
                    if (sibling != null)
                        if (sibling.TagName.Equals(ActivityTypes.IntermediateEventCatch, StringComparison.OrdinalIgnoreCase))
                            ParseIntermediateCatchEvent(sibling, scope, activity);
                        else
                            AddError(
                                "Event based gateway can only be connected to elements of type intermediateCatchEvent",
                                sibling);
                }
            }

            return activity;
        }

        /// <summary>
        ///     Parses a parallel gateway declaration.
        /// </summary>
        public virtual ActivityImpl ParseParallelGateway(Element parallelGwElement, ScopeImpl scope)
        {
            var activity = CreateActivityOnScope(parallelGwElement, scope);
            activity.ActivityBehavior =
                new ParallelGatewayActivityBehavior();

            ParseAsynchronousContinuationForActivity(parallelGwElement, activity);

            ParseExecutionListenersOnScope(parallelGwElement, activity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseParallelGateway(parallelGwElement, scope, activity);
            return activity;
        }

        /// <summary>
        ///     Parses a scriptTask declaration.
        /// </summary>
        public virtual ActivityImpl ParseScriptTask(Element scriptTaskElement, ScopeImpl scope)
        {
            var activity = CreateActivityOnScope(scriptTaskElement, scope);

            var activityBehavior = ParseScriptTaskElement(scriptTaskElement);

            if (activityBehavior != null)
            {
                ParseAsynchronousContinuationForActivity(scriptTaskElement, activity);

                activity.ActivityBehavior = activityBehavior;

                ParseExecutionListenersOnScope(scriptTaskElement, activity);

                foreach (var parseListener in ParseListeners)
                    parseListener.ParseScriptTask(scriptTaskElement, scope, activity);
            }

            return activity;
        }

        /// <summary>
        ///     Returns a <seealso cref="ScriptTaskActivityBehavior" /> for the script ITask element
        ///     corresponding to the script source or resource specified.
        /// </summary>
        /// <param name="scriptTaskElement">
        ///     the script ITask element
        /// </param>
        /// <returns> the corresponding <seealso cref="ScriptTaskActivityBehavior" /> </returns>
        protected internal virtual ScriptTaskActivityBehavior ParseScriptTaskElement(Element scriptTaskElement)
        {
            // determine script language
            var language = scriptTaskElement.GetAttributeValue("scriptFormat");
            if (language == null)
            {
                language = "juel"; //ScriptingEngines.DEFAULT_SCRIPTING_LANGUAGE;
            }
            if (language == "groovy")
            {
                throw new NotSupportedException(".net不支持groovy语法,");
            }
            var resultVariableName = ParseResultVariable(scriptTaskElement);

            // determine script source
            string scriptSource = null;
            var scriptElement = scriptTaskElement.element("script");
            if (scriptElement != null)
                scriptSource = scriptElement.Text;
            var scriptResource = scriptTaskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                PropertynameResource);

            try
            {
                ExecutableScript script = ScriptUtil.GetScript(language, scriptSource, scriptResource, ExpressionManager);
                return new ScriptTaskActivityBehavior(script, resultVariableName);
            }
            catch (ProcessEngineException e)
            {
                AddError("Unable to process ScriptTask: " + e.Message, scriptElement);
                return null;
            }
            catch(System.Exception ex)
            {
                throw ex;
            }
        }

        protected internal virtual string ParseResultVariable(Element element)
        {
            // determine if result variable exists
            var resultVariableName = element.GetAttributeNS(CamundaBpmnExtensionsNs, "resultVariable");
            if (ReferenceEquals(resultVariableName, null))
                resultVariableName = element.GetAttributeNS(CamundaBpmnExtensionsNs,
                    "resultVariableName");
            return resultVariableName;
        }

        /// <summary>
        ///     Parses a serviceTask declaration.
        /// </summary>
        public virtual ActivityImpl ParseServiceTask(Element serviceTaskElement, ScopeImpl scope)
        {
            return ParseServiceTaskLike("serviceTask", serviceTaskElement, scope);
        }

        public virtual ActivityImpl ParseServiceTaskLike(string elementName, Element serviceTaskElement, ScopeImpl scope)
        {
            var activity = CreateActivityOnScope(serviceTaskElement, scope);

            var type = serviceTaskElement.GetAttributeNS(CamundaBpmnExtensionsNs, Type);
            var className = serviceTaskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                PropertynameClass);
            var methodName = serviceTaskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                PropertynameMethod);
            var expression = serviceTaskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                PropertynameExpression);
            var delegateExpression = serviceTaskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                PropertynameDelegateExpression);
            var resultVariableName = ParseResultVariable(serviceTaskElement);

            ParseAsynchronousContinuationForActivity(serviceTaskElement, activity);

            if (!ReferenceEquals(type, null))
            {
                if (type.Equals("mail", StringComparison.CurrentCultureIgnoreCase))
                    ParseEmailServiceTask(activity, serviceTaskElement, ParseFieldDeclarations(serviceTaskElement));
                else if (type.Equals("shell", StringComparison.CurrentCultureIgnoreCase))
                    ParseShellServiceTask(activity, serviceTaskElement, ParseFieldDeclarations(serviceTaskElement));
                else if (type.Equals("external", StringComparison.CurrentCultureIgnoreCase))
                    ParseExternalServiceTask(activity, serviceTaskElement);
                else
                    AddError("Invalid usage of type attribute on " + elementName + ": '" + type + "'",
                        serviceTaskElement);
            }
            else if (!ReferenceEquals(className, null) && (className.Trim().Length > 0))
            {
                if (!ReferenceEquals(resultVariableName, null))
                    AddError("'resultVariableName' not supported for " + elementName + " elements using 'class'",
                        serviceTaskElement);
                activity.SetActivityBehavior(new ClassDelegateActivityBehavior(className, ParseFieldDeclarations(serviceTaskElement)));
            }
            else if (!ReferenceEquals(methodName, null) && (methodName.Trim().Length > 0))
            {
                activity.SetActivityBehavior(new ClassMethodDelegateActivityBehavior(methodName, ParseFieldDeclarations(serviceTaskElement), resultVariableName));
            }
            else if (!ReferenceEquals(delegateExpression, null))
            {
                if (!ReferenceEquals(resultVariableName, null))
                    AddError(
                        "'resultVariableName' not supported for " + elementName + " elements using 'delegateExpression'",
                        serviceTaskElement);
                activity.SetActivityBehavior(new ServiceTaskDelegateExpressionActivityBehavior(ExpressionManager.CreateExpression(delegateExpression), ParseFieldDeclarations(serviceTaskElement)));
            }
            else if (!ReferenceEquals(expression, null) && (expression.Trim().Length > 0))
            {
                activity.SetActivityBehavior(new ServiceTaskExpressionActivityBehavior(ExpressionManager.CreateExpression(expression),
                        resultVariableName));
            }

            ParseExecutionListenersOnScope(serviceTaskElement, activity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseServiceTask(serviceTaskElement, scope, activity);

            // activity behavior could be set by a listener (e.g. connector); thus,
            // check is after listener invocation
            if (activity.ActivityBehavior == null)
                AddError(
                    "One of the attributes 'class', 'delegateExpression', 'type', or 'expression' is mandatory on " +
                    elementName + ".", serviceTaskElement);

            return activity;
        }

        /// <summary>
        ///     Parses a businessRuleTask declaration.
        /// </summary>
        public virtual ActivityImpl ParseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope)
        {
            var decisionRef = businessRuleTaskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                "decisionRef");
            if (!ReferenceEquals(decisionRef, null))
                return ParseDmnBusinessRuleTask(businessRuleTaskElement, scope);
            return ParseServiceTaskLike("businessRuleTask", businessRuleTaskElement, scope);
        }

        /// <summary>
        ///     Parse a Business Rule ITask which references a decision.
        /// </summary>
        protected internal virtual ActivityImpl ParseDmnBusinessRuleTask(Element businessRuleTaskElement,
            ScopeImpl scope)
        {
            var activity = CreateActivityOnScope(businessRuleTaskElement, scope);
            // the activity is a scope since the result variable is stored as local variable
            activity.IsScope = true;

            ParseAsynchronousContinuationForActivity(businessRuleTaskElement, activity);

            var decisionRef = businessRuleTaskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                "decisionRef");

            var callableElement = new BaseCallableElement();
            callableElement.DeploymentId = DeploymentRenamed.Id;

            var definitionKeyProvider = CreateParameterValueProvider(decisionRef, ExpressionManager);
            callableElement.DefinitionKeyValueProvider = definitionKeyProvider;

            ParseBinding(businessRuleTaskElement, activity, callableElement, "decisionRefBinding");
            ParseVersion(businessRuleTaskElement, activity, callableElement, "decisionRefBinding", "decisionRefVersion");
            ParseTenantId(businessRuleTaskElement, activity, callableElement, "decisionRefTenantId");

            var resultVariable = ParseResultVariable(businessRuleTaskElement);
            var decisionResultMapper = ParseDecisionResultMapper(businessRuleTaskElement);

            var behavior = new DmnBusinessRuleTaskActivityBehavior(callableElement, resultVariable, decisionResultMapper);
            activity.ActivityBehavior = behavior;

            ParseExecutionListenersOnScope(businessRuleTaskElement, activity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseBusinessRuleTask(businessRuleTaskElement, scope, activity);

            return activity;
        }

        protected internal virtual IDecisionResultMapper ParseDecisionResultMapper(Element businessRuleTaskElement)
        {
            // default mapper is 'resultList'
            var decisionResultMapper = businessRuleTaskElement.GetAttributeNS(
                CamundaBpmnExtensionsNs,
                "mapDecisionResult");
            var mapper = DecisionEvaluationUtil.GetDecisionResultMapperForName(decisionResultMapper);

            if (mapper == null)
                AddError(
                    "No decision result mapper found for name '" + decisionResultMapper +
                    "'. Supported mappers are 'singleEntry', 'singleResult', 'collectEntries' and 'resultList'.",
                    businessRuleTaskElement);

            return mapper;
        }

        /// <summary>
        ///     Parse async continuation of an activity and create async jobs for the activity.
        ///     <br /> <br />
        ///     When the activity is marked as multi instance, then async jobs create instead for the multi instance body.
        ///     When the wrapped activity has async characteristics in 'multiInstanceLoopCharacteristics' element,
        ///     then async jobs create additionally for the wrapped activity.
        /// </summary>
        protected internal virtual void ParseAsynchronousContinuationForActivity(Element activityElement,
            ActivityImpl activity)
        {
            // can't use #getMultiInstanceScope here to determine whether the Task is multi-instance,
            // since the property hasn't been set yet (cf parseActivity)
            var parentFlowScopeActivity = activity.ParentFlowScopeActivity;
            if ((parentFlowScopeActivity != null) &&
                parentFlowScopeActivity.ActivityBehavior is MultiInstanceActivityBehavior &&
                !activity.CompensationHandler)
            {
                ParseAsynchronousContinuation(activityElement, parentFlowScopeActivity);

                var miLoopCharacteristics = activityElement.element("multiInstanceLoopCharacteristics");
                ParseAsynchronousContinuation(miLoopCharacteristics, activity);
            }
            else
            {
                ParseAsynchronousContinuation(activityElement, activity);
            }
        }

        /// <summary>
        ///     Parse async continuation of the given element and create async jobs for the activity.
        /// </summary>
        /// <param name="element"> with async characteristics </param>
        /// <param name="activity"> </param>
        protected internal virtual void ParseAsynchronousContinuation(Element element, ActivityImpl activity)
        {
            bool isAsyncBefore = IsAsyncBefore(element);
            bool isAsyncAfter = IsAsyncAfter(element);
            bool isExclusive = IsExclusive(element);

            //// set properties on activity
            activity.SetAsyncBefore(isAsyncBefore, isExclusive);
            activity.SetAsyncAfter(isAsyncAfter, isExclusive);
        }

        /// <summary>
        ///     解析数据优先级
        /// </summary>
        /// <param name="element"></param>
        /// <param name="priorityAttribute"></param>
        /// <returns></returns>
        protected internal virtual IParameterValueProvider ParsePriority(Element element, string priorityAttribute)
        {
            var priorityAttributeValue = element.GetAttributeNS(CamundaBpmnExtensionsNs,
                priorityAttribute);

            if (ReferenceEquals(priorityAttributeValue, null))
                return null;
            if (!StringUtil.IsExpression(priorityAttributeValue))
                try
                {
                    int.Parse(priorityAttributeValue);
                }
                catch (FormatException)
                {
                    AddError(
                        "Value '" + priorityAttributeValue + "' for attribute '" + priorityAttribute +
                        "' is not a valid number", element);
                }
            //value解析成int失败 为默认字符串jobPriority
            return CreateParameterValueProvider(priorityAttributeValue, ExpressionManager);
        }

        protected internal virtual void AddMessageJobDeclarationToActivity(MessageJobDeclaration messageJobDeclaration,
            ActivityImpl activity)
        {
            var messageJobDeclarations =
                (IList<MessageJobDeclaration>)activity.GetProperty(PropertynameMessageJobDeclaration);
            if (messageJobDeclarations == null)
            {
                messageJobDeclarations = new List<MessageJobDeclaration>();
                activity.SetProperty(PropertynameMessageJobDeclaration, messageJobDeclarations);
            }
            messageJobDeclarations.Add(messageJobDeclaration);
        }
        //JobDeclarations
        protected internal virtual void AddJobDeclarationToProcessDefinition(/*JobDeclaration<JobEntity>*/ IJobDeclaration jobDeclaration, IProcessDefinition processDefinition)
        {
            var key = processDefinition.Key;

            var containingJobDeclarations = jobDeclarations.ContainsKey(key) ? jobDeclarations[key] : null;
            if (containingJobDeclarations == null)
            {
                containingJobDeclarations = new List<IJobDeclaration>();
                jobDeclarations[key] = containingJobDeclarations;
            }

            containingJobDeclarations.Add(jobDeclaration);
        }

        /// <summary>
        ///     Parses a sendTask declaration.
        /// </summary>
        public virtual ActivityImpl ParseSendTask(Element sendTaskElement, ScopeImpl scope)
        {
            if (IsServiceTaskLike(sendTaskElement))
                return ParseServiceTaskLike("sendTask", sendTaskElement, scope);
            var activity = CreateActivityOnScope(sendTaskElement, scope);

            ParseAsynchronousContinuationForActivity(sendTaskElement, activity);
            ParseExecutionListenersOnScope(sendTaskElement, activity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseSendTask(sendTaskElement, scope, activity);

            // activity behavior could be set by a listener; thus, check is after listener invocation
            if (activity.ActivityBehavior == null)
                AddError(
                    "One of the attributes 'class', 'delegateExpression', 'type', or 'expression' is mandatory on sendTask.",
                    sendTaskElement);

            return activity;
        }

        protected internal virtual void ParseEmailServiceTask(ActivityImpl activity, Element serviceTaskElement,
            IList<FieldDeclaration> fieldDeclarations)
        {
            ValidateFieldDeclarationsForEmail(serviceTaskElement, fieldDeclarations);
            throw new NotImplementedException();
            //activity.ActivityBehavior =(IActivityBehavior)ClassDelegateUtil.InstantiateDelegate(typeof(MailActivityBehavior), fieldDeclarations);
        }

        protected internal virtual void ParseShellServiceTask(ActivityImpl activity, Element serviceTaskElement,
            IList<FieldDeclaration> fieldDeclarations)
        {
            ValidateFieldDeclarationsForShell(serviceTaskElement, fieldDeclarations);
            activity.SetActivityBehavior(ClassDelegateUtil.InstantiateDelegate(typeof(ShellActivityBehavior), fieldDeclarations) as IActivityBehavior);
            //    (IActivityBehavior)
            //        ClassDelegateUtil.InstantiateDelegate(typeof(ShellActivityBehavior), fieldDeclarations);
        }

        protected internal virtual void ParseExternalServiceTask(ActivityImpl activity, Element serviceTaskElement)
        {
            activity.IsScope = true;

            var topicName = serviceTaskElement.GetAttributeNS(CamundaBpmnExtensionsNs, "topic");
            if (ReferenceEquals(topicName, null))
                AddError("External tasks must specify a 'topic' attribute in the camunda namespace", serviceTaskElement);

            var provider = ParsePriority(serviceTaskElement, PropertynameTaskPriority);
            activity.ActivityBehavior =
               new ExternalTaskActivityBehavior(topicName, provider);
        }

        protected internal virtual void ValidateFieldDeclarationsForEmail(Element serviceTaskElement,
            IList<FieldDeclaration> fieldDeclarations)
        {
            var toDefined = false;
            var textOrHtmlDefined = false;
            foreach (var fieldDeclaration in fieldDeclarations)
            {
                if (fieldDeclaration.Name.Equals("to"))
                    toDefined = true;
                if (fieldDeclaration.Name.Equals("html"))
                    textOrHtmlDefined = true;
                if (fieldDeclaration.Name.Equals("text"))
                    textOrHtmlDefined = true;
            }

            if (!toDefined)
                AddError("No recipient is defined on the mail activity", serviceTaskElement);
            if (!textOrHtmlDefined)
                AddError("Text or html field should be provided", serviceTaskElement);
        }

        protected internal virtual void ValidateFieldDeclarationsForShell(Element serviceTaskElement,
            IList<FieldDeclaration> fieldDeclarations)
        {
            var shellCommandDefined = false;

            foreach (var fieldDeclaration in fieldDeclarations)
            {
                var fieldName = fieldDeclaration.Name;
                var fieldFixedValue = (FixedValue)fieldDeclaration.Value;
                var fieldValue = fieldFixedValue.ExpressionText;

                shellCommandDefined |= fieldName.Equals("command");

                if ((fieldName.Equals("wait") || fieldName.Equals("redirectError") || fieldName.Equals("cleanEnv")) &&
                    !fieldValue.Equals(True, StringComparison.CurrentCultureIgnoreCase) && !fieldValue.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                    AddError("undefined value for shell " + fieldName + " parameter :" + fieldValue, serviceTaskElement);
            }

            if (!shellCommandDefined)
                AddError("No shell command is defined on the shell activity", serviceTaskElement);
        }

        public virtual IList<FieldDeclaration> ParseFieldDeclarations(Element element)
        {
            IList<FieldDeclaration> fieldDeclarations = new List<FieldDeclaration>();

            var elementWithFieldInjections = element.element("extensionElements");
            if (elementWithFieldInjections == null)
                elementWithFieldInjections = element;
            var fieldDeclarationElements = elementWithFieldInjections.ElementsNS(
                CamundaBpmnExtensionsNs, "field");
            if ((fieldDeclarationElements != null) && (fieldDeclarationElements.Count > 0))
                foreach (var fieldDeclarationElement in fieldDeclarationElements)
                {
                    var fieldDeclaration = ParseFieldDeclaration(element, fieldDeclarationElement);
                    if (fieldDeclaration != null)
                        fieldDeclarations.Add(fieldDeclaration);
                }

            return fieldDeclarations;
        }

        protected internal virtual FieldDeclaration ParseFieldDeclaration(Element serviceTaskElement,
            Element fieldDeclarationElement)
        {
            var fieldName = fieldDeclarationElement.GetAttributeValue("name");

            var fieldDeclaration = ParseStringFieldDeclaration(fieldDeclarationElement, serviceTaskElement, fieldName);
            if (fieldDeclaration == null)
                fieldDeclaration = ParseExpressionFieldDeclaration(fieldDeclarationElement, serviceTaskElement,
                    fieldName);

            if (fieldDeclaration == null)
                AddError(
                    "One of the following is mandatory on a field declaration: one of attributes stringValue|expression " +
                    "or one of child elements string|expression", serviceTaskElement);
            return fieldDeclaration;
        }

        protected internal virtual FieldDeclaration ParseStringFieldDeclaration(Element fieldDeclarationElement,
            Element serviceTaskElement, string fieldName)
        {
            try
            {
                var fieldValue = GetStringValueFromAttributeOrElement("stringValue", "string", fieldDeclarationElement);
                if (!ReferenceEquals(fieldValue, null))
                    return new FieldDeclaration(fieldName, typeof(IExpression).FullName, new FixedValue(fieldValue));
            }
            catch (ProcessEngineException ae)
            {
                if (ae.Message.Contains("multiple elements with tag name"))
                    AddError("Multiple string field declarations found", serviceTaskElement);
                else
                    AddError("Error when paring field declarations: " + ae.Message, serviceTaskElement);
            }
            return null;
        }

        protected internal virtual FieldDeclaration ParseExpressionFieldDeclaration(Element fieldDeclarationElement,
            Element serviceTaskElement, string fieldName)
        {
            try
            {
                var expression = GetStringValueFromAttributeOrElement(PropertynameExpression, PropertynameExpression,
                    fieldDeclarationElement);
                if (!ReferenceEquals(expression, null) && (expression.Trim().Length > 0))
                    return new FieldDeclaration(fieldName, typeof(IExpression).FullName,
                        ExpressionManager.CreateExpression(expression));
            }
            catch (ProcessEngineException ae)
            {
                if (ae.Message.Contains("multiple elements with tag name"))
                    AddError("Multiple expression field declarations found", serviceTaskElement);
                else
                    AddError("Error when paring field declarations: " + ae.Message, serviceTaskElement);
            }
            return null;
        }

        protected internal virtual string GetStringValueFromAttributeOrElement(string attributeName, string elementName,
            Element element)
        {
            string value = null;

            var attributeValue = element.GetAttributeValue(attributeName);
            var childElement = element.ElementNs(CamundaBpmnExtensionsNs, elementName);
            string stringElementText = null;

            if (!string.IsNullOrEmpty(attributeValue) && (childElement != null))
            // !ReferenceEquals(attributeValue, null) && childElement != null)
            {
                AddError(
                    "Can't use attribute '" + attributeName + "' and element '" + elementName +
                    "' together, only use one", element);
            }
            else if (childElement != null)
            {
                stringElementText = childElement.Text;
                if (string.IsNullOrEmpty(stringElementText))
                    AddError(
                        "No valid value found in attribute '" + attributeName + "' nor element '" + elementName + "'",
                        element);
                else
                    value = stringElementText;
            }
            else if (!string.IsNullOrEmpty(attributeValue))
            // !ReferenceEquals(attributeValue, null) && attributeValue.Length > 0)
            {
                // Using attribute
                value = attributeValue;
            }

            return value;
        }

        /// <summary>
        ///     Parses a ITask with no specific type (behaves as passthrough).
        /// </summary>
        public virtual ActivityImpl ParseTask(Element taskElement, ScopeImpl scope)
        {
            var activity = CreateActivityOnScope(taskElement, scope);

            activity.SetActivityBehavior(new TaskActivityBehavior());

            ParseAsynchronousContinuationForActivity(taskElement, activity);

            ParseExecutionListenersOnScope(taskElement, activity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseTask(taskElement, scope, activity);
            //源码就是注释？
            //createMessageJobDeclForAsyncActivity(activity, true);
            return activity;
        }

        /// <summary>
        ///     Parses a manual ITask.
        /// </summary>
        public virtual ActivityImpl ParseManualTask(Element manualTaskElement, ScopeImpl scope)
        {
            var activity = CreateActivityOnScope(manualTaskElement, scope);
            activity.SetActivityBehavior(new ManualTaskActivityBehavior());

            ParseAsynchronousContinuationForActivity(manualTaskElement, activity);

            ParseExecutionListenersOnScope(manualTaskElement, activity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseManualTask(manualTaskElement, scope, activity);
            return activity;
        }

        /// <summary>
        ///     Parses a receive ITask.
        /// </summary>
        public virtual ActivityImpl ParseReceiveTask(Element receiveTaskElement, ScopeImpl scope)
        {
            var activity = CreateActivityOnScope(receiveTaskElement, scope);
            activity.SetActivityBehavior(new ReceiveTaskActivityBehavior());

            ParseAsynchronousContinuationForActivity(receiveTaskElement, activity);

            ParseExecutionListenersOnScope(receiveTaskElement, activity);

            if (!ReferenceEquals(receiveTaskElement.GetAttributeValue("messageRef"), null))
            {
                activity.IsScope = true;
                activity.EventScope = activity;
                var declaration = ParseMessageEventDefinition(receiveTaskElement);
                declaration.ActivityId = activity.ActivityId;
                declaration.EventScopeActivityId = activity.ActivityId;
                AddEventSubscriptionDeclaration(declaration, activity, receiveTaskElement);
            }

            foreach (var parseListener in ParseListeners)
                parseListener.ParseReceiveTask(receiveTaskElement, scope, activity);
            return activity;
        }

        /// <summary>
        ///     Parses a userTask declaration.
        /// </summary>
        public virtual ActivityImpl ParseUserTask(Element userTaskElement, ScopeImpl scope)
        {
            ActivityImpl activity = CreateActivityOnScope(userTaskElement, scope);

            ParseAsynchronousContinuationForActivity(userTaskElement, activity);

            TaskDefinition taskDefinition = ParseTaskDefinition(userTaskElement, activity.Id,
                (ProcessDefinitionEntity)scope.ProcessDefinition);
            var taskDecorator = new TaskDecorator(taskDefinition, ExpressionManager);
            Log.LogDebug("解析生成userTaskActivity：", string.Format("userTaskElement.TagName:{3},taskDefinition.Key:{0},taskDefinition.TaskListeners.Count:{1},ExpressionManager.ToString():{2}", taskDefinition.Key, taskDefinition.TaskListeners.Count, ExpressionManager.ToString(), userTaskElement.TagName));
            var userTaskActivity = new UserTaskActivityBehavior(taskDecorator);
            activity.SetActivityBehavior(userTaskActivity);

            ParseProperties(userTaskElement, activity);
            ParseExecutionListenersOnScope(userTaskElement, activity);

            foreach (var parseListener in ParseListeners)
            {
                parseListener.ParseUserTask(userTaskElement, scope, activity);
            }
            return activity;
        }

        public virtual TaskDefinition ParseTaskDefinition(Element taskElement, string taskDefinitionKey,
            ProcessDefinitionEntity processDefinition)
        {
            ITaskFormHandler taskFormHandler;
            var taskFormHandlerClassName = taskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                "formHandlerClass");
            if (!ReferenceEquals(taskFormHandlerClassName, null))
                taskFormHandler = (ITaskFormHandler)ReflectUtil.Instantiate(taskFormHandlerClassName);
            else
                taskFormHandler = new DefaultTaskFormHandler();
            taskFormHandler.ParseConfiguration(taskElement, DeploymentRenamed, processDefinition, this);

            var taskDefinition = new TaskDefinition(new DelegateTaskFormHandler(taskFormHandler, DeploymentRenamed));

            taskDefinition.Key = taskDefinitionKey;
            processDefinition.TaskDefinitions[taskDefinitionKey] = taskDefinition;

            var formKeyAttribute = taskElement.GetAttributeNS(CamundaBpmnExtensionsNs, "formKey");

            if (!ReferenceEquals(formKeyAttribute, null))
                taskDefinition.FormKey = ExpressionManager.CreateExpression(formKeyAttribute);

            var name = taskElement.GetAttributeValue("name");
            if (!ReferenceEquals(name, null))
                taskDefinition.NameExpression = ExpressionManager.CreateExpression(name);

            var descriptionStr = ParseDocumentation(taskElement);
            if (!ReferenceEquals(descriptionStr, null))
                taskDefinition.DescriptionExpression = ExpressionManager.CreateExpression(descriptionStr);

            ParseHumanPerformer(taskElement, taskDefinition);
            ParsePotentialOwner(taskElement, taskDefinition);

            // Activiti custom extension
            ParseUserTaskCustomExtensions(taskElement, taskDefinition);

            return taskDefinition;
        }

        protected internal virtual void ParseHumanPerformer(Element taskElement, TaskDefinition taskDefinition)
        {
            var humanPerformerElements = taskElement.Elements(HumanPerformer);

            if (humanPerformerElements.Count > 1)
            {
                AddError(
                    "Invalid ITask definition: multiple " + HumanPerformer + " sub elements defined for " +
                    taskDefinition.NameExpression, taskElement);
            }
            else if (humanPerformerElements.Count == 1)
            {
                var humanPerformerElement = humanPerformerElements[0];
                if (humanPerformerElement != null)
                    ParseHumanPerformerResourceAssignment(humanPerformerElement, taskDefinition);
            }
        }

        protected internal virtual void ParsePotentialOwner(Element taskElement, TaskDefinition taskDefinition)
        {
            var potentialOwnerElements = taskElement.Elements(PotentialOwner);
            foreach (var potentialOwnerElement in potentialOwnerElements)
                ParsePotentialOwnerResourceAssignment(potentialOwnerElement, taskDefinition);
        }

        protected internal virtual void ParseHumanPerformerResourceAssignment(Element performerElement,
            TaskDefinition taskDefinition)
        {
            var raeElement = performerElement.element(ResourceAssignmentExpr);
            if (raeElement != null)
            {
                var feElement = raeElement.element(FormalExpression);
                if (feElement != null)
                    taskDefinition.AssigneeExpression = ExpressionManager.CreateExpression(feElement.Text);
            }
        }

        protected internal virtual void ParsePotentialOwnerResourceAssignment(Element performerElement,
            TaskDefinition taskDefinition)
        {
            var raeElement = performerElement.element(ResourceAssignmentExpr);
            if (raeElement != null)
            {
                var feElement = raeElement.element(FormalExpression);
                if (feElement != null)
                {
                    var assignmentExpressions = ParseCommaSeparatedList(feElement.Text);
                    foreach (var assignmentExpression in assignmentExpressions)
                        if (assignmentExpression.StartsWith(UserPrefix, StringComparison.Ordinal))
                        {
                            var userAssignementId = GetAssignmentId(assignmentExpression, UserPrefix);
                            taskDefinition.AddCandidateUserIdExpression(
                                ExpressionManager.CreateExpression(userAssignementId));
                        }
                        else if (assignmentExpression.StartsWith(GroupPrefix, StringComparison.Ordinal))
                        {
                            var groupAssignementId = GetAssignmentId(assignmentExpression, GroupPrefix);
                            taskDefinition.AddCandidateGroupIdExpression(
                                ExpressionManager.CreateExpression(groupAssignementId));
                        }
                        else
                        {
                            // default: given string is a goupId, as-is.
                            taskDefinition.AddCandidateGroupIdExpression(
                                ExpressionManager.CreateExpression(assignmentExpression));
                        }
                }
            }
        }

        protected internal virtual string GetAssignmentId(string expression, string prefix)
        {
            return expression.SubstringSpecial(prefix.Length, expression.Length - 1).Trim();
        }

        protected internal virtual void ParseUserTaskCustomExtensions(Element taskElement, TaskDefinition taskDefinition)
        {
            // assignee
            var assignee = taskElement.GetAttributeNS(CamundaBpmnExtensionsNs, AssigneeExtension);
            if (!ReferenceEquals(assignee, null))
                if (taskDefinition.AssigneeExpression == null)
                    taskDefinition.AssigneeExpression = ExpressionManager.CreateExpression(assignee);
                else
                    AddError("Invalid usage: duplicate assignee declaration for ITask " + taskDefinition.NameExpression,
                        taskElement);

            // Candidate users
            var candidateUsersString = taskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                CandidateUsersExtension);
            if (!ReferenceEquals(candidateUsersString, null))
            {
                var candidateUsers = ParseCommaSeparatedList(candidateUsersString);
                foreach (var candidateUser in candidateUsers)
                    taskDefinition.AddCandidateUserIdExpression(ExpressionManager.CreateExpression(candidateUser.Trim()));
            }

            // Candidate groups
            var candidateGroupsString = taskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                CandidateGroupsExtension);
            if (!ReferenceEquals(candidateGroupsString, null))
            {
                var candidateGroups = ParseCommaSeparatedList(candidateGroupsString);
                foreach (var candidateGroup in candidateGroups)
                    taskDefinition.AddCandidateGroupIdExpression(
                        ExpressionManager.CreateExpression(candidateGroup.Trim()));
            }

            // Task listeners
            ParseTaskListeners(taskElement, taskDefinition);

            // Due date
            var dueDateExpression = taskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                DueDateExtension);
            if (!ReferenceEquals(dueDateExpression, null))
                taskDefinition.DueDateExpression = ExpressionManager.CreateExpression(dueDateExpression);

            // follow up date
            var followUpDateExpression = taskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                FollowUpDateExtension);
            if (!ReferenceEquals(followUpDateExpression, null))
                taskDefinition.FollowUpDateExpression = ExpressionManager.CreateExpression(followUpDateExpression);

            // Priority
            var priorityExpression = taskElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                PriorityExtension);
            if (!ReferenceEquals(priorityExpression, null))
                taskDefinition.PriorityExpression = ExpressionManager.CreateExpression(priorityExpression);
        }

        /// <summary>
        ///     Parses the given String as a list of comma separated entries, where an
        ///     entry can possibly be an expression that has comma's.
        ///     If somebody is smart enough to write a regex for this, please let us know.
        /// </summary>
        /// <returns> the entries of the comma separated list, trimmed. </returns>
        protected internal virtual IList<string> ParseCommaSeparatedList(string s)
        {
            IList<string> result = new List<string>();
            if (!ReferenceEquals(s, null) && !"".Equals(s))
            {
                IEnumerable<char> iterator = s.ToCharArray();

                var strb = new StringBuilder();
                var insideExpression = false;

                foreach (var c in iterator)
                {
                    if ((c == '{') || (c == '$'))
                    {
                        insideExpression = true;
                    }
                    else if (c == '}')
                    {
                        insideExpression = false;
                    }
                    else if ((c == ',') && !insideExpression)
                    {
                        result.Add(strb.ToString().Trim());
                        strb.Remove(0, strb.Length);
                    }

                    if ((c != ',') || insideExpression)
                        strb.Append(c);
                }

                if (strb.Length > 0)
                    result.Add(strb.ToString().Trim());
            }
            return result;
        }

        protected internal virtual void ParseTaskListeners(Element userTaskElement, TaskDefinition taskDefinition)
        {
            var extentionsElement = userTaskElement.element("extensionElements");
            if (extentionsElement != null)
            {
                var taskListenerElements = extentionsElement.ElementsNS(CamundaBpmnExtensionsNs,
                    "taskListener");
                foreach (var taskListenerElement in taskListenerElements)
                {
                    var eventName = taskListenerElement.GetAttributeValue("event");
                    if (!ReferenceEquals(eventName, null))
                        if (TaskListenerFields.EventnameCreate.Equals(eventName.ToLower()) ||
                            TaskListenerFields.EventnameAssignment.Equals(eventName.ToLower()) ||
                            TaskListenerFields.EventnameComplete.Equals(eventName.ToLower()) ||
                            TaskListenerFields.EventnameDelete.Equals(eventName))
                        {
                            var taskListener = ParseTaskListener(taskListenerElement);
                            taskDefinition.AddTaskListener(eventName, taskListener);
                        }
                        else
                        {
                            AddError("Attribute 'event' must be one of {create|assignment|complete|delete}",
                                userTaskElement);
                        }
                    else
                        AddError("Attribute 'event' is mandatory on taskListener", userTaskElement);
                }
            }
        }

        protected internal virtual ITaskListener ParseTaskListener(Element taskListenerElement)
        {
            ITaskListener taskListener = null;

            var className = taskListenerElement.GetAttributeValue(PropertynameClass);
            var expression = taskListenerElement.GetAttributeValue(PropertynameExpression);
            var delegateExpression = taskListenerElement.GetAttributeValue(PropertynameDelegateExpression);
            var scriptElement = taskListenerElement.ElementNs(CamundaBpmnExtensionsNs, "script");

            if (!ReferenceEquals(className, null))
                taskListener = new ClassDelegateTaskListener(className, ParseFieldDeclarations(taskListenerElement));
            else if (!ReferenceEquals(expression, null))
                taskListener = new ExpressionTaskListener(ExpressionManager.CreateExpression(expression));
            else if (!ReferenceEquals(delegateExpression, null))
                taskListener = new DelegateExpressionTaskListener(
                    ExpressionManager.CreateExpression(delegateExpression), ParseFieldDeclarations(taskListenerElement));
            else if (scriptElement != null)
                try
                {
                    //TODO ExecutableScript
                    ExecutableScript executableScript = BpmnParseUtil.ParseCamundaScript(scriptElement);
                    if (executableScript != null)
                    {
                        taskListener = new ScriptTaskListener(executableScript);
                    }
                }
                catch (BpmnParseException e)
                {
                    AddError(e);
                }
            else
                AddError(
                    "Element 'class', 'expression', 'delegateExpression' or 'script' is mandatory on taskListener",
                    taskListenerElement);
            return taskListener;
        }

        /// <summary>
        ///     Parses the end events of a certain level in the process (process,
        ///     subprocess or another scope).
        /// </summary>
        /// <param name="parentElement">
        ///     The 'parent' element that contains the end events (process,
        ///     subprocess).
        /// </param>
        /// <param name="scope">
        ///     The <seealso cref="ScopeImpl" /> to which the end events must be added.
        /// </param>
        public virtual void ParseEndEvents(Element parentElement, ScopeImpl scope)
        {
            foreach (var endEventElement in parentElement.Elements("endEvent"))
            {
                var activity = CreateActivityOnScope(endEventElement, scope);

                var errorEventDefinition = endEventElement.element(ErrorEventDefinition);
                var cancelEventDefinition = endEventElement.element(CancelEventDefinition);
                var terminateEventDefinition = endEventElement.element("terminateEventDefinition");
                var messageEventDefinitionElement = endEventElement.element(MessageEventDefinition);
                var signalEventDefinition = endEventElement.element(SignalEventDefinition);
                var compensateEventDefinitionElement = endEventElement.element(CompensateEventDefinition);
                var escalationEventDefinition = endEventElement.element(EscalationEventDefinition);

                if (errorEventDefinition != null)
                {
                    // error end event
                    var errorRef = errorEventDefinition.GetAttributeValue("errorRef");

                    if (ReferenceEquals(errorRef, null) || "".Equals(errorRef))
                    {
                        AddError("'errorRef' attribute is mandatory on error end event", errorEventDefinition);
                    }
                    else
                    {
                        var error = BpmnErrors.ContainsKey(errorRef) ? BpmnErrors[errorRef] : null;
                        if ((error != null) && (ReferenceEquals(error.ErrorCode, null) || "".Equals(error.ErrorCode)))
                            AddError(
                                "'errorCode' is mandatory on errors referenced by throwing error event definitions, but the error '" +
                                error.Id + "' does not define one.", errorEventDefinition);
                        activity.Properties.Set(BpmnProperties.Type, ActivityTypes.EndEventError);
                        if (error != null)
                            activity.SetActivityBehavior(new ErrorEndEventActivityBehavior(error.ErrorCode));// =
                                                                                                             //(ICoreActivityBehavior<IBaseDelegateExecution>)
                                                                                                             //new ErrorEndEventActivityBehavior(error.ErrorCode);
                        else
                            activity.SetActivityBehavior(new ErrorEndEventActivityBehavior(errorRef));// =
                                                                                                      //(ICoreActivityBehavior<IBaseDelegateExecution>)
                                                                                                      //new ErrorEndEventActivityBehavior(errorRef);
                    }
                }
                else if (cancelEventDefinition != null)
                {
                    if ((scope.GetProperty(BpmnProperties.Type.Name) == null) ||
                        !scope.GetProperty(BpmnProperties.Type.Name).Equals("transaction"))
                    {
                        AddError("end event with cancelEventDefinition only supported inside transaction subprocess",
                            cancelEventDefinition);
                    }
                    else
                    {
                        activity.Properties.Set(BpmnProperties.Type, ActivityTypes.EndEventCancel);
                        activity.SetActivityBehavior(new CancelEndEventActivityBehavior());// =
                                                                                           //(ICoreActivityBehavior<IBaseDelegateExecution>)new CancelEndEventActivityBehavior();
                        activity.ActivityStartBehavior = ActivityStartBehavior.InterruptFlowScope;
                        activity.SetProperty(PropertynameThrowsCompensation, true);
                        activity.IsScope = true;
                    }
                }
                else if (terminateEventDefinition != null)
                {
                    activity.Properties.Set(BpmnProperties.Type, ActivityTypes.EndEventTerminate);
                    activity.SetActivityBehavior(new TerminateEndEventActivityBehavior());// =
                                                                                          //(ICoreActivityBehavior<IBaseDelegateExecution>)new TerminateEndEventActivityBehavior();
                    activity.ActivityStartBehavior = ActivityStartBehavior.InterruptFlowScope;
                }
                else if (messageEventDefinitionElement != null)
                {
                    activity.Properties.Set(BpmnProperties.Type, ActivityTypes.EndEventMessage);
                    if (IsServiceTaskLike(messageEventDefinitionElement))
                    {
                        // CAM-436 same behaviour as service ITask
                        activity.ActivityBehavior =
                            ParseServiceTaskLike(ActivityTypes.EndEventMessage, messageEventDefinitionElement, scope)
                                .ActivityBehavior;
                    }
                    else
                    {
                        // default to servicebus behavior if no service ITask
                        // properties have been specified

                        var messageDeclaration = ParseMessageEventDefinition(messageEventDefinitionElement);
                        activity.SetActivityBehavior(new IntermediateMessageThrowEventActivityBehavior(messageDeclaration.EventName));// =
                                                                                                       //(ICoreActivityBehavior<IBaseDelegateExecution>)
                                                                                                       //new IntermediateThrowNoneEventActivityBehavior();
                    }
                }
                else if (signalEventDefinition != null)
                {
                    activity.Properties.Set(BpmnProperties.Type, ActivityTypes.EndEventSignal);
                    var signalDefinition = ParseSignalEventDefinition(signalEventDefinition);
                    activity.ActivityBehavior =
                        new ThrowSignalEventActivityBehavior(signalDefinition);
                }
                else if (compensateEventDefinitionElement != null)
                {
                    activity.Properties.Set(BpmnProperties.Type, ActivityTypes.EndEventCompensation);
                    var compensateEventDefinition = ParseThrowCompensateEventDefinition(
                        compensateEventDefinitionElement, scope);
                    activity.ActivityBehavior =
                        new CompensationEventActivityBehavior(compensateEventDefinition);
                    activity.SetProperty(PropertynameThrowsCompensation, true);
                    activity.IsScope = true;
                }
                else if (escalationEventDefinition != null)
                {
                    activity.Properties.Set(BpmnProperties.Type, ActivityTypes.EndEventEscalation);

                    var escalation = FindEscalationForEscalationEventDefinition(escalationEventDefinition);
                    if ((escalation != null) && ReferenceEquals(escalation.EscalationCode, null))
                        AddError("escalation end event must have an 'escalationCode'", escalationEventDefinition);
                    activity.ActivityBehavior =
                        new ThrowEscalationEventActivityBehavior(escalation);
                }
                else
                {
                    // default: none end event
                    activity.Properties.Set(BpmnProperties.Type, ActivityTypes.EndEventNone);

                    activity.ActivityBehavior = //(CoreActivityBehavior<IBaseDelegateExecution>)
                        new NoneEndEventActivityBehavior();
                }

                if (activity != null)
                    ParseActivityInputOutput(endEventElement, activity);

                ParseAsynchronousContinuationForActivity(endEventElement, activity);

                foreach (var parseListener in ParseListeners)
                    parseListener.ParseEndEvent(endEventElement, scope, activity);

                ParseExecutionListenersOnScope(endEventElement, activity);
            }
        }

        /// <summary>
        ///     Parses the boundary events of a certain 'level' (process, subprocess or
        ///     other scope).
        ///     Note that the boundary events are not parsed during the parsing of the bpmn
        ///     activities, since the semantics are different (boundaryEvent needs to be
        ///     added as nested activity to the reference activity on PVM level).
        /// </summary>
        /// <param name="parentElement">
        ///     The 'parent' element that contains the activities (process,
        ///     subprocess).
        /// </param>
        /// <param name="flowScope">
        ///     The <seealso cref="ScopeImpl" /> to which the activities must be added.
        /// </param>
        public virtual void ParseBoundaryEvents(Element parentElement, ScopeImpl flowScope)
        {
            foreach (var boundaryEventElement in parentElement.Elements("boundaryEvent"))
            {
                // The boundary event is attached to an activity, reference by the
                // 'attachedToRef' attribute
                var attachedToRef = boundaryEventElement.GetAttributeValue("attachedToRef");
                if (ReferenceEquals(attachedToRef, null) || attachedToRef.Equals(""))
                    AddError("AttachedToRef is required when using a timerEventDefinition", boundaryEventElement);

                // Representation structure-wise is a nested activity in the activity to
                // which its attached
                var id = boundaryEventElement.GetAttributeValue("id");

                Log.ParsingElement("boundary event", id);

                // Depending on the sub-element definition, the correct activityBehavior
                // parsing is selected
                var timerEventDefinition = boundaryEventElement.element(TimerEventDefinition);
                var errorEventDefinition = boundaryEventElement.element(ErrorEventDefinition);
                var signalEventDefinition = boundaryEventElement.element(SignalEventDefinition);
                var cancelEventDefinition = boundaryEventElement.element(CancelEventDefinition);
                var compensateEventDefinition = boundaryEventElement.element(CompensateEventDefinition);
                var messageEventDefinition = boundaryEventElement.element(MessageEventDefinition);
                var escalationEventDefinition = boundaryEventElement.element(EscalationEventDefinition);
                var conditionalEventDefinition = boundaryEventElement.element(ConditionalEventDefinition);

                // create the boundary event activity
                var boundaryEventActivity = CreateActivityOnScope(boundaryEventElement, flowScope);

                var attachedActivity = (ActivityImpl)flowScope.FindActivityAtLevelOfSubprocess(attachedToRef);
                if (attachedActivity == null)
                    AddError(
                        "Invalid reference in boundary event. Make sure that the referenced activity is defined in the same scope as the boundary event",
                        boundaryEventElement);

                // determine the correct event scope (the scope in which the boundary event catches events)
                if (compensateEventDefinition == null)
                {
                    var multiInstanceScope = GetMultiInstanceScope(attachedActivity);
                    if (multiInstanceScope != null)
                    {
                        // if the boundary event is attached to a multi instance activity,
                        // then the scope of the boundary event is the multi instance body.
                        boundaryEventActivity.EventScope = multiInstanceScope;
                    }
                    else
                    {
                        attachedActivity.IsScope = true;
                        boundaryEventActivity.EventScope = attachedActivity;
                    }
                }
                else
                {
                    boundaryEventActivity.EventScope = attachedActivity;
                }

                // except escalation, by default is assumed to abort the activity
                var cancelActivityAttr = boundaryEventElement.GetAttributeValue("cancelActivity", True);
                var isCancelActivity = Convert.ToBoolean(cancelActivityAttr);

                // determine start behavior
                if (isCancelActivity)
                    boundaryEventActivity.ActivityStartBehavior = ActivityStartBehavior.CancelEventScope;
                else
                    boundaryEventActivity.ActivityStartBehavior = ActivityStartBehavior.ConcurrentInFlowScope;

                // Catch event behavior is the same for most types
                IActivityBehavior behavior = new BoundaryEventActivityBehavior();
                if (timerEventDefinition != null)
                    ParseBoundaryTimerEventDefinition(timerEventDefinition, isCancelActivity, boundaryEventActivity);
                else if (errorEventDefinition != null)
                    ParseBoundaryErrorEventDefinition(errorEventDefinition, boundaryEventActivity);
                else if (signalEventDefinition != null)
                    ParseBoundarySignalEventDefinition(signalEventDefinition, isCancelActivity, boundaryEventActivity);
                else if (cancelEventDefinition != null)
                    behavior = ParseBoundaryCancelEventDefinition(cancelEventDefinition, boundaryEventActivity);
                else if (compensateEventDefinition != null)
                    ParseBoundaryCompensateEventDefinition(compensateEventDefinition, boundaryEventActivity);
                else if (messageEventDefinition != null)
                    ParseBoundaryMessageEventDefinition(messageEventDefinition, isCancelActivity, boundaryEventActivity);
                else if (escalationEventDefinition != null)
                    if (attachedActivity.SubProcessScope || attachedActivity.ActivityBehavior is CallActivityBehavior)
                        ParseBoundaryEscalationEventDefinition(escalationEventDefinition, isCancelActivity,
                            boundaryEventActivity);
                    else
                        AddError(
                            "An escalation boundary event should only be attached to a subprocess or a call activity",
                            boundaryEventElement);
                else if (conditionalEventDefinition != null)
                    behavior = ParseBoundaryConditionalEventDefinition(conditionalEventDefinition, isCancelActivity,
                        boundaryEventActivity);
                else
                    AddError("Unsupported boundary event type", boundaryEventElement);

                EnsureNoIoMappingDefined(boundaryEventElement);

                foreach (var parseListener in ParseListeners)
                    parseListener.ParseBoundaryEvent(boundaryEventElement, flowScope, boundaryEventActivity);

                boundaryEventActivity.ActivityBehavior = behavior;

                ParseExecutionListenersOnScope(boundaryEventElement, boundaryEventActivity);
            }
        }

        protected internal virtual ActivityImpl GetMultiInstanceScope(ActivityImpl activity)
        {
            if (activity.MultiInstance)
                return activity.ParentFlowScopeActivity;
            return null;
        }

        /// <summary>
        ///     Parses a boundary timer event. The end-result will be that the given nested
        ///     activity will get the appropriate <seealso cref="IActivityBehavior" />.
        /// </summary>
        /// <param name="timerEventDefinition">
        ///     The XML element corresponding with the timer event details
        /// </param>
        /// <param name="interrupting">
        ///     Indicates whether this timer is interrupting.
        /// </param>
        /// <param name="boundaryActivity">
        ///     The activity which maps to the structure of the timer event on the
        ///     boundary of another activity. Note that this is NOT the activity
        ///     onto which the boundary event is attached, but a nested activity
        ///     inside this activity, specifically created for this event.
        /// </param>
        public virtual void ParseBoundaryTimerEventDefinition(Element timerEventDefinition, bool interrupting,
            ActivityImpl boundaryActivity)
        {
            boundaryActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.BoundaryTimer);
            var timerDeclaration = ParseTimer(timerEventDefinition, boundaryActivity,
                TimerExecuteNestedActivityJobHandler.TYPE);

            // ACT-1427
            if (interrupting)
            {
                timerDeclaration.InterruptingTimer = true;

                var timeCycleElement = timerEventDefinition.element("timeCycle");
                if (timeCycleElement != null)
                    AddTimeCycleWarning(timeCycleElement, "cancelling boundary");
            }

            AddTimerDeclaration((ScopeImpl)boundaryActivity.EventScope, timerDeclaration);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseBoundaryTimerEventDefinition(timerEventDefinition, interrupting, boundaryActivity);
        }

        public virtual void ParseBoundarySignalEventDefinition(Element element, bool interrupting,
            ActivityImpl signalActivity)
        {
            signalActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.BoundarySignal);

            var signalDefinition = ParseSignalEventDefinition(element);
            if (ReferenceEquals(signalActivity.Id, null))
                AddError("boundary event has no id", element);
            signalDefinition.ActivityId = signalActivity.Id;
            AddEventSubscriptionDeclaration(signalDefinition, (ScopeImpl)signalActivity.EventScope, element);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseBoundarySignalEventDefinition(element, interrupting, signalActivity);
        }

        public virtual void ParseBoundaryMessageEventDefinition(Element element, bool interrupting,
            ActivityImpl messageActivity)
        {
            messageActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.BoundaryMessage);

            var messageEventDefinition = ParseMessageEventDefinition(element);
            if (ReferenceEquals(messageActivity.Id, null))
                AddError("boundary event has no id", element);
            messageEventDefinition.ActivityId = messageActivity.Id;
            AddEventSubscriptionDeclaration(messageEventDefinition, (ScopeImpl)messageActivity.EventScope, element);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseBoundaryMessageEventDefinition(element, interrupting, messageActivity);
        }

        protected internal virtual void ParseTimerStartEventDefinition(Element timerEventDefinition,
            ActivityImpl timerActivity, ProcessDefinitionEntity processDefinition)
        {
            timerActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.StartEventTimer);
            var timerDeclaration = ParseTimer(timerEventDefinition, timerActivity, TimerStartEventJobHandler.TYPE);
            timerDeclaration.RawJobHandlerConfiguration = processDefinition.Key;

            var timerDeclarations =
                (IList<TimerDeclarationImpl>)processDefinition.GetProperty(PropertynameStartTimer);
            if (timerDeclarations == null)
            {
                timerDeclarations = new List<TimerDeclarationImpl>();
                processDefinition.SetProperty(PropertynameStartTimer, timerDeclarations);
            }
            timerDeclarations.Add(timerDeclaration);
        }

        protected internal virtual void ParseTimerStartEventDefinitionForEventSubprocess(Element timerEventDefinition,
            ActivityImpl timerActivity, bool interrupting)
        {
            timerActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.StartEventTimer);

            var timerDeclaration = ParseTimer(timerEventDefinition, timerActivity,
                TimerStartEventSubprocessJobHandler.TYPE);

            timerDeclaration.Activity = timerActivity;
            timerDeclaration.EventScopeActivityId = timerActivity.EventScope.Id;
            timerDeclaration.RawJobHandlerConfiguration = timerActivity.FlowScope.Id;
            timerDeclaration.InterruptingTimer = interrupting;

            if (interrupting)
            {
                var timeCycleElement = timerEventDefinition.element("timeCycle");
                if (timeCycleElement != null)
                    AddTimeCycleWarning(timeCycleElement, "interrupting start");
            }

            AddTimerDeclaration((ScopeImpl)timerActivity.EventScope, timerDeclaration);
        }

        protected internal virtual void ParseEventDefinitionForSubprocess(
            EventSubscriptionDeclaration subscriptionDeclaration, ActivityImpl activity, Element element)
        {
            subscriptionDeclaration.ActivityId = activity.Id;
            subscriptionDeclaration.EventScopeActivityId = activity.EventScope.Id;
            subscriptionDeclaration.StartEvent = false;
            AddEventSubscriptionDeclaration(subscriptionDeclaration, (ScopeImpl)activity.EventScope, element);
        }

        protected internal virtual void ParseIntermediateSignalEventDefinition(Element element,
            ActivityImpl signalActivity)
        {
            signalActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.IntermediateEventSignal);

            ParseSignalCatchEventDefinition(element, signalActivity, false);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseIntermediateSignalCatchEventDefinition(element, signalActivity);
        }

        protected internal virtual void ParseSignalCatchEventDefinition(Element element, ActivityImpl signalActivity,
            bool isStartEvent)
        {
            var signalDefinition = ParseSignalEventDefinition(element);
            signalDefinition.ActivityId = signalActivity.Id;
            signalDefinition.StartEvent = isStartEvent;
            AddEventSubscriptionDeclaration(signalDefinition, (ScopeImpl)signalActivity.EventScope, element);

            var catchingAsyncDeclaration = new EventSubscriptionJobDeclaration(signalDefinition);
            //TODO JobPriorityProvider
            catchingAsyncDeclaration.JobPriorityProvider = (IParameterValueProvider)signalActivity.GetProperty(PropertynameJobPriority);
            catchingAsyncDeclaration.Activity = signalActivity;
            signalDefinition.JobDeclaration = catchingAsyncDeclaration;
            AddEventSubscriptionJobDeclaration(catchingAsyncDeclaration, signalActivity, element);
        }

        protected internal virtual EventSubscriptionDeclaration ParseSignalEventDefinition(
            Element signalEventDefinitionElement)
        {
            var signalRef = signalEventDefinitionElement.GetAttributeValue("signalRef");
            if (ReferenceEquals(signalRef, null))
            {
                AddError("signalEventDefinition does not have required property 'signalRef'",
                    signalEventDefinitionElement);
                return null;
            }
            //语法重写，可能为null
            var signalDefinition = Signals.ContainsKey(ResolveName(signalRef)) ? Signals[ResolveName(signalRef)] : null;
            EventSubscriptionDeclaration signalEventDefinition;
            if (signalDefinition == null)
            {
                AddError("Could not find signal with id '" + signalRef + "'", signalEventDefinitionElement);
                signalEventDefinition = new EventSubscriptionDeclaration(null, EventType.Signal);
            }
            else
            {
                signalEventDefinition = new EventSubscriptionDeclaration(signalDefinition.Expression, EventType.Signal);
            }

            var throwingAsynch =
                True.Equals(signalEventDefinitionElement.GetAttributeNS(CamundaBpmnExtensionsNs, "async"), StringComparison.CurrentCultureIgnoreCase);
            signalEventDefinition.Async = throwingAsynch;

            return signalEventDefinition;
        }

        protected internal virtual void ParseIntermediateTimerEventDefinition(Element timerEventDefinition,
            ActivityImpl timerActivity)
        {
            timerActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.IntermediateEventTimer);
            var timerDeclaration = ParseTimer(timerEventDefinition, timerActivity,
                TimerCatchIntermediateEventJobHandler.TYPE);

            var timeCycleElement = timerEventDefinition.element("timeCycle");
            if (timeCycleElement != null)
                AddTimeCycleWarning(timeCycleElement, "intermediate catch");

            AddTimerDeclaration((ScopeImpl)timerActivity.EventScope, timerDeclaration);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseIntermediateTimerEventDefinition(timerEventDefinition, timerActivity);
        }

        protected internal virtual TimerDeclarationImpl ParseTimer(Element timerEventDefinition, ActivityImpl timerActivity, string jobHandlerType)
        {
            // TimeDate
            var type = TimerDeclarationType.Date;
            var expression = ParseExpression(timerEventDefinition, "timeDate");
            // TimeCycle
            if (expression == null)
            {
                type = TimerDeclarationType.Cycle;
                expression = ParseExpression(timerEventDefinition, "timeCycle");
            }
            // TimeDuration
            if (expression == null)
            {
                type = TimerDeclarationType.Duration;
                expression = ParseExpression(timerEventDefinition, "timeDuration");
            }
            // neither date, cycle or duration configured!
            if (expression == null)
                AddError("Timer needs configuration (either timeDate, timeCycle or timeDuration is needed).", timerEventDefinition);

            // Parse the timer declaration
            var timerDeclaration = new TimerDeclarationImpl((Engine.Delegate.IExpression)expression, type, jobHandlerType);
            timerDeclaration.RawJobHandlerConfiguration = timerActivity.Id;
            timerDeclaration.Exclusive = Convert.ToBoolean(timerEventDefinition.GetAttributeNS(CamundaBpmnExtensionsNs, "exclusive", JobEntity.DefaultExclusive.ToString()));
            if (string.IsNullOrEmpty(timerActivity.Id))
                AddError("Attribute \"id\" is required!", timerEventDefinition);
            timerDeclaration.Activity = timerActivity;
            timerDeclaration.JobConfiguration = type + ": " + expression.ExpressionText;
            AddJobDeclarationToProcessDefinition(timerDeclaration, (IProcessDefinition)timerActivity.ProcessDefinition);

            timerDeclaration.JobPriorityProvider = (IParameterValueProvider)timerActivity.GetProperty(PropertynameJobPriority);

            return timerDeclaration;
        }

        protected internal virtual IExpression ParseExpression(Element parent, string name)
        {
            var value = parent.element(name);
            if (value != null)
            {
                var expressionText = value.Text.Trim();
                return (IExpression)ExpressionManager.CreateExpression(expressionText);
            }
            return null;
        }

        public virtual void ParseBoundaryErrorEventDefinition(Element errorEventDefinition,
            ActivityImpl boundaryEventActivity)
        {
            boundaryEventActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.BoundaryError);

            var errorRef = errorEventDefinition.GetAttributeValue("errorRef");
            Error error = null;
            var definition = new ErrorEventDefinition(boundaryEventActivity.Id);
            if (!ReferenceEquals(errorRef, null))
            {
                error = BpmnErrors.ContainsKey(errorRef) ? BpmnErrors[errorRef] : null;
                definition.ErrorCode = error == null ? errorRef : error.ErrorCode;
            }
            SetErrorCodeVariableOnErrorEventDefinition(errorEventDefinition, definition);
            SetErrorMessageVariableOnErrorEventDefinition(errorEventDefinition, definition);

            AddErrorEventDefinition(definition, (ScopeImpl)boundaryEventActivity.EventScope);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseBoundaryErrorEventDefinition(errorEventDefinition, true,
                    (ActivityImpl)boundaryEventActivity.EventScope, boundaryEventActivity);
        }

        protected internal virtual void AddErrorEventDefinition(ErrorEventDefinition errorEventDefinition,
            ScopeImpl catchingScope)
        {
            catchingScope.Properties.AddListItem(BpmnProperties.ErrorEventDefinitions, errorEventDefinition);

            var errorEventDefinitions = catchingScope.Properties.Get(BpmnProperties.ErrorEventDefinitions);
            //TODO
            //errorEventDefinitions.Sort(ErrorEventDefinition.comparator);
        }

        protected internal virtual void ParseBoundaryEscalationEventDefinition(Element escalationEventDefinitionElement,
            bool cancelActivity, ActivityImpl boundaryEventActivity)
        {
            boundaryEventActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.BoundaryEscalation);

            var escalationEventDefinition =
                createEscalationEventDefinitionForEscalationHandler(escalationEventDefinitionElement,
                    boundaryEventActivity, cancelActivity);
            AddEscalationEventDefinition((ScopeImpl)boundaryEventActivity.EventScope, escalationEventDefinition,
                escalationEventDefinitionElement);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseBoundaryEscalationEventDefinition(escalationEventDefinitionElement, cancelActivity,
                    boundaryEventActivity);
        }

        /// <summary>
        ///     Find the referenced escalation of the given escalation event definition.
        ///     Add errors if the referenced escalation not found.
        /// </summary>
        /// <returns> referenced escalation or <code>null</code>, if referenced escalation not found </returns>
        protected internal virtual Escalation FindEscalationForEscalationEventDefinition(
            Element escalationEventDefinition)
        {
            var escalationRef = escalationEventDefinition.GetAttributeValue("escalationRef");
            if (ReferenceEquals(escalationRef, null))
                AddError("escalationEventDefinition does not have required attribute 'escalationRef'",
                    escalationEventDefinition);
            else if (!Escalations.ContainsKey(escalationRef))
                AddError("could not find escalation with id '" + escalationRef + "'", escalationEventDefinition);
            else
                return Escalations[escalationRef];
            return null;
        }

        protected internal virtual EscalationEventDefinition createEscalationEventDefinitionForEscalationHandler(
            Element escalationEventDefinitionElement, ActivityImpl escalationHandler, bool cancelActivity)
        {
            var escalationEventDefinition = new EscalationEventDefinition(escalationHandler, cancelActivity);

            var escalationRef = escalationEventDefinitionElement.GetAttributeValue("escalationRef");
            if (!ReferenceEquals(escalationRef, null))
                if (!Escalations.ContainsKey(escalationRef))
                {
                    AddError("could not find escalation with id '" + escalationRef + "'",
                        escalationEventDefinitionElement);
                }
                else
                {
                    var escalation = Escalations[escalationRef];
                    escalationEventDefinition.EscalationCode = escalation.EscalationCode;
                }

            var escalationCodeVariable =
                escalationEventDefinitionElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                    "escalationCodeVariable");
            if (!ReferenceEquals(escalationCodeVariable, null))
                escalationEventDefinition.EscalationCodeVariable = escalationCodeVariable;

            return escalationEventDefinition;
        }

        protected internal virtual void AddEscalationEventDefinition(ScopeImpl catchingScope,
            EscalationEventDefinition escalationEventDefinition, Element element)
        {
            // ensure there is only one escalation handler (e.g. escalation boundary event, escalation event subprocess) what can catch the escalation event
            foreach (
                var existingEscalationEventDefinition in
                catchingScope.Properties.Get(BpmnProperties.EscalationEventDefinitions))
                if (existingEscalationEventDefinition.EscalationHandler.SubProcessScope &&
                    escalationEventDefinition.EscalationHandler.SubProcessScope)
                {
                    if (ReferenceEquals(existingEscalationEventDefinition.EscalationCode, null) &&
                        ReferenceEquals(escalationEventDefinition.EscalationCode, null))
                        AddError(
                            "The same scope can not contains more than one escalation event subprocess without escalation code. " +
                            "An escalation event subprocess without escalation code catch all escalation events.",
                            element);
                    else if (ReferenceEquals(existingEscalationEventDefinition.EscalationCode, null) ||
                             ReferenceEquals(escalationEventDefinition.EscalationCode, null))
                        AddError(
                            "The same scope can not contains an escalation event subprocess without escalation code and another one with escalation code. " +
                            "The escalation event subprocess without escalation code catch all escalation events.",
                            element);
                    else if (
                        existingEscalationEventDefinition.EscalationCode.Equals(
                            escalationEventDefinition.EscalationCode))
                        AddError(
                            "multiple escalation event subprocesses with the same escalationCode '" +
                            escalationEventDefinition.EscalationCode + "' are not supported on same scope",
                            element);
                }
                else if (!existingEscalationEventDefinition.EscalationHandler.SubProcessScope &&
                         !escalationEventDefinition.EscalationHandler.SubProcessScope)
                {
                    if (ReferenceEquals(existingEscalationEventDefinition.EscalationCode, null) &&
                        ReferenceEquals(escalationEventDefinition.EscalationCode, null))
                        AddError(
                            "The same scope can not contains more than one escalation boundary event without escalation code. " +
                            "An escalation boundary event without escalation code catch all escalation events.",
                            element);
                    else if (ReferenceEquals(existingEscalationEventDefinition.EscalationCode, null) ||
                             ReferenceEquals(escalationEventDefinition.EscalationCode, null))
                        AddError(
                            "The same scope can not contains an escalation boundary event without escalation code and another one with escalation code. " +
                            "The escalation boundary event without escalation code catch all escalation events.",
                            element);
                    else if (
                        existingEscalationEventDefinition.EscalationCode.Equals(
                            escalationEventDefinition.EscalationCode))
                        AddError(
                            "multiple escalation boundary events with the same escalationCode '" +
                            escalationEventDefinition.EscalationCode + "' are not supported on same scope",
                            element);
                }

            catchingScope.Properties.AddListItem(BpmnProperties.EscalationEventDefinitions, escalationEventDefinition);
        }

        protected internal virtual void AddTimerDeclaration(ScopeImpl scope, TimerDeclarationImpl timerDeclaration)
        {
            scope.Properties.PutMapEntry(BpmnProperties.TimerDeclarations, timerDeclaration.ActivityId,
                timerDeclaration);
        }

        protected internal virtual void AddVariableDeclaration(ScopeImpl scope, VariableDeclaration variableDeclaration)
        {
            var variableDeclarations =
                (IList<VariableDeclaration>)scope.GetProperty(PropertynameVariableDeclarations);
            if (variableDeclarations == null)
            {
                variableDeclarations = new List<VariableDeclaration>();
                scope.SetProperty(PropertynameVariableDeclarations, variableDeclarations);
            }
            variableDeclarations.Add(variableDeclaration);
        }

        /// <summary>
        ///     Parses the given element as conditional boundary event.
        /// </summary>
        /// <param name="element"> the XML element which contains the conditional event information </param>
        /// <param name="interrupting"> indicates if the event is interrupting or not </param>
        /// <param name="conditionalActivity"> the conditional event activity </param>
        /// <returns> the boundary conditional event behavior which contains the condition </returns>
        public virtual BoundaryConditionalEventActivityBehavior ParseBoundaryConditionalEventDefinition(Element element,
            bool interrupting, ActivityImpl conditionalActivity)
        {
            conditionalActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.BoundaryConditional);

            var conditionalEventDefinition = ParseConditionalEventDefinition(element, conditionalActivity);
            conditionalEventDefinition.Interrupting = interrupting;
            AddEventSubscriptionDeclaration(conditionalEventDefinition, (ScopeImpl)conditionalActivity.EventScope,
                element);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseBoundaryConditionalEventDefinition(element, interrupting, conditionalActivity);

            return new BoundaryConditionalEventActivityBehavior(conditionalEventDefinition);
        }

        /// <summary>
        ///     Parses the given element as intermediate conditional event.
        /// </summary>
        /// <param name="element"> the XML element which contains the conditional event information </param>
        /// <param name="conditionalActivity"> the conditional event activity </param>
        /// <returns> returns the conditional activity with the parsed information </returns>
        public virtual ConditionalEventDefinition ParseIntermediateConditionalEventDefinition(Element element,
            ActivityImpl conditionalActivity)
        {
            conditionalActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.IntermediateEventConditional);

            var conditionalEventDefinition = ParseConditionalEventDefinition(element, conditionalActivity);
            AddEventSubscriptionDeclaration(conditionalEventDefinition, (ScopeImpl)conditionalActivity.EventScope,
                element);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseIntermediateConditionalEventDefinition(element, conditionalActivity);

            return conditionalEventDefinition;
        }

        /// <summary>
        ///     Parses the given element as conditional start event of an event subprocess.
        /// </summary>
        /// <param name="element"> the XML element which contains the conditional event information </param>
        /// <param name="interrupting"> indicates if the event is interrupting or not </param>
        /// <param name="conditionalActivity">
        ///     the conditional event activity
        ///     @return
        /// </param>
        public virtual ConditionalEventDefinition ParseConditionalStartEventForEventSubprocess(Element element,
            ActivityImpl conditionalActivity, bool interrupting)
        {
            conditionalActivity.Properties.Set(BpmnProperties.Type, ActivityTypes.StartEventConditional);

            var conditionalEventDefinition = ParseConditionalEventDefinition(element, conditionalActivity);
            conditionalEventDefinition.Interrupting = interrupting;
            AddEventSubscriptionDeclaration(conditionalEventDefinition, (ScopeImpl)conditionalActivity.EventScope,
                element);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseConditionalStartEventForEventSubprocess(element, conditionalActivity, interrupting);

            return conditionalEventDefinition;
        }

        /// <summary>
        ///     Parses the given element and returns an ConditionalEventDefinition object.
        /// </summary>
        /// <param name="element"> the XML element which contains the conditional event information </param>
        /// <param name="conditionalActivity"> the conditional event activity </param>
        /// <returns> the conditional event definition which was parsed </returns>
        protected internal virtual ConditionalEventDefinition ParseConditionalEventDefinition(Element element,
            ActivityImpl conditionalActivity)
        {
            ConditionalEventDefinition conditionalEventDefinition = null;

            var conditionExprElement = element.element(Condition);
            if (conditionExprElement != null)
            {
                var condition = ParseConditionExpression(conditionExprElement);
                conditionalEventDefinition = new ConditionalEventDefinition(condition, conditionalActivity);

                conditionalActivity.ProcessDefinition.Properties.Set(BpmnProperties.HasConditionalEvents, true);

                var variableName = element.GetAttributeNS(CamundaBpmnExtensionsNs, "variableName");
                conditionalEventDefinition.VariableName = variableName;

                var variableEvents = element.GetAttributeNS(CamundaBpmnExtensionsNs, "variableEvents");
                var variableEventsList = ParseCommaSeparatedList(variableEvents);
                conditionalEventDefinition.VariableEvents = new HashSet<string>(variableEventsList);

                foreach (var variableEvent in variableEventsList)
                    if (!VariableEvents.Contains(variableEvent))
                        AddWarning(
                            "Variable event: " + variableEvent + " is not valid. Possible variable change events are: " +
                            string.Join(",", VariableEvents.ToArray()), element);
            }
            else
            {
                AddError("Conditional event must contain an expression for evaluation.", element);
            }

            return conditionalEventDefinition;
        }

        /// <summary>
        ///     Parses a subprocess (formally known as an embedded subprocess): a
        ///     subprocess defined within another process definition.
        /// </summary>
        /// <param name="subProcessElement">
        ///     The XML element corresponding with the subprocess definition
        /// </param>
        /// <param name="scope">
        ///     The current scope on which the subprocess is defined.
        /// </param>
        public virtual ActivityImpl ParseSubProcess(Element subProcessElement, ScopeImpl scope)
        {
            ActivityImpl subProcessActivity = CreateActivityOnScope(subProcessElement, scope);
            subProcessActivity.SubProcessScope = true;

            ParseAsynchronousContinuationForActivity(subProcessElement, subProcessActivity);

            bool? isTriggeredByEvent = ParseBooleanAttribute(subProcessElement.GetAttributeValue("triggeredByEvent"),
                false);
            subProcessActivity.Properties.Set(BpmnProperties.TriggeredByEvent, isTriggeredByEvent.GetValueOrDefault());
            subProcessActivity.SetProperty(PropertynameConsumesCompensation, !isTriggeredByEvent);

            subProcessActivity.IsScope = true;
            if (isTriggeredByEvent.Value)
            {
                subProcessActivity.ActivityBehavior =
                    new EventSubProcessActivityBehavior();
                subProcessActivity.EventScope = scope;
            }
            else
            {
                subProcessActivity.SetActivityBehavior(new SubProcessActivityBehavior());
            }
            ParseScope(subProcessElement, subProcessActivity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseSubProcess(subProcessElement, scope, subProcessActivity);
            return subProcessActivity;
        }
        /// <summary>
        /// 解析事务 添加SubProcessActivityBehavior
        /// </summary>
        /// <param name="transactionElement"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        protected internal virtual ActivityImpl ParseTransaction(Element transactionElement, ScopeImpl scope)
        {
            ActivityImpl activity = CreateActivityOnScope(transactionElement, scope);

            ParseAsynchronousContinuationForActivity(transactionElement, activity);

            activity.IsScope = true;
            activity.SubProcessScope = true;
            activity.ActivityBehavior = new SubProcessActivityBehavior();
            activity.Properties.Set(BpmnProperties.TriggeredByEvent, false);
            activity.SetIsTransaction();
            ParseScope(transactionElement, activity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseTransaction(transactionElement, scope, activity);
            return activity;
        }

        /// <summary>
        ///     Parses a call activity (currently only supporting calling subprocesses).
        /// </summary>
        /// <param name="callActivityElement">
        ///     The XML element defining the call activity
        /// </param>
        /// <param name="scope">
        ///     The current scope on which the call activity is defined.
        /// </param>
        public virtual ActivityImpl ParseCallActivity(Element callActivityElement, ScopeImpl scope, bool isMultiInstance)
        {
            var activity = CreateActivityOnScope(callActivityElement, scope);

            // parse async
            ParseAsynchronousContinuationForActivity(callActivityElement, activity);

            // parse definition key (and behavior)
            var calledElement = callActivityElement.GetAttributeValue("calledElement");
            var caseRef = callActivityElement.GetAttributeNS(CamundaBpmnExtensionsNs, "caseRef");
            var className = callActivityElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                PropertynameVariableMappingClass);
            var delegateExpression = callActivityElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                PropertynameVariableMappingDelegateExpression);

            if (ReferenceEquals(calledElement, null) && ReferenceEquals(caseRef, null))
                AddError("Missing attribute 'calledElement' or 'caseRef'", callActivityElement);
            else if (!ReferenceEquals(calledElement, null) && !ReferenceEquals(caseRef, null))
                AddError(
                    "The attributes 'calledElement' or 'caseRef' cannot be used together: Use either 'calledElement' or 'caseRef'",
                    callActivityElement);

            var bindingAttributeName = "calledElementBinding";
            var versionAttributeName = "calledElementVersion";
            var tenantIdAttributeName = "calledElementTenantId";

            var deploymentId = DeploymentRenamed.Id;

            var callableElement = new CallableElement();
            callableElement.DeploymentId = deploymentId;

            CallableElementActivityBehavior behavior = null;

            if (!ReferenceEquals(calledElement, null))
            {
                if (!ReferenceEquals(className, null))
                {
                    //TODO activitybehavior
                    behavior = new CallActivityBehavior(className);
                }
                else if (!ReferenceEquals(delegateExpression, null))
                {
                    var exp = ExpressionManager.CreateExpression(delegateExpression);
                    behavior = new CallActivityBehavior(exp);
                }
                else
                {
                    //TODO activitybehavior
                    behavior = new CallActivityBehavior();
                }
                var definitionKeyProvider = CreateParameterValueProvider(calledElement, ExpressionManager);
                callableElement.DefinitionKeyValueProvider = definitionKeyProvider;
            }
            else
            {
                behavior = new CaseCallActivityBehavior();
                var definitionKeyProvider = CreateParameterValueProvider(caseRef, ExpressionManager);
                callableElement.DefinitionKeyValueProvider = definitionKeyProvider;
                bindingAttributeName = "caseBinding";
                versionAttributeName = "caseVersion";
                tenantIdAttributeName = "caseTenantId";
            }

            behavior.CallableElement = callableElement;

            // parse binding
            ParseBinding(callActivityElement, activity, callableElement, bindingAttributeName);

            // parse version
            ParseVersion(callActivityElement, activity, callableElement, bindingAttributeName, versionAttributeName);

            // parse tenant id
            ParseTenantId(callActivityElement, activity, callableElement, tenantIdAttributeName);

            // parse input parameter
            ParseInputParameter(callActivityElement, activity, callableElement);

            // parse output parameter
            ParseOutputParameter(callActivityElement, activity, callableElement);

            if (!isMultiInstance)
                activity.IsScope = true;
            activity.ActivityBehavior = behavior;

            ParseExecutionListenersOnScope(callActivityElement, activity);

            foreach (var parseListener in ParseListeners)
                parseListener.ParseCallActivity(callActivityElement, scope, activity);
            return activity;
        }

        protected internal virtual void ParseBinding(Element callActivityElement, ActivityImpl activity,
            BaseCallableElement callableElement, string bindingAttributeName)
        {
            var binding = callActivityElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                bindingAttributeName);

            if (BaseCallableElement.CallableElementBinding.Deployment.Value.Equals(binding))
                callableElement.Binding = BaseCallableElement.CallableElementBinding.Deployment;
            else if (BaseCallableElement.CallableElementBinding.Latest.Value.Equals(binding))
                callableElement.Binding = BaseCallableElement.CallableElementBinding.Latest;
            else if (BaseCallableElement.CallableElementBinding.Version.Value.Equals(binding))
                callableElement.Binding = BaseCallableElement.CallableElementBinding.Version;
        }

        protected internal virtual void ParseTenantId(Element callingActivityElement, ActivityImpl activity,
            BaseCallableElement callableElement, string attrName)
        {
            IParameterValueProvider tenantIdValueProvider;

            var tenantId = callingActivityElement.GetAttributeNS(CamundaBpmnExtensionsNs, attrName);
            if (!ReferenceEquals(tenantId, null) && (tenantId.Length > 0))
                tenantIdValueProvider = CreateParameterValueProvider(tenantId, ExpressionManager);
            else
                tenantIdValueProvider = new DefaultCallableElementTenantIdProvider();

            callableElement.TenantIdProvider = tenantIdValueProvider;
        }

        protected internal virtual void ParseVersion(Element callingActivityElement, ActivityImpl activity,
            BaseCallableElement callableElement, string bindingAttributeName, string versionAttributeName)
        {
            string version = null;

            var binding = callableElement.Binding;
            version = callingActivityElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                versionAttributeName);

            if ((binding != null) && binding.Equals(BaseCallableElement.CallableElementBinding.Version) &&
                ReferenceEquals(version, null))
                AddError(
                    "Missing attribute '" + versionAttributeName + "' when '" + bindingAttributeName + "' has value '" +
                    BaseCallableElement.CallableElementBinding.Version.Value + "'", callingActivityElement);

            var versionProvider = CreateParameterValueProvider(version, ExpressionManager);
            callableElement.VersionValueProvider = versionProvider;
        }

        protected internal virtual void ParseInputParameter(Element callActivityElement, ActivityImpl activity,
            CallableElement callableElement)
        {
            var extensionsElement = callActivityElement.element("extensionElements");

            if (extensionsElement != null)
                foreach (var inElement in extensionsElement.ElementsNS(CamundaBpmnExtensionsNs, "in"))
                {
                    var businessKey = inElement.GetAttributeValue("businessKey");

                    if (!ReferenceEquals(businessKey, null) && (businessKey.Length > 0))
                    {
                        var businessKeyValueProvider = CreateParameterValueProvider(businessKey, ExpressionManager);
                        callableElement.BusinessKeyValueProvider = businessKeyValueProvider;
                    }
                    else
                    {
                        var parameter = ParseCallableElementProvider(inElement);

                        if (AttributeValueEquals(inElement, "local", True))
                            parameter.ReadLocal = true;

                        callableElement.AddInput(parameter);
                    }
                }
        }

        protected internal virtual void ParseOutputParameter(Element callActivityElement, ActivityImpl activity,
            CallableElement callableElement)
        {
            var extensionsElement = callActivityElement.element("extensionElements");

            if (extensionsElement != null)
                foreach (var outElement in extensionsElement.ElementsNS(CamundaBpmnExtensionsNs, "out"))
                {
                    var parameter = ParseCallableElementProvider(outElement);

                    if (AttributeValueEquals(outElement, "local", True))
                        callableElement.AddOutputLocal(parameter);
                    else
                        callableElement.AddOutput(parameter);
                }
        }

        protected internal virtual bool AttributeValueEquals(Element element, string attribute, string comparisonValue)
        {
            var value = element.GetAttributeValue(attribute);

            return comparisonValue.Equals(value, StringComparison.CurrentCultureIgnoreCase);
        }

        protected internal virtual CallableElementParameter ParseCallableElementProvider(Element parameterElement)
        {
            var parameter = new CallableElementParameter();

            var variables = parameterElement.GetAttributeValue("variables");

            if (All.Equals(variables))
            {
                parameter.AllVariables = true;
            }
            else
            {
                var strictValidation = !Context.ProcessEngineConfiguration.DisableStrictCallActivityValidation;

                IParameterValueProvider sourceValueProvider = new NullValueProvider();

                var source = parameterElement.GetAttributeValue("source");
                if (!ReferenceEquals(source, null))
                    if (source.Length > 0)
                    {
                        sourceValueProvider = new ConstantValueProvider(source);
                    }
                    else
                    {
                        if (strictValidation)
                            AddError("Empty attribute 'source' when passing variables", parameterElement);
                        else
                            source = null;
                    }

                if (ReferenceEquals(source, null))
                {
                    source = parameterElement.GetAttributeValue("sourceExpression");

                    if (!ReferenceEquals(source, null))
                        if (source.Length > 0)
                        {
                            var expression = ExpressionManager.CreateExpression(source);
                            sourceValueProvider = new ElValueProvider(expression);
                        }
                        else if (strictValidation)
                        {
                            AddError("Empty attribute 'sourceExpression' when passing variables", parameterElement);
                        }
                }

                if (strictValidation && ReferenceEquals(source, null))
                    AddError("Missing parameter 'source' or 'sourceExpression' when passing variables", parameterElement);

                parameter.SourceValueProvider = sourceValueProvider;

                var target = parameterElement.GetAttributeValue("target");
                if ((strictValidation || (!ReferenceEquals(source, null) && (source.Length > 0))) &&
                    ReferenceEquals(target, null))
                    AddError("Missing attribute 'target' when attribute 'source' or 'sourceExpression' is set",
                        parameterElement);
                else if (strictValidation && !ReferenceEquals(target, null) && (target.Length == 0))
                    AddError("Empty attribute 'target' when attribute 'source' or 'sourceExpression' is set",
                        parameterElement);
                parameter.Target = target;
            }

            return parameter;
        }

        /// <summary>
        ///     Parses the properties of an element (if any) that can contain properties
        ///     (processes, activities, etc.)
        ///     Returns true if property subelemens are found.
        /// </summary>
        /// <param name="element">
        ///     The element that can contain properties.
        /// </param>
        /// <param name="activity">
        ///     The activity where the property declaration is done.
        /// </param>
        public virtual void ParseProperties(Element element, ActivityImpl activity)
        {
            var propertyElements = element.Elements("property");
            foreach (var propertyElement in propertyElements)
                ParseProperty(propertyElement, activity);
        }

        /// <summary>
        ///     Parses one property definition.
        /// </summary>
        /// <param name="propertyElement">
        ///     The 'property' element that defines how a property looks like and
        ///     is handled.
        /// </param>
        public virtual void ParseProperty(Element propertyElement, ActivityImpl activity)
        {
            var id = propertyElement.GetAttributeValue("id");
            var name = propertyElement.GetAttributeValue("name");

            // If name isn't given, use the id as name
            if (ReferenceEquals(name, null))
                if (ReferenceEquals(id, null))
                    AddError("Invalid property usage on line " + propertyElement.Line + ": no id or name specified.",
                        propertyElement);
                else
                    name = id;

            string type = null;
            ParsePropertyCustomExtensions(activity, propertyElement, name, type);
        }

        /// <summary>
        ///     Parses the custom extensions for properties.
        /// </summary>
        /// <param name="activity">
        ///     The activity where the property declaration is done.
        /// </param>
        /// <param name="propertyElement">
        ///     The 'property' element defining the property.
        /// </param>
        /// <param name="propertyName">
        ///     The name of the property.
        /// </param>
        /// <param name="propertyType">
        ///     The type of the property.
        /// </param>
        public virtual void ParsePropertyCustomExtensions(ActivityImpl activity, Element propertyElement,
            string propertyName, string propertyType)
        {
            if (ReferenceEquals(propertyType, null))
            {
                var type = propertyElement.GetAttributeNS(CamundaBpmnExtensionsNs, Type);
                propertyType = !ReferenceEquals(type, null) ? type : "string"; // default is string
            }

            var variableDeclaration = new VariableDeclaration(propertyName, propertyType);
            AddVariableDeclaration(activity, variableDeclaration);
            activity.IsScope = true;

            var src = propertyElement.GetAttributeNS(CamundaBpmnExtensionsNs, "src");
            if (!ReferenceEquals(src, null))
                variableDeclaration.SourceVariableName = src;

            var srcExpr = propertyElement.GetAttributeNS(CamundaBpmnExtensionsNs, "srcExpr");
            if (!ReferenceEquals(srcExpr, null))
            {
                var sourceExpression = ExpressionManager.CreateExpression(srcExpr);
                variableDeclaration.SourceExpression = sourceExpression;
            }

            var dst = propertyElement.GetAttributeNS(CamundaBpmnExtensionsNs, "dst");
            if (!ReferenceEquals(dst, null))
                variableDeclaration.DestinationVariableName = dst;

            var destExpr = propertyElement.GetAttributeNS(CamundaBpmnExtensionsNs, "dstExpr");
            if (!ReferenceEquals(destExpr, null))
            {
                var destinationExpression = ExpressionManager.CreateExpression(destExpr);
                variableDeclaration.DestinationExpression = destinationExpression;
            }

            var link = propertyElement.GetAttributeNS(CamundaBpmnExtensionsNs, "link");
            if (!ReferenceEquals(link, null))
                variableDeclaration.Link = link;

            var linkExpr = propertyElement.GetAttributeNS(CamundaBpmnExtensionsNs, "linkExpr");
            if (!ReferenceEquals(linkExpr, null))
            {
                var linkExpression = ExpressionManager.CreateExpression(linkExpr);
                variableDeclaration.LinkExpression = linkExpression;
            }

            foreach (var parseListener in ParseListeners)
                parseListener.ParseProperty(propertyElement, variableDeclaration, activity);
        }

        /// <summary>
        ///     Parses all sequence flow of a scope.
        /// </summary>
        /// <param name="processElement">
        ///     The 'process' element wherein the sequence flow are defined.
        /// </param>
        /// <param name="scope">
        ///     The scope to which the sequence flow must be added.
        /// </param>
        /// <param name="compensationHandlers"> </param>
        public virtual void ParseSequenceFlow(Element processElement, ScopeImpl scope,
            IDictionary<string, Element> compensationHandlers)
        {
            foreach (var sequenceFlowElement in processElement.Elements("sequenceFlow"))
            {
                var id = sequenceFlowElement.GetAttributeValue("id");
                var sourceRef = sequenceFlowElement.GetAttributeValue("sourceRef");
                var destinationRef = sequenceFlowElement.GetAttributeValue("targetRef");

                // check if destination is a throwing link event (event source) which mean
                // we have
                // to target the catching link event (event target) here:
                if (_eventLinkSources.ContainsKey(destinationRef))
                {
                    var linkName = _eventLinkSources[destinationRef];
                    destinationRef = _eventLinkTargets[linkName];
                    if (ReferenceEquals(destinationRef, null))
                    {
                        AddError(
                            "sequence flow points to link event source with name '" + linkName +
                            "' but no event target with that name exists. Most probably your link events are not configured correctly.",
                            sequenceFlowElement);
                        // we cannot do anything useful now
                        return;
                    }
                    // Reminder: Maybe we should log a warning if we use intermediate link
                    // events which are not used?
                    // e.g. we have a catching event without the corresponding throwing one.
                    // not done for the moment as it does not break executability
                }

                // Implicit check: sequence flow cannot cross (sub) process boundaries: we
                // don't do a processDefinition.findActivity here
                var sourceActivity = (ActivityImpl)scope.FindActivityAtLevelOfSubprocess(sourceRef);
                var destinationActivity = (ActivityImpl)scope.FindActivityAtLevelOfSubprocess(destinationRef);

                if (((sourceActivity == null) && compensationHandlers.ContainsKey(sourceRef)) ||
                    ((sourceActivity != null) && sourceActivity.CompensationHandler))
                {
                    AddError(
                        "Invalid outgoing sequence flow of compensation activity '" + sourceRef +
                        "'. A compensation activity should not have an incoming or outgoing sequence flow.",
                        sequenceFlowElement);
                }
                else if (((destinationActivity == null) && compensationHandlers.ContainsKey(destinationRef)) ||
                         ((destinationActivity != null) && destinationActivity.CompensationHandler))
                {
                    AddError(
                        "Invalid incoming sequence flow of compensation activity '" + destinationRef +
                        "'. A compensation activity should not have an incoming or outgoing sequence flow.",
                        sequenceFlowElement);
                }
                else if (sourceActivity == null)
                {
                    AddError("Invalid source '" + sourceRef + "' of sequence flow '" + id + "'", sequenceFlowElement);
                }
                else if (destinationActivity == null)
                {
                    AddError("Invalid destination '" + destinationRef + "' of sequence flow '" + id + "'",
                        sequenceFlowElement);
                }
                else if (sourceActivity.ActivityBehavior is EventBasedGatewayActivityBehavior)
                {
                    // ignore
                }
                else if (destinationActivity.ActivityBehavior is IntermediateCatchEventActivityBehavior &&
                         (destinationActivity.EventScope != null) &&
                         ((destinationActivity.EventScope as ActivityImpl).ActivityBehavior is EventBasedGatewayActivityBehavior))
                {
                    AddError(
                        "Invalid incoming sequenceflow for intermediateCatchEvent with id '" +
                        destinationActivity.Id + "' connected to an event-based gateway.", sequenceFlowElement);
                }
                else if (sourceActivity.ActivityBehavior is SubProcessActivityBehavior &&
                         sourceActivity.TriggeredByEvent)
                {
                    AddError("Invalid outgoing sequence flow of event subprocess", sequenceFlowElement);
                }
                else if (destinationActivity.ActivityBehavior is SubProcessActivityBehavior &&
                         destinationActivity.TriggeredByEvent)
                {
                    AddError("Invalid incoming sequence flow of event subprocess", sequenceFlowElement);
                }
                else
                {
                    if (GetMultiInstanceScope(sourceActivity) != null)
                        sourceActivity = GetMultiInstanceScope(sourceActivity);
                    if (GetMultiInstanceScope(destinationActivity) != null)
                        destinationActivity = GetMultiInstanceScope(destinationActivity);

                    var transition = sourceActivity.CreateOutgoingTransition(id);
                    if (id != null)
                    {
                        SequenceFlows.Add(id, transition);
                    }

                    transition.SetProperty("name", sequenceFlowElement.GetAttributeValue("name"));
                    transition.Name = sequenceFlowElement.GetAttributeValue("name");
                    transition.SetProperty("documentation", ParseDocumentation(sequenceFlowElement));
                    transition.SetDestination(destinationActivity);
                    ParseSequenceFlowConditionExpression(sequenceFlowElement, transition);
                    ParseExecutionListenersOnTransition(sequenceFlowElement, transition);

                    foreach (var parseListener in ParseListeners)
                        parseListener.ParseSequenceFlow(sequenceFlowElement, scope, transition);
                }
            }
        }

        /// <summary>
        ///     Parses a condition expression on a sequence flow.
        /// </summary>
        /// <param name="seqFlowElement">
        ///     The 'sequenceFlow' element that can contain a condition.
        /// </param>
        /// <param name="seqFlow">
        ///     The sequenceFlow object representation to which the condition must
        ///     be added.
        /// </param>
        public virtual void ParseSequenceFlowConditionExpression(Element seqFlowElement, TransitionImpl seqFlow)
        {
            var conditionExprElement = seqFlowElement.element(ConditionExpression);
            if (conditionExprElement != null)
            {
                var condition = ParseConditionExpression(conditionExprElement);
                seqFlow.SetProperty(PropertynameConditionText, conditionExprElement.Text.Trim());
                seqFlow.SetProperty(PropertynameCondition, condition);
            }
        }

        protected internal virtual ICondition ParseConditionExpression(Element conditionExprElement)
        {
            var expression = conditionExprElement.Text.Trim();
            var type = conditionExprElement.GetAttributeNS(XsiNs, Type);
            var language = conditionExprElement.GetAttributeValue(PropertynameLanguage);
            var resource = conditionExprElement.GetAttributeNS(CamundaBpmnExtensionsNs,
                PropertynameResource);
            if (!ReferenceEquals(type, null))
            {
                var value = type.Contains(":") ? ResolveName(type) : BpmnParser.Bpmn20Ns + ":" + type;
                if (!value.Equals(AttributevalueTFormalExpression))
                    AddError("Invalid type, only tFormalExpression is currently supported", conditionExprElement);
            }
            ICondition condition = null;
            if (ReferenceEquals(language, null))
                condition = new UelExpressionCondition(ExpressionManager.CreateExpression(expression));
            else
                try
                {
                    //TODO 自定义script扩展
                    //ExecutableScript script = ScriptUtil.getScript(language, expression, resource, expressionManager);
                    //condition = new ScriptCondition(script);
                }
                catch (ProcessEngineException e)
                {
                    AddError("Unable to process condition expression:" + e.Message, conditionExprElement);
                }
            return condition;
        }

        /// <summary>
        ///     Parses all execution-listeners on a scope.
        /// </summary>
        /// <param name="scopeElement">
        ///     the XML element containing the scope definition.
        /// </param>
        /// <param name="scope">
        ///     the scope to add the executionListeners to.
        /// </param>
        public virtual void ParseExecutionListenersOnScope(Element scopeElement, ScopeImpl scope)
        {
            var extentionsElement = scopeElement.element("extensionElements");
            if (extentionsElement != null)
            {
                //TODO (后期实现)自定义扩展解析入口
                //只解析包含指定uri executionListener属性
                var listenerElements = extentionsElement.ElementsNS(CamundaBpmnExtensionsNs,
                    "executionListener");
                foreach (var listenerElement in listenerElements)
                {
                    var eventName = listenerElement.GetAttributeValue("event");
                    if (IsValidEventNameForScope(eventName, listenerElement))
                    {
                        var listener = ParseExecutionListener(listenerElement);
                        if (listener != null)
                        {
                            scope.AddExecutionListener(eventName, listener);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Check if the given event name is valid. If not, an appropriate error is
        ///     added.
        /// </summary>
        protected internal virtual bool IsValidEventNameForScope(string eventName, Element listenerElement)
        {
            if (!ReferenceEquals(eventName, null) && (eventName.Trim().Length > 0))
            {
                if ("start".Equals(eventName) || "end".Equals(eventName))
                    return true;
                AddError("Attribute 'event' must be one of {start|end}", listenerElement);
            }
            else
            {
                AddError("Attribute 'event' is mandatory on listener", listenerElement);
            }
            return false;
        }

        public virtual void ParseExecutionListenersOnTransition(Element activitiElement, TransitionImpl activity)
        {
            var extensionElements = activitiElement.element("extensionElements");
            if (extensionElements != null)
            {
                var listenerElements = extensionElements.ElementsNS(CamundaBpmnExtensionsNs,
                    "executionListener");
                foreach (var listenerElement in listenerElements)
                {
                    var listener = ParseExecutionListener(listenerElement);
                    if (listener != null)
                    {
                        // Since a transition only fires event 'take', we don't parse the
                        // event attribute, it is ignored
                        //TODO 自定义扩展侦听器
                        activity.AddExecutionListener(listener);
                    }
                }
            }
        }

        /// <summary>
        ///     Parses an <seealso cref="ExecutionListener" /> implementation for the given
        ///     executionListener element.
        /// </summary>
        /// <param name="executionListenerElement">
        ///     the XML element containing the executionListener definition.
        /// </param>
        public virtual IDelegateListener<IBaseDelegateExecution> ParseExecutionListener(Element executionListenerElement)
        {
            IDelegateListener<IBaseDelegateExecution> executionListener = null;

            var className = executionListenerElement.GetAttributeValue(PropertynameClass);//解析 class="test.bpmn.Event.IntermediateNoneEventTest$MyExecutionListener" event="start"
            var expression = executionListenerElement.GetAttributeValue(PropertynameExpression);
            var delegateExpression = executionListenerElement.GetAttributeValue(PropertynameDelegateExpression);
            var scriptElement = executionListenerElement.ElementNs(CamundaBpmnExtensionsNs, "script");

            if (!ReferenceEquals(className, null))
                executionListener = new ClassDelegateExecutionListener(className,
                    ParseFieldDeclarations(executionListenerElement));
            else if (!ReferenceEquals(expression, null))
                executionListener = new ExpressionExecutionListener(ExpressionManager.CreateExpression(expression));
            else if (!ReferenceEquals(delegateExpression, null))
                executionListener =
                    new DelegateExpressionExecutionListener(ExpressionManager.CreateExpression(delegateExpression),
                        ParseFieldDeclarations(executionListenerElement));
            else if (scriptElement != null)
                try
                {
                    //TODO 自定义Script处理 需要侦听器
                    ExecutableScript executableScript = BpmnParseUtil.ParseCamundaScript(scriptElement);
                    if (executableScript != null)
                    {
                        executionListener = new ScriptExecutionListener(executableScript);
                    }
                }
                catch (BpmnParseException e)
                {
                    AddError(e);
                }
            else
                AddError(
                    "Element 'class', 'expression', 'delegateExpression' or 'script' is mandatory on executionListener",
                    executionListenerElement);
            return executionListener;
        }

        // Diagram interchange
        // /////////////////////////////////////////////////////////////////

        public virtual void ParseDiagramInterchangeElements()
        {
            // Multiple BPMNDiagram possible
            var diagrams = rootElement.ElementsNS(BpmnDiNs.NamespaceUri, "BPMNDiagram");
            //var diagrams = rootElement.elementsNS("", "BPMNDiagram");
            if (diagrams.Count > 0)
                foreach (var diagramElement in diagrams)
                    ParseBpmnDiagram(diagramElement);
        }

        public virtual void ParseBpmnDiagram(Element bpmndiagramElement)
        {
            // Each BPMNdiagram needs to have exactly one BPMNPlane
            var bpmnPlane = bpmndiagramElement.ElementNs(BpmnDiNs, "BPMNPlane");
            if (bpmnPlane != null)
                ParseBpmnPlane(bpmnPlane);
        }

        public virtual void ParseBpmnPlane(Element bpmnPlaneElement)
        {
            var bpmnElement = bpmnPlaneElement.GetAttributeValue("bpmnElement");
            if (!ReferenceEquals(bpmnElement, null) && !"".Equals(bpmnElement))
            {
                // there seems to be only on process without collaboration
                if (GetProcessDefinition(bpmnElement) != null)
                    GetProcessDefinition(bpmnElement).GraphicalNotationDefined = true;

                var shapes = bpmnPlaneElement.ElementsNS(BpmnDiNs.NamespaceUri, "BPMNShape");
                foreach (var shape in shapes)
                    ParseBpmnShape(shape);

                var edges = bpmnPlaneElement.ElementsNS(BpmnDiNs.NamespaceUri, "BPMNEdge");
                foreach (var edge in edges)
                    ParseBpmnEdge(edge);
            }
            else
            {
                AddError("'bpmnElement' attribute is required on BPMNPlane ", bpmnPlaneElement);
            }
        }

        public virtual void ParseBpmnShape(Element bpmnShapeElement)
        {
            var bpmnElement = bpmnShapeElement.GetAttributeValue("bpmnElement");

            if (!ReferenceEquals(bpmnElement, null) && !"".Equals(bpmnElement))
            {
                // For collaborations, their are also shape definitions for the
                // participants / processes
                //if (!ReferenceEquals(participantProcesses[bpmnElement], null))
                if (ParticipantProcesses.ContainsKey(bpmnElement))
                {
                    var procDef = GetProcessDefinition(ParticipantProcesses[bpmnElement]);
                    procDef.GraphicalNotationDefined = true;

                    // The participation that references this process, has a bounds to be
                    // rendered + a name as wel
                    ParseDiBounds(bpmnShapeElement, procDef.ParticipantProcess);
                    return;
                }

                foreach (var processDefinition in ProcessDefinitions)
                {
                    var activity = (ActivityImpl)processDefinition.FindActivity(bpmnElement);
                    if (activity != null)
                    {
                        ParseDiBounds(bpmnShapeElement, activity);

                        // collapsed or expanded
                        var isExpanded = bpmnShapeElement.GetAttributeValue("isExpanded");
                        if (!ReferenceEquals(isExpanded, null))
                            activity.SetProperty(PropertynameIsexpanded, ParseBooleanAttribute(isExpanded));
                    }
                    else
                    {
                        var lane = processDefinition.GetLaneForId(bpmnElement);

                        if (lane != null)
                            ParseDiBounds(bpmnShapeElement, lane);
                        else if (!ElementIds.Contains(bpmnElement))
                            AddError(
                                "Invalid reference in 'bpmnElement' attribute, activity " + bpmnElement + "not found",
                                bpmnShapeElement);
                    }
                }
            }
            else
            {
                AddError("'bpmnElement' attribute is required on BPMNShape", bpmnShapeElement);
            }
        }

        protected internal virtual void ParseDiBounds(Element bpmnShapeElement, IHasDiBounds target)
        {
            var bounds = bpmnShapeElement.ElementNs(BpmnDcNs, "Bounds");
            if (bounds != null)
            {
                target.X = (int)ParseDoubleAttribute(bpmnShapeElement, "x", bounds.GetAttributeValue("x"), true).Value;
                target.Y = (int)ParseDoubleAttribute(bpmnShapeElement, "y", bounds.GetAttributeValue("y"), true).Value;
                target.Width =
                    (int)ParseDoubleAttribute(bpmnShapeElement, "width", bounds.GetAttributeValue("width"), true).Value;
                target.Height =
                    (int)
                    ParseDoubleAttribute(bpmnShapeElement, "height", bounds.GetAttributeValue("height"), true).Value;
            }
            else
            {
                AddError("'Bounds' element is required", bpmnShapeElement);
            }
        }

        public virtual void ParseBpmnEdge(Element bpmnEdgeElement)
        {
            var sequenceFlowId = bpmnEdgeElement.GetAttributeValue("bpmnElement");
            if (!ReferenceEquals(sequenceFlowId, null) && !"".Equals(sequenceFlowId))
            {
                if ((SequenceFlows != null) && SequenceFlows.ContainsKey(sequenceFlowId))
                {
                    var sequenceFlow = SequenceFlows[sequenceFlowId];
                    var waypointElements = bpmnEdgeElement.ElementsNS(OmgDiNs.NamespaceUri, "waypoint");
                    if (waypointElements.Count >= 2)
                    {
                        IList<int> waypoints = new List<int>();
                        foreach (var waypointElement in waypointElements)
                        {
                            waypoints.Add(
                                (int)
                                ParseDoubleAttribute(waypointElement, "x", waypointElement.GetAttributeValue("x"), true)
                                    .Value);
                            waypoints.Add(
                                (int)
                                ParseDoubleAttribute(waypointElement, "y", waypointElement.GetAttributeValue("y"), true)
                                    .Value);
                        }
                        sequenceFlow.Waypoints = waypoints;
                    }
                    else
                    {
                        AddError("Minimum 2 waypoint elements must be definted for a 'BPMNEdge'", bpmnEdgeElement);
                    }
                }
                else if (!ElementIds.Contains(sequenceFlowId))
                {
                    // it might not be a
                    // sequenceFlow but it
                    // might still
                    // reference
                    // 'something'
                    AddError(
                        "Invalid reference in 'bpmnElement' attribute, sequenceFlow " + sequenceFlowId + "not found",
                        bpmnEdgeElement);
                }
            }
            else
            {
                AddError("'bpmnElement' attribute is required on BPMNEdge", bpmnEdgeElement);
            }
        }

        public virtual ProcessDefinitionEntity GetProcessDefinition(string processDefinitionKey)
        {
            foreach (var processDefinition in processDefinitions)
                if (processDefinition.Key.Equals(processDefinitionKey))
                    return processDefinition;
            return null;
        }

        //添加 暴露连接线
        public IDictionary<string, TransitionImpl> GetSequenceFlows()
        {
            return SequenceFlows;
        }

        public new BpmnParse SetName(string name)
        {
            base.SetName(name);
            return this;
        }
        //JAVA语法不同，重写父类C#必须返回类型相同
        //TODO 统一写入xml数据源入口
        public new BpmnParse SourceInputStream(Stream inputStream)
        {
            base.SourceInputStream(inputStream);

            var xml = new XmlDocument();
            xml.Load(XmlReader.Create(inputStream, XmlReaderSettings));
            rootElement = new Element(xml.DocumentElement, null);
            xml = null;
            return this;
        }

        /// <summary>
        ///     xml数据源的bpmn2.0协议认证,错误和warring会保存到变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bpmn20_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
                BpmnErrors.Add(e.Exception.SourceUri, new Error { Id = e.Exception.SourceUri, ErrorCode = e.Message });
            if (e.Severity == XmlSeverityType.Warning)
                Warnings.Add(new Problem(e.Message, e.Exception.SourceUri, (Element)sender));
        }
        //JAVA语法不同，重写父类C#必须返回类型相同
        public new BpmnParse SourceResource(string resource)
        {
            base.SourceResource(resource);
            return this;
        }

        public new BpmnParse SourceString(string @string)
        {
            base.SourceString(@string);
            return this;
        }

        public new BpmnParse SourceUrl(string url)
        {
            base.SourceUrl(url);
            return this;
        }

        public new BpmnParse SourceUrl(Uri url)
        {
            base.SourceUrl(url);
            return this;
        }

        public virtual bool? ParseBooleanAttribute(string booleanText, bool defaultValue)
        {
            if (ReferenceEquals(booleanText, null))
                return defaultValue;
            return ParseBooleanAttribute(booleanText);
        }

        public virtual bool? ParseBooleanAttribute(string booleanText)
        {
            if (True.Equals(booleanText, StringComparison.CurrentCultureIgnoreCase) 
                || "enabled".Equals(booleanText, StringComparison.CurrentCultureIgnoreCase) 
                || "on".Equals(booleanText, StringComparison.CurrentCultureIgnoreCase) 
                || "active".Equals(booleanText, StringComparison.CurrentCultureIgnoreCase) 
                || "yes".Equals(booleanText))
                return true;
            if ("false".Equals(booleanText, StringComparison.CurrentCultureIgnoreCase) 
                || "disabled".Equals(booleanText, StringComparison.CurrentCultureIgnoreCase) 
                || "off".Equals(booleanText, StringComparison.CurrentCultureIgnoreCase) 
                || "inactive".Equals(booleanText, StringComparison.CurrentCultureIgnoreCase) 
                || "no".Equals(booleanText, StringComparison.CurrentCultureIgnoreCase))
                return false;
            return null;
        }

        public virtual double? ParseDoubleAttribute(Element element, string attributeName, string doubleText,
            bool required)
        {
            if (required && (ReferenceEquals(doubleText, null) || "".Equals(doubleText)))
                AddError(attributeName + " is required", element);
            else
                try
                {
                    return double.Parse(doubleText);
                }
                catch (FormatException e)
                {
                    AddError("Cannot parse " + attributeName + ": " + e.Message, element);
                }
            return -1.0;
        }

        protected internal virtual bool IsExclusive(Element element)
        {
            return True.Equals(element.GetAttributeNS(CamundaBpmnExtensionsNs, "exclusive",JobEntity.DefaultExclusive.ToString()), StringComparison.CurrentCultureIgnoreCase);
        }

        protected internal virtual bool IsAsyncBefore(Element element)
        {
            return True.Equals(element.GetAttributeNS(CamundaBpmnExtensionsNs, "async"), StringComparison.CurrentCultureIgnoreCase) ||
                   True.Equals(element.GetAttributeNS(CamundaBpmnExtensionsNs, "asyncBefore"), StringComparison.CurrentCultureIgnoreCase);
        }

        protected internal virtual bool IsAsyncAfter(Element element)
        {
            return True.Equals(element.GetAttributeNS(CamundaBpmnExtensionsNs, "asyncAfter"), StringComparison.CurrentCultureIgnoreCase);
        }

        protected internal virtual bool IsServiceTaskLike(Element element)
        {
            return
                !ReferenceEquals(
                    element.GetAttributeNS(CamundaBpmnExtensionsNs, PropertynameClass), null) ||
                !ReferenceEquals(
                    element.GetAttributeNS(CamundaBpmnExtensionsNs, PropertynameExpression), null) ||
                !ReferenceEquals(
                    element.GetAttributeNS(CamundaBpmnExtensionsNs, PropertynameDelegateExpression),
                    null) ||
                !ReferenceEquals(element.GetAttributeNS(CamundaBpmnExtensionsNs, Type), null) ||
                HasConnector(element);
        }

        protected internal virtual bool HasConnector(Element element)
        {
            var extensionElements = element.element("extensionElements");
            return (extensionElements != null) && (extensionElements.element("connector") != null);
        }

        // IoMappings ////////////////////////////////////////////////////////

        protected internal virtual void ParseActivityInputOutput(Element activityElement, ActivityImpl activity)
        {
            var extensionElements = activityElement.element("extensionElements");
            if (extensionElements != null)
            {
                IoMapping inputOutput = null;
                try
                {
                    inputOutput = BpmnParseUtil.ParseInputOutput(extensionElements);
                }
                catch (BpmnParseException e)
                {
                    AddError(e);
                }

                if (inputOutput != null)
                    if (CheckActivityInputOutputSupported(activityElement, activity, inputOutput))
                    {
                        activity.ioMapping = inputOutput;

                        if (GetMultiInstanceScope(activity) == null)
                            activity.IsScope = true;
                    }
            }
        }

        protected internal virtual bool CheckActivityInputOutputSupported(Element activityElement, ActivityImpl activity,
            IoMapping inputOutput)
        {
            var tagName = activityElement.TagName.ToLower();

            if (
                !(tagName.Contains("task") || tagName.Contains("event") || tagName.Equals("transaction") ||
                  tagName.Equals("subprocess") || tagName.Equals("callactivity")))
            {
                AddError("camunda:inputOutput mapping unsupported for element type '" + tagName + "'.", activityElement);
                return false;
            }

            if (tagName.Equals("subprocess") && True.Equals(activityElement.GetAttributeValue("triggeredbyevent"), StringComparison.CurrentCultureIgnoreCase))
            {
                AddError(
                    "camunda:inputOutput mapping unsupported for element type '" + tagName +
                    "' with attribute 'triggeredByEvent = true'.", activityElement);
                return false;
            }

            if ((inputOutput.OutputParameters != null) && (inputOutput.OutputParameters.Count > 0))
                return CheckActivityOutputParameterSupported(activityElement, activity);
            return true;
        }

        protected internal virtual bool CheckActivityOutputParameterSupported(Element activityElement,
            ActivityImpl activity)
        {
            var tagName = activityElement.TagName.ToLower();

            if (tagName.Equals("endevent"))
            {
                AddError("camunda:outputParameter not allowed for element type '" + tagName + "'.", activityElement);
                return true;
            }
            if (GetMultiInstanceScope(activity) != null)
            {
                AddError("camunda:outputParameter not allowed for multi-instance constructs", activityElement);
                return false;
            }
            return true;
        }

        protected internal virtual void EnsureNoIoMappingDefined(Element element)
        {
            var inputOutput = BpmnParseUtil.FindCamundaExtensionElement(element, "inputOutput");
            if (inputOutput != null)
                AddError("camunda:inputOutput mapping unsupported for element type '" + element.TagName + "'.", element);
        }

        protected internal virtual IParameterValueProvider CreateParameterValueProvider(object value,
            ExpressionManager expressionManager)
        {
            if (value == null)
                return new NullValueProvider();
            if (value is string)
            {
                var expression = expressionManager.CreateExpression((string)value);
                return new ElValueProvider(expression);
            }
            return new ConstantValueProvider(value);
        }

        protected internal virtual void AddTimeCycleWarning(Element timeCycleElement, string type)
        {
            var warning = "It is not recommended to use a " + type + " timer event with a time cycle.";
            AddWarning(warning, timeCycleElement);
        }

        protected internal virtual void EnsureNoExpressionInMessageStartEvent(Element element,
            EventSubscriptionDeclaration messageStartEventSubscriptionDeclaration)
        {
            var eventNameContainsExpression = false;
            if (messageStartEventSubscriptionDeclaration.HasEventName())
                eventNameContainsExpression = !messageStartEventSubscriptionDeclaration.EventNameLiteralText;
            if (eventNameContainsExpression)
            {
                var messageStartName = messageStartEventSubscriptionDeclaration.UnresolvedEventName;
                AddError(
                    "Invalid message name '" + messageStartName + "' for element '" + element.TagName +
                    "': expressions in the message start event name are not allowed!", element);
            }
        }

        private class BacklogErrorCallbackAnonymousInnerClass : ScopeImpl.IBacklogErrorCallback
        {
            private readonly string _activityRef;

            private readonly Element _compensateEventDefinitionElement;
            private readonly BpmnParse _outerInstance;
            private readonly string _scopeId;

            public BacklogErrorCallbackAnonymousInnerClass(BpmnParse outerInstance,
                Element compensateEventDefinitionElement, string activityRef, string scopeId)
            {
                this._outerInstance = outerInstance;
                this._compensateEventDefinitionElement = compensateEventDefinitionElement;
                this._activityRef = activityRef;
                this._scopeId = scopeId;
            }


            public virtual void Callback()
            {
                _outerInstance.AddError(
                    "Invalid attribute value for 'activityRef': no activity with id '" + _activityRef + "' in scope '" +
                    _scopeId + "'", _compensateEventDefinitionElement);
            }
        }

        private class AsyncAfterUpdateAnonymousInnerClass : ActivityImpl.IAsyncAfterUpdate
        {
            private readonly ActivityImpl _activity;
            private readonly BpmnParse _outerInstance;

            public AsyncAfterUpdateAnonymousInnerClass(BpmnParse outerInstance, ActivityImpl activity)
            {
                this._outerInstance = outerInstance;
                this._activity = activity;
            }

            public virtual void UpdateAsyncAfter(bool asyncAfter, bool exclusive)
            {
                if (asyncAfter)
                    _outerInstance.AddMessageJobDeclaration(new AsyncAfterMessageJobDeclaration(), _activity, exclusive);
                else
                    _outerInstance.RemoveMessageJobDeclarationWithJobConfiguration(_activity,
                        MessageJobDeclaration.AsyncAfter);
            }
        }

        private class AsyncBeforeUpdateAnonymousInnerClass : ActivityImpl.IAsyncBeforeUpdate
        {
            private readonly ActivityImpl _activity;
            private readonly BpmnParse _outerInstance;

            public AsyncBeforeUpdateAnonymousInnerClass(BpmnParse outerInstance, ActivityImpl activity)
            {
                this._outerInstance = outerInstance;
                this._activity = activity;
            }

            public virtual void UpdateAsyncBefore(bool asyncBefore, bool exclusive)
            {
                if (asyncBefore)
                    _outerInstance.AddMessageJobDeclaration(new AsyncBeforeMessageJobDeclaration(), _activity, exclusive);
                else
                    _outerInstance.RemoveMessageJobDeclarationWithJobConfiguration(_activity,
                        MessageJobDeclaration.AsyncBefore);
            }
        }
    }
}