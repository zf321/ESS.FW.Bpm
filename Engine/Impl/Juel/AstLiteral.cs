 
namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	public abstract class AstLiteral : AstRightValue
	{
		public sealed override int Cardinality
		{
			get
			{
				return 0;
			}
		}

		public sealed override INode GetChild(int i)
		{
			return null;
		}
	}

}