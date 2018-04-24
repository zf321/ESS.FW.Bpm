using System;

namespace Engine.Tests.Bpmn.ServiceTask.Util
{
    [Serializable]
    public class ToUppercaseMethod
	{
        
        public string Invoke(string s)
        {
            return s.ToUpper();
        }
    }

}