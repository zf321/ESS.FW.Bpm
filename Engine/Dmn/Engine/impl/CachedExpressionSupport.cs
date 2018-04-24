using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    /// <summary>
    ///     
    /// </summary>
    public interface CachedExpressionSupport
    {
        IELExpression CachedExpression { set; get; }
    }
}