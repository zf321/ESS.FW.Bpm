using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl
{

    /// <summary>
    /// </summary>
    public class RestartProcessInstanceBuilderImpl : IRestartProcessInstanceBuilder
    {

        protected internal ICommandExecutor commandExecutor;
        protected internal IList<string> processInstanceIds;
        protected internal IList<AbstractProcessInstanceModificationCommand> instructions;
        protected internal string processDefinitionId;
        protected internal IQueryable<IHistoricProcessInstance> query;
        protected internal bool initialVariables;
        protected internal bool skipCustomListeners;
        protected internal bool skipIoMappings;
        protected internal bool withoutBusinessKey;

        public RestartProcessInstanceBuilderImpl(ICommandExecutor commandExecutor, string processDefinitionId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "processDefinitionId", processDefinitionId);
            this.commandExecutor = commandExecutor;
            this.instructions = new List<AbstractProcessInstanceModificationCommand>();
            this.processDefinitionId = processDefinitionId;
            this.processInstanceIds = new List<string>();
        }

        public IRestartProcessInstanceBuilder StartBeforeActivity(string activityId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityId", activityId);
            instructions.Add(new ActivityBeforeInstantiationCmd(null, activityId));
            return this;
        }

        public IRestartProcessInstanceBuilder StartAfterActivity(string activityId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityId", activityId);
            instructions.Add(new ActivityAfterInstantiationCmd(null, activityId));
            return this;
        }

        public IRestartProcessInstanceBuilder StartTransition(string transitionId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityId", transitionId);
            instructions.Add(new TransitionInstantiationCmd(null, transitionId));
            return this;
        }

        public virtual void Execute()
        {
            Execute(true);
        }

        public virtual void Execute(bool writeUserOperationLog)
        {
            commandExecutor.Execute(new RestartProcessInstancesCmd(commandExecutor, this, writeUserOperationLog));
        }

        public virtual IBatch ExecuteAsync()
        {
            return commandExecutor.Execute(new RestartProcessInstancesBatchCmd(commandExecutor, this));
        }

        public virtual IList<AbstractProcessInstanceModificationCommand> Instructions
        {
            get
            {
                return instructions;
            }
            set
            {
                this.instructions = value;
            }
        }

        public virtual IList<string> ProcessInstanceIds
        {
            get
            {
                return processInstanceIds;
            }
        }

        public  IRestartProcessInstanceBuilder SetProcessInstanceIds(params string[] processInstanceIds)
        {
            ((List<string>)this.processInstanceIds).AddRange((processInstanceIds));
            return this;
        }

        public IRestartProcessInstanceBuilder SetHistoricProcessInstanceQuery(IQueryable<IHistoricProcessInstance> query)
        {
            this.query = query;
            return this;
        }

        public  IQueryable<IHistoricProcessInstance> HistoricProcessInstanceQuery
        {
            get
            {
                return query;
            }
        }

        public virtual string ProcessDefinitionId
        {
            get
            {
                return processDefinitionId;
            }
            set
            {
                this.processDefinitionId = value;
            }
        }



        public  IRestartProcessInstanceBuilder SetProcessInstanceIds(IList<string> processInstanceIds)
        {
            ((List<string>)this.processInstanceIds).AddRange(processInstanceIds);
            return this;
        }

        public  IRestartProcessInstanceBuilder InitialSetOfVariables()
        {
            this.initialVariables = true;
            return this;
        }

        public virtual bool InitialVariables
        {
            get
            {
                return initialVariables;
            }
        }

        public  IRestartProcessInstanceBuilder SetSkipCustomListeners()
        {
            this.skipCustomListeners = true;
            return this;
        }

        public  IRestartProcessInstanceBuilder SetSkipIoMappings()
        {
            this.skipIoMappings = true;
            return this;
        }

        public virtual bool SkipCustomListeners
        {
            get
            {
                return skipCustomListeners;
            }
        }

        public virtual bool SkipIoMappings
        {
            get
            {
                return skipIoMappings;
            }
        }

        public  IRestartProcessInstanceBuilder SetWithoutBusinessKey()
        {
            withoutBusinessKey = true;
            return this;
        }

        public virtual bool WithoutBusinessKey
        {
            get
            {
                return withoutBusinessKey;
            }
        }
    }
}
