 
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    

	/// <summary>
	/// Exception type thrown in build phase (scan/parse).
	/// 
	/// 
	/// </summary>
	public class TreeBuilderException : ELException
	{
		private const long serialVersionUID = 1L;

		private readonly string expression;
		private readonly int position;
		private readonly string encountered;
		private readonly string expected;

		public TreeBuilderException(string expression, int position, string encountered, string expected, string message) 
            : base($"error.build, {expression}, {message}")
		{
			this.expression = expression;
			this.position = position;
			this.encountered = encountered;
			this.expected = expected;
		}

		/// <returns> the expression string </returns>
		public virtual string Expression
		{
			get
			{
				return expression;
			}
		}

		/// <returns> the error position </returns>
		public virtual int Position
		{
			get
			{
				return position;
			}
		}

		/// <returns> the substring (or description) that has been encountered </returns>
		public virtual string Encountered
		{
			get
			{
				return encountered;
			}
		}

		/// <returns> the substring (or description) that was expected </returns>
		public virtual string Expected
		{
			get
			{
				return expected;
			}
		}
	}

}