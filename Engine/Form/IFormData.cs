using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Form
{
    /// <summary>
    ///     Contains all metadata for displaying a form and serves as
    ///     base interface for <seealso cref="IStartFormData" /> and <seealso cref="ITaskFormData" />
    ///     
    /// </summary>
    public interface IFormData
    {
        /// <summary>
        ///     User-defined reference to a form. In the camunda tasklist application,
        ///     it is assumed that the form key specifies a resource in the deployment
        ///     which is the template for the form.  But users are free to
        ///     use this property differently.
        /// </summary>
        string FormKey { get; }

        /// <summary>
        ///     The deployment id of the process definition to which this form is related
        /// </summary>
        string DeploymentId { get; }

        /// <summary>
        ///     Properties containing the dynamic information that needs to be displayed in the form.
        /// </summary>
        [Obsolete]
        IList<IFormProperty> FormProperties { get; }

        /// <summary>
        ///     returns the form fields which make up this form.
        /// </summary>
        IList<IFormField> FormFields { get; }
    }
}