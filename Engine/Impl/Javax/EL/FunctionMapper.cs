

using System.Reflection;

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     The interface to a map between EL function names and methods. A FunctionMapper maps
    ///     ${prefix:name()} style functions to a static method that can execute that function.
    /// </summary>
    public abstract class FunctionMapper
    {
        /// <summary>
        ///     Resolves the specified prefix and local name into a java.lang.MethodInfo. Returns null if no
        ///     function could be found that matches the given prefix and local name.
        /// </summary>
        /// <param name="prefix">
        ///     the prefix of the function, or "" if no prefix. For example, "fn" in
        ///     ${fn:method()}, or "" in ${method()}.
        /// </param>
        /// <param name="localName">
        ///     the short name of the function. For example, "method" in ${fn:method()}.
        /// </param>
        /// <returns> the static method to invoke, or null if no match was found. </returns>
        public abstract MethodInfo ResolveFunction(string prefix, string localName);
    }
}

