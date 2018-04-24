using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Form.Impl.Engine
{
    /// <summary>
    ///     
    /// </summary>
    public class FormPropertyAdapter : IFormField
    {
        protected internal IFormProperty FormProperty;
        protected internal IList<IFormFieldValidationConstraint> validationConstraints;

        public FormPropertyAdapter(IFormProperty formProperty)
        {
            this.FormProperty = formProperty;

            validationConstraints = new List<IFormFieldValidationConstraint>();
            if (formProperty.Required)
                validationConstraints.Add(new FormFieldValidationConstraintImpl("required", null));
            if (!formProperty.Writable)
                validationConstraints.Add(new FormFieldValidationConstraintImpl("readonly", null));
        }

        public virtual ITypedValue DefaultITypeValued
        {
            get { return Value; }
        }

        public virtual string Id
        {
            get { return FormProperty.Id; }
        }

        public virtual string Label
        {
            get { return FormProperty.Name; }
        }

        public virtual IFormType Type
        {
            get { return FormProperty.Type; }
        }

        public virtual string TypeName
        {
            get { return FormProperty.Type.Name; }
        }

        public virtual object DefaultValue
        {
            get { return FormProperty.Value; }
        }

        public virtual IList<IFormFieldValidationConstraint> ValidationConstraints
        {
            get { return validationConstraints; }
        }

        public virtual IDictionary<string, string> Properties
        {
            get { return new Dictionary<string, string>(); }
        }

        public virtual bool BusinessKey
        {
            get { return false; }
        }

        public virtual ITypedValue Value
        {
            get
            {
                return null;
                //return Variables.stringValue(formProperty.Value);
            }
        }
    }
}