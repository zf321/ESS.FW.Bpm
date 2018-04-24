using System;

namespace Engine.Tests.Bpmn.ServiceTask.Util
{
    [Serializable]
    public class OkReturningService
    {
        public virtual string Invoke()
        {
            return "ok";
        }
    }
}