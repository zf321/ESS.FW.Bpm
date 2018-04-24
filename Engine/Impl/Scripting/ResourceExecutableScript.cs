using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    public class ResourceExecutableScript : SourceExecutableScript
    {
        protected String scriptResource;

        public ResourceExecutableScript(String language, String scriptResource) : base(language, null)
        {
            this.scriptResource = scriptResource;
        }
        protected override object Evaluate(IScriptEngine engine, IVariableScope variableScope, IBindings bindings)
        {
            if (scriptSource == null)
            {
                LoadScriptSource();
            }
            return base.Evaluate(engine, variableScope, bindings);
        }

        protected void LoadScriptSource()
        {
            if (GetScriptSource() == null)
            {
                DeploymentEntity deployment = Context.CoreExecutionContext.Deployment;//.GetCoreExecutionContext().getDeployment();
                String source = ResourceUtil.LoadResourceContent(scriptResource, deployment);
                SetScriptSource(source);
            }
        }
    }
}
