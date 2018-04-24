using System;
using ESS.FW.Common.Components;
using MassTransit;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///     generates <seealso cref="IdBlock" />s that are used to assign ids to new objects.
    ///     The scope of an instance of this class is process engine,
    ///     which means that there is only one instance in one process engine instance.
    ///     
    /// </summary>
    public interface IDGenerator
    {
        string NextId { get; }
        string NewGuid();
    }
    /// <summary>
    /// 简单的Id生成器
    /// </summary>
    [Component]
    public class IDGeneratorGuid:IDGenerator
    {
        string IDGenerator.NextId
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
        
        public string NewGuid()
        {
            return NewId.Next().ToString();
        }
    }
}