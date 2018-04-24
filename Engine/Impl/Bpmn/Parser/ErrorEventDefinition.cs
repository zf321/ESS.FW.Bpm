using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    [Serializable]
    public class ErrorEventDefinition
    {
        private const long SerialVersionUid = 1L;

        public static IComparer<ErrorEventDefinition> Comparator = new ComparatorAnonymousInnerClass();

        protected internal readonly string handlerActivityId;
        protected internal string errorCode;
        protected internal string errorCodeVariable;
        protected internal string errorMessageVariable;
        protected internal int? precedence = 0;

        public ErrorEventDefinition(string handlerActivityId)
        {
            this.handlerActivityId = handlerActivityId;
        }

        public virtual string ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }


        public virtual string HandlerActivityId
        {
            get { return handlerActivityId; }
        }

        public virtual int? Precedence
        {
            get
            {
                // handlers with error code take precedence over catchall-handlers
                return precedence + (!ReferenceEquals(errorCode, null) ? 1 : 0);
            }
            set { precedence = value; }
        }

        public virtual string ErrorCodeVariable
        {
            set { errorCodeVariable = value; }
            get { return errorCodeVariable; }
        }


        public virtual string ErrorMessageVariable
        {
            set { errorMessageVariable = value; }
            get { return errorMessageVariable; }
        }


        public virtual bool CatchesError(string errorCode)
        {
            return ReferenceEquals(this.errorCode, null) || this.errorCode.Equals(errorCode);
        }

        public virtual bool CatchesException(System.Exception ex)
        {
            // no errorCode: catch any error
            if (ReferenceEquals(errorCode, null))
                return true;
            // unbox exception
            // while ((ex is ProcessEngineException || ex is ScriptException) && ex.InnerException != null)
            // {
            //ex = (Exception) ex.InnerException;
            // }

            // check exception hierarchy
            var exceptionClass = ex.GetType();
            do
            {
                if (errorCode.Equals(exceptionClass.FullName))
                    return true;
                exceptionClass = exceptionClass.BaseType;
            } while (exceptionClass != null);

            return false;
        }

        private class ComparatorAnonymousInnerClass : IComparer<ErrorEventDefinition>
        {
            public int Compare(ErrorEventDefinition o1, ErrorEventDefinition o2)
            {
                return o2.Precedence.Value.CompareTo(o1.Precedence);
            }
        }
    }
}