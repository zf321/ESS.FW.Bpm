using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    using ModelTypeInstanceContext = ModelTypeInstanceContext;
    using ModelUtil = ModelUtil;

    /// <summary>
    /// A helper interface for camunda extension elements which
    /// hold a generic child element like camunda:inputParameter,
    /// camunda:outputParameter and camunda:entry.
    /// 
    /// 
    /// </summary>
    public class CamundaGenericValueElementImpl : BpmnModelElementInstanceImpl, ICamundaGenericValueElement
    {

        public CamundaGenericValueElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public T GetValue<T>() where T : IBpmnModelElementInstance
        {
            IDomElement domElement = DomElement;
            IList<IDomElement> childElements = domElement.ChildElements;
            if (childElements.Count == 0)
            {
                return default(T);
            }
            else
            {
                return (T)ModelUtil.GetModelElement(childElements[0], modelInstance);
            }
        }

        public void SetValue<T>(T value) where T : IBpmnModelElementInstance
        {
            RemoveValue();
            DomElement.AppendChild(value.DomElement);
        }
        public virtual void RemoveValue()
        {
            IDomElement domElement = DomElement;
            IList<IDomElement> childElements = domElement.ChildElements;
            foreach (IDomElement childElement in childElements)
            {
                domElement.RemoveChild(childElement);
            }
        }
    }
}