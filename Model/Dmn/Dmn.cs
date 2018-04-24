

using System.IO;
using ESS.FW.Bpm.Model.Dmn.impl;
using ESS.FW.Bpm.Model.Dmn.impl.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;

namespace ESS.FW.Bpm.Model.Dmn
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_ALTERNATIVE_NS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_NS;
    //TODO Dmn实例化异常
    public class Dmn
    {
        /// <summary>
        /// 单例
        /// the singleton instance of <seealso cref="Dmn"/>. If you want to customize the behavior of Dmn,
        /// replace this instance with an instance of a custom subclass of <seealso cref="Dmn"/>. 
        /// </summary>
        public static  Dmn Instance
        { get
            {
                if (_instance == null)
                {
                  _instance= new Dmn();
                }
                return _instance;
            } }
        private static Dmn _instance;
            /// <summary>
                                  /// the parser used by the Dmn implementation. </summary>
        //private BpmnParser dmnParser = (BpmnParser) (new DmnParser() as AbstractModelParser);
        private DmnParser _dmnParser = new DmnParser();// (BpmnParser)(new DmnParser() as AbstractModelParser);
        private readonly ModelBuilder _dmnModelBuilder;

        /// <summary>
        /// The <seealso cref="IModel"/>
        /// </summary>
        private Xml.IModel _dmnModel;

        /// <summary>
        /// Allows reading a <seealso cref="IDmnModelInstance"/> from a File.
        /// </summary>
        /// <param name="file"> the <seealso cref="File"/> to read the <seealso cref="IDmnModelInstance"/> from </param>
        /// <returns> the model read </returns>
        /// <exception cref="DmnModelException"> if the model cannot be read </exception>
        public static IDmnModelInstance ReadModelFromFile(string file)
        {
            return Instance.DoReadModelFromFile(file);
        }

        /// <summary>
        /// Allows reading a <seealso cref="IDmnModelInstance"/> from an <seealso cref="InputStream"/>
        /// </summary>
        /// <param name="stream"> the <seealso cref="InputStream"/> to read the <seealso cref="IDmnModelInstance"/> from </param>
        /// <returns> the model read </returns>
        /// <exception cref="ModelParseException"> if the model cannot be read </exception>
        public static IDmnModelInstance ReadModelFromStream(System.IO.Stream stream)
        {
            return Instance.DoReadModelFromInputStream(stream);
        }

        /// <summary>
        /// Allows writing a <seealso cref="IDmnModelInstance"/> to a File. It will be
        /// validated before writing.
        /// </summary>
        /// <param name="file"> the <seealso cref="File"/> to write the <seealso cref="IDmnModelInstance"/> to </param>
        /// <param name="modelInstance"> the <seealso cref="IDmnModelInstance"/> to write </param>
        /// <exception cref="DmnModelException"> if the model cannot be written </exception>
        /// <exception cref="ModelValidationException"> if the model is not valid </exception>
        public static void WriteModelToFile(string file, IDmnModelInstance modelInstance)
        {
            Instance.DoWriteModelToFile(file, modelInstance);
        }

        /// <summary>
        /// Allows writing a <seealso cref="IDmnModelInstance"/> to an <seealso cref="OutputStream"/>. It will be
        /// validated before writing.
        /// </summary>
        /// <param name="stream"> the <seealso cref="OutputStream"/> to write the <seealso cref="IDmnModelInstance"/> to </param>
        /// <param name="modelInstance"> the <seealso cref="IDmnModelInstance"/> to write </param>
        /// <exception cref="ModelException"> if the model cannot be written </exception>
        /// <exception cref="ModelValidationException"> if the model is not valid </exception>
        public static void WriteModelToStream(System.IO.Stream stream, IDmnModelInstance modelInstance)
        {
            Instance.DoWriteModelToOutputStream(stream, modelInstance);
        }

        /// <summary>
        /// Allows the conversion of a <seealso cref="IDmnModelInstance"/> to an <seealso cref="String"/>. It will
        /// be validated before conversion.
        /// </summary>
        /// <param name="modelInstance">  the model instance to convert </param>
        /// <returns> the XML string representation of the model instance </returns>
        public static string ConvertToString(IDmnModelInstance modelInstance)
        {
            return Instance.DoConvertToString(modelInstance);
        }

        /// <summary>
        /// Validate model DOM document
        /// </summary>
        /// <param name="modelInstance"> the <seealso cref="IDmnModelInstance"/> to validate </param>
        /// <exception cref="ModelValidationException"> if the model is not valid </exception>
        public static void ValidateModel(IDmnModelInstance modelInstance)
        {
            Instance.DoValidateModel(modelInstance);
        }

        /// <summary>
        /// Allows creating an new, empty <seealso cref="IDmnModelInstance"/>.
        /// </summary>
        /// <returns> the empty model. </returns>
        public static IDmnModelInstance CreateEmptyModel()
        {
            return Instance.DoCreateEmptyModel();
        }

        /// <summary>
        /// Register known types of the Dmn model
        /// </summary>
        protected internal Dmn()
        {
            _dmnModelBuilder = ModelBuilder.CreateInstance("DMN Model");
            _dmnModelBuilder.AlternativeNamespace(DmnModelConstants.Dmn11AlternativeNs, DmnModelConstants.Dmn11Ns);
            DoRegisterTypes(_dmnModelBuilder);
            _dmnModel = _dmnModelBuilder.Build();
        }
       


        protected internal virtual IDmnModelInstance DoReadModelFromFile(string file)
        {
            System.IO.Stream steam = null;
            try
            {
                steam = new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                return DoReadModelFromInputStream(steam);

            }
            catch (FileNotFoundException)
            {
                throw new DmnModelException("Cannot read model from file " + file + ": file does not exist.");

            }
            finally
            {
                //DomUtil.closeSilently(steam);
                steam.Dispose();
            }
        }

        protected internal virtual IDmnModelInstance DoReadModelFromInputStream(System.IO.Stream stream)
        {
            return (IDmnModelInstance)_dmnParser.ParseModelFromStream(stream);// as DmnModelInstance;
        }

        protected internal virtual void DoWriteModelToFile(string file, IDmnModelInstance modelInstance)
        {
            System.IO.Stream os = null;
            try
            {
                os = new System.IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                DoWriteModelToOutputStream(os, modelInstance);
            }
            catch (FileNotFoundException)
            {
                throw new DmnModelException("Cannot write model to file " + file + ": file does not exist.");
            }
            finally
            {
                os.Dispose();
            }
        }

        protected internal virtual void DoWriteModelToOutputStream(System.IO.Stream os, IDmnModelInstance modelInstance)
        {
            // validate DOM document
            DoValidateModel(modelInstance);
            // write XML
            IoUtil.WriteDocumentToOutputStream(modelInstance.Document, os);
        }

        protected internal virtual string DoConvertToString(IDmnModelInstance modelInstance)
        {
            // validate DOM document
            DoValidateModel(modelInstance);
            // convert to XML string
            return IoUtil.ConvertXmlDocumentToString(modelInstance.Document);
        }

        protected internal virtual void DoValidateModel(IDmnModelInstance modelInstance)
        {
            _dmnParser.ValidateModel(modelInstance.Document);
        }

        protected internal virtual IDmnModelInstance DoCreateEmptyModel()
        {
            return (IDmnModelInstance)_dmnParser.EmptyModel;
        }
        /// <summary>
        /// 用于创建modelBuilder
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected internal virtual void DoRegisterTypes(ModelBuilder modelBuilder)
        {
            //TODO Dmn类型注册
            try
            {
                AllowedAnswersImpl.RegisterType(modelBuilder);
                AllowedValuesImpl.RegisterType(modelBuilder);
                ArtifactImpl.RegisterType(modelBuilder);
                AssociationImpl.RegisterType(modelBuilder);
                AuthorityRequirementImpl.RegisterType(modelBuilder);
                BindingImpl.RegisterType(modelBuilder);
                BusinessContextElementImpl.RegisterType(modelBuilder);
                BusinessKnowledgeModelImpl.RegisterType(modelBuilder);
                ColumnImpl.RegisterType(modelBuilder);
                ContextEntryImpl.RegisterType(modelBuilder);
                ContextImpl.RegisterType(modelBuilder);
                DecisionImpl.RegisterType(modelBuilder);
                DecisionMadeReferenceImpl.RegisterType(modelBuilder);
                DecisionMakerReferenceImpl.RegisterType(modelBuilder);
                DecisionOwnedReferenceImpl.RegisterType(modelBuilder);
                DecisionOwnerReferenceImpl.RegisterType(modelBuilder);
                DecisionRuleImpl.RegisterType(modelBuilder);
                DecisionServiceImpl.RegisterType(modelBuilder);
                DecisionTableImpl.RegisterType(modelBuilder);
                DefaultOutputEntryImpl.RegisterType(modelBuilder);
                DefinitionsImpl.RegisterType(modelBuilder);
                DescriptionImpl.RegisterType(modelBuilder);
                DmnElementImpl.RegisterType(modelBuilder);
                DmnElementReferenceImpl.RegisterType(modelBuilder);
                DrgElementImpl.RegisterType(modelBuilder);
                DrgElementReferenceImpl.RegisterType(modelBuilder);
                ElementCollectionImpl.RegisterType(modelBuilder);
                EncapsulatedDecisionReferenceImpl.RegisterType(modelBuilder);
                EncapsulatedLogicImpl.RegisterType(modelBuilder);
                ExpressionImpl.RegisterType(modelBuilder);
                ExtensionElementsImpl.RegisterType(modelBuilder);
                FormalParameterImpl.RegisterType(modelBuilder);
                FunctionDefinitionImpl.RegisterType(modelBuilder);
                ImpactedPerformanceIndicatorReferenceImpl.RegisterType(modelBuilder);
                ImpactingDecisionReferenceImpl.RegisterType(modelBuilder);
                ImportImpl.RegisterType(modelBuilder);
                ImportedElementImpl.RegisterType(modelBuilder);
                ImportedValuesImpl.RegisterType(modelBuilder);
                InformationItemImpl.RegisterType(modelBuilder);
                InformationRequirementImpl.RegisterType(modelBuilder);
                InputImpl.RegisterType(modelBuilder);
                InputClauseImpl.RegisterType(modelBuilder);
                InputDataImpl.RegisterType(modelBuilder);
                InputDataReferenceImpl.RegisterType(modelBuilder);
                InputDecisionReferenceImpl.RegisterType(modelBuilder);
                InputEntryImpl.RegisterType(modelBuilder);
                InputExpressionImpl.RegisterType(modelBuilder);
                InputValuesImpl.RegisterType(modelBuilder);
                InvocationImpl.RegisterType(modelBuilder);
                ItemComponentImpl.RegisterType(modelBuilder);
                ItemDefinitionImpl.RegisterType(modelBuilder);
                ItemDefinitionReferenceImpl.RegisterType(modelBuilder);
                KnowledgeRequirementImpl.RegisterType(modelBuilder);
                KnowledgeSourceImpl.RegisterType(modelBuilder);
                ListImpl.RegisterType(modelBuilder);
                LiteralExpressionImpl.RegisterType(modelBuilder);
                ModelElementInstanceImpl.RegisterType(modelBuilder);
                NamedElementImpl.RegisterType(modelBuilder);
                OrganizationUnitImpl.RegisterType(modelBuilder);
                OutputImpl.RegisterType(modelBuilder);
                OutputClauseImpl.RegisterType(modelBuilder);
                OutputDecisionReferenceImpl.RegisterType(modelBuilder);
                OutputEntryImpl.RegisterType(modelBuilder);
                OutputValuesImpl.RegisterType(modelBuilder);
                OwnerReferenceImpl.RegisterType(modelBuilder);
                ParameterReferenceImpl.RegisterType(modelBuilder);
                PerformanceIndicatorImpl.RegisterType(modelBuilder);
                QuestionImpl.RegisterType(modelBuilder);
                RelationImpl.RegisterType(modelBuilder);
                RequiredAuthorityReferenceImpl.RegisterType(modelBuilder);
                RequiredDecisionReferenceImpl.RegisterType(modelBuilder);
                RequiredInputReferenceImpl.RegisterType(modelBuilder);
                RequiredKnowledgeReferenceImpl.RegisterType(modelBuilder);
                RowImpl.RegisterType(modelBuilder);
                RuleImpl.RegisterType(modelBuilder);
                SourceRefImpl.RegisterType(modelBuilder);
                SupportedObjectiveReferenceImpl.RegisterType(modelBuilder);
                TargetRefImpl.RegisterType(modelBuilder);
                TextImpl.RegisterType(modelBuilder);
                TextAnnotationImpl.RegisterType(modelBuilder);
                TypeImpl.RegisterType(modelBuilder);
                TypeRefImpl.RegisterType(modelBuilder);
                UnaryTestsImpl.RegisterType(modelBuilder);
                UsingProcessReferenceImpl.RegisterType(modelBuilder);
                UsingTaskReferenceImpl.RegisterType(modelBuilder);
                VariableImpl.RegisterType(modelBuilder);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            /// <summary>
            /// camunda extensions </summary>
        }

        /// <returns> the <seealso cref="IModel"/> instance to use </returns>
        public virtual Xml.IModel DmnModel
        {
            get
            {
                return _dmnModel;
            }
            set
            {
                this._dmnModel = value;
            }
        }

        public virtual ModelBuilder DmnModelBuilder
        {
            get
            {
                return _dmnModelBuilder;
            }
        }
    }
}