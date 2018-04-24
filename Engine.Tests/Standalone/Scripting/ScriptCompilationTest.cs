using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Scripting;
using ESS.FW.Bpm.Engine.Impl.Scripting.Env;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Scripting
{

    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ScriptCompilationTest : PluggableProcessEngineTestCase
    {

        //protected internal const string SCRIPT_LANGUAGE = "groovy";
        //protected internal const string EXAMPLE_SCRIPT = "println 'hello world'";
        protected internal const string SCRIPT_LANGUAGE = "cs";
        protected internal const string EXAMPLE_SCRIPT = @"public class Test{
int Sqr(int a)
                                            {
                                                return a * a;
                                            }}"
;


        protected internal ScriptFactory scriptFactory;
        [SetUp]
        public virtual void SetUp()
        {
            scriptFactory = processEngineConfiguration.ScriptFactory;
        }

        protected internal virtual SourceExecutableScript CreateScript(string language, string source)
        {
            return (SourceExecutableScript)scriptFactory.CreateScriptFromSource(language, source);
        }
        [Test]
        public virtual void testScriptShouldBeCompiledByDefault()
        {
            // when a script is created
            SourceExecutableScript script = CreateScript(SCRIPT_LANGUAGE, EXAMPLE_SCRIPT);
            Assert.NotNull(script);

            // then it should not be compiled on creation
            Assert.True(script.IsShouldBeCompiled());
            Assert.IsNull(script.CompiledScript);

            // but after first execution
            ExecuteScript(script);

            // it was compiled
            Assert.IsFalse(script.IsShouldBeCompiled());
            Assert.NotNull(script.CompiledScript);
        }
        [Test]
        public virtual void testDisableScriptCompilation()
        {
            // when script compilation is disabled and a script is created
            processEngineConfiguration.EnableScriptCompilation = false;
            SourceExecutableScript script = CreateScript(SCRIPT_LANGUAGE, EXAMPLE_SCRIPT);
            Assert.NotNull(script);

            // then it should not be compiled on creation
            Assert.True(script.IsShouldBeCompiled());
            Assert.IsNull(script.CompiledScript);

            // and after first execution
            ExecuteScript(script);

            // it was also not compiled
            Assert.IsFalse(script.IsShouldBeCompiled());
            Assert.IsNull(script.CompiledScript);

            // re-enable script compilation
            processEngineConfiguration.EnableScriptCompilation = true;
        }
        [Test]
        public virtual void testDisableScriptCompilationByDisabledScriptEngineCaching()
        {
            // when script engine caching is disabled and a script is created
            processEngineConfiguration.SetEnableScriptEngineCaching(false);
            SourceExecutableScript script = CreateScript(SCRIPT_LANGUAGE, EXAMPLE_SCRIPT);
            Assert.NotNull(script);

            // then it should not be compiled on creation
            Assert.True(script.IsShouldBeCompiled());
            Assert.IsNull(script.CompiledScript);

            // and after first execution
            ExecuteScript(script);

            // it was also not compiled
            Assert.IsFalse(script.IsShouldBeCompiled());
            Assert.IsNull(script.CompiledScript);

            // re-enable script engine caching
            processEngineConfiguration.SetEnableScriptEngineCaching(true);
        }
        [Test]
        public virtual void testOverrideScriptSource()
        {
            // when a script is created and executed
            SourceExecutableScript script = CreateScript(SCRIPT_LANGUAGE, EXAMPLE_SCRIPT);
            Assert.NotNull(script);
            ExecuteScript(script);

            // it was compiled
            Assert.IsFalse(script.IsShouldBeCompiled());
            Assert.NotNull(script.CompiledScript);

            // if the script source changes
            script.SetScriptSource(EXAMPLE_SCRIPT);

            // then it should not be compiled after change
            Assert.True(script.IsShouldBeCompiled());
            Assert.IsNull(script.CompiledScript);

            // but after next execution
            ExecuteScript(script);

            // it is compiled again
            Assert.IsFalse(script.IsShouldBeCompiled());
            Assert.NotNull(script.CompiledScript);
        }

        protected internal virtual object ExecuteScript(ExecutableScript script)
        {
            ScriptingEnvironment scriptingEnvironment = processEngineConfiguration.ScriptingEnvironment;
            return processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, script, scriptingEnvironment));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly ScriptCompilationTest outerInstance;

            private ExecutableScript script;
            private ScriptingEnvironment scriptingEnvironment;

            public CommandAnonymousInnerClass(ScriptCompilationTest outerInstance, ExecutableScript script, ScriptingEnvironment scriptingEnvironment)
            {
                this.outerInstance = outerInstance;
                this.script = script;
                this.scriptingEnvironment = scriptingEnvironment;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                return scriptingEnvironment.Execute(script, null);
            }
        }

    }

}