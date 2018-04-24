using System;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Create and destroy artifacts of a given class in a container specific way.
    ///     This SPI hides differences between CDI, Spring, etc.
    ///     <br />
    ///     Samples:
    ///     <pre>
    ///         &lt;camunda:taskListener class="org.mypackage.MyListener".../&gt;
    ///         or
    ///         &lt;serviceTask camunda:class=""org.mypackage.MyJavaDelegate".. /&gt;
    ///     </pre>
    ///     The default implementation uses Class.newInstance to create artifacts.
    ///     The CDI specific version utilizes the BeanManager to resolve the
    ///     Contextual Instances.
    /// </summary>
    public interface IArtifactFactory
    {
        /// <param name="clazz"> of the artifact to create </param>
        /// <returns> the instance of the fullyQualifiedClassName </returns>
        T GetArtifact<T>(Type clazz) where T : class;
    }
}