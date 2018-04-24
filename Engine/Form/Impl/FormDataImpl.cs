using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Form.Impl
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    [Serializable]
    public abstract class FormDataImpl : IFormData
    {
        private const long SerialVersionUid = 1L;
        protected internal string deploymentId;

        protected internal IList<IFormField> formFields = new List<IFormField>();

        protected internal string formKey;
        protected internal IList<IFormProperty> formProperties = new List<IFormProperty>();

        // getters and setters //////////////////////////////////////////////////////

        public virtual string FormKey
        {
            get { return formKey; }
            set { formKey = value; }
        }

        public virtual string DeploymentId
        {
            get { return deploymentId; }
            set { deploymentId = value; }
        }

        public virtual IList<IFormProperty> FormProperties
        {
            get { return formProperties; }
            set { formProperties = value; }
        }


        public virtual IList<IFormField> FormFields
        {
            get { return formFields; }
            set { formFields = value; }
        }
    }
}