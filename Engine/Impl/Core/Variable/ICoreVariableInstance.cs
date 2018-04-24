using System;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable
{
    /// <summary>
    ///     
    /// </summary>
    public interface ICoreVariableInstance
    {
        string Name { get;  set; }
        string TaskId { get; set; }

        void SetValue(ITypedValue value);

        ITypedValue GetTypedValue(bool deserializeValue);

        bool GetIsTransient();

        void Delete();

        void IncrementSequenceCounter();

        bool IsConcurrentLocal { get; set; }
        [NotMapped]
        ExecutionEntity Execution { get; set; }
        [NotMapped]
        TaskEntity Task { get; set; }
    }
}