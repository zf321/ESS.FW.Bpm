using System;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     Default ArtifactService implementation.
    ///     This version uses Class.newInstance() to create
    ///     new Artifacts.
    ///     This is the default behaviour like has been in old
    ///     camunda/activity versions.
    /// </summary>
    public class DefaultArtifactFactory : IArtifactFactory
    {
        public virtual T GetArtifact<T>(Type clazz) where T : class
        {
            try
            {
                return Activator.CreateInstance(clazz) as T;
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException("couldn't instantiate class " + clazz.FullName, e);
            }
        }
    }
}