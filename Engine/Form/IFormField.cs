using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Form
{
    /// <summary>
    ///     <para>Represents an individual field in a form.</para>
    ///     
    /// </summary>
    public interface IFormField
    {
        /// <returns>
        ///     the Id of a form property. Must be unique for a given form.
        ///     The id is used for mapping the form field to a process variable.
        /// </returns>
        string Id { get; }

        /// <returns> the human-readable display name of a form property. </returns>
        string Label { get; }

        /// <returns> the type of this form field. </returns>
        IFormType Type { get; }

        /// <returns> the name of the type of this form field </returns>
        string TypeName { get; }

        /// <returns> the default value for this form field. </returns>
        [Obsolete]
        object DefaultValue { get; }

        /// <returns> the value for this form field </returns>
        ITypedValue Value { get; }

        /// <returns> a list of <seealso cref="IFormFieldValidationConstraint" />. </returns>
        IList<IFormFieldValidationConstraint> ValidationConstraints { get; }

        /// <returns>
        ///     a <seealso cref="Map" /> of additional properties. This map may be used for adding additional configuration
        ///     to a form field. An example may be layout hints such as the size of the rendered form field or information
        ///     about an icon to prepend or append to the rendered form field.
        /// </returns>
        IDictionary<string, string> Properties { get; }

        /// <returns> true if field is defined as businessKey, false otherwise </returns>
        bool BusinessKey { get; }
    }
}