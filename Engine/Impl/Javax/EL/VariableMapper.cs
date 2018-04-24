

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     The interface to a map between EL variables and the EL expressions they are associated with.
    /// </summary>
    public abstract class VariableMapper
    {
        /// <summary>
        ///     Resolves the specified variable name to a ValueExpression.
        /// </summary>
        /// <param name="variable">
        ///     The variable name
        /// </param>
        /// <returns>
        ///     the ValueExpression assigned to the variable, null if there is no previous assignment
        ///     to this variable.
        /// </returns>
        public abstract ValueExpression ResolveVariable(string variable);

        /// <summary>
        ///     Assign a ValueExpression to an EL variable, replacing any previously assignment to the same
        ///     variable. The assignment for the variable is removed if the expression is null.
        /// </summary>
        /// <param name="variable">
        ///     The variable name
        /// </param>
        /// <param name="expression">
        ///     The ValueExpression to be assigned to the variable.
        /// </param>
        /// <returns>
        ///     The previous ValueExpression assigned to this variable, null if there is no previous
        ///     assignment to this variable.
        /// </returns>
        public abstract ValueExpression SetVariable(string variable, ValueExpression expression);
    }
}

