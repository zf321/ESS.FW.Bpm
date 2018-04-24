using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope
{
    /// <summary>
    ///     
    /// </summary>
    public interface IVariableInstanceFactory<out T> where T : ICoreVariableInstance
    {
        T Build(string name, ITypedValue value, bool isTransient);
    }
}