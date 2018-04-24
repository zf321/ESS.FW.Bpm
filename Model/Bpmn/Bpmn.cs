using System.IO;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.impl;
using ESS.FW.Bpm.Model.Bpmn.impl.instance;
using ESS.FW.Bpm.Model.Bpmn.impl.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda;
using ESS.FW.Bpm.Model.Bpmn.impl.instance.dc;
using ESS.FW.Bpm.Model.Bpmn.impl.instance.di;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ExtensionImpl = ESS.FW.Bpm.Model.Bpmn.impl.instance.ExtensionImpl;
using ESS.FW.Bpm.Model.Xml.impl.util;

namespace ESS.FW.Bpm.Model.Bpmn
{
    /// <summary>
    /// <para>Provides access to the camunda BPMN model api.</para>
    /// 
    /// 
    /// 
    /// </summary>
    public class Bpmn
    {

        /// <summary>
        /// the singleton instance of <seealso cref="Bpmn"/>. If you want to customize the behavior of Bpmn,
        /// replace this instance with an instance of a custom subclass of <seealso cref="Bpmn"/>. 
        /// </summary>
        public static Bpmn Instance = new Bpmn();

        /// <summary>
        /// the parser used by the Bpmn implementation. </summary>
        private BpmnParser _bpmnParser = new BpmnParser();

        /// <summary>
        /// Allows reading a <seealso cref="IBpmnModelInstance"/> from a File.
        /// </summary>
        /// <param name="file"> the <seealso cref="File"/> to read the <seealso cref="IBpmnModelInstance"/> from </param>
        /// <returns> the model read </returns>
        /// <exception cref="BpmnModelException"> if the model cannot be read </exception>
        public static IBpmnModelInstance ReadModelFromFile(string file)
        {
            return Instance.DoReadModelFromFile(file);
        }

        /// <summary>
        /// Allows reading a <seealso cref="IBpmnModelInstance"/> from an <seealso cref="InputStream"/>
        /// </summary>
        /// <param name="stream"> the <seealso cref="InputStream"/> to read the <seealso cref="IBpmnModelInstance"/> from </param>
        /// <returns> the model read </returns>
        /// <exception cref="ModelParseException"> if the model cannot be read </exception>
        public static IBpmnModelInstance ReadModelFromStream(System.IO.Stream stream)
        {
            return Instance.DoReadModelFromInputStream(stream);
        }

        /// <summary>
        /// Allows writing a <seealso cref="IBpmnModelInstance"/> to a File. It will be
        /// validated before writing.
        /// </summary>
        /// <param name="file"> the <seealso cref="File"/> to write the <seealso cref="IBpmnModelInstance"/> to </param>
        /// <param name="modelInstance"> the <seealso cref="IBpmnModelInstance"/> to write </param>
        /// <exception cref="BpmnModelException"> if the model cannot be written </exception>
        /// <exception cref="ModelValidationException"> if the model is not valid </exception>
        public static void WriteModelToFile(string file, IBpmnModelInstance modelInstance)
        {
            Instance.DoWriteModelToFile(file, modelInstance);
        }

        /// <summary>
        /// Allows writing a <seealso cref="IBpmnModelInstance"/> to an <seealso cref="OutputStream"/>. It will be
        /// validated before writing.
        /// </summary>
        /// <param name="stream"> the <seealso cref="OutputStream"/> to write the <seealso cref="IBpmnModelInstance"/> to </param>
        /// <param name="modelInstance"> the <seealso cref="IBpmnModelInstance"/> to write </param>
        /// <exception cref="ModelException"> if the model cannot be written </exception>
        /// <exception cref="ModelValidationException"> if the model is not valid </exception>
        public static void WriteModelToStream(System.IO.Stream stream, IBpmnModelInstance modelInstance)
        {
            Instance.DoWriteModelToOutputStream(stream, modelInstance);
        }

        /// <summary>
        /// Allows the conversion of a <seealso cref="IBpmnModelInstance"/> to an <seealso cref="string"/>. It will
        /// be validated before conversion.
        /// </summary>
        /// <param name="modelInstance">  the model instance to convert </param>
        /// <returns> the XML string representation of the model instance </returns>
        public static string ConvertToString(IBpmnModelInstance modelInstance)
        {
            return Instance.DoConvertToString(modelInstance);
        }

