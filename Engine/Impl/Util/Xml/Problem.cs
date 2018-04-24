using System;

namespace ESS.FW.Bpm.Engine.Impl.Util.xml
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    public class Problem
    {
        protected internal int Column;

        protected internal string ErrorMessage;
        protected internal int Line;
        protected internal string Resource;

        //public Problem(SAXParseException e, string resource)
        //{
        //    concatenateErrorMessages(e);
        //    this.resource = resource;
        //    line = e.LineNumber;
        //    column = e.ColumnNumber;
        //}

        public Problem(string errorMessage, string resourceName, Element element)
        {
            this.ErrorMessage = errorMessage;
            Resource = resourceName;
            if (element != null)
            {
                Line = element.Line;
                Column = element.Column;
            }
        }

        public Problem(BpmnParseException exception, string resourceName)
        {
            ConcatenateErrorMessages(exception);
            Resource = resourceName;
            var element = exception.Element;
            if (element != null)
            {
                Line = element.Line;
                Column = element.Column;
            }
        }

        protected internal virtual void ConcatenateErrorMessages(System.Exception throwable)
        {
            while (throwable != null)
            {
                if (ReferenceEquals(ErrorMessage, null))
                    ErrorMessage = throwable.Message;
                else
                    ErrorMessage += ": " + throwable.Message;
                throwable = throwable.InnerException;
            }
        }

        public override string ToString()
        {
            return ErrorMessage + (!ReferenceEquals(Resource, null) ? " | " + Resource : "") + " | line " + Line +
                   " | column " + Column;
        }
    }
}