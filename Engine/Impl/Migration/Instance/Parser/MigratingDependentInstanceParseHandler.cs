namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public interface IMigratingDependentInstanceParseHandler<TS, T>
    {
        void Handle(MigratingInstanceParseContext parseContext, TS owningInstance, T element);
    }
}