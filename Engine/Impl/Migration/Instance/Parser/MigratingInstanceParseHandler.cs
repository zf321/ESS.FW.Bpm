namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public interface IMigratingInstanceParseHandler<in T>
    {
        void Handle(MigratingInstanceParseContext parseContext, T element);
    }
}