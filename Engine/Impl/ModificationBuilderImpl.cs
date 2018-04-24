using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Common.Utilities;

namespace ESS.FW.Bpm.Engine.Impl
{
    

    public class ModificationBuilderImpl : IModificationBuilder
    {

        protected internal ICommandExecutor commandExecutor;
        protected internal IQueryable<IProcessInstance> processInstanceQuery;
        protected internal IList<string> processInstanceIds;
        protected internal IList<AbstractProcessInstanceModificationCommand> instructions;
        protected internal string processDefinitionId;

        protected internal bool skipCustomListeners;
        protected internal bool skipIoMappings;

        public ModificationBuilderImpl(ICommandExecutor commandExecutor, string processDefinitionId)
        {
            this.commandExecutor = commandExecutor;
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "processDefinitionId", processDefinitionId);
            this.processDefinitionId = processDefinitionId;
            processInstanceIds = new List<string>();
            instructions = new List<AbstractProcessInstanceModificationCommand>();
        }

        public  IModificationBuilder StartBeforeActivity(string activityId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityId", activityId);
            instructions.Add(new ActivityBeforeInstantiationCmd(activityId));
            return this;
        }

        public  IModificationBuilder StartAfterActivity(string activityId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityId", activityId);
            instructions.Add(new ActivityAfterInstantiationCmd(activityId));
            return this;
        }

        public  IModificationBuilder StartTransition(string transitionId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "transitionId", transitionId);
            instructions.Add(new TransitionInstantiationCmd(transitionId));
            return this;
        }

        public  IModificationBuilder CancelAllForActivity(string activityId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "activityId", activityId);
            instructions.Add(new ActivityCancellationCmd(activityId));
            return this;
        }

        public  IModificationBuilder SetProcessInstanceIds(IList<string> processInstanceIds)
        {
            this.processInstanceIds = processInstanceIds;
            return this;
        }

        public  IModificationBuilder SetProcessInstanceIds(params string[] processInstanceIds)
        {
            if (processInstanceIds == null)
            {
                this.processInstanceIds = new List<string>();
            }
            else
            {
                this.processInstanceIds = (processInstanceIds);
            }
            return this;
        }

        public IModificationBuilder SetProcessInstanceQuery(IQueryable<IProcessInstance> processInstanceQuery)
        {
            this.processInstanceQuery = processInstanceQuery;
            return this;
        }

        public  IModificationBuilder SetSkipCustomListeners()
        {
            this.skipCustomListeners = true;
            return this;
        }

        public  IModificationBuilder SetSkipIoMappings()
        {
            this.skipIoMappings = true;
            return this;
        }

        public virtual void Execute(bool writeUserOperationLog)
        {
            commandExecutor.Execute(new ProcessInstanceModificationCmd(this, writeUserOperationLog));
        }

        public  void Execute()
        {
            Execute(true);
        }

        public  IBatch ExecuteAsync()
        {
            return commandExecutor.Execute(new ProcessInstanceModificationBatchCmd(this));
        }

        public virtual ICommandExecutor CommandExecutor
        {
            get
            {
                return commandExecutor;
            }
        }

        public virtual IQueryable<IProcessInstance> ProcessInstanceQuery
        {
            get
            {
                return processInstanceQuery;
            }
        }

        public virtual IList<string> ProcessInstanceIds
        {
            get
            {
                return processInstanceIds;
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

    }
}
