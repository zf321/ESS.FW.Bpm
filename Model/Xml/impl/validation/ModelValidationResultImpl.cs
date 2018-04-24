

using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.validation;

namespace ESS.FW.Bpm.Model.Xml.impl.validation
{
    public class ModelValidationResultImpl : IValidationResult
    {

        protected internal int code;
        protected internal ValidationResultType type;
        protected internal IModelElementInstance Element;
        protected internal string message;

        public ModelValidationResultImpl(IModelElementInstance element, ValidationResultType type, int code, string message)
        {
            this.Element = element;
            this.type = type;
            this.code = code;
            this.message = message;
        }

        public virtual ValidationResultType Type => type;

        public virtual IModelElementInstance XmlElement => Element;

        public virtual string Message => message;

        public virtual int Code => code;
    }

}