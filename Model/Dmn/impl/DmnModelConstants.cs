

namespace ESS.FW.Bpm.Model.Dmn.impl
{

    public sealed class DmnModelConstants
    {

        /// <summary>
        /// The DMN 1.1 namespace </summary>
        public const string Dmn11Ns = "http://www.omg.org/spec/DMN/20151101/dmn.xsd";

        /// <summary>
        /// The DMN 1.1 namespace URL release with Camunda 7.4.0
        /// </summary>
        public const string Dmn11AlternativeNs = "http://www.omg.org/spec/DMN/20151101/dmn11.xsd";

        /// <summary>
        /// The location of the DMN 1.1 XML schema. </summary>
        //public const string DMN_11_SCHEMA_LOCATION = "DMN11.xsd";
        public const string Dmn11SchemaLocation = "Dmn/DMN11.xsd";

        /// <summary>
        /// The location of the DMN 1.1 XML schema released with Camunda 7.4.0
        /// </summary>
        public const string Dmn11AlternativeSchemaLocation = "Dmn/DMN11_Alternative.xsd";

        /// <summary>
        /// The FEEL namespace </summary>
        public const string FeelNs = "http://www.omg.org/spec/FEEL/20140401";

        /// <summary>
        /// Camunda namespace </summary>
        public const string CamundaNs = "http://camunda.org/schema/1.0/dmn";

        /// <summary>
        /// DMN element </summary>

        public const string DmnElement = "DMNElement";
        public const string DmnElementAllowedAnswers = "allowedAnswers";
        public const string DmnElementAllowedValue = "allowedValue";
        public const string DmnElementArtifact = "artifact";
        public const string DmnElementAssociation = "association";
        public const string DmnElementAuthorityRequirement = "authorityRequirement";
        public const string DmnElementBinding = "binding";
        public const string DmnElementBusinessContextElement = "businessContextElement";
        public const string DmnElementBusinessKnowledgeModel = "businessKnowledgeModel";
        public const string DmnElementColumn = "column";
        public const string DmnElementContext = "context";
        public const string DmnElementContextEntry = "contextEntry";
        public const string DmnElementDecision = "decision";
        public const string DmnElementDecisionMade = "decisionMade";
        public const string DmnElementDecisionMaker = "decisionMaker";
        public const string DmnElementDecisionOwned = "decisionOwned";
        public const string DmnElementDecisionOwner = "decisionOwner";
        public const string DmnElementDecisionRule = "decisionRule";
        public const string DmnElementDecisionService = "decisionService";
        public const string DmnElementDecisionTable = "decisionTable";
        public const string DmnElementDefaultOutputEntry = "defaultOutputEntry";
        public const string DmnElementDefinitions = "definitions";
        public const string DmnElementDescription = "description";
        public const string DmnElementDrgElement = "drgElement";
        public const string DmnElementDrgElementReference = "drgElement";
        public const string DmnElementElementCollection = "elementCollection";
        public const string DmnElementEncapsulatedDecisionReference = "encapsulatedDecision";
        public const string DmnElementEncapsulatedLogic = "encapsulatedLogic";
        public const string DmnElementExpression = "expression";
        public const string DmnElementExtensionElements = "extensionElements";
        public const string DmnElementFormalParameter = "formalParameter";
        public const string DmnElementFunctionDefinition = "functionDefinition";
        public const string DmnElementImpactedPerformanceIndicator = "impactedPerformanceIndicator";
        public const string DmnElementImpactingDecision = "impactingDecision";
        public const string DmnElementImport = "import";
        public const string DmnElementImportedElement = "importedElement";
        public const string DmnElementImportedValues = "importedValues";
        public const string DmnElementInformationItem = "informationItem";
        public const string DmnElementInformationRequirement = "informationRequirement";
        public const string DmnElementInput = "input";
        public const string DmnElementInputClause = "inputClause";
        public const string DmnElementInputData = "inputData";
        public const string DmnElementInputDataReference = "inputData";
        public const string DmnElementInputDecisionReference = "inputDecision";
        public const string DmnElementInputEntry = "inputEntry";
        public const string DmnElementInputExpression = "inputExpression";
        public const string DmnElementInputValues = "inputValues";
        public const string DmnElementInvocation = "invocation";
        public const string DmnElementItemComponent = "itemComponent";
        public const string DmnElementItemDefinition = "itemDefinition";
        public const string DmnElementItemDefinitionReference = "itemDefinition";
        public const string DmnElementKnowledgeRequirement = "knowledgeRequirement";
        public const string DmnElementKnowledgeSource = "knowledgeSource";
        public const string DmnElementList = "list";
        public const string DmnElementLiteralExpression = "literalExpression";
        public const string DmnElementNamedElement = "namedElement";
        public const string DmnElementOrganizationUnit = "organizationUnit";
        public const string DmnElementOutput = "output";
        public const string DmnElementOutputClause = "outputClause";
        public const string DmnElementOutputDecisionReference = "outputDecision";
        public const string DmnElementOutputEntry = "outputEntry";
        public const string DmnElementOutputValues = "outputValues";
        public const string DmnElementOwner = "owner";
        public const string DmnElementParameter = "parameter";
        public const string DmnElementPerformanceIndicator = "performanceIndicator";
        public const string DmnElementQuestion = "question";
        public const string DmnElementReference = "DMNElementReference";
        public const string DmnElementRelation = "relation";
        public const string DmnElementRequiredAuthority = "requiredAuthority";
        public const string DmnElementRequiredDecision = "requiredDecision";
        public const string DmnElementRequiredInput = "requiredInput";
        public const string DmnElementRequiredKnowledge = "requiredKnowledge";
        public const string DmnElementRow = "row";
        public const string DmnElementRule = "rule";
        public const string DmnElementSourceRef = "sourceRef";
        public const string DmnElementSupportedObject = "supportedObjective";
        public const string DmnElementTargetRef = "targetRef";
        public const string DmnElementText = "text";
        public const string DmnElementTextAnnotation = "textAnnotation";
        public const string DmnElementType = "type";
        public const string DmnElementTypeRef = "typeRef";
        public const string DmnElementUnaryTests = "unaryTests";
        public const string DmnElementUsingProcess = "usingProcess";
        public const string DmnElementUsingTask = "usingTask";
        public const string DmnElementVariable = "variable";

        /// <summary>
        /// DMN attributes </summary>

        public const string DmnAttributeAggregation = "aggregation";
        public const string DmnAttributeAssociationDirection = "associationDirection";
        public const string DmnAttributeExpressionLanguage = "expressionLanguage";
        public const string DmnAttributeExporter = "exporter";
        public const string DmnAttributeExporterVersion = "exporterVersion";
        public const string DmnAttributeHitPolicy = "hitPolicy";
        public const string DmnAttributeHref = "href";
        public const string DmnAttributeId = "id";
        public const string DmnAttributeImportType = "importType";
        public const string DmnAttributeIsCollection = "isCollection";
        public const string DmnAttributeLabel = "label";
        public const string DmnAttributeLocationUri = "locationURI";
        public const string DmnAttributeName = "name";
        public const string DmnAttributeNamespace = "namespace";
        public const string DmnAttributeOutputLabel = "outputLabel";
        public const string DmnAttributePreferredOrientation = "preferredOrientation";
        public const string DmnAttributeTextFormat = "textFormat";
        public const string DmnAttributeTypeLanguage = "typeLanguage";
        public const string DmnAttributeTypeRef = "typeRef";
        public const string DmnAttributeUri = "URI";

        /// <summary>
        /// camunda extensions </summary>

        public const string CamundaAttributeInputVariable = "inputVariable";

    }

}