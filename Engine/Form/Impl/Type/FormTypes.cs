using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Form.Impl.Handler;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Util.xml;

namespace ESS.FW.Bpm.Engine.Form.Impl.Type
{
    /// <summary>
    ///      
    /// </summary>
    public class FormTypes
    {
        protected internal IDictionary<string, AbstractFormFieldType> formTypes =
            new Dictionary<string, AbstractFormFieldType>();

        public virtual void AddFormType(AbstractFormFieldType formType)
        {
            formTypes[formType.Name] = formType;
        }

        public virtual AbstractFormFieldType ParseFormPropertyType(Element formFieldElement, BpmnParse bpmnParse)
        {
            AbstractFormFieldType formType = null;

            var typeText = formFieldElement.GetAttributeValue("type");
            var datePatternText = formFieldElement.GetAttributeValue("datePattern");

            if (ReferenceEquals(typeText, null) &&
                DefaultFormHandler.FormFieldElement.Equals(formFieldElement.TagName))
                bpmnParse.AddError("form field must have a 'type' attribute", formFieldElement);

            if ("date".Equals(typeText) && !ReferenceEquals(datePatternText, null))
            {
                formType = new DateFormType(datePatternText);
            }
            else if ("enum".Equals(typeText))
            {
                // ACT-1023: Using linked hashmap to preserve the order in which the entries are defined
                IDictionary<string, string> values = new Dictionary<string, string>();
                foreach (
                    var valueElement in
                    formFieldElement.ElementsNS(BpmnParse.CamundaBpmnExtensionsNs, "value"))
                {
                    var valueId = valueElement.GetAttributeValue("id");
                    var valueName = valueElement.GetAttributeValue("name");
                    values[valueId] = valueName;
                }
                //formType = new EnumFormType(values);
            }
            else if (!ReferenceEquals(typeText, null))
            {
                formType = formTypes[typeText];
                if (formType == null)
                    bpmnParse.AddError("unknown type '" + typeText + "'", formFieldElement);
            }
            return formType;
        }

        public virtual AbstractFormFieldType GetFormType(string name)
        {
            return formTypes[name];
        }
    }
}