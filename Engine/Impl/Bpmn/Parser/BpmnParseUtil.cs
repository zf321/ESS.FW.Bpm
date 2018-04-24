using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Impl.Scripting;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     Helper methods to reused for common parsing tasks.
    /// </summary>
    public sealed class BpmnParseUtil
    {
        protected internal static ExpressionManager ExpressionManager
        {
            get { return Context.ProcessEngineConfiguration.ExpressionManager; }
        }

        /// <summary>
        ///     Returns the camunda extension element in the camunda namespace
        ///     and the given name.
        /// </summary>
        /// <param name="element"> the parent element of the extension element </param>
        /// <param name="extensionElementName"> the name of the extension element to find </param>
        /// <returns> the extension element or null if not found </returns>
        public static Element FindCamundaExtensionElement(Element element, string extensionElementName)
        {
            var extensionElements = element.element("extensionElements");
            if (extensionElements != null)
                return extensionElements.ElementNs(BpmnParse.CamundaBpmnExtensionsNs, extensionElementName);
            return null;
        }

        /// <summary>
        ///     Returns the <seealso cref="IoMapping" /> of an element.
        /// </summary>
        /// <param name="element"> the element to parse </param>
        /// <returns> the input output mapping or null if non defined </returns>
        /// <exception cref="BpmnParseException"> if a input/output parameter element is malformed </exception>
        public static IoMapping ParseInputOutput(Element element)
        {
            var inputOutputElement = element.ElementNs(BpmnParse.CamundaBpmnExtensionsNs, "inputOutput");
            if (inputOutputElement != null)
            {
                var ioMapping = new IoMapping();
                ParseCamundaInputParameters(inputOutputElement, ioMapping);
                ParseCamundaOutputParameters(inputOutputElement, ioMapping);
                return ioMapping;
            }
            return null;
        }

        /// <summary>
        ///     Parses all input parameters of an input output element and adds them to
        ///     the <seealso cref="IoMapping" />.
        /// </summary>
        /// <param name="inputOutputElement"> the input output element to process </param>
        /// <param name="ioMapping"> the input output mapping to add input parameters to </param>
        /// <exception cref="BpmnParseException"> if a input parameter element is malformed </exception>
        public static void ParseCamundaInputParameters(Element inputOutputElement, IoMapping ioMapping)
        {
            var inputParameters = inputOutputElement.ElementsNS(BpmnParse.CamundaBpmnExtensionsNs,
                "inputParameter");
            foreach (var inputParameterElement in inputParameters)
                ParseInputParameterElement(inputParameterElement, ioMapping);
        }

        /// <summary>
        ///     Parses all output parameters of an input output element and adds them to
        ///     the <seealso cref="IoMapping" />.
        /// </summary>
        /// <param name="inputOutputElement"> the input output element to process </param>
        /// <param name="ioMapping"> the input output mapping to add input parameters to </param>
        /// <exception cref="BpmnParseException"> if a output parameter element is malformed </exception>
        public static void ParseCamundaOutputParameters(Element inputOutputElement, IoMapping ioMapping)
        {
            var outputParameters = inputOutputElement.ElementsNS(BpmnParse.CamundaBpmnExtensionsNs,
                "outputParameter");
            foreach (var outputParameterElement in outputParameters)
                ParseOutputParameterElement(outputParameterElement, ioMapping);
        }

        /// <summary>
        ///     Parses a input parameter and adds it to the <seealso cref="IoMapping" />.
        /// </summary>
        /// <param name="inputParameterElement"> the input parameter element </param>
        /// <param name="ioMapping"> the mapping to add the element </param>
        /// <exception cref="BpmnParseException"> if the input parameter element is malformed </exception>
        public static void ParseInputParameterElement(Element inputParameterElement, IoMapping ioMapping)
        {
            var nameAttribute = inputParameterElement.GetAttributeValue("name");
            if (ReferenceEquals(nameAttribute, null) || (nameAttribute.Length == 0))
                throw new BpmnParseException("Missing attribute 'name' for inputParameter", inputParameterElement);

            var valueProvider = ParseNestedParamValueProvider(inputParameterElement);

            // add parameter
            ioMapping.AddInputParameter(new InputParameter(nameAttribute, valueProvider));
        }

        /// <summary>
        ///     Parses a output parameter and adds it to the <seealso cref="IoMapping" />.
        /// </summary>
        /// <param name="outputParameterElement"> the output parameter element </param>
        /// <param name="ioMapping"> the mapping to add the element </param>
        /// <exception cref="BpmnParseException"> if the output parameter element is malformed </exception>
        public static void ParseOutputParameterElement(Element outputParameterElement, IoMapping ioMapping)
        {
            var nameAttribute = outputParameterElement.GetAttributeValue("name");
            if (ReferenceEquals(nameAttribute, null) || (nameAttribute.Length == 0))
                throw new BpmnParseException("Missing attribute 'name' for outputParameter", outputParameterElement);

            var valueProvider = ParseNestedParamValueProvider(outputParameterElement);

            // add parameter
            ioMapping.AddOutputParameter(new OutputParameter(nameAttribute, valueProvider));
        }

        /// <exception cref="BpmnParseException"> if the parameter is invalid </exception>
        protected internal static IParameterValueProvider ParseNestedParamValueProvider(Element element)
        {
            // parse value provider
            if (element.GetAllElement().Count == 0)
                return ParseParamValueProvider(element);
            if (element.GetAllElement().Count == 1)
                return ParseParamValueProvider(element.GetAllElement()[0]);
            throw new BpmnParseException("Nested parameter can at most have one child element", element);
        }

        /// <exception cref="BpmnParseException"> if the parameter is invalid </exception>
        protected internal static IParameterValueProvider ParseParamValueProvider(Element parameterElement)
        {
            // LIST
            if ("list".Equals(parameterElement.TagName))
            {
                IList<IParameterValueProvider> providerList = new List<IParameterValueProvider>();
                foreach (var element in parameterElement.GetAllElement())
                    providerList.Add(ParseParamValueProvider(element));
                return new ListValueProvider(providerList);
            }

            // MAP
            if ("map".Equals(parameterElement.TagName))
            {
                var providerMap = new SortedDictionary<string, IParameterValueProvider>();
                foreach (var entryElement in parameterElement.Elements("entry"))
                {
                    // entry must provide key
                    var keyAttribute = entryElement.GetAttributeValue("key");
                    if (ReferenceEquals(keyAttribute, null) || (keyAttribute.Length == 0))
                        throw new BpmnParseException("Missing attribute 'key' for 'entry' element", entryElement);
                    // parse nested provider
                    providerMap[keyAttribute] = ParseNestedParamValueProvider(entryElement);
                }
                return new MapValueProvider(providerMap);
            }

            // SCRIPT
            if ("script".Equals(parameterElement.TagName))
            {
                ExecutableScript executableScript = ParseCamundaScript(parameterElement);
                if (executableScript != null)
                {
                    return new ScriptValueProvider(executableScript);
                }
                else
                {
                    return new NullValueProvider();
                }
            }

            var textContent = parameterElement.Text.Trim();
            if (textContent.Length > 0)
                return new ElValueProvider(ExpressionManager.CreateExpression(textContent));
            // NULL value
            return new NullValueProvider();
        }

        /// <summary>
        ///     Parses a camunda script element.
        /// </summary>
        /// <param name="scriptElement"> the script element ot parse </param>
        /// <returns> the generated executable script </returns>
        /// <exception cref="BpmnParseException"> if the a attribute is missing or the script cannot be processed </exception>
        public static ExecutableScript ParseCamundaScript(Element scriptElement)
        {

            string scriptLanguage = scriptElement.GetAttributeValue("scriptFormat");
            if (ReferenceEquals(scriptLanguage, null) || scriptLanguage.Length == 0)
            {
                throw new BpmnParseException("Missing attribute 'scriptFormatAttribute' for 'script' element",
                    scriptElement);
            }
            string scriptResource = scriptElement.GetAttributeValue("resource");
            var scriptSource = scriptElement.Text;
            try
            {
                return ScriptUtil.GetScript(scriptLanguage, scriptSource, scriptResource, ExpressionManager);
            }
            catch (ProcessEngineException e)
            {
                throw new BpmnParseException("Unable to process script", scriptElement, e);
            }
        }
    }
}