        /// <summary>
        /// Validate model DOM document
        /// </summary>
        /// <param name="modelInstance"> the <seealso cref="IBpmnModelInstance"/> to validate </param>
        /// <exception cref="ModelValidationException"> if the model is not valid </exception>
        public static void ValidateModel(IBpmnModelInstance modelInstance)
        {
            Instance.DoValidateModel(modelInstance);
        }

        /// <summary>
        /// Allows creating an new, empty <seealso cref="IBpmnModelInstance"/>.
        /// </summary>
        /// <returns> the empty model. </returns>
        public static IBpmnModelInstance CreateEmptyModel()
        {
            return Instance.DoCreateEmptyModel();
        }

        public static IProcessBuilder CreateProcess()
        {
            IBpmnModelInstance modelInstance = Instance.DoCreateEmptyModel();
            IDefinitions definitions = modelInstance.NewInstance<IDefinitions>(typeof(IDefinitions));
            definitions.TargetNamespace = BpmnModelConstants.Bpmn20Ns;
            definitions.DomElement.RegisterNamespace("camunda", BpmnModelConstants.CamundaNs);
            modelInstance.Definitions = definitions;
            IProcess process = modelInstance.NewInstance<IProcess>(typeof(IProcess));
            definitions.AddChildElement(process);

            IBpmnDiagram bpmnDiagram = modelInstance.NewInstance<IBpmnDiagram>(typeof(IBpmnDiagram));

            IBpmnPlane bpmnPlane = modelInstance.NewInstance<IBpmnPlane>(typeof(IBpmnPlane));
            bpmnPlane.BpmnElement = process;

            bpmnDiagram.AddChildElement(bpmnPlane);
            definitions.AddChildElement(bpmnDiagram);

            return process.Builder();
        }

        public static IProcessBuilder CreateProcess(string processId)
        {
            return CreateProcess().Id<IProcessBuilder>(processId);
        }

        public static IProcessBuilder CreateExecutableProcess()
        {
            return CreateProcess().Executable();
        }

        public static IProcessBuilder CreateExecutableProcess(string processId)
        {
            return CreateProcess(processId).Executable();
        }


        /// <summary>
        /// Register known types of the BPMN model
        /// </summary>
        protected internal Bpmn()
        {
            BpmnModelBuilder = ModelBuilder.CreateInstance("BPMN Model");
            BpmnModelBuilder.AlternativeNamespace(BpmnModelConstants.ActivitiNs, BpmnModelConstants.CamundaNs);
            DoRegisterTypes(BpmnModelBuilder);
            BpmnModel = BpmnModelBuilder.Build();
        }

        protected internal virtual IBpmnModelInstance DoReadModelFromFile(string file)
        {
            System.IO.Stream stream = null;
            try
            {
                stream = new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                return DoReadModelFromInputStream(stream);

            }
            catch (FileNotFoundException)
            {
                throw new BpmnModelException("Cannot read model from file " + file + ": file does not exist.");

            }
            finally
            {
                //IoUtil.closeSilently(@is);
                stream.Dispose();
            }
        }

        protected internal virtual IBpmnModelInstance DoReadModelFromInputStream(System.IO.Stream @is)
        {
            return (IBpmnModelInstance) _bpmnParser.ParseModelFromStream(@is);
        }

        protected internal virtual void DoWriteModelToFile(string file, IBpmnModelInstance modelInstance)
        {
            System.IO.Stream os = null;
            try
            {
                os = new System.IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                DoWriteModelToOutputStream(os, modelInstance);
            }
            catch (FileNotFoundException)
            {
                throw new BpmnModelException("Cannot write model to file " + file + ": file does not exist.");
            }
            finally
            {
                os.Dispose();
            }
        }

        protected internal virtual void DoWriteModelToOutputStream(Stream os, IBpmnModelInstance modelInstance)
        {
            // validate DOM document
            DoValidateModel(modelInstance);
            // write XML
            IoUtil.WriteDocumentToOutputStream(modelInstance.Document, os);
        }

