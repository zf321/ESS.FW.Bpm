using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Core.Model;

namespace ESS.FW.Bpm.Engine.Impl.Core.Transformer
{
    /// <summary>
    ///     
    /// </summary>
    public interface ITransform<T> where T : CoreActivity
    {
        IList<T> Transform();
    }
}