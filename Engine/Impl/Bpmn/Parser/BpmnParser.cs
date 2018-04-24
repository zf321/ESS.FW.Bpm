using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Util.xml;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     bpmn2.0流程模型解析器
    ///     Parser for BPMN 2.0 process models.
    ///     There is only one instance of this parser in the process engine.
    ///     This <seealso cref="Parser" /> creates <seealso cref="BpmnParse" /> instances that
    ///     can be used to actually parse the BPMN 2.0 XML process definitions.
    ///      
    ///     
    /// </summary>
    public class BpmnParser : Util.xml.Parser
    {
        /// <summary>
        ///     The BPMN 2.0 namespace命名空间
        /// </summary>
        public const string Bpmn20Ns = "http://www.omg.org/spec/BPMN/20100524/MODEL";

        /// <summary>
        ///     The location of the BPMN 2.0 XML schema.本地路径
        /// </summary>
        //public const string BPMN_20_SCHEMA_LOCATION = "org/camunda/bpm/engine/impl/bpmn/parser/BPMN20.xsd";
        //public const string Bpmn20SchemaLocation = "resources/Bpmn/BPMN20.xsd";

        /// <summary>
        ///     The namespace of the camunda custom BPMN extensions.
        /// </summary>
        public const string CamundaBpmnExtensionsNs = "http://camunda.org/schema/1.0/bpmn";

        /// <summary>
        ///     The namespace of the Activiti custom BPMN extensions.
        /// </summary>
        /// @deprecated use
        /// <seealso cref="#CAMUNDA_BPMN_EXTENSIONS_NS" />
        [Obsolete("use <seealso cref=\"#CAMUNDA_BPMN_EXTENSIONS_NS\"/>")] public const string
            ActivitiBpmnExtensionsNs = "http://activiti.org/bpmn";

        /// <summary>
        ///     The namepace of the BPMN 2.0 diagram interchange elements.
        /// </summary>
        public const string BpmnDiNs = "http://www.omg.org/spec/BPMN/20100524/DI";

        /// <summary>
        ///     The namespace of the BPMN 2.0 diagram common elements.
        /// </summary>
        public const string BpmnDcNs = "http://www.omg.org/spec/DD/20100524/DC";

        /// <summary>
        ///     The namespace of the generic OMG DI elements (don't ask me why they didnt use the BPMN_DI_NS ...)
        /// </summary>
        public const string OmgDiNs = "http://www.omg.org/spec/DD/20100524/DI";

        /// <summary>
        ///     The Schema-Instance namespace.
        /// </summary>
        public const string XsiNs = "http://www.w3.org/2001/XMLSchema-instance";

        protected internal IBpmnParseFactory BpmnParseFactory;

        //TODO 后期考虑表达式
        public XmlSchemaSet XmlSchemaSetBase;

        public BpmnParser(ExpressionManager expressionManager, IBpmnParseFactory bpmnParseFactory)
        {
            this.ExpressionManager = expressionManager;
            this.BpmnParseFactory = bpmnParseFactory;
            //Parser实例中定义好协议标准 供校验 bpmn2.0
            XmlSchemaSetBase = new XmlSchemaSet();
            XmlSchemaSetBase.Add(null, XmlReader.Create(new StringReader(Properties.Resources.DI)));
            XmlSchemaSetBase.Add(null, XmlReader.Create(new StringReader(Properties.Resources.DC)));
            XmlSchemaSetBase.Add(null, XmlReader.Create(new StringReader(Properties.Resources.Semantic)));
            XmlSchemaSetBase.Add(null, XmlReader.Create(new StringReader(Properties.Resources.BPMNDI)));
            XmlSchemaSetBase.Add(Bpmn20Ns, XmlReader.Create(new StringReader(Properties.Resources.BPMN20)) );
            XmlSchemaSetBase.Compile();
        }

        public virtual ExpressionManager ExpressionManager { get; set; }

        /// <summary>
        ///    Parser中已初始化
        /// </summary>
        public virtual List<IBpmnParseListener> ParseListeners { get; set; } = new List<IBpmnParseListener>();

        /// <summary>
        ///     Creates a new <seealso cref="BpmnParse" /> instance that can be used
        ///     to parse only one BPMN 2.0 process definition.
        /// </summary>
        public override Parse CreateParse()
        {
            return BpmnParseFactory.CreateBpmnParse(this);
        }

        public BpmnParse CreateBpmnParse()
        {
            return BpmnParseFactory.CreateBpmnParse(this);
        }
    }
}