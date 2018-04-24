namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     
    /// </summary>
    public interface IExpression
    {
        string ExpressionText { get; }

        bool LiteralText { get; }

        // public API for field injection

        object GetValue(IVariableScope variableScope);

        void SetValue(object value, IVariableScope variableScope);
    }
}