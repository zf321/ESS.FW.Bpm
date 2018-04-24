using System;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Error
{
    [Serializable]
    public class BpmnErrorBean
    {

        private const long serialVersionUID = 1L;

        public virtual void throwBpmnError()
        {
            throw new BpmnError("23", "This is a business fault, which can be caught by a BPMN Error Event.");
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void throwBpmnException() throws MyBusinessException
        public virtual void throwBpmnException()
        {
            throw new MyBusinessException("This is a business exception, which can be caught by a BPMN Error Event.");
        }

        public virtual IJavaDelegate Delegate
        {
            get
            {
                return new ThrowBpmnErrorDelegate();
            }
        }
    }
}