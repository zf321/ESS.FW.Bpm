using System;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Shell
{

    public class ShellTaskTest : PluggableProcessEngineTestCase
    {

        internal enum OsType
        {
            LINUX,
            WINDOWS,
            MAC,
            SOLARIS,
            UNKOWN
        }

        internal OsType osType;

        internal virtual OsType SystemOsType
        {
            get
            {
                string osName = Environment.OSVersion.Platform.ToString().ToLower();
                    //System.GetProperty("os.Name").ToLower();

                if (osName.IndexOf("win", StringComparison.Ordinal) >= 0)
                {
                    return OsType.WINDOWS;
                }
                else if (osName.IndexOf("mac", StringComparison.Ordinal) >= 0)
                {
                    return OsType.MAC;
                }
                else if ((osName.IndexOf("nix", StringComparison.Ordinal) >= 0) ||
                         (osName.IndexOf("nux", StringComparison.Ordinal) >= 0))
                {
                    return OsType.LINUX;
                }
                else if (osName.IndexOf("sunos", StringComparison.Ordinal) >= 0)
                {
                    return OsType.SOLARIS;
                }
                else
                {
                    return OsType.UNKOWN;
                }
            }
        }

        [SetUp]
        [Deployment]
        protected internal virtual void setUp()
        {
            osType = SystemOsType;
        }


        [Test]
        public virtual void testOsDetection()
        {
            Assert.True(osType != OsType.UNKOWN);
        }

        
        [Deployment]
        public virtual void testEchoShellWindows()
        {
            if (osType == OsType.WINDOWS)
            {

                IProcessInstance pi = runtimeService.StartProcessInstanceByKey("echoShellWindows");

                string st = (string) runtimeService.GetVariable(pi.Id, "resultVar");
                Assert.NotNull(st);
                Assert.True(st.StartsWith("EchoTest", StringComparison.Ordinal));
            }
        }

        
        [Deployment]
        public virtual void testEchoShellLinux()
        {
            if (osType == OsType.LINUX)
            {

                IProcessInstance pi = runtimeService.StartProcessInstanceByKey("echoShellLinux");

                string st = (string) runtimeService.GetVariable(pi.Id, "resultVar");
                Assert.NotNull(st);
                Assert.True(st.StartsWith("EchoTest", StringComparison.Ordinal));
            }
        }

        
        [Deployment]
        public virtual void testEchoShellMac()
        {
            if (osType == OsType.MAC)
            {

                IProcessInstance pi = runtimeService.StartProcessInstanceByKey("echoShellMac");

                string st = (string) runtimeService.GetVariable(pi.Id, "resultVar");
                Assert.NotNull(st);
                Assert.True(st.StartsWith("EchoTest", StringComparison.Ordinal));
            }
        }
    }

}