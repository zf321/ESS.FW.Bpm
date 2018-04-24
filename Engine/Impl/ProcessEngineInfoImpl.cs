using System;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class ProcessEngineInfoImpl : IProcessEngineInfo
    {
        private const long SerialVersionUid = 1L;

        public ProcessEngineInfoImpl(string name, string resourceUrl, string exception)
        {
            this.Name = name;
            this.ResourceUrl = resourceUrl;
            this.Exception = exception;
        }

        public virtual string Name { get; }

        public virtual string ResourceUrl { get; }

        public virtual string Exception { get; }
    }
}