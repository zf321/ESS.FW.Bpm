using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el;
using ESS.FW.Bpm.Engine.Impl.EL;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.EL
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessEngineElProvider : IELProvider
    {
        protected internal readonly ExpressionManager ExpressionManager;

        public ProcessEngineElProvider(ExpressionManager expressionManager)
        {
            this.ExpressionManager = expressionManager;
        }

        public virtual IELExpression CreateExpression(string expression)
        {
            return new ProcessEngineElExpression(ExpressionManager.CreateValueExpression(expression));
        }
    }
}