        protected internal virtual string DoConvertToString(IBpmnModelInstance modelInstance)
        {
            // validate DOM document
            DoValidateModel(modelInstance);
            // convert to XML string
            return IoUtil.ConvertXmlDocumentToString(modelInstance.Document);
        }

        protected internal virtual void DoValidateModel(IBpmnModelInstance modelInstance)
        {
            _bpmnParser.ValidateModel(modelInstance.Document);
        }

        protected internal virtual IBpmnModelInstance DoCreateEmptyModel()
        {
            return (IBpmnModelInstance) _bpmnParser.EmptyModel;
        }

        protected internal virtual void DoRegisterTypes(ModelBuilder bpmnModelBuilder)
        {
            ActivationConditionImpl.RegisterType(bpmnModelBuilder);
            ActivityImpl.RegisterType(bpmnModelBuilder);
            ArtifactImpl.RegisterType(bpmnModelBuilder);
            AssignmentImpl.RegisterType(bpmnModelBuilder);
            AssociationImpl.RegisterType(bpmnModelBuilder);
            AuditingImpl.RegisterType(bpmnModelBuilder);
            BaseElementImpl.RegisterType(bpmnModelBuilder);
            BoundaryEventImpl.RegisterType(bpmnModelBuilder);
            BusinessRuleTaskImpl.RegisterType(bpmnModelBuilder);
            CallableElementImpl.RegisterType(bpmnModelBuilder);
            CallActivityImpl.RegisterType(bpmnModelBuilder);
            CallConversationImpl.RegisterType(bpmnModelBuilder);
            CancelEventDefinitionImpl.RegisterType(bpmnModelBuilder);
            CatchEventImpl.RegisterType(bpmnModelBuilder);
            CategoryValueImpl.RegisterType(bpmnModelBuilder);
            CategoryValueRef.RegisterType(bpmnModelBuilder);
            ChildLaneSet.RegisterType(bpmnModelBuilder);
            CollaborationImpl.RegisterType(bpmnModelBuilder);
            CompensateEventDefinitionImpl.RegisterType(bpmnModelBuilder);
            ConditionImpl.RegisterType(bpmnModelBuilder);
            ConditionalEventDefinitionImpl.RegisterType(bpmnModelBuilder);
            CompletionConditionImpl.RegisterType(bpmnModelBuilder);
            ComplexBehaviorDefinitionImpl.RegisterType(bpmnModelBuilder);
            ComplexGatewayImpl.RegisterType(bpmnModelBuilder);
            ConditionExpressionImpl.RegisterType(bpmnModelBuilder);
            ConversationAssociationImpl.RegisterType(bpmnModelBuilder);
            ConversationImpl.RegisterType(bpmnModelBuilder);
            ConversationLinkImpl.RegisterType(bpmnModelBuilder);
            ConversationNodeImpl.RegisterType(bpmnModelBuilder);
            CorrelationKeyImpl.RegisterType(bpmnModelBuilder);
            CorrelationPropertyBindingImpl.RegisterType(bpmnModelBuilder);
            CorrelationPropertyImpl.RegisterType(bpmnModelBuilder);
            CorrelationPropertyRef.RegisterType(bpmnModelBuilder);
            CorrelationPropertyRetrievalExpressionImpl.RegisterType(bpmnModelBuilder);
            CorrelationSubscriptionImpl.RegisterType(bpmnModelBuilder);
            DataAssociationImpl.RegisterType(bpmnModelBuilder);
            DataInputAssociationImpl.RegisterType(bpmnModelBuilder);
            DataInputImpl.RegisterType(bpmnModelBuilder);
            DataInputRefs.RegisterType(bpmnModelBuilder);
            DataOutputAssociationImpl.RegisterType(bpmnModelBuilder);
            DataOutputImpl.RegisterType(bpmnModelBuilder);
            DataOutputRefs.RegisterType(bpmnModelBuilder);
            DataPath.RegisterType(bpmnModelBuilder);
            DataStateImpl.RegisterType(bpmnModelBuilder);
            DataObjectImpl.RegisterType(bpmnModelBuilder);
            DataObjectReferenceImpl.RegisterType(bpmnModelBuilder);
            DataStoreImpl.RegisterType(bpmnModelBuilder);
            DefinitionsImpl.RegisterType(bpmnModelBuilder);
            DocumentationImpl.RegisterType(bpmnModelBuilder);
            EndEventImpl.RegisterType(bpmnModelBuilder);
            EndPointImpl.RegisterType(bpmnModelBuilder);
            EndPointRef.RegisterType(bpmnModelBuilder);
            ErrorEventDefinitionImpl.RegisterType(bpmnModelBuilder);
            ErrorImpl.RegisterType(bpmnModelBuilder);
            ErrorRef.RegisterType(bpmnModelBuilder);
            EscalationImpl.RegisterType(bpmnModelBuilder);
            EscalationEventDefinitionImpl.RegisterType(bpmnModelBuilder);
            EventBasedGatewayImpl.RegisterType(bpmnModelBuilder);
            EventDefinitionImpl.RegisterType(bpmnModelBuilder);
            EventDefinitionRef.RegisterType(bpmnModelBuilder);
            EventImpl.RegisterType(bpmnModelBuilder);
            ExclusiveGatewayImpl.RegisterType(bpmnModelBuilder);
            ExpressionImpl.RegisterType(bpmnModelBuilder);
            ExtensionElementsImpl.RegisterType(bpmnModelBuilder);
            ExtensionImpl.RegisterType(bpmnModelBuilder);
            FlowElementImpl.RegisterType(bpmnModelBuilder);
            FlowNodeImpl.RegisterType(bpmnModelBuilder);
            FlowNodeRef.RegisterType(bpmnModelBuilder);
            FormalExpressionImpl.RegisterType(bpmnModelBuilder);
            From.RegisterType(bpmnModelBuilder);
            GatewayImpl.RegisterType(bpmnModelBuilder);
            GlobalConversationImpl.RegisterType(bpmnModelBuilder);
            HumanPerformerImpl.RegisterType(bpmnModelBuilder);
            ImportImpl.RegisterType(bpmnModelBuilder);
            InclusiveGatewayImpl.RegisterType(bpmnModelBuilder);
            Incoming.RegisterType(bpmnModelBuilder);
            InMessageRef.RegisterType(bpmnModelBuilder);
            InnerParticipantRef.RegisterType(bpmnModelBuilder);
            InputDataItemImpl.RegisterType(bpmnModelBuilder);
            InputSetImpl.RegisterType(bpmnModelBuilder);
            InputSetRefs.RegisterType(bpmnModelBuilder);
            InteractionNodeImpl.RegisterType(bpmnModelBuilder);
            InterfaceImpl.RegisterType(bpmnModelBuilder);
            InterfaceRef.RegisterType(bpmnModelBuilder);
            IntermediateCatchEventImpl.RegisterType(bpmnModelBuilder);
            IntermediateThrowEventImpl.RegisterType(bpmnModelBuilder);
            IoBindingImpl.RegisterType(bpmnModelBuilder);
            IoSpecificationImpl.RegisterType(bpmnModelBuilder);
            ItemAwareElementImpl.RegisterType(bpmnModelBuilder);
            ItemDefinitionImpl.RegisterType(bpmnModelBuilder);
            LaneImpl.RegisterType(bpmnModelBuilder);
            LaneSetImpl.RegisterType(bpmnModelBuilder);
            LinkEventDefinitionImpl.RegisterType(bpmnModelBuilder);
            LoopCardinalityImpl.RegisterType(bpmnModelBuilder);
            LoopCharacteristicsImpl.RegisterType(bpmnModelBuilder);
            LoopDataInputRef.RegisterType(bpmnModelBuilder);
            LoopDataOutputRef.RegisterType(bpmnModelBuilder);
            ManualTaskImpl.RegisterType(bpmnModelBuilder);
            MessageEventDefinitionImpl.RegisterType(bpmnModelBuilder);
            MessageFlowAssociationImpl.RegisterType(bpmnModelBuilder);
            MessageFlowImpl.RegisterType(bpmnModelBuilder);
            MessageFlowRef.RegisterType(bpmnModelBuilder);
            MessageImpl.RegisterType(bpmnModelBuilder);
            MessagePath.RegisterType(bpmnModelBuilder);
            ModelElementInstanceImpl.RegisterType(bpmnModelBuilder);
            MonitoringImpl.RegisterType(bpmnModelBuilder);
            MultiInstanceLoopCharacteristicsImpl.RegisterType(bpmnModelBuilder);
            OperationImpl.RegisterType(bpmnModelBuilder);
            OperationRef.RegisterType(bpmnModelBuilder);
            OptionalInputRefs.RegisterType(bpmnModelBuilder);
            OptionalOutputRefs.RegisterType(bpmnModelBuilder);
            OuterParticipantRef.RegisterType(bpmnModelBuilder);
            OutMessageRef.RegisterType(bpmnModelBuilder);
            Outgoing.RegisterType(bpmnModelBuilder);
            OutputDataItemImpl.RegisterType(bpmnModelBuilder);
            OutputSetImpl.RegisterType(bpmnModelBuilder);
            OutputSetRefs.RegisterType(bpmnModelBuilder);
            ParallelGatewayImpl.RegisterType(bpmnModelBuilder);
            ParticipantAssociationImpl.RegisterType(bpmnModelBuilder);
            ParticipantImpl.RegisterType(bpmnModelBuilder);
            ParticipantMultiplicityImpl.RegisterType(bpmnModelBuilder);
            ParticipantRef.RegisterType(bpmnModelBuilder);
            PartitionElement.RegisterType(bpmnModelBuilder);
            PerformerImpl.RegisterType(bpmnModelBuilder);
            PotentialOwnerImpl.RegisterType(bpmnModelBuilder);
            ProcessImpl.RegisterType(bpmnModelBuilder);
            PropertyImpl.RegisterType(bpmnModelBuilder);
            ReceiveTaskImpl.RegisterType(bpmnModelBuilder);
            RelationshipImpl.RegisterType(bpmnModelBuilder);
            RenderingImpl.RegisterType(bpmnModelBuilder);
            ResourceAssignmentExpressionImpl.RegisterType(bpmnModelBuilder);
            ResourceImpl.RegisterType(bpmnModelBuilder);
            ResourceParameterBindingImpl.RegisterType(bpmnModelBuilder);
            ResourceParameterImpl.RegisterType(bpmnModelBuilder);
            ResourceRef.RegisterType(bpmnModelBuilder);
            ResourceRoleImpl.RegisterType(bpmnModelBuilder);
            RootElementImpl.RegisterType(bpmnModelBuilder);
            ScriptImpl.RegisterType(bpmnModelBuilder);
            ScriptTaskImpl.RegisterType(bpmnModelBuilder);
            SendTaskImpl.RegisterType(bpmnModelBuilder);
            SequenceFlowImpl.RegisterType(bpmnModelBuilder);
            ServiceTaskImpl.RegisterType(bpmnModelBuilder);
            SignalEventDefinitionImpl.RegisterType(bpmnModelBuilder);
            SignalImpl.RegisterType(bpmnModelBuilder);
            Source.RegisterType(bpmnModelBuilder);
            SourceRef.RegisterType(bpmnModelBuilder);
            StartEventImpl.RegisterType(bpmnModelBuilder);
            SubConversationImpl.RegisterType(bpmnModelBuilder);
            SubProcessImpl.RegisterType(bpmnModelBuilder);
            SupportedInterfaceRef.RegisterType(bpmnModelBuilder);
            Supports.RegisterType(bpmnModelBuilder);
            Target.RegisterType(bpmnModelBuilder);
            TargetRef.RegisterType(bpmnModelBuilder);
            TaskImpl.RegisterType(bpmnModelBuilder);
            TerminateEventDefinitionImpl.RegisterType(bpmnModelBuilder);
            TextImpl.RegisterType(bpmnModelBuilder);
            TextAnnotationImpl.RegisterType(bpmnModelBuilder);
            ThrowEventImpl.RegisterType(bpmnModelBuilder);
            TimeCycleImpl.RegisterType(bpmnModelBuilder);
            TimeDateImpl.RegisterType(bpmnModelBuilder);
            TimeDurationImpl.RegisterType(bpmnModelBuilder);
            TimerEventDefinitionImpl.RegisterType(bpmnModelBuilder);
            To.RegisterType(bpmnModelBuilder);
            TransactionImpl.RegisterType(bpmnModelBuilder);
            Transformation.RegisterType(bpmnModelBuilder);
            UserTaskImpl.RegisterType(bpmnModelBuilder);
            WhileExecutingInputRefs.RegisterType(bpmnModelBuilder);
            WhileExecutingOutputRefs.RegisterType(bpmnModelBuilder);

            // DC 
            FontImpl.RegisterType(bpmnModelBuilder);
            PointImpl.RegisterType(bpmnModelBuilder);
            BoundsImpl.RegisterType(bpmnModelBuilder);

            // DI 
            DiagramImpl.RegisterType(bpmnModelBuilder);
            DiagramElementImpl.RegisterType(bpmnModelBuilder);
            EdgeImpl.RegisterType(bpmnModelBuilder);
            impl.instance.di.ExtensionImpl.RegisterType(bpmnModelBuilder);
            LabelImpl.RegisterType(bpmnModelBuilder);
            LabeledEdgeImpl.RegisterType(bpmnModelBuilder);
            LabeledShapeImpl.RegisterType(bpmnModelBuilder);
            NodeImpl.RegisterType(bpmnModelBuilder);
            PlaneImpl.RegisterType(bpmnModelBuilder);
            ShapeImpl.RegisterType(bpmnModelBuilder);
            StyleImpl.RegisterType(bpmnModelBuilder);
            WaypointImpl.RegisterType(bpmnModelBuilder);
            
            // BPMNDI
            BpmnDiagramImpl.RegisterType(bpmnModelBuilder);
            BpmnEdgeImpl.RegisterType(bpmnModelBuilder);
            BpmnLabelImpl.RegisterType(bpmnModelBuilder);
            BpmnLabelStyleImpl.RegisterType(bpmnModelBuilder);
            BpmnPlaneImpl.RegisterType(bpmnModelBuilder);
            BpmnShapeImpl.RegisterType(bpmnModelBuilder);
            
            // camunda extensions 
            CamundaConnectorImpl.RegisterType(bpmnModelBuilder);
            CamundaConnectorIdImpl.RegisterType(bpmnModelBuilder);
            CamundaConstraintImpl.RegisterType(bpmnModelBuilder);
            CamundaEntryImpl.RegisterType(bpmnModelBuilder);
            CamundaExecutionListenerImpl.RegisterType(bpmnModelBuilder);
            CamundaExpressionImpl.RegisterType(bpmnModelBuilder);
            CamundaFailedJobRetryTimeCycleImpl.RegisterType(bpmnModelBuilder);
            CamundaFieldImpl.RegisterType(bpmnModelBuilder);
            CamundaFormDataImpl.RegisterType(bpmnModelBuilder);
            CamundaFormFieldImpl.RegisterType(bpmnModelBuilder);
            CamundaFormPropertyImpl.RegisterType(bpmnModelBuilder);
            CamundaInImpl.RegisterType(bpmnModelBuilder);
            CamundaInputOutputImpl.RegisterType(bpmnModelBuilder);
            CamundaInputParameterImpl.RegisterType(bpmnModelBuilder);
            CamundaListImpl.RegisterType(bpmnModelBuilder);
            CamundaMapImpl.RegisterType(bpmnModelBuilder);
            CamundaOutputParameterImpl.RegisterType(bpmnModelBuilder);
            CamundaOutImpl.RegisterType(bpmnModelBuilder);
            CamundaPotentialStarterImpl.RegisterType(bpmnModelBuilder);
            CamundaPropertiesImpl.RegisterType(bpmnModelBuilder);
            CamundaPropertyImpl.RegisterType(bpmnModelBuilder);
            CamundaScriptImpl.RegisterType(bpmnModelBuilder);
            CamundaStringImpl.RegisterType(bpmnModelBuilder);
            CamundaTaskListenerImpl.RegisterType(bpmnModelBuilder);
            CamundaValidationImpl.RegisterType(bpmnModelBuilder);
            CamundaValueImpl.RegisterType(bpmnModelBuilder);
        }

        /// <returns> the <seealso cref="IModel"/> instance to use </returns>
        public virtual Xml.IModel BpmnModel { get; set; }

        public virtual ModelBuilder BpmnModelBuilder { get; }
    }
}