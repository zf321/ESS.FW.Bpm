namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     <para>
    ///         A variable listener can be defined on a scope in a case model.
    ///         Depending on its configuration, it is invoked when a variable is create/updated/deleted
    ///         on a case execution that corresponds to that scope or to any of its descendant scopes.
    ///     </para>
    ///     <para>
    ///         <strong>Beware:</strong> If you set a variable inside a <seealso cref="IVariableListener{T}" /> implementation,
    ///         this will result in new variable listener invocations. Make sure that your implementation
    ///         allows to exit such a cascade as otherwise there will be an <strong>infinite loop</strong>.
    ///     </para>
    ///     
    /// </summary>
    public interface ICaseVariableListener : IVariableListener<IDelegateCaseVariableInstance>
    {
    }

    public static class CaseVariableListenerFields
    {
        public const string Create = VariableListenerFields.Create;
        public const string Update = VariableListenerFields.Update;
        public const string Delete = VariableListenerFields.Delete;
    }
}