 
using System;

namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    public interface ITypeConverter
	{
		/// <summary>
		/// Default conversions as from JSR245.
		/// </summary>

		/// <summary>
		/// Convert the given input value to the specified target type. </summary>
		/// <param name="value"> input value </param>
		/// <param name="type"> target type </param>
		/// <returns> conversion result </returns>
		T Convert<T>(Object value, Type type);
	}

	public static class TypeConverterFields
	{
		public static readonly ITypeConverter Default = new TypeConverterImpl();
	}

}