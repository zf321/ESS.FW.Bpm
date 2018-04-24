using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.type;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    public class DmnExpressionTransformHelper
    {
        public static IDmnTypeDefinition CreateTypeDefinition(IDmnElementTransformContext context,
            ILiteralExpression expression)
        {
            return CreateTypeDefinition(context, expression.TypeRef);
        }

        public static IDmnTypeDefinition CreateTypeDefinition(IDmnElementTransformContext context,
INformationItem informationItem)
        {
            return CreateTypeDefinition(context, informationItem.TypeRef);
        }

        protected internal static IDmnTypeDefinition CreateTypeDefinition(IDmnElementTransformContext context,
            string typeRef)
        {
            if (!ReferenceEquals(typeRef, null))
            {
                var transformer = context.DataTypeTransformerRegistry.GetTransformer(typeRef);
                return new DmnTypeDefinitionImpl(typeRef, transformer);
            }
            return new DefaultTypeDefinition();
        }

        public static string GetExpressionLanguage(IDmnElementTransformContext context, ILiteralExpression expression)
        {
            return GetExpressionLanguage(context, expression.ExpressionLanguage);
        }

        public static string GetExpressionLanguage(IDmnElementTransformContext context, IUnaryTests expression)
        {
            return GetExpressionLanguage(context, expression.ExpressionLanguage);
        }

        protected internal static string GetExpressionLanguage(IDmnElementTransformContext context,
            string expressionLanguage)
        {
            if (!ReferenceEquals(expressionLanguage, null))
                return expressionLanguage;
            return GetGlobalExpressionLanguage(context);
        }

        protected internal static string GetGlobalExpressionLanguage(IDmnElementTransformContext context)
        {
            var expressionLanguage = context.ModelInstance.Definitions.ExpressionLanguage;
            if (!DefaultDmnEngineConfiguration.FEEL_EXPRESSION_LANGUAGE.Equals(expressionLanguage))
                return expressionLanguage;
            return null;
        }

        public static string GetExpression(ILiteralExpression expression)
        {
            return GetExpression(expression.Text);
        }

        public static string GetExpression(IUnaryTests expression)
        {
            return GetExpression(expression.Text);
        }

        protected internal static string GetExpression(IText text)
        {
            if (text != null)
            {
                var textContent = text.TextContent;
                if (!ReferenceEquals(textContent, null) && (textContent.Length > 0))
                    return textContent;
            }
            return null;
        }
    }
}