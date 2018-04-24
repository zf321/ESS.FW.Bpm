using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Juel;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     Class used to get hold of a <seealso cref="ExpressionFactory" />.
    ///     
    /// </summary>
    public abstract class ExpressionFactoryResolver
    {
        public static ExpressionFactory ResolveExpressionFactory()
        {
            // Return instance of custom JUEL implementation
            return new ExpressionFactoryImpl();
        }
    }
}