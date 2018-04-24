using System;

namespace ESS.FW.Bpm.Engine.Form
{
    /// <summary>
    ///     Represents a single property on a form.
    ///      
    /// </summary>
    [Obsolete]
    public interface IFormProperty
    {
        /// <summary>
        ///     The key used to submit the property in <seealso cref="IFormService#submitStartFormData(String, java.Util.Map)" />
        ///     or <seealso cref="IFormService#submitTaskFormData(String, java.Util.Map)" />
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The display label
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Type of the property.
        /// </summary>
        IFormType Type { get; }

        /// <summary>
        ///     Optional value that should be used to display in this property
        /// </summary>
        string Value { get; }

        /// <summary>
        ///     Is this property read to be displayed in the form and made accessible with the methods
        ///     <seealso cref="IFormService#getStartFormData(String)" /> and
        ///     <seealso cref="IFormService#getTaskFormData(String)" />.
        /// </summary>
        bool Readable { get; }

        /// <summary>
        ///     Is this property expected when a user submits the form?
        /// </summary>
        bool Writable { get; }

        /// <summary>
        ///     Is this property a required input field
        /// </summary>
        bool Required { get; }
    }
}