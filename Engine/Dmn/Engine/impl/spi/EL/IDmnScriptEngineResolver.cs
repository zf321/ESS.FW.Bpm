using ESS.FW.Bpm.Engine.Impl.Javax.Script;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el
{
    /// <summary>
    ///     Resolver for available script engines.
    /// </summary>
    public interface IDmnScriptEngineResolver
    {
        /// <summary>
        ///     Get a script engine by script engine language.
        /// </summary>
        /// <param name="language"> the language of the script engine </param>
        /// <returns> the script engine or null if no script engine for this language exists </returns>
        IScriptEngine GetScriptEngineForLanguage(string language);
    }
}