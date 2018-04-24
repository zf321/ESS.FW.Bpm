using System;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform
{
    public class EqualTransformer : IFeelToJuelTransformer
    {
        public bool CanTransform(String feelExpression)
        {
            return true;
        }

        public String Transform(IFeelToJuelTransform transform, String feelExpression, String inputName)
        {
            String juelEndpoint = transform.TransformEndpoint(feelExpression, inputName);
            return String.Format("{0} == {1}", inputName, juelEndpoint );
        }
    }
}