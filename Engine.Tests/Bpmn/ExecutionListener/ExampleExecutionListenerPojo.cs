using System;

namespace Engine.Tests.Bpmn.ExecutionListener
{

    /// <summary>
    /// Simple pojo than will be used to act as an event listener.
    /// 
    /// 
    /// </summary>
    [Serializable]
    public class ExampleExecutionListenerPojo
    {

        private const long serialVersionUID = 1L;

        private string receivedEventName;

        public virtual void myMethod(string eventName)
        {
            this.receivedEventName = eventName;
        }

        public virtual string ReceivedEventName
        {
            get
            {
                return receivedEventName;
            }
            set
            {
                this.receivedEventName = value;
            }
        }


    }

}