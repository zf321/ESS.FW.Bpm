using System;

namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     Used to annotate a user-provided <seealso cref="AbstractProcessApplication" /> class and specify
    ///     the unique name of the process application.
    ///     
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ProcessApplicationAttribute : Attribute
    {
        internal static string DefaultMetaInfProcessesXml = "META-INF/processes.xml";

        /// <summary>
        ///     Returns the location(s) of the <code>processes.xml</code> deployment descriptors.
        ///     The default value is<code>{META-INF/processes.xml}</code>. The provided path(s)
        ///     must be resolvable through the <seealso cref="ClassLoader#getResourceAsStream(String)" />-MethodInfo
        ///     of the classloader returned  by the
        ///     <seealso cref="AbstractProcessApplication#getProcessApplicationClassloader()" />
        ///     method provided by the process application.
        /// </summary>
        /// <returns> the location of the <code>processes.xml</code> file. </returns>
        internal string[] DeploymentDescriptors =
        {
            "META-INF/processes.xml"
        };

        /// <summary>
        ///     Allows specifying the name of the process application.
        ///     Only applies if the {@code value} property is not set.
        /// </summary>
        internal string Name = "";

        /// <summary>
        ///     Allows specifying the name of the process application.
        ///     Overrides the {@code name} property.
        /// </summary>
        internal string Value = "";
    }
}