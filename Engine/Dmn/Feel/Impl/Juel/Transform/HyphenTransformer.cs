namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform
{
    public class HyphenTransformer : IFeelToJuelTransformer
    {
        public HyphenTransformer()
        {
        }

        public bool CanTransform(string feelExpression)
        {
            return feelExpression.Equals("-");
        }

        public string Transform(IFeelToJuelTransform transform, string feelExpression, string inputName)
        {
            return "true";
        }
    }
}