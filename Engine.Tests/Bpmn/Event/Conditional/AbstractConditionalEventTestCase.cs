using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.builder;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Conditional
{
    public abstract class AbstractConditionalEventTestCase
    {

        protected internal const string ConditionalEventProcessKey = "conditionalEventProcess";
        protected internal const string ConditionalEvent = "conditionalEvent";
        protected internal const string ConditionExpr = "${variable == 1}";
        protected internal static readonly string ExprSetVariable = "${execution.SetVariable(\"variable\", 1)}";
        protected internal static readonly string ExprSetVariableOnParent = "${execution.GetParent().SetVariable(\"variable\", 1)}";
        protected internal const string ConditionalModel = "conditionalModel.bpmn20.xml";
        protected internal const string ConditionalVarEvents = "create, update";
        protected internal const string ConditionalVarEventUpdate = "update";

        protected internal const string TaskBeforeCondition = "Before Condition";
        protected internal const string TaskBeforeConditionId = "beforeConditionId";
        protected internal const string TaskAfterCondition = "After Condition";
        public const string TaskAfterConditionId = "afterConditionId";
        protected internal const string TaskAfterServiceTask = "afterServiceTask";
        protected internal const string TaskInSubProcessId = "taskInSubProcess";
        protected internal const string TaskInSubProcess = "ITask in Subprocess";
        protected internal const string TaskWithCondition = "ITask with condition";
        protected internal const string TaskWithConditionId = "taskWithCondition";
        protected internal const string AfterTask = "After Task";

        protected internal const string VariableName = "variable";
        protected internal const string TrueCondition = "${true}";
        protected internal const string SubProcessId = "subProcess";
        protected internal const string FlowId = "flow";
        protected internal const string DelegatedProcessKey = "delegatedProcess";

        protected internal static readonly IBpmnModelInstance TaskModel = GetUP();
        private static IBpmnModelInstance GetUP()
        {
            var start = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent();
            start.UserTask(TaskBeforeConditionId)
            .Name<UserTaskBuilder>(TaskBeforeCondition);
           return start.EndEvent().Done();
        }
        protected internal static readonly IBpmnModelInstance DelegatedProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(DelegatedProcessKey).StartEvent().ServiceTask().CamundaExpression(ExprSetVariable).EndEvent().Done();
        protected internal const string TaskAfterOutputMapping = "afterOutputMapping";

        protected internal IList<ITask> TasksAfterVariableIsSet;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public final ProcessEngineRule engine = new util.ProvidedProcessEngineRule();
        public readonly ProcessEngineRule Engine = new ProvidedProcessEngineRule();

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException expectException = org.junit.Rules.ExpectedException.None();
        //public ExpectedException expectException = ExpectedException.None();

        protected internal IRuntimeService RuntimeService;
        protected internal ITaskService TaskService;
        protected internal IRepositoryService RepositoryService;
        protected internal ProcessEngineConfigurationImpl ProcessEngineConfiguration;
        protected internal IQueryable<IEventSubscription> ConditionEventSubscriptionQuery;

        [SetUp]
        public virtual void Init()
        {
            this.RuntimeService = Engine.RuntimeService;
            this.TaskService = Engine.TaskService;
            this.RepositoryService = Engine.RepositoryService;
            this.ProcessEngineConfiguration = Engine.ProcessEngineConfiguration;
            //this.ConditionEventSubscriptionQuery = (new IQueryable<IEventSubscription>(ProcessEngineConfiguration.CommandExecutorTxRequired)).eventType(EventType.Conditonal.Name());
        }

        [TearDown]
        public virtual void CheckIfProcessCanBeFinished()
        {
            //given tasks after variable was set
            Assert.NotNull(TasksAfterVariableIsSet);

            //when tasks are completed
            foreach (ITask task in TasksAfterVariableIsSet)
            {
                TaskService.Complete(task.Id);
            }

            //then
            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
            Assert.IsNull(TaskService.CreateTaskQuery().First());
            Assert.IsNull(RuntimeService.CreateProcessInstanceQuery().First());
            TasksAfterVariableIsSet = null;
        }



        public static void AssertTaskNames(IList<ITask> actualTasks, params string[] expectedTaskNames)
        {
            IList<string> expectedNames = new List<string>((expectedTaskNames));
            foreach (ITask task in actualTasks)
            {
                string actualTaskName = task.Name;
                if (expectedNames.Contains(actualTaskName))
                {
                    expectedNames.Remove(actualTaskName);
                }
            }
            Assert.True(expectedNames.Count == 0);
        }

        // conditional event sub process //////////////////////////////////////////////////////////////////////////////////////////

        protected internal virtual void DeployConditionalEventSubProcess(IBpmnModelInstance model, string parentId, bool isInterrupting)
        {
            DeployConditionalEventSubProcess(model, parentId, ConditionExpr, isInterrupting);
        }

        protected internal virtual void DeployConditionalEventSubProcess(IBpmnModelInstance model, string parentId, string conditionExpr, bool isInterrupting)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = addConditionalEventSubProcess(model, parentId, conditionExpr, TASK_AFTER_CONDITION_ID, isInterrupting);
            IBpmnModelInstance modelInstance = AddConditionalEventSubProcess(model, parentId, conditionExpr, TaskAfterConditionId, isInterrupting);
            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());
        }


        protected internal virtual IBpmnModelInstance AddConditionalEventSubProcess(IBpmnModelInstance model, string parentId, string userTaskId, bool isInterrupting)
        {
            return AddConditionalEventSubProcess(model, parentId, ConditionExpr, userTaskId, isInterrupting);
        }

        protected internal virtual IBpmnModelInstance AddConditionalEventSubProcess(IBpmnModelInstance model, string parentId, string conditionExpr, string userTaskId, bool isInterrupting)
        {
            //todo:ESS.FW.Bpm.Engine.Tests.Api.Runtime.Migration.ModifiableBpmnModelInstance.Modify()
            //return ModifiableBpmnModelInstance.Modify(model).AddSubProcessTo(parentId).TriggerByEvent()//.EmbeddedSubProcess().StartEvent().interrupting(isInterrupting).Condition(conditionExpr).UserTask(userTaskId).Name(TaskAfterCondition).EndEvent().Done();
            return null;
        }

        // conditional boundary event //////////////////////////////////////////////////////////////////////////////////////////


        protected internal virtual void DeployConditionalBoundaryEventProcess(IBpmnModelInstance model, string activityId, bool isInterrupting)
        {
            DeployConditionalBoundaryEventProcess(model, activityId, ConditionExpr, isInterrupting);
        }

        protected internal virtual void DeployConditionalBoundaryEventProcess(IBpmnModelInstance model, string activityId, string conditionExpr, bool isInterrupting)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = addConditionalBoundaryEvent(model, activityId, conditionExpr, TASK_AFTER_CONDITION_ID, isInterrupting);
            IBpmnModelInstance modelInstance = AddConditionalBoundaryEvent(model, activityId, conditionExpr, TaskAfterConditionId, isInterrupting);
            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());
        }

        protected internal virtual IBpmnModelInstance AddConditionalBoundaryEvent(IBpmnModelInstance model, string activityId, string userTaskId, bool isInterrupting)
        {
            return AddConditionalBoundaryEvent(model, activityId, ConditionExpr, userTaskId, isInterrupting);
        }

        protected internal virtual IBpmnModelInstance AddConditionalBoundaryEvent(IBpmnModelInstance model, string activityId, string conditionExpr, string userTaskId, bool isInterrupting)
        {
            //todo:ESS.FW.Bpm.Engine.Tests.Api.Runtime.Migration.ModifiableBpmnModelInstance.Modify()
            return null;
            //return ModifiableBpmnModelInstance.Modify(model).ActivityBuilder(activityId).BoundaryEvent().CancelActivity(isInterrupting).Condition(conditionExpr).UserTask(userTaskId).Name(TaskAfterCondition).EndEvent().Done();
        }
    }

}