using System;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel
{

    public class FeelEngineLogger : FeelLogger
    {
        public FeelEngineLogger()
        {
        }

        protected internal virtual FeelSyntaxException SyntaxException(string id, string feelExpression, string description)
        {
            return new FeelSyntaxException(this.SyntaxExceptionMessage(id, feelExpression, description), feelExpression, description);
        }

        protected internal virtual FeelSyntaxException SyntaxException(string id, string feelExpression, string description, System.Exception cause)
        {
            return new FeelSyntaxException(this.SyntaxExceptionMessage(id, feelExpression, description), feelExpression, description, cause);
        }

        protected internal virtual string SyntaxExceptionMessage(string id, string feelExpression, string description)
        {
            return description != null ? this.ExceptionMessage(id, "Syntax error in expression '{}': {}", new object[] { feelExpression, description }) : this.ExceptionMessage(id, "Syntax error in expression '{}'", new object[] { feelExpression });
        }

        public virtual FeelSyntaxException invalidNotExpression(string feelExpression)
        {
            string description = "Expression should have format 'not(...)'";
            return this.SyntaxException("001", feelExpression, description);
        }

        public virtual FeelSyntaxException invalidIntervalExpression(string feelExpression)
        {
            string description = "Expression should have format '[|(|] endpoint .. endpoint ]|)|['";
            return this.SyntaxException("002", feelExpression, description);
        }

        public virtual FeelSyntaxException invalidComparisonExpression(string feelExpression)
        {
            string description = "Expression should have format '<=|<|>=|> endpoint'";
            return this.SyntaxException("003", feelExpression, description);
        }

        public virtual FeelException variableMapperIsReadOnly()
        {
            return new FeelException(this.ExceptionMessage("004", "The variable mapper is read only.", new object[0]));
        }

        public virtual FeelException unableToFindMethod(System.Exception cause, string name, params Type[] parameterTypes)
        {
            return new FeelException(this.ExceptionMessage("005", "Unable to find method '{}' with parameter types '{}'", new object[] { name, parameterTypes }), cause);
        }

        public virtual FeelMissingFunctionException unknownFunction(string prefix, string localName)
        {
            string function = localName;
            if (prefix != null && prefix.Length > 0)
            {
                function = prefix + ":" + localName;
            }

            return new FeelMissingFunctionException(this.ExceptionMessage("006", "Unable to resolve function '{}'", new object[] { function }), function);
        }

        public virtual FeelMissingFunctionException UnknownFunction(string feelExpression, FeelMissingFunctionException cause)
        {
            string function = cause.GetFunction();
            return new FeelMissingFunctionException(this.ExceptionMessage("007", "Unable to resolve function '{}' in expression '{}'", new object[] { function, feelExpression }), function);
        }

        public virtual FeelMissingVariableException unknownVariable(string variable)
        {
            return new FeelMissingVariableException(this.ExceptionMessage("008", "Unable to resolve variable '{}'", new object[] { variable }), variable);
        }

        public virtual FeelMissingVariableException unknownVariable(string feelExpression, FeelMissingVariableException cause)
        {
            string variable = cause.GetVariable();
            return new FeelMissingVariableException(this.ExceptionMessage("009", "Unable to resolve variable '{}' in expression '{}'", new object[] { variable, feelExpression }), variable);
        }

        public virtual FeelSyntaxException invalidExpression(string feelExpression, System.Exception cause)
        {
            return this.SyntaxException("010", feelExpression, (string)null, cause);
        }

        public virtual FeelException unableToInitializeFeelEngine(System.Exception cause)
        {
            return new FeelException(this.ExceptionMessage("011", "Unable to initialize FEEL engine", new object[0]), cause);
        }

        public virtual FeelException unableToEvaluateExpression(string simpleUnaryTests, System.Exception cause)
        {
            return new FeelException(this.ExceptionMessage("012", "Unable to evaluate expression '{}'", new object[] { simpleUnaryTests }), cause);
        }

        public virtual FeelConvertException unableToConvertValue(object value, Type type)
        {
            return new FeelConvertException(this.ExceptionMessage("013", "Unable to convert value '{}' of type '{}' to type '{}'", new object[] { value, value.GetType(), type }), value, type);
        }

        public virtual FeelConvertException unableToConvertValue(object value, Type type, System.Exception cause)
        {
            return new FeelConvertException(this.ExceptionMessage("014", "Unable to convert value '{}' of type '{}' to type '{}'", new object[] { value, value.GetType(), type }), value, type, cause);
        }

        public virtual FeelConvertException unableToConvertValue(string feelExpression, FeelConvertException cause)
        {
            object value = cause.GetValue();
            Type type = cause.GetType();
            return new FeelConvertException(this.ExceptionMessage("015", "Unable to convert value '{}' of type '{}' to type '{}' in expression '{}'", new object[] { value, value.GetType(), type, feelExpression }), cause);
        }

        public virtual System.NotSupportedException simpleExpressionNotSupported()
        {
            return new System.NotSupportedException(this.ExceptionMessage("016", "Simple Expression not supported by FEEL engine", new object[0]));
        }

        public virtual FeelException unableToEvaluateExpressionAsNotInputIsSet(string simpleUnaryTests, FeelMissingVariableException e)
        {
            return new FeelException(this.ExceptionMessage("017", "Unable to evaluate expression '{0}' as no input is set. Maybe the inputExpression is missing or empty.", new object[] { simpleUnaryTests }), e);
        }

        public virtual FeelMethodInvocationException invalidDateAndTimeFormat(string dateTimeString, System.Exception cause)
        {
            return new FeelMethodInvocationException(this.ExceptionMessage("018", "Invalid date and time format in '{0}'", new object[] { dateTimeString }), cause, "date and time", new string[] { dateTimeString });
        }

        public virtual FeelMethodInvocationException unableToInvokeMethod(string simpleUnaryTests, FeelMethodInvocationException cause)
        {
            string method = cause.GetMethod();
            string[] parameters = cause.GetParameters();
            return new FeelMethodInvocationException(this.ExceptionMessage("019", "Unable to invoke method '{0}' with parameters '{1}' in expression '{2}'", new object[] { method, parameters, simpleUnaryTests }), cause.InnerException, method, parameters);
        }

        public virtual FeelSyntaxException invalidListExpression(string feelExpression)
        {
            string description = "List expression can not have empty elements";
            return this.SyntaxException("020", feelExpression, description);
        }
    }
}