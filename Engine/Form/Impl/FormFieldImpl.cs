using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Form.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class FormFieldImpl : IFormField
    {
        // getters / setters ///////////////////////////////////////////

        public virtual string Id { get; set; }


        public virtual string Label { get; set; }


        public virtual IFormType Type { get; set; }

        public virtual string TypeName => Type.Name;


        public virtual object DefaultValue { get; set; }

        public virtual ITypedValue Value { get; set; }


        public virtual IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();


        public virtual IList<IFormFieldValidationConstraint> ValidationConstraints { get; set; } =
            new List<IFormFieldValidationConstraint>();


        public virtual bool BusinessKey { get; set; }
    }
}