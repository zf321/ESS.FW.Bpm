using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.tree;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime
{
    /// <summary>
    ///     Callback for being notified when a model instance has started.
    ///     
    /// </summary>
    [System.Serializable]
    public class ProcessInstanceStartContext : ExecutionStartContext
    {
        protected internal InstantiationStack instantiationStack;

        /// <param name="initial"> </param>
        public ProcessInstanceStartContext(ActivityImpl initial)
        {
            this.Initial = initial;
        }

        public ActivityImpl Initial { get; set; }


        public override InstantiationStack InstantiationStack
        {
            get
            {
                if (instantiationStack == null)
                {
                    FlowScopeWalker flowScopeWalker;
                    if (Initial == null)
                        flowScopeWalker = new FlowScopeWalker(null);
                    else
                        flowScopeWalker = new FlowScopeWalker(Initial.FlowScope);
                    var scopeCollector = new ScopeCollector();
                    flowScopeWalker.AddPreVisitor(scopeCollector)
                        .WalkWhile(element => element == null || element == Initial.ProcessDefinition);

                    IList<IPvmActivity> scopeActivities = scopeCollector.Scopes.Cast<IPvmActivity>().ToList();


                    instantiationStack = new InstantiationStack(scopeActivities.Reverse().ToList(), Initial, null);
                }

                return instantiationStack;
            }
        }
        public bool IsAsync => Initial.IsAsyncBefore;
    }
}