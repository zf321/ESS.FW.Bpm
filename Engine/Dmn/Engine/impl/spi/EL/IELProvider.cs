namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el
{
    /// <summary>
    ///     Provider for Java Expression Language (EL) support.
    ///     
    /// </summary>
    public interface IELProvider
    {
        /// <summary>
        ///     Create a new expression
        /// </summary>
        /// <returns> an InternalExpression </returns>
        IELExpression CreateExpression(string expression);
    }
}