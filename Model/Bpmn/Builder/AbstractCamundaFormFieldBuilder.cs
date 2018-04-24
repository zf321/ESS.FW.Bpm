using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;


namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractCamundaFormFieldBuilder<TP> : AbstractBpmnModelElementBuilder<ICamundaFormField> where TP:IBpmnModelElementBuilder<ICamundaFormField>
    {

        protected internal IBaseElement Parent;

        protected internal AbstractCamundaFormFieldBuilder(IBpmnModelInstance modelInstance, IBaseElement parent, ICamundaFormField element) : base(modelInstance, element)
        {
            this.Parent = parent;
        }


        /// <summary>
        /// Sets the form field id.
        /// </summary>
        /// <param name="id"> the form field id </param>
        /// <returns>  the builder object </returns>
        public virtual AbstractCamundaFormFieldBuilder<TP> CamundaId(string id)
        {
            element.CamundaId = id;
            return this;
        }

        /// <summary>
        /// Sets form field label.
        /// </summary>
        /// <param name="label"> the form field label </param>
        /// <returns>  the builder object </returns>
        public virtual AbstractCamundaFormFieldBuilder<TP> CamundaLabel(string label)
        {
            element.CamundaLabel = label;
            return this;
        }

        /// <summary>
        /// Sets the form field type.
        /// </summary>
        /// <param name="type"> the form field type </param>
        /// <returns> the builder object </returns>
        public virtual AbstractCamundaFormFieldBuilder<TP> CamundaType(string type)
        {
            element.CamundaType = type;
            return this;
        }

        /// <summary>
        /// Sets the form field default value.
        /// </summary>
        /// <param name="defaultValue"> the form field default value </param>
        /// <returns> the builder object </returns>
        public virtual AbstractCamundaFormFieldBuilder<TP> CamundaDefaultValue(string defaultValue)
        {
            element.CamundaDefaultValue = defaultValue;
            return this;
        }

        /// <summary>
        /// Finishes the building of a form field.
        /// </summary>
        /// <returns> the parent activity builder </returns>
        public virtual TP CamundaFormFieldDone()
        {
            //throw new NotImplementedException();
            return Parent.Builder<TP,ICamundaFormField>();
        }
    }